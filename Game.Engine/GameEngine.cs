using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ira.Game {
    public class GameEngine {
        private readonly IPainter painter;

        private Finish finish;
        private Player player;
        private readonly List<Enemy> enemies = new List<Enemy>();
        private GameBoard board;
        private int currentLevel;
        
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
                            State = GameStates.Exit;
                            break;
                        case ControllerActions.Start:
                            State = GameStates.Game;
                            StartLevel();
                            break;
                    }
                    break;
                case GameStates.Exit:
                    //nothing to do
                    Environment.Exit(0);
                    break;
                case GameStates.Win:
                    if (action == ControllerActions.Start) {
                        if (currentLevel < 6) {
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
                    break;
                case GameStates.Lose:
                    switch (action) {
                        case ControllerActions.Start:
                            State = GameStates.Start;
                            Start();
                            break;
                        case ControllerActions.Exit:
                            State = GameStates.Exit;
                            break;
                    }
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
                        default:
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
        
        public void Start(int level = 1) {
            painter.DrawStart();
            currentLevel = level;
        }

        private void StartLevel() {
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
                            var intelegentEnemy =
                                new IntelligentEnemy(x, y, board,
                                    player); //todo player could not be found at that moment
                            board[x, y] = null;
                            enemies.Add(intelegentEnemy);
                            break;
                        case 'G':
                            var ghost = new Ghost(x, y, board, player); //todo player could not be found at that moment
                            board[x, y] = null;
                            enemies.Add(ghost);
                            break;

                        case 'P':
                            if (player == null) {
                                player = new Player(x, y, board, 3);
                            }
                            else {
                                player.SetRespawnLocation(x, y, board);
                                // throw new Exception("ERROR: Игррок уже существует.");
                            }

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