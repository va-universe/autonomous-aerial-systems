# Autonomous Aerial Systems
Real-time autonomous air combat and aerodynamics simulation in Unity.

## Overview
The purpose of this project is to explore how artificial neural networks can enhance simulated air combat in real-time flight dynamics. This project will include:
 - An aerodynamic flight simulation.
 - Control surface-based maneuvering.
 - Autonomous within visual range (WVR) combat.
 - AI-guided missile systems.
 - Artificial neural networks with evolving topologies.
 - Larger neural networks trained using backpropagation.

### Current Progress
The latest release, `v0.3.0`, is the `Fly-By-Wire Prototype`. This release contains an enhanced `Wing Camber Prototype`, with a fly-by-wire system. This aircraft has G-force limiting, stall protection, yaw damping, rudder activation when rolling, and deflection smoothening. Beneath are details about all the current content:
 - There are currently `4` prototype prefabs and `3` scenes where some of these prototypes can be tested.
 - There are currently `3` physics models: a generalized unmaneuverable model, a distributed aerodynamic surface model, and a wing-camber-based model.
 - There are currently `1` flight assisting system: the first fly-by-wire system.

For more information about releases, check out the [Development Log](./docs/devlog.md) and [Releases](https://github.com/va-universe/Autonomous-Aerial-Systems/releases).

### Future Content
The next planned release, `v0.4.0`, is the `Rule-based AI Prototype`. This release will contain a fully autonomous aircraft, which utilizes a predetermined rule set in order to maneuver, track waypoints and avoid collisions. This prototype will be an extension of the `v0.3.0` prototype, `Fly-By-Wire Prototype`.

### Roadmap
There are currently `14` planned milestones/releases, where `4` of them have already been completed. These releases are NOT set in stone, and are susceptible for change. Beneath is a checklist of the currently planned releases:

 - [x] Pre-v0.1.0 First Takeoff
 - [x] v0.1.0 Aerodynamic Surface Prototype
 - [x] v0.2.0 Wing Camber Prototype
 - [x] v0.3.0 Fly-By-Wire Prototype
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

![FlyByWireTakeoff](./docs/media/FlyByWireTakeoff.gif)
![FlyByWireManeuvering](./docs/media/FlyByWireManeuvering.gif)
![Stalling](./docs/media/Stalling.gif)
![Landing](./docs/media/Landing.gif)
![First Maneuvering](./docs/media/FirstManeuvering.gif)
![First Takeoff](./docs/media/FirstTakeoff.gif)
