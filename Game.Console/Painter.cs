using System;
using System.Collections.Generic;
using System.Linq;

namespace Ira.Game {
  
  public class Painter : IPainter {
    
    private const string Title = "Bomberfox";

    public Painter() {
      Console.CursorVisible = false;
    }

    public void Clear() {
      Console.Clear();
    }

    public void DrawBoard(GameBoard board, Player player, List<Enemy> enemies) {
      Console.SetCursorPosition(0,0);
      
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
              ColorConsole.Write("░", ConsoleColor.DarkYellow);
              break;

            case Coins c:
              ColorConsole.Write("@", ConsoleColor.Yellow);
              break;
            case Bomb b:
                ColorConsole.Write(b.Ticks.ToString(), ConsoleColor.White, ConsoleColor.Red);
              break;
            case Fire f:
                ColorConsole.Write(" ", ConsoleColor.White, ConsoleColor.Red);
              break;
            case Trap t:
                ColorConsole.Write("T", ConsoleColor.White, ConsoleColor.Red);
              break;
            
            case Armor a:
              ColorConsole.Write("A", ConsoleColor.Magenta);
              break;
            case BombPowerBonus p:
              ColorConsole.Write("!", ConsoleColor.Magenta);
              break;
            case BombCountBonus c:
              ColorConsole.Write("C", ConsoleColor.Magenta);
              break;

            
            
            default:
              //var enemy = enemies.FirstOrDefault(e => e.X == x && e.Y == y);
              if (enemies.Any(e => e.X == x && e.Y == y)) {
                ColorConsole.Write("E", ConsoleColor.DarkRed);
              }
              else if (x == player.X && y == player.Y) {
                ColorConsole.Write("P", ConsoleColor.Blue);
              }
              else if (board[x, y] is Finish f) {
                ColorConsole.Write("F", f.ExitMode ? ConsoleColor.Yellow : ConsoleColor.Green, f.ExitMode ? ConsoleColor.Blue : ConsoleColor.Black);
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

    public void DrawScreen(string[] fileLines) {
      var height = fileLines.Length;
      for (int y = 0; y < height; y++) { // перебираем все файловые строки
        var line = fileLines[y];
        ColorConsole.Write("\t");
        for (int x = 0; x < line.Length; x++) { //перебираем все символы строки
          Console.Write(line[x]);
        }
        ColorConsole.WriteLine();
      }
      ColorConsole.WriteLine();

    }
  }
}