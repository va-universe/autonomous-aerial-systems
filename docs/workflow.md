# Development Workflow

This document describes the workflow and naming conventions.

## Branches

All branches expect main follow the format:
```
type/branch-title
```

**Common branch types:**
| Type | Purpose |
|---|---|
| feature | New additions |
| bugfix | Bug fixes |
| hotfix | After release fixes |
| docs | Documentation |
| prototype | New prototype's dev branch |

### Examples

```
feature/g-force-limiter
prototype/wing-camber
bugfix/center-of-mass
docs/manual
```

## Commits

Commit messages should describe the change being made. When possible, commits should reference issues using issue keywords.

Commits should follow either format:
```
Fix #X: Action & description.
Action & description.
```

### Examples

```
Fix #18: Implement wing stalling.
Fix #54: Refactor the rule-based AI controller.
Add fly-by-wire G-force limiter.
```

Keep in mind that `Fix #X` only take effect once the commit reaches the main branch.

## Issues

Each issue represent an individual task, bug or planned feature. The title of each issue should clearly state the work being performed, similar to the commits. Issue descriptions are optional.

### Examples

```
Implement wing stalling.
Refactor the rule-based AI controller.
Add fly-by-wire G-force limiter.
```

## Pull Requests

Pull request are always used in order to merge into dev branches or `main`.

All pull requests should contain:
 - A summary of all changes
 - A summary of all testing
 - Linked issues with closing keywords

### Example

#### Title

```
Implement fly-by-wire stall protection.
```

#### Description
```
## Changes
 - Implemented stall protection system.
 - Added protection override capabilities.
 - Added stall protection status UI display.

## Testing
 - Verified stall protection working.
 - Tested protection overriding.

Fixes #18
Fixes #29
```

### Squash and Merge

Always use `Squash and Merge` when merging a pull request into any branch. Review all commits, edit the commit message and make sure issue closing keywords are included.

#### Commit Message Example

```
Changes
 - Implemented stall protection.
 - Added protection override.
 - Added stall protection UI.

Testing
 - Verified stall protection working.

Fixes #18
Fixes #29
```

## Milestones

Milestones represent major goals and usually correspond to a future release and prototype. A milestone's description should contain a clear prototype or system goal, multiple issues, and should result in a releasable version. The milestone should be closed when all related issues are fixed or closed.

### Example

#### Title

```
Complete the rule-based AI prototype (v0.4.0)
```

#### Description

```
The goal with this milestone is to take the first step towards autonomous aircrafts, by creating the first prototype that utilize rules to fly, maneuver and avoid collisions. This prototype should also bne capable of tracking/following other aircrafts mid-air. Adding landing and takeoff may or may not be included in this milestone depending on difficulty.
```

## Releases

Releases are created after a milestone has been completed and merged into `main`.

### Release Notes Structure

Each release note should contain:

 - Description & Media
 - Major Changes
 - Features
 - Bugfixes & Improvements
 - Testing & Verification
 - Documentation
 - Coming Next

Optional sections:

 - Coming In The Future

### Example

#### Title

```
Fly-By-Wire Prototype
```

#### Tag

```
v0.3.0
```

#### Description

```
# Release Notes v0.3.0

This release contains a new aircraft prototype called `Fly-By-Wire Prototype`. This aircraft is an enhanced version of the `Wing Camber Prototype`, with a fly-by-wire system. This system includes a G-force limiter, stall protection, yaw damping and rolling rudder activation, as well as deflection smoothening. Both the G-force limiter and the stall protection can be decreased by holding `Left Shift` in order to "override" the fly-by-wire system. The `FlyByWireController` and the `SecondAircraftController` is now built on top of the `Controller` class, containing the wing-camber-based model.

![FlyByWireManeuvering](docs/media/FlyByWireManeuvering.gif)

## Major Changes
 - Created a fly-by-wire system for the wing-camber-based aircraft.

## Features
 - Created `FlyByWireScene` and renamed the other prototype scenes.
 - Created the `FlyByWirePrototype` prefab, by duplicating the `WingCamberPrototype`.
 - Added a runway and runway texture to the `FlyByWireScene`.
 - Implemented new unified `Controller` class and the `FlyByWireController`.
 - Implemented a G-force limiter.
 - Implemented maneuvering/deflection smoothening.
 - Implemented stall protection.
 - Added fly-by-wire override input, as well as `Override` and `Braking` UI display.
 - Implemented yaw damping and rolling rudder activation.

## Bugfixes & Improvements
 - Discovered and fixed non-local `CenterOfMass` placement.
 - Discovered and fixed inverted yaw damping.

## Testing & Verification
 - Verified functioning G-force limiting and stall protection.
 - Verified enhanced and smoother flight.
 - Verified that adverse yaw was fixed.
 - Tested different fly-by-wire parameters.

## Documentation
 - Added prototype information to `manual.md`, `devlog.md` and `README.md`.
 - Recorded and added `FlyByWireTakoff.gif` and `FlyByWireManeuvering.gif`.
 
 ## Coming Next
 - Rule-based AI prototype.
 - Rule-based takeoff.
 - Rule-based waypoint and plane tracking.
 - Sensors and rule-based anti-crash system.
```

### Release Process

 1. Complete all milestone issues.
 3. Add development log and other documentation.
 2. Squash and Merge into `main`.
 3. Close the milestone.
 4. Create release notes and release.
