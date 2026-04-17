using RogueLib.Dungeon;
using RogueLib.Enemies;
using RogueLib.Engine;
using RogueLib.Utilities;
using SandBox01;
using System;
using System.Collections.Generic;
using System.Linq;
using TileSet = System.Collections.Generic.HashSet<RogueLib.Utilities.Vector2>;

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
public class Level : Scene {
   // ---- level config ---- 
   protected string? _map;
   protected int     _senseRadius = 4;

   // --- Tile Sets -----
   // used to keep track of state of tiles on the map
   protected TileSet _walkables; // walkable tiles 
   protected TileSet _floor;
   protected TileSet _tunnel;
   protected TileSet _door;
   protected TileSet _decor; // walls and other decorations, always visible once discovered

   protected TileSet _discovered; // tiles the player has seen
   protected TileSet _inFov;      // current fov of player
   protected List<Item> _items;

   //List to track enemies in the level
   protected List<Enemy> _enemies;

    //Message Log and Game Over state
    protected List<string> _messageLog = new List<string>();
    protected bool _isGameOver = false;

    public Level(Player p, string map, Game game) {
      if (game == null || p == null || map == null)
         throw new ArgumentNullException("game, player, or map cannot be null");

      _player     = p;
      _map        = map;
      _game       = game;
      _items      = new List<Item>();
      _enemies    = new List<Enemy>(); // Initialize the enemies list

      initMapTileSets(map);

      // Dynamically spawn the player on the first available floor tile 
      // instead of hardcoding coordinates so they never spawn in a wall!
      _player.Pos = _floor.ElementAt(0);

      
      updateDiscovered();
      registerCommandsWithScene();
      spreadGold();
      spawnEnemies(); // Call the spawn method
    }

    //Helper method to add messages to the screen
    public void Log(string message)
    {
        _messageLog.Add(message);
        // Keep only the last 4 messages so they fit at the top of the map
        if (_messageLog.Count > 4)
        {
            _messageLog.RemoveAt(0);
        }
    }


    //Spawn enemies randomly on floor tiles
    private void spawnEnemies()
    {
        var rng = new Random();
        int enemyCount = rng.Next(5, 10);

        for (int i = 0; i < enemyCount; i++)
        {
            var pos = _floor.ElementAt(rng.Next(_floor.Count));

            // Randomly choose an enemy type
            int type = rng.Next(3);
            if (type == 0) _enemies.Add(new Goblin(pos));
            else if (type == 1) _enemies.Add(new Orc(pos));
            else _enemies.Add(new Troll(pos));

            //Make the enemies solid so the player and other enemies can't walk through them
            _walkables.Remove(pos); 
        }
    }

    private void spreadGold()
   {
        var rng = new Random();
        var hm = rng.Next(10, 20); //This determines how many gold piles exist

        for (int i = 0; i < hm; i++)
        {
            var pos = _floor.ElementAt(rng.Next(_floor.Count));
            _items.Add(new Gold(pos, 1));//Each pick up increases gold by 1
        }
   }

   protected void updateDiscovered() {
      _inFov = fovCalc(_player!.Pos, _senseRadius);

      if (_discovered is null)
         _discovered = new TileSet();

      _discovered.UnionWith(_inFov);
   }

   protected TileSet fovCalc(Vector2 pos, int sens)
      => Vector2.getAllTiles().Where(t => (pos - t).RookLength < sens).ToHashSet();

    // -----------------------------------------------------------------------


    public override void Update()
    {
        // Stop updating if the player is dead
        if (_isGameOver) return;

        updateDiscovered();

        // Check if the player is standing on an item
        var item = _items.Find(i => i.Pos == _player!.Pos);
        if (item is not null && item is Gold gold)
        {
            _player!._gold += gold.Amount;
            Log($"You picked up {gold.Amount} gold."); // Log pickups!
            //Remove the item from the list so it is no longer updated or drawn
            _items.Remove(item);
        }

        _player!.Update();
        // foreach item update
        // foreach NPC update 
        // check for player death -- on death build RIP message

        //Update all living enemies
        foreach (var enemy in _enemies)
        {
            if (enemy.IsAlive)
            {
                // Pass the Log method down to the enemy
                enemy.TakeTurn(_player, _walkables, Log);
            }
        }

        //Check if the player died during the enemy turns
        if (!_player.IsAlive)
        {
            Log(">>> YOU HAVE DIED. Press 'Q' to exit. <<<");
            _isGameOver = true;
        }
    }


    public override void Draw(IRenderWindow? disp)
    {
        // using custom RenderWindow, cast to my RenderWindow
        var tilesToDraw = new TileSet(_decor);
        tilesToDraw.IntersectWith(_discovered);
        tilesToDraw.UnionWith(_inFov);

        disp.fDraw(tilesToDraw, _map, ConsoleColor.Gray);

        drawItems(disp);

        var rng = new Random();
        if (_player.Turn % 5 == 0)
            _player._color = (ConsoleColor)rng.Next(10, 16);

        // Don't draw the player if they are dead
        if (_player.IsAlive)
            _player!.Draw(disp);

        drawEnemies(disp); // Now renders enemies
        //Added PadRight(75) to the player HUD so it clears old stats properly
        disp.Draw(_player.HUD.PadRight(75), new Vector2(0, 24), ConsoleColor.Green);

        //Draw the message log at the very top of the screen
        //Draw the message log with padding so it clears old text
        for (int i = 0; i < _messageLog.Count; i++)
        {
            disp.Draw(_messageLog[i].PadRight(75), new Vector2(0, i), ConsoleColor.White);
        }
    }

    public override void DoCommand(Command command)
    {
        //If game is over, intercept all commands except quit
        if (_isGameOver)
        {
            if (command.Name == "quit") _levelActive = false;
            return;
        }

        if (command.Name == "up") MovePlayer(Vector2.N);
        else if (command.Name == "down") MovePlayer(Vector2.S);
        else if (command.Name == "left") MovePlayer(Vector2.W);
        else if (command.Name == "right") MovePlayer(Vector2.E);
        else if (command.Name == "quit") _levelActive = false;
    }

    // -------------------------------------------------------------------------

    private void drawItems(IRenderWindow disp)
    {
        foreach (var item in _items)
        {
            if (_discovered.Contains(item.Pos))
                disp.Draw(item.Glyph, item.Pos, ConsoleColor.Yellow);
        }
    }

    //Render the enemies if they are currently in the Player's FOV
    private void drawEnemies(IRenderWindow disp)
    {
        foreach (var enemy in _enemies)
        {
            // Only draw living enemies that the player can see
            if (enemy.IsAlive && _inFov.Contains(enemy.Pos))
            {
                disp.Draw(enemy.Glyph, enemy.Pos, ConsoleColor.Red);
            }
        }
    }

    private void initMapTileSets(string map) {
      var lines = map.Split('\n');

      // ------ rules for map ------
      // . - floor, walkable and transparent.
      // + - door, walkable and transparent // # - tunnel, walkable and transparent
      // ' ' - solid stone, not walkable, not transparent.
      // '|' - wall, not walkable, not transparent, but discoverable.'
      //  others are treated the same as wall.
      // tunnel, wall, and doorways are decor, once discovered they are visible.

      _floor  = new TileSet();
      _tunnel = new TileSet();
      _door   = new TileSet();
      _decor  = new TileSet();

      foreach (var (c, p) in Vector2.Parse(map)) {
         if (c == '.') _floor.Add(p);
         else if (c == '+') _door.Add(p);
         else if (c == '#') _tunnel.Add(p);
         else if (c != ' ') _decor.Add(p);
      }

      _walkables = _floor.Union(_tunnel).Union(_door).ToHashSet();

//      for (int row = 0; row < lines.Length; ++row) {
//         for (int col = 0; col < lines[row].Length; ++col) {
//            char tile = lines[row][col];
//
//            if (tile == '.' || tile == '+' || tile == '#') {
//               _walkables.Add(new Vector2(col, row));
//               _decor.Add(new Vector2(col, row));
//            } else if (tile != ' ') {
//               _decor.Add(new Vector2(col, row));
//            }
//         }
//      }
   }

    // ------------------------------------------------------
    // Commands 
    // ------------------------------------------------------


    private void registerCommandsWithScene()
    {
        RegisterCommand(ConsoleKey.UpArrow, "up");
        RegisterCommand(ConsoleKey.W, "up");
        RegisterCommand(ConsoleKey.K, "up");

        RegisterCommand(ConsoleKey.DownArrow, "down");
        RegisterCommand(ConsoleKey.S, "down");
        RegisterCommand(ConsoleKey.J, "down");

        // Changed DownArrow to LeftArrow here
        RegisterCommand(ConsoleKey.LeftArrow, "left");
        RegisterCommand(ConsoleKey.A, "left");
        RegisterCommand(ConsoleKey.H, "left");

        // Changed DownArrow to RightArrow here
        RegisterCommand(ConsoleKey.RightArrow, "right");
        RegisterCommand(ConsoleKey.D, "right");
        RegisterCommand(ConsoleKey.L, "right");

        RegisterCommand(ConsoleKey.Q, "quit");
    }


    // Update MovePlayer to handle the "Bump-to-Attack" rogue mechanic
    public void MovePlayer(Vector2 delta)
    {
        var newPos = _player!.Pos + delta;

        // 1. Check if an enemy is in the tile we are trying to move into
        var targetEnemy = _enemies.Find(e => e.Pos == newPos && e.IsAlive);

        if (targetEnemy != null)
        {
            // BUMP ATTACK AND LOG
            int dmg = targetEnemy.TakeDamage(_player!.AttackPower);
            Log($"You strike the {targetEnemy.Name} for {dmg} damage.");

            // If the enemy dies, clear their body from the map so we can walk there again
            if (!targetEnemy.IsAlive)
            {
                Log($"The {targetEnemy.Name} dies!");
                _walkables.Add(targetEnemy.Pos);
            }
            return; // Turn is over, do not actually move
        }

        // 2. If no enemy, try to move normally
        if (_walkables.Contains(newPos))
        {
            var oldPos = _player!.Pos;
            _player!.Pos = newPos;
            _walkables.Remove(newPos);
            _walkables.Add(oldPos);
        }
    }

    public void QuitLevel() {
      _levelActive = false;
   }
}