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
            
            msg = "Const.a prefabs has unassigned slots!";
            check = true;
            for (int i=0;i<Const.a.prefabs.Length;i++) {
                if (Const.a.prefabs[i] == null) check = false;
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
        
        [UnityTest]
        public IEnumerator TargetnamesTargetted() {
            RunBeforeAnyTests();
            yield return new WaitUntil(() => sceneLoaded);
            yield return new WaitForSeconds(2f);
            
            string msg;
            int i=0;
            int k=0;
            Scene citmain = SceneManager.GetSceneByName("CitadelScene");
            List<GameObject> allGOs = new List<GameObject>();
            List<GameObject> allParents = citmain.GetRootGameObjects().ToList();
            msg = "Failed to populate allParents: ";
            Assert.That(allParents.Count > 1,msg + allParents.Count.ToString());

            for (i=0;i<allParents.Count;i++) {
                Component[] compArray =
                  allParents[i].GetComponentsInChildren(typeof(Transform),true);

                for (k=0;k<compArray.Length;k++) {
                    // Add to full list, separate so we don't infinite loop
                    allGOs.Add(compArray[k].gameObject);
                }
            }
            
            msg = "TargetnamesTargetted failed to populate allGOs: ";
            if (allGOs == null) { Assert.That(false,"allGOs was null"); yield return null; }
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
            
            ButtonSwitch bsTemp = null;
            ChargeStation csTemp = null;
            CyberAccess caTemp = null;
            CyberSwitch cswTemp = null;
            Door drTemp = null;
            HealthManager hmTemp = null;
            InteractablePanel ipTemp = null;
            KeypadElevator keTemp = null;
            KeypadKeycode kkTemp = null;
            LogicBranch lbTemp = null;
            LogicRelay lrTemp = null;
            LogicTimer ltTemp = null;
            PuzzleGridPuzzle pgpTemp = null;
            PuzzleWirePuzzle pwpTemp = null;
            QuestBitRelay qbrTemp = null;
            TargetIO tio = null;
            Trigger trgTemp = null;
            TriggerCounter trgcTemp = null;
            HashSet<string> targetNames = new HashSet<string>();
            for (i=0;i<allGOs.Count;i++) {
                tio = allGOs[i].GetComponent<TargetIO>();
                if (tio == null) continue;

                if (!string.IsNullOrWhiteSpace(tio.targetname)) {
                    targetNames.Add(tio.targetname);
                }
            }

            HashSet<string> targets = new HashSet<string>();
            for (int m=0;m<allGOs.Count;m++) {
                bsTemp = allGOs[m].GetComponent<ButtonSwitch>();
                if (bsTemp != null) {
                    if (!string.IsNullOrWhiteSpace(bsTemp.target)) {
                        targets.Add(bsTemp.target);
                    }
                }

                csTemp = allGOs[m].GetComponent<ChargeStation>();
                if (csTemp != null) {
                    if (!string.IsNullOrWhiteSpace(csTemp.target)) {
                        targets.Add(csTemp.target);	
                    }
                }

                caTemp = allGOs[m].GetComponent<CyberAccess>();
                if (caTemp != null) {
                    if (!string.IsNullOrWhiteSpace(caTemp.target)) {
                        targets.Add(caTemp.target);	
                    }
                }

                cswTemp = allGOs[m].GetComponent<CyberSwitch>();
                if (cswTemp != null) {
                    if (!string.IsNullOrWhiteSpace(cswTemp.target)) {
                        targets.Add(cswTemp.target);	
                    }
                }

                drTemp = allGOs[m].GetComponent<Door>();
                if (drTemp != null) {
                    if (!string.IsNullOrWhiteSpace(drTemp.target)) {
                        targets.Add(drTemp.target);	
                    }
                }

                hmTemp = allGOs[m].GetComponent<HealthManager>();
                if (hmTemp != null) {
                    if (!string.IsNullOrWhiteSpace(hmTemp.targetOnDeath)) {
                        targets.Add(hmTemp.targetOnDeath);	
                    }
                }
                ipTemp = allGOs[m].GetComponent<InteractablePanel>();
                if (ipTemp != null) {
                    if (!string.IsNullOrWhiteSpace(ipTemp.target)) {
                        targets.Add(ipTemp.target);	
                    }
                }

                keTemp = allGOs[m].GetComponent<KeypadElevator>();
                if (keTemp != null) {
                    if (!string.IsNullOrWhiteSpace(keTemp.lockedTarget)) {
                        targets.Add(keTemp.lockedTarget);	
                    }
                }

                kkTemp = allGOs[m].GetComponent<KeypadKeycode>();
                if (kkTemp != null) {
                    if (!string.IsNullOrWhiteSpace(kkTemp.target)) {
                        targets.Add(kkTemp.target);	
                    }

                    if (!string.IsNullOrWhiteSpace(kkTemp.lockedTarget)) {
                        targets.Add(kkTemp.lockedTarget);	
                    }
                }

                lbTemp = allGOs[m].GetComponent<LogicBranch>();
                if (lbTemp != null) {
                    if (!string.IsNullOrWhiteSpace(lbTemp.target)) {
                        targets.Add(lbTemp.target);	
                    }

                    if (!string.IsNullOrWhiteSpace(lbTemp.target2)) {
                        targets.Add(lbTemp.target2);	
                    }
                }

                lrTemp = allGOs[m].GetComponent<LogicRelay>();
                if (lrTemp != null) {
                    if (!string.IsNullOrWhiteSpace(lrTemp.target)) {
                        targets.Add(lrTemp.target);	
                    }
                }

                ltTemp = allGOs[m].GetComponent<LogicTimer>();
                if (ltTemp != null) {
                    if (!string.IsNullOrWhiteSpace(ltTemp.target)) {
                        targets.Add(ltTemp.target);	
                    }
                }

                pgpTemp = allGOs[m].GetComponent<PuzzleGridPuzzle>();
                if (pgpTemp != null) {
                    if (!string.IsNullOrWhiteSpace(pgpTemp.target)) {
                        targets.Add(pgpTemp.target);	
                    }
                }

                pwpTemp = allGOs[m].GetComponent<PuzzleWirePuzzle>();
                if (pwpTemp != null) {
                    if (!string.IsNullOrWhiteSpace(pwpTemp.target)) {
                        targets.Add(pwpTemp.target);	
                    }
                }

                qbrTemp = allGOs[m].GetComponent<QuestBitRelay>();
                if (qbrTemp != null) {
                    if (!string.IsNullOrWhiteSpace(qbrTemp.target)) {
                        targets.Add(qbrTemp.target);	
                    }

                    if (!string.IsNullOrWhiteSpace(qbrTemp.targetIfFalse)) {
                        targets.Add(qbrTemp.targetIfFalse);	
                    }
                }

                trgTemp = allGOs[m].GetComponent<Trigger>();
                if (trgTemp != null) {
                    if (!string.IsNullOrWhiteSpace(trgTemp.target)) {
                        targets.Add(trgTemp.target);	
                    }
                }

                trgcTemp = allGOs[m].GetComponent<TriggerCounter>();
                if (trgcTemp != null) {
                    if (!string.IsNullOrWhiteSpace(trgcTemp.target)) {
                        targets.Add(trgcTemp.target);	
                    }
                }
            }

            HashSet<string> targetsNotInTargetNames =
                new HashSet<string>(targets);

            targetsNotInTargetNames.ExceptWith(targetNames);
            int missingTargetsCount = targetsNotInTargetNames.Count;


            HashSet<string> targetNamesNotInTargets =
                new HashSet<string>(targetNames);

            targetNamesNotInTargets.ExceptWith(targets);
            int missingTargetNamesCount = targetNamesNotInTargets.Count;


            IEnumerator<string> notar = targetsNotInTargetNames.GetEnumerator();
            msg = "No matching targetName found for target: ";
            while (notar.MoveNext()) {
                Assert.That(false,msg + notar.Current.ToString());
            }

            IEnumerator<string> notrg = targetNamesNotInTargets.GetEnumerator();
            msg = "No matching target found for targetName: ";
            while (notrg.MoveNext()) {
                Assert.That(false,msg + notrg.Current.ToString());
            }
        }
        
        [UnityTest]
        public IEnumerator CheckAllLevelsLoadFine() {
            RunBeforeAnyTests();
            yield return new WaitUntil(() => sceneLoaded);
            
            yield return new WaitForSeconds(2f);
            
            bool check = LevelManager.a != null;
            string msg = "LevelManager.a was null";
            Assert.That(check,msg);
            
            MainMenuHandler.a.StartGame(true);
            yield return new WaitForSeconds(5f);
            
            PlayerMovement.a.hm.god = true;
            yield return new WaitForSeconds(0.1f);
            
            ConsoleEmulator.CheatLoadLevel(0);
            yield return new WaitForSeconds(3f);
            
            ConsoleEmulator.CheatLoadLevel(1);
            yield return new WaitForSeconds(3f);
            
            ConsoleEmulator.CheatLoadLevel(2);
            yield return new WaitForSeconds(3f);
            
            ConsoleEmulator.CheatLoadLevel(3);
            yield return new WaitForSeconds(3f);
            
            ConsoleEmulator.CheatLoadLevel(4);
            yield return new WaitForSeconds(3f);
            
            ConsoleEmulator.CheatLoadLevel(5);
            yield return new WaitForSeconds(3f);
            
            ConsoleEmulator.CheatLoadLevel(6);
            yield return new WaitForSeconds(3f);
            
            ConsoleEmulator.CheatLoadLevel(7);
            yield return new WaitForSeconds(3f);
            
            ConsoleEmulator.CheatLoadLevel(8);
            yield return new WaitForSeconds(3f);
            
            ConsoleEmulator.CheatLoadLevel(9);
            yield return new WaitForSeconds(3f);
            
            ConsoleEmulator.CheatLoadLevel(10);
            yield return new WaitForSeconds(3f);
            
            ConsoleEmulator.CheatLoadLevel(11);
            yield return new WaitForSeconds(3f);
            
            ConsoleEmulator.CheatLoadLevel(12);
            yield return new WaitForSeconds(3f);
            
            ConsoleEmulator.CheatLoadLevel(1);
            yield return new WaitForSeconds(3f);
            
            // Test cyberspace loads
            MouseLookScript.a.EnterCyberspace(LevelManager.a.ressurectionLocation[1].position);
            
            yield return new WaitForSeconds(3f);
            
            MouseLookScript.a.ExitCyberspace();
            yield return new WaitForSeconds(1f);
            
        }
    }
}
