using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Ira.Game
{
    public class IntelligentEnemy : Enemy
    {
        protected const int EmptySellCode = 0;
        protected const int PlayerCode = -1;
        protected const int BarrierCode = -2;
        
        private readonly Player player;

        public IntelligentEnemy(int x, int y, GameBoard board, Player player) : base(x, y, board, 0)
        {
            this.player = player;
        }

        public override void Move()
        {
            var arr = GetArrayCopy();
            arr[player.Position.X, player.Position.Y] = PlayerCode;
            var input = new List<Point> {new Point(player.Position.X, player.Position.Y)};
            CalcNextStep(input, 1);

            var moved = TryMoveTo(Position.X - 1, Position.Y, arr) ||
                        TryMoveTo(Position.X + 1, Position.Y, arr) ||
                        TryMoveTo(Position.X, Position.Y - 1, arr) ||
                        TryMoveTo(Position.X, Position.Y + 1, arr);



            void CalcNextStep(List<Point> previousWavePoints, int step)
            {
                var nextWavePoints = new List<Point>();

                foreach (var point in previousWavePoints)
                {
                    ProcessNeighbour(point.X, point.Y - 1);
                    ProcessNeighbour(point.X, point.Y + 1);
                    ProcessNeighbour(point.X - 1, point.Y);
                    ProcessNeighbour(point.X + 1, point.Y);
                }

                if (nextWavePoints.Count == 0)
                    return; // выхода нет

                var findFinish = FindAny(nextWavePoints, p => p.X == Position.X && p.Y == Position.Y);
                if (findFinish)
                {
                    return;
                }

                //Better to use Any, but it was a task not to do it
                // if (nextWavePoints.Any(p => p.X == Position.X && p.Y == Position.Y))
                //     return; // выход есть

                CalcNextStep(nextWavePoints, step + 1);

                void ProcessNeighbour(int x, int y)
                {
                    if (x < 0 || y < 0 || x >= board.Width || y >= board.Height || arr[x, y] != EmptySellCode) return;
                    arr[x, y] = step;
                    nextWavePoints.Add(new Point(x, y));
                }
            }
        }

        // private delegate bool Function(Point p);
        // private bool FindAny(List<Point> points, Function f)
        
        private bool FindAny(List<Point> points, Func<Point, bool> CheckFinish)
        {
            foreach (var point in points)
            {
                if (CheckFinish(point))
                {
                    return true;
                }
            }
            return false;
        }
        
        protected virtual bool TryMoveTo(int x, int y, int[,] arr)
        {
            return (arr[x, y] > EmptySellCode || arr[x, y] == PlayerCode) && base.TryMoveTo(x, y);
        }

        protected virtual int[,] GetArrayCopy()
        {
            var result = new int[board.Width, board.Height];
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    result[x, y] = board[x, y] == null || !board[x, y].IsBarrier ? EmptySellCode : BarrierCode;
                }
            }

            return result;
        }
    }
}