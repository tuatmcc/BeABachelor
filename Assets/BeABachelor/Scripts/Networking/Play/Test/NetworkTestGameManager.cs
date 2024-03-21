using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BeABachelor.Networking.Play.Test
{
    public class NetworkTestGameManager : MonoBehaviour, IGamemanagerNetwor
    {
        private int tickCount;
        private int score;
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject enemy;
        [SerializeField] private PlayerType playerType;
        [SerializeField] private GameObject[] items;
        

        [SerializeField] private float speed = 1.0f;
        [SerializeField] KeyCode upKey = KeyCode.W;
        [SerializeField] KeyCode leftKey = KeyCode.A;
        [SerializeField] KeyCode downKey = KeyCode.S;
        [SerializeField] KeyCode rightKey = KeyCode.D;
        private bool _up = false, _left = false, _down = false, _right = false;

        
        private void Start()
        {
            tickCount = 0;
            score = 0;
            foreach (var item in items)
            {
                item.GetComponent<Button>().onClick.AddListener(() =>
                {
                    item.SetActive(false);
                });
            }
        }
        
        private void FixedUpdate()
        {
            tickCount++;
            var playerPosition = player.transform.localPosition;
            playerPosition += new Vector3(_left ? -speed : _right ? speed : 0.0f, _up ? speed : _down ? -speed : 0.0f, 0.0f);
            player.transform.localPosition = new Vector3(
                Mathf.Clamp(playerPosition.x, 50.0f, 910.0f),
                Mathf.Clamp(playerPosition.y, 50.0f, 1030.0f),
                0.0f);
        }

        private void Update()
        {
            if (Input.GetKeyDown(upKey))
            {
                _up = true;
            }
            if (Input.GetKeyDown(leftKey))
            {
                _left = true;
            }
            if (Input.GetKeyDown(downKey))
            {
                _down = true;
            }
            if (Input.GetKeyDown(rightKey))
            {
                _right = true;
            }
            
            if (Input.GetKeyUp(upKey))
            {
                _up = false;
            }
            if (Input.GetKeyUp(leftKey))
            {
                _left = false;
            }
            if (Input.GetKeyUp(downKey))
            {
                _down = false;
            }
            if (Input.GetKeyUp(rightKey))
            {
                _right = false;
            }
        }

        public int GetTickCount()
        {
            return tickCount;
        }

        public void SetTickCount(int tickCount)
        {
            this.tickCount = tickCount;
        }

        public int GetScore()
        {
            return score;
        }

        public void SetScore(int score)
        {
            this.score = score;
        }

        public void AddScore(int score)
        {
            this.score += score;
        }

        public bool[] GetEnableItems()
        {
            var i =  items.Select(item => item.activeSelf).ToArray();
            return i;
        }

        public void SetEnableItems(int index, bool value)
        {
            items[index].SetActive(value);
        }

        public Vector3 GetPlayerPosition()
        {
            return player.transform.localPosition;
        }

        public void SetPlayerPosition(Vector3 position)
        {
            player.transform.localPosition = position;
        }

        public Vector3 GetEnemyPosition()
        {
            return enemy.transform.localPosition;
        }

        public void SetEnemyPosition(Vector3 position)
        {
            enemy.transform.localPosition = position;
        }

        public bool IsHakken()
        {
            return playerType == PlayerType.Hakken;
        }
    }
}