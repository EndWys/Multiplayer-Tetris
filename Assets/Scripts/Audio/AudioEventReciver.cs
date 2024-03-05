namespace TetrisNetwork
{
    public class AudioEventReciver : IEventReceiver<MatchStartEvent>, IEventReceiver<DeleteLineEvent>,
        IEventReceiver<DetonateBombEvent>, IEventReceiver<PlaceTerominoEvent>
    {
        public UniqueId Id { get; } = new UniqueId();

        public void Initialize()
        {
            EventBusHolder.EventBus.Register(this as IEventReceiver<MatchStartEvent>);
            EventBusHolder.EventBus.Register(this as IEventReceiver<DeleteLineEvent>);
            EventBusHolder.EventBus.Register(this as IEventReceiver<DetonateBombEvent>);
            EventBusHolder.EventBus.Register(this as IEventReceiver<PlaceTerominoEvent>);
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