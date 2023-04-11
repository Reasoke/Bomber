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

            while(level < 6) {
                Read_Data(level);
                painter.Clear();
                painter.DrawBoard(board, player, enemies);//startScreen
                Console.Write("Нажмите Enter для начала игры: ");
                Console.ReadLine();
                if (!PlayTheGame())
                    break;
                painter.DrawWinScreen();
                level++;
            }
            painter.DrawDieScreen();
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
                board.ProcessElements(player, enemies);
                foreach (var e in enemies) {
                    e.Move();
                }
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
        
        private void Read_Data(int level) {
            var fileName = $".\\media\\Level_{level}.txt";
            var fileLines = File.ReadAllLines(fileName);
            var height = fileLines.Length;
            var width = Get_Max_Length(fileLines);

            board = new GameBoard(width, height);
            for (int y = 0; y < height; y++) { // перебираем все файловые строки
                var line = fileLines[y];
                for (int x = 0; x < line.Length; x++) { //перебираем все символы строки
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
                            board[x, y]= finish = new Finish();
                            break;

                        case 'E':
                            var enemy = new Enemy(x, y, board);
                            board[x, y] = null;
                            enemies.Add(enemy);
                            break;

                        case 'P':
                            if (player == null) {
                                player = new Player(x, y, board, 3);
                            }
                            else {
                                player.SetRespawnLocation(x,y,board);
                                // throw new Exception("ERROR: Игррок уже существует.");
                            }

                            board[x, y] = null;
                            break;
                    }
                }
            }
        }

        private int Get_Max_Length(string[] lines) {
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
