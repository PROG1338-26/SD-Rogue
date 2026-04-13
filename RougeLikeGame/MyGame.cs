using RlGameNS;
using RogueLib.Dungeon;
using RogueLib.Engine;
using RogueLib.Utilities;
using SandBox01.Levels;
using Spectre.Console;
using System.Text.Json;
using System.Xml.Linq;

namespace SandBox01;


public class MyGame : Game
{
    private Player _player;

    public MyGame(Player p)
    {
        init(p);
    }
    private void init(Player? chosenPlayer = null)
    {
        // To create a new game just need to 
        // 'inject' an IRenderWindow to draw the game one
        // 'inject' a Player, the player lives outside or the Scene's because the 
        // player visits all the scenes and takes their inventory with them. 
        // you must load the first leveel, and your level or your game must manage 
        // the level switching. 

        _window = new ScreenBuff();
        if (chosenPlayer != null)
            _player = chosenPlayer;
        _currentLevel = new Level(_player!, map1, this);

    }

    public MyGame()
    {
        init();
    }
    public void ShowInventory()
    {
        try { Console.SetCursorPosition(0, 0); } catch { }
        Console.Clear();
        AnsiConsole.MarkupLine("[yellow]Inventory:[/]");
        AnsiConsole.MarkupLine($"[green]Gold:[/] {_player.Gold}");
        AnsiConsole.MarkupLine($"[green]XP:[/] {_player.Exp}");

        if (_player.Items.Count != 0)
        {
            foreach (var item in _player.Items)
            {
                AnsiConsole.MarkupLine($"- {item.Name}: {item.Description}");
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[grey]Your inventory is empty.[/]");
        }

        AnsiConsole.MarkupLine("[grey]Press any key to go back.[/]");
        Console.ReadKey(true);
        try { Console.SetCursorPosition(0, 0); } catch { }
        Console.Clear();
    }
    private record GameDTO
    {
        public string PlayerName { get; init; }
        public int PlayerGold { get; init; }
        public int PlayerXP { get; init; }
        public int PlayerStrength { get; init; } = 0;
        public int PlayerLevel { get; init; }
        public int PlayerHealth { get; init; }
        public string EnemyName { get; init; }
        public int EnemyGold { get; init; }
        public int EnemyXP { get; init; }
        public int EnemyHP { get; init; }
        public string Map { get; init; }
        public Item.ItemDTO[]? Inventory { get; init; }
    }


    // ----------------------------------------------------------------
    // string to use as the backgound on our first level
    // ----------------------------------------------------------------

    public const string map1 =
       """

               ┌──────┐          ┌─────────────┐
               │......│        ##+.............│            ┌───────┐
               │......│        # │.............+##          │.......│
               │......+######### └──────────+──┘ ###########+.......│
               │......│                     #               └───────┘
               └──+───┘                     #
           ########                 #########
      ┌────+┐                     ┌─+───────┐              ┌──────────────────┐
      │.....│                     │.........│              │..................│
      │.....+#####################+.........│              │..................│
      │.....│                     │.........│              │..................│
      │.....│                     │.........│              │..................│
      │.....│                     │.........+##############+..................│
      └─+───┘                     └───+─────┘              └───────────────+──┘
        #                             #                                    #
        ######               ┌────────+──────────────┐                     #
             #             ##+.......................|                     #
             #             # |.......................|   ###################
             #             # |.......................|   #
             #             # |.......................+####
             #             # └───────────────────────┘
             ###############
             
             
      """;



}
