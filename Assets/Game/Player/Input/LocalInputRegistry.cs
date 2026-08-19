using System;

public sealed class LocalInputRegistry
{
    public const int Capacity = 2;

    private readonly LocalInputProvider[] _providers =
        new LocalInputProvider[Capacity];

    public event Action<int> ProviderRegistered;
    public event Action<int> ProviderUnregistered;
    public event Action<int, bool> DeviceConnectionChanged;

    public bool TryGet(int playerIndex, out LocalInputProvider provider)
    {
        if ((uint)playerIndex >= Capacity)
        {
            provider = null;
            return false;
        }

        provider = _providers[playerIndex];
        return provider != null;
    }

    public void Register(int playerIndex, LocalInputProvider provider)
    {
        if ((uint)playerIndex >= Capacity)
            throw new ArgumentOutOfRangeException(nameof(playerIndex));

        _providers[playerIndex] = provider
                                  ?? throw new ArgumentNullException(nameof(provider));

        ProviderRegistered?.Invoke(playerIndex);
    }

    public void Unregister(
        int playerIndex,
        LocalInputProvider provider)
    {
        if ((uint)playerIndex >= Capacity ||
            !ReferenceEquals(_providers[playerIndex], provider))
            return;

        _providers[playerIndex] = null;
        ProviderUnregistered?.Invoke(playerIndex);
    }

    public void NotifyDeviceConnectionChanged(
        int playerIndex,
        bool connected)
    {
        if ((uint)playerIndex >= Capacity)
            throw new ArgumentOutOfRangeException(nameof(playerIndex));

        DeviceConnectionChanged?.Invoke(playerIndex, connected);
    }
}
