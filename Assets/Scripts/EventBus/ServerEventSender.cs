using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace TetrisNetwork
{
    public class ServerEventSender : NetworkBehaviour
    {
        private readonly Dictionary<GameEventType, Action> _serverEventMap = new()
        {
            { GameEventType.MatchStart, () => RaiseEvent(new MatchStartEvent()) },
            { GameEventType.DeleteLine, () => RaiseEvent(new DeleteLineEvent()) },
            { GameEventType.PlaceTeromino, () => RaiseEvent(new PlaceTerominoEvent()) },
            { GameEventType.DetonateBomb, () => RaiseEvent(new DetonateBombEvent()) },
        };

        private static ServerEventSender _instance;
        public static ServerEventSender Instance => _instance;

        private void Awake()
        {
            _instance = this;
        }

        [ClientRpc]
        public void SendEventClientRpc(GameEventType eventType)
        {
            if(_serverEventMap.TryGetValue(eventType, out Action call))
            {
                call?.Invoke();
            }
            else
            {
                Debug.LogError($"No event of EventType: {eventType} on the client");
            }
        }

        public static void RaiseEvent<T>(T @event) where T : struct, IEvent
        {
            EventBusHolder.EventBus.Raise(@event);
        } 
    }
}
