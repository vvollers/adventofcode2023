original source: [https://adventofcode.com/2023/day/15](https://adventofcode.com/2023/day/15)
## --- Day 15: Lens Library ---
The newly-focused parabolic reflector dish is sending all of the collected light to a point on the side of yet another mountain - the largest mountain on Lava Island. As you approach the mountain, you find that the light is being collected by the wall of a large facility embedded in the mountainside.

You find a door under a large sign that says "Lava Production Facility" and next to a smaller sign that says "Danger - Personal Protective Equipment required beyond this point".

As you step inside, you are immediately greeted by a somewhat panicked reindeer wearing goggles and a loose-fitting [hard hat](https://en.wikipedia.org/wiki/Hard_hat). The reindeer leads you to a shelf of goggles and hard hats (you quickly find some that fit) and then further into the facility. At one point, you pass a button with a faint snout mark and the label "PUSH FOR HELP". No wonder you were loaded into that [trebuchet](1) so quickly!

You pass through a final set of doors surrounded with even more warning signs and into what must be the room that collects all of the light from outside. As you admire the large assortment of lenses available to further focus the light, the reindeer brings you a book titled "Initialization Manual".

"Hello!", the book cheerfully begins, apparently unaware of the concerned reindeer reading over your shoulder. "This procedure will let you bring the Lava Production Facility online - all without burning or melting anything unintended!"

"Before you begin, please be prepared to use the Holiday ASCII String Helper algorithm (appendix 1A)." You turn to appendix 1A. The reindeer leans closer with interest.

The HASH algorithm is a way to turn any [string](https://en.wikipedia.org/wiki/String_(computer_science)) of characters into a single <em>number</em> in the range 0 to 255. To run the HASH algorithm on a string, start with a <em>current value</em> of <code>0</code>. Then, for each character in the string starting from the beginning:


 - Determine the [ASCII code](https://en.wikipedia.org/wiki/ASCII#Printable_characters) for the current character of the string.
 - Increase the <em>current value</em> by the ASCII code you just determined.
 - Set the <em>current value</em> to itself multiplied by <code>17</code>.
 - Set the <em>current value</em> to the [remainder](https://en.wikipedia.org/wiki/Modulo) of dividing itself by <code>256</code>.

After following these steps for each character in the string in order, the <em>current value</em> is the output of the HASH algorithm.

So, to find the result of running the HASH algorithm on the string <code>HASH</code>:


 - The <em>current value</em> starts at <code>0</code>.
 - The first character is <code>H</code>; its ASCII code is <code>72</code>.
 - The <em>current value</em> increases to <code>72</code>.
 - The <em>current value</em> is multiplied by <code>17</code> to become <code>1224</code>.
 - The <em>current value</em> becomes <code><em>200</em></code> (the remainder of <code>1224</code> divided by <code>256</code>).
 - The next character is <code>A</code>; its ASCII code is <code>65</code>.
 - The <em>current value</em> increases to <code>265</code>.
 - The <em>current value</em> is multiplied by <code>17</code> to become <code>4505</code>.
 - The <em>current value</em> becomes <code><em>153</em></code> (the remainder of <code>4505</code> divided by <code>256</code>).
 - The next character is <code>S</code>; its ASCII code is <code>83</code>.
 - The <em>current value</em> increases to <code>236</code>.
 - The <em>current value</em> is multiplied by <code>17</code> to become <code>4012</code>.
 - The <em>current value</em> becomes <code><em>172</em></code> (the remainder of <code>4012</code> divided by <code>256</code>).
 - The next character is <code>H</code>; its ASCII code is <code>72</code>.
 - The <em>current value</em> increases to <code>244</code>.
 - The <em>current value</em> is multiplied by <code>17</code> to become <code>4148</code>.
 - The <em>current value</em> becomes <code><em>52</em></code> (the remainder of <code>4148</code> divided by <code>256</code>).

So, the result of running the HASH algorithm on the string <code>HASH</code> is <code><em>52</em></code>.

The <em>initialization sequence</em> (your puzzle input) is a comma-separated list of steps to start the Lava Production Facility. <em>Ignore newline characters</em> when parsing the initialization sequence. To verify that your HASH algorithm is working, the book offers the sum of the result of running the HASH algorithm on each step in the initialization sequence.

For example:

<pre>
<code>rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7</code>
</pre>

This initialization sequence specifies 11 individual steps; the result of running the HASH algorithm on each of the steps is as follows:


 - <code>rn=1</code> becomes <code><em>30</em></code>.
 - <code>cm-</code> becomes <code><em>253</em></code>.
 - <code>qp=3</code> becomes <code><em>97</em></code>.
 - <code>cm=2</code> becomes <code><em>47</em></code>.
 - <code>qp-</code> becomes <code><em>14</em></code>.
 - <code>pc=4</code> becomes <code><em>180</em></code>.
 - <code>ot=9</code> becomes <code><em>9</em></code>.
 - <code>ab=5</code> becomes <code><em>197</em></code>.
 - <code>pc-</code> becomes <code><em>48</em></code>.
 - <code>pc=6</code> becomes <code><em>214</em></code>.
 - <code>ot=7</code> becomes <code><em>231</em></code>.

In this example, the sum of these results is <code><em>1320</em></code>. Unfortunately, the reindeer has stolen the page containing the expected verification number and is currently running around the facility with it excitedly.

Run the HASH algorithm on each step in the initialization sequence. <em>What is the sum of the results?</em> (The initialization sequence is one long line; be careful when copy-pasting it.)


## --- Part Two ---
You convince the reindeer to bring you the page; the page confirms that your HASH algorithm is working.

The book goes on to describe a series of 256 <em>boxes</em> numbered <code>0</code> through <code>255</code>. The boxes are arranged in a line starting from the point where light enters the facility. The boxes have holes that allow light to pass from one box to the next all the way down the line.

<pre>
<code>      +-----+  +-----+         +-----+
Light | Box |  | Box |   ...   | Box |
----------------------------------------->
      |  0  |  |  1  |   ...   | 255 |
      +-----+  +-----+         +-----+
</code>
</pre>

Inside each box, there are several <em>lens slots</em> that will keep a lens correctly positioned to focus light passing through the box. The side of each box has a panel that opens to allow you to insert or remove lenses as necessary.

Along the wall running parallel to the boxes is a large library containing lenses organized by <em>focal length</em> ranging from <code>1</code> through <code>9</code>. The reindeer also brings you a small handheld [label printer](https://en.wikipedia.org/wiki/Label_printer).

The book goes on to explain how to perform each step in the initialization sequence, a process it calls the Holiday ASCII String Helper Manual Arrangement Procedure, or <em>HASHMAP</em> for short.

Each step begins with a sequence of letters that indicate the <em>label</em> of the lens on which the step operates. The result of running the HASH algorithm on the label indicates the correct box for that step.

The label will be immediately followed by a character that indicates the <em>operation</em> to perform: either an equals sign (<code>=</code>) or a dash (<code>-</code>).

If the operation character is a <em>dash</em> (<code>-</code>), go to the relevant box and remove the lens with the given label if it is present in the box. Then, move any remaining lenses as far forward in the box as they can go without changing their order, filling any space made by removing the indicated lens. (If no lens in that box has the given label, nothing happens.)

If the operation character is an <em>equals sign</em> (<code>=</code>), it will be followed by a number indicating the <em>focal length</em> of the lens that needs to go into the relevant box; be sure to use the label maker to mark the lens with the label given in the beginning of the step so you can find it later. There are two possible situations:


 - If there is already a lens in the box with the same label, <em>replace the old lens</em> with the new lens: remove the old lens and put the new lens in its place, not moving any other lenses in the box.
 - If there is <em>not</em> already a lens in the box with the same label, add the lens to the box immediately behind any lenses already in the box. Don't move any of the other lenses when you do this. If there aren't any lenses in the box, the new lens goes all the way to the front of the box.

Here is the contents of every box after each step in the example initialization sequence above:

<pre>
<code>After "rn=1":
Box 0: [rn 1]

After "cm-":
Box 0: [rn 1]

After "qp=3":
Box 0: [rn 1]
Box 1: [qp 3]

After "cm=2":
Box 0: [rn 1] [cm 2]
Box 1: [qp 3]

After "qp-":
Box 0: [rn 1] [cm 2]

After "pc=4":
Box 0: [rn 1] [cm 2]
Box 3: [pc 4]

After "ot=9":
Box 0: [rn 1] [cm 2]
Box 3: [pc 4] [ot 9]

After "ab=5":
Box 0: [rn 1] [cm 2]
Box 3: [pc 4] [ot 9] [ab 5]

After "pc-":
Box 0: [rn 1] [cm 2]
Box 3: [ot 9] [ab 5]

After "pc=6":
Box 0: [rn 1] [cm 2]
Box 3: [ot 9] [ab 5] [pc 6]

After "ot=7":
Box 0: [rn 1] [cm 2]
Box 3: [ot 7] [ab 5] [pc 6]
</code>
</pre>

All 256 boxes are always present; only the boxes that contain any lenses are shown here. Within each box, lenses are listed from front to back; each lens is shown as its label and focal length in square brackets.

To confirm that all of the lenses are installed correctly, add up the <em>focusing power</em> of all of the lenses. The focusing power of a single lens is the result of multiplying together:


 - One plus the box number of the lens in question.
 - The slot number of the lens within the box: <code>1</code> for the first lens, <code>2</code> for the second lens, and so on.
 - The focal length of the lens.

At the end of the above example, the focusing power of each lens is as follows:


 - <code>rn</code>: <code>1</code> (box 0) * <code>1</code> (first slot) * <code>1</code> (focal length) = <code><em>1</em></code>
 - <code>cm</code>: <code>1</code> (box 0) * <code>2</code> (second slot) * <code>2</code> (focal length) = <code><em>4</em></code>
 - <code>ot</code>: <code>4</code> (box 3) * <code>1</code> (first slot) * <code>7</code> (focal length) = <code><em>28</em></code>
 - <code>ab</code>: <code>4</code> (box 3) * <code>2</code> (second slot) * <code>5</code> (focal length) = <code><em>40</em></code>
 - <code>pc</code>: <code>4</code> (box 3) * <code>3</code> (third slot) * <code>6</code> (focal length) = <code><em>72</em></code>

So, the above example ends up with a total focusing power of <code><em>145</em></code>.

With the help of an over-enthusiastic reindeer in a hard hat, follow the initialization sequence. <em>What is the focusing power of the resulting lens configuration?</em>


