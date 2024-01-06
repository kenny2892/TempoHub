# TempoHub
This is a work in progress C# WPF personal project that I started 11/25/2023 and had a version 1.0 deadline of 12/25/2023 (as you can probably guess, this was a spontaneous Christmas gift). It is a custom audio file player and editor similar to iTunes. The main focus is to add more options for filtering and tagging. I also aim to ensure all data is kept on the actual audio file instead of in an external database. With the only database (a SQLite file) being used to keep track of which audio files should be loaded into the application, handle custom playlists, and keeping a play history.

<p align="center">
  <img src="https://i.gyazo.com/47fe45d6e39d7d99618f7c74b7c25246.png" width="800">
</p>

## Current Features
- Load in mp3, m4a, and wav audio files either individually or by folder.
- Play audio files added into the application.
- Queue up songs to be played.
- Import and export song data.
- Ability to copy and paste data between songs.
- Customize the applications color scheme along with various other options within the Settings menu.
- View your files in 4 different layouts:
  -  Albums: Groups together songs with matching Album names and matching Artists/Album Artists.
    <p align="center">
      <img src="https://i.gyazo.com/ea6d3844a84678f2e59f7e5cfc0d5643.png" width="800">
    </p>
    
  -  Artists: Groups together songs with matching Artists/Album Artists.
    <p align="center">
      <img src="https://i.gyazo.com/11979295a9820e793cd1aa7d07216d1e.png" width="800">
    </p>
    
  -  Songs: Lists out each song individualy and displays the details of just that one song.
    <p align="center">
      <img src="https://i.gyazo.com/03c9f7c685f049ba20b7fceeb92b6226.png" width="800">
    </p>
    
  -  Song Details: Lists out all songs in a more grid like fashion allowing you to sort and filter by the various song tags.
    <p align="center">
      <img src="https://i.gyazo.com/bc1747ce034a78b1113858f3a93a5167.png" width="800">
    </p>
    
- An editor to modify many different tags for each individual file or for a group of files.
<p align="center">
  <img src="https://i.gyazo.com/90de2a4412a8437c6315bc1ace2b0da7.png" width="400">
</p>

- Can add custom album images along with searching for album covers via the MusicBrainz API.
<p align="center">
  <img src="https://i.gyazo.com/5e12f6349fb7952f3295de55dc322690.png" width="400">
</p>

- Search for tags using the MusicBrainz API
<p align="center">
  <img src="https://i.gyazo.com/faee0f925a2717c7c178e6e21a7b333f.png" width="600">
</p>

- Listen to custom playlists, automatically made playlists that group by artist or genre, or your most recently listened to songs.
<p align="center">
  <img src="https://i.gyazo.com/039d973d86b8e36a1e61e4e78207542f.png" width="600">
</p>

## Credits
- Styles using [Mahapps](https://mahapps.com/)
- Album covers and tag information found using [MusicBrainz API](https://musicbrainz.org/)
- MusicBrainz API access using [MetaBrainz.MusicBrainz](https://github.com/Zastai/MetaBrainz.MusicBrainz) and [MetaBrainz.MusicBrainz.CoverArt](https://github.com/Zastai/MetaBrainz.MusicBrainz.CoverArt)
- Song Length found using [NAudio](https://github.com/naudio/NAudio)
- Folder Selection Dialog using [Ookii.Dialogs.Wpf](https://github.com/ookii-dialogs/ookii-dialogs-wpf)
