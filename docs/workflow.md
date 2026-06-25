# Development Workflow

This document describes the workflow and naming conventions.

## Branches

All branches expect main follow the format:
```
type/branch-title
```

**Common branch types:**
| Type | Purpose |
|---------|---------|
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
Fix #X: Description.
Description.
```

### Examples

```
Fix #18: Implement wing stalling.
Fix #54: Refactor the rule-based AI controller.
Add fly-by-wire G-force limiter.
```

Keep in mind that `Fix #X` only take effect once the commit reaches the main branch.

## Issues
Similar to the commit messages, issue titles should clearly state what task they are related to and should start with something such as `Create`, `Add`, `Implement` or `Fix`. Having a description for the issue is optional.

**Issue title example:** `Implement an activation function library.`

## Pull Requests
Every pull request will have a title generally describing the changes done. All changes and tests will be stated in the description of the pull request. Issue keywords may also be included in the description (not in the title).

**Pull request title example:** `Implement neural network ground work.`

**Pull request description example:**
```
## Changes
 - Created neural network classes.
 - Implemented network propagation.
 - Fixed network initialization issue.

## Testing
 - Tested network propagation.
 - Verified network initialization working after fix.

Fixes #4
Fixes #12
```

**NOTE:** Pull requests are merged using `squash and merge` to maintain a cleaner commit histroy.
