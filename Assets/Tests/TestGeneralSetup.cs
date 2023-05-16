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
    public class TestGeneralSetup {
        public CitadelTests tester;

        [Test]
        public void TestGeneralSetupSimplePasses() {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            GameObject go = GameObject.Find("GlobalConsts");
            if (go != null) tester = go.GetComponent<CitadelTests>();
            Assert.That(tester != null,"Unable to find CitadelTests attached to a GameObject GlobalConsts in scene");
            int i=0;
            int k=0;
            List<GameObject> allGOs = new List<GameObject>();
            //Scene citmain = SceneManager.GetActiveScene();
            Scene citmain = SceneManager.GetSceneByName("CitadelScene");
            Assert.That(citmain.name == "CitadelScene","Not testing main Citadel scene, instead testing: " + citmain.name);
            List<GameObject> allParents = citmain.GetRootGameObjects().ToList();
            Assert.That(allParents.Count > 1,"Failed to populate allParents:" + allParents.Count.ToString());
            for (i=0;i<allParents.Count;i++) {
                Component[] compArray = allParents[i].GetComponentsInChildren(typeof(Transform),true);
                for (k=0;k<compArray.Length;k++) {
                    allGOs.Add(compArray[k].gameObject); // Add to full list, separate so we don't infinite loop
                }
            }

            Assert.That(allGOs.Count > 1,"CitadelTests.SetupLists failed to populate allGOs:" + allGOs.Count.ToString());
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        //[UnityTest]
        //public IEnumerator TestGeneralSetupWithEnumeratorPasses() {
            //// Use the Assert class to test conditions.
            //// Use yield to skip a frame.
            //GameObject go = GameObject.Find("GlobalConsts");
            //if (go != null) tester = go.GetComponent<CitadelTests>();
            //Assert.That(tester != null,"Unable to find CitadelTests attached to a GameObject GlobalConsts in scene");
            //int i=0;
            //int k=0;
            //List<GameObject> allGOs = new List<GameObject>();
            //List<GameObject> allParents = SceneManager.GetActiveScene().GetRootGameObjects().ToList();
            //Assert.That(allParents.Count > 1,"Failed to populate allParents:" + allParents.Count.ToString());
            //for (i=0;i<allParents.Count;i++) {
            //    Component[] compArray = allParents[i].GetComponentsInChildren(typeof(Transform),true);
            //    for (k=0;k<compArray.Length;k++) {
            //        allGOs.Add(compArray[k].gameObject); // Add to full list, separate so we don't infinite loop
            //    }
            //}

            //Assert.That(allGOs.Count > 1,"CitadelTests.SetupLists failed to populate allGOs:" + allGOs.Count.ToString());
        //    yield return null;
        //}
    }
}
