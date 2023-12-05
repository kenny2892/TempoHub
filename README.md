# TempoHub
This is a work in progress C# WPF personal project that I've been working on for a little over a week now. It is a custom audio file player and editor similar to iTunes. The main focus is to add more options for filtering and tagging. I also aim to ensure all data is kept on the actual audio file instead of in an external database. With the only database (a SQLite file) being the one to track which audio files should be loaded into the software.

<p align="center">
  <img src="https://i.gyazo.com/4816322f3f9c98340b8817ceaffc7ba1.png" width="600">
</p>

## Current Features
- Can load in mp3, m4a, and wav audio files.
- Currently, the program will group all songs by Album. Will be implementing other viewing layouts.
- Can play all audio files.
- Can create a queue of audio files.
- Can edit various common tags for each individual file.
<p align="center">
  <img src="https://i.gyazo.com/6067230fcff35fde38471ff3b97fd108.png" width="600">
</p>
- Can add custom album images along with searching for album covers via the MusicBrainz API.
<p align="center">
  <img src="https://i.gyazo.com/89697969e5d38abbb70ebf23eba39dd7.png" width="600">
</p>

## Credits
- Styles using [Mahapps](https://mahapps.com/)
- Album covers found using [MusicBrainz API](https://musicbrainz.org/)
- MusicBrainz API access using [MetaBrainz.MusicBrainz](https://github.com/Zastai/MetaBrainz.MusicBrainz) and [MetaBrainz.MusicBrainz.CoverArt](https://github.com/Zastai/MetaBrainz.MusicBrainz.CoverArt)
