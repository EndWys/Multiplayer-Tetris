using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TetrisNetwork {
    public class BuildGameInput : CachedMonoBehaviour, IGameInput
    {
        [SerializeField] Button MoveLeft;
        [SerializeField] Button MoveRight;
        [SerializeField] Button RotateLeft;
        [SerializeField] Button RotateRight;
        [SerializeField] Button MoveDown;

        public Action<InputT> OnInput { get; set; }

        public Action OnMoveLeft { get; set; }
        public Action OnMoveRight { get; set; }
        public Action OnMoveDown { get; set; }
        public Action OnRotateLeft { get; set; }
        public Action OnRotateRight { get; set; }

        public void Initialize()
        {
            CachedGameObject.SetActive(true);

            MoveLeft.onClick.AddListener(() => OnInput.Invoke(InputT.MoveLeft));
            MoveRight.onClick.AddListener(() => OnInput.Invoke(InputT.MoveRight));
            RotateLeft.onClick.AddListener(() => OnInput.Invoke(InputT.RotateLeft));
            RotateRight.onClick.AddListener(() => OnInput.Invoke(InputT.RotateRight));
            MoveDown.onClick.AddListener(() => OnInput.Invoke(InputT.MoveDown));
        }
    }
}
