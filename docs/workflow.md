# Development Workflow

This document describes the workflow and naming conventions.

## Branches

All branches expect main follow the format:
```type/branch-title```

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
Commit messages may references issues using keywords such as `Fix #X` or `Close #X`. These will only close issues if merged into main. The message will describe what the commit did, for instance `Add (feature)`, `Implement (code feature)` or `Fix (bug)`.

**Commit message example:** `Fix #8: Implement an activation function library.`

For squash merges, the final commit message is edited before merging. Keywords for closing issues (for example Fixes #X) should be included in the final squash commit message or pull request description so they are applied when merged into main.

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
