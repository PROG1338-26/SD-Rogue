using RogueLib.Utilities; // Required for Vector2
using System;

namespace RogueLib.Enemies;

public abstract class Enemy
{
    public string Name { get; protected set; }
    public int MaxHealth { get; protected set; }
    public int CurrentHealth { get; protected set; }
    public int AttackPower { get; protected set; }
    public int DefenseValue { get; protected set; }
    public int Speed { get; protected set; }

    // Properties required for the game grid and rendering
    public Vector2 Pos { get; set; }
    public string Glyph { get; protected set; }

    public bool IsAlive => CurrentHealth > 0;

    protected Enemy(string name, int maxHealth, int attack, int defense, int speed, Vector2 pos, string glyph)
    {
        Name = name;
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        AttackPower = attack;
        DefenseValue = defense;
        Speed = speed;
        Pos = pos;
        Glyph = glyph;
    }

    //Returns the integer damage dealt
    public virtual int TakeDamage(int damageAmount)
    {
        int finalDamage = Math.Max(1, damageAmount - DefenseValue);
        CurrentHealth -= finalDamage;
        if (CurrentHealth < 0) CurrentHealth = 0;
        return finalDamage;
    }

    //Changing the signature of the TakeTurn method we can pass the Player and the Walkable tiles to the enemy
    //Added Action<string> logMessage so the enemy can talk to the Level's HUD
    public abstract void TakeTurn(Player player, HashSet<Vector2> walkables, Action<string> logMessage);
}