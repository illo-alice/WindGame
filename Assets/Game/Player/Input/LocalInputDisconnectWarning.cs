using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using VContainer;

[DisallowMultipleComponent]
public sealed class LocalInputDisconnectWarning : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private readonly string[][] _disconnectedDevices =
        new string[LocalInputRegistry.Capacity][];

    private LocalInputRegistry _registry;

    [Inject]
    public void Construct(LocalInputRegistry registry)
    {
        _registry = registry;
        _registry.DeviceConnectionChanged += OnDeviceConnectionChanged;
        _registry.ProviderUnregistered += OnProviderUnregistered;
        Refresh();
    }

    private void OnDestroy()
    {
        if (_registry != null)
        {
            _registry.DeviceConnectionChanged -= OnDeviceConnectionChanged;
            _registry.ProviderUnregistered -= OnProviderUnregistered;
        }
    }

    private void OnDeviceConnectionChanged(
        int playerIndex,
        bool connected,
        IReadOnlyList<string> deviceNames)
    {
        if ((uint)playerIndex >= LocalInputRegistry.Capacity)
            return;

        _disconnectedDevices[playerIndex] = connected
            ? null
            : CopyDeviceNames(deviceNames);

        Refresh();
    }

    private void OnProviderUnregistered(int playerIndex)
    {
        if ((uint)playerIndex >= LocalInputRegistry.Capacity)
            return;

        _disconnectedDevices[playerIndex] = null;
        Refresh();
    }

    private void Refresh()
    {
        if (_text == null)
            return;

        var builder = new StringBuilder();

        for (var i = 0; i < _disconnectedDevices.Length; i++)
        {
            var deviceNames = _disconnectedDevices[i];

            if (deviceNames == null)
                continue;

            if (builder.Length > 0)
                builder.AppendLine();

            builder.Append("Player ")
                .Append(i + 1)
                .Append(": ");

            if (deviceNames.Length == 0)
            {
                builder.Append("input device disconnected");
                continue;
            }

            builder.AppendJoin(", ", deviceNames)
                .Append(" disconnected");
        }

        _text.text = builder.ToString();

        _text.gameObject.SetActive(builder.Length > 0);
    }

    private static string[] CopyDeviceNames(
        IReadOnlyList<string> deviceNames)
    {
        if (deviceNames == null || deviceNames.Count == 0)
            return Array.Empty<string>();

        var copy = new string[deviceNames.Count];

        for (var i = 0; i < deviceNames.Count; i++)
            copy[i] = deviceNames[i];

        return copy;
    }
}
