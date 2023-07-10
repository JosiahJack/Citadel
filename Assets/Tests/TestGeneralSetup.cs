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

        public void RunBeforeAnyTests() {
            if (acquiredGOs && allGOs != null && tester != null) return;

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            GameObject go = GameObject.Find("GlobalConsts");
            if (go != null) tester = go.GetComponent<CitadelTests>();
            Assert.That(tester != null,"Unable to find CitadelTests attached to a GameObject GlobalConsts in scene");
            int i=0;
            int k=0;
            allGOs = new List<GameObject>();
            //Scene citmain = SceneManager.GetActiveScene();
            Scene citmain = SceneManager.GetSceneByName("CitadelScene");
            Assert.That(citmain.name == "CitadelScene","Not testing main Citadel scene, instead testing: " + citmain.name);
            allParents = citmain.GetRootGameObjects().ToList();
            Assert.That(allParents.Count > 1,"Failed to populate allParents:" + allParents.Count.ToString());
            for (i=0;i<allParents.Count;i++) {
                Component[] compArray = allParents[i].GetComponentsInChildren(typeof(Transform),true);
                for (k=0;k<compArray.Length;k++) {
                    allGOs.Add(compArray[k].gameObject); // Add to full list, separate so we don't infinite loop
                }
            }
            acquiredGOs = true;
        }

        [Test]
        public void TargetnamesTargetted() {
            RunBeforeAnyTests();
            int i;
            Assert.That(allGOs.Count > 1,"TestSetup.RunBeforeAnyTests failed to populate allGOs:" + allGOs.Count.ToString());
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


            IEnumerator<string> notargs = targetsNotInTargetNames.GetEnumerator();
            while (notargs.MoveNext()) {
                Assert.That(false,"No matching targetName found for target: "
                                    + notargs.Current.ToString());
            }

            IEnumerator<string> notrigs = targetNamesNotInTargets.GetEnumerator();
            while (notrigs.MoveNext()) {
                Assert.That(false,"No matching target found for targetName: "
                                    + notrigs.Current.ToString());
            }
        }

        [Test]
        public void AIControllersSetupProperly() {
            RunBeforeAnyTests();
            Assert.That(allGOs.Count > 1,"TestSetup.RunBeforeAnyTests failed to populate allGOs:" + allGOs.Count.ToString());
		    string script = "AIController";

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
                AIController aic = allGOs[i].GetComponent<AIController>();
                if (aic != null) {
                    BoundsError(script,allGOs[i],0,28,aic.index,"index");
                    MissingComponent(script,allGOs[i],typeof(Rigidbody));
                    MissingComponent(script,allGOs[i],typeof(HealthManager));
                    if (aic.walkPathOnStart) {
                        Assert.That(!(aic.walkWaypoints.Length < 1),script + " missing any walkWaypoints but walkPathOnStart is true.");
                        if (aic.walkWaypoints.Length > 0) {
                            for (int k=0;k<aic.walkWaypoints.Length;k++) {
                                Assert.That(aic.walkWaypoints[k] != null,script + " missing walkWaypoints["+k.ToString()+"].");
                            }
                        }
                    }
                }
            }
        }

        [Test]
        public void AIAnimationControllersSetupProperly() {
            RunBeforeAnyTests();
            Assert.That(allGOs.Count > 1,"TestSetup.RunBeforeAnyTests failed to populate allGOs:" + allGOs.Count.ToString());
		    string script = "AIAnimationController";

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
                AIAnimationController aiac =
                    allGOs[i].GetComponent<AIAnimationController>();

                if (aiac != null) {
                    BoundsError(script,allGOs[i],0,10,
                                aiac.minWalkSpeedToAnimate,
                                "minWalkSpeedToAnimate");

                    MissingComponent(script,allGOs[i],typeof(Animator));
                    MissingReference(script,allGOs[i],aiac.aic,"aic");
                }
            }
        }

        [Test]
        public void HealthManagersSetupProperly() {
            RunBeforeAnyTests();
            Assert.That(allGOs.Count > 1,"TestSetup.RunBeforeAnyTests failed to populate allGOs:" + allGOs.Count.ToString());
		    string script = "HealthManager";

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
                HealthManager hm = allGOs[i].GetComponent<HealthManager>();
                if (hm == null) continue;

                if (hm.isNPC) {
                    MissingComponent(script,allGOs[i],typeof(AIController));
                    Assert.That(!(hm.isSecCamera || hm.isPlayer || hm.isObject
                                  || hm.isIce || hm.isScreen || hm.isGrenade),
                                FailMessage(script,allGOs[i], "not marked only"
                                                             + " as an NPC"));
                    Assert.That(!(hm.teleportOnDeath
                                  && hm.teleportEffect == null),
                                FailMessage(script,allGOs[i], "teleportOnDeath"
                                + " set but is missing teleportEffect"));
                } else {
                    Assert.That(!hm.teleportOnDeath,
                                FailMessage(script,allGOs[i],"teleportOnDeath"
                                                             + " set but not "
                                                             + "marked NPC"));
                }

                if (hm.isSecCamera) {
                    BoundsError(script,allGOs[i],0,13,hm.levelIndex,
                                "levelIndex");

                    Assert.That(hm.isObject,FailMessage(script,allGOs[i],
                                            "marked as security camera but"
                                            + " isn't also isObject"));

                    Assert.That(!(hm.isNPC || hm.isPlayer || hm.isIce
                                  || hm.isScreen || hm.isGrenade),
                                FailMessage(script,allGOs[i],"marked as "
                                                             + "security "
                                                             + "camera and "
                                                             + "another object"
                                                             + " as well."));

                    Assert.That(!(hm.securityAffected == SecurityType.None),
                                script + " is marked as an security camera and"
                                + " securityAffected is marked as None for "
                                + "gameObject " + allGOs[i].name
                                + " with parent of "
                                + allGOs[i].transform.parent.name);
                }

                if (hm.isPlayer) {
                    Assert.That(!(hm.isNPC || hm.isSecCamera || hm.isObject
                                  || hm.isIce || hm.isScreen || hm.isGrenade),
                                FailMessage(script,allGOs[i],"marked as player"
                                                             + " and another "
                                                             + "object as "
                                                             + "well."));
                }

                if (hm.isGrenade) {
                    MissingComponent(script,allGOs[i],typeof(GrenadeActivate));
                    Assert.That(!(hm.isNPC || hm.isSecCamera || hm.isObject || hm.isIce || hm.isScreen || hm.isPlayer),script + " is marked as a grenade and another object as well.");
                }

                if (hm.isObject) {
                    Assert.That(!(hm.isNPC || hm.isGrenade || hm.isIce || hm.isScreen || hm.isPlayer),script + " is marked as an object and another object as well."); // Can also be security camera (isSecCamera)
                }

                if (hm.isScreen) {
                    Assert.That(!(hm.backupDeathSound == null),script + " missing backupDeathSound when isScreen on gameObject " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
                    MissingComponent(script,allGOs[i],typeof(ImageSequenceTextureArray));
                    Assert.That(!(hm.isNPC || hm.isSecCamera || hm.isGrenade || hm.isIce || hm.isObject || hm.isPlayer),script + " is marked as a screen and another object as well.");
                }
                if (hm.gibOnDeath) {
                    Assert.That(!(hm.gibObjects.Length < 1 && !hm.dropItemsOnGib),script + " no gibObjects when gibOnDeath is true on gameObject " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
                    for (int k=0;k<hm.gibObjects.Length;k++) {
                        Assert.That(!(hm.gibObjects[k] == null && !hm.dropItemsOnGib),script + "missing gibObjects["+k.ToString()+"].");
                    } // Exception for dropItemsOnGib since some objects need to drop their searchables.
                    Assert.That(!(hm.gibsGetVelocity && hm.gibVelocityBoost.magnitude < Mathf.Epsilon),script + " has gibsGetVelocity set to true but it's gibVelocityBoost is zero.");
                } else {
                    Assert.That(!(hm.gibsGetVelocity),script + " has gibsGetVelocity set to true but isn't also set to gibOnDeath.");
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
                    Assert.That(numtargetsfound > 0,script + " has no matching targets for " + hm.targetOnDeath + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
                }
            }
        }

        [Test]
        public void ElevatorButtonsSetupProperly() {
            RunBeforeAnyTests();
            Assert.That(allGOs.Count > 1,"TestSetup.RunBeforeAnyTests failed to populate allGOs:" + allGOs.Count.ToString());
		    string script = "ElevatorButton";

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
                ElevatorButton evb = allGOs[i].GetComponent<ElevatorButton>();
                if (evb == null) continue;

                Assert.That(!(evb.GetComponentInChildren<Text>(true) == null),
                            FailMessage(script,allGOs[i],"missing childText "
                                                         + "Text component"));
            }
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you
        // can use `yield return null;` to skip a frame.
        //[UnityTest]
        //public IEnumerator TestGeneralSetupWithEnumeratorPasses() {
            //// Use the Assert class to test conditions.
            //// Use yield to skip a frame.
        //    yield return null;
        //}

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
