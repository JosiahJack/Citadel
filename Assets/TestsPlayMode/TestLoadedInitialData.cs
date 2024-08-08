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
    public class TestLoadedInitialData {
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
        public IEnumerator CheckConstInitialization() {
            RunBeforeAnyTests();
            yield return new WaitUntil(() => sceneLoaded);

            yield return new WaitForSeconds(2f);
            
            bool check = Const.a != null;
            string msg = "Const.a was null";
            Assert.That(check,msg);
            
            msg = "Const.a useableItems has unassigned slots!";
            check = true;
            for (int i=0;i<Const.a.useableItems.Length;i++) {
                if (Const.a.useableItems[i] == null) check = false;
            }
            Assert.That(check,msg);


            msg = "Const.a useableItemsFrobIcons has unassigned slots!";
            check = true;
            for (int i=0;i<Const.a.useableItemsFrobIcons.Length;i++) {
                if (Const.a.useableItemsFrobIcons[i] == null) check = false;
            }
            Assert.That(check,msg);
            //msg = "Placeholder";
            //check = true;
            //Assert.That(check,msg);
        }

        [UnityTest]
        public IEnumerator CheckLogsLoadedHaveAssignedAudio() {
            RunBeforeAnyTests();
            yield return new WaitUntil(() => sceneLoaded);

            yield return new WaitForSeconds(2f);
            
            bool check = Const.a.audioLogType.Length == Const.a.audioLogs.Length;
            string msg = "Const.a.audioLogType length of "
                         + Const.a.audioLogType.Length.ToString()
                         + " not equal to Const.a.audioLogs length of "
                         + Const.a.audioLogs.Length.ToString();
            Assert.That(check,msg);
            
            //for (int i=0; i< Const.a.audioLogType.Length;i++) {
            for (int i=0; i< Const.a.audioLogs.Length;i++) {
                if (Const.a.audioLogType[i] == AudioLogType.TextOnly) continue;
                if (Const.a.audioLogType[i] == AudioLogType.Papers) continue;
                if (Const.a.audioLogType[i] == AudioLogType.Vmail) continue;
                if (Const.a.audioLogType[i] == AudioLogType.Game) continue;

                // Only normal and emails have .wav files
                // 6 is a known TextOnly and has null.wav assigned to it.
                check = Const.a.audioLogs[i] != Const.a.audioLogs[6];
                msg = "audioLogs " + i.ToString() + " using null.wav";
                Assert.That(check,msg);
            }
        }
        
        // Find all info_e ails and confirm all emails are present in scene.
        [UnityTest]
        public IEnumerator EmailsSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitUntil(() => sceneLoaded);

            yield return new WaitForSeconds(2f);
            
            string msg;
            bool check = true;
            int i = 0;
            int iter = 0;
            
            List<GameObject> emGOs = new List<GameObject>();
			List<GameObject> allParents =
			  SceneManager.GetActiveScene().GetRootGameObjects().ToList();
			
			// Find all HealthManager components.
            bool includeInactive = true;
            iter = 0;
			for (i=0;i<allParents.Count;i++) {
				Component[] compArray = allParents[i].GetComponentsInChildren(
				    typeof(Email),includeInactive
				);

				for (int k=0;k<compArray.Length;k++) {
				    // Add the gameObject associated with all Email
				    // components in the scene.
					emGOs.Add(compArray[k].gameObject);
				}
                iter++;
                if (iter > 10000) break;
			}
			
            iter = 0;
			for (i=0;i<emGOs.Count;i++) {
				if (emGOs[i] == null) continue;

				Email em = emGOs[i].GetComponent<Email>();
				if (em == null) continue;


                check = em.emailIndex >= 0;
                msg = "Bad Email.emailIndex " + em.emailIndex.ToString();
                Assert.That(check,msg);
                iter++;
                if (iter > 10000) break;
			}
            
            iter = 0;
            for (i=0; i< Const.a.audioLogs.Length;i++) {
                if (Const.a.audioLogType[i] != AudioLogType.Email
                    && Const.a.audioLogType[i] != AudioLogType.Vmail) continue;

                check = false;
                for (int j=0;j<emGOs.Count;j++) {
			    	if (emGOs[j] == null) continue;
			    	
			    	Email em = emGOs[j].GetComponent<Email>();
    				if (em == null) continue;

                    if (em.emailIndex == i) {
                        check = true;
                    }
                }
                
                msg = "Could not find info_email in scene for " + i.ToString();
                Assert.That(check,msg);
                iter++;
                if (iter > 10000) break;
            }
        }
    }
}
