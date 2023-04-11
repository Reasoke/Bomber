namespace Ira.Game {
  public abstract class BaseElement {
    public bool IsMovable { get; protected set; }
    public bool IsProtected { get; protected set; }
    public bool IsBarrier { get; }

    public BaseElement(bool isBarrier = false) {
      this.IsBarrier = isBarrier;
      this.IsMovable = false;
      this.IsProtected = false;
    }

    public virtual void Move() {
      if (IsBarrier || !IsMovable) return;
    }
  }
}