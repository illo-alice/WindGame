using System;
using Steamworks;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class SteamService : MonoBehaviour
{
    public bool IsInitialized { get; private set; }

    public string SteamId
    {
        get
        {
            if (!IsInitialized)
                throw new InvalidOperationException(
                    "Steam is not initialized."
                );

            return SteamUser
                .GetSteamID()
                .m_SteamID
                .ToString();
        }
    }

    public string ProfileId =>
        $"steam:{SteamId}";

    private void Awake()
    {
        IsInitialized = SteamAPI.Init();

        if (!IsInitialized)
        {
            Debug.LogError(
                "SteamAPI.Init failed. Make sure Steam is running " +
                "and steam_appid.txt contains 480."
            );

            return;
        }

        Debug.Log(
            $"Steam initialized. " +
            $"User: {SteamFriends.GetPersonaName()}, " +
            $"Profile: {ProfileId}"
        );
    }

    private void Update()
    {
        if (IsInitialized)
            SteamAPI.RunCallbacks();
    }

    private void OnDestroy()
    {
        if (!IsInitialized)
            return;

        SteamAPI.Shutdown();
        IsInitialized = false;
    }
}