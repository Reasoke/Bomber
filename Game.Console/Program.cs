using System;

namespace Ira.Game {
    internal class Program {
        static void Main(string[] args) {

            var painter = new Painter();
            var game = new GameEngine(painter);
            game.StartNew();

            Console.WriteLine("Game is finished. Press any key to exit.");
            Console.ReadKey();
        }

    }
}
