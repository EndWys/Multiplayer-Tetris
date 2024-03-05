using System;
using System.Collections.Generic;
using UnityEngine;

namespace TetrisNetwork
{
    public class PlayerInputConnenctor : MonoBehaviour
    {
        [SerializeField] ServerInputController _serverInputController;

        private int _currentClientId;

        private Dictionary<InputT, Action> _actionsMap = new Dictionary<InputT, Action>();

        public void SetClientId(int currentClientId)
        {
            _currentClientId = currentClientId;
        }

        public void ConnectSignal()
        {
            Debug.Log("Connector connect signals" + _currentClientId);
            _serverInputController.OnInputMove += DoAction;
        }

        public void ConnectAction(InputT inputT, Action action)
        {
            _actionsMap.Add(inputT, action);
        }

        void DoAction(InputT inputT, int id)
        {
            Debug.Log("Validate action: " + _currentClientId + " " + id);
            if (_currentClientId != id) return;

            _actionsMap[inputT].Invoke();
        }

    }
}