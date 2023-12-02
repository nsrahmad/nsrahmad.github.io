---
title: "Advent of Code 2023 Using Pharo Smalltalk Day 1"
date: 2023-12-01T22:57:52+05:30
---

I will do this year's Advent of Code challeges in Pharo Smalltalk. Let's see
how far I can go with it.

## Day 1 - Part 1
Here is the sample input for the first part of the puzzle:

```
1abc2
pqr3stu8vwx
a1b2c3d4e5f
treb7uchet
```
Our task here is to extract the first digit and the last digit to form a single
two digit number. For the four sample lines, these numbers will be 12, 38, 15,
and 77. The answer to the puzzle is the sum of all these numbers, 142 in this
case.

well, Its time to start our pharo 11 vm.

![The playground in Pharo Smalltalk](/img/day-1-playground.png)

Let's get Petit Parser into our image from
[Github](https://github.com/kursjan/petitparser2). Enter the following in the
playground and "Do it" :

```smalltalk
Metacello new
    baseline: 'PetitParser2';
    repository: 'github://kursjan/petitparser2';
    load.
```
This will install the Petit parser 2. I spent some time playing with the
[documentaion](https://kursjan.github.io/petitparser2/) to learn the basics. The
first step is trying to parse all digits:

```smalltalk
| input parser  parseResult |

input := '1abc2
pqr3stu8vwx
a1b2c3d4e5f
treb7uchet'.

parser := #digit asParser.

parseResult := parser matchesIn: input.
```
which recognises all digits in the give input:

![The digits in input](/img/day-1-first-digits.png)

Update our `parser` to also recognise new line:

```smalltalk
parser := #digit asParser / #space asParser.
```
![Also recognise End of Line](/img/day-1-eol.png)

There is probably a better and faster way of doing this with Petit Parser
itself, but let's stop using it and try the collections API to do the rest.

First, Separate digits by line.

```smalltalk
parseResult splitOn: Character cr.
```
and we have a collection of digits per line of input.

![digits per line](/img/day-1-lines.png)

I made a mistake here of not assigning the result of `splitOn` to the variable
and spent few minutes wondering about it. It is very easy to step through and
fix the mistake.

To make a single two digit number, first concatenate the first and last
characters in each line (pharo uses `,` to join strings) and then cast
it as a number:

```smalltalk
asNumbers := parseResult collect:
	[ :each | ((each first asString), (each last asString)) asNumber ].
```

![a single two digit number](/img/day-1-single-number.png)

`inject: into:` reduces/folds `asNumbers` to the sum of all elements.

```smalltalk
asNumbers inject: 0 into: [ :sum :each | sum + each ].
```

![finally the result](/img/day-1-reduce.png)

Looks good. Now download the `input.txt` into the same directory where the
pharo image is for simple lookup, because that is our current working directory
in pharo. And replace the sample input with actual input.

```smalltalk
input := 'input.txt' asFileReference readStreamDo:
	[ :stream | stream upToEnd  ].
```
this required few additional tweaks, like the line ending of the file was
`Charactel lf`, and there was an extra empty collection at the end of file. But
since we can always see the result, it was pretty easy to fix.

![well the actual final result](/img/day-1-part-1-final.png)

This concludes part 1.

## Day 1 - Part 2

The sample input for second part is :

```code
two1nine
eightwothree
abcone2threexyz
xtwone3four
4nineeightseven2
zoneight234
7pqrstsixteen
```
The end goal is still the same, but numbers in words like `one`,`two`,`three`
etc. are also counted as single digits. Accepted results for the input are 29,
83, 13, 24, 42, 14, and 76. Adding these together produces 281.

Trying to recognise words from `one` to `nine`:
```smalltalk
wordDigit := 'one' asParser
	/ 'two' asParser
	/ 'three' asParser
	/ 'four' asParser
	/ 'five' asParser
	/ 'six' asParser
	/ 'seven' asParser
	/ 'eight' asParser
	/ 'nine' asParser .

wordDigit matchesIn: input.
```
This gives:
![parsing the words](/img/day-1-part2-1.png)

One interesting case is `eightwothree`. should we treat it as `eight`, `wo`,
`three` or `eight`, `two`, `three`. Since the result is knows to be 83, lets go
with the first choice here. Instead of using `matchesIn:` , we will use
`matchesSkipIn:`, which doesn't recognise partially overlapping words.
![skip the partial words](/img/day-1-part2-2.png)

we can just combine this parser with the digit and space parsers from part 1:
```smalltalk
parser := (wordDigit / #digit asParser) / #space asParser.
parser matchesSkipIn: input.
```
![the whole parser](/img/day-1-part2-3.png)

If we could replace the words with corresponding characters, the rest will be
the same as part 1. Petit parser makes it really easy. To replace `two` with
`$2`, we can just transform it as:
```smalltalk
'two` asParser ==> [:str | $2]
```
![replace `two` with `2`](/img/day-1-part2-4.png)

The block takes `two` as parameter. we ignore it simply return `$2`
(character literal). We shouldn't try to convert it to a number just yet. Since
this works, do this to all words.

```smalltalk
wordDigit := ('one' asParser ==> [ :str | $1 ])
	/ ('two' asParser ==> [ :str | $2 ])
	/ ('three' asParser ==> [ :str | $3 ])
	/ ('four' asParser ==> [ :str | $4 ])
	/ ('five' asParser ==> [ :str | $5 ])
	/ ('six' asParser ==> [ :str | $6 ])
	/ ('seven' asParser ==> [ :str | $7 ])
	/ ('eight' asParser ==> [ :str | $8 ])
	/ ('nine' asParser ==> [:str | $9]).
```
![replace all words](/img/day-1-part2-5.png)

And rest of the problem is same as part 1.

![rest of the problem](/img/day-1-part2-6.png)

`281` is the expected answer. So trying this with the actual `input.txt`:
![the final answer](/img/day-1-part2-7.png)

The answer is rejected as incorrect. I spent going over everything and can't
find anything wrong at all. Finally I turned to Reddit, and the top post at AOC
subreddit mentioned the partial case, that `eightwothree` should be treated as
`eight`, `two` and `three`. Felt an oversight, but on second thought, something
happening in real life all the time. All we have to do is use `matchesIn:`
instead of `matchesSkipIn:`.

![lol the acutal final answer](/img/day-1-part2-8.png)

And the answer is accepted. We are done for the day. See you tomorrow.
