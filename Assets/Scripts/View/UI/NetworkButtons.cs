using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkButtons : MonoBehaviour
{
    [SerializeField] Button _clientButton;

    [SerializeField] Button _hostButton;

    private void Awake()
    {
        _clientButton.onClick.AddListener(() => NetworkManager.Singleton.StartClient());
        _hostButton.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
    }
}
