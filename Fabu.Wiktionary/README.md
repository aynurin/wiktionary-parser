# Overview

This project is a part of the larger solution codenamed Fabu. The larger solution is a
mobile app that enhances user's vocabulary.

This project is a parser of a Wiktionary dump to provide a source of word definitions.

# Part in overall solution

This parser has to parse Wiktionary dump, extract separate words, and add them to the search index. This search index will be
used by mobile apps to provide content to users.

# Problem definition

Wiktionary is semi-structured. We can separate different writings between each other (because different writings are 
located at different URLs), but we cannot easily separate different terms (meanings) from each other in case they are written 
in the same way.

There is a series of blog posts devoted to solving this problem:
* [getting around wiktionary](https://shv.svbtle.com/getting-around-wiktionary)
* [optimizing wiktionary section names](https://shv.svbtle.com/wiktionary-sections-flat)
* [structured unstructured wiktionary](https://shv.svbtle.com/structured-unstructured-wiktionary)
* [extracting structure from wiktionary](https://shv.svbtle.com/extracting-structure-from-wiktionary)

The next problem is that wikitext is very rich in it's [templates](https://en.wiktionary.org/wiki/Wiktionary:Templates). 
And there is no seasoned C# parser or HTML converter for Wikitext.

So the first goal is to convert wikitext to the most basic HTML. We want this HTML to be parseable in the future to allow 
implementing custom renderers on mobile devices in the future (for performance and resource consumption on mobile devices).

# Project structure

* `Commands`: The main executable is implemented using [CommandLineParser](https://github.com/commandlineparser/commandline)'s verbs. 
Every verb is implemented as a separate command class.
* `FuzzySearch`: Utilities to analyze Wiktionary section names. Mostly to fix typos and normalize section names.
* `Graph`: Classes to build a graph from Wiktionary sections. This was necessary only to analyze sections structure and normalize section names.
* `TermProcessing`: Classes to extract terms from the dump.
* `TextConverters`: Classes to convert wikitext to HTML.
* `Transform`: Different section names normalization implementations.

# Workflow

0. Run tests in `Fabu.Wiktionary.Tests`: `dotnet test`
1. See default dump dir value in `Fabu.Wiktionary.Commands.BaseArgs.WiktionaryDumpFile`
2. Download and extract a [fresh wiktionary dump](https://dumps.wikimedia.org/enwiktionary/)
3. Extract all section names: `dotnet run -- prep --in enwiktionary-20180120-pages-articles.xml`
4. See what happened: `%DUMP_DIR%\sections.json`, `%DUMP_DIR%\languages.json`
5. Normalize section names: `dotnet run -- sectionsdict`
6. Add statistics to section names: `dotnet run -- graph --in enwiktionary-20180120-pages-articles.xml`
7. See what happened: `%DUMP_DIR%\sections_dict.json`
8. WIP: Extract terms: `dotnet run -- extract --in enwiktionary-20180120-pages-articles.xml`
9. See what happened: `%DUMP_DIR%\templates.json`, `%DUMP_DIR%\nodes.json` - these are all the nodes and templates we need to address to make magic happen.

# Challenges

0. When parsing Wiktionary, you have no means to tell if you did something right or wrong. It is very difficult to test, and sometimes there is just 
no correct solution. So you need to be creative in how to test your work and make sure not to overcomplicate (maybe I did overcomplicate the 
section names normalization, but now it looks nice to me now).
1. Parsing Wikitext is EXTREMELY EXTREMELY SLOW. With no wikitext parsing I get 30k pages per second, with parsing I get up to 500 pages per second.
All wikitext parsers are based on negligent use of Regular Expressions. I will live with that because I don't need to parse the entire dump 
all the time, instead I will be careful in what needs to be parsed while debugging, and in prod we don't need very fast updates of the search index.
2. Parsing Wikitext is very tedious. Very many templates and tags to address, and in many cases you can't just convert them to HTML 
(e.g. pronunciation audio files?..)