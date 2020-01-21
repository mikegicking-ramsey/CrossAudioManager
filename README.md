Multi-Platform Template
=======================

This repo acts as a template for creating multi-platform plugins as NuGet packages

In order to use it, create a fork of this repo and then do the following:
1. Rename the solution from MultiPlatformTemplate to whatever name you want your package to have
2. Rename the project from MultiPlatformTemplate to whatever name you want your package to have
3. Fill out the information under the TODOs in the .csproj file (package name, authors, owners, etc.)
4. Find and replace all instances of "$safeprojectname$" with the name of your plugin
5. Find and replace the word "Plugin" in all classes (both the file name and class declaration)
6. Find and replace all instances of "PLUGINNAMEGOESHERE" with the name of your plugin