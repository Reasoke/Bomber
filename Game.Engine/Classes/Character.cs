namespace Ira.Game {
  public class Character : BaseElement {
    protected GameBoard board;
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
}