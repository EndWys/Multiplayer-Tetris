using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TetrisNetwork
{
    public class ScoreScreenBase<T> : UIScreenBase<T> where T : Component
    {
        [SerializeField]
        protected Text _scoreText;

        protected string _scorePrefix = "SCORE\n";

        protected void SetScoreText(int value)
        {
            _scoreText.text = _scorePrefix + value;
        }
    }
}
