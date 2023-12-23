original source: [https://adventofcode.com/2023/day/23](https://adventofcode.com/2023/day/23)
## --- Day 23: A Long Walk ---
The Elves resume water filtering operations! Clean water starts flowing over the edge of Island Island.

They offer to help <em>you</em> go over the edge of Island Island, too! Just hold on tight to one end of this impossibly long rope and they'll lower you down a safe distance from the massive waterfall you just created.

As you finally reach Snow Island, you see that the water isn't really reaching the ground: it's being <em>absorbed by the air</em> itself. It looks like you'll finally have a little downtime while the moisture builds up to snow-producing levels. Snow Island is pretty scenic, even without any snow; why not take a walk?

There's a map of nearby hiking trails (your puzzle input) that indicates <em>paths</em> (<code>.</code>), <em>forest</em> (<code>#</code>), and steep <em>slopes</em> (<code>^</code>, <code>></code>, <code>v</code>, and <code><</code>).

For example:

<pre>
<code>#.#####################
#.......#########...###
#######.#########.#.###
###.....#.>.>.###.#.###
###v#####.#v#.###.#.###
###.>...#.#.#.....#...#
###v###.#.#.#########.#
###...#.#.#.......#...#
#####.#.#.#######.#.###
#.....#.#.#.......#...#
#.#####.#.#.#########v#
#.#...#...#...###...>.#
#.#.#v#######v###.###v#
#...#.>.#...>.>.#.###.#
#####v#.#.###v#.#.###.#
#.....#...#...#.#.#...#
#.#########.###.#.#.###
#...###...#...#...#.###
###.###.#.###v#####v###
#...#...#.#.>.>.#.>.###
#.###.###.#.###.#.#v###
#.....###...###...#...#
#####################.#
</code>
</pre>

You're currently on the single path tile in the top row; your goal is to reach the single path tile in the bottom row. Because of all the mist from the waterfall, the slopes are probably quite <em>icy</em>; if you step onto a slope tile, your next step must be <em>downhill</em> (in the direction the arrow is pointing). To make sure you have the most scenic hike possible, <em>never step onto the same tile twice</em>. What is the longest hike you can take?

In the example above, the longest hike you can take is marked with <code>O</code>, and your starting position is marked <code>S</code>:

<pre>
<code>#S#####################
#OOOOOOO#########...###
#######O#########.#.###
###OOOOO#OOO>.###.#.###
###O#####O#O#.###.#.###
###OOOOO#O#O#.....#...#
###v###O#O#O#########.#
###...#O#O#OOOOOOO#...#
#####.#O#O#######O#.###
#.....#O#O#OOOOOOO#...#
#.#####O#O#O#########v#
#.#...#OOO#OOO###OOOOO#
#.#.#v#######O###O###O#
#...#.>.#...>OOO#O###O#
#####v#.#.###v#O#O###O#
#.....#...#...#O#O#OOO#
#.#########.###O#O#O###
#...###...#...#OOO#O###
###.###.#.###v#####O###
#...#...#.#.>.>.#.>O###
#.###.###.#.###.#.#O###
#.....###...###...#OOO#
#####################O#
</code>
</pre>

This hike contains <code><em>94</em></code> steps. (The other possible hikes you could have taken were <code>90</code>, <code>86</code>, <code>82</code>, <code>82</code>, and <code>74</code> steps long.)

Find the longest hike you can take through the hiking trails listed on your map. <em>How many steps long is the longest hike?</em>


## --- Part Two ---
As you reach the trailhead, you realize that the ground isn't as slippery as you expected; you'll have <em>no problem</em> climbing up the steep slopes.

Now, treat all <em>slopes</em> as if they were normal <em>paths</em> (<code>.</code>). You still want to make sure you have the most scenic hike possible, so continue to ensure that you <em>never step onto the same tile twice</em>. What is the longest hike you can take?

In the example above, this increases the longest hike to <code><em>154</em></code> steps:

<pre>
<code>#S#####################
#OOOOOOO#########OOO###
#######O#########O#O###
###OOOOO#.>OOO###O#O###
###O#####.#O#O###O#O###
###O>...#.#O#OOOOO#OOO#
###O###.#.#O#########O#
###OOO#.#.#OOOOOOO#OOO#
#####O#.#.#######O#O###
#OOOOO#.#.#OOOOOOO#OOO#
#O#####.#.#O#########O#
#O#OOO#...#OOO###...>O#
#O#O#O#######O###.###O#
#OOO#O>.#...>O>.#.###O#
#####O#.#.###O#.#.###O#
#OOOOO#...#OOO#.#.#OOO#
#O#########O###.#.#O###
#OOO###OOO#OOO#...#O###
###O###O#O###O#####O###
#OOO#OOO#O#OOO>.#.>O###
#O###O###O#O###.#.#O###
#OOOOO###OOO###...#OOO#
#####################O#
</code>
</pre>

Find the longest hike you can take through the surprisingly dry hiking trails listed on your map. <em>How many steps long is the longest hike?</em>


