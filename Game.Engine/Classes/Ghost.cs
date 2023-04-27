using System.Collections.Generic;
using System.Drawing;

namespace Ira.Game {
  public class Ghost : Enemy {
    private readonly Player player;
    private int[,] arr;

    public Point previousPosition;

    public Ghost(int x, int y, GameBoard board, Player player) : base(x, y, board, 0) {
      this.player = player;
      previousPosition.X = x;
      previousPosition.Y = y;
    }

    protected override void InternalMove() {
      arr = Get_Array_Copy(board, board.Width, board.Height);
      arr[player.X, player.Y] = -1;
      var input = new List<Point>();
      input.Add(new Point(player.X, player.Y));
      Point enemyPosition = new Point(X, Y);
      Calc_Next_Step(arr, board.Width, board.Height, input, 1, enemyPosition);

      if (CanMoveTo2(enemyPosition.X - 1, enemyPosition.Y, arr)) {
        X--;
      }
      else if (CanMoveTo2(enemyPosition.X + 1, enemyPosition.Y, arr)) {
        X++;
      }
      else if (CanMoveTo2(enemyPosition.X, enemyPosition.Y - 1, arr)) {
        Y--;
      }
      else if (CanMoveTo2(enemyPosition.X, enemyPosition.Y + 1, arr)) {
        Y++;
      }
    }

    public void InteractWithBoard() {
      switch (board[X, Y]) {
        case Armor a: {
          IsProtected = true;
          board[X, Y] = null;
          break;
        }
      }
    }

    private bool CanMoveTo2(int x, int y, int[,] arr) {
      if ((arr[x, y] > 0 || arr[x, y] == -1) && x >= 0 && x < board.Width && y >= 0 && y < board.Height && (board[x, y] == null || board[x, y] is CrumblingWall)) {
        return true;
      }

      return false;
    }

    static int Calc_Next_Step(int[,] array, int width, int height, List<Point> input, int step, Point target) {
      var result = new List<Point>();

      foreach (var point in input) {

        if (point.X == target.X && point.Y == target.Y) {
          // цель уже достигнута
          return step;
        }


        if (point.Y - 1 >= 0 && array[point.X, point.Y - 1] == 0) {
          array[point.X, point.Y - 1] = step;
          result.Add(new Point(point.X, point.Y - 1));
        }

        if (point.Y + 1 < height && array[point.X, point.Y + 1] == 0) {
          array[point.X, point.Y + 1] = step;
          result.Add(new Point(point.X, point.Y + 1));
        }

        if (point.X - 1 >= 0 && array[point.X - 1, point.Y] == 0) {
          array[point.X - 1, point.Y] = step;
          result.Add(new Point(point.X - 1, point.Y));
        }

        if (point.X + 1 < width && array[point.X + 1, point.Y] == 0) {
          array[point.X + 1, point.Y] = step;
          result.Add(new Point(point.X + 1, point.Y));
        }
      }

      if (result.Count == 0) {
        // выхода нет
        return -1;
      }

      foreach (var point in result) {
        if (point.X == target.X && point.Y == target.Y) {
          // выход есть
          return step;
        }
      }

      var exitValue = Calc_Next_Step(array, width, height, result, step + 1, target);
      return exitValue;
    }

    private static int[,] Get_Array_Copy(GameBoard board, int width, int height) {
      var result = new int[width, height];
      for (int y = 0; y < height; y++) {
        for (int x = 0; x < width; x++) {
          result[x, y] = 0;
        }
      }
      return result;
    }
  }
}