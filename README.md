# WindGame

WindGame is a work-in-progress two-player cooperative physics climber. Players use air blowers to push, pull, launch, and catch each other while climbing an obstacle course.

## Requirements

- Unity `6000.3.9f1`
- Photon Fusion SDK `2.1.1 Stable` (build `2177`)
- Fusion Physics Addon `2.1.1` (build `1199`)

Unity Package Manager restores the remaining dependencies automatically from the `Packages` directory.

## Setup

1. Clone the repository.
2. Open it with Unity.
3. Download and import the [Photon Fusion SDK](https://doc.photonengine.com/fusion/current/getting-started/sdk-download).
4. Download and import the [Fusion Physics Addon](https://doc.photonengine.com/fusion/current/addons/physics-addon-2.1).
5. If importing Photon overwrites the project configuration, restore these files from Git:
   - `Assets/Photon/Fusion/Resources/NetworkProjectConfig.fusion`
   - `Assets/Photon/Fusion/Resources/PhotonAppSettings.asset`

The Photon SDK itself is not included in the repository. The project-specific Fusion configuration and Photon AppId are tracked in Git.

To test multiplayer, run a standalone build alongside the Unity Editor or open the project in a second Editor instance. The first player hosts `WindGameRoom`; the second joins as a client.

## Controls

| Action | Input |
| --- | --- |
| Move | `WASD` or arrow keys |
| Look | Mouse |
| Jump | `Space` |
| Sprint | `Left Shift` |
| Blower action (WIP) | Left mouse button |

## Status

The multiplayer player controller and predicted physics foundation are implemented. Blower interactions, level progression, and the win/reset loop are still in development.
