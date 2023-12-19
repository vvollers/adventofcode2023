original source: [https://adventofcode.com/2023/day/19](https://adventofcode.com/2023/day/19)
## --- Day 19: Aplenty ---
The Elves of Gear Island are thankful for your help and send you on your way. They even have a hang glider that someone [stole](9) from Desert Island; since you're already going that direction, it would help them a lot if you would use it to get down there and return it to them.

As you reach the bottom of the <em>relentless avalanche of machine parts</em>, you discover that they're already forming a formidable heap. Don't worry, though - a group of Elves is already here organizing the parts, and they have a <em>system</em>.

To start, each part is rated in each of four categories:


 - <code>x</code>: E<em>x</em>tremely cool looking
 - <code>m</code>: <em>M</em>usical (it makes a noise when you hit it)
 - <code>a</code>: <em>A</em>erodynamic
 - <code>s</code>: <em>S</em>hiny

Then, each part is sent through a series of <em>workflows</em> that will ultimately <em>accept</em> or <em>reject</em> the part. Each workflow has a name and contains a list of <em>rules</em>; each rule specifies a condition and where to send the part if the condition is true. The first rule that matches the part being considered is applied immediately, and the part moves on to the destination described by the rule. (The last rule in each workflow has no condition and always applies if reached.)

Consider the workflow <code>ex{x>10:one,m<20:two,a>30:R,A}</code>. This workflow is named <code>ex</code> and contains four rules. If workflow <code>ex</code> were considering a specific part, it would perform the following steps in order:


 - Rule "<code>x>10:one</code>": If the part's <code>x</code> is more than <code>10</code>, send the part to the workflow named <code>one</code>.
 - Rule "<code>m<20:two</code>": Otherwise, if the part's <code>m</code> is less than <code>20</code>, send the part to the workflow named <code>two</code>.
 - Rule "<code>a>30:R</code>": Otherwise, if the part's <code>a</code> is more than <code>30</code>, the part is immediately <em>rejected</em> (<code>R</code>).
 - Rule "<code>A</code>": Otherwise, because no other rules matched the part, the part is immediately <em>accepted</em> (<code>A</code>).

If a part is sent to another workflow, it immediately switches to the start of that workflow instead and never returns. If a part is <em>accepted</em> (sent to <code>A</code>) or <em>rejected</em> (sent to <code>R</code>), the part immediately stops any further processing.

The system works, but it's not keeping up with the torrent of weird metal shapes. The Elves ask if you can help sort a few parts and give you the list of workflows and some part ratings (your puzzle input). For example:

<pre>
<code>px{a<2006:qkq,m>2090:A,rfg}
pv{a>1716:R,A}
lnx{m>1548:A,A}
rfg{s<537:gd,x>2440:R,A}
qs{s>3448:A,lnx}
qkq{x<1416:A,crn}
crn{x>2662:A,R}
in{s<1351:px,qqz}
qqz{s>2770:qs,m<1801:hdj,R}
gd{a>3333:R,R}
hdj{m>838:A,pv}

{x=787,m=2655,a=1222,s=2876}
{x=1679,m=44,a=2067,s=496}
{x=2036,m=264,a=79,s=2244}
{x=2461,m=1339,a=466,s=291}
{x=2127,m=1623,a=2188,s=1013}
</code>
</pre>

The workflows are listed first, followed by a blank line, then the ratings of the parts the Elves would like you to sort. All parts begin in the workflow named <code>in</code>. In this example, the five listed parts go through the following workflows:


 - <code>{x=787,m=2655,a=1222,s=2876}</code>: <code>in</code> -> <code>qqz</code> -> <code>qs</code> -> <code>lnx</code> -> <code><em>A</em></code>
 - <code>{x=1679,m=44,a=2067,s=496}</code>: <code>in</code> -> <code>px</code> -> <code>rfg</code> -> <code>gd</code> -> <code><em>R</em></code>
 - <code>{x=2036,m=264,a=79,s=2244}</code>: <code>in</code> -> <code>qqz</code> -> <code>hdj</code> -> <code>pv</code> -> <code><em>A</em></code>
 - <code>{x=2461,m=1339,a=466,s=291}</code>: <code>in</code> -> <code>px</code> -> <code>qkq</code> -> <code>crn</code> -> <code><em>R</em></code>
 - <code>{x=2127,m=1623,a=2188,s=1013}</code>: <code>in</code> -> <code>px</code> -> <code>rfg</code> -> <code><em>A</em></code>

Ultimately, three parts are <em>accepted</em>. Adding up the <code>x</code>, <code>m</code>, <code>a</code>, and <code>s</code> rating for each of the accepted parts gives <code>7540</code> for the part with <code>x=787</code>, <code>4623</code> for the part with <code>x=2036</code>, and <code>6951</code> for the part with <code>x=2127</code>. Adding all of the ratings for <em>all</em> of the accepted parts gives the sum total of <code><em>19114</em></code>.

Sort through all of the parts you've been given; <em>what do you get if you add together all of the rating numbers for all of the parts that ultimately get accepted?</em>


## --- Part Two ---
Even with your help, the sorting process <em>still</em> isn't fast enough.

One of the Elves comes up with a new plan: rather than sort parts individually through all of these workflows, maybe you can figure out in advance which combinations of ratings will be accepted or rejected.

Each of the four ratings (<code>x</code>, <code>m</code>, <code>a</code>, <code>s</code>) can have an integer value ranging from a minimum of <code>1</code> to a maximum of <code>4000</code>. Of <em>all possible distinct combinations</em> of ratings, your job is to figure out which ones will be <em>accepted</em>.

In the above example, there are <code><em>167409079868000</em></code> distinct combinations of ratings that will be accepted.

Consider only your list of workflows; the list of part ratings that the Elves wanted you to sort is no longer relevant. <em>How many distinct combinations of ratings will be accepted by the Elves' workflows?</em>


