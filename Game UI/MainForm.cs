using System;
using System.Windows.Forms;

namespace Ira.Game {
    public partial class MainForm : Form {
        GameEngine game;
        Painter painter;
        
        public MainForm() {
            InitializeComponent();
            
            painter = new Painter(pictureBox);
            game = new GameEngine(painter);
            game.StartNew();
        }
    }
}