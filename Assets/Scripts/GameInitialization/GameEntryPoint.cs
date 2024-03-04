using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TetrisNetwork
{
    public class GameEntryPoint : LifetimeScope
    {
        [SerializeField] LocalPlayerInputController playerInputController;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(playerInputController);
        }
    }
}
