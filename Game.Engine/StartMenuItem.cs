using System;
using System.Collections.Generic;

namespace Ira.Game
{
    public enum StartMenuItem
    {
        LocalOnePlayer,
        LocalTwoPlayers,
        StartServer,
        Connect,
        Exit,
    }
    
    public static class StartMenuHelper
    {
        public static Dictionary<StartMenuItem, string> StartMenuItems = new Dictionary<StartMenuItem, string> {
            {StartMenuItem.LocalOnePlayer, "1 player"},
            {StartMenuItem.LocalTwoPlayers, "2 players"},
            {StartMenuItem.StartServer, "Start server"},
            {StartMenuItem.Connect, "Connect"},
            {StartMenuItem.Exit, "Exit"},
        };
        
    }
}