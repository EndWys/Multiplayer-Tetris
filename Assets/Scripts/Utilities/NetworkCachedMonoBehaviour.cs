using Unity.Netcode;
using UnityEngine;

namespace TetrisNetwork {
    public class NetworkCachedMonoBehaviour : NetworkBehaviour
    {
        Transform _cachedTransform;
        GameObject _cachedGameObject;

        public Transform CachedTransform
        {
            get
            {
                if (!_cachedTransform)
                {
                    _cachedTransform = transform;
                }
                return _cachedTransform;
            }
        }

        public GameObject CachedGameObject
        {
            get
            {
                if (!_cachedGameObject)
                {
                    _cachedGameObject = gameObject;
                }
                return _cachedGameObject;
            }
        }
    }
}
