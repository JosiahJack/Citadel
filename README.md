# Citadel

[![DownloadsTotal](https://img.shields.io/github/downloads/JosiahJack/Citadel/total?color=teal&label=Downloads)](https://github.com/JosiahJack/Citadel/releases)
[![Hits](https://hits.seeyoufarm.com/api/count/incr/badge.svg?url=https%3A%2F%2Fgithub.com%2FJosiahJack%2FCitadel&count_bg=%23336B6A&title_bg=%23555555&icon=&icon_color=%23E7E7E7&title=Page+Hits&edge_flat=false)](https://hits.seeyoufarm.com)

[![Build](https://github.com/JosiahJack/Citadel/actions/workflows/main.yml/badge.svg)](https://github.com/JosiahJack/Citadel/actions/workflows/main.yml)
[![Tests](https://github.com/JosiahJack/Citadel/actions/workflows/runtests.yml/badge.svg?branch=master)](https://github.com/JosiahJack/Citadel/actions/workflows/runtests.yml)

### News
We are in Beta now!  Bugs are very likely for the next several patch releases.
Get the latest here: https://github.com/JosiahJack/Citadel/tags

- Sincerely, Josiah 3/14/2022

### Information
[![tag](https://img.shields.io/github/v/tag/JosiahJack/Citadel?label=Latest%20Release)](https://github.com/JosiahJack/Citadel/releases)

Started 7/6/2014

The System Shock Fan Remake.  The goal is to recreate the original closely while enhancing it with 3D models instead of 2D sprites, 3D details to the station in and out, particle effects, and subtle sound effect additions.  After releasing a playable version, focus will shift to making mod and mapping tools.

PLEASE submit bug and feature requests here on this github.

If you would like to join and aid in any capacity, please email Josiah Jack, the main author, at josiahjackcitadel@gmail.com

Special thanks to Looking Glass Studios and Origin Games for the original 1994 product. 
Special thanks to Night Dive Studios for allowing this project to live on (unhindered).

>DISCLAIMER: Citadel is in-progress. That means features may be broken, missing, or not in a final state of polish.

### System Requirements:
- Vulkan, Direct3D11 or higher, or OpenGL 3.0 support or higher.  On systems that do not support Vulkan, it may be necessary to force the game to launch in OpenGL with argument `-force-glcore`.
- Windows 7 or higher, MacOS 10.12 or newer, Linux Ubuntu 16.04 and newer or CentOS 7 and newer.
- At least 1GB video RAM
- At least 3GB RAM
- At least 1.4GB hard drive space
- At least 3 CPU threads
- X11 on Linux systems

### Install Instructions:
Everything needed to play should be in the zip, though at first launch the path to your original game install .RES files is needed for audio.  If installing a newer version, replace the old version.  Saves should be compatible unless noted in the tag release notes.

1.  Download the latest tagged release.
2.  Extract it to a location of your choosing.  For best results, install in your user directory so that Citadel has read/write access for save files.
3.  Launch Citadel.exe if on Windows, or Citadelv#.##.x86_64 on Linux (mark as executable if not already).

---

### Developer Dependencies:
- Must have [Blender 2.79b](https://download.blender.org/release/Blender2.79/) or later installed and on your system path (happens as a part of the install msi on Windows systems, symlink in /usr/bin/ on Linux can be manually created).  This is for Unity to be able to import .blend files as .FBX by running blender in the background when first loading the project.
- Must have [Texture2DArray Importer v1.5.0](https://github.com/pschraut/UnityTexture2DArrayImportPipeline) in order for level geometry to display properly as they use a single material with array texture lookup for a substantial performance gain.
- Texture arrays require Vulkan, Direct3D11+, or OpenGL 3.0 support or higher.  To support building all api's, the dev packages for each will need installed.
- To support launching editor as OpenGL only on older systems that do not have Vulkan by default, add `-force-glcore` comand argument when launching the editor.

### Dev Play Instructions (In Editor):

1. Download the Github repository (or clone it via git).
2. Install UnityVersion2019.4.35f1 Personal: https://unity3d.com/get-unity/download/archive
3. Extract all into a folder and name of your liking.
4. Open Unity.
5. Click on Open Other or Open Project (the name if the button changes depending on whether it is a fresh install or already installed.)
6. Select the top folder where you extracted the game files. It must be only one level above the Assets folder, e.g. Citadel-v0.1/Assets.
7. Wait as Unity loads in the project and reimports all assets and auto-compiles the script code (may take an hour for the very first time).
8. Either A. Click the play button at the top of the screen (it's a triangle), or B. In the top menu bar go to File->Build & Run.

---

### Cyberspace Jump Points

- Dev Log: https://www.systemshock.org/index.php?topic=6977.0
- InsieDB page: https://www.indiedb.com/games/citadel
- Discord: https://discord.gg/mrmkMCD
