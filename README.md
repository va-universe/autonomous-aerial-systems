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

## Prototype Progression

| Release | Prototype | Focus |
|----------|----------|----------|
| v0.1.0 | Aerodynamic Surface Prototype | Distributed aerodynamic forces |
| v0.2.0 | Wing Camber Prototype | Wing-camber-based aerodynamics and stalling |
| v0.3.0 | Fly-By-Wire Prototype | Flight assistance systems (FBW) |
| v0.4.0 | Rule-Based AI Prototype | Autonomous flight without neural networks |

## Next Release

The next planned release, `v0.4.0`, is the `Rule-based AI Prototype`. This release will introduce autonomous flight using rule-based systems, including waypoint and plane tracking, terrain avoidance and autonomous maneuvering. 

### Roadmap

Current development path (subject to change):

 - [x] Aerodynamic Surface Prototype
 - [x] Wing Camber Prototype
 - [x] Fly-By-Wire Prototype
 - [ ] Rule-based AI Prototype
 - [ ] Weapon Systems Prototype
 - [ ] Combat AI Prototype
 - [ ] Neural Network Prototype
 - [ ] Curriculum Learning Prototype
 - [ ] JAS 39 Gripen E

See the full roadmap in [Roadmap](./docs/roadmap.md).

## Documentation
 - [Manual](./docs/manual.md)
 - [Development Log](./docs/devlog.md)
 - [Roadmap](./docs/roadmap.md)
 - [Release Notes](https://github.com/va-universe/Autonomous-Aerial-Systems/releases)

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

### Fly-By-Wire Prototype

![FlyByWireDisplay](./docs/media/FlyByWireDisplay.png)
![FlyByWireTakeoff](./docs/media/FlyByWireTakeoff.gif)
![FlyByWireManeuvering](./docs/media/FlyByWireManeuvering.gif)

## Wing Camber Prototype

![Stalling](./docs/media/Stalling.gif)
![Landing](./docs/media/Landing.gif)

## Aerodynamic Surface Prototype

![First Maneuvering](./docs/media/FirstManeuvering.gif)

## First Aircraft Prototype

![First Takeoff](./docs/media/FirstTakeoff.gif)
