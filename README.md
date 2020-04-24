# PFAdmin
Admin API GUI application for PlayFab (Now with Unreal support!)

<img src="https://i.imgur.com/NzKHvml.png" alt="main" width="200"/> <img src="https://i.imgur.com/uUPctYA.png" alt="server" width="200"/> <img src="https://i.imgur.com/fjUg3m6.png" alt="server" width="200"/>

# How To Use
- Download a [release](https://github.com/bphillips09/PFAdmin/releases/latest)

To manage your Title (add/remove Assets, create Builds):
- Nothing else is required

To create a Linux image to run a build:
- Ensure you have Docker Desktop and PowerShell installed, and that Docker is running
- Select "Create Container"
- In the new window that opened, enter a server image name (something like "linux_image" etc)
- Enter your TCP or UDP port (for example: 7777)
- Once the program builds and uploads the container, return to PFAdmin
- Select "Create Server Build"
- Set up your server like you would in the Game Manager
- Add your server Asset to the "Assets" field, make sure you set the mount point (or leave it at the default '/data/Assets/')
- Set your start game command (for example: /data/Assets/GameServer.x86_64)
- Select "Create Build"
- The next time you want to update your server, you can just reuse the same base image. 

# How To Update Your Game Server
- Run PFAdmin
- Upload your new server asset
- Select "Create Server Build", and select your new asset
- Change the "Start Game Command" to reflect your new asset name
- Create the build

# CLI
- Available Commands: 
    * *-cli* (Required)
    * *-titleid [TitleID]* (Required; Logs in with TitleID)
    * *-key [SecretKey]* (Required; Logs in with Secret Key)
    * *-uploadasset [/Path/To/Asset]* (Optional; Uploads an asset)
    * *-config [/Path/To/JSON Build Config]* (Optional; Creates the build with this configuration)
    * *-overrideasset [UploadedAssetName]* (Optional; Overrides the asset in the build JSON)
    * *-createbuild* (From JSON config) (Optional; Created a build)
    * *-editqueue [Queue Name]* (Optional; Adds created build to this Matchmaking Queue for allocation)
    
     
- The application should return exit code '0' on successful completion of the command. If there's an error, it will exit with code '1'.
- For an example .json file for a build configuration, see [here](https://github.com/bphillips09/PFAdmin/blob/master/build.json).
- CLI Examples:
    * This configuration will 
        * Upload the .zip
        * Create the build with the .zip as the server asset (if overridden) or use the asset specified in the build JSON
        * Set the selected Matchmaking queue to allocate this server build when a match is made
    * Windows
        ```
        .\PFACLI.exe -cli -titleid [TITLE ID] -key [SECRET KEY] -uploadasset "[Path to Server .zip]" -config "[Path to Build Config .json]" -overrideasset "[Asset Name .zip]" -createbuild -editqueue [Queue Name]
        ```
    * Mac
        ```
        .\PFACLI.app -cli -titleid [TITLE ID] -key [SECRET KEY] -uploadasset "[Path to Server .zip]" -config "[Path to Build Config .json]" -overrideasset "[Asset Name .zip]" -createbuild -editqueue [Queue Name]
        ```
    * Linux
        ```
        .\PFACLI.x86_64 -cli -titleid [TITLE ID] -key [SECRET KEY] -uploadasset "[Path to Server .zip]" -config "[Path to Build Config .json]" -overrideasset "[Asset Name .zip]" -createbuild -editqueue [Queue Name]
        ```

# Dependencies
- [Docker](https://www.docker.com/products/docker-desktop) and [PowerShell](https://github.com/PowerShell/PowerShell/releases/latest)

# FAQ
What is it?
- An application for creating/updating/pushing/managing PlayFab Linux-based Game Servers (and some other functionality like segmented player management)

What is it not?
- A full replacement for PlayFab's Game Manager

Why is your code so bad?
- Because I wrote this in a couple hours
