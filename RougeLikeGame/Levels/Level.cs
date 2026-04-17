using RogueLib.Dungeon;
using RogueLib.enemies;
using RogueLib.Engine;
using RogueLib.Utilities;
using SandBox01;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TileSet = System.Collections.Generic.HashSet<RogueLib.Utilities.Vector2>;
using Vector2 = RogueLib.Utilities.Vector2;

namespace RlGameNS;

public class Level : Scene
{
    protected string? _map;
    protected int _senseRadius = 4;

    protected TileSet _walkables;
    protected TileSet _floor;
    protected TileSet _tunnel;
    protected TileSet _door;
    protected TileSet _decor;
    protected TileSet _discovered;
    protected TileSet _inFov;

    protected List<Item> _items;
    private List<NPC> _npcsList = new List<NPC>();

    public string Map1 { get; }
    public MyGame MyGame { get; }
    public Vector2 pos { get; private set; }

    public Level(Player p, string map, Game game, IRenderWindow window)
    {
        if (game == null || p == null || map == null || window == null)
            throw new ArgumentNullException("game, player, map, or window cannot be null");

        _player = p;

        // só coloca a posição inicial se ainda não existir posição salva
        if (_player.Pos == Vector2.Zero)
            _player.Pos = new Vector2(4, 12);

        _map = map;
        _game = game;
        _items = new List<Item>();
        _inFov = new TileSet();

        initMapTileSets(map);
        UpdateDiscovered();
        RegisterCommandsWithScene();
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

    private static void FadeOutGame(IRenderWindow window)
    {
        for (int step = 0; step < 3; step++)
        {
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
        Draw(window);

        for (int step = 0; step < 3; step++)
        {
            window.Display();
            Thread.Sleep(40);
        }
    }

    private static void ClearMessageLine()
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

    private void PrintMessage(string msg)
    {
        int line = 23;
        Console.SetCursorPosition(0, line);
        Console.Write(new string(' ', Console.WindowWidth));
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
            double roll = rng.NextDouble();

            if (roll < 0.4)
                _items.Add(new Potion(pos, "Health Potion", "❤️", ConsoleColor.Red));
            else if (roll < 0.7)
                _items.Add(new ManaPotion(pos, 10));
            else
                _items.Add(new StrengthPotion(pos));
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

    protected void UpdateDiscovered()
    {
        _inFov = fovCalc(_player.Pos, _senseRadius);

        if (_discovered is null)
            _discovered = new TileSet();

        _discovered.UnionWith(_inFov);
    }

    protected TileSet fovCalc(Vector2 pos, int sens)
        => Vector2.getAllTiles().Where(t => (pos - t).RookLength < sens).ToHashSet();

    public override void Update()
    {
        _player.Update();

        foreach (var npc in _npcsList)
            npc.Update();
    }

    public override void Draw(IRenderWindow? disp)
    {
        if (disp == null) return;

        var tilesToDraw = new TileSet(_decor);
        tilesToDraw.IntersectWith(_discovered);

        if (_inFov != null)
            tilesToDraw.UnionWith(_inFov);

        disp.fDraw(tilesToDraw, _map, ConsoleColor.Gray);
        _player.Draw(disp);

        drawItems(disp);

        if (_inFov != null)
            DrawEnemies(disp);

        disp.Draw(_player.HUD, new Vector2(0, 24), ConsoleColor.Green);
    }

    public override void DoCommand(Command command)
    {
        if (command.Name == "up") MovePlayer(Vector2.N);
        else if (command.Name == "down") MovePlayer(Vector2.S);
        else if (command.Name == "left") MovePlayer(Vector2.W);
        else if (command.Name == "right") MovePlayer(Vector2.E);
        else if (command.Name == "inventory")
        {
            _player.ShowInventory();

            var bpage = "";
            for (int i = 0; i < 25; ++i)
                bpage += new string(' ', 78) + "\n";

            ClearMessageLine();
            FadeOutGame(_game.Window);
            UpdateDiscovered();
            FadeInGame(_game.Window);
            _game.Window.Draw(bpage, Console.ForegroundColor);
            Draw(_game.Window);
            _game.Window.Display();
        }
        else if (command.Name == "quit")
        {
            try { AnsiConsole.Clear(); Console.SetCursorPosition(0, 0); } catch { }

            Console.Write("Save before returning to menu? (y/n): ");
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Y && _game is MyGame mg)
            {
                try
                {
                    mg.SaveToFile("save.json");
                    Console.WriteLine(" Saved.");
                }
                catch
                {
                    Console.WriteLine(" \nSave failed.");
                }
            }
            else if (key.Key == ConsoleKey.N)
            {
                Console.Write("\nfile not saved.");
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

    private Item? GetItemAt(Vector2 pos)
        => _items.FirstOrDefault(i => i.Pos.Equals(pos));

    private void RemoveItem(Item item)
        => _items.Remove(item);

    public void HandlePlayerMove(Player player, Vector2 newPos)
    {
        player.Pos = newPos;

        var item = GetItemAt(newPos);
        if (item != null)
        {
            player.Add(item);
            RemoveItem(item);
            ClearMessageLine();
            PrintMessage($"Picked up {item.Name}!");
        }
    }

    private void RegisterCommandsWithScene()
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
        var newPos = _player.Pos + delta;

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
            var oldPos = _player.Pos;
            _player.Pos = newPos;
            _walkables.Remove(newPos);
            _walkables.Add(oldPos);
            UpdateDiscovered();

            var item = _items.FirstOrDefault(it => it.Pos.Equals(newPos));
            if (item != null)
            {
                if (item is Gold g)
                {
                    _player.AddGold(g.Amount);
                    _items.Remove(item);
                    PrintMessage($"Picked up {g.Amount} gold.");
                }
                else if (item is XP xp)
                {
                    _player.AddExp(xp.Amount);
                    _items.Remove(item);
                    PrintMessage($"Picked up {xp.Amount} XP.");
                }
                else
                {
                    _player.Add(item);
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