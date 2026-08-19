using Fusion;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private CMS _cms;

    protected override void Configure(IContainerBuilder builder)
    {
        if (_cms == null)
            throw new MissingReferenceException("CMS is not assigned in GameLifetimeScope.");

        if (!_cms.BuildIndex())
            throw new System.InvalidOperationException("CMS contains invalid or duplicate content IDs.");

        builder.RegisterInstance(_cms).As<ICMS>();
        builder.Register<MotorModuleFactory>(Lifetime.Singleton);
        builder.RegisterComponentInHierarchy<CameraService>();
        builder.RegisterComponentInHierarchy<NetworkRunner>();
        builder.RegisterComponentInHierarchy<VContainerNetworkObjectProvider>();
        builder.RegisterComponentInHierarchy<Main>();
        builder.Register<Connection>(Lifetime.Scoped);

        // Spawn
        builder.Register<GameSessionSettings>(Lifetime.Scoped);
        builder.Register<OnlinePlayerSpawnProvider>(Lifetime.Scoped);
        builder.Register<LocalPlayerSpawnProvider>(Lifetime.Scoped);
        builder.Register<RuntimePlayerSpawnProvider>(Lifetime.Scoped)
            .As<IPlayerSpawnerProvider>();
        builder.RegisterComponentInHierarchy<Spawn>();

        builder.Register<LocalInputRegistry>(Lifetime.Scoped);
        builder.RegisterComponentInHierarchy<LocalPlayerInputManager>();
        builder.RegisterComponentInHierarchy<LocalInputDisconnectWarning>();
        builder.RegisterComponentInHierarchy<FusionInputProvider>();
    }
}
