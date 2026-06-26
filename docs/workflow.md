# Development Workflow

This document describes the workflow and naming conventions.

## Branches

All branches expect main follow the format:
```
type/branch-title
```

**Common branch types:**
| Type | Purpose |
|-|-|
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