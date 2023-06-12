using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ira.Game
{
    internal static class BoardReader
    {
        //TODO consts for letters

        public static GameBoard ReadData(int level, List<Enemy> enemies, Player player, Player player2, ref Finish finish)
        {
            var fileName = $".\\media\\Level_{level}.txt";
            var fileLines = File.ReadAllLines(fileName);
            var height = fileLines.Length;
            var width = GetMaxLength(fileLines);

            var board = new GameBoard(width, height);
            enemies.Clear();
            for (int y = 0; y < height; y++)
            {
                // перебираем все файловые строки
                var line = fileLines[y];
                for (int x = 0; x < line.Length; x++)
                {
                    //перебираем все символы строки
                    var symbol = line[x];
                    switch (symbol)
                    {
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
                        case 'L':
                            board[x, y] = new Life();
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
                        case 'Q':
                            player2.SetRespawnLocation(x, y, board);
                            board[x, y] = null;
                            break;
                    }
                }
            }

            return board;
        }

        private static int GetMaxLength(string[] lines)
        {
            return lines == null || lines.Length == 0 ? 0 : lines.Max(l => l.Length);
        }
    }
}