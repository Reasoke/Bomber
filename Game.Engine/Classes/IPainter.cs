using System.Collections.Generic;

namespace Ira.Game
{
    public interface IPainter
    {
        void Clear();
        void DrawBoard(GameBoard board, Player player1, Player player2, List<Enemy> enemies, bool player1Move);
        void DrawStart(int selectedItem);
        void DrawLose();
        void DrawWin();
        void DrawShop(int selectedItem, Player player);
        void DrawMessageScreen(string text);
    }
}