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
        [Test]
        public void CyborgConversionToggle() {
            GameObject go = new GameObject();
            LevelManager lev = go.AddComponent<LevelManager>();
            lev.ressurectionActive = new bool[14];
            for (int i=0; i<14; i++) lev.ressurectionActive[i] = false;
            lev.currentLevel = -1; // Test area
            lev.CyborgConversionToggleForCurrentLevel();
            lev.currentLevel = 0;
            bool prev = lev.ressurectionActive[0];
            lev.CyborgConversionToggleForCurrentLevel();
            // Should have early exited else will give oob exception.
            
            bool check = lev.ressurectionActive[0] != prev
                         && lev.ressurectionActive[0];
            string msg = "Reactor cyborg conversion failed to toggle";
            
            prev = lev.ressurectionActive[1];
            lev.currentLevel = 1;
            lev.CyborgConversionToggleForCurrentLevel();
            check = lev.ressurectionActive[1] != prev
                    && lev.ressurectionActive[1];
            msg = "Medical cyborg conversion failed to toggle";
            Assert.That(check,msg);
            
            prev = lev.ressurectionActive[6];
            lev.currentLevel = 6;
            lev.CyborgConversionToggleForCurrentLevel();
            check = lev.ressurectionActive[6] != prev
                    && lev.ressurectionActive[6]
                    && lev.ressurectionActive[10]
                    && lev.ressurectionActive[11]
                    && lev.ressurectionActive[12];
            msg = "Executive and Groves cyborg conversion failed to activate";
            Assert.That(check,msg);
            
            prev = lev.ressurectionActive[12];
            lev.CyborgConversionToggleForCurrentLevel();
            check = lev.ressurectionActive[12] != prev
                    && !lev.ressurectionActive[6]
                    && !lev.ressurectionActive[10]
                    && !lev.ressurectionActive[11]
                    && !lev.ressurectionActive[12];
            msg = "Executive and Groves cyborg conversion failed to deactivate";
            Assert.That(check,msg);
        }
    }
}
