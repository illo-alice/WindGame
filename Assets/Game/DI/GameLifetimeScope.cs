using Fusion;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<CameraService>();
        builder.RegisterComponentInHierarchy<NetworkRunner>();
        builder.RegisterComponentInHierarchy<VContainerNetworkObjectProvider>();
        builder.RegisterComponentInHierarchy<Main>();
        builder.Register<Connection>(Lifetime.Scoped);
        builder.RegisterComponentInHierarchy<Spawn>();
        builder.RegisterComponentInHierarchy<FusionInputProvider>();
        builder.RegisterComponentInHierarchy<LocalInputProvider>();
    }
}
