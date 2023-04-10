using System.Collections.Generic;

namespace Ira.Game {
  public interface IPainter {
    void DrawBoard(GameBoard board, Player player, List<Enemy> enemies);
  }
}