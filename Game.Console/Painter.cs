using System;
using System.Collections.Generic;
using System.Linq;

namespace Ira.Game {
  
  public class Painter : IPainter {
    
    private const string Title = "Bomberfox";

    public void DrawBoard(GameBoard board, Player player, List<Enemy> enemies) {
      Console.Clear();
      Console.Title = $"{Title} (player position: {player.X} - {player.Y}) Player Score: {player.Score} Player Lives: {player.LivesCount}";

      for (var y = 0; y < board.Height; y++) {
        ColorConsole.Write("\t");
        for (var x = 0; x < board.Width; x++) {
          switch (board[x, y]) {
            case PermanentWall el: {
              var wUp = y - 1 >= 0 &&
                        board[x, y - 1] is PermanentWall; // array[x, y - 1] == Wall - сравнение
              var wDown = y + 1 < board.Height && board[x, y + 1] is PermanentWall;
              var wLeft = x - 1 >= 0 && board[x - 1, y] is PermanentWall;
              var wRight = x + 1 < board.Width && board[x + 1, y] is PermanentWall;


              if (wUp && wDown && wLeft && wRight) {
                ColorConsole.Write("┼");
              }
              else if (wUp && wDown && wLeft) {
                ColorConsole.Write("┤");
              }
              else if (wUp && wDown && wRight) {
                ColorConsole.Write("├");
              }
              else if (wUp && wDown) {
                ColorConsole.Write("│");
              }
              else if (wUp && wLeft && wRight) {
                ColorConsole.Write("┴");
              }
              else if (wUp && wLeft) {
                ColorConsole.Write("┘");
              }
              else if (wUp && wRight) {
                ColorConsole.Write("└");
              }
              else if (wUp) {
                ColorConsole.Write("│");
              }
              else if (wDown && wLeft && wRight) {
                ColorConsole.Write("┬");
              }
              else if (wDown && wLeft) {
                ColorConsole.Write("┐");
              }
              else if (wDown && wRight) {
                ColorConsole.Write("┌");
              }
              else if (wDown) {
                ColorConsole.Write("│");
              }
              else if (wLeft && wRight) {
                ColorConsole.Write("─");
              }
              else if (wLeft) {
                ColorConsole.Write("─");
              }
              else if (wRight) {
                ColorConsole.Write("─");
              }
              else {
                ColorConsole.Write("■");
              }

              //output = "┐ └ ┘ ┌ ┴ ┬ ┤ ├ │ ─ ┼ ■ ° ∙ ░";
              break;
            }

            case CrumblingWall el:
              ColorConsole.Write("░", ConsoleColor.DarkCyan);
              break;

            case Coins c:
              ColorConsole.Write("@", ConsoleColor.Yellow);
              break;
            default:
              if (enemies.Any(e => e.X == x && e.Y == y)) {
                ColorConsole.Write("E", ConsoleColor.Red);
              }
              else if (x == player.X && y == player.Y) {
                ColorConsole.Write("P", ConsoleColor.Blue);
              }
              else if (board[x, y] is Finish) {
                ColorConsole.Write("F", ConsoleColor.Cyan);
              }
              else {
                ColorConsole.Write(" ");
              }

              break;
          }
        }

        ColorConsole.WriteLine();
      }

      ColorConsole.WriteLine();
    }
  }
}