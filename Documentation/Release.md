## Release Process

1. Bump the version number in Directory.Build.props and commit this change
2. Tag the commit with the bumped version number.

   Tag should have the syntax v[version-number], for example: v0.15.0.

   Example git command: `git tag -a v0.15.0`
3. Then run the command `git push --tags`
4. Go to https://github.com/NorskHelsenett/HelseID.Library/releases to
   start the release process.
5. When the release is published the workflow nuget_deploy.yml is triggered and will
   publish the package to NuGet.
