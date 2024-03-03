namespace TetrisNetwork
{
    public class GameWinnerScreen : ScoreScreenBase<GameWinnerScreen>
    {
        public override void ShowScreen(float timeToTween = TIME_TO_TWEEN)
        {
            SetScoreText(GameScoreScreen.Instance.PlayerScore);
            base.ShowScreen(timeToTween);
        }
    }
}
