# tRSS

* Portable
* Smart filter
* Visible Regex pattern
* Periodic updates
* Forced updates

![Application window](https://github.com/backlof/tRSS/blob/master/Media/Screenshot.png?raw=true)

##	Filters
In order to get a match:

* Filter must be active (top checkbox)
* Feed must be selected
* The beginning of your title must match `Filter` (Wildcards are optional)

| Symbol  | Meaning                             | Regex     |
|:-------:|-------------------------------------|:---------:|
|`*`      | Wildcard; zero or more characters.  |`.*`       |
|`.`		  | 1 whitespace character			      	|`[\s._-]`	|
|`?`		  | 0 or 1 character of any type.		  	|`.?`     	|

* Title must include all terms in `Include`, separated by `;`
* Title must not include any terms in `Exclude`, separated by `;`
* Episode must be equal or higher if you've checked `TV Show`

**Note**

* You can ignore caps in `Filter`, `Include` and `Exclude`
* `Match once` will deactivate a filter after download, unless `TV Show`
* Each episode will only download once with `Match once`
