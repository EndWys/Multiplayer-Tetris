using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TetrisNetwork
{
    public interface IPooling
    {
        string ObjectName { get; }
        bool IsUsing { get; set; }
        void OnCollect();
        void OnRelease();
        void Spawn();
    }
}