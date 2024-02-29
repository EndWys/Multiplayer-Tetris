using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace TetrisNetwork
{
    public class GameScoreScreen : ScoreScreenBase<GameScoreScreen>
    {
        public int PlayerScore => _internalPoints;

        private int _internalPoints = 0;

        public void AddPoints(int points)
        {
            _internalPoints += points;
            SetScoreText(_internalPoints);
            ShowScreen();
        }

        public void ResetScore()
        {
            _internalPoints = 0;
            SetScoreText(_internalPoints);
        }

        public override void ShowScreen(float timeToTween = 1f)
        {
            base.ShowScreen(timeToTween);
            StopCoroutine(WaitAndHide());
            StartCoroutine(WaitAndHide());
        }

        private IEnumerator WaitAndHide()
        {
            yield return new WaitForSeconds(2f);
            HideScreen();
        }

        protected override void InternalAlphaScreen(float timeToTween, float alpha, TweenCallback callback)
        {
            base.InternalAlphaScreen(timeToTween, Mathf.Clamp(alpha, 0.2f, 1f), callback);
        }
    }
}
