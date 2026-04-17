using RogueLib.Dungeon;
using RogueLib.Engine;
using RogueLib.Utilities;
using System.Drawing;
using SandBox01;
using TileSet = System.Collections.Generic.HashSet<RogueLib.Utilities.Vector2>;
using RogueLib.Enemies;
using Microsoft.VisualBasic;

namespace RlGameNS;

// -----------------------------------------------------------------------
// The Level is the model, all the game world objects live in the model. 
// player input updates the model, the model updates the view, and the 
// controller runs the whole thing. 
//
// Scene is the base class for all game scenes (levels). Scene is an 
// abstract class that implements IDrawable and ICommandable. 
// 
// A dungeon level is a collection or rooms and tunnels in a 78x25 grid. 
// each tile is at a point, or grid location, represented by a Vector2. 
// 
// *TileSets* are HashSets of grid points, TileSets can be used to tell 
// GameScreen what tiles to draw. TileSets can be combined with Union and 
// Intersect to create complex tile sets.
// -----------------------------------------------------------------------
public class Level : Scene
{
   // ---- level config ---- 
   protected string? _map;
   protected int _senseRadius = 4;

   // --- Tile Sets -----
   protected TileSet _walkables;
   protected TileSet _floor;
   protected TileSet _tunnel;
   protected TileSet _door;
   protected TileSet _decor;

   protected TileSet _discovered;
   protected TileSet _inFov;
   protected List<Item> _items;
   private bool _showInventory = false;
   protected List<Enemy> _enemies;
   private string _inventoryMessage = "";

   public Level(Player p, string map, Game game)
   {
      if (game == null || p == null || map == null)
         throw new ArgumentNullException("game, player, or map cannot be null");

      _player = p;
      _player.Pos = new Vector2(4, 12);
      _map = map;
      _game = game;
      _items = new List<Item>();
      _enemies = new List<Enemy>();


      initMapTileSets(map);
      updateDiscovered();
      registerCommandsWithScene();
      spreadGold();
      spreadWeapons();
      spreadHealingPotions();
      spreadEnemies();
   }

   private void spreadEnemies()
   {
      var rng = new Random();
       
      var pos1 = _floor.ElementAt(rng.Next(_floor.Count));
      _enemies.Add(new Goblin(pos1));

      var pos2 = _floor.ElementAt(rng.Next(_floor.Count));
      _enemies.Add(new Orc(pos2));

   }
   private void spreadGold()
   {
      var rng = new Random();
      int howMany = rng.Next(10, 20);

      for (int i = 0; i < howMany; i++)
      {
         var pos = _floor.ElementAt(rng.Next(_floor.Count));
         _items.Add(new Gold(pos, rng.Next(100, 200)));
      }
   }

   private void spreadWeapons()
   {
      var rng = new Random();
      int howMany = rng.Next(2, 5);

      string[] weaponNames = { "Dagger", "Sword", "Axe" };
      int[] weaponBonuses = { 1, 3, 5 };

      for (int i = 0; i < howMany; i++)
      {
         var pos = _floor.ElementAt(rng.Next(_floor.Count));
         int index = rng.Next(weaponNames.Length);
         _items.Add(new Weapon(weaponNames[index], weaponBonuses[index], pos));
      }
   }

   private void spreadHealingPotions()
   {
      var rng = new Random();
      int howMany = rng.Next(2, 5);

      for (int i = 0; i < howMany; i++)
      {
         var pos = _floor.ElementAt(rng.Next(_floor.Count));
         _items.Add(new HealingPotion(pos, 5));
      }
   }

   protected void updateDiscovered()
   {
      _inFov = fovCalc(_player!.Pos, _senseRadius);

      if (_discovered is null)
         _discovered = new TileSet();

      _discovered.UnionWith(_inFov);
   }

   protected TileSet fovCalc(Vector2 pos, int sens)
   {
      var validMapTiles = _walkables
          .Union(_decor)
          .ToHashSet();

      return validMapTiles
          .Where(t => (pos - t).RookLength < sens)
          .ToHashSet();
   }

   // -----------------------------------------------------------------------
   public override void Update()
   {
      updateDiscovered();

      var item = _items.Find(i => i.Pos == _player!.Pos);

      if (item is not null && item is Gold gold)
      {
         _player!._gold += gold.Amount;
         // Author: Joshua Watson
         _items.Remove(item); // Remove the item from the level after picking it up
         if (gold.Amount >= 150)
         {
            _player.AddMessage(
                $"You pick up a large pile of {gold.Amount} gold.", ConsoleColor.DarkYellow);
         }
         else if (gold.Amount >= 120)
         {
            _player.AddMessage(
                $"You pick up a medium pile of {gold.Amount} gold.", ConsoleColor.DarkYellow);
         }
         else
         {
            _player.AddMessage(
                $"You pick up a small pile of {gold.Amount} gold.", ConsoleColor.DarkYellow);
         }
      }
      else if (item is Weapon weapon)
      {

         _player!.Inventory.AddWeapon(weapon);

         _items.Remove(item);

      }
      else if (item is HealingPotion potion)
      {

         _player!.Inventory.AddHealingPotion();

         _items.Remove(item);

      }

      _player!.Update();
   }

   public override void Draw(IRenderWindow? disp)
   {
      for (int y = 0; y < 25; y++)
      {
         disp.Draw(new string(' ', 78), new Vector2(0, y), ConsoleColor.White);
      }

      if (_showInventory)
      {
         disp.Draw("=== INVENTORY ===", new Vector2(2, 2), ConsoleColor.White);
         disp.Draw(_player.Inventory.GetDisplayText(_player), new Vector2(2, 4), ConsoleColor.White);

         if (!string.IsNullOrEmpty(_inventoryMessage))

         {

            disp.Draw(_inventoryMessage, new Vector2(2, 18), ConsoleColor.Yellow);

         }

         disp.Draw("Press I to return to the map", new Vector2(2, 16), ConsoleColor.DarkGray);
         return;
      }

      var discoveredWalkables = _walkables
          .Intersect(_discovered)
          .ToHashSet();

      var discoveredDecor = _decor
          .Intersect(_discovered)
          .ToHashSet();

      var tilesToDraw = discoveredWalkables
          .Union(discoveredDecor)
          .ToHashSet();

      disp.fDraw(tilesToDraw, _map, ConsoleColor.Gray);

      drawItems(disp);

      var rng = new Random();
      if (_player.Turn % 5 == 0)
         _player._color = (ConsoleColor)rng.Next(10, 16);

      _player!.Draw(disp);

      drawEnemies(disp);
      // Author: Joshua Watson
      // Draw player messages and HUD
      int line = 23;
      for (int i = 0; i < _player.Messages.Count; i++)
      {
         disp.Draw(_player.Messages[i], new Vector2(0, line), _player.MessageColors[i]);
         line++;
      }


      disp.Draw(_player.HUD, new Vector2(0, 24), ConsoleColor.Green);
   }

   public override void DoCommand(Command command)
   {
      if (command.Name == "inventory")
      {
         _showInventory = !_showInventory;
         _inventoryMessage = "";
         return;
      }

      if (_showInventory)
      {
         if (command.Name == "use_healing_potion")

         {

            if (_player!.UseHealingPotion())

            {

               _inventoryMessage = "Healing potion used!";

            }

            else
            {

               _inventoryMessage = "No healing potions available.";

            }
            return;
         }
      }

      if (command.Name == "up")
      {
         MovePlayer(Vector2.N);
      }
      else if (command.Name == "down")
      {
         MovePlayer(Vector2.S);
      }
      else if (command.Name == "left")
      {
         MovePlayer(Vector2.W);
      }
      else if (command.Name == "right")
      {
         MovePlayer(Vector2.E);
      }
      else if (command.Name == "quit")
      {
         _levelActive = false;
      }
   }

   private void drawItems(IRenderWindow disp)
   {
      foreach (var item in _items)
      {
         if (_discovered.Contains(item.Pos))
            item.Draw(disp);
      }
   }


   private void drawEnemies(IRenderWindow disp)
   {
      foreach (var enemy in _enemies)
      {
         if (_discovered.Contains(enemy.Pos))
            disp.Draw(enemy.Glyph, enemy.Pos, ConsoleColor.Red);
         if (enemy.IsAlive == false)
            disp.Draw(enemy.Glyph, enemy.Pos, ConsoleColor.White);
      }

   }

   private void initMapTileSets(string map)
   {
      _floor = new TileSet();
      _tunnel = new TileSet();
      _door = new TileSet();
      _decor = new TileSet();

      foreach (var (c, p) in Vector2.Parse(map))
      {
         if (c == '.') _floor.Add(p);
         else if (c == '+') _door.Add(p);
         else if (c == '#') _tunnel.Add(p);
         else if (c != ' ') _decor.Add(p);
      }

      _walkables = _floor.Union(_tunnel).Union(_door).ToHashSet();
   }

   // ------------------------------------------------------
   // Commands 
   // ------------------------------------------------------


   private void registerCommandsWithScene()
   {
      // Movement: Arrow keys
      RegisterCommand(ConsoleKey.UpArrow, "up");
      RegisterCommand(ConsoleKey.DownArrow, "down");
      RegisterCommand(ConsoleKey.LeftArrow, "left");
      RegisterCommand(ConsoleKey.RightArrow, "right");

      // Movement: WASD
      RegisterCommand(ConsoleKey.W, "up");
      RegisterCommand(ConsoleKey.S, "down");
      RegisterCommand(ConsoleKey.A, "left");
      RegisterCommand(ConsoleKey.D, "right");

      // Movement: Vim keys (HJKL)
      RegisterCommand(ConsoleKey.K, "up");
      RegisterCommand(ConsoleKey.J, "down");
      RegisterCommand(ConsoleKey.H, "left");
      RegisterCommand(ConsoleKey.L, "right");

      RegisterCommand(ConsoleKey.I, "inventory");
      RegisterCommand(ConsoleKey.P, "use_healing_potion");
      RegisterCommand(ConsoleKey.Q, "quit");
   }



   public void MovePlayer(Vector2 delta) {
      var newPos = _player!.Pos + delta;


      foreach (var enemy in _enemies)
      {
         if (newPos == enemy.Pos && enemy.IsAlive)
         {
            enemy.TakeDamage(_player.Attack());
            if (enemy.IsAlive == true)
            {
               _player.TakeDamage(enemy.Attack());
            }
            if (enemy.Health == 0)
            {
               enemy.IsAlive = false;
            }
         }
         else if (_walkables.Contains(newPos))
         {
            var oldPos = _player!.Pos;
            _player!.Pos = newPos;
            _walkables.Remove(newPos); // new tile is now occupied
            _walkables.Add(oldPos);    // old tile is now free
         }
      }

   }

   public void QuitLevel() {
      _levelActive = false;
   }

}