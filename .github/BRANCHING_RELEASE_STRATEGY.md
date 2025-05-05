# Branching and Release Strategy

The following page describes our branching and release strategy.

## Protected branches

Some branches are protected in our repositories.

Points below detail the policy that determines in our repositories which branches are protected.

### Main branch

The default branch is named `main` and is protected. Commits can be merged only via pull requests passing different checks:
- At least one reviewer, different from the pull request's author, must have approved the pull request.
- Each commits must be signed correctly by its author as descrived in the [CONTRIBUTING](CONTRIBUTING.md) file.
- The ENTSO-E ICT Committee must approve the publication of the release.
- ENTSO-E security scan must be performed.

### Release/development branches

Release and development branches, which pattern is  `release-v*.*.0`, are created from the corresponding tag `v*.*.0` each time a corrective release is needed.
Only [maintainers](MAINTAINERS.md) can create or force-push into these branches.
Other contributors must create a pull request to do push into these branches and pass the same checks as the `main` branch.
These branches are always up to the most recent patch of the release.

## Releasing a repository

In order to release a ENTSO-E repository, you must be a maintainer of the repository you wish to release.

The release tag must respect the pattern `vX.Y.Z`.

You can then publish a Release note pointing to your newly created tag.

Please make sure that your release note is comprehensive to all bug fixes of the corrective release.
