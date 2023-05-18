using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Ira.Game {
    public class IntelligentEnemy : Enemy {
        private readonly Player player;

        public IntelligentEnemy(int x, int y, GameBoard board, Player player) : base(x, y, board, 0) {
            this.player = player;
        }

        protected override void InternalMove() {
            var arr = GetArrayCopy(board, board.Width, board.Height);
            arr[player.Position.X, player.Position.Y] = -1;
            var input = new List<Point> {new Point(player.Position.X, player.Position.Y)};
            CalcNextStep(arr, board.Width, board.Height, input, 1, Position);

            if (IsValidMove(Position.X - 1, Position.Y, arr) ||
                IsValidMove(Position.X + 1, Position.Y, arr) || 
                IsValidMove(Position.X, Position.Y - 1, arr) || 
                IsValidMove(Position.X, Position.Y + 1, arr)) {
                //moved
            }
        }

        protected virtual bool IsValidMove(int x, int y, int[,] arr) {
            return (arr[x, y] > 0 || arr[x, y] == -1) && TryMoveTo(x, y);
        }

        private static void CheckPoint(int x, int y, int[,] array, int width, int height, List<Point> points, int step) {
            if (x < 0 || y < 0 || x >= width || y >= height || array[x, y] != 0) return;
            array[x, y] = step;
            points.Add(new Point(x, y));
        }
        
        private static void CalcNextStep(int[,] array, int width, int height, List<Point> input, int step, Point target) {
            var points = new List<Point>();

            foreach (var point in input) {
                CheckPoint(point.X, point.Y - 1, array, width, height, points, step);
                CheckPoint(point.X, point.Y + 1, array, width, height, points, step);
                CheckPoint(point.X - 1, point.Y, array, width, height, points, step);
                CheckPoint(point.X + 1, point.Y, array, width, height, points, step);
            }

            if (points.Count == 0)
                return; // выхода нет
            if (points.Any(p => p.X == target.X && p.Y == target.Y))
                return; // выход есть

            CalcNextStep(array, width, height, points, step + 1, target);
        }

        protected virtual int[,] GetArrayCopy(GameBoard board, int width, int height) {
            var result = new int[width, height];
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    result[x, y] = board[x, y] == null || !board[x, y].IsBarrier ? 0 : -2;
                }
            }

            return result;
        }
    }
}