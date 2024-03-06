using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TetrisNetwork
{
    public class GameEntryPoint : LifetimeScope
    {
        [SerializeField] LocalPlayerInputController _playerInputController;
        [SerializeField] ServerMatchController _serverMatchController;
        [SerializeField] AudioControllerInitializer _audioControllerInitializer;
        [SerializeField] EventBusHolder _busHolder;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_serverMatchController);
            builder.RegisterEntryPoint<LocalMatchStarter>(Lifetime.Singleton).AsSelf();

            builder.Register<EventBus>(Lifetime.Singleton).AsSelf();
            builder.Register<AudioController>(Lifetime.Singleton).AsSelf();
            builder.Register<AudioEventReciver>(Lifetime.Singleton).AsSelf();

            builder.RegisterComponent(_playerInputController);
            builder.RegisterComponent(_audioControllerInitializer);
            builder.RegisterComponent(_busHolder);
        }
    }
}
