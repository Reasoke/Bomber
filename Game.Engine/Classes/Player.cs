using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Ira.Game
{
    public class Player : Character
    {
        private Point respawnPoint;

        public int BombPower { get; private set; }
        public int BombsLimit { get; private set; }
        public int BombsUsed { get; set; }
        public MoveDirection MoveDirection { get; set; }

        public bool IsWinner { get; set; }

        public Player(int x, int y, GameBoard board, int lives) : base(x, y, board, lives)
        {
            LivesCount = lives;
            SetRespawnLocation(x, y, board);
            SetDefaults();
        }

        public void AddLife()
        {
            LivesCount++;
        }

        public void AddBomb(int count = 1)
        {
            BombsLimit += count;
        }

        public void AddBombPower()
        {
            BombPower++;
        }


        public void SetRespawnLocation(int x, int y, GameBoard board)
        {
            respawnPoint.X = x;
            respawnPoint.Y = y;
            Position = new Point(x, y);
            base.board = board;
        }

        private void SetDefaults()
        {
            BombsLimit = 1;
            BombPower = 2;
            BombsUsed = 0;

            Position = new Point(respawnPoint.X, respawnPoint.Y);
        }

        protected override void PositionChanged()
        {
            base.PositionChanged();
            Utils.PlaySoundMove();
        }

        public void Move()
        {
            switch (MoveDirection)
            {
                case MoveDirection.None:
                    //stay here
                    break;
                case MoveDirection.Up:
                    TryMoveTo(Position.X, Position.Y - 1);
                    break;
                case MoveDirection.Right:
                    TryMoveTo(Position.X + 1, Position.Y);
                    break;
                case MoveDirection.Down:
                    TryMoveTo(Position.X, Position.Y + 1);
                    break;
                case MoveDirection.Left:
                    TryMoveTo(Position.X - 1, Position.Y);
                    break;
            }

            MoveDirection = MoveDirection.None;
        }

        public void SetTheBomb(int ticks)
        {
            if (BombsUsed >= BombsLimit) return;
            if (board.AddTheBomb(Position.X, Position.Y, BombPower, ticks))
            {
                BombsUsed++;
            }
        }

        public void InteractWithBoard(List<Enemy> enemies)
        {
            switch (board[Position.X, Position.Y])
            {
                case Coins c:
                {
                    this.Score++;
                    board[Position.X, Position.Y] = null;
                    break;
                }
                case BombCountBonus c:
                {
                    this.BombsLimit++;
                    board[Position.X, Position.Y] = null;
                    break;
                }
                case BombPowerBonus c:
                {
                    this.BombPower++;
                    board[Position.X, Position.Y] = null;
                    break;
                }
                case Armor a:
                {
                    this.IsProtected = true;
                    board[Position.X, Position.Y] = null;
                    break;
                }
                case Finish f:
                {
                    if (f.ExitMode)
                    {
                        IsWinner = true;
                    }

                    break;
                }
                case Fire f:
                {
                    if (!IsProtected) this.Die();
                    break;
                }
                case Trap t:
                {
                    if (!IsProtected) this.Die();
                    break;
                }
            }

            if (enemies.Any(e =>
                    (e.Position.X == Position.X && e.Position.Y == Position.Y) ||
                    (e.PreviousPosition.X == Position.X && e.PreviousPosition.Y == Position.Y)))
            {
                if (IsProtected)
                {
                    IsProtected = false;
                }
                else
                {
                    Die();
                }
            }
        }

        public bool Die()
        {
            LivesCount--;
            Utils.PlaySoundKill();
            if (IsAlive)
            {
                SetDefaults();
                return false;
            }

            return true;
        }
    }
}