using RogueLib.Dungeon;
using RogueLib.enemies;
using RogueLib.Engine;
using RogueLib.Utilities;
using RougeLikeGame.Levels;
using SandBox01;
using Spectre.Console;
using TileSet = System.Collections.Generic.HashSet<RogueLib.Utilities.Vector2>;
using System.Collections.Generic;
using System.Linq;
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
    private readonly Queue<string> _messageLog = new Queue<string>();
    private const int MaxMessages = 5;
    // --- Tile Sets ----- 
    protected TileSet _walkables; // used to keep track of state of tiles on the map
    protected TileSet _floor;// walkable tiles 
    protected TileSet _tunnel;
    protected TileSet _door;// walls and other decorations, always visible once discovered
    protected TileSet _decor;
    // tiles the player has seen
    // current fov of player
    // random, or at stairs
    protected TileSet _discovered;
    protected TileSet _inFov;

    protected List<Item> _items;
    private List<NPC> _npcsList = new List<NPC>();

    public string Map1 { get; }
    public MyGame MyGame { get; }

    public Level(RogueLib.Utilities.Player p, string map, Game game, IRenderWindow window)
    {
        if (game == null || p == null || map == null || window == null)
            throw new ArgumentNullException("game, player, map, or window cannot be null");
        _player = p;
        _player.Pos = new Vector2(4, 12);
        _map = map;
        window = window;
        _game = game;
        _items = new List<Item>();
        _inFov = new TileSet();
        initMapTileSets(map);
        updateDiscovered();
        registerCommandsWithScene();
        SpreadTheGold();
        SpreadTheXP();
        SpreadTheItems();
        SpreadTheEnemies();
        ClearMessageLine();
        
    }

    public Level(Player player, string map1, MyGame myGame)
    {
        _player = player;
        Map1 = map1;
        MyGame = myGame;
    }

    private void ClearInventoryPanel()//helper method 4
    {
        for (int row = 5; row <= 22; row++)
        {
            Console.SetCursorPosition(0, row);
            Console.Write(new string(' ', Console.WindowWidth));
        }
    }
    //private void DrawMessageLog(IRenderWindow disp)
    //{
    //    int startLine = 19; // 19–23 = 5 lines
    //    int i = 0;

    //    foreach (var msg in _messageLog)
    //    {
    //        disp.Draw(msg, new Vector2(0, startLine + i), ConsoleColor.Yellow);
    //        i++;
    //    }
    //}
    private void FadeOutGame(IRenderWindow window)
    {
        // 3 fade steps
        for (int step = 0; step < 3; step++)
        {
            // Clear the entire back buffer
            for (int y = 0; y < window.Height; y++)
            {
                for (int x = 0; x < window.Width; x++)
                {
                    window.Draw(' ', new Vector2(x, y), ConsoleColor.Black);
                }
            }

            window.Display();
            Thread.Sleep(40);
        }
    }
    private void FadeInGame(IRenderWindow window)
    {
        // Draw the game normally first
        Draw(window);

        // 3 fade steps
        for (int step = 0; step < 3; step++)
        {
            window.Display();
            Thread.Sleep(40);
        }
    }

    private void AddMessage(string msg)//helper method 3
    {
        if (_messageLog.Count >= MaxMessages)
            _messageLog.Dequeue();

        _messageLog.Enqueue(msg);
    }
    private void ClearMessageLine()//helper method 1
    {
        Console.SetCursorPosition(0, 23);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, 23);
    }
    private void SpreadTheGold()
    {
        var rng = new Random();
        var howMuch = rng.Next(5, 20);
        for (int i = 0; i < howMuch; i++)
        {
            var pos = _floor.ElementAt(rng.Next(_floor.Count));
            _items.Add(new Gold(pos, rng.Next(100, 200)));
        }
    }

    private void PrintMessage(string msg)//helper method 2
    {
        int line = 23; // one line above HUD
        Console.SetCursorPosition(0, line);
        Console.Write(new string(' ', Console.WindowWidth)); // clear
        Console.SetCursorPosition(0, line);
        Console.Write(msg);
    }

    private void SpreadTheItems()
    {
        var rng = new Random();
        var howMuch = rng.Next(5, 20);
        for (int i = 0; i < howMuch; i++)
        {
            var pos = _floor.ElementAt(rng.Next(_floor.Count));
            _items.Add(new Potion(pos, "Health Potion", '!', ConsoleColor.Red));
        }
    }

    private void SpreadTheEnemies()
    {
        var rng = new Random();
        var howMuch = rng.Next(5, 10);

        for (int i = 0; i < howMuch; i++)
        {
            var pos = _walkables.ElementAt(rng.Next(_walkables.Count));

            var gob = new Goblin(pos, "Goblin", 10);
            gob.PlayerRef = _player;

            _npcsList.Add(gob);
        }
    }

    private void DrawEnemies(IRenderWindow disp)
    {
        if (_inFov is null) return;

        foreach (var npc in _npcsList)
        {
            if (_inFov.Contains(npc.Pos))
                npc.Draw(disp);
        }
    }

    private void SpreadTheXP()
    {
        var rng = new Random();
        var howMuch = rng.Next(5, 15);
        for (int i = 0; i < howMuch; i++)
        {
            var pos = _floor.ElementAt(rng.Next(_floor.Count));
            _items.Add(new XP(pos, rng.Next(1, 5)));
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
        => Vector2.getAllTiles().Where(t => (pos - t).RookLength < sens).ToHashSet();
    // -----------------------------------------------------------------------
    public override void Update()
    {
        _player!.Update();
        foreach (var npc in _npcsList)
            npc.Update();
        // foreach item update
        //foreach (Item item in _items)
        //{

        //}
        // check for player death -- on death build RIP message
        //if (_player.HP <= 0)
        //{

        //}
        
    }
    public override void Draw(IRenderWindow? disp)
    {
        var tilesToDraw = new TileSet(_decor);
        tilesToDraw.IntersectWith(_discovered);
        if (_inFov != null)
            tilesToDraw.UnionWith(_inFov);

        disp.fDraw(tilesToDraw, _map, ConsoleColor.Gray);
        // disp.Draw(_player!.Glyph, _player!.Pos, ConsoleColor.Cyan);
        var rng = new Random();
        if (_player.Turn % 5 == 0)
            _player._color = (ConsoleColor)rng.Next(10, 16);

        _player!.Draw(disp);

        drawItems(disp);
        if (_inFov != null)
            DrawEnemies(disp);
        // draw only items that are currently in the player's field-of-view
        disp.Draw(_player.HUD, new Vector2(0, 24), ConsoleColor.Green);
    }

    public override void DoCommand(Command command)
    {
        // player ctl 
        if (command.Name == "up") MovePlayer(Vector2.N);
        else if (command.Name == "down") MovePlayer(Vector2.S);
        else if (command.Name == "left") MovePlayer(Vector2.W);
        else if (command.Name == "right") MovePlayer(Vector2.E);
        else if (command.Name == "inventory")
        {
            _player?.ShowInventory();
            var bpage = "";
            for (int i = 0; i < 25; ++i)
                bpage += new string(' ', 78) + "\n";
            ClearMessageLine();
            FadeOutGame(_game.Window);
            updateDiscovered();
            FadeInGame(_game.Window);
            _game.Window.Draw(bpage, Console.ForegroundColor);
            Draw(_game.Window);
            _game.Window.Display();
        }
        else if (command.Name == "quit")
        {
  // -----------------------------------------------------------------------
  // save and exit to menu
  // -------------------------------------------------------------------------
            try { AnsiConsole.Clear(); Console.SetCursorPosition(0, 0); } catch { }

            Console.Write("Save before returning to menu? (y/n): ");
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Y && _game is MyGame mg)
            {
                try { mg.SaveToFile("save.json"); Console.WriteLine(" Saved."); }
                catch { Console.WriteLine(" Save failed."); }
            }
            else if (key.Key == ConsoleKey.N)
            {
                Console.Write("file not saved.");
            }

            try { Console.SetCursorPosition(0, 0); } catch { }
            _levelActive = false;
        }
    }

    private void drawItems(IRenderWindow disp)
    {
        if (_inFov is null) return;

        foreach (var item in _items.Where(it => _inFov.Contains(it.Pos)))
            item.Draw(disp);
    }   
    private void initMapTileSets(string map)
    {
        // ------ rules for map ------
        // . - floor, walkable and transparent.
        // + - door, walkable and transparent // # - tunnel, walkable and transparent
        // ' ' - solid stone, not walkable, not transparent.
        // '|' - wall, not walkable, not transparent, but discoverable.'
        //  others are treated the same as wall.
        // tunnel, wall, and doorways are decor, once discovered they are visible.
        var lines = map.Split('\n');

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
    private Item? GetItemAt(Vector2 pos)
        => _items.FirstOrDefault(i => i.Pos.Equals(pos));
    private void RemoveItem(Item item)
        => _items.Remove(item);

    // ------------------------------------------------------
    // Commands 
    // ------------------------------------------------------
    public void HandlePlayerMove(Player player, Vector2 newPos)
    {
        player.Pos = newPos;

        var item = GetItemAt(newPos);
        if (item != null)
        {
            player.ShowInventory();
            RemoveItem(item);
            ClearMessageLine();
            PrintMessage($"Picked up {item.Name}!");
        }
    }

    private void registerCommandsWithScene()
    {
        RegisterCommand(ConsoleKey.UpArrow, "up");
        RegisterCommand(ConsoleKey.W, "up");
        RegisterCommand(ConsoleKey.K, "up");

        RegisterCommand(ConsoleKey.DownArrow, "down");
        RegisterCommand(ConsoleKey.S, "down");
        RegisterCommand(ConsoleKey.J, "down");

        RegisterCommand(ConsoleKey.LeftArrow, "left");
        RegisterCommand(ConsoleKey.A, "left");
        RegisterCommand(ConsoleKey.H, "left");

        RegisterCommand(ConsoleKey.RightArrow, "right");
        RegisterCommand(ConsoleKey.D, "right");
        RegisterCommand(ConsoleKey.L, "right");

        RegisterCommand(ConsoleKey.Q, "quit");
        RegisterCommand(ConsoleKey.I, "inventory");
    }

    public void MovePlayer(Vector2 delta)
    {
        var newPos = _player!.Pos + delta;

        var enemy = _npcsList.FirstOrDefault(n => n.Pos.Equals(newPos));
        if (enemy != null)
        {
            enemy.TakeDamage(_player.Strength);
            ClearMessageLine();
            PrintMessage($"You hit the {enemy.Name}!");

            if (enemy.HP <= 0)
            {
                ClearMessageLine();
                PrintMessage($"{enemy.Name} dies!");
                _npcsList.Remove(enemy);
                _player.AddExp(5);
            }

            return;
        }

        if (_walkables.Contains(newPos))
        {
            var oldPos = _player!.Pos;
            _player!.Pos = newPos;
            _walkables.Remove(newPos);// new tile is now occupied
            _walkables.Add(oldPos);// old tile is now free
            updateDiscovered();
            // check for items at the new position and pick them up
            var item = _items.FirstOrDefault(it => it.Pos.Equals(newPos));
            if (item != null)
            {
                // add item to player's inventorys
                if (item is Gold g) 
                {
                    _player!.AddGold(g.Amount);  
                    _items.Remove(item);
                    PrintMessage($"Picked up {g.Amount} gold.");
                }
                else if (item is XP xp)
                {
                    _player!.AddExp(xp.Amount);
                    _items.Remove(item);
                    PrintMessage($"Picked up {xp.Amount} XP.");
                }
                else
                {
                    _player!.ShowInventory();
                    _items.Remove(item);
                    PrintMessage($"Picked up {item.Name}.");
                }
            }
        }
    }

    public void QuitLevel()
    {
        _levelActive = false;
    }
}
