using System;

namespace Ira.Game {
    internal class Program {
        static void Main(string[] args) {
            var painter = new Painter();
            var game = new GameEngine(painter);
            while (game.State != GameStates.Exit) {
                //keyboard
                var key = Console.ReadKey(true);
                switch (key.Key) {
                    case ConsoleKey.UpArrow:
                        game.ProcessAction(ControllerActions.Up);
                        break;
                    case ConsoleKey.DownArrow:
                        game.ProcessAction(ControllerActions.Down);
                        break;
                    case ConsoleKey.RightArrow:
                        game.ProcessAction(ControllerActions.Right);
                        break;
                    case ConsoleKey.LeftArrow:
                        game.ProcessAction(ControllerActions.Left);
                        break;
                    case ConsoleKey.Spacebar:
                        game.ProcessAction(ControllerActions.Bomb);
                        break;
                    case ConsoleKey.Escape:
                        game.ProcessAction(ControllerActions.Exit);
                        break;
                    case ConsoleKey.Enter:
                        game.ProcessAction(ControllerActions.Start);
                        break;
                    default:
                        game.ProcessAction(ControllerActions.Other);
                        break;
                }
                //Clear keyboard buffer
                while (Console.KeyAvailable) {
                    Console.ReadKey(true);
                }
            }
        }
    }
}