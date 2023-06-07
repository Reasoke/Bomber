using System;

namespace Ira.Game
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var painter = new Painter();
            var game = new GameEngine(painter);
            while (true)
            {
                //keyboard
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        game.ProcessAction(ControllerActions.Player1Up);
                        break;
                    case ConsoleKey.DownArrow:
                        game.ProcessAction(ControllerActions.Player1Down);
                        break;
                    case ConsoleKey.RightArrow:
                        game.ProcessAction(ControllerActions.Player1Right);
                        break;
                    case ConsoleKey.LeftArrow:
                        game.ProcessAction(ControllerActions.Player1Left);
                        break;
                    case ConsoleKey.Spacebar:
                        game.ProcessAction(ControllerActions.Player1Bomb);
                        break;
                    case ConsoleKey.Escape:
                        game.ProcessAction(ControllerActions.Player1Exit);
                        break;
                    case ConsoleKey.Enter:
                        game.ProcessAction(ControllerActions.Player1Start);
                        break;

                    case ConsoleKey.W:
                        game.ProcessAction(ControllerActions.Player2Up);
                        break;
                    case ConsoleKey.S:
                        game.ProcessAction(ControllerActions.Player2Down);
                        break;
                    case ConsoleKey.D:
                        game.ProcessAction(ControllerActions.Player2Right);
                        break;
                    case ConsoleKey.A:
                        game.ProcessAction(ControllerActions.Player2Left);
                        break;
                    case ConsoleKey.E:
                        game.ProcessAction(ControllerActions.Player2Bomb);
                        break;
                    case ConsoleKey.Q:
                        game.ProcessAction(ControllerActions.Player2Exit);
                        break;
                    case ConsoleKey.F:
                        game.ProcessAction(ControllerActions.Player2Start);
                        break;

                    default:
                        game.ProcessAction(ControllerActions.Other);
                        break;
                }

                //Clear keyboard buffer
                while (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                }
            }
        }
    }
}