using Simpl.Networking;
using System;
using System.Collections.Generic;

namespace Ira.Game
{
    public class GameEngine
    {
        private readonly IPainter painter;

        private Finish finish;
        private Player player1;
        private Player player2;
        private readonly List<Enemy> enemies = new List<Enemy>();
        private GameBoard board;
        private int currentLevel;
        private int currentSelectedItem;
        private bool singlePlayerGame = true;
        private bool player1Move = true;
        private NetworkConnection socket;
        private GameState state;

        public GameEngine(IPainter painter)
        {
            this.painter = painter;
            state = GameState.Start;
            StartScreen();
        }

        public void ProcessAction(ControllerAction action, bool localEvent = true)
        {
            if (socket != null && localEvent)
            {
                var iAmPlayerOne = socket is ServerNetworkConnection;
                //ignore actions from other player keyboard
                if (iAmPlayerOne)
                {
                    switch (action)
                    {
                        case ControllerAction.Player2Up:
                        case ControllerAction.Player2Down:
                        case ControllerAction.Player2Left:
                        case ControllerAction.Player2Right:
                        case ControllerAction.Player2Bomb:
                        case ControllerAction.Player2Exit:
                        case ControllerAction.Player2Start:
                            return;
                    }
                }
                else
                {
                    switch (action)
                    {
                        case ControllerAction.Player1Up:
                        case ControllerAction.Player1Down:
                        case ControllerAction.Player1Left:
                        case ControllerAction.Player1Right:
                        case ControllerAction.Player1Bomb:
                        // case ControllerActions.Player1Exit:
                        case ControllerAction.Player1Start:
                            return;
                    }
                }

                if (!socket.Send(action.ToString()))
                {
                    //network error
                    OnSocketDisconnected();
                    return;
                }
            }

            switch (state)
            {
                case GameState.Start:
                    ProcessStartScreenActions(action);
                    break;
                case GameState.Shop:
                    ProcessShopScreenActions(action);
                    break;
                case GameState.Win:
                    currentSelectedItem = 0;
                    if (singlePlayerGame && action == ControllerAction.Player1Start)
                    {
                        state = GameState.Shop;
                        Shop();
                    }
                    else if(singlePlayerGame && action == ControllerAction.Player1Exit)
                    {
                        state = GameState.Start;
                        StartScreen();
                    }

                    break;
                case GameState.Lose:
                    state = GameState.Start;
                    currentSelectedItem = 0;
                    StartScreen();
                    break;
                case GameState.Game:
                    ProcessGameScreenActions(action);
                    break;
                case GameState.Loading:
                    switch (action)
                    {
                        case ControllerAction.Player1Exit:
                        case ControllerAction.Player2Exit:
                            state = GameState.Start;
                            currentSelectedItem = 0;
                            StartScreen();
                            break;
                    }

                    break;
            }
        }

        private void ProcessGameScreenActions(ControllerAction action)
        {
            bool makeMove = false;
            // if (player1Move)
            {
                switch (action)
                {
                    case ControllerAction.Player1Exit:
                    case ControllerAction.Player2Exit:
                        state = GameState.Lose;
                        StopLevel();
                        return;
                    case ControllerAction.Player1Up:
                        player1.MoveDirection = MoveDirection.Up;
                        makeMove = true;
                        break;
                    case ControllerAction.Player1Down:
                        player1.MoveDirection = MoveDirection.Down;
                        makeMove = true;
                        break;
                    case ControllerAction.Player1Right:
                        player1.MoveDirection = MoveDirection.Right;
                        makeMove = true;
                        break;
                    case ControllerAction.Player1Left:
                        player1.MoveDirection = MoveDirection.Left;
                        makeMove = true;
                        break;
                    case ControllerAction.Player1Bomb:
                        player1.SetTheBomb(singlePlayerGame ? 6 : 12);
                        makeMove = true;
                        break;
                    // case ControllerActions.Other:
                        // makeMove = true;
                        // break;
                // }
            // }
            // else
            // {
                // switch (action)
                // {
                    // case ControllerActions.Player1Exit:
                    // case ControllerActions.Player2Exit:
                        // state = GameStates.Lose;
                        // StopLevel();
                        // return;

                    case ControllerAction.Player2Up:
                        player2.MoveDirection = MoveDirection.Up;
                        makeMove = true;
                        break;
                    case ControllerAction.Player2Down:
                        player2.MoveDirection = MoveDirection.Down;
                        makeMove = true;
                        break;
                    case ControllerAction.Player2Right:
                        player2.MoveDirection = MoveDirection.Right;
                        makeMove = true;
                        break;
                    case ControllerAction.Player2Left:
                        player2.MoveDirection = MoveDirection.Left;
                        makeMove = true;
                        break;
                    case ControllerAction.Player2Bomb:
                        makeMove = true;
                        player2.SetTheBomb(12);
                        break;
                    case ControllerAction.Other:
                        makeMove = true;
                        break;
                }
            }

            if (!makeMove)
            {
                return;
            }

            if (player2 != null) 
                player1Move = !player1Move;
            MakeMove();
            if (!player1.IsAlive || (player2 != null && !player2.IsAlive))
            {
                if (socket != null)
                {
                    var iAmPlayerOne = socket is ServerNetworkConnection;
                    if (iAmPlayerOne && player1.IsAlive || !iAmPlayerOne && !player1.IsAlive)
                    {
                        state = GameState.Win;
                    }
                    else
                        state = GameState.Lose;
                }
                else
                {
                    state = GameState.Lose;
                }

                StopLevel();
            }
            else if (player1.IsWinner)
            {
                Utils.PlaySoundExit();
                player1.IsWinner = false;
                state = GameState.Win;
                StopLevel();
            }
        }

        private void ProcessShopScreenActions(ControllerAction action)
        {
            switch (action)
            {
                case ControllerAction.Player1Exit:
                    Environment.Exit(0);
                    break;
                case ControllerAction.Player1Up:
                    currentSelectedItem--;
                    Shop();
                    break;
                case ControllerAction.Player1Down:
                    currentSelectedItem++;
                    Shop();
                    break;

                case ControllerAction.Player1Start:
                    bool restartShop = false;
                    var selectedItem = (ShopMenuItem)currentSelectedItem;
                    switch (selectedItem)
                    {
                        case ShopMenuItem.Life:
                            ProcessSelectedItem(ShopMenuItem.Life);
                            break;
                        case ShopMenuItem.BombCountBonus:
                            ProcessSelectedItem(ShopMenuItem.BombCountBonus);
                            break;
                        case ShopMenuItem.BombPowerBonus:
                            ProcessSelectedItem(ShopMenuItem.BombPowerBonus);
                            break;
                        case ShopMenuItem.Armor:
                            ProcessSelectedItem(ShopMenuItem.Armor);
                            break;
                    }

                    if (!restartShop)
                    {
                        if (currentLevel < 5)
                        {
                            state = GameState.Game;
                            currentLevel++;
                            StartLevel();
                        }
                        else
                        {
                            //no more levels supported
                            state = GameState.Start;
                            StartScreen();
                        }
                    }
                    else Utils.PlaySoundError();

                    break;
                    
                    void ProcessSelectedItem(ShopMenuItem item)
                    {
                        if (player1.Score >= ShopHelper.Prices[item])
                        {
                            player1.Score -= ShopHelper.Prices[item];
                            switch (item)
                            {
                                case ShopMenuItem.Life:
                                    player1.AddLife();
                                    break;
                                case ShopMenuItem.BombCountBonus:
                                    player1.AddBomb();
                                    break;
                                case ShopMenuItem.BombPowerBonus:
                                    player1.AddBombPower();
                                    break;
                                case ShopMenuItem.Armor:
                                    player1.IsProtected = true;
                                    break;
                            }
                        }
                        else restartShop = true;   
                    }
            }
            
        }

        private void ProcessStartScreenActions(ControllerAction action)
        {
            switch (action)
            {
                case ControllerAction.Player1Exit:
                    Environment.Exit(0);
                    break;
                case ControllerAction.Player1Up:
                    currentSelectedItem--;
                    StartScreen();
                    break;
                case ControllerAction.Player1Down:
                    currentSelectedItem++;
                    StartScreen();
                    break;

                case ControllerAction.Player1Start:
                    var selectedItem = (StartMenuItem)currentSelectedItem;
                    switch (selectedItem)
                    {
                        case StartMenuItem.LocalOnePlayer:
                            state = GameState.Game;
                            singlePlayerGame = true;
                            StartLevel();
                            break;
                        case StartMenuItem.LocalTwoPlayers:
                            state = GameState.Game;
                            singlePlayerGame = false;
                            StartLevel();
                            break;
                        case StartMenuItem.StartServer: //server
                            state = GameState.Loading;
                            StartServer();
                            break;
                        case StartMenuItem.Connect: //client
                            state = GameState.Loading;
                            StartClient();
                            break;
                        case StartMenuItem.Exit:
                            Environment.Exit(0);
                            break;
                    }

                    break;
            }
        }
        
        private void StartServer()
        {
            painter.DrawMessageScreen("Waiting...");
            try
            {
                socket = new ServerNetworkConnection();
                socket.Connect(ProcessConnectingResult, 5 * 60 * 1000);
            }
            catch (Exception)
            {
                painter.DrawMessageScreen("Error");
            }
        }

        private void StartClient()
        {
            painter.DrawMessageScreen("Searching...");
            try
            {
                socket = new NetworkConnection();
                socket.Connect(ProcessConnectingResult, 30 * 1000);
            }
            catch (Exception)
            {
                painter.DrawMessageScreen("Error");
            }
        }

        private void ProcessConnectingResult()
        {
            if (!socket.Connected)
            {
                OnSocketDisconnected("Timeout");
                return;
            }

            socket.OnDisconnected += OnSocketDisconnected;
            socket.OnMessageReceived += OnSocketMessage;
            state = GameState.Game;
            singlePlayerGame = false;
            StartLevel();
        }

        private void CloseNetwork()
        {
            if (socket == null) return;
            socket.OnDisconnected -= OnSocketDisconnected;
            socket.OnMessageReceived -= OnSocketMessage;
            socket.Dispose();
            socket = null;
        }

        private void OnSocketDisconnected(string error = "Disconnected")
        {
            CloseNetwork();
            state = GameState.Loading;
            painter.DrawMessageScreen(error);
        }

        private void OnSocketMessage(string message)
        {
            if (Enum.TryParse(message, out ControllerAction action))
            {
                ProcessAction(action, false);
            }
            else
            {
                //ignore?
            }
        }
        
        private void StartScreen(int level = 0)
        {
            CloseNetwork(); //probably useless, but lets check for sure

            if (currentSelectedItem < 0) 
                currentSelectedItem = 4;
            if (currentSelectedItem > 4) 
                currentSelectedItem = 0;
            painter.DrawStart(currentSelectedItem);
            Utils.PlaySoundMove();
            currentLevel = level;
        }

        private void Shop()
        {
            if (currentSelectedItem < 0) 
                currentSelectedItem = 4;
            if (currentSelectedItem > 4) 
                currentSelectedItem = 0;
            painter.DrawShop(currentSelectedItem, player1);
            Utils.PlaySoundMove();
        }

        private void StartLevel()
        {
            if (player1 == null) 
                player1 = new Player(1, 1, board, 3);
            if (singlePlayerGame == false)
            {
                if (player2 == null) 
                    player2 = new Player(13, 15, board, 3);
                player1.AddBomb(2);
                player2.AddBomb(2);
            }

            Utils.PlayMainTheme();
            board = BoardReader.ReadData(singlePlayerGame ? currentLevel : -1, enemies, player1, player2, ref finish);
            painter.Clear();
            painter.DrawBoard(board, player1, player2, enemies, player1Move);
        }

        private void StopLevel()
        {
            Utils.StopMainTheme();
            if (state == GameState.Win)
            {
                painter.DrawWin();
            }
            else
            {
                player1 = null;
                player2 = null;
                painter.DrawLose();
            }
        }

        private void MakeMove()
        {
            //game logic
            player1.Move();
            if (player2 != null) 
                player2.Move();
            foreach (var e in enemies)
            {
                e.Move();
                e.InteractWithBoard();
            }

            board.ProcessElements(player1, enemies); //todo maybe add player 2
            if (finish != null && enemies.Count == 0)
            {
                finish.ExitMode = true;
            }

            player1.InteractWithBoard(enemies);
            if (player2 != null) 
                player2.InteractWithBoard(enemies);

            painter.DrawBoard(board, player1, player2, enemies, player1Move);
        }
    }
}