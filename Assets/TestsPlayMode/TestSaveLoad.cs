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
        
        private static int GetTotalDescendantCount(GameObject go) {
            int count = 0;
            foreach (Transform child in go.transform) {
                count++; // Count the direct child
                count += GetTotalDescendantCount(child.gameObject); // Recursively count its descendants
            }
            
            return count;
        }
        
        private string WriteHierarchyCounts() {
            List<GameObject> allParents = SceneManager.GetActiveScene().GetRootGameObjects().ToList();
            int childCount = 0;
            GameObject childGO = null;
            StringBuilder s1 = new StringBuilder();
            s1.Append(Environment.NewLine);
            s1.Append("GameObject Hierarchy Counts::");
            for (int i=0;i<allParents.Count;i++) {
                s1.Append(Environment.NewLine);
                if (allParents[i] == null) continue;
                
                s1.Append(allParents[i].name);
                childCount = allParents[i].transform.childCount;
                if (childCount < 1) {  s1.Append(":0"); continue; }
                
                s1.Append(":");
                s1.Append(childCount.ToString());
                for (int j=0;j<childCount;j++) {
                    s1.Append(Environment.NewLine);
                    childGO = allParents[i].transform.GetChild(j).gameObject;
                    if (childGO == null) continue;
                    
                    s1.Append(allParents[i].name);
                    s1.Append(":");
                    s1.Append(childGO.name);
                    s1.Append(":");
                    int level2CountOfAllContained = GetTotalDescendantCount(childGO);
                    s1.Append(level2CountOfAllContained.ToString()); // 
                }
            }
            
            s1.Append(Environment.NewLine); // EOF newline for Linux systems.
            return s1.ToString();
        }
        
        [UnityTest]
        public IEnumerator ObjectCountsPreservedPreSaveVsPostLoad() {
            RunBeforeAnyTests();
            yield return new WaitUntil(() => sceneLoaded);

            yield return new WaitForSeconds(2f);
            
            MainMenuHandler.a.StartGame(true);
            yield return new WaitForSeconds(2f);
            PlayerMovement.a.hm.god = true;
            string countResultsPath = Utils.SafePathCombine(Application.streamingAssetsPath,"CitadelHierarchyCounts.txt");
//             List<GameObject> npcPrefabsPrevious = new List<GameObject>();
//             List<GameObject> npcPrefabsAfter = new List<GameObject>();
//             int npcPrefabCount = 0;
            for (int i=0;i<13;i++) {
                ConsoleEmulator.CheatLoadLevel(i);
                yield return new WaitForSeconds(2f);
                
                List<GameObject> erthang = Utils.GetAllObjectsOnlyInScene();
                int countPreSave = erthang.Count;
                string countsPriorString = "-- Counts Prior to Save --" + WriteHierarchyCounts();
               
//                 npcPrefabsPrevious.Clear();
//                 npcPrefabCount = LevelManager.a.npcContainers[0].transform.childCount; // Reactor level
//                 for (int prevI=0;prevI<npcPrefabCount;prevI++) npcPrefabsPrevious.Add(LevelManager.a.npcContainers[0].transform.GetChild(prevI).gameObject);
                
                MainMenuHandler.a.SaveGame(7,"quicksave");
                yield return new WaitForSeconds(2f);
                MainMenuHandler.a.LoadGame(7);
                erthang.Clear();
                erthang = Utils.GetAllObjectsOnlyInScene();
                bool check = erthang.Count == countPreSave;
                
//                 npcPrefabsAfter.Clear();
//                 npcPrefabCount = LevelManager.a.npcContainers[0].transform.childCount; // Reactor level
//                 for (int aftI=0;aftI<npcPrefabCount;aftI++) npcPrefabsAfter.Add(LevelManager.a.npcContainers[0].transform.GetChild(aftI).gameObject);
                
                yield return new WaitForSeconds(5f); // Absolutely necessary to return function to LoadRoutine Coroutine's yields!
                
                string msg = "Number of save items was different on level " + i.ToString() + ", preSave: " + countPreSave.ToString() + ", postLoad: " + erthang.Count.ToString();
                if (!check) {
//                     for (int outer=0;outer<npcPrefabsPrevious.Count;outer++) {
//                         bool foundMatch = false;
//                         for (int inner=0;inner<npcPrefabsAfter.Count;inner++) {
//                             if (npcPrefabsPrevious[outer].name == npcPrefabsAfter[inner].name) foundMatch = true;
//                         }
//                         
//                         if (!foundMatch) UnityEngine.Debug.Log("No match found for previous GO: " + npcPrefabsPrevious[outer].name);
//                     }
//                     
//                     for (int outer=0;outer<npcPrefabsAfter.Count;outer++) {
//                         bool foundMatch = false;
//                         for (int inner=0;inner<npcPrefabsPrevious.Count;inner++) {
//                             if (npcPrefabsAfter[outer].name == npcPrefabsPrevious[inner].name) foundMatch = true;
//                         }
//                         
//                         if (!foundMatch) UnityEngine.Debug.Log("No match found for after GO: " + npcPrefabsPrevious[outer].name);
//                     }
                    string[] resultsBefore = countsPriorString.Split(Environment.NewLine.ToCharArray());
                    string countsAfterString = "-- Counts After Load --" +  WriteHierarchyCounts();
                    string[] resultsAfter = countsAfterString.Split(Environment.NewLine.ToCharArray());
                    File.WriteAllLines(countResultsPath,resultsBefore,Encoding.ASCII);
                    File.AppendAllLines(countResultsPath,resultsAfter,Encoding.ASCII);
                }
                Assert.That(check,msg);
                if (check) UnityEngine.Debug.Log("Number of save items was the same for level " + i.ToString() + ", preSave: " + countPreSave.ToString() + ", postLoad: " + erthang.Count.ToString());
            }
            
//             // Now save, leave level to another, load back
//             for (int i=0;i<13;i++) {
//                 ConsoleEmulator.CheatLoadLevel(i);
//                 yield return new WaitForSeconds(2f);
//                 
//                 List<GameObject> erthang = Utils.GetAllObjectsOnlyInScene();
//                 int countPreSave = erthang.Count;
//                 string countsPriorString = "-- Counts Prior to Save --" + WriteHierarchyCounts();
//                 MainMenuHandler.a.SaveGame(7,"quicksave");
//                 yield return new WaitForSeconds(2f);
//                 int nextLev = (i + 1) >= 13 ? 0 : i + 1;
//                 ConsoleEmulator.CheatLoadLevel(nextLev);
//                 yield return new WaitForSeconds(2f);
//                 MainMenuHandler.a.LoadGame(7);
//                 erthang.Clear();
//                 erthang = Utils.GetAllObjectsOnlyInScene();
//                 bool check = erthang.Count == countPreSave;
//                 yield return new WaitForSeconds(5f); // Absolutely necessary to return function to LoadRoutine Coroutine's yields!
//                 string msg = "Number of save items was different on level " + i.ToString() + " after having changed to level " + nextLev.ToString() + ", preSave: " + countPreSave.ToString() + ", postLoad: " + erthang.Count.ToString();
//                 if (!check) {
//                     string[] resultsBefore = countsPriorString.Split(Environment.NewLine.ToCharArray());
//                     string countsAfterString = "-- Counts After Load --" +  WriteHierarchyCounts();
//                     string[] resultsAfter = countsAfterString.Split(Environment.NewLine.ToCharArray());
//                     File.WriteAllLines(countResultsPath,resultsBefore,Encoding.ASCII);
//                     File.AppendAllLines(countResultsPath,resultsAfter,Encoding.ASCII);
//                 }
//                 Assert.That(check,msg);
//                 if (check) UnityEngine.Debug.Log("Number of save items was the same for level " + i.ToString() + " after having changed to level " + nextLev.ToString() +  ", preSave: " + countPreSave.ToString() + ", postLoad: " + erthang.Count.ToString());
//             }
//             
//             // Now, save, leave level, come back to level, then load
//             for (int i=0;i<13;i++) {
//                 ConsoleEmulator.CheatLoadLevel(i);
//                 yield return new WaitForSeconds(2f);
//                 
//                 List<GameObject> erthang = Utils.GetAllObjectsOnlyInScene();
//                 int countPreSave = erthang.Count;
//                 string countsPriorString = "-- Counts Prior to Save --" + WriteHierarchyCounts();
//                 MainMenuHandler.a.SaveGame(7,"quicksave");
//                 yield return new WaitForSeconds(2f);
//                 int nextLev = (i + 1) >= 13 ? 0 : i + 1;
//                 ConsoleEmulator.CheatLoadLevel(nextLev);
//                 yield return new WaitForSeconds(2f);
//                 ConsoleEmulator.CheatLoadLevel(i);
//                 yield return new WaitForSeconds(2f);
//                 MainMenuHandler.a.LoadGame(7);
//                 erthang.Clear();
//                 erthang = Utils.GetAllObjectsOnlyInScene();
//                 bool check = erthang.Count == countPreSave;
//                 yield return new WaitForSeconds(5f); // Absolutely necessary to return function to LoadRoutine Coroutine's yields!
// 
//                 string msg = "Number of save items was different on level " + i.ToString() + " after having changed to level " + nextLev.ToString() + " and came back to " + i.ToString() + ", preSave: " + countPreSave.ToString() + ", postLoad: " + erthang.Count.ToString();
//                 if (!check) {
//                     string[] resultsBefore = countsPriorString.Split(Environment.NewLine.ToCharArray());
//                     string countsAfterString = "-- Counts After Load --" +  WriteHierarchyCounts();
//                     string[] resultsAfter = countsAfterString.Split(Environment.NewLine.ToCharArray());
//                     File.WriteAllLines(countResultsPath,resultsBefore,Encoding.ASCII);
//                     File.AppendAllLines(countResultsPath,resultsAfter,Encoding.ASCII);
//                 }
//                 Assert.That(check,msg);
//                 if (check) UnityEngine.Debug.Log("Number of save items was the same for level " + i.ToString() + " after having changed to level " + nextLev.ToString() + " and came back to " + i.ToString() + ", preSave: " + countPreSave.ToString() + ", postLoad: " + erthang.Count.ToString());
//             }
            Utils.CopyLogFiles(false);
        }
    }
}
