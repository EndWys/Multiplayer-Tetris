using UnityEngine;

namespace TetrisNetwork
{
    public class FieldArrowScreen : UIScreenBase<FieldArrowScreen>
    {
        const int FIRST_PLAYER_ARROW_ROTATION = 180;
        const int SECOND_PLAYER_ARROW_ROTATION = 0;

        [SerializeField] RectTransform _arrow;
        public override void ShowScreen(float timeToTween = TIME_TO_TWEEN)
        {
            InternalAlphaScreen(timeToTween, 1f, () => { });
        }

        public void UpdateArrow(int clientId)
        {
            int zArrowRotation = clientId == 1 ? SECOND_PLAYER_ARROW_ROTATION : FIRST_PLAYER_ARROW_ROTATION;

            _arrow.rotation = new Quaternion(0, 0, zArrowRotation, 0);
        }
    }
}
