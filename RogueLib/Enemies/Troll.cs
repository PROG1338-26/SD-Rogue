using RogueLib.Utilities;

namespace RogueLib.Enemies;

public class Troll : Enemy
{
    public Troll(Vector2 startPos) : base("Troll", 100, 25, 15, 2, startPos, "T")
    {
    }

    public override void TakeTurn(Player player, HashSet<Vector2> walkables, Action<string> logMessage)
    {
        Vector2 distanceVec = player.Pos - this.Pos;

        // 1. Sense Radius: If player is too far (e.g. > 8 tiles), do nothing (Sleep)
        if (distanceVec.KingLength > 8) return;

        // 2. Attack: If adjacent to the player, attack them!
        if (distanceVec.KingLength <= 1)
        {
            // BUMP ATTACK PLAYER AND LOG IT
            int dmg = player.TakeDamage(this.AttackPower);
            logMessage($"{this.Name} hits you for {dmg} damage!");
            return;
        }

        // 3. Move: Dumb pathfinding towards the player
        int dx = Math.Sign(distanceVec.X);
        int dy = Math.Sign(distanceVec.Y);
        Vector2 step = new Vector2(dx, dy);
        Vector2 newPos = this.Pos + step;

        // If the next step is free, move there
        if (walkables.Contains(newPos))
        {
            walkables.Remove(newPos); // Claim the new tile
            walkables.Add(this.Pos);  // Free up the old tile
            this.Pos = newPos;
        }
    }
}