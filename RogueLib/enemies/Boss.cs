using System.Numerics;

namespace RogueLib.enemies
{
    internal class Boss
    {
        private Vector2 pos;
        private string v1;
        private int v2;

        public Boss(Vector2 pos, string v1, int v2)
        {
            this.pos = pos;
            this.v1 = v1;
            this.v2 = v2;
        }
    }
}