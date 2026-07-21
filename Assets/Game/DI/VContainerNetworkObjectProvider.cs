using Fusion;
using VContainer;
using VContainer.Unity;

public class VContainerNetworkObjectProvider : NetworkObjectProviderDefault
{
    private IObjectResolver _resolver;

    [Inject]
    public void Construct(IObjectResolver resolver)
    {
        _resolver = resolver;
    }

    protected override NetworkObject InstantiatePrefab(
        NetworkRunner runner,
        NetworkObject prefab)
    {
        NetworkObject instance = Instantiate(prefab);

        _resolver.InjectGameObject(instance.gameObject);

        return instance;
    }
}
