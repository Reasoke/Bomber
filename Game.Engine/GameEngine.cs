using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Ira.Game {
    public class GameEngine {
        private readonly IPainter painter;

        private Finish finish;
        private Player player;
        private readonly List<Enemy> enemies = new List<Enemy>();
        private GameBoard board;
        private int currentLevel;
        private int currentSelectedItem;
        
        public GameStates State { get; private set; }
        
        public GameEngine(IPainter painter) {
            this.painter = painter;
            State = GameStates.Start;
            Start();
        }

        public void ProcessAction(ControllerActions action) {
            switch (State) {
                case GameStates.Start:
                    switch (action) {
                        case ControllerActions.Exit:
                            Environment.Exit(0);
                            break;
                        case ControllerActions.Up:
                            currentSelectedItem--;
                            Start();
                            break;
                        case ControllerActions.Down:
                            currentSelectedItem++;
                            Start();
                            break;

                        case ControllerActions.Start:
                            switch (currentSelectedItem) {
                                case 0:
                                    State = GameStates.Game;
                                    StartLevel();
                                    break;
                                case 1:
                                    // TODO: State = GameStates.Options;
                                    break;
                                case 2:
                                    // State = GameStates.Exit;
                                    Environment.Exit(0);
                                    break;
                            }

                            break;
                    }

                    break;
                case GameStates.Shop:
                    switch (action) {
                        case ControllerActions.Exit:
                            Environment.Exit(0);
                            break;
                        case ControllerActions.Up:
                            currentSelectedItem--;
                            Shop();
                            break;
                        case ControllerActions.Down:
                            currentSelectedItem++;
                            Shop();
                            break;

                        case ControllerActions.Start:
                            bool restartShop = false;
                            switch (currentSelectedItem) {
                                case 0:
                                    if (player.Score >= 5) {
                                        player.Score -= 5;
                                        player.AddLife();
                                    }
                                    else restartShop = true;
                                    break;
                                case 1:
                                    if (player.Score >= 3) {
                                        player.Score -= 3;                                   
                                        player.AddBomb();
                                    }
                                    else restartShop = true;
                                    break;
                                case 2:
                                    if (player.Score >= 3) {
                                        player.Score -= 3;                                   
                                        player.AddBombPower();
                                    }
                                    else restartShop = true;
                                    break;
                                case 3:
                                    if (player.Score >= 4) {
                                        player.Score -= 4;                                   
                                        player.IsProtected = true;
                                    }
                                    else restartShop = true;
                                    break;
                            }

                            if (!restartShop) {
                                if (currentLevel < 5) {
                                    State = GameStates.Game;
                                    currentLevel++;
                                    StartLevel();
                                }
                                else {
                                    //no more levels supported
                                    State = GameStates.Start;
                                    Start();
                                }
                            }
                            else Utils.PlaySoundError();
                            break;
                    }
                    break;

                case GameStates.Exit:
                    //nothing to do
                    Environment.Exit(0);
                    break;
                case GameStates.Win:
                    if (action == ControllerActions.Start) {
                        State = GameStates.Shop;
                        currentSelectedItem = 0;
                        Shop();
                    }

                    break;
                case GameStates.Lose:
                    // switch (action) {
                    //     case ControllerActions.Start:
                            State = GameStates.Start;
                            currentSelectedItem = 0;
                            Start();
                    //         break;
                    //     case ControllerActions.Exit:
                    //         State = GameStates.Exit;
                    //         break;
                    // }
                    break;
                case GameStates.Game:
                    switch (action) {
                        case ControllerActions.Exit:
                            State = GameStates.Lose;
                            StopLevel();
                            return;
                        
                        case ControllerActions.Up:
                            player.MoveDirection = MoveDirection.Up;
                            break;
                        case ControllerActions.Down:
                            player.MoveDirection = MoveDirection.Down;
                            break;
                        case ControllerActions.Right:
                            player.MoveDirection = MoveDirection.Right;
                            break;
                        case ControllerActions.Left:
                            player.MoveDirection = MoveDirection.Left;
                            break;
                        case ControllerActions.Bomb:
                            player.SetTheBomb();
                            break;
                    }

                    MakeMove();
                    if (!player.IsAlive) {
                        State = GameStates.Lose;
                        StopLevel();
                    }
                    else if (player.IsWinner) {
                        Utils.PlaySoundExit();
                        player.IsWinner = false;
                        State = GameStates.Win;
                        StopLevel();
                    }
                    break;
            }
        }
        
        public void Start(int level = 0) {
            if (currentSelectedItem < 0) currentSelectedItem = 2;
            if (currentSelectedItem > 2) currentSelectedItem = 0;
            painter.DrawStart(currentSelectedItem);
            Utils.PlaySoundMove();
            currentLevel = level;
        }
        
        public void Shop() {
            if (currentSelectedItem < 0) currentSelectedItem = 4;
            if (currentSelectedItem > 4) currentSelectedItem = 0;
            painter.DrawShop(currentSelectedItem, player);
            Utils.PlaySoundMove();
        }

        private void StartLevel() {
            if(player == null) player = new Player(1, 1, board, 3);
            Utils.PlayMainTheme();
            ReadData(currentLevel);
            painter.Clear();
            painter.DrawBoard(board, player, enemies);
        }

        private void StopLevel() {
            Utils.StopMainTheme();
            if (State == GameStates.Win){
                painter.DrawWin();
            }
            else {
                player = null;
                painter.DrawLose();
            }
        }

        private void MakeMove() {
            //game logic
            player.Move();
            foreach (var e in enemies) {
                e.Move();
                e.InteractWithBoard();
            }

            board.ProcessElements(player, enemies);
            if (enemies.Count == 0) {
                finish.ExitMode = true;
            }

            player.InteractWithBoard(enemies);

            painter.DrawBoard(board, player, enemies);
        }

        private void ReadData(int level) {
            var fileName = $".\\media\\Level_{level}.txt";
            var fileLines = File.ReadAllLines(fileName);
            var height = fileLines.Length;
            var width = GetMaxLength(fileLines);

            board = new GameBoard(width, height);
            enemies.Clear();
            for (int y = 0; y < height; y++) {
                // перебираем все файловые строки
                var line = fileLines[y];
                for (int x = 0; x < line.Length; x++) {
                    //перебираем все символы строки
                    var symbol = line[x];
                    switch (symbol) {
                        case '#':
                            board[x, y] = new PermanentWall();
                            break;

                        case '@':
                            board[x, y] = new CrumblingWall();
                            break;

                        case ' ':
                            board[x, y] = null;
                            break;
                        case 'C':
                            board[x, y] = new Coins();
                            break;

                        case 'F':
                            board[x, y] = finish = new Finish();
                            break;

                        //bonus elements
                        case 'A':
                            board[x, y] = new Armor();
                            break;
                        case 'W':
                            board[x, y] = new BombPowerBonus();
                            break;
                        case 'Z':
                            board[x, y] = new BombCountBonus();
                            break;

                        //characters
                        case 'E':
                            var enemy = new Enemy(x, y, board, 0);
                            board[x, y] = null;
                            enemies.Add(enemy);
                            break;
                        case 'S':
                            var smartEnemy = new Enemy(x, y, board, 1);
                            board[x, y] = null;
                            enemies.Add(smartEnemy);
                            break;
                        case 'I':
                            var intelligentEnemy =
                                new IntelligentEnemy(x, y, board,
                                    player); //todo player could not be found at that moment
                            board[x, y] = null;
                            enemies.Add(intelligentEnemy);
                            break;
                        case 'G':
                            var ghost = new Ghost(x, y, board, player); //todo player could not be found at that moment
                            board[x, y] = null;
                            enemies.Add(ghost);
                            break;

                        case 'P':
                            player.SetRespawnLocation(x, y, board);
                            board[x, y] = null;
                            break;
                    }
                }
            }
        }

        private int GetMaxLength(string[] lines) {
            return lines == null || lines.Length == 0 ? 0 : lines.Max(l => l.Length);
        }
    }
}