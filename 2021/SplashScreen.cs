
using System;

namespace AdventOfCode.Y2021;

class SplashScreenImpl : SplashScreen {

    public void Show() {

        var color = Console.ForegroundColor;
        Write(0xcc00, false, "           ▄█▄ ▄▄█ ▄ ▄ ▄▄▄ ▄▄ ▄█▄  ▄▄▄ ▄█  ▄▄ ▄▄▄ ▄▄█ ▄▄▄\n           █▄█ █ █ █ █ █▄█ █ █ █   █ █ █▄ ");
            Write(0xcc00, false, " █  █ █ █ █ █▄█\n           █ █ █▄█ ▀▄▀ █▄▄ █ █ █▄  █▄█ █   █▄ █▄█ █▄█ █▄▄  {:year 2021}\n            ");
            Write(0xcc00, false, "\n           ");
            Write(0x666666, false, " ~  ~  ~ ~~");
            Write(0xc8ff, false, "~");
            Write(0x666666, false, "~~");
            Write(0xc8ff, false, "~");
            Write(0x666666, false, "~");
            Write(0xc8ff, false, "~~");
            Write(0x666666, false, "~");
            Write(0xc8ff, false, "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~  ");
            Write(0xcccccc, false, " 1 ");
            Write(0xffff66, false, "**\n           ");
            Write(0x666666, false, "            .                    ");
            Write(0xffffff, false, ". ");
            Write(0xb5ed, false, ". .   ' ");
            Write(0xa47a4d, false, "..''''  ");
            Write(0xcccccc, false, " 2 ");
            Write(0xffff66, false, "**\n           ");
            Write(0x666666, false, "               .    .");
            Write(0xa2db, false, "  . .    . ");
            Write(0xffffff, false, ". ");
            Write(0xa2db, false, "    ..  ");
            Write(0xa47a4d, false, ":        ");
            Write(0xcccccc, false, " 3 ");
            Write(0xffff66, false, "**\n           ");
            Write(0x666666, false, "               .. '.            ");
            Write(0xffffff, false, ".' ");
            Write(0x91cc, false, " ' ");
            Write(0xa47a4d, false, "....'        ");
            Write(0xcccccc, false, " 4 ");
            Write(0xffff66, false, "**\n           ");
            Write(0x666666, false, "                . .  ~ .      ");
            Write(0xc74c30, false, ".");
            Write(0xff0000, false, ".");
            Write(0xffffff, false, "|\\");
            Write(0xff0000, false, ".");
            Write(0xc74c30, false, ".");
            Write(0xa47a4d, false, "''             ");
            Write(0xcccccc, false, " 5 ");
            Write(0xffff66, false, "**\n           ");
            Write(0x333333, false, "                             :                     ");
            Write(0x666666, false, " 6\n                                                               7\n                                ");
            Write(0x666666, false, "                               8\n                                                               9\n  ");
            Write(0x666666, false, "                                                            10\n                                     ");
            Write(0x666666, false, "                         11\n                                                              12\n       ");
            Write(0x666666, false, "                                                       13\n                                          ");
            Write(0x666666, false, "                    14\n                                                              15\n            ");
            Write(0x666666, false, "                                                  16\n                                               ");
            Write(0x666666, false, "               17\n                                                              18\n                 ");
            Write(0x666666, false, "                                             19\n                                                    ");
            Write(0x666666, false, "          20\n                                                              21\n                      ");
            Write(0x666666, false, "                                        22\n                                                         ");
            Write(0x666666, false, "     23\n                                                              24\n                           ");
            Write(0x666666, false, "                                   25\n           \n");
            
        Console.ForegroundColor = color;
        Console.WriteLine();
    }

   private static void Write(int rgb, bool bold, string text){
       Console.Write($"\u001b[38;2;{(rgb>>16)&255};{(rgb>>8)&255};{rgb&255}{(bold ? ";1" : "")}m{text}");
   }
}