using RlGameNS;
using RogueLib.Engine;
using RogueLib.Utilities;
using System.IO;
using System.Text.Json;

namespace SandBox01;

public class MyGame : Game
{
    private Player _playerRef;

    public MyGame(Player p)
    {
        Init(p);
    }

    public MyGame()
    {
        Init();
    }

    private void Init(Player? chosenPlayer = null)
    {
        _window = new ScreenBuff();

        if (chosenPlayer != null)
            _playerRef = chosenPlayer;
        else
            throw new Exception("Player must be provided.");

        _player = _playerRef;
        _currentLevel = new RlGameNS.Level(_playerRef, map1, this, _window);
    }

    public override void SaveToFile(string path)
    {
        SaveData data = new SaveData
        {
            PlayerClass = _playerRef.RogueClass ?? "Rogue",
            PlayerName = _playerRef.Name,
            PlayerX = _playerRef.Pos.X,
            PlayerY = _playerRef.Pos.Y,
            HP = _playerRef.HP,
            MaxHP = _playerRef.MaxHP,
            Mana = _playerRef.Mana,
            MaxMana = _playerRef.MaxMana,
            Strength = _playerRef.BaseStrength,
            Armour = _playerRef.Armour,
            Exp = _playerRef.Exp,
            Gold = _playerRef.Gold,
            Level = _playerRef.LevelNumber,
            Turn = _playerRef.Turn
        };

        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(path, json);
    }

    public const string blanks = "";

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