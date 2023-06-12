using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Ira.Game
{
    public partial class MainForm : Form
    {
        private readonly GameEngine game;
        Dictionary<Keys, ControllerAction> keyEvents = new Dictionary<Keys, ControllerAction>
        {
            {Keys.Up, ControllerAction.Player1Up},
            {Keys.Down, ControllerAction.Player1Down},
            {Keys.Right, ControllerAction.Player1Right},
            {Keys.Left, ControllerAction.Player1Left},
            {Keys.Space, ControllerAction.Player1Bomb},
            {Keys.Escape, ControllerAction.Player1Exit},
            {Keys.Enter, ControllerAction.Player1Start},
            {Keys.W, ControllerAction.Player2Up},
            {Keys.S, ControllerAction.Player2Down},
            {Keys.D, ControllerAction.Player2Right},
            {Keys.A, ControllerAction.Player2Left},
            {Keys.E, ControllerAction.Player2Bomb},
            {Keys.Q, ControllerAction.Player2Exit},
            {Keys.F, ControllerAction.Player2Start},
        };

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
            if(keyEvents.TryGetValue(e.KeyCode, out var action))
                game.ProcessAction(action);
            
            // if(keyEvents.ContainsKey(e.KeyCode))
            //     game.ProcessAction(keyEvents[e.KeyCode]);
            
            e.SuppressKeyPress = true;
        }
    }
}