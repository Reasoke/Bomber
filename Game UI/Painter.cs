using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Ira.Game {
    public class Painter: IPainter {

        private readonly PictureBox pictureBox;
        private readonly Image playerImage;
        private readonly Image enemyImage;
        private readonly Image enemy2Image;
        private readonly Image ghostImage;
        private readonly Image wallImage;
        private readonly Image wall2Image;
        private readonly Image coinImage;
        private readonly Image bombImage;
        private readonly Image exitImage;
        private readonly Image exitLockedImage;
        private readonly Image armorImage;
        private readonly Image fireImage;
        private readonly Image trapImage;
        private readonly Image bombCountImage;
        private readonly Image bombPowerImage;
        private readonly Image selectorImage;
        private readonly Image shopSelectorImage;

        public Painter(PictureBox pictureBox) {
            this.pictureBox = pictureBox;
            playerImage =  Image.FromFile("./media/player.png");
            enemyImage =  Image.FromFile("./media/enemy.png");
            enemy2Image =  Image.FromFile("./media/enemy2.png");
            ghostImage =  Image.FromFile("./media/ghost.png");
            wallImage =  Image.FromFile("./media/wall.jpg");
            wall2Image =  Image.FromFile("./media/wall2.jpg");
            coinImage =  Image.FromFile("./media/coin.png");
            bombImage =  Image.FromFile("./media/bomb.png");
            exitImage =  Image.FromFile("./media/exit.png");
            exitLockedImage =  Image.FromFile("./media/exitLocked.png");
            armorImage =  Image.FromFile("./media/armor.png");
            fireImage =  Image.FromFile("./media/fire.png");
            trapImage =  Image.FromFile("./media/trap.png");
            bombCountImage =  Image.FromFile("./media/bombCount.png");
            bombPowerImage =  Image.FromFile("./media/bombPower.png");
            selectorImage =  Image.FromFile("./media/selector.png");
            shopSelectorImage =  Image.FromFile("./media/shopSelector.png");
        }
        
        private void DrawEnemy(Graphics g, Rectangle rect, Enemy e) {
            if (e.IsProtected) 
                g.FillRectangle(new SolidBrush(Color.Magenta), rect);

            switch (e) {
                case Ghost _:
                    DrawImageStretched(g, ghostImage, rect);
                    break;
                case IntelligentEnemy _:
                    DrawImageStretched(g, enemy2Image, rect);
                    break;
                default:
                    DrawImageStretched(g, enemyImage, rect);
                    var enemySymbol = e.Smartness == 1 ? "S" : "E";
                    g.DrawString(enemySymbol, new Font("Tahoma", 14), Brushes.Black, rect);
                    break;
            }
        }

        private static void DrawImageStretched(Graphics g, Image image, Rectangle rect) {
            g.DrawImage(image, rect, new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
        }
        
        public void DrawBoard(GameBoard board, Player player, List<Enemy> enemies) {
            const int cellHeight = 100;
            const int cellWidth = 100;
            const int scorePanelHeight = 60;
            var image = new Bitmap(board.Width * cellWidth, board.Height * cellHeight + scorePanelHeight);
            var score = $"Pomb limit: {player.BombsLimit}; Pomb power: {player.BombPower}; Score: {player.Score}; Lives: {player.LivesCount}; Has armor: {player.IsProtected}";
            using (var g = Graphics.FromImage(image)) {
                g.Clear(Color.Gainsboro);
                g.DrawString(score, new Font("Tahoma", 20), Brushes.Black, 0, board.Height * cellHeight + 20);
                for (var y = 0; y < board.Height; y++) {
                    for (var x = 0; x < board.Width; x++) {
                        var item = board[x, y];
                        var rect = new Rectangle(x * cellWidth, y * cellHeight, cellWidth, cellHeight);
                        switch (item) {
                            case PermanentWall _:
                                DrawImageStretched(g, wallImage, rect);
                                break;
                            case CrumblingWall _:
                                DrawImageStretched(g, wall2Image, rect);
                                break;

                            case Coins _:
                                DrawImageStretched(g, coinImage, rect);
                                break;
                            case Bomb _:
                                DrawImageStretched(g, bombImage, rect);
                                break;
                            case Fire _:
                                DrawImageStretched(g, fireImage, rect);
                                break;
                            case Trap _:
                                DrawImageStretched(g, trapImage, rect);
                                break;
                            case Armor _:
                                DrawImageStretched(g, armorImage, rect);
                                break;
                            case BombPowerBonus _:
                                DrawImageStretched(g, bombPowerImage, rect);
                                break;
                            case BombCountBonus _:
                                DrawImageStretched(g, bombCountImage, rect);
                                break;
                            case Finish f:
                                DrawImageStretched(g, !f.ExitMode ? exitLockedImage : exitImage, rect);
                                break;
                        }
                    }
                }

                foreach (var enemy in enemies) {
                    DrawEnemy(g, new Rectangle(enemy.Position.X * cellWidth, enemy.Position.Y * cellHeight, 
                        cellWidth, cellHeight), enemy);
                }

                var pRect = new Rectangle(player.Position.X * cellWidth, player.Position.Y * cellHeight, cellWidth,
                    cellHeight);
                if (player.IsProtected) 
                    g.FillRectangle(new SolidBrush(Color.Magenta), pRect);
                DrawImageStretched(g, playerImage, pRect);
            }

            pictureBox.Image = image;
        }

        public void DrawStart(int selectedItem) {
            // pictureBox.ImageLocation = "./media/start.png";
            var image = new Bitmap("./media/start.png");
            using (var g = Graphics.FromImage(image)) {
                DrawImageStretched(g, selectorImage,
                    new Rectangle(825, 585 + 130 * selectedItem, selectorImage.Width, selectorImage.Height));
            }
            pictureBox.Image = image;
        }

        public void DrawLose() {
            pictureBox.ImageLocation = "./media/die.png";
        }

        public void DrawWin() {
            pictureBox.ImageLocation = "./media/win.png";
        }

        public void DrawShop(int selectedItem, Player player) {
            var image = new Bitmap("./media/shop.png");
            var score = $"Pomb limit: {player.BombsLimit}; Pomb power: {player.BombPower}; Score: {player.Score}; Lives: {player.LivesCount}; Has armor: {player.IsProtected}";
            using (var g = Graphics.FromImage(image)) {
                g.DrawString(score, new Font("Tahoma", 20), Brushes.Black, 10, image.Height - 50);
                DrawImageStretched(g, shopSelectorImage,
                    new Rectangle(905, 205 + 145 * selectedItem, shopSelectorImage.Width, shopSelectorImage.Height));
            }
            pictureBox.Image = image;
        }

        public void Clear() {
            // Redraw();
        }
    }
}