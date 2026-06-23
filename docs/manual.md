# Project Manual

## Contents

 - Fly-By-Wire Prototype
 - Wing Camber Prototype
 - Aerodynamic Surface Prototype
 - First Aircraft Prototype

## Prototype Overview

| Prototype | Takeoff | Landing | Maneuvering | Stalling | Fly-By-Wire | Autonomous |
|------------|----------|----------|----------|----------|----------|------------|
| First Aircraft | Yes | No | No | No | No | No |
| Aerodynamic Surface | Yes | Difficult | Yes | No | No | No |
| Wing Camber | Yes | Yes | Yes | Yes | No | No |
| Fly-By-Wire | Yes | Yes | Very Easily | Yes | Yes | No |

## Fly-By-Wire Prototype
**(Release: v0.3.1)**

This prototype is a continuation of the `Wing Camber Prototype` with new flight assisting features. The fly-by-wire system includes a G-force limiter, stall protection, yaw damping, automatic rolling rudder, and deflection smoothening.

**Scene:** `FlyByWireScene`

### Keybinds

 - **W/S:** Pitch
 - **A/S:** Roll
 - **Q/E:** Yaw
 - **R/F:** Flap
 - **Space:** Thrust
 - **Left Shift:** Override
 - **G:** Wheel Brake
 - **1:** Toggle G-Force Limiter
 - **2:** Toggle Stall Protection
 - **3:** Toggle Input Smoothening
 - **4:** Toggle Yaw Damping
 - **5:** Toggle Turning Yaw

### How To Fly

 1. Use the flaps and pitch up during takeoff.
 2. Apply thrust using Space.
 3. Maneuver the aircraft using pitch, roll and yaw.
 4. Apply wheel brakes during landing.
 5. Use Left Shift to temporarily override the fly-by-wire limits.

### Fly-By-Wire Systems

 - **G-Force Limiter:** Prevents the aircraft from exceeding predetermined positive and negative G-force limits.
 - **Stall Protection:** Reduces elevator and flap controls to prevent stalling.
 - **Input/Deflection Smoothening: Gradually changes the angle of the control surfaces to avoid oscillations and sharp movement.
 - **Yaw Damping:** Reduces unwanted yaw drift and oscillations.
 - **Turning Rudder:** Automatically applies rudder during rolls for reduced yaw drift.

### UI Display

 - **Speed:** Current aircraft speed in meters per second.
 - **Altitude:** Current altitude above sea level in meters.
 - **G-Force:**Current aircraft G-Force.
 - **Stalling Alert:** Warns if most wings are stalling.
 - **Override Alert:** Indicates when you are overriding the fly-by-wire comfort settings.
 - **Braking Alert:** Indicates when you are activating the wheel brakes.

#### Fly-By-Wire Systems Display

 - **G-Force Limiter:** Indicates when the G-force limiter is turned on/off.
 - **Stall Protection:** Indicates when the stall protection is turned on/off.
 - **Smoothening:** Indicates when the input/deflection smoothening is turned on/off.
 - **Yaw Damping:** Indicates when the yaw damping is turned on/off.
 - **Turning Rudder:** Indicates when the turning yaw is turned on/off.

#### Wing Specific Display

 - **Lift:** The lift force generated in kilonewtons.
 - **Drag:** The drag force generated in kilonewtons.
 - **AoA:** Current angle of attack in degrees.
 - **Stall:** Current stall percentage of the wing.

### Media

![FlyByWireDisplay](media/FlyByWireDisplay.png)

## Wing Camber Prototype
**(Release: v0.2.0)**

This prototype introduces a new wing-camber-based aerodynamic model where control surfaces alter the properties of their parent wings, rather than generating their own forces. This prototype also has working landing gear which allows easier takeoff and landing.

**Scene:** `WingCamberScene`

### Keybinds

 - **W/S:** Pitch
 - **A/S:** Roll
 - **Q/E:** Yaw
 - **R/F:** Flap
 - **Space:** Thrust
 - **G:** Wheel Brake

### How To Fly

 1. Use the flaps and pitch down during takeoff.
 2. Apply thrust using Space.
 3. Maneuver the aircraft using pitch, roll and yaw.
 4. Apply wheel brakes during landing.

### UI Display

 - **Speed:** Current aircraft speed in meters per second.
 - **Altitude:** Current altitude above sea level in meters.
 - **G-Force:** Current aircraft G-Force.
 - **Stalling Alert:** Warns if most wings are stalling.

#### Wing Specific Display

 - **Lift:** The lift force generated in kilonewtons.
 - **Drag:** The drag force generated in kilonewtons.
 - **AoA:** Current angle of attack in degrees.
 - **Stall:** Current stall percentage of the wing.

## Aerodynamic Surface Prototype
**(Release: v0.1.0)**

This is the first maneuverable prototype. It demonstrates distributed aerodynamic surfaces where each wing and control surface generates its own local lift and drag.

**Scene:** `AerodynamicSurfaceScene`

### Keybinds

 - **W/S:** Pitch
 - **A/S:** Roll
 - **Q/E:** Yaw
 - **Space:** Thrust

### How To Fly

 1. Adjust pitch carefully during takeoff.
 2. Apply thrust using Space.
 3. Maneuver the aircraft using pitch, roll and yaw.

### UI Display

 - **Speed:** Current aircraft speed in meters per second.
 - **Altitude:** Current altitude above sea level in meters.

## First Aircraft Prototype
**(Release: Pre-v0.1.0)**

The first prototype of this project. It utilize a single physics model that generate forces at the center of mass. This prototype is therefore only capable of takeoff as it cannot generate any torque.

**Scene:** None

### Keybinds

 - **Space:** Thrust

### How To Fly

 1. Hold space to apply thrust.