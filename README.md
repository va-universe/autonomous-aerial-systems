# Autonomous Aerial Systems
**Real-time flight simulation, autonomous controls and air combat research in Unity.**

This project explores how advanced control systems, artificial intelligence and machine learning can be used to simulate air combat. Development moves forward through a series of prototypes, each introducing new aerodynamics, systems or autonomy.

![FlyByWireManeuvering](./docs/media/FlyByWireManeuvering.gif)

## Project Goals
The long-term goal is to develop a complete autonomous air combat simulation, that includes:
 - Real-time and realistic aerodynamic flight simulations.
 - Control surface and wing-camber-based maneuvering.
 - Autonomous within visual range (WVR) combat.
 - Missile guidance and interception.
 - Aircrafts maneuvered through artificial neural networks.

## Current Status
The latest release, `v0.3.1`, is the `Fly-By-Wire Prototype`. This prototype extends the previous prototype, the `Wing Camber Prototype`, with a fly-by-wire system, that contains:

 - G-force limiting
 - Stall protection
 - Yaw damping
 - Turn assistance
 - Input/deflection smoothening

Current project content:

 - 4 aircraft prototypes
 - 3 test scenes
 - 3 aerodynamic models
 - 1 fly-by-wire system

For more information about recent releases, check out the [Development Log](./docs/devlog.md) and [Releases](https://github.com/va-universe/Autonomous-Aerial-Systems/releases).

## Next Release

The next planned release, `v0.4.0`, is the `Rule-based AI Prototype`. This release will introduce autonomous flight using rule-based systems, including waypoint and plane tracking, terrain avoidance and autonomous maneuvering. 

### Roadmap
There are currently `15` planned milestones/releases, where `5` of them have already been completed. These releases are NOT set in stone, and are susceptible for change. Beneath is a checklist of the currently planned releases:

 - [x] Pre-v0.1.0 First Takeoff
 - [x] v0.1.0 Aerodynamic Surface Prototype
 - [x] v0.2.0 Wing Camber Prototype
 - [x] v0.3.0 Fly-By-Wire Prototype
 - [x] v0.3.1 Fly-By-Wire Hotfix
 - [ ] v0.4.0 Rule-based AI Prototype
 - [ ] v0.5.0 Gun Systems & Prototype
 - [ ] Pre-v0.6.0 First Missile Launch
 - [ ] v0.6.0 First Missile Prototype
 - [ ] v0.7.0 Fly-By-Wire Combat Prototype
 - [ ] v0.8.0 Rule-based Combat AI Prototype
 - [ ] v0.9.0 Modular Network AI Prototype
 - [ ] v0.10.0 MNN Combat AI Prototype
 - [ ] v0.11.0 Curriculum Learning AI Prototype
 - [ ] v1.0.0 JAS 39 Gripen E

For more information about the roadmap and future releases, check out the [Roadmap](./docs/roadmap.md).

## Running Instructions
To test the different prototypes, follow the steps below:
 1. On GitHub, click `Code` and then `Download ZIP`.
 2. Extract the ZIP file to your computer, or clone the repository.
 3. Install Unity Hub if it is not already installed.
 4. In Unity Hub, go to `Installs` then click `Install Editor` and install Unity version `6000.3.12f1`.
 5. In Unity Hub, go to `Projects` and click `Add` then `Add project from disk`, then select the downloaded project folder.
 6. Open the project and wait for Unity to finish importing all assets.
 7. In the `Scenes` folder in `Assets`, open one of the following scenes and click the play icon.
    - `AerodynamicSurfaceScene` to test the `AerodynamicSurfacePrototype`.
    - `WingCamberScene` to test the `WingCamberPrototype`.
    - `FlyByWireScene` to test the `FlyByWirePrototype`.
   
For more information about maneuvering the aircraft, check out the [Manual](./docs/manual.md).

### Requirements
 - Unity Editor `6000.3.12f1`.
 - Unity Hub.

## Media

![FlyByWireDisplay](./docs/media/FlyByWireDisplay.png)
![FlyByWireTakeoff](./docs/media/FlyByWireTakeoff.gif)
![FlyByWireManeuvering](./docs/media/FlyByWireManeuvering.gif)
![Stalling](./docs/media/Stalling.gif)
![Landing](./docs/media/Landing.gif)
![First Maneuvering](./docs/media/FirstManeuvering.gif)
![First Takeoff](./docs/media/FirstTakeoff.gif)
