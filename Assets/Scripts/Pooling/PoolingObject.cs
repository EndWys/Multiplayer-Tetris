
using Unity.Netcode;
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
            CachedTransform.localPosition = new Vector3(1000, 1000, 0);
        }

        public virtual void Spawn() { }
    }
}
