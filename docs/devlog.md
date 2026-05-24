# Development Log

## v0.1.0 First Aircraft Prototype

**Date: 2026-05-24**

This aircraft prototype now flies using distributed aerodynamic forces, allowing the control surfaces to generate torque in order to maneuver. The goal moving into the making of the `Second Aircraft Prototype` is to implement stalling, flaps and wing camber. 

### Checklist
 - Implemented distributed aerodynamic surfaces.
 - Implemented angle of attack for the `liftCoefficient`.
 - Modified the wing structure and ailerons of the `First Aircraft Prototype`.
 - Added terrain in `TestScene`.
 - Added UI display for speed and altitude.

### Challenges
 - During the initial stages of production, the `AerodynamicSurface` script were overly complex making it difficult to adjust parameters and bugfix.
 - Incorrect positioning of `CenterOfMass` and low `Angular Damping` caused extreme instability during inital testing.

### Media
![First Maneuvering](media/FirstManeuvering.gif)

## Pre-v0.1.0 First Takeoff

**Date: 2026-05-19**

This aircraft prototype used simplified flight physics to takeoff. Moving into `v0.1.0`, the goal is to switch from a centralized flight model to distributed aerodynamic surfaces with their own local forces applied.

### Checklist
 - Created `First Aircraft Prototype` prefab.
 - Implemented simplified thrust, lift and drag.
 - Implemented simplified air density calculation.
 - Added input handling for thrust and control surface deflection.

### Media
![First Takeoff](media/FirstTakeoff.gif)
