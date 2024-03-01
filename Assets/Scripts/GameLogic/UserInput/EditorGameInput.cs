using System;
using UnityEngine;

namespace TetrisNetwork
{
    public class EditorGameInput : CachedMonoBehaviour, IGameInput
    {
        public Action OnMoveLeft { get; set; }
        public Action OnMoveRight { get; set; }
        public Action OnMoveDown { get; set; }
        public Action OnRotateLeft { get; set; }
        public Action OnRotateRight { get; set; }

        bool _chenckForButtons = false;

        public void Initialize()
        {
            _chenckForButtons = true;
        }

        private void Update()
        {
            if(!_chenckForButtons)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                OnRotateLeft?.Invoke();
            }

            if(Input.GetKeyDown(KeyCode.RightArrow)) 
            {
                OnRotateRight?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                OnMoveLeft?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                OnMoveRight?.Invoke();
            }

            if (Input.GetKey(KeyCode.S))
            {
                OnMoveDown?.Invoke();
            }
        }
    }
}
