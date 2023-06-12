namespace Ira.Game
{
    public class Finish : BaseElement
    {
        public bool ExitMode = false;

        public Finish() : base()
        {
        }
    }

    #region Walls

    public class Wall : BaseElement
    {
        public bool IsBreakable { get; }

        public Wall(bool isBreakable = false) : base(isBarrier: true)
        {
            IsBreakable = isBreakable;
        }
    }

    public class CrumblingWall : Wall
    {
        public CrumblingWall() : base(isBreakable: true)
        {
        }
    }

    public class PermanentWall : Wall
    {
        public PermanentWall() : base()
        {
        }
    }

    #endregion

    #region Items

    public class BaseItem : BaseElement
    {
        public bool IsCollectable { get; }

        public BaseItem(bool isBarrier, bool isCollectable) : base(isBarrier)
        {
            this.IsCollectable = isCollectable;
        }
    }

    public class Coins : BaseItem
    {
        public Coins() : base(isBarrier: false, isCollectable: true)
        {
        }
    }
    
    public class Life : BaseItem
    {
        public Life() : base(isBarrier: false, isCollectable: true)
        {
        }
    }

    public class Armor : BaseItem
    {
        public Armor() : base(isBarrier: false, isCollectable: true)
        {
        }
    }

    public class BombCountBonus : BaseItem
    {
        public BombCountBonus() : base(isBarrier: false, isCollectable: true)
        {
        }
    }

    public class BombPowerBonus : BaseItem
    {
        public BombPowerBonus() : base(isBarrier: false, isCollectable: true)
        {
        }
    }

    public class Bomb : BaseItem
    {
        public int Power { get; }
        public int Ticks { get; private set; }

        public Bomb(int power, int ticks) : base(isBarrier: true, isCollectable: false)
        {
            Power = power;
            Ticks = ticks;
        }

        public void Tick()
        {
            if (Ticks > 0)
            {
                Ticks--;
            }
        }
    }

    public class Fire : BaseItem
    {
        public int StepNumber { get; set; }

        public Fire(int stepNumber) : base(isBarrier: false, isCollectable: false)
        {
            StepNumber = stepNumber;
        }
    }

    public class Trap : BaseItem
    {
        public int StepNumber { get; set; }
        public int LifeTime { get; set; }

        public Trap(int stepNumber) : base(isBarrier: false, isCollectable: false)
        {
            StepNumber = stepNumber;
            LifeTime = 3;
        }
    }

    #endregion
}