# Torrent PassKey Replacer
Accidentally leaked your torrent passkey? Replace all of your passkeys in the torrents in a snap!

## Command-line Usage

```bash
    --input     DIR            The directory containing files that need to be patched.
    --prev-key  PASSKEY        The previously invalidated passkey.
    --new-key   PASSKEY        The newly acquired passkey.
    --dry-run                  Enables dry-run; no files will be saved/overwritten.
    --verbose                  Enables verbose output.
```

## Details
There are several things to note when using this tool:
   * REMEMBER TO BACKUP YOUR FILES. 
      - All of your files specified by `--input` will have the passkey replaced. In event that the tool screws up your torrent list, you can still recover it.
   * This tool has only been tested with qBittorrent.
      - If you are using qBitTorrent, the target directory should be `%localappdata%\qBittorrent\BT_Backup`.
      - If you are using a different client and have had success using this tool, please report by opening an issue or a pull request on this README.
   * Both passkeys need to be the exact length. Chances are, it will be.