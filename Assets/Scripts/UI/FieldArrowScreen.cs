using UnityEngine;

namespace TetrisNetwork
{
    public class FieldArrowScreen : UIScreenBase<FieldArrowScreen>
    {
        [SerializeField] RectTransform _arrow;
        public override void ShowScreen(float timeToTween = TIME_TO_TWEEN)
        {
            InternalAlphaScreen(timeToTween, 1f, () => { });
        }

        public void UpdateArrow(int clientId)
        {
            _arrow.rotation = new Quaternion(0, 0, clientId == 1 ? 0 : 180, 0);
        }
    }
}
