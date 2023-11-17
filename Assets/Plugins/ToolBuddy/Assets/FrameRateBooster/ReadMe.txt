// =====================================================================
// Copyright 2018-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

Frame Rate Booster increases the frame rate of Unity based applications with zero effort from you.

USAGE
=====
  * Have your project use the Mono scripting backend. If you don't know what scripting backend your project uses, you can find that setting in Unity's "Player Settings" then "Other Settings"
  * Import the package
  * Rebuild your game
  * Your game is now optimized  

HOW IT WORKS
============
Frame Rate Booster implements some commonly used Unity operations in a more optimized way. When you build your application, Frame Rate Booster will make it use the optimized version of those operations rather than Unity's.

EXPECTED FRAME RATE GAIN
========================
It depends on how heavily your code relies on operations on vectors, quaternions and similar objects. The more such operations there are, the better the optimization will be.
* On benchmarks, I had a 10% increase.
* On my other asset, Curvy Splines, I got also a 10% increase for operations like mesh generation and splines cache building.
* On games doing thousands of geometry operations per frame (like moving a lot of objects), I expect a few percent increase at most. Not too much, but hey, it's free!
* On the remaining situations, I don't expect any noticeable increase.

COMPATIBILITY WITH IL2CPP
=========================
Short answer: probably not. Long answer: https://forum.curvyeditor.com/thread-861.html

COMPATIBILITY WITH ANDROID
==========================
You will need to manually unpack you apk file, run Frame Rate Booster code on the mono assemblies in the apk, then repack it. More details are provided in the console once you do a build that needs such operations. If you find a way to automate this, please let me know.

LICENSE
=======
Asset is governed by the Asset Store EULA.

Contact & Support
=================
If you have any questions, feedback or requests, please write to admin@toolbuddy.net

VERSION HISTORY
===============
1.3.0
	Fixed build with Unity versions 2021 or newer
	The OptimizeBuild method now takes as an input the path to build directory instead of the path to a file in that directory
	Changed folders hierarchy (to unify it with other ToolBuddy assets)
1.2.1
	Fixed compatibility issue with Multiplayer HLAPI package.
1.2.0
	FRB can now be run on IL2CPP builds
	Optimize now return the number of optimized methods
	Enhanced some log messages
	Now disposes assemblies when finished with them, that avoids errors in some scenarios (IL2CPP build being one of them)
1.1.1
	Corrected the error message when multiple builds are detected inside the target folder
1.1.0
	Added warnings and helpful logs when trying to use Frame Rate Booster with unsupported platforms
1.0.0
	First release