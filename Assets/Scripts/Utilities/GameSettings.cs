using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TetrisNetwork
{
    public class GameSettings
    {
        private const float MIN_TIME_TO_STEP = 0.01f;

        public float TimeToStep;

        public int PointsByBreakingLine;

        public bool ControledRandomMode;

        [SerializeField]
        public List<TetrominoSpecs> Pieces;

        public void CheckValidSettings()
        {
            if (TimeToStep < MIN_TIME_TO_STEP)
                throw new System.Exception(string.Format("timeToStep inside GameSettings.json must be higher than {0}", MIN_TIME_TO_STEP));

            if (PointsByBreakingLine < 0)
                throw new System.Exception("pointsByBreakingLine inside GameSettings.json must be higher or equal 0");
        }
    }
}
