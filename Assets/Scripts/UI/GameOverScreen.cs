
namespace TetrisNetwork
{
    public class GameOverScreen : ScoreScreenBase<GameOverScreen>
    {
        public override void ShowScreen(float timeToTween = TIME_TO_TWEEN)
        {
            SetScoreText(GameScoreScreen.Instance.PlayerScore);
            base.ShowScreen(timeToTween);
        }
    }
}
