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
        builder.RegisterComponentInHierarchy<Spawn>();
        builder.RegisterComponentInHierarchy<FusionInputProvider>();
        builder.RegisterComponentInHierarchy<LocalInputProvider>();
    }
}
