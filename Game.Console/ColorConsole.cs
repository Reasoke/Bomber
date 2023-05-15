using System;

namespace Ira.Game {
    public static class ColorConsole {
        public static void Write(string text, ConsoleColor color = ConsoleColor.White,
            ConsoleColor background = ConsoleColor.Black) {
            Console.BackgroundColor = background;
            Console.ForegroundColor = color;
            Console.Write(text);
        }

        public static void WriteLine(string text = null, ConsoleColor color = ConsoleColor.White,
            ConsoleColor background = ConsoleColor.Black) {
            Write(text + "\n", color, background);
        }
    }
}