using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace TetrisNetwork
{
    public class ServerEventSender : NetworkBehaviour
    {
        private EventBus _eventBus;

        private Dictionary<GameEventType, Action> _serverEventMap;

        private static ServerEventSender _instance;
        public static ServerEventSender Instance => _instance;

        [Inject]
        public void Construct(EventBus eventBus)
        {
            _instance = this;
            _eventBus = eventBus;
            _serverEventMap = BuildMap();
        }

        private Dictionary<GameEventType, Action> BuildMap()
        {
            return new()
            {
                { GameEventType.MatchStart, () => RaiseEvent(new MatchStartEvent()) },
                { GameEventType.DeleteLine, () => RaiseEvent(new DeleteLineEvent()) },
                { GameEventType.PlaceTeromino, () => RaiseEvent(new PlaceTerominoEvent()) },
                { GameEventType.DetonateBomb, () => RaiseEvent(new DetonateBombEvent()) },
            };
        }

        [ClientRpc]
        public void SendEventClientRpc(GameEventType eventType)
        {
            if (_serverEventMap.TryGetValue(eventType, out Action call))
            {
                call?.Invoke();
            }
            else
            {
                Debug.LogError($"No event of EventType: {eventType} on the client");
            }
        }

        public void RaiseEvent<T>(T @event) where T : struct, IEvent
        {
            _eventBus.Raise(@event);
        }
    }
}
