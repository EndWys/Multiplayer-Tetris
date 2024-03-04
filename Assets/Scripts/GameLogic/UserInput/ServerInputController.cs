using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace TetrisNetwork
{
    public class ServerInputController : NetworkCachedMonoBehaviour
    {
        public Action<InputT,int> OnInputMove; 

        [ServerRpc(RequireOwnership = false)]
        public void MakeInputMoveServerRpc(InputT t,int clientId)
        {
            Debug.Log("Server Input: " + clientId);
            OnInputMove?.Invoke(t, clientId);
        }
    }
}