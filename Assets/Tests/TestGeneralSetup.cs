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
        public List<GameObject> allGOs;
        private List<GameObject> allParents;
        private bool acquiredGOs;
        private bool sceneLoaded;

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you
        // can use `yield return null;` to skip a frame.
        //[UnityTest]
        //public IEnumerator TestGeneralSetupWithEnumeratorPasses() {
            //// Use the Assert class to test conditions.
            //// Use yield to skip a frame.
        //    yield return null;
        //}

        public void RunBeforeAnyTests() {
            if (sceneLoaded) return;

            Scene citmain = SceneManager.GetSceneByName("CitadelScene");
            if (citmain.name != "CitadelScene") {
                EditorSceneManager.OpenScene("Assets/Scenes/CitadelScene.unity",
                                             OpenSceneMode.Single);
            }
        }

        public void SetupTests() {
            if (acquiredGOs && allGOs != null && tester != null) return;

            Scene citmain = SceneManager.GetSceneByName("CitadelScene");
            string msg = "Not testing main Citadel scene, instead testing: ";
            Assert.That(citmain.name == "CitadelScene",msg + citmain.name);

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            GameObject go = GameObject.Find("GlobalConsts");
            if (go != null) tester = go.GetComponent<CitadelTests>();
            msg = "Unable to find CitadelTests attached to a GameObject with "
                  + "name GlobalConsts in scene";

            Assert.That(tester != null,msg);

            int i=0;
            int k=0;
            allGOs = new List<GameObject>();
            allParents = citmain.GetRootGameObjects().ToList();
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
            acquiredGOs = true;
        }

        private bool SceneLoaded() {
            Scene citmain = SceneManager.GetSceneByName("CitadelScene");
            if (citmain.name != "CitadelScene") return false;
            return true;
        }

        [UnityTest]
        public IEnumerator TargetnamesTargetted() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());

            int i;
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
        public IEnumerator AIControllersSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());

		    string script = "AIController";
            bool check = true;

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
                AIController aic = allGOs[i].GetComponent<AIController>();
                if (aic == null) continue;

                BoundsError(script,allGOs[i],0,28,aic.index,"index");
                MissingComponent(script,allGOs[i],typeof(Rigidbody));
                MissingComponent(script,allGOs[i],typeof(HealthManager));
                if (!aic.walkPathOnStart) continue; // Only path checks next...

                msg = "missing walkWaypoints but walkPathOnStart is true";
                check = !(aic.walkWaypoints.Length < 1);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                if (aic.walkWaypoints.Length > 0) {
                    for (int k=0;k<aic.walkWaypoints.Length;k++) {
                        msg = "missing walkWaypoints[" + k.ToString() + "].";
                        check = aic.walkWaypoints[k] != null;
                        Assert.That(check,FailMessage(script,allGOs[i],msg));
                    }
                }
            }
        }

        [UnityTest]
        public IEnumerator AIAnimationControllersSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "AIAnimationController";

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
                AIAnimationController aiac =
                    allGOs[i].GetComponent<AIAnimationController>();

                if (aiac == null) continue;

                BoundsError(script,allGOs[i],0,10,aiac.minWalkSpeedToAnimate,
                            "minWalkSpeedToAnimate");

                MissingComponent(script,allGOs[i],typeof(Animator));
                MissingReference(script,allGOs[i],aiac.aic,"aic");
            }
        }

        [UnityTest]
        public IEnumerator HealthManagersSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "HealthManager";
            bool check = true;

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
                HealthManager hm = allGOs[i].GetComponent<HealthManager>();
                if (hm == null) continue;

                if (hm.isNPC) {
                    MissingComponent(script,allGOs[i],typeof(AIController));
                    msg = "not marked only as an NPC";
                    check = !(hm.isSecCamera || hm.isPlayer || hm.isObject
                              || hm.isIce || hm.isScreen || hm.isGrenade);

                    Assert.That(check,FailMessage(script,allGOs[i],msg));

                    msg = "teleportOnDeath set but is missing teleportEffect";
                    check = !(hm.teleportOnDeath && hm.teleportEffect == null);
                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                } else {
                    msg = "teleportOnDeath set but not marked isNPC";
                    check = !hm.teleportOnDeath;
                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                }

                if (hm.isSecCamera) {
                    BoundsError(script,allGOs[i],0,13,hm.levelIndex,
                                "levelIndex");

                    msg = "marked security camera but isn't also isObject";
                    Assert.That(hm.isObject,FailMessage(script,allGOs[i],msg));

                    msg = "marked security camera and another object as well";
                    check = !(hm.isNPC || hm.isPlayer || hm.isIce
                              || hm.isScreen || hm.isGrenade);

                    Assert.That(check,FailMessage(script,allGOs[i],msg));

                    msg = "marked security and securityAffected is None";
                    check = !(hm.securityAffected == SecurityType.None);
                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                }

                if (hm.isPlayer) {
                    msg = "marked as isPlayer and another object as well";
                    check = !(hm.isNPC || hm.isSecCamera || hm.isObject
                              || hm.isIce || hm.isScreen || hm.isGrenade);

                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                }

                if (hm.isGrenade) {
                    MissingComponent(script,allGOs[i],typeof(GrenadeActivate));
                    msg = "marked as grenade and another object as well";
                    check = !(hm.isNPC || hm.isSecCamera || hm.isObject
                              || hm.isIce || hm.isScreen || hm.isPlayer);

                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                }

                if (hm.isObject) {
                    msg = "marked as isObject and another object as well";

                    // Exception: security camera (isSecCamera) + isObject
                    check = !(hm.isNPC || hm.isGrenade || hm.isIce
                              || hm.isScreen || hm.isPlayer);

                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                }

                if (hm.isScreen) {
                    msg = "missing backupDeathSound when isScreen";
                    check = !(hm.backupDeathSound == null);
                    Assert.That(check,FailMessage(script,allGOs[i],msg));

                    MissingComponent(script,allGOs[i],
                                     typeof(ImageSequenceTextureArray));

                    msg = "marked as a screen and another object as well";
                    check = !(hm.isNPC || hm.isSecCamera || hm.isGrenade
                              || hm.isIce || hm.isObject || hm.isPlayer);

                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                }
                if (hm.gibOnDeath) {
                    msg = "no gibObjects when gibOnDeath is true";
                    check = !(hm.gibObjects.Length < 1 && !hm.dropItemsOnGib);
                    Assert.That(check,FailMessage(script,allGOs[i],msg));

                    // Exception for dropItemsOnGib since some objects need to
                    // drop their searchables.
                    for (int k=0;k<hm.gibObjects.Length;k++) {
                        msg = "missing gibObjects["+k.ToString()+"].";
                        check = !(hm.gibObjects[k] == null
                                  && !hm.dropItemsOnGib);

                        Assert.That(check,FailMessage(script,allGOs[i],msg));
                    }

                    msg = " has gibsGetVelocity set to true but it's "
                          + "gibVelocityBoost is zero";
                    check = !(hm.gibsGetVelocity
                              && hm.gibVelocityBoost.magnitude < Mathf.Epsilon);

                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                } else {
                    msg = " has gibsGetVelocity set to true but isn't also set"
                          + " to gibOnDeath.";

                    Assert.That(!(hm.gibsGetVelocity),
                                  FailMessage(script,allGOs[i],msg));
                }

                if (!string.IsNullOrWhiteSpace(hm.targetOnDeath)) { 
                    float numtargetsfound = 0;
                    for (int m=0;m<allGOs.Count;m++) {
                        TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
                        if (tioTemp == null) continue;

                        if (tioTemp.targetname == hm.targetOnDeath) {
                            numtargetsfound++;
                        }
                    }

                    msg = " has no matching targets for " + hm.targetOnDeath;
                    check = numtargetsfound > 0;
                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                }
            }
        }

        [UnityTest]
        public IEnumerator ElevatorButtonsSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "ElevatorButton";
            bool check = true;

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
                ElevatorButton eb = allGOs[i].GetComponent<ElevatorButton>();
                if (eb == null) continue;

                msg = "missing childText Text component";
                check = !(eb.GetComponentInChildren<Text>(true) == null);
                Assert.That(check,FailMessage(script,allGOs[i],msg));
            }
        }

        [UnityTest]
        public IEnumerator MouseLookScriptSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "MouseLookScript";
            bool check = true;
            int num_MouseLookScript = 0;

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
			    MouseLookScript mls = allGOs[i].GetComponent<MouseLookScript>();
			    if (mls == null) continue;

                num_MouseLookScript++;
                msg = "has more than one instance";
                check = !(num_MouseLookScript > 1);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "missing player";
                check = !(mls.player == null);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "missing canvasContainer";
                check = !(mls.canvasContainer == null);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "missing compassContainer";
                check = !(mls.compassContainer == null);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "missing automapContainerLH";
                check = !(mls.automapContainerLH == null);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "missing automapContainerRH";
                check = !(mls.automapContainerRH == null);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "missing compassMidpoints";
                check = !(mls.compassMidpoints == null);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "missing compassLargeTicks";
                check = !(mls.compassLargeTicks == null);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "missing compassSmallTicks";
                check = !(mls.compassSmallTicks == null);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "missing tabControl";
                check = !(mls.tabControl == null);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "missing dataTabNoItemsText";
                check = !(mls.dataTabNoItemsText == null);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "missing logContentsManager";
                check = !(mls.logContentsManager == null);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "missing SFXSource";
                check = !(mls.SFXSource == null);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "missing hardwareButtons";
                check = !(mls.hardwareButtons == null);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "missing puzzleWire";
                check = !(mls.puzzleWire == null);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "missing puzzleGrid";
                check = !(mls.puzzleGrid == null);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "missing shootModeButton";
                check = !(mls.shootModeButton == null);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "missing hm";
                check = !(mls.hm == null);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "missing playerRadiationTreatmentFlash";
                check = !(mls.playerRadiationTreatmentFlash == null);
                Assert.That(check,FailMessage(script,allGOs[i],msg));
            }
        }

        private string FailMessage(string className, GameObject go,
                                   string msg) {

            string self = (go != null) ? go.name : "errname";
            string parent;
            if (go.transform.parent != null) parent = go.transform.parent.name;
            else parent = "none";

            return ("BUG: " + className + " " + msg + " on GameObject " + self
                    + " with parent of " + parent);
        }

        private bool MissingReference(string className, GameObject go,
                                      Component comp, string variableName) {
            if (comp == null) {
                string msg = "missing reference " + comp.ToString() + "("
                             + variableName + ")";

                Assert.That(false,FailMessage(className,go,msg));
                return true;
            } else return false;
        }

        private bool MissingComponent(string className, GameObject go,
                                      System.Type type) {

            if (go.GetComponent(type) == null) {
                string msg = "missing component " + type.ToString();
                Assert.That(false,FailMessage(className,go,msg));
                return true;
            } else return false;
        }

        private bool BoundsError(string className, GameObject go,
                                 int expectedMin, int expectedMax,
                                 int foundVal, string valueName) {

            if (foundVal > expectedMax || foundVal < expectedMin) {
                string msg = "invalid value for "+ valueName;
                Assert.That(false,FailMessage(className,go,msg)
                                  + ". Expected value in range: "
                                  + expectedMin.ToString() + " to "
                                  + expectedMax.ToString() + "; Found value: "
                                  + foundVal.ToString());
                return true;
            } else return false;
        }

        private bool BoundsError(string className, GameObject go,
                                 float expectedMin, float expectedMax,
                                 float foundVal, string valueName) {

            if (foundVal > expectedMax || foundVal < expectedMin) {
                string msg = "invalid value for "+ valueName;
                Assert.That(false,FailMessage(className,go,msg)
                                  + ". Expected value in range: "
                                  + expectedMin.ToString() + " to "
                                  + expectedMax.ToString() + "; Found value: "
                                  + foundVal.ToString());
                return true;
            } else return false;
        }
    }
}
