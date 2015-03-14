# tRSS
Download torrent files from RSS Feeds to a watched folder in your torrent client with this application.

* Portable
* Smart filter
* Visible Regex pattern
* Periodic updates

![Application window](https://github.com/backlof/tRSS/blob/master/Media/Screenshot.png?raw=true)

##	Filters


How to get a match

* Filter is `Active`
* Feed is selected for the filter
* The beginning of the torrent title matches `Filter`
* Torrent title contains all terms in `Include` (separated by `;`)
* Torrent contains no terms in `Exclude` (separated by `;`)

For TV shows

* Torrent episode is equal or higher
* With `Match once` episodes will only download once

Also, you can

* `Ignore caps` in `Filter`, `Include` and `Exclude`
* `Match once` to deactivate filter after download

### Filter
#### Symbols

|Symbol	|Meaning											|Regex		|
|:------:|--------------------------------------|:---------:|
|`*`		|Wildcard; zero or more characters.		|`.*`       |
|`.`		|1 whitespace characters					|`[\s._-]`	|
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
