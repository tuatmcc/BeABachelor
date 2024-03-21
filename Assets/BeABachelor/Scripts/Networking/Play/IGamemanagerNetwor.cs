using UnityEngine;

namespace BeABachelor.Networking.Play
{
    public interface IGamemanagerNetwor
    {
        int GetTickCount();
        void SetTickCount(int tickCount);
        int GetScore();
        void SetScore(int score);
        void AddScore(int score);
        bool[] GetEnableItems();
        void SetEnableItems(int index, bool value);
        Vector3 GetPlayerPosition();
        void SetPlayerPosition(Vector3 position);
        Vector3 GetEnemyPosition();
        void SetEnemyPosition(Vector3 position);
        bool IsHakken();
    }
}