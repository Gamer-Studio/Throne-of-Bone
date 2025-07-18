namespace ToB.Entities
{
    public static class EnemyBehaviourUtility
    {
        public static bool TargetMissedUntilDestinationX(Enemy enemy, float destX)
        {
            if (enemy.target) return false;
            if ((destX - enemy.transform.position.x) * enemy.LookDirectionHorizontal.x < 0) return true;
            return enemy.Physics.IsLedgeBelow() || enemy.Physics.IsWallLeft || enemy.Physics.IsWallRight;
        }
    }
}