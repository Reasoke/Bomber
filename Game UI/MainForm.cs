using System.Drawing;
using System.Windows.Forms;

namespace Ira.Game
{
    public partial class MainForm : Form
    {
        private readonly GameEngine game;

        public MainForm()
        {
            InitializeComponent();
            KeyPreview = true;
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1024, 768);

            var painter = new Painter(pictureBox);
            game = new GameEngine(painter);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    game.ProcessAction(ControllerActions.Player1Up);
                    break;
                case Keys.Down:
                    game.ProcessAction(ControllerActions.Player1Down);
                    break;
                case Keys.Right:
                    game.ProcessAction(ControllerActions.Player1Right);
                    break;
                case Keys.Left:
                    game.ProcessAction(ControllerActions.Player1Left);
                    break;
                case Keys.Space:
                    game.ProcessAction(ControllerActions.Player1Bomb);
                    break;
                case Keys.Escape:
                    game.ProcessAction(ControllerActions.Player1Exit);
                    break;
                case Keys.Enter:
                    game.ProcessAction(ControllerActions.Player1Start);
                    break;

                case Keys.W:
                    game.ProcessAction(ControllerActions.Player2Up);
                    break;
                case Keys.S:
                    game.ProcessAction(ControllerActions.Player2Down);
                    break;
                case Keys.D:
                    game.ProcessAction(ControllerActions.Player2Right);
                    break;
                case Keys.A:
                    game.ProcessAction(ControllerActions.Player2Left);
                    break;
                case Keys.E:
                    game.ProcessAction(ControllerActions.Player2Bomb);
                    break;
                case Keys.Q:
                    game.ProcessAction(ControllerActions.Player2Exit);
                    break;
                case Keys.F:
                    game.ProcessAction(ControllerActions.Player2Start);
                    break;
                default:
                    game.ProcessAction(ControllerActions.Other);
                    break;
            }

            e.SuppressKeyPress = true;
        }
    }
}