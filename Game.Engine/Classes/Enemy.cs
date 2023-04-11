using System;
using System.Drawing;

namespace Ira.Game {
  public class Enemy : Character {
    private MoveDirection moveDirection;
        
    public Point previousPosition;
        
    public Enemy(int x, int y, GameBoard board) : base(x, y, board) {
      previousPosition.X = x;
      previousPosition.Y = y;
      var rnd = new Random();
      var nextDir = rnd.Next(1, 4);
      moveDirection = (MoveDirection)nextDir;
    }

    public override void Move() {
      var moved = false;
      int tries = 0;
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

        if (CanMoveTo(nextX, nextY)) {
          X = nextX;
          Y = nextY;
          moved = true;
        }
        else {
          var rnd = new Random();
          var nextDir = rnd.Next(1, 4);
          moveDirection = (MoveDirection)nextDir;
          tries++;
          if(tries >= 4) {
            break;
          }
        }
      }
    }
  }
}