using System.Windows.Forms;

namespace Ira.Game {
    public partial class MainForm : Form {
        private readonly GameEngine game;

        public MainForm() {
            InitializeComponent();
            KeyPreview = true;
            
            var painter = new Painter(pictureBox);
            game = new GameEngine(painter);
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Up:
                    game.ProcessAction(ControllerActions.Up);
                    break;
                case Keys.Down:
                    game.ProcessAction(ControllerActions.Down);
                    break;
                case Keys.Right:
                    game.ProcessAction(ControllerActions.Right);
                    break;
                case Keys.Left:
                    game.ProcessAction(ControllerActions.Left);
                    break;
                case Keys.Space:
                    game.ProcessAction(ControllerActions.Bomb);
                    break;
                case Keys.Escape:
                    game.ProcessAction(ControllerActions.Exit);
                    break;
                case Keys.Enter:
                    game.ProcessAction(ControllerActions.Start);
                    break;
                default:
                    game.ProcessAction(ControllerActions.Other);
                    break;
            }
        }
    }
}