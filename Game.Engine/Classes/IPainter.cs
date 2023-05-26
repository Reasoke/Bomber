using System.Collections.Generic;

namespace Ira.Game {
    public interface IPainter {
        void Clear();
        void DrawBoard(GameBoard board, Player player, List<Enemy> enemies);
        void DrawStart(int selectedItem);
        void DrawLose();
        void DrawWin();
        void DrawShop(int selectedItem, Player player);
    }
}