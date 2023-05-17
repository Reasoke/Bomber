using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Ira.Game {
    public class Painter: IPainter {

        private readonly PictureBox pictureBox;

        public Painter(PictureBox pictureBox) {
            this.pictureBox = pictureBox;
        }
        private static void DrawWall(Bitmap image, Rectangle rect, bool isBreakable) {
            var color = Color.Black;
            using (Graphics g = Graphics.FromImage(image)) {
                if (isBreakable) {
                    color = Color.Khaki;
                }
                g.FillRectangle(new SolidBrush(color), rect);
            }
        }
        
        private static void DrawEmpty(Bitmap image, Rectangle rect) {
            using (Graphics g = Graphics.FromImage(image)) {
                g.FillRectangle(new SolidBrush(Color.Gainsboro), rect);
            }
        }
        
        private static void DrawFinish(Bitmap image, Rectangle rect, bool exitMode) {
            var color = Color.GreenYellow;
            using (Graphics g = Graphics.FromImage(image)) {
                if (!exitMode) {
                    color = Color.RosyBrown;
                }
                g.FillRectangle(new SolidBrush(color), rect);
                g.DrawString("Exit", new Font("Tahoma", 14), Brushes.Black, rect);
            }
        }
        
        private static void DrawPlayer(Bitmap image, Rectangle rect) {
            using (Graphics g = Graphics.FromImage(image)) {
                g.FillRectangle(new SolidBrush(Color.Blue), rect);
                g.DrawString("Player", new Font("Tahoma", 14), Brushes.Beige, rect);
            }
        }
        
        private static void DrawEnemy(Bitmap image, Rectangle rect, Enemy e) {
            var color = Color.DarkMagenta;
            using (Graphics g = Graphics.FromImage(image)) {
                if (e.IsProtected) {
                    color = Color.Magenta;
                }
                g.FillRectangle(new SolidBrush(color), rect);
                
                var enemySymbol = e is Ghost ? "G" :  e is IntelligentEnemy? "I" : e.Smartness == 1 ? "S" : "E";
                g.DrawString(enemySymbol, new Font("Tahoma", 14), Brushes.Black, rect);
            }
        }
        
        private static void DrawElement(Bitmap image, Rectangle rect, BaseElement element) {
            var color = Color.Gainsboro;
            var symbol = "";
            using (Graphics g = Graphics.FromImage(image)) {
                switch (element) {
                    case Coins e:
                        color = Color.Gold;
                        symbol = "C";
                        break;
                    case Bomb e:
                        color = Color.Brown;
                        break;
                    case Fire e:
                        color = Color.Red;
                        break;
                    case Trap e:
                        color = Color.Aqua;
                        break;
                    case Armor e:
                        color = Color.Magenta;
                        symbol = "A";
                        break;
                    case BombPowerBonus e:
                        color = Color.Orange;
                        symbol = "BP";
                        break;
                    case BombCountBonus e:
                        color = Color.Plum;
                        symbol = "BC";
                        break;
                }
                g.FillRectangle(new SolidBrush(color), rect);
                if(!string.IsNullOrEmpty(symbol))
                    g.DrawString(symbol, new Font("Tahoma", 14), Brushes.Black, rect);
            }
        }
        
        public void DrawBoard(GameBoard board, Player player, List<Enemy> enemies) {
            var cellHeight = 100;
            var cellWidth = 100;
            var image = new Bitmap(board.Width * cellWidth, board.Height * cellHeight);
            // Console.Title = $"{Title} (player position: {player.Position.X} - {player.Position.Y}) Player Score: {player.Score} Player Lives: {player.LivesCount}";

            for (var y = 0; y < board.Height; y++) {
                for (var x = 0; x < board.Width; x++) {
                    var item = board[x, y];
                    switch (item) {
                        case PermanentWall el: {
                            DrawWall(image, new Rectangle(x*cellWidth, y* cellHeight, cellWidth, cellHeight), false);
                            break;
                        }
                        case CrumblingWall el:
                            DrawWall(image, new Rectangle(x*cellWidth, y* cellHeight, cellWidth, cellHeight), true);
                            break;

                        case Coins c:
                            DrawElement(image, new Rectangle(x*cellWidth, y* cellHeight, cellWidth, cellHeight), item);
                            break;
                        case Bomb b:
                            DrawElement(image, new Rectangle(x*cellWidth, y* cellHeight, cellWidth, cellHeight), item);
                            break;
                        case Fire f:
                            DrawElement(image, new Rectangle(x*cellWidth, y* cellHeight, cellWidth, cellHeight), item);
                            break;
                        case Trap t:
                            DrawElement(image, new Rectangle(x*cellWidth, y* cellHeight, cellWidth, cellHeight), item);
                            break;
                        case Armor a:
                            DrawElement(image, new Rectangle(x*cellWidth, y* cellHeight, cellWidth, cellHeight), item);
                            break;
                        case BombPowerBonus p:
                            DrawElement(image, new Rectangle(x*cellWidth, y* cellHeight, cellWidth, cellHeight), item);
                            break;
                        case BombCountBonus c:
                            DrawElement(image, new Rectangle(x*cellWidth, y* cellHeight, cellWidth, cellHeight), item);
                            break;
                        
                        case Finish f:
                            DrawFinish(image, new Rectangle(x*cellWidth, y* cellHeight, cellWidth, cellHeight), f.ExitMode);
                            break;

                        default:
                            DrawEmpty(image, new Rectangle(x*cellWidth, y* cellHeight, cellWidth, cellHeight));
                            break;
                    }
                }
            }

            foreach (var enemy in enemies) {
                // Console.SetCursorPosition(enemy.Position.X + 8, enemy.Position.Y);
                DrawEnemy(image, new Rectangle((enemy.Position.X)*cellWidth, enemy.Position.Y* cellHeight, cellWidth, cellHeight), enemy);
            }

            // Console.SetCursorPosition(player.Position.X + 8, player.Position.Y);
            // ColorConsole.Write("P", ConsoleColor.Blue);
            DrawPlayer(image, new Rectangle((player.Position.X)*cellWidth, player.Position.Y* cellHeight, cellWidth, cellHeight));
            pictureBox.Image = image;
        }

        public void DrawStart() {
            pictureBox.ImageLocation = "./media/start.png";
        }

        public void DrawLose() {
            pictureBox.ImageLocation = "./media/die.png";
        }

        public void DrawWin() {
            pictureBox.ImageLocation = "./media/win.png";
        }

        public void Clear() {
            // Redraw();
        }
        
    }
}