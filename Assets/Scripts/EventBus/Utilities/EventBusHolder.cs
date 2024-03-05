using System;
using UnityEngine;
using VContainer;

namespace TetrisNetwork
{
    public class EventBusHolder : MonoBehaviour 
    {
        private static EventBus _eventBus;
        public static EventBus EventBus => _eventBus;

        [Inject]
        public EventBusHolder()
        {
            _eventBus = new EventBus();
        }
    }
}
