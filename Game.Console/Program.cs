using System;

namespace Ira.Game {
    internal class Program {
        static void Main(string[] args) {
            
            var game = new GameEngine();
            game.StartNew();

            Console.WriteLine("Game is finished. Press any key to exit.");
            Console.ReadKey();
        }

    }
}
