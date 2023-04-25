using System.Collections.Generic;

namespace Ira.Game {
  public class GameBoard {
    public int Width{ get; set; }
    public int Height{ get; set; }
    public int StepNumber { get; set; }
    private BaseElement[,] Map { get; }
        
    public GameBoard(int w, int h) {
      this.Width = w;
      this.Height = h;
      this.Map = new BaseElement[Width, Height];
    }

    public BaseElement this[int x, int y] {
      get { return Map[x, y]; }
      set { Map[x, y] = value; }
    }

    public bool AddTheBomb(int x, int y, int power) {
      if (this[x, y] != null) return false;
      var b = new Bomb(power);
      this[x, y] = b;
      return true;
    }

    public void ProcessElements(Player player, List<Enemy> enemies) {
      StepNumber++;
      for (var x = 0; x < Width; x++) {
        for (var y = 0; y < Height; y++) {
          switch (this[x, y]) {
            case Bomb b: {
              b.Tick();
              if (b.Ticks == 0) {
                player.BombsUsed--;
                this[x, y] = new Fire(this.StepNumber);
                for (var i = 1; i <= b.Power; i++) {
                  if (!SetTheFire(x + i, y)) break;
                }

                for (var i = 1; i <= b.Power; i++) {
                  if (!SetTheFire(x - i, y)) break;
                }

                for (var i = 1; i <= b.Power; i++) {
                  if (!SetTheFire(x, y + i)) break;
                }

                for (var i = 1; i <= b.Power; i++) {
                  if (!SetTheFire(x, y - i)) break;
                }
              }

              break;
            }
            case Fire f: {
              if (f.StepNumber != this.StepNumber) {
                this[x, y] = null;
              }
              break;
            }
            case Trap t: {
              if (t.StepNumber+t.LifeTime <= this.StepNumber) {
                this[x, y] = null;
              }
              break;
            }
            
          }
        }
      }

      for (var i = 0; i < enemies.Count;) {
        var e = enemies[i];
        if (this[e.X, e.Y] is Fire) {
          if (e.IsProtected) {
            e.IsProtected = false;
            i++;
          }
          else enemies.Remove(e);
        }
        else {
          i++;
        }
      }
    }

    /// <summary>
    /// return true if fire can move on, otherwise - false
    /// </summary>
    private bool SetTheFire(int x, int y) {
      if (x <= 0 || x >= this.Width || y <= 0 || y >= this.Height) return false;
      var el = this[x, y]; 
      if (el is PermanentWall) return false;
      if (el is Bomb) return false;
      if (el is Finish) return false;
      this[x, y] = new Fire(this.StepNumber);
      if (el is CrumblingWall) return false;
      return true;
    }
  }
}