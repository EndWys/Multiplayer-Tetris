using VContainer;

namespace TetrisNetwork
{
    public class AudioEventReciver : IEventReceiver<MatchStartEvent>, IEventReceiver<DeleteLineEvent>,
        IEventReceiver<DetonateBombEvent>, IEventReceiver<PlaceTerominoEvent>
    {
        public UniqueId Id { get; } = new UniqueId();

        [Inject]
        public AudioEventReciver(EventBus eventBus)
        {
            eventBus.Register(this as IEventReceiver<MatchStartEvent>);
            eventBus.Register(this as IEventReceiver<DeleteLineEvent>);
            eventBus.Register(this as IEventReceiver<DetonateBombEvent>);
            eventBus.Register(this as IEventReceiver<PlaceTerominoEvent>);
        }

        public void OnEvent(MatchStartEvent @event)
        {
             AudioController.PlayMusic(AudioController.Music._gameMusic);
        }

        public void OnEvent(DeleteLineEvent @event)
        {
            AudioController.PlaySound(AudioController.Sounds.DeleteLineSound);
        }

        public void OnEvent(DetonateBombEvent @event)
        {
            AudioController.PlaySound(AudioController.Sounds.DetonateSound);
        }

        public void OnEvent(PlaceTerominoEvent @event)
        {
            AudioController.PlaySound(AudioController.Sounds.TetrominoPlaceSound);
        }
    }
}