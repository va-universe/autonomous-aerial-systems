# Development Log

## v0.2.0 Wing Camber Prototype

**Date: 2026-05-30**

This aircraft prototype introduces a new wing-camber-based aerodynamic model, where control surfaces no longer generate their own lift forces but instead alter the properties of the parent wing surfaces. This aircraft is capable of takeoff, maneuvering, and landing. It now includes wing specific stalling that reduces lift and increases drag at high angles of attack. 

### Checklist
 - Implemented wing-camber-based aerodynamics model.
 - Implemented wing local stalling.
 - Added landing gear with `Wheel Collider`s.
 - Added flaps and wheel braking.
 - Added wing specific UI data display.

### Challenges
 - A major challenge was adjusting the landing gear suspension, damping, wheel friction, and center of mass to achieve stable ground handling during takeoff, landing and braking.

### Media
![Stalling](media/Stalling.gif)
![Landing](media/Landing.gif)

## v0.1.0 Aerodynamic Surface Prototype

**Date: 2026-05-24**

This aircraft prototype now flies using distributed aerodynamic forces, allowing the control surfaces to generate torque in order to maneuver. The goal moving into the making of the `Wing Camber Prototype` is to implement stalling, flaps and wing camber. 

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
