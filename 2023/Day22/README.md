original source: [https://adventofcode.com/2023/day/22](https://adventofcode.com/2023/day/22)
## --- Day 22: Sand Slabs ---
Enough sand has fallen; it can finally filter water for Snow Island.

Well, <em>almost</em>.

The sand has been falling as large compacted <em>bricks</em> of sand, piling up to form an impressive stack here near the edge of Island Island. In order to make use of the sand to filter water, some of the bricks will need to be broken apart - nay, <em>disintegrated</em> - back into freely flowing sand.

The stack is tall enough that you'll have to be careful about choosing which bricks to disintegrate; if you disintegrate the wrong brick, large portions of the stack could topple, which sounds pretty dangerous.

The Elves responsible for water filtering operations took a <em>snapshot of the bricks while they were still falling</em> (your puzzle input) which should let you work out which bricks are safe to disintegrate. For example:

<pre>
<code>1,0,1~1,2,1
0,0,2~2,0,2
0,2,3~2,2,3
0,0,4~0,2,4
2,0,5~2,2,5
0,1,6~2,1,6
1,1,8~1,1,9
</code>
</pre>

Each line of text in the snapshot represents the position of a single brick at the time the snapshot was taken. The position is given as two <code>x,y,z</code> coordinates - one for each end of the brick - separated by a tilde (<code>~</code>). Each brick is made up of a single straight line of cubes, and the Elves were even careful to choose a time for the snapshot that had all of the free-falling bricks at <em>integer positions above the ground</em>, so the whole snapshot is aligned to a three-dimensional cube grid.

A line like <code>2,2,2~2,2,2</code> means that both ends of the brick are at the same coordinate - in other words, that the brick is a single cube.

Lines like <code>0,0,10~1,0,10</code> or <code>0,0,10~0,1,10</code> both represent bricks that are <em>two cubes</em> in volume, both oriented horizontally. The first brick extends in the <code>x</code> direction, while the second brick extends in the <code>y</code> direction.

A line like <code>0,0,1~0,0,10</code> represents a <em>ten-cube brick</em> which is oriented <em>vertically</em>. One end of the brick is the cube located at <code>0,0,1</code>, while the other end of the brick is located directly above it at <code>0,0,10</code>.

The ground is at <code>z=0</code> and is perfectly flat; the lowest <code>z</code> value a brick can have is therefore <code>1</code>. So, <code>5,5,1~5,6,1</code> and <code>0,2,1~0,2,5</code> are both resting on the ground, but <code>3,3,2~3,3,3</code> was above the ground at the time of the snapshot.

Because the snapshot was taken while the bricks were still falling, some bricks will <em>still be in the air</em>; you'll need to start by figuring out where they will end up. Bricks are magically stabilized, so they <em>never rotate</em>, even in weird situations like where a long horizontal brick is only supported on one end. Two bricks cannot occupy the same position, so a falling brick will come to rest upon the first other brick it encounters.

Here is the same example again, this time with each brick given a letter so it can be marked in diagrams:

<pre>
<code>1,0,1~1,2,1   <- A
0,0,2~2,0,2   <- B
0,2,3~2,2,3   <- C
0,0,4~0,2,4   <- D
2,0,5~2,2,5   <- E
0,1,6~2,1,6   <- F
1,1,8~1,1,9   <- G
</code>
</pre>

At the time of the snapshot, from the side so the <code>x</code> axis goes left to right, these bricks are arranged like this:

<pre>
<code> x
012
.G. 9
.G. 8
... 7
FFF 6
..E 5 z
D.. 4
CCC 3
BBB 2
.A. 1
--- 0
</code>
</pre>

Rotating the perspective 90 degrees so the <code>y</code> axis now goes left to right, the same bricks are arranged like this:

<pre>
<code> y
012
.G. 9
.G. 8
... 7
.F. 6
EEE 5 z
DDD 4
..C 3
B.. 2
AAA 1
--- 0
</code>
</pre>

Once all of the bricks fall downward as far as they can go, the stack looks like this, where <code>?</code> means bricks are hidden behind other bricks at that location:

<pre>
<code> x
012
.G. 6
.G. 5
FFF 4
D.E 3 z
??? 2
.A. 1
--- 0
</code>
</pre>

Again from the side:

<pre>
<code> y
012
.G. 6
.G. 5
.F. 4
??? 3 z
B.C 2
AAA 1
--- 0
</code>
</pre>

Now that all of the bricks have settled, it becomes easier to tell which bricks are supporting which other bricks:


 - Brick <code>A</code> is the only brick supporting bricks <code>B</code> and <code>C</code>.
 - Brick <code>B</code> is one of two bricks supporting brick <code>D</code> and brick <code>E</code>.
 - Brick <code>C</code> is the other brick supporting brick <code>D</code> and brick <code>E</code>.
 - Brick <code>D</code> supports brick <code>F</code>.
 - Brick <code>E</code> also supports brick <code>F</code>.
 - Brick <code>F</code> supports brick <code>G</code>.
 - Brick <code>G</code> isn't supporting any bricks.

Your first task is to figure out <em>which bricks are safe to disintegrate</em>. A brick can be safely disintegrated if, after removing it, <em>no other bricks</em> would fall further directly downward. Don't actually disintegrate any bricks - just determine what would happen if, for each brick, only that brick were disintegrated. Bricks can be disintegrated even if they're completely surrounded by other bricks; you can squeeze between bricks if you need to.

In this example, the bricks can be disintegrated as follows:


 - Brick <code>A</code> cannot be disintegrated safely; if it were disintegrated, bricks <code>B</code> and <code>C</code> would both fall.
 - Brick <code>B</code> <em>can</em> be disintegrated; the bricks above it (<code>D</code> and <code>E</code>) would still be supported by brick <code>C</code>.
 - Brick <code>C</code> <em>can</em> be disintegrated; the bricks above it (<code>D</code> and <code>E</code>) would still be supported by brick <code>B</code>.
 - Brick <code>D</code> <em>can</em> be disintegrated; the brick above it (<code>F</code>) would still be supported by brick <code>E</code>.
 - Brick <code>E</code> <em>can</em> be disintegrated; the brick above it (<code>F</code>) would still be supported by brick <code>D</code>.
 - Brick <code>F</code> cannot be disintegrated; the brick above it (<code>G</code>) would fall.
 - Brick <code>G</code> <em>can</em> be disintegrated; it does not support any other bricks.

So, in this example, <code><em>5</em></code> bricks can be safely disintegrated.

Figure how the blocks will settle based on the snapshot. Once they've settled, consider disintegrating a single brick; <em>how many bricks could be safely chosen as the one to get disintegrated?</em>


## --- Part Two ---
Disintegrating bricks one at a time isn't going to be fast enough. While it might sound dangerous, what you really need is a <em>chain reaction</em>.

You'll need to figure out the best brick to disintegrate. For each brick, determine how many <em>other bricks would fall</em> if that brick were disintegrated.

Using the same example as above:


 - Disintegrating brick <code>A</code> would cause all <code><em>6</em></code> other bricks to fall.
 - Disintegrating brick <code>F</code> would cause only <code><em>1</em></code> other brick, <code>G</code>, to fall.

Disintegrating any other brick would cause <em>no other bricks</em> to fall. So, in this example, the sum of <em>the number of other bricks that would fall</em> as a result of disintegrating each brick is <code><em>7</em></code>.

For each brick, determine how many <em>other bricks</em> would fall if that brick were disintegrated. <em>What is the sum of the number of other bricks that would fall?</em>


