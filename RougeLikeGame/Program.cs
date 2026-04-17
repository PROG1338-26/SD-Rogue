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
        bool isRunning = true;

        while (isRunning)
        {
            AnsiConsole.MarkupLine("[bold red]Welcome to the DungeonClawer Game![/]\n");

            string userChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
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
                    {
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
                        }

                        break;
                    }

                case "(1) New Game:":
                    {
                        AnsiConsole.MarkupLine("[green]Starting new game...[/]");
                        try { System.Threading.Thread.Sleep(700); } catch { }

                        var newGameClassChoice = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("Choose your class:")
                                .AddChoices(RogueFactory.GetOptions())
                        );

                        RogueClass chosen = RogueFactory.Create(newGameClassChoice);

                        Console.WriteLine($"You selected: {newGameClassChoice}");
                        try { System.Threading.Thread.Sleep(700); } catch { }
                        Console.Clear();

                        Game game = new MyGame(chosen);
                        game.run();
                        break;
                    }

                case "(2) Load Game:":
                    {
                        var saveFile = "save.json";

                        if (!File.Exists(saveFile))
                        {
                            AnsiConsole.MarkupLine("[red]No save file found (save.json).[/]");
                            break;
                        }

                        var loadedGame = LoadGame.LoadMyGame(saveFile);

                        if (loadedGame == null)
                        {
                            AnsiConsole.MarkupLine("[red]Failed to load save file.[/]");
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[green]Game loaded successfully![/]");
                            try { System.Threading.Thread.Sleep(700); } catch { }
                            Console.Clear();
                            loadedGame.run();
                        }

                        break;
                    }

                case "(3) Characters:":
                    {
                        Console.Clear();
                        var classOptions = RogueFactory.GetOptions().ToList();
                        classOptions.Add("Go back");

                        while (true)
                        {
                            var characterClassChoice = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .Title("Choose a class:")
                                    .AddChoices(classOptions)
                            );

                            if (characterClassChoice == "Go back")
                            {
                                Console.Clear();
                                break;
                            }

                            var player = RogueFactory.Create(characterClassChoice);

                            if (player is RogueClass rc)
                            {
                                AnsiConsole.MarkupLine(
                                    $"Selected: [green]{Markup.Escape(rc.Name)}[/] - {Markup.Escape(rc.Description)} - {Markup.Escape(rc.HUD)}"
                                );
                            }
                            else
                            {
                                AnsiConsole.MarkupLine($"Selected: [green]{player.Name}[/] - {player.HUD}");
                            }

                            AnsiConsole.MarkupLine("Press any key to go back.");
                            Console.ReadKey(true);
                            Console.Clear();
                        }

                        break;
                    }

                case "(4) Rules:":
                    {
                        var options = new[]
                        {
                        "Show Rules",
                        "Go back"
                    };

                        while (true)
                        {
                            var rulesChoice = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .Title("Rules:")
                                    .AddChoices(options)
                            );

                            if (rulesChoice == "Go back")
                            {
                                Console.Clear();
                                break;
                            }
                            else if (rulesChoice == "Show Rules")
                            {
                                Console.Clear();
                                Rules rules = new Rules();
                                AnsiConsole.MarkupLine($"[yellow]{rules.GetRules()}[/]");
                            }
                        }

                        break;
                    }

                case "(5) Story:":
                    {
                        Console.Clear();
                        Story story = new Story();
                        AnsiConsole.MarkupLine($"[#cd0b15]{story.GetStory()}[/]");
                        AnsiConsole.MarkupLine("Press any key to go back.");
                        Console.ReadKey(true);
                        Console.Clear();
                        break;
                    }

                case "(Q) Quit":
                    {
                        isRunning = false;
                        break;
                    }
            }
        }
    }
}