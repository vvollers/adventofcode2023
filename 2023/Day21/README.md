original source: [https://adventofcode.com/2023/day/21](https://adventofcode.com/2023/day/21)
## --- Day 21: Step Counter ---
You manage to catch the [airship](7) right as it's dropping someone else off on their all-expenses-paid trip to Desert Island! It even helpfully drops you off near the [gardener](5) and his massive farm.

"You got the sand flowing again! Great work! Now we just need to wait until we have enough sand to filter the water for Snow Island and we'll have snow again in no time."

While you wait, one of the Elves that works with the gardener heard how good you are at solving problems and would like your help. He needs to get his [steps](https://en.wikipedia.org/wiki/Pedometer) in for the day, and so he'd like to know <em>which garden plots he can reach with exactly his remaining <code>64</code> steps</em>.

He gives you an up-to-date map (your puzzle input) of his starting position (<code>S</code>), garden plots (<code>.</code>), and rocks (<code>#</code>). For example:

<pre>
<code>...........
.....###.#.
.###.##..#.
..#.#...#..
....#.#....
.##..S####.
.##..#...#.
.......##..
.##.#.####.
.##..##.##.
...........
</code>
</pre>

The Elf starts at the starting position (<code>S</code>) which also counts as a garden plot. Then, he can take one step north, south, east, or west, but only onto tiles that are garden plots. This would allow him to reach any of the tiles marked <code>O</code>:

<pre>
<code>...........
.....###.#.
.###.##..#.
..#.#...#..
....#O#....
.##.OS####.
.##..#...#.
.......##..
.##.#.####.
.##..##.##.
...........
</code>
</pre>

Then, he takes a second step. Since at this point he could be at <em>either</em> tile marked <code>O</code>, his second step would allow him to reach any garden plot that is one step north, south, east, or west of <em>any</em> tile that he could have reached after the first step:

<pre>
<code>...........
.....###.#.
.###.##..#.
..#.#O..#..
....#.#....
.##O.O####.
.##.O#...#.
.......##..
.##.#.####.
.##..##.##.
...........
</code>
</pre>

After two steps, he could be at any of the tiles marked <code>O</code> above, including the starting position (either by going north-then-south or by going west-then-east).

A single third step leads to even more possibilities:

<pre>
<code>...........
.....###.#.
.###.##..#.
..#.#.O.#..
...O#O#....
.##.OS####.
.##O.#...#.
....O..##..
.##.#.####.
.##..##.##.
...........
</code>
</pre>

He will continue like this until his steps for the day have been exhausted. After a total of <code>6</code> steps, he could reach any of the garden plots marked <code>O</code>:

<pre>
<code>...........
.....###.#.
.###.##.O#.
.O#O#O.O#..
O.O.#.#.O..
.##O.O####.
.##.O#O..#.
.O.O.O.##..
.##.#.####.
.##O.##.##.
...........
</code>
</pre>

In this example, if the Elf's goal was to get exactly <code>6</code> more steps today, he could use them to reach any of <code><em>16</em></code> garden plots.

However, the Elf <em>actually needs to get <code>64</code> steps today</em>, and the map he's handed you is much larger than the example map.

Starting from the garden plot marked <code>S</code> on your map, <em>how many garden plots could the Elf reach in exactly <code>64</code> steps?</em>


## --- Part Two ---
The Elf seems confused by your answer until he realizes his mistake: he was reading from a list of his favorite numbers that are both perfect squares and perfect cubes, not his step counter.

The <em>actual</em> number of steps he needs to get today is exactly <code><em>26501365</em></code>.

He also points out that the garden plots and rocks are set up so that the map <em>repeats infinitely</em> in every direction.

So, if you were to look one additional map-width or map-height out from the edge of the example map above, you would find that it keeps repeating:

<pre>
<code>.................................
.....###.#......###.#......###.#.
.###.##..#..###.##..#..###.##..#.
..#.#...#....#.#...#....#.#...#..
....#.#........#.#........#.#....
.##...####..##...####..##...####.
.##..#...#..##..#...#..##..#...#.
.......##.........##.........##..
.##.#.####..##.#.####..##.#.####.
.##..##.##..##..##.##..##..##.##.
.................................
.................................
.....###.#......###.#......###.#.
.###.##..#..###.##..#..###.##..#.
..#.#...#....#.#...#....#.#...#..
....#.#........#.#........#.#....
.##...####..##..S####..##...####.
.##..#...#..##..#...#..##..#...#.
.......##.........##.........##..
.##.#.####..##.#.####..##.#.####.
.##..##.##..##..##.##..##..##.##.
.................................
.................................
.....###.#......###.#......###.#.
.###.##..#..###.##..#..###.##..#.
..#.#...#....#.#...#....#.#...#..
....#.#........#.#........#.#....
.##...####..##...####..##...####.
.##..#...#..##..#...#..##..#...#.
.......##.........##.........##..
.##.#.####..##.#.####..##.#.####.
.##..##.##..##..##.##..##..##.##.
.................................
</code>
</pre>

This is just a tiny three-map-by-three-map slice of the inexplicably-infinite farm layout; garden plots and rocks repeat as far as you can see. The Elf still starts on the one middle tile marked <code>S</code>, though - every other repeated <code>S</code> is replaced with a normal garden plot (<code>.</code>).

Here are the number of reachable garden plots in this new infinite version of the example map for different numbers of steps:


 - In exactly <code>6</code> steps, he can still reach <code><em>16</em></code> garden plots.
 - In exactly <code>10</code> steps, he can reach any of <code><em>50</em></code> garden plots.
 - In exactly <code>50</code> steps, he can reach <code><em>1594</em></code> garden plots.
 - In exactly <code>100</code> steps, he can reach <code><em>6536</em></code> garden plots.
 - In exactly <code>500</code> steps, he can reach <code><em>167004</em></code> garden plots.
 - In exactly <code>1000</code> steps, he can reach <code><em>668697</em></code> garden plots.
 - In exactly <code>5000</code> steps, he can reach <code><em>16733044</em></code> garden plots.

However, the step count the Elf needs is much larger! Starting from the garden plot marked <code>S</code> on your infinite map, <em>how many garden plots could the Elf reach in exactly <code>26501365</code> steps?</em>


