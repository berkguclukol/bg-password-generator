# Privacy

_Last updated: November 4, 2026_

BG Password Generator is a small offline tool. It has no accounts, no telemetry, no analytics and no ads, and the developer receives no data from it.

## Passwords and UUIDs

- Passwords and UUIDs are generated on your device with the operating system's cryptographically secure random number generator.
- They are never saved to disk, written to logs or sent over the network. They exist only in the app's memory and are gone when it closes.

## Text you hash or encode

- Text you enter on the **Hash / Base64** page is processed on your device and, like the results, is never saved, logged or sent over the network. It's gone when the app closes.
- Spell check and text prediction are turned off for that box.

## Clipboard

- A copied **password, hash or Base64 result** is kept out of Windows clipboard history (Win+V) and out of cloud clipboard sync between your devices. It is removed from the clipboard after 30 seconds or when the app closes, unless you've copied something else since. While it's on the clipboard, other programs on your computer can read it, as with anything you copy.
- A copied **UUID** is treated like any other copied text: it can appear in clipboard history, it syncs if you've turned on cloud clipboard, and it stays on the clipboard after the app closes.

## Update check

This is the only time the app uses the network.

- Installed copies (Setup or Portable) check for a new version when the app starts and when you select **Check for updates** on the About page.
- The app contacts GitHub only: it asks `api.github.com` for this repository's latest releases and, if an update exists and you choose to install it, downloads the update files from GitHub's download servers.
- These requests contain no information about you or your passwords. They don't include an ID, your app version or your system details; the only identifying header is a User-Agent naming the update library (Velopack) and its version.
- Like any web request, GitHub can see your IP address and the request itself. GitHub's handling of that data is covered by the [GitHub General Privacy Statement](https://docs.github.com/en/site-policy/privacy-policies/github-general-privacy-statement).
- Nothing is downloaded or installed unless you select **Update and restart**. If you're offline the check fails silently and the app works normally.

## Files on your device

- The app's files and downloaded update packages live in its install folder (`%LocalAppData%\BgPasswordGenerator` for Setup, or wherever you extracted the Portable zip).
- The updater creates a random install ID in `packages\.betaId` inside that folder. It's used by Velopack's own hosted update service, which this app doesn't use, so it never leaves your device.
- The updater keeps a log of install and update activity at `%LocalAppData%\velopack\velopack_BgPasswordGenerator.log`. It contains version numbers and file paths (which can include your Windows user name) but never passwords or UUIDs.
- Setup adds Desktop and Start Menu shortcuts and an entry in Apps and features. Uninstalling removes the install folder, the shortcuts and that entry. The updater log above stays behind; you can delete it by hand. For the Portable version, deleting its folder removes everything except that log.

## Questions

Open an issue at [github.com/berkguclukol/bg-password-generator/issues](https://github.com/berkguclukol/bg-password-generator/issues).
