using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Ira.Game {
  public class Enemy : Character {
    private MoveDirection moveDirection;

    public int Smartness;
    public Point previousPosition;

    public Enemy(int x, int y, GameBoard board, int smartness) : base(x, y, board) {
      Smartness = smartness;
      previousPosition.X = x;
      previousPosition.Y = y;
      var rnd = new Random();
      var nextDir = rnd.Next(1, 4);
      moveDirection = (MoveDirection) nextDir;
    }

    protected override void InternalMove() {
      var moved = false;
      int tries = 0;
      // var direction = ((MoveDirection[])Enum.GetValues(typeof(MoveDirection))).ToList();
      var direction = new List<MoveDirection> {
        MoveDirection.Up,
        MoveDirection.Right,
        MoveDirection.Down,
        MoveDirection.Left
      };
      while (!moved) {
        var nextX = X;
        var nextY = Y;
        switch (moveDirection) {
          case MoveDirection.Up:
            nextY = Y - 1;
            previousPosition.Y = Y;
            break;
          case MoveDirection.Right:
            nextX = X + 1;
            previousPosition.X = X;
            break;
          case MoveDirection.Down:
            nextY = Y + 1;
            previousPosition.Y = Y;
            break;
          case MoveDirection.Left:
            nextX = X - 1;
            previousPosition.X = X;
            break;
        }

        if (this.Smartness == 1 && board[X, Y] == null && Utils.GetRandome(1, 10) == 7) {
          board[X, Y] = new Trap(board.StepNumber);
        }

        if (CanMoveTo(nextX, nextY)) {
          X = nextX;
          Y = nextY;
          moved = true;
        }
        else {
          tries = ChooseDirection(tries, direction);
          if (tries == 4) {
            break;
          }
        }
      }


    }

    public int ChooseDirection(int tries, List<MoveDirection> direction) {
      int nextDir;
      switch (Smartness) {
        case 0:
          nextDir = Utils.GetRandome(0, 3);
          moveDirection = direction[nextDir];
          break;
        case 1:
          nextDir = Utils.GetRandome(0, direction.Count);
          moveDirection = direction[nextDir];
          direction.RemoveAt(nextDir);
          break;
      }
      return ++tries;
    }

    public void InteractWithBoard() {
      switch (board[X, Y]) {
        case Armor a: {
          this.IsProtected = true;
          board[X, Y] = null;
          break;
        }
      }
    }
  }
}