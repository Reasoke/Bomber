using System.Collections.Generic;

namespace Ira.Game {
  public interface IPainter {
    void Clear();
    void DrawBoard(GameBoard board, Player player, List<Enemy> enemies);
    void DrawStartScreen();
    void DrawWinScreen();
    void DrawDieScreen();
  }
}