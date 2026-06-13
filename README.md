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
The latest release, `v0.2.0`, is the `Wing Camber Prototype`. This release contains a fully functioning aircraft prototype that utilizes a wing-camber-based model, in order to fly and maneuver. Beneath are details about all the current content:
 - There are currently `3` prototype prefabs and `2` scenes where some of these prototypes can be tested.
 - There are currently `3` physics models: a generalized unmaneuverable model, a distributed aerodynamic surface model, and a wing-camber-based model.

For more information about releases, check out the [Development Log](./docs/devlog.md) and [Releases](https://github.com/va-universe/Autonomous-Aerial-Systems/releases).

### Future Content
The next planned release, `v0.3.0`, is the `Fly-By-Wire Prototype`. This release will contain a fully functioning fly-by-wire system with a G force limiter, stall protection, and more flight-assisting features. This prototype will then be utilized in `v0.4.0` for the creation of the first rule-based AI prototype.

## Running Instructions
To test the different prototypes, follow the steps below:
 1. On GitHub, click `Code` and then `Download ZIP`.
 2. Extract the ZIP file to your computer, or clone the repository.
 3. Install Unity Hub if it is not already installed.
 4. In Unity Hub, go to `Installs` then click `Install Editor` and install Unity version `6000.3.12f1`.
 5. In Unity Hub, go to `Projects` and click `Add` then `Add project from disk`, then select the downloaded project folder.
 6. Open the project and wait for Unity to finish importing all assets.
 7. In the `Scenes` folder in `Assets`, open one of the following scenes and click the play icon.
    - `FirstPrototypeScene` to test the `AerodynamicSurfacePrototype`.
    - `SecondPrototypeScene` to test the `WingCamberPrototype`.
   
For more information about maneuvering the aircraft, check out the [Manual](./docs/manual.md).

### Requirements
 - Unity Editor `6000.3.12f1`.
 - Unity Hub.

## Media

![Stalling](./docs/media/Stalling.gif)
![Landing](./docs/media/Landing.gif)
![First Maneuvering](./docs/media/FirstManeuvering.gif)
![First Takeoff](./docs/media/FirstTakeoff.gif)
