using System;
using System.Collections.Generic;

namespace Ira.Game
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var painter = new Painter();
            var game = new GameEngine(painter);
            Dictionary<ConsoleKey, ControllerAction> keyEvents = new Dictionary<ConsoleKey, ControllerAction>
            {
                {ConsoleKey.UpArrow, ControllerAction.Player1Up},
                {ConsoleKey.DownArrow, ControllerAction.Player1Down},
                {ConsoleKey.RightArrow, ControllerAction.Player1Right},
                {ConsoleKey.LeftArrow, ControllerAction.Player1Left},
                {ConsoleKey.Spacebar, ControllerAction.Player1Bomb},
                {ConsoleKey.Escape, ControllerAction.Player1Exit},
                {ConsoleKey.Enter, ControllerAction.Player1Start},
                {ConsoleKey.W, ControllerAction.Player2Up},
                {ConsoleKey.S, ControllerAction.Player2Down},
                {ConsoleKey.D, ControllerAction.Player2Right},
                {ConsoleKey.A, ControllerAction.Player2Left},
                {ConsoleKey.E, ControllerAction.Player2Bomb},
                {ConsoleKey.Q, ControllerAction.Player2Exit},
                {ConsoleKey.F, ControllerAction.Player2Start},
            };

            while (true)
            {
                //keyboard
                var key = Console.ReadKey(true);
                if(keyEvents.TryGetValue(key.Key, out var action))
                    game.ProcessAction(action);
                //Clear keyboard buffer
                while (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                }
            }
        }
    }
}