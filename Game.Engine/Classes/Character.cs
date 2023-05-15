using System.Drawing;

namespace Ira.Game {
    public class Character : BaseElement {
        protected GameBoard board;
        public int Score = 0;
        private Point position;

        public Point Position {
            get => position;
            protected set {
                position = value;
                PositionChanged();
            }
        }

        protected virtual void PositionChanged() {
        }
        
        public int LivesCount { get; protected set; }

        public bool IsAlive {
            get { return LivesCount > 0; }
        }

        public Character(int x, int y, GameBoard board, int lives = 1) : base() {
            this.Position = new Point(x, y);
            this.LivesCount = lives;
            this.IsMovable = true;
            this.board = board;
        }

        protected bool TryMoveTo(int x, int y) {
            if (x < 0 || x >= board.Width || y < 0 || y >= board.Height ||
                (board[x, y] != null && board[x, y].IsBarrier)) return false;
            Position = new Point(x, y);
            return true;
        }
    }
}