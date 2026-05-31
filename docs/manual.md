# Project Manual

## Introduction
The purpose of this project is to test and explore aircraft flight dynamics, fly-by-wire systems and autonomous flight in Unity. Each prototype focuses on a specific milestone and introduces new systems or aerodynamic models.

## Future Prototypes
 - v0.3.0 Fly-By-Wire Prototype
 - v0.4.0 Rulebased-AI Prototype
 - v0.5.0 Autopilot Takeoff & Landing

## Aircraft Prototypes
Below are all the current aircraft prototypes with descriptions, scene locations, and controls.

### Wing Camber Prototype (v0.2.0)
This prototype introduces a new wing-camber-based aerodynamic model where control surfaces alter the properties of their parent wings, rather than generating their own forces. This prototype also has working landing gear which allows easier takeoff and landing.

**Scene:** `SecondPrototypeScene`

#### Controls
 - W/S: Pitch
 - A/S: Roll
 - Q/E: Yaw
 - R/F: Flap
 - Space: Thrust
 - G: Wheel Brake

#### UI Display
 - Speed: Current aircraft speed in meters per second.
 - Altitude: Current altitude above sea level in meters.
 - G-Force: Current aircraft G-Force.
 - Stalling Alert: Warns if most wings are stalling.

#### Wing Specific UI Display
 - Lift: The lift force generated in kilonewtons.
 - Drag: The drag force generated in kilonewtons.
 - AoA: Current angle of attack in degrees.
 - Stall: Current stall percentage of the wing.

#### Media
![Landing](media/Landing.gif)
![Stalling](media/Stalling.gif)

### Aerodynamic Surface Prototype (v0.1.0)
This is the first maneuverable prototype. It demonstrates distributed aerodynamic surfaces where each wing and control surface generates its own local lift and drag.

**Scene:** `FirstPrototypeScene`

#### Controls
 - W/S: Pitch
 - A/S: Roll
 - Q/E: Yaw
 - Space: Thrust

#### UI Display
 - Speed: Current aircraft speed in meters per second.
 - Altitude: Current altitude above sea level in meters.

#### Media
![First Maneuvering](media/FirstManeuvering.gif)

### First Aircraft Prototype (Pre-v0.1.0)
The first prototype of this project. It utilize a single physics model that generate forces at the center of mass. This prototype is therefore only capable of takeoff as it cannot generate any torque.

**Scene:** None

#### Controls
 - Space: Thrust

#### Media
![First Takeoff](media/FirstTakeoff.gif)
