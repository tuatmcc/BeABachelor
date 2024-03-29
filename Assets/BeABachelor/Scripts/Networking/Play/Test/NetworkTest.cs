﻿using BeABachelor.Networking.Interface;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeABachelor.Networking.Play.Test
{
    public class NetworkTest : MonoBehaviour
    {
        [SerializeField] private InputField ipInputField;
        [SerializeField] private InputField portInputField;
        [SerializeField] private InputField clientPortInputField;
        [SerializeField] private Button connectButton;
        [SerializeField] private Toggle hostToggle;
        [SerializeField] private SynchronizationController synchronizationController;
        [SerializeField] private GameObject p1;
        [SerializeField] private GameObject p2;
        [SerializeField] private GameObject field;
        [SerializeField] private float speed = 0.1f;
        [SerializeField] private Text stateText;
        
        [Inject] private INetworkManager _networkManager;
        
        [SerializeField] private GameObject _player;

        private bool _w;
        private bool _a;
        private bool _s;
        private bool _d;
        
        private void Start()
        {
            connectButton.onClick.AddListener(() =>
            {
                _player = hostToggle.isOn ? p1 : p2;
                p1.GetComponent<TransformSynchronization>().UseHostData = !hostToggle.isOn;
                p2.GetComponent<TransformSynchronization>().UseHostData = hostToggle.isOn;
                if (_networkManager.IsConnected)
                {
                    _networkManager.Disconnect();
                    return;
                }
                _networkManager.ConnectAsync(hostToggle.isOn, ipInputField.text, int.Parse(portInputField.text),
                    int.Parse(clientPortInputField.text)).Forget();
                UniTask.WaitUntil(() =>
                        _networkManager.IsConnected || _networkManager.NetworkState == NetworkState.Disconnected)
                    .ContinueWith(() =>
                    {
                        if (!_networkManager.IsConnected) return;
                    }).Forget();
            });
            _networkManager.SynchronizationController = synchronizationController;
            _networkManager.OnConnected += _ =>
            {
                stateText.text = "Connected";
            };
            _networkManager.OnConnecting += _ =>
            {
                stateText.text = "Connecting";
            };
            _networkManager.OnDisconnected += () =>
            {
                stateText.text = "Disconnected";
            };
        }

        private void Update()
        {
            if (!_networkManager.IsConnected) return;
            if (Input.GetKeyDown(KeyCode.W))
            {
                _w = true;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                _a = true;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                _s = true;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                _d = true;
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                _w = false;
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                _a = false;
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                _s = false;
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                _d = false;
            }
        }

        private void FixedUpdate()
        {
            if (!_networkManager.IsConnected) return;
            var t = _player.transform;
            var p = t.position;
            if (_w)
            {
                p.z += speed;
            }
            if (_a)
            {
                p.x -= speed;
            }
            if (_s)
            {
                p.z -= speed;
            }
            if (_d)
            {
                p.x += speed;
            }
            t.position = p;
        }
    }
}