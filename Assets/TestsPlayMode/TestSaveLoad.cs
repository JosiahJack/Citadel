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
        public CitadelTests tester;
        public List<GameObject> allGOs;
        private List<GameObject> allParents;
        private List<TargetIO> allTIOs;
        private bool acquiredGOs;
        private bool sceneLoaded;

        public void RunBeforeAnyTests() {
            if (sceneLoaded) return;

            SceneManager.LoadScene("CitadelScene");

            Scene citmain = SceneManager.GetSceneByName("CitadelScene");
            if (citmain.name != "CitadelScene") {
            }
        }

        public void SetupTests() {
            //if (acquiredGOs && allGOs != null && tester != null) return;

            //Scene citmain = SceneManager.GetSceneByName("CitadelScene");
            //string msg = "Not testing main Citadel scene, instead testing: ";
            //Assert.That(citmain.name == "CitadelScene",msg + citmain.name);

            //// Use the Assert class to test conditions.
            //// Use yield to skip a frame.
            //GameObject go = GameObject.Find("GlobalConsts");
            //if (go != null) tester = go.GetComponent<CitadelTests>();
            //msg = "Unable to find CitadelTests attached to a GameObject with "
            //      + "name GlobalConsts in scene";

            //Assert.That(tester != null,msg);

            //int i=0;
            //int k=0;
            //allGOs = new List<GameObject>();
            //allParents = citmain.GetRootGameObjects().ToList();
            //msg = "Failed to populate allParents: ";
            //Assert.That(allParents.Count > 1,msg + allParents.Count.ToString());

            //for (i=0;i<allParents.Count;i++) {
            //    Component[] compArray =
            //      allParents[i].GetComponentsInChildren(typeof(Transform),true);

            //    for (k=0;k<compArray.Length;k++) {
            //        // Add to full list, separate so we don't infinite loop
            //        allGOs.Add(compArray[k].gameObject);
            //    }
            //}
            //acquiredGOs = true;
        }

        private bool SceneLoaded() {
            return sceneLoaded;
            //Scene citmain = SceneManager.GetSceneByName("CitadelScene");
            //if (citmain == null) return false;
            //if (citmain.name != "CitadelScene") return false;

            //SceneManager.SetActiveScene(citmain);
            //return true;
        }

        // Custom method to be called when a scene is loaded
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.name == "CitadelScene") {
                // Unsubscribe from the event to avoid handling it multiple times
                SceneManager.sceneLoaded -= OnSceneLoaded;

                // Set the flag to indicate that the scene is loaded
                sceneLoaded = true;
            }
        }

        [UnityTest]
        public IEnumerator ConfirmInstancesOfCoreClassesCreated() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            //SetupTests();

            //string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            //Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());

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

            check = BiomonitorGraphSystem.a != null;
            msg = "BiomonitorGraphSystem.a was null";
            Assert.That(check,msg);
        }
    }
}
