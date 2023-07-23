using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Tests {
    public class TestLevelManager {
        [UnityTest]
        public IEnumerator TestAwakeWhiteroom() {
            LevelManager lev = new LevelManager();
            Const con = new Const();
            Const.a = con;
            GameObject camGO = new GameObject();
            Const.a.player1CapsuleMainCameragGO = camGO;
            Camera cam = camGO.AddCompenent<Camera>();
            cam.useOcclusionCulling = true;
            lev.currentLevel = -1; // Test area
            lev.Awake();
            
            bool check = cam.useOcclusionCulling == false;
            string msg = "Wrong camera culling settin for test area";
            Assert.That(check,msg);
        }
    }
}
