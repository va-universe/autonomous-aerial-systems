# Development Log

This document summarizes the major milestones and prototype progression of Autonomous Aerial Systems.

For a more detailed feature lists, implementation notes, bug fixes, changes and testing information, see the official GitHub release page:
https://github.com/va-universe/Autonomous-Aerial-Systems/releases

## v0.3.1 Fly-By-Wire Hotfix
**Date: 2026-06-21**

This hotfix improves the testing possibilities of the `Fly-By-Wire Prototype` by making individual fly-by-wire systems toggleable, as well as adding missing testing features. The update also introduces new UI display that indicates what systems are turned on or off.

### Main Additions

 - Added booleans for yaw damping and turning rudder.
 - Added inputs for toggling individual fly-by-wire systems.
 - Added UI display on/off indicators for each system.

### Media

![FlyByWireDisplay](media/FlyByWireDisplay.png)

## v0.3.0 Fly-By-Wire Prototype
**Date: 2026-06-20**

This release introduces the first fly-by-wire system built on top of the `Wing Camber Prototype`. Rather than directly applying the pilot inputs to the control surfaces, the aircraft now includes flight assisting systems that improve handling of the aircraft.

The goal for this prototype was to investigate how fly-by-wire can improve aircraft stability and maneuverability. The improvements in flight are significant. 

### Main Additions

 - Implemented a G-force limiter.
 - Implemented stall protection.
 - Implemented maneuver/deflection smoothening.
 - Implemented yaw damping and rolling rudder.
 - Reworked all wing-camber-based controllers, and created a unified `Controller` class.
 - Added `Override` and `Braking` UI display.
 - Added runway and runway texture.

### Media

![FlyByWireTakeoff](media/FlyByWireTakeoff.gif)
![FlyByWireManeuvering](media/FlyByWireManeuvering.gif)

## v0.2.0 Wing Camber Prototype
**Date: 2026-05-30**

This release replaces the distributed aerodynamic surface model with a wing-camber-based approach. Control surfaces no longer generate independent lift forces and instead modify the aerodynamic properties of their parent wing surfaces. The new and improved landing gear allows this aircraft to land.

The purpose of this prototype was to create a new physics model capable of stalling without having the control surfaces stall on deflection.

### Main Additions

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

This release introduces the first maneuverable aircraft in the project. Each wing and control surface generates its own local aerodynamic forces, allowing the aircraft to produce torque and perform maneuvers.

The purpose of this prototype was to move away from the previous centeralized flight physics and begin experimenting with distributed aerodynamic force generation and torque.

### Main Additions

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

This milestone represents the first successful aircraft takeoff within the project.

The prototype used a simplified flight model capable of generating centered thrust, lift and drag. However, this prototype lacks the ability to produce any torque, and can therefore not maneuver.

### Main Additions

 - Created `First Aircraft Prototype` prefab.
 - Implemented simplified thrust, lift and drag.
 - Implemented simplified air density calculation.
 - Added input handling for thrust and control surface deflection.

### Media

![First Takeoff](media/FirstTakeoff.gif)
