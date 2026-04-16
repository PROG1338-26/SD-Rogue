using RogueLib.Dungeon;
using RogueLib.Engine;
using RogueLib.Utilities;
using SandBox01;
using SandBox01.Levels;
using Spectre.Console;
using System.IO;
using System.Linq;

namespace RlGameNS;

class Program
{

    static void Main(string[] args)
    {
        Console.Clear();
        // Game game = new MyGame();
        //game.run();
        bool isRunning = true;
        

        while (isRunning)
        {
            AnsiConsole.MarkupLine("[bold red]Welcome to the DungeonClawer Game![/]\n");
            string userChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                .Title("\nMain menu:")
                                .AddChoices(
                                    "(0) Continue Game:",
                                    "(1) New Game:",
                                    "(2) Load Game:",
                                    "(3) Characters:",
                                    "(4) Rules:",
                                    "(5) Story:",
                                    "(Q) Quit"
                                )
                            );


            switch (userChoice)
            {
                case "(0) Continue Game:":
                    var savePath = "save.json";
                    LoadGame loader = new LoadGame();
                    if (!File.Exists(savePath))
                    {
                        AnsiConsole.MarkupLine("[red]No save file found (save.json).[/]");
                        break;
                    }
                    if (AnsiConsole.Confirm($"Load save file '{savePath}'?"))
                    {
                        var loaded = LoadGame.LoadMyGame(savePath);
                        if (loaded == null)
                        {
                            AnsiConsole.MarkupLine("[red]Failed to load save file.[/]");
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[green]Save loaded. Starting game...[/]");
                            try { System.Threading.Thread.Sleep(700); } catch { }
                            Console.Clear();
                            loaded.run();
                        }
                    }
                    break;
                case "(1) New Game:":
                    AnsiConsole.MarkupLine("[green]Starting new game...[/]");
                    try { System.Threading.Thread.Sleep(700); } catch { }
                    var Choice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Choose your class:")
                            .AddChoices(RogueFactory.GetOptions())
                    );

                    Player chosen = RogueFactory.Create(Choice);

                    Console.WriteLine($"You selected: {Choice}");
                    try { System.Threading.Thread.Sleep(700); } catch { }
                    Console.Clear();
                    Player player = RogueFactory.Create(Choice);
                    Game game = new MyGame(chosen);
                    game.run();
                    break;
                case "(2) Load Game":
                        
                    break;
                case "(3) Characters:":
                    Console.Clear();
                    var classOptions = RogueFactory.GetOptions().ToList();
                    classOptions.Add("Go back");
                    while (true)
                    {
                        var classChoice = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("Choose a class:")
                                .AddChoices(classOptions)
                        );

                        if (classChoice == "Go back")
                        {
                            Console.Clear();
                            break;
                        }

                        var Player = RogueFactory.Create(classChoice);
                        if (Player is RogueClass rc)
                            AnsiConsole.MarkupLine(
                                $"Selected: [green]{Markup.Escape(rc.Name)}[/] - {Markup.Escape(rc.Description)} - {Markup.Escape(rc.HUD)}"
                            );
                        else
                            AnsiConsole.MarkupLine($"Selected: [green]{Player.Name}[/] - {Player.HUD}");
                        AnsiConsole.MarkupLine("Press any key to go back.");
                        Console.ReadKey(true);
                        Console.Clear();
                    }
                    break;
                case "(4) Rules:":
                    var options = new[]
                    {
                        "Show Rules",
                        "Go back"
                    };
                    while (true)
                    {
                        var choice = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("Rules:")
                                .AddChoices(options)
                        );
                        if (choice == "Go back")
                        {
                            Console.Clear();
                            break;
                        }
                        else if (choice == "Show Rules")
                        {
                            Console.Clear();
                            Rules rules = new Rules();
                            AnsiConsole.MarkupLine($"[yellow]{rules.GetRules()}[/]");
                           
                        }
                    }
                    break;
                case "(5) Story:":
                    Console.Clear();
                    Story story = new Story();
                    AnsiConsole.MarkupLine($"[#cd0b15]{story.GetStory()}[/]");
                    AnsiConsole.MarkupLine("Press any key to go back.");
                    Console.ReadKey(true);
                    Console.Clear();
                    break;
                case "(Q) Quit":
                    isRunning = false;
                    break;
            }

        }
    }
    //load game stuff
    /*
       try to load default save file first
      var savePath = "save.json";
      if (!File.Exists(savePath))
      {
          AnsiConsole.MarkupLine("[red]No save file found (save.json).[/]");
          break;
      }
     if (AnsiConsole.Confirm($"Load save file '{savePath}'?"))
{
    var loaded = LoadGame.LoadMyGame(savePath);
    if (loaded == null)
    {
        AnsiConsole.MarkupLine("[red]Failed to load save file.[/]");
    }
    else
    {
        AnsiConsole.MarkupLine("[green]Save loaded. Starting game...[/]");
        try { System.Threading.Thread.Sleep(700); } catch { }
        Console.Clear();
        loaded.run();
    }
}*/

    //option for menu 
    /**
     *  var choices = new[]
            {
                  "(1) New Game:",
                  "(2) Load Game:",
                  "(3) Characters:",
                  "(4) Rules:",
                  "(5) Story:",
                  "(Q) Quit"
             };

        string userChoice = ShowCenteredMenu("Main Menu:", choices);
         static string ShowCenteredMenu(string title, string[] choices)
        {
            int maxLen = choices.Max(s => s.Length);
            int width = Math.Max(maxLen, title.Length) + 4;
            int height = choices.Length + 4;
            int left = Math.Max((Console.WindowWidth - width) / 2, 0);
            int top = Math.Max((Console.WindowHeight - height) / 2, 0);

            try { Console.SetCursorPosition(left, top); } catch { }

            int titleLeft = left + Math.Max((width - title.Length) / 2, 0);
            try { Console.SetCursorPosition(titleLeft, Console.CursorTop); } catch { }
            Console.WriteLine(title);
            Console.WriteLine();

            for (int i = 0; i < choices.Length; i++)
            {
                string text = choices[i];
                int textLeft = left + Math.Max((width - text.Length) / 2, 0);
                try { Console.SetCursorPosition(textLeft, Console.CursorTop); } catch { }
                Console.WriteLine(text);
            }

            Console.WriteLine();
            string prompt = $"Enter choice [1-{choices.Length}]: ";
            int promptLeft = left + Math.Max((width - prompt.Length) / 2, 0);

            while (true)
            {
                try { Console.SetCursorPosition(promptLeft, Console.CursorTop); } catch { }
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (int.TryParse(input, out int idx) && idx >= 1 && idx <= choices.Length)
                    return choices[idx - 1];

                string err = "Invalid choice. Try again.";
                int errLeft = left + Math.Max((width - err.Length) / 2, 0);
                try { Console.SetCursorPosition(errLeft, Console.CursorTop); } catch { }
                Console.WriteLine(err);
            }
        }*/
}