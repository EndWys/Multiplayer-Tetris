using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TetrisNetwork;
using UnityEngine;

namespace TetrisNetwork
{
    public class PoolingObject : CachedMonoBehaviour, IPooling
    {
        public virtual string ObjectName => "";

        public bool IsUsing { get; set; }

        public virtual void OnCollect()
        {
            IsUsing = true;
            CachedGameObject.SetActive(true);
        }

        public virtual void OnRelease()
        {
            IsUsing = false;
            CachedGameObject.SetActive(false);
        }
    }
}
