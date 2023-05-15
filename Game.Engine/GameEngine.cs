using System;
using System.Collections.Generic;
using System.IO;

namespace Ira.Game {
    public class GameEngine {
        private readonly IPainter painter;

        private Finish finish;
        private Player player;
        private readonly List<Enemy> enemies = new List<Enemy>();
        private GameBoard board;

        public GameEngine(IPainter painter) {
            this.painter = painter;
        }

        public void StartNew(int level = 1) {
            painter.Clear();
            PlayStartScreen();
            Console.Write("Нажмите Enter для начала игры: ");
            Console.ReadLine();
            while (level < 6) {
                Utils.PlayMainTheme();
                ReadData(level);
                var win = PlayTheGame();
                Utils.StopMainTheme();
                if (!win)
                    break;
                Utils.PlaySoundExit();
                PlayWinScreen();
                level++;
                Console.ReadLine();
            }

            PlayDieScreen();
        }

        private bool PlayTheGame() {
            painter.Clear();
            while (true) {
                painter.DrawBoard(board, player, enemies);

                //keyboard
                var key = Console.ReadKey(true);
                switch (key.Key) {
                    case ConsoleKey.UpArrow:
                        player.MoveDirection = MoveDirection.Up;
                        break;
                    case ConsoleKey.DownArrow:
                        player.MoveDirection = MoveDirection.Down;
                        break;
                    case ConsoleKey.RightArrow:
                        player.MoveDirection = MoveDirection.Right;
                        break;
                    case ConsoleKey.LeftArrow:
                        player.MoveDirection = MoveDirection.Left;
                        break;
                    case ConsoleKey.Spacebar:
                        player.SetTheBomb();
                        break;
                    case ConsoleKey.Escape:
                        return false;
                }

                //Clear keyboard buffer
                while (Console.KeyAvailable) {
                    Console.ReadKey(true);
                }

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


                if (!player.IsAlive) {
                    return false;
                }

                if (player.IsWinner) {
                    player.IsWinner = false;
                    return true;
                }
            }
        }

        private void PlayStartScreen() {
            painter.Clear();
            var fileName = ".\\media\\StartScreen.txt";
            string[] fileLines = File.ReadAllLines(fileName);
            painter.DrawScreen(fileLines);
        }

        private void PlayWinScreen() {
            painter.Clear();
            var fileName = ".\\media\\WinScreen.txt";
            string[] fileLines = File.ReadAllLines(fileName);
            painter.DrawScreen(fileLines);
        }

        private void PlayDieScreen() {
            painter.Clear();
            var fileName = ".\\media\\DieScreen.txt";
            string[] fileLines = File.ReadAllLines(fileName);
            painter.DrawScreen(fileLines);
        }

        private void ReadData(int level) {
            var fileName = $".\\media\\Level_{level}.txt";
            var fileLines = File.ReadAllLines(fileName);
            var height = fileLines.Length;
            var width = GetMaxLength(fileLines);

            board = new GameBoard(width, height);
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
            if (lines == null) {
                return 0;
            }

            var linesCount = lines.Length;
            if (linesCount == 0) {
                return 0;
            }

            var i = 0;
            var result = lines[i++].Length;
            while (i < linesCount) {
                var len = lines[i].Length;
                if (result < len) {
                    result = len;
                }

                i++;
            }

            return result;
        }
    }
}