[![Header](.github/images/header.png)](https://www.lightmattergame.com/)

# Cached Shadow Maps

Cached Shadow Maps is a unity package for caching shadow maps on unity's built in lighting system, storing
 them (while valid) for
 later
 use effectively reduces the number of shadows casters to be rendered each frame. This include
  automatic recomputation when a light source context is detected to be dirty.

---

_Cached Shadow Maps was developed for the published first person
 puzzle game [Lightmatter](https://www.lightmattergame.com/), this technology is now released to the public under Apache License 2.0_


---

<p align="center" width="100%">
  <a href="https://unity3d.com/">
    <img alt="unity" src=".github/images/unity.svg" height="40" align="left">
  </a>
  <a href="https://docs.microsoft.com/en-us/dotnet/csharp/index">
    <img alt="csharp" src=".github/images/csharp.svg" height="40" align="center">
  </a>
</p>

## Usage

- Edit your Unity projects "Packages/manifest.json" to include the string
  `"com.cnheider.cachedshadowmaps": "https://github.com/cnheider/cachedshadowmaps.git"}`.

  Example `manifest.json`
  ````
  {
    "dependencies": {
      "com.unity.package-manager-ui": "0.0.0-builtin",
      ...
      "com.aivclab.cachedshadowmaps": "https://github.com/aivclab/cachedshadowmaps.git",
    }
  }
  ````
  You can use `"com.aivclab.cachedshadowmaps": "https://github.com/aivclab/cachedshadowmaps.git#branch"` for a specific
   branch.

***Or***

- Download the newest CSM.unitypackage from [releases](https://github.com/cnheider/cachedshadowmaps/releases
) and
 import into your Unity project.

***Or***

- Acquire the [Cached Shadow Maps (Temporarily down)](http://u3d.as/14cC) package from the built-in asset
 store of the Unity Editor.

## Demo

![demo](.github/images/demo.gif)

## Repository Structure
---
<!--        ├  └  ─  │        -->
    cnheider/cachedshadowmaps         # This repository
    │
    ├── Samples                  # Sample Project
    │
    ├── Editor                   # Editor Implementation
    │
    ├── Runtime                 # Runtime Implementation
    │
    ├── Documentation           # Unity Package Documentation
    │
    ├── Gizmos                  # Icons
    │
    ├── Tests                   # Tests
    │
    ├── .github                 # Images and such for this README
    │
    ├── LICENSE.md              # License file (Important but boring)
    ├── README.md               # The top-level README
    └── TUTORIAL.md             # Very quick tutorial to get you started
---

# Citation

For citation you may use the following bibtex entry:
````
@misc{cachedshadowmaps,
  author = {Heider, Christian},
  title = {Cached Shadow Maps},
  year = {2020},
  publisher = {GitHub},
  journal = {GitHub repository},
  howpublished = {\url{https://github.com/cnheider/cachedshadowmaps}},
}
````

