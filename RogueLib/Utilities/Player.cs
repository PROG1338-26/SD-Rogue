using RogueLib.Dungeon;
using RogueLib.Utilities;
using System.Drawing;

public abstract class Player : IActor, IDrawable {
   public string       Name { get; set; }
   public Vector2      Pos;
   public int Gold {  get; set; }
   public char         Glyph => '@';
   public ConsoleColor _color = ConsoleColor.White;

   protected int _level  = 0;
   protected int _hp     = 12;
   protected int _str    = 16;
   protected int _arm    = 4;
   protected int _exp    = 0;
   public int _gold   = 0;
   protected int _maxHp  = 12;
   protected int _maxStr = 16;
   protected int _turn   = 0;
   
   public int Turn => _turn;

   public Player() {
      Name = "Rogue";
      Pos  = Vector2.Zero;
   }


   public string HUD =>
      $"Level:{_level}  Gold: {_gold}    Hp: {_hp}({_maxHp})" +
      $"  Str: {_str}({_maxStr})" +
      $"  Arm: {_arm}   Exp: {_exp}/{10} Turn: {_turn}";


   public virtual void Update() {
      _turn++;
   }

   public virtual void Draw(IRenderWindow disp) {
      disp.Draw(Glyph, Pos, _color);
   }
    // Author: Joshua Watson
    public List<string> Messages { get; private set; } = new();
    public List<ConsoleColor> MessageColors = new();


    // Add a message to the player's message list
    public void AddMessage(string msg, ConsoleColor color)
    {
        Messages.Add(msg);
        MessageColors.Add(color);

        if (Messages.Count > 1)
        {
            Messages.RemoveAt(0);
            MessageColors.RemoveAt(0);
        }
    }
}