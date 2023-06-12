using System.Drawing;

namespace Ira.Game
{
    public class Ghost : IntelligentEnemy
    {
        public Ghost(int x, int y, GameBoard board, Player player) : base(x, y, board, player)
        {
        }

        protected override bool TryMoveTo(int x, int y, int[,] arr)
        {
            if (arr[x, y] <= EmptySellCode && arr[x, y] != PlayerCode) return false;
            if (board[x, y] != null && !(board[x, y] is CrumblingWall)) return false;
            if (x < 0 || x >= board.Width || y < 0 || y >= board.Height) return false;
            Position = new Point(x, y);
            return true;
        }

        protected override int[,] GetArrayCopy()
        {
            var result = new int[board.Width, board.Height];
            for (var y = 0; y < board.Height; y++)
            {
                for (var x = 0; x < board.Width; x++)
                {
                    result[x, y] = EmptySellCode;
                }
            }

            return result;
        }
    }
}