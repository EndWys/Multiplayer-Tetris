using DG.Tweening;
using UnityEngine;

namespace TetrisNetwork
{
    public abstract class UIScreenBase<T> : CachedMonoBehaviour where T : Component
    {
        public const float TIME_TO_TWEEN = 1f;
        public static T Instance;

        protected CanvasGroup _canvasGroup;
        
        protected virtual void Awake()
        {
            Instance = GetComponent<T>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual void ShowScreen(float timeToTween = TIME_TO_TWEEN)
        {
            InternalAlphaScreen(timeToTween, 1f, () => {
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            });
        }

        public virtual void HideScreen(float timeToTween = TIME_TO_TWEEN)
        {
            InternalAlphaScreen(timeToTween, 0f, () => {
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            });
        }

        protected virtual void InternalAlphaScreen(float timeToTween, float alpha, TweenCallback callback)
        {
            _canvasGroup.DOFade(alpha, timeToTween).OnComplete(callback);
        } 
    }
}
