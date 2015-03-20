# tRSS
Download torrent files from RSS Feeds to a watched folder in your torrent client with this application.

* Portable
* Smart filter
* Visible Regex pattern
* Periodic updates

![Application window](https://github.com/backlof/tRSS/blob/master/Media/Screenshot.png?raw=true)

## How to get a match

* Filter is `Active`
* Feed is selected for the filter
* The beginning of the torrent title matches `Filter` (see below)
* Torrent title contains all terms in `Include` (see below)
* Torrent title contains no terms in `Exclude` (see below)

Also, you can

* `Ignore caps` affecting `Filter`, `Include` and `Exclude`
* `Match once` to deactivate filter after download (unless `TV Show`)

For TV shows

* Torrent episode is equal or higher (only supports format `S{00}E{00}`)
* With `Match once`, each episode will only download once

### Filter
#### Symbols

|Symbol	|Meaning											|Regex		|
|:------:|--------------------------------------|:---------:|
|`*`		|Wildcard; zero or more characters.		|`.*`       |
|`.`		|1 whitespace character 					|`[\s._-]`	|
|`?`		|0 or 1 character of any type.			|`.?`     	|

#### Examples


|Filter				|Torrent								|Match	|
|-----------------|-----------------------------|:------:|
|`TV Show`			|`Bob's TV Show S01E02 720P`	|No		|
|`*TV Show`			|`Bob's TV Show S01E02 720P`	|Yes		|
|`Bob?s TV Show`	|`Bob's TV Show S01E02 720P`	|Yes		|
|`Bob?s TV Show`	|`Bobs TV Show S01E02 720P` 	|Yes		|
|`Bob's.TV.Show`	|`Bob's TV Show S01E02 720P`	|Yes		|
|`Bob's.TV.Show`	|`Bob's.TV.Show.S01E02.720P`	|Yes		|
|`Bob's.TV.Show`	|`Bob's_TV_Show_S01E02_720P`	|Yes		|

### Include and Exclude

- Both are optional
- Separate each term by `;` (empty ones are discarded)
- Use `Ignore caps` if cases vary

#### Examples

|Include			|Exclude	|Torrent											|Match	|
|--------------|--------|--------------------------------------|:------:|
|` `				|`720p`	|`TV Show S01E07 1080p WEBRip DD5.1 `	|Yes		|
|` `				|`1080p`	|`TV Show S01E07 1080p WEBRip DD5.1 `	|No		|
|`1080p;DD5.1`	|` `		|`TV Show S01E07 1080p WEBRip DD5.1 `	|Yes		|
|`WEB;`			|`1080p`	|`TV Show S01E07 1080p WEBRip DD5.1 `	|No		|
|`HDTV;1080p`	|`720p`	|`TV Show S01E07 1080p WEBRip DD5.1 `	|No		|

## How to update

Before you update to a new version, you should take a backup of your filters and feeds

- Click `File` / `Backup`
- Close application
- Extract the .zip containing update
- Copy files into your excisting application (overwrite)
- Run application

If the application crashes

- Delete `Data` folder
- Run application
- Click `File` / `Restore`

This will restore your feed and filters
