using UnityEngine;

namespace BeABachelor.Networking
{
    public record TickData : InteractionObject
    {
        public bool[] EnableItems;
        public Vector3 EnemyPosition;
        public Vector3 PlayerPosition;
        
        public TickData(int tickCount, bool[] enableItems, Vector3 playerPosition, Vector3 enemyPosition) => (ReRequestFlag, TickCount, EnableItems, PlayerPosition, EnemyPosition) = (false, tickCount, enableItems, playerPosition, enemyPosition);
        public override string ToString() => $"TickData: {TickCount}, {EnableItems}, {PlayerPosition}, {EnemyPosition}";
    }
}