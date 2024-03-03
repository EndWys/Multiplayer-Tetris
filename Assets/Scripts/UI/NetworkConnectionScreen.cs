using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace TetrisNetwork
{
    public class NetworkConnectionScreen : UIScreenBase<Component>
    {
        [SerializeField] Button _clientButton;

        [SerializeField] Button _hostButton;

        protected override void Awake()
        {
            base.Awake();
            _clientButton.onClick.AddListener(OnClientClick);
            _hostButton.onClick.AddListener(OnHostClick);
        }
        

        void OnClientClick()
        {
            NetworkManager.Singleton.StartClient();
            HideScreen();
        }

        void OnHostClick()
        {
            NetworkManager.Singleton.StartHost();
            HideScreen();
        }
    }
}
