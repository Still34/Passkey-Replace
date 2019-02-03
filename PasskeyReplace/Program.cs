using System;
using System.IO;
using System.Text;
using Serilog;

namespace PasskeyReplace
{
    internal class Program
    {
        public static void Main(DirectoryInfo input = default, string prevKey = null, string newKey = null,
            bool verbose = false, bool dryRun = false)
        {
            // Setup logger
            var loggerConfig = new LoggerConfiguration();
            if (verbose) loggerConfig = loggerConfig.MinimumLevel.Verbose();
            Log.Logger = loggerConfig.WriteTo.Console().CreateLogger();

            if (input == default(DirectoryInfo))
                ErrorAndExit("You must specify a directory.");
            if (prevKey == null || newKey == null)
                ErrorAndExit("You must specify a before and after key.");
            if (prevKey.Length != newKey.Length)
                ErrorAndExit($"The keys must have the same length. Old key is ({prevKey.Length}), and new key is ({newKey.Length}).");

            // Get files and prep for operation
            var files = input.GetFiles();
            Log.Information($"{files.Length} files found under {input.FullName}.");
            foreach (var file in files) ChangePasskey(file, prevKey, newKey, dryRun);

            Log.Information("Finished. Press any key to exit...");
            Console.ReadKey();
        }

        public static void ChangePasskey(FileInfo inputFile, string prevKey, string newKey, bool dryRun)
        {
            if (inputFile.Length > 1024000)
            {
                Log.Warning("File {inputFile} is greater than 1MB, skipping...", inputFile.FullName);
                return;
            }

            using (var fs = new FileStream(inputFile.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                Span<byte> span = stackalloc byte[(int)fs.Length];
                fs.Read(span);
                var oldKeySpan = GetBytes(prevKey);
                var newKeySpan = GetBytes(newKey);
                var index = span.IndexOf(oldKeySpan);
                if (index == -1)
                {
                    Log.Warning("File {inputFile} does not contain reference to {prevKey}, skipping...", inputFile,
                        prevKey);
                    return;
                }

                while (index != -1)
                {
                    Log.Verbose("Replacing reference at offset {index} for file {inputFile}", index, inputFile);
                    for (var i = 0; i < oldKeySpan.Length; i++)
                    {
                        Log.Verbose("Replacing offset {offset} with {newKeySpanChar}", index+ i, newKeySpan[i]);
                        span[index + i] = newKeySpan[i];
                    }

                    index = span.IndexOf(oldKeySpan);
                }

                Log.Verbose("Finished replacing offsets with new key value, saving...");
                if (!dryRun)
                {
                    fs.Position = 0;
                    fs.Write(span);
                }
                Log.Information("Saved file {inputFile}", inputFile);
            }
        }

        public static void ErrorAndExit(string message)
        {
            Log.Error(message);
            Environment.Exit(0);
        }

        public static Span<byte> GetBytes(string str)
        {
            var byteBuffer = new Span<byte>(new byte[str.Length]);
            Encoding.UTF8.GetBytes(str.AsSpan(), byteBuffer);
            return byteBuffer;
        }
    }
}