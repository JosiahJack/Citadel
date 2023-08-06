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
            yield return new WaitForSeconds(5f);
            MainMenuHandler.a.SaveGame(7,"quicksave");
            yield return new WaitForSeconds(5f);
            MainMenuHandler.a.LoadGame(7);
            yield return new WaitForSeconds(10f);
            Utils.CopyLogFiles(false);
        }
    }
}
