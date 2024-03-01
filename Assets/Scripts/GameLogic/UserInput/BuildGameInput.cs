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

        public Action OnMoveLeft { get; set; }
        public Action OnMoveRight { get; set; }
        public Action OnMoveDown { get; set; }
        public Action OnRotateLeft { get; set; }
        public Action OnRotateRight { get; set; }

        public void Initialize()
        {
            CachedGameObject.SetActive(true);

            MoveLeft.onClick.AddListener(OnMoveLeft.Invoke);
            MoveRight.onClick.AddListener(OnMoveRight.Invoke);
            RotateLeft.onClick.AddListener(OnRotateLeft.Invoke);
            RotateRight.onClick.AddListener(OnRotateRight.Invoke);
            MoveDown.onClick.AddListener(OnMoveDown.Invoke);
        }
    }
}
