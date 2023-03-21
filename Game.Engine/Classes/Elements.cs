namespace Ira.Game {

    public class GameBoard {
        public int W;
        public int H;
        public EmptyElement[,] map;

        public GameBoard(int w, int h) {
            this.W = w;
            this.H = h;
            this.map = new EmptyElement[W, H];
        }
    }

    public abstract class EmptyElement {
        public int X { get; }
        public int Y { get; }
        public bool IsMovable { get; protected set; }
        public bool IsBarrier { get; }

        public EmptyElement(int x, int y, bool isBarrier = false) {
            this.X = x;
            this.Y = y;
            this.IsBarrier = isBarrier;
            this.IsMovable = false;
        }

        public virtual void Move() {
            if (IsBarrier || !IsMovable) return;
            //todo: change X and Y
        }
    }

    #region Character

    public class Character : EmptyElement {
        public int LivesCount { get; }
        public bool IsAlive {
            get {
                return LivesCount > 0;
            }
        }

        public Character(int x, int y, int lives = 1) : base(x, y) {
            this.LivesCount = lives;
            this.IsMovable = true;
        }
    }

    public class Player : Character {
        public MoveDirection MoveDirection { get; set; }
        public Player(int x, int y, int lives) : base(x, y, lives) {
        }
    }

    public class Enemy : Character {
        public Enemy(int x, int y) : base(x, y) {
        }
    }

    #endregion

    public class Finish : EmptyElement {
        public Finish(int x, int y) : base(x, y) {
        }
    }

    #region Walls

    public class Wall : EmptyElement {
        public bool IsBreakable { get; }

        public Wall(int x, int y, bool isBreakable = false) : base(x, y, isBarrier: true) {
            IsBreakable = isBreakable;
        }
    }

    public class CrumblingWall : Wall {
        public CrumblingWall(int x, int y) : base(x, y, isBreakable: true) {
        }
    }
    
    public class PermanentWall : Wall {
        public PermanentWall(int x, int y) : base(x, y) {
        }
    }

    #endregion

    #region Items

    public class BaseItem : EmptyElement {
        public bool IsCollectable { get; }
        public BaseItem(int x, int y, bool isBarrier, bool isCollectable) : base(x, y, isBarrier) {
            this.IsCollectable = isCollectable;
        }
    }

    public class Coins : BaseItem {
        public Coins(int x, int y) : base(x, y, isBarrier: false, isCollectable: true) {
        }
    }

    public class Bomb : BaseItem {
        public Bomb(int x, int y) : base(x, y, isBarrier: true, isCollectable: false) {
        }
    }

    #endregion
}