original source: [https://adventofcode.com/2023/day/20](https://adventofcode.com/2023/day/20)
## --- Day 20: Pulse Propagation ---
With your help, the Elves manage to find the right parts and fix all of the machines. Now, they just need to send the command to boot up the machines and get the sand flowing again.

The machines are far apart and wired together with long <em>cables</em>. The cables don't connect to the machines directly, but rather to communication <em>modules</em> attached to the machines that perform various initialization tasks and also act as communication relays.

Modules communicate using <em>pulses</em>. Each pulse is either a <em>high pulse</em> or a <em>low pulse</em>. When a module sends a pulse, it sends that type of pulse to each module in its list of <em>destination modules</em>.

There are several different types of modules:

<em>Flip-flop</em> modules (prefix <code>%</code>) are either <em>on</em> or <em>off</em>; they are initially <em>off</em>. If a flip-flop module receives a high pulse, it is ignored and nothing happens. However, if a flip-flop module receives a low pulse, it <em>flips between on and off</em>. If it was off, it turns on and sends a high pulse. If it was on, it turns off and sends a low pulse.

<em>Conjunction</em> modules (prefix <code>&</code>) <em>remember</em> the type of the most recent pulse received from <em>each</em> of their connected input modules; they initially default to remembering a <em>low pulse</em> for each input. When a pulse is received, the conjunction module first updates its memory for that input. Then, if it remembers <em>high pulses</em> for all inputs, it sends a <em>low pulse</em>; otherwise, it sends a <em>high pulse</em>.

There is a single <em>broadcast module</em> (named <code>broadcaster</code>). When it receives a pulse, it sends the same pulse to all of its destination modules.

Here at Desert Machine Headquarters, there is a module with a single button on it called, aptly, the <em>button module</em>. When you push the button, a single <em>low pulse</em> is sent directly to the <code>broadcaster</code> module.

After pushing the button, you must wait until all pulses have been delivered and fully handled before pushing it again. Never push the button if modules are still processing pulses.

Pulses are always processed <em>in the order they are sent</em>. So, if a pulse is sent to modules <code>a</code>, <code>b</code>, and <code>c</code>, and then module <code>a</code> processes its pulse and sends more pulses, the pulses sent to modules <code>b</code> and <code>c</code> would have to be handled first.

The module configuration (your puzzle input) lists each module. The name of the module is preceded by a symbol identifying its type, if any. The name is then followed by an arrow and a list of its destination modules. For example:

<pre>
<code>broadcaster -> a, b, c
%a -> b
%b -> c
%c -> inv
&inv -> a
</code>
</pre>

In this module configuration, the broadcaster has three destination modules named <code>a</code>, <code>b</code>, and <code>c</code>. Each of these modules is a flip-flop module (as indicated by the <code>%</code> prefix). <code>a</code> outputs to <code>b</code> which outputs to <code>c</code> which outputs to another module named <code>inv</code>. <code>inv</code> is a conjunction module (as indicated by the <code>&</code> prefix) which, because it has only one input, acts like an inverter (it sends the opposite of the pulse type it receives); it outputs to <code>a</code>.

By pushing the button once, the following pulses are sent:

<pre>
<code>button -low-> broadcaster
broadcaster -low-> a
broadcaster -low-> b
broadcaster -low-> c
a -high-> b
b -high-> c
c -high-> inv
inv -low-> a
a -low-> b
b -low-> c
c -low-> inv
inv -high-> a
</code>
</pre>

After this sequence, the flip-flop modules all end up <em>off</em>, so pushing the button again repeats the same sequence.

Here's a more interesting example:

<pre>
<code>broadcaster -> a
%a -> inv, con
&inv -> b
%b -> con
&con -> output
</code>
</pre>

This module configuration includes the <code>broadcaster</code>, two flip-flops (named <code>a</code> and <code>b</code>), a single-input conjunction module (<code>inv</code>), a multi-input conjunction module (<code>con</code>), and an untyped module named <code>output</code> (for testing purposes). The multi-input conjunction module <code>con</code> watches the two flip-flop modules and, if they're both on, sends a <em>low pulse</em> to the <code>output</code> module.

Here's what happens if you push the button once:

<pre>
<code>button -low-> broadcaster
broadcaster -low-> a
a -high-> inv
a -high-> con
inv -low-> b
con -high-> output
b -high-> con
con -low-> output
</code>
</pre>

Both flip-flops turn on and a low pulse is sent to <code>output</code>! However, now that both flip-flops are on and <code>con</code> remembers a high pulse from each of its two inputs, pushing the button a second time does something different:

<pre>
<code>button -low-> broadcaster
broadcaster -low-> a
a -low-> inv
a -low-> con
inv -high-> b
con -high-> output
</code>
</pre>

Flip-flop <code>a</code> turns off! Now, <code>con</code> remembers a low pulse from module <code>a</code>, and so it sends only a high pulse to <code>output</code>.

Push the button a third time:

<pre>
<code>button -low-> broadcaster
broadcaster -low-> a
a -high-> inv
a -high-> con
inv -low-> b
con -low-> output
b -low-> con
con -high-> output
</code>
</pre>

This time, flip-flop <code>a</code> turns on, then flip-flop <code>b</code> turns off. However, before <code>b</code> can turn off, the pulse sent to <code>con</code> is handled first, so it <em>briefly remembers all high pulses</em> for its inputs and sends a low pulse to <code>output</code>. After that, flip-flop <code>b</code> turns off, which causes <code>con</code> to update its state and send a high pulse to <code>output</code>.

Finally, with <code>a</code> on and <code>b</code> off, push the button a fourth time:

<pre>
<code>button -low-> broadcaster
broadcaster -low-> a
a -low-> inv
a -low-> con
inv -high-> b
con -high-> output
</code>
</pre>

This completes the cycle: <code>a</code> turns off, causing <code>con</code> to remember only low pulses and restoring all modules to their original states.

To get the cables warmed up, the Elves have pushed the button <code>1000</code> times. How many pulses got sent as a result (including the pulses sent by the button itself)?

In the first example, the same thing happens every time the button is pushed: <code>8</code> low pulses and <code>4</code> high pulses are sent. So, after pushing the button <code>1000</code> times, <code>8000</code> low pulses and <code>4000</code> high pulses are sent. Multiplying these together gives <code><em>32000000</em></code>.

In the second example, after pushing the button <code>1000</code> times, <code>4250</code> low pulses and <code>2750</code> high pulses are sent. Multiplying these together gives <code><em>11687500</em></code>.

Consult your module configuration; determine the number of low pulses and high pulses that would be sent after pushing the button <code>1000</code> times, waiting for all pulses to be fully handled after each push of the button. <em>What do you get if you multiply the total number of low pulses sent by the total number of high pulses sent?</em>


## --- Part Two ---
The final machine responsible for moving the sand down to Island Island has a module attached named <code>rx</code>. The machine turns on when a <em>single low pulse</em> is sent to <code>rx</code>.

Reset all modules to their default states. Waiting for all pulses to be fully handled after each button press, <em>what is the fewest number of button presses required to deliver a single low pulse to the module named <code>rx</code>?</em>


