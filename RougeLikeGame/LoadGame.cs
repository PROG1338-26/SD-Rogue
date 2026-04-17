using RogueLib.Engine;
using RogueLib.Utilities;
using SandBox01;
using System.IO;
using System.Text.Json;

namespace RlGameNS
{
    public class LoadGame
    {
        public static Game? LoadMyGame(string savePath)
        {
            try
            {
                if (!File.Exists(savePath))
                    return null;

                string json = File.ReadAllText(savePath);
                SaveData? data = JsonSerializer.Deserialize<SaveData>(json);

                if (data == null)
                    return null;

                RogueClass player = RogueFactory.Create(data.PlayerClass);
                player.Name = data.PlayerName;
                player.Pos = new Vector2(data.PlayerX, data.PlayerY);
                player.HP = data.HP;
                player.MaxHP = data.MaxHP;
                player.Mana = data.Mana;
                player.MaxMana = data.MaxMana;
                player.BaseStrength = data.Strength;
                player.Armour = data.Armour;
                player.Exp = data.Exp;
                player.Gold = data.Gold;
                player.LevelNumber = data.Level;
                player.Turn = data.Turn;

                return new MyGame(player);
            }
            catch
            {
                return null;
            }
        }
    }
}