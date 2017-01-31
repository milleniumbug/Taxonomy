# Taxonomy

[![License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](LICENSE) [![Gitter](https://badges.gitter.im/taxonomy-net/Lobby.svg)](https://gitter.im/taxonomy-net/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
======

This project allows to tag arbitrary files, by storing the metadata non-intrusively, that is, not inside the files, not using the filesystem capabilities, but in a separate file which you can transfer together with your collection. 

This, of course, has the disadvantage of needing additional synchronization in case the files are moved outside of the program's knowledge, but allows for it being independent of file system limitations, allowing to transfer your collection to a pendrive or a phone.

For many collections, which are read more often than modified, this is an acceptable trade-off.

Structure
=======

This project is separated into a library `TaxonomyLib` which the API to the tag mappings, and multiple frontends:

- `TaxonomyWpf`, the WPF GUI
- `TaxonomyCli`, the console application (planned)
- `TaxonomyMobile`, the mobile app in Xamarin.Forms (work in progress, currently only Android supported)

The common and portable parts are separated into the `Common` project