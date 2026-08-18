# WindGame

WindGame is a work-in-progress two-player cooperative physics climber. Players use air blowers to push, pull, launch, and catch each other while climbing an obstacle course.

## Requirements

- Unity `6000.5.8f1`
- Photon Fusion SDK `2.1.1 Stable`
- Fusion Physics Addon `2.1.2`

Unity Package Manager restores the remaining dependencies automatically from the `Packages` directory.

## Setup

1. Clone the repository.
2. Open it with Unity.
3. Download and import the [Photon Fusion SDK](https://doc.photonengine.com/fusion/current/getting-started/sdk-download).
4. Download and import the [Fusion Physics Addon](https://doc.photonengine.com/fusion/current/addons/physics-addon-2.1).
5. If importing Photon overwrites the project configuration, restore these files from Git:
   - `Assets/Photon/Fusion/Resources/NetworkProjectConfig.fusion`
   - `Assets/Photon/Fusion/Resources/PhotonAppSettings.asset`
6. Enter your Photon Fusion App ID in `PhotonAppSettings.asset` locally.

The Photon SDK itself is not included in the repository. Git tracks the Fusion configuration and an empty `PhotonAppSettings.asset` template. A populated Photon App ID is local configuration and must not be committed.

To test multiplayer, run a standalone build alongside the Unity Editor or open the project in a second Editor instance. The first player hosts `WindGameRoom`; the second joins as a client.

## Controls

| Action | Input |
| --- | --- |
| Move | `WASD` or arrow keys |
| Look | Mouse |
| Jump | `Space` |
| Sprint | `Left Shift` |
| Fire / grapple (suction mode) | Left mouse button |

## Status

The multiplayer player controller, predicted physics foundation, runtime motor-module sources, and networked grapple swinging are implemented. Air blower push/pull behavior, mode switching, level progression, and the win/reset loop are still in development.
