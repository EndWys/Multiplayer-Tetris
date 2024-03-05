using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TetrisNetwork
{
    public class GameEntryPoint : LifetimeScope
    {
        [SerializeField] LocalPlayerInputController _playerInputController;
        [SerializeField] LocalMatchStarter _localMatchStarter;
        [SerializeField] AudioControllerInitializer _audioControllerInitializer;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_playerInputController);
            builder.RegisterComponent(_localMatchStarter);
            builder.RegisterComponent(_audioControllerInitializer);
        }
    }
}
