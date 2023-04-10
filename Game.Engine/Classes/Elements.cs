using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Ira.Game {
    
    public class GameBoard {
        public int Width;
        public int Height;
        public BaseElement[,] Map { get; }

        public GameBoard(int w, int h) {
            this.Width = w;
            this.Height = h;
            this.Map = new BaseElement[Width, Height];
        }

        public BaseElement this[int x, int y] {
            get { return Map[x, y]; }
            set { Map[x, y] = value; }
        }
    }

    public abstract class BaseElement {
        public bool IsMovable { get; protected set; }
        public bool IsBarrier { get; }

        public BaseElement(bool isBarrier = false) {
            this.IsBarrier = isBarrier;
            this.IsMovable = false;
        }

        public virtual void Move() {
            if (IsBarrier || !IsMovable) return;
        }
    }

    #region Character

    public class Character : BaseElement {
        protected readonly GameBoard board;
        public int Score = 0;

        public int X { get; protected set; }
        public int Y { get; protected set; }
        public int LivesCount { get; protected set; }
        public bool IsAlive {
            get {
                return LivesCount > 0;
            }
        }

        public Character(int x, int y, GameBoard board, int lives = 1) : base() {
            this.X = x;
            this.Y = y;
            this.LivesCount = lives;
            this.IsMovable = true;
            this.board = board;
        }

        protected bool CanMoveTo(int x, int y) {
            if (x >= 0 && x < board.Width && y >= 0 && y < board.Height && (board[x, y] == null || !board[x, y].IsBarrier)) {
                return true;
            }

            return false;
        }

    }

    public class Player : Character {
        private readonly Point respawnPoint;
        
        public int BombPower { get; private set; }
        public int BombsLimit { get; private set; }
        public int BombsUsed { get; private set; }
        public MoveDirection MoveDirection { get; set; }
        
        public Player(int x, int y, GameBoard board, int lives) : base(x, y, board, lives) {
            LivesCount = lives;
            BombsUsed = 0;
            respawnPoint.X = x;
            respawnPoint.Y = y;
            SetDefaults();
        }

        private void SetDefaults() {
            BombsLimit = 1;
            BombPower = 2;
            X = respawnPoint.X;
            Y = respawnPoint.Y;
        }
        
        public override void Move() {
            switch (MoveDirection) {
                case MoveDirection.None:
                    //stay here
                    break;
                case MoveDirection.Up:
                    if (CanMoveTo(X, Y - 1))
                        this.Y--;
                    break;
                case MoveDirection.Right:
                    if (CanMoveTo(X + 1, Y)) 
                        this.X++;
                    break;
                case MoveDirection.Down:
                    if (CanMoveTo(X, Y+ 1))
                        this.Y++;
                    break;
                case MoveDirection.Left:
                    if (CanMoveTo(X - 1, Y))
                        this.X--;
                    break;
            }
            MoveDirection = MoveDirection.None;
        }

        public void SetTheBomb(GameBoard board) {
            if (BombsUsed >= BombsLimit || board[X, Y] != null) return;
            BombsUsed++;
            board[X, Y] = new Bomb(BombPower);
        }

       public void InteractWithBoard(GameBoard board, List<Enemy> enemies) {
           switch (board[X, Y]) {
               case Coins c: {
                   this.Score++;
                   board[X, Y] = null;
                   break;
               }
               case BombCountBonus c: {
                   this.BombsLimit++;
                   board[X, Y] = null;
                   break;
               }
               case BombPowerBonus c: {
                   this.BombPower++;
                   board[X, Y] = null;
                   break;
               }
               case Finish f: {
                   if (f.ExitMode) {
                       //exit to next level or exit game
                   }
                   break;
               }
           }

           if (enemies.Any(e => e.X == X && e.Y == Y)) {
                this.LivesCount--;
                //todo: pause + resspawn player at the begining(no enemies)
            }

        }
    }

    public class Enemy : Character {
        private MoveDirection moveDirection;

        public Enemy(int x, int y, GameBoard board) : base(x, y, board) {
            moveDirection = MoveDirection.Up; //todo random
        }

        public override void Move() {
            var moved = false;
            int tries = 0;
            while (!moved) {
                var nextX = X;
                var nextY = Y;
                switch (moveDirection) {
                    case MoveDirection.Up:
                        nextY = Y - 1;
                        break;
                    case MoveDirection.Right:
                        nextX = X + 1;
                        break;
                    case MoveDirection.Down:
                        nextY = Y + 1;
                        break;
                    case MoveDirection.Left:
                        nextX = X - 1;
                        break;
                }

                if (CanMoveTo(nextX, nextY)) {
                    X = nextX;
                    Y = nextY;
                    moved = true;
                }
                else {
                    var rnd = new Random();
                    var nextDir = rnd.Next(1, 4);
                    moveDirection = (MoveDirection)nextDir;
                    tries++;
                    if(tries >= 4) {
                        break;
                    }
                }
            }
        }
    }

    #endregion

    public class Finish : BaseElement {
        public bool ExitMode = false;
        public Finish() : base() {
        }
    }

    #region Walls

    public class Wall : BaseElement {
        public bool IsBreakable { get; }

        public Wall(bool isBreakable = false) : base(isBarrier: true) {
            IsBreakable = isBreakable;
        }
    }

    public class CrumblingWall : Wall {
        public CrumblingWall() : base(isBreakable: true) {
        }
    }
    
    public class PermanentWall : Wall {
        public PermanentWall() : base() {
        }
    }

    #endregion

    #region Items

    public class BaseItem : BaseElement {
        public bool IsCollectable { get; }
        public BaseItem(bool isBarrier, bool isCollectable) : base(isBarrier) {
            this.IsCollectable = isCollectable;
        }
    }

    public class Coins : BaseItem {
        public Coins() : base(isBarrier: false, isCollectable: true) {
        }
    }

    public class BombCountBonus : BaseItem {
        public BombCountBonus() : base(isBarrier: false, isCollectable: true) {
        }
    }
    
    public class BombPowerBonus : BaseItem {
        public BombPowerBonus() : base(isBarrier: false, isCollectable: true) {
        }
    }

    public class Bomb : BaseItem {
        public int Power { get; }
        public int Ticks { get; private set; }

        public Bomb(int power) : base(isBarrier: true, isCollectable: false) {
            Power = power;
            Ticks = 5;
        }

        public void Tick() {
            Ticks--;
        }
    }

    #endregion
}