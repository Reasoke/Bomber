using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ira.Game {
    public class GameEngine {

        private string title = "Bomberfox";
        private Finish finish;
        private Player player;
        private readonly List<Enemy> enemies = new List<Enemy>();

        private GameBoard board;
        private int width;
        private int height;
        
        public void StartNew() {
            
            Read_Data();
            DrawBoard();
            
            Console.Title = $"{title} (player position: {player.X} - {player.Y})";
 
            Console.Write("Нажмите Enter для начала игры: ");
            Console.ReadLine();
            PlayTheGame();
        }

        private void PlayTheGame() {
            DrawBoard();
            Console.CursorVisible = false;
            while (true) {
                var moved = false;
                var key = Console.ReadKey(true);
                switch (key.Key) {
                    case ConsoleKey.UpArrow:
                        if (CanMoveTo(player.X, player.Y-1)) {
                            player.Move(MoveDirection.Up);
                            moved = true;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (CanMoveTo(player.X, player.Y+1)) {
                            player.Move(MoveDirection.Down);
                            moved = true;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (CanMoveTo(player.X+1, player.Y)) {
                            player.Move(MoveDirection.Right);
                            moved = true;
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (CanMoveTo(player.X-1, player.Y)) {
                            player.Move(MoveDirection.Left);
                            moved = true;
                        }
                        break;
                    case ConsoleKey.Escape:
                        return;
                }
                
                if (moved) {
                    Console.Title = $"{title} (player position: {player.X} - {player.Y})";
                }

                //todo: move enemies
                
                DrawBoard();
                
                while (Console.KeyAvailable) {
                    Console.ReadKey(true);
                }
            }
        }
        
        private bool CanMoveTo(int x, int y) {
            if (x >= 0 && x < width && y >= 0 && y < height && !board[x, y].IsBarrier)
                return true;
            return false;
        }
        
        private void Read_Data() {
            var fileName = ".\\media\\Input.txt";
            var fileLines = File.ReadAllLines(fileName);
            height = fileLines.Length;
            width = Get_Max_Length(fileLines);

            board = new GameBoard(width, height);
            for (int y = 0; y < height; y++) { // перебираем все файловые строки
                var line = fileLines[y];
                for (int x = 0; x < line.Length; x++) { //перебираем все символы строки
                    var symbol = line[x];
                    switch (symbol) {
                        case '#':
                            board[x, y] = new PermanentWall(x, y);
                            break;

                        case '@':
                            board[x, y] = new CrumblingWall(x, y);
                            break;

                        case ' ':
                            board[x, y] = new EmptyElement(x, y);
                            break;

                        case 'F':
                            if (finish != null) {
                                throw new Exception("ERROR: Выход уже существует.");
                            }
                            board[x, y] = finish = new Finish(x, y);
                            break;

                        case 'E':
                            var enemy = new Enemy(x, y);
                            board[x, y] = enemy;
                            enemies.Add(enemy);
                            break;

                        case 'P':
                            if (player != null) {
                                throw new Exception("ERROR: Игррок уже существует.");
                            }
                            board[x, y] = player = new Player(x, y, 3);
                            break;
                    }
                }
            }

            Console.Write(2);
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
        
        private void DrawBoard() {
            Console.Clear();

            for (int y = 0; y < height; y++) {
                Console.Write("\t");
                for (int x = 0; x < width; x++) {
                    string output;
                    switch (board[x, y]) {
                        case PermanentWall el:
                            var w_up = y - 1 >= 0 && board[x, y - 1] is PermanentWall; // array[x, y - 1] == Wall - сравнение
                            var w_down = y + 1 < height && board[x, y + 1] is PermanentWall;
                            var w_left = x - 1 >= 0 && board[x - 1, y] is PermanentWall;
                            var w_right = x + 1 < width && board[x + 1, y] is PermanentWall;
                            
                            
                            if (w_up && w_down && w_left && w_right) {
                                output = "┼"; 
                            }
                            else if (w_up && w_down && w_left) {
                                output = "┤";
                            }
                            else if (w_up && w_down && w_right) {
                                output = "├";
                            }
                            else if (w_up && w_down) {
                                output = "│";
                            }
                            else if (w_up && w_left && w_right) {
                                output = "┴";
                            }
                            else if (w_up && w_left) {
                                output = "┘";
                            }
                            else if (w_up && w_right) {
                                output = "└";
                            }
                            else if (w_up) {
                                output = "│";
                            }
                            else if (w_down && w_left && w_right) {
                                output = "┬";
                            }
                            else if ( w_down && w_left) {
                                output = "┐";
                            }
                            else if (w_down && w_right) {
                                output = "┌";
                            }
                            else if (w_down) {
                                output = "│";
                            }
                            else if (w_left && w_right) {
                                output = "─";
                            }
                            else if (w_left) {
                                output = "─";
                            }
                            else if (w_right) {
                                output = "─";
                            }
                            else {
                                output = "■";
                            }

                            //output = "┐ └ ┘ ┌ ┴ ┬ ┤ ├ │ ─ ┼ ■ ° ∙ ░";
                            break;
                        
                        case CrumblingWall el:
                            output = "░";  
                            break;
                        
                        case EmptyElement el:
                        default:
                            if (enemies.Any(e=>e.X == x && e.Y == y)) {
                                output = "E";
                            }
                            else if (x == player.X && y == player.Y) {
                                output = "P";
                            }else if (finish!= null && x == finish.X && y == finish.Y) {
                                output = "F";
                            }
                            else {
                                output = " ";
                            }
                            break;
                    }

                    Console.Write(output);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        
        
    }
}
