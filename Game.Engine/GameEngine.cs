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
        private GameStates state;

        public GameEngine(IPainter painter)
        {
            this.painter = painter;
            state = GameStates.Start;
            StartScreen();
        }

        public void ProcessAction(ControllerActions action, bool localEvent = true)
        {
            if (socket != null && localEvent)
            {
                var iAmPlayerOne = socket is ServerNetworkConnection;
                //ignore actions from other player keyboard
                if (iAmPlayerOne)
                {
                    switch (action)
                    {
                        case ControllerActions.Player2Up:
                        case ControllerActions.Player2Down:
                        case ControllerActions.Player2Left:
                        case ControllerActions.Player2Right:
                        case ControllerActions.Player2Bomb:
                        case ControllerActions.Player2Exit:
                        case ControllerActions.Player2Start:
                            return;
                    }
                }
                else
                {
                    switch (action)
                    {
                        case ControllerActions.Player1Up:
                        case ControllerActions.Player1Down:
                        case ControllerActions.Player1Left:
                        case ControllerActions.Player1Right:
                        case ControllerActions.Player1Bomb:
                        // case ControllerActions.Player1Exit:
                        case ControllerActions.Player1Start:
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
                case GameStates.Start:
                    ProcessStartScreenActions(action);
                    break;
                case GameStates.Shop:
                    ProcessShopScreenActions(action);
                    break;
                case GameStates.Win:
                    currentSelectedItem = 0;
                    if (singlePlayerGame && action == ControllerActions.Player1Start)
                    {
                        state = GameStates.Shop;
                        Shop();
                    }
                    else
                    {
                        state = GameStates.Start;
                        StartScreen();
                    }

                    break;
                case GameStates.Lose:
                    state = GameStates.Start;
                    currentSelectedItem = 0;
                    StartScreen();
                    break;
                case GameStates.Game:
                    ProcessGameScreenActions(action);
                    break;
                case GameStates.Loading:
                    switch (action)
                    {
                        case ControllerActions.Player1Exit:
                        case ControllerActions.Player2Exit:
                            state = GameStates.Start;
                            currentSelectedItem = 0;
                            StartScreen();
                            break;
                    }

                    break;
            }
        }

        private void ProcessGameScreenActions(ControllerActions action)
        {
            bool makeMove = false;
            // if (player1Move)
            {
                switch (action)
                {
                    case ControllerActions.Player1Exit:
                    case ControllerActions.Player2Exit:
                        state = GameStates.Lose;
                        StopLevel();
                        return;
                    case ControllerActions.Player1Up:
                        player1.MoveDirection = MoveDirection.Up;
                        makeMove = true;
                        break;
                    case ControllerActions.Player1Down:
                        player1.MoveDirection = MoveDirection.Down;
                        makeMove = true;
                        break;
                    case ControllerActions.Player1Right:
                        player1.MoveDirection = MoveDirection.Right;
                        makeMove = true;
                        break;
                    case ControllerActions.Player1Left:
                        player1.MoveDirection = MoveDirection.Left;
                        makeMove = true;
                        break;
                    case ControllerActions.Player1Bomb:
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

                    case ControllerActions.Player2Up:
                        player2.MoveDirection = MoveDirection.Up;
                        makeMove = true;
                        break;
                    case ControllerActions.Player2Down:
                        player2.MoveDirection = MoveDirection.Down;
                        makeMove = true;
                        break;
                    case ControllerActions.Player2Right:
                        player2.MoveDirection = MoveDirection.Right;
                        makeMove = true;
                        break;
                    case ControllerActions.Player2Left:
                        player2.MoveDirection = MoveDirection.Left;
                        makeMove = true;
                        break;
                    case ControllerActions.Player2Bomb:
                        makeMove = true;
                        player2.SetTheBomb(12);
                        break;
                    case ControllerActions.Other:
                        makeMove = true;
                        break;
                }
            }

            if (!makeMove)
            {
                return;
            }

            if (player2 != null) player1Move = !player1Move;
            MakeMove();
            if (!player1.IsAlive || (player2 != null && !player2.IsAlive))
            {
                if (socket != null)
                {
                    var iAmPlayerOne = socket is ServerNetworkConnection;
                    if (iAmPlayerOne && player1.IsAlive || !iAmPlayerOne && !player1.IsAlive)
                    {
                        state = GameStates.Win;
                    }
                    else
                        state = GameStates.Lose;
                }
                else
                {
                    state = GameStates.Lose; //TODO player1/player2 win for multiplayer
                }

                StopLevel();
            }
            else if (player1.IsWinner)
            {
                Utils.PlaySoundExit();
                player1.IsWinner = false;
                state = GameStates.Win;
                StopLevel();
            }
        }

        private void ProcessShopScreenActions(ControllerActions action)
        {
            switch (action)
            {
                case ControllerActions.Player1Exit:
                    Environment.Exit(0);
                    break;
                case ControllerActions.Player1Up:
                    currentSelectedItem--;
                    Shop();
                    break;
                case ControllerActions.Player1Down:
                    currentSelectedItem++;
                    Shop();
                    break;

                case ControllerActions.Player1Start:
                    bool restartShop = false;
                    switch (currentSelectedItem)
                    {
                        case 0:
                            if (player1.Score >= 5)
                            {
                                player1.Score -= 5;
                                player1.AddLife();
                            }
                            else restartShop = true;

                            break;
                        case 1:
                            if (player1.Score >= 3)
                            {
                                player1.Score -= 3;
                                player1.AddBomb();
                            }
                            else restartShop = true;

                            break;
                        case 2:
                            if (player1.Score >= 3)
                            {
                                player1.Score -= 3;
                                player1.AddBombPower();
                            }
                            else restartShop = true;

                            break;
                        case 3:
                            if (player1.Score >= 4)
                            {
                                player1.Score -= 4;
                                player1.IsProtected = true;
                            }
                            else restartShop = true;

                            break;
                    }

                    if (!restartShop)
                    {
                        if (currentLevel < 5)
                        {
                            state = GameStates.Game;
                            currentLevel++;
                            StartLevel();
                        }
                        else
                        {
                            //no more levels supported
                            state = GameStates.Start;
                            StartScreen();
                        }
                    }
                    else Utils.PlaySoundError();

                    break;
            }
        }

        private void ProcessStartScreenActions(ControllerActions action)
        {
            switch (action)
            {
                case ControllerActions.Player1Exit:
                    Environment.Exit(0);
                    break;
                case ControllerActions.Player1Up:
                    currentSelectedItem--;
                    StartScreen();
                    break;
                case ControllerActions.Player1Down:
                    currentSelectedItem++;
                    StartScreen();
                    break;

                case ControllerActions.Player1Start:
                    switch (currentSelectedItem)
                    {
                        case 0:
                            state = GameStates.Game;
                            singlePlayerGame = true;
                            StartLevel();
                            break;
                        case 1:
                            state = GameStates.Game;
                            singlePlayerGame = false;
                            StartLevel();
                            break;
                        case 2: //server
                            state = GameStates.Loading;
                            StartServer();
                            break;
                        case 3: //client
                            state = GameStates.Loading;
                            StartClient();
                            break;
                        case 4:
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
            catch (Exception ex)
            {
                painter.DrawMessageScreen(ex.Message);
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
            catch (Exception ex)
            {
                painter.DrawMessageScreen(ex.Message);
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
            state = GameStates.Game;
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
            state = GameStates.Loading;
            painter.DrawMessageScreen(error);
        }

        private void OnSocketMessage(string message)
        {
            if (Enum.TryParse(message, out ControllerActions action))
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

            if (currentSelectedItem < 0) currentSelectedItem = 4;
            if (currentSelectedItem > 4) currentSelectedItem = 0;
            painter.DrawStart(currentSelectedItem);
            Utils.PlaySoundMove();
            currentLevel = level;
        }

        private void Shop()
        {
            if (currentSelectedItem < 0) currentSelectedItem = 4;
            if (currentSelectedItem > 4) currentSelectedItem = 0;
            painter.DrawShop(currentSelectedItem, player1);
            Utils.PlaySoundMove();
        }

        private void StartLevel()
        {
            if (player1 == null) player1 = new Player(1, 1, board, 3);
            if (singlePlayerGame == false)
            {
                if (player2 == null) player2 = new Player(13, 15, board, 3);
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
            if (state == GameStates.Win)
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
            if (player2 != null) player2.Move();
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
            if (player2 != null) player2.InteractWithBoard(enemies);

            painter.DrawBoard(board, player1, player2, enemies, player1Move);
        }
    }
}