using System.Drawing;

namespace Ira.Game
{
    public class Ghost : IntelligentEnemy
    {
        public Ghost(int x, int y, GameBoard board, Player player) : base(x, y, board, player)
        {
        }

        protected override bool IsValidMove(int x, int y, int[,] arr)
        {
            if (arr[x, y] <= 0 && arr[x, y] != -1) return false;
            if (x < 0 || x >= board.Width || y < 0 || y >= board.Height) return false;
            if (board[x, y] != null && !(board[x, y] is CrumblingWall)) return false;
            Position = new Point(x, y);
            return true;
        }

        protected override int[,] GetArrayCopy(GameBoard board, int width, int height)
        {
            var result = new int[width, height];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    result[x, y] = 0;
                }
            }

            return result;
        }
    }
}