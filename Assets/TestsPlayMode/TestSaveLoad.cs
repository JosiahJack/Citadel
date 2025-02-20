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
    public class TestSaveLoad {
        private bool sceneLoaded = false;

        //private IEnumerator LoadSceneAsync(string sceneName) {
        //    var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        //    while (!asyncLoad.isDone) {
        //        yield return null;
        //    }
        //}

        public void RunBeforeAnyTests() {
            if (sceneLoaded) return;

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("CitadelScene");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.name == "CitadelScene") {
                // Unsubscribe from the event to avoid handling it multiple
                // times
                SceneManager.sceneLoaded -= OnSceneLoaded;
                sceneLoaded = true; // Indicate that the scene is loaded.
            }
        }

        [UnityTest]
        public IEnumerator ConfirmInstancesOfCoreClassesCreated() {
            RunBeforeAnyTests();
            yield return new WaitUntil(() => sceneLoaded);

            yield return new WaitForSeconds(2f);
            
            bool check = Const.a != null;
            string msg = "Const.a was null";
            Assert.That(check,msg);

            check = LevelManager.a != null;
            msg = "LevelManager.a was null";
            Assert.That(check,msg);

            check = MFDManager.a != null;
            msg = "MFDManager.a was null";
            Assert.That(check,msg);

            check = Automap.a != null;
            msg = "Automap.a was null";
            Assert.That(check,msg);

            check = MainMenuHandler.a != null;
            msg = "MainMenuHandler.a was null";
            Assert.That(check,msg);
        }
        
        [UnityTest]
        public IEnumerator SaveLoadSimple() {
            RunBeforeAnyTests();
            yield return new WaitUntil(() => sceneLoaded);

            yield return new WaitForSeconds(2f);

            bool check = MainMenuHandler.a != null;
            string msg = "MainMenuHandler.a was null";
            Assert.That(check,msg);
            
            MainMenuHandler.a.StartGame(true);
            yield return new WaitForSeconds(2f);
            MainMenuHandler.a.SaveGame(7,"quicksave");
            yield return new WaitForSeconds(2f);
            MainMenuHandler.a.LoadGame(7);
            yield return new WaitForSeconds(2f);
            Utils.CopyLogFiles(false);
        }
        
        [UnityTest]
        public IEnumerator ObjectCountsPreservedPreSaveVsPostLoad() {
            RunBeforeAnyTests();
            yield return new WaitUntil(() => sceneLoaded);

            yield return new WaitForSeconds(2f);
            
            MainMenuHandler.a.StartGame(true);
            yield return new WaitForSeconds(2f);
            PlayerMovement.a.hm.god = true;
            for (int i=0;i<14;i++) {
                ConsoleEmulator.CheatLoadLevel(i);
                yield return new WaitForSeconds(2f);
                
                List<GameObject> erthang = Utils.GetAllObjectsOnlyInScene();
                int countPreSave = erthang.Count;
                MainMenuHandler.a.SaveGame(7,"quicksave");
                yield return new WaitForSeconds(2f);
                MainMenuHandler.a.LoadGame(7);
                erthang.Clear();
                erthang = Utils.GetAllObjectsOnlyInScene();
                bool check = erthang.Count == countPreSave;
                string msg = "Number of save items was different on level " + i.ToString() + ", preSave: " + countPreSave.ToString() + ", postLoad: " + erthang.Count.ToString();
                Assert.That(check,msg);
                if (check) UnityEngine.Debug.Log("Number of save items was the same for level " + i.ToString() + ", preSave: " + countPreSave.ToString() + ", postLoad: " + erthang.Count.ToString());
            }
            
            // Now save, leave level to another, load back
            for (int i=0;i<14;i++) {
                ConsoleEmulator.CheatLoadLevel(i);
                yield return new WaitForSeconds(2f);
                
                List<GameObject> erthang = Utils.GetAllObjectsOnlyInScene();
                int countPreSave = erthang.Count;
                MainMenuHandler.a.SaveGame(7,"quicksave");
                yield return new WaitForSeconds(2f);
                int nextLev = (i + 1) >= 13 ? 0 : i + 1;
                ConsoleEmulator.CheatLoadLevel(nextLev);
                yield return new WaitForSeconds(2f);
                MainMenuHandler.a.LoadGame(7);
                erthang.Clear();
                erthang = Utils.GetAllObjectsOnlyInScene();
                bool check = erthang.Count == countPreSave;
                string msg = "Number of save items was different on level " + i.ToString() + " after having changed to level " + nextLev.ToString() + ", preSave: " + countPreSave.ToString() + ", postLoad: " + erthang.Count.ToString();
                Assert.That(check,msg);
                if (check) UnityEngine.Debug.Log("Number of save items was the same for level " + i.ToString() + " after having changed to level " + nextLev.ToString() +  ", preSave: " + countPreSave.ToString() + ", postLoad: " + erthang.Count.ToString());
            }
            
            // Now, save, leave level, come back to level, then load
            for (int i=0;i<14;i++) {
                ConsoleEmulator.CheatLoadLevel(i);
                yield return new WaitForSeconds(2f);
                
                List<GameObject> erthang = Utils.GetAllObjectsOnlyInScene();
                int countPreSave = erthang.Count;
                MainMenuHandler.a.SaveGame(7,"quicksave");
                yield return new WaitForSeconds(2f);
                int nextLev = (i + 1) >= 13 ? 0 : i + 1;
                ConsoleEmulator.CheatLoadLevel(nextLev);
                yield return new WaitForSeconds(2f);
                ConsoleEmulator.CheatLoadLevel(i);
                yield return new WaitForSeconds(2f);
                MainMenuHandler.a.LoadGame(7);
                erthang.Clear();
                erthang = Utils.GetAllObjectsOnlyInScene();
                bool check = erthang.Count == countPreSave;
                string msg = "Number of save items was different on level " + i.ToString() + " after having changed to level " + nextLev.ToString() + " and came back to " + i.ToString() + ", preSave: " + countPreSave.ToString() + ", postLoad: " + erthang.Count.ToString();
                Assert.That(check,msg);
                if (check) UnityEngine.Debug.Log("Number of save items was the same for level " + i.ToString() + " after having changed to level " + nextLev.ToString() + " and came back to " + i.ToString() + ", preSave: " + countPreSave.ToString() + ", postLoad: " + erthang.Count.ToString());
            }
            Utils.CopyLogFiles(false);
        }
    }
}
