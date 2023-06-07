using System;
using System.Collections.Generic;
using System.Drawing;

namespace Ira.Game
{
    public class Enemy : Character
    {
        private MoveDirection moveDirection;

        public readonly int Smartness;
        public Point PreviousPosition;

        public Enemy(int x, int y, GameBoard board, int smartness) : base(x, y, board)
        {
            Smartness = smartness;
            PreviousPosition.X = x;
            PreviousPosition.Y = y;
            var rnd = new Random();
            var nextDir = rnd.Next(1, 4);
            moveDirection = (MoveDirection) nextDir;
        }

        protected override void InternalMove()
        {
            var moved = false;
            int tries = 0;
            // var direction = ((MoveDirection[])Enum.GetValues(typeof(MoveDirection))).ToList();
            var direction = new List<MoveDirection>
            {
                MoveDirection.Up,
                MoveDirection.Right,
                MoveDirection.Down,
                MoveDirection.Left
            };
            while (!moved)
            {
                var nextX = Position.X;
                var nextY = Position.Y;
                switch (moveDirection)
                {
                    case MoveDirection.Up:
                        nextY = Position.Y - 1;
                        PreviousPosition.Y = Position.Y;
                        break;
                    case MoveDirection.Right:
                        nextX = Position.X + 1;
                        PreviousPosition.X = Position.X;
                        break;
                    case MoveDirection.Down:
                        nextY = Position.Y + 1;
                        PreviousPosition.Y = Position.Y;
                        break;
                    case MoveDirection.Left:
                        nextX = Position.X - 1;
                        PreviousPosition.X = Position.X;
                        break;
                }

                if (this.Smartness == 1 && board[Position.X, Position.Y] == null && Utils.GetRandome(1, 10) == 7)
                {
                    board[Position.X, Position.Y] = new Trap(board.StepNumber);
                }

                if (TryMoveTo(nextX, nextY))
                {
                    moved = true;
                }
                else
                {
                    tries = ChooseDirection(tries, direction);
                    if (tries == 4)
                    {
                        break;
                    }
                }
            }
        }

        public int ChooseDirection(int tries, List<MoveDirection> direction)
        {
            int nextDir;
            switch (Smartness)
            {
                case 0:
                    nextDir = Utils.GetRandome(0, 3);
                    moveDirection = direction[nextDir];
                    break;
                case 1:
                    nextDir = Utils.GetRandome(0, direction.Count);
                    moveDirection = direction[nextDir];
                    direction.RemoveAt(nextDir);
                    break;
                case 2:
                    break;
            }

            return ++tries;
        }

        public void InteractWithBoard()
        {
            switch (board[Position.X, Position.Y])
            {
                case Armor a:
                {
                    this.IsProtected = true;
                    board[Position.X, Position.Y] = null;
                    break;
                }
            }
        }
    }
}