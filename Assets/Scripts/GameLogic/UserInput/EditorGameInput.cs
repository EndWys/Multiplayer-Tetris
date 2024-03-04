using System;
using UnityEngine;

namespace TetrisNetwork
{
    public class EditorGameInput : CachedMonoBehaviour, IGameInput
    {
        public Action<InputT> OnInput { get; set; }

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
                OnInput?.Invoke(InputT.RotateLeft);
            }

            if(Input.GetKeyDown(KeyCode.RightArrow)) 
            {
                OnInput?.Invoke(InputT.RotateRight);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                OnInput?.Invoke(InputT.MoveLeft);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                OnInput?.Invoke(InputT.MoveRight);
            }

            if (Input.GetKey(KeyCode.S))
            {
                OnInput?.Invoke(InputT.MoveDown);
            }
        }
    }
}
