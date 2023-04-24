using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Ira.Game {
  public class Player : Character {
    private Point respawnPoint;

    public int BombPower { get; private set; }
    public int BombsLimit { get; private set; }
    public int BombsUsed { get; set; }
    public MoveDirection MoveDirection { get; set; }

    public bool IsWinner { get; set; }

    public Player(int x, int y, GameBoard board, int lives) : base(x, y, board, lives) {
      LivesCount = lives;
      BombsUsed = 0;
      SetRespawnLocation(x,y, board);
      SetDefaults();
    }

    public void SetRespawnLocation(int x, int y, GameBoard board) {
      respawnPoint.X = x;
      respawnPoint.Y = y;
      X = x;
      Y = y;
      base.board = board;
    }
    
    private void SetDefaults() {
      BombsLimit = 1;
      BombPower = 2;
      X = respawnPoint.X;
      Y = respawnPoint.Y;
    }

    protected override void InternalMove() {
      switch (MoveDirection) {
        case MoveDirection.None:
          //stay here
          break;
        case MoveDirection.Up:
          if (CanMoveTo(X, Y - 1)) {
            this.Y--;
            Utils.Play_Sound_Move();
          }

          break;
        case MoveDirection.Right:
          if (CanMoveTo(X + 1, Y)) {
            this.X++;
            Utils.Play_Sound_Move();
          }

          break;
        case MoveDirection.Down:
          if (CanMoveTo(X, Y + 1)) {
            this.Y++;
            Utils.Play_Sound_Move();
          }

          break;
        case MoveDirection.Left:
          if (CanMoveTo(X - 1, Y)) {
            this.X--;
            Utils.Play_Sound_Move();
          }
          break;
      }

      MoveDirection = MoveDirection.None;
    }

    public void SetTheBomb() {
      if (BombsUsed >= BombsLimit) return;
      if (board.AddTheBomb(X, Y, BombPower)) {
        BombsUsed++;
      }
    }

    public void InteractWithBoard(List<Enemy> enemies){
      switch (board[X, Y]) {
        case Coins c: {
          this.Score++;
          board[X, Y] = null;
          break;
        }
        case BombCountBonus c: {
          this.BombsLimit++;
          board[X, Y] = null;
          break;
        }
        case BombPowerBonus c: {
          this.BombPower++;
          board[X, Y] = null;
          break;
        }
        case Armor a: {
          this.IsProtected = true;
          board[X, Y] = null;
          break;
        }
        case Finish f: {
          if (f.ExitMode) {
            IsWinner = true;
          }
          break;
        }
        case Fire f: {
          if(!IsProtected) this.Die();
          break;
        }
        case Trap t: {
          if(!IsProtected) this.Die();
          break;
        }
      }
      
      if (enemies.Any(e => (e.X == X && e.Y == Y) || (e.previousPosition.X == X && e.previousPosition.Y == Y))) {
        if (IsProtected) {
          IsProtected = false;
        }
        else {
          Die();
        }

      }

    }

    public bool Die() {
      LivesCount--;
      Utils.Play_Sound_Kill();
      if (IsAlive) {
        SetDefaults();
        return false;
      }
      return true;
    }
  }
}