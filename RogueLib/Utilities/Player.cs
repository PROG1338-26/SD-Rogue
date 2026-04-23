using RogueLib.Dungeon;
using RogueLib.Utilities;
using System.Collections;

public abstract class Player : IActor, IDrawable {
   public string       Name { get; set; }
   public Vector2      Pos;
   public int          Gold {  get; set; }
   public char         Glyph => '@';
   public ConsoleColor _color = ConsoleColor.White;

    //Inventory property (Diogo)
    public Inventory Inventory { get; set; }

    //Expose strength for attacking
    //AttackPower now includes base strength + accumulated weapon bonus
    public int AttackPower => _str + Inventory.GetWeaponBonus();

    //Helper property to check if player is dead
    public bool IsAlive => _hp > 0;

   //Start at level 1
   protected int _level = 1;

   protected int _hp     = 200;
   protected int _maxHp  = 200;

   protected int _str    = 40;
   protected int _maxStr = 40;

   protected int _arm    = 4;
   protected int _exp    = 0;
   public int _gold   = 0;
   protected int _turn   = 0;
   
   public int Turn => _turn;

   public Player() {
      Name = "Rogue";
      Pos  = Vector2.Zero;
      Inventory = new Inventory(); // Initialize inventory
    }

    // MERGED: HUD shows the dynamically calculated AttackPower
    public string HUD =>
       $"Level:{_level}  Gold: {_gold}    Hp: {_hp}({_maxHp})" +
       $"  Str: {AttackPower}({_maxStr})" +
       $"  Arm: {_arm}   Exp: {_exp}/{10} Turn: {_turn}";


    //Method to handle taking damage from enemies, it returns the integer damage dealt
    public virtual int TakeDamage(int damage)
    {
        int finalDamage = Math.Max(1, damage - _arm);
        _hp -= finalDamage;
        if (_hp < 0) _hp = 0;
        return finalDamage;
    }

    public virtual void Update() {
      _turn++;

      // Passive Health Regeneration
      // Heals 1 HP every 5 turns as long as the player is alive and missing health
      if (_turn % 5 == 0 && _hp < _maxHp && IsAlive) 
      {
          _hp++;
      }
   }

   public virtual void Draw(IRenderWindow disp) {
      disp.Draw(Glyph, Pos, _color);
   }

    public virtual void Heal(int amount)
    {
        _hp = Math.Min(_maxHp, _hp + amount);
    }
}