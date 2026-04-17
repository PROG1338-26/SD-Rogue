namespace RlGameNS
{
    public class SaveData
    {
        public string PlayerClass { get; set; } = "";
        public string PlayerName { get; set; } = "";
        public int PlayerX { get; set; }
        public int PlayerY { get; set; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int Mana { get; set; }
        public int MaxMana { get; set; }
        public int Strength { get; set; }
        public int Armour { get; set; }
        public int Exp { get; set; }
        public int Gold { get; set; }
        public int Level { get; set; }
        public int Turn { get; set; }
    }
}