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
    [SetUpFixture]
    public class TestSetup {
        public List<GameObject> allGOs;
        private List<GameObject> allParents;
        private bool acquiredGOs;

        [OneTimeSetup]
        public void RunBeforeAnyTests() {
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
        }
    }

    public class TestGeneralSetup {
        [Test]
        public void ConfirmTargetnamesTargetted() {
            int i;
            Assert.That(TestSetup.allGOs.Count > 1,"TestSetup.RunBeforeAnyTests failed to populate allGOs:" + TestSetup.allGOs.Count.ToString());
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
            for (i=0;i<TestSetup.allGOs.Count;i++) {
                tio = TestSetup.allGOs[i].GetComponent<TargetIO>();
                if (tio == null) continue;

                if (!string.IsNullOrWhiteSpace(tio.targetname)) {
                    targetNames.Add(tio.targetname);
                }
            }

            HashSet<string> targets = new HashSet<string>();
            for (int m=0;m<TestSetup.allGOs.Count;m++) {
                bsTemp = TestSetup.allGOs[m].GetComponent<ButtonSwitch>();
                if (bsTemp != null) {
                    if (!string.IsNullOrWhiteSpace(bsTemp.target)) {
                        targets.Add(bsTemp.target);
                    }
                }

                csTemp = TestSetup.allGOs[m].GetComponent<ChargeStation>();
                if (csTemp != null) {
                    if (!string.IsNullOrWhiteSpace(csTemp.target)) {
                        targets.Add(csTemp.target);	
                    }
                }

                caTemp = TestSetup.allGOs[m].GetComponent<CyberAccess>();
                if (caTemp != null) {
                    if (!string.IsNullOrWhiteSpace(caTemp.target)) {
                        targets.Add(caTemp.target);	
                    }
                }

                cswTemp = TestSetup.allGOs[m].GetComponent<CyberSwitch>();
                if (cswTemp != null) {
                    if (!string.IsNullOrWhiteSpace(cswTemp.target)) {
                        targets.Add(cswTemp.target);	
                    }
                }

                drTemp = TestSetup.allGOs[m].GetComponent<Door>();
                if (drTemp != null) {
                    if (!string.IsNullOrWhiteSpace(drTemp.target)) {
                        targets.Add(drTemp.target);	
                    }
                }

                hmTemp = TestSetup.allGOs[m].GetComponent<HealthManager>();
                if (hmTemp != null) {
                    if (!string.IsNullOrWhiteSpace(hmTemp.targetOnDeath)) {
                        targets.Add(hmTemp.targetOnDeath);	
                    }
                }
                ipTemp = TestSetup.allGOs[m].GetComponent<InteractablePanel>();
                if (ipTemp != null) {
                    if (!string.IsNullOrWhiteSpace(ipTemp.target)) {
                        targets.Add(ipTemp.target);	
                    }
                }

                keTemp = TestSetup.allGOs[m].GetComponent<KeypadElevator>();
                if (keTemp != null) {
                    if (!string.IsNullOrWhiteSpace(keTemp.lockedTarget)) {
                        targets.Add(keTemp.lockedTarget);	
                    }
                }

                kkTemp = TestSetup.allGOs[m].GetComponent<KeypadKeycode>();
                if (kkTemp != null) {
                    if (!string.IsNullOrWhiteSpace(kkTemp.target)) {
                        targets.Add(kkTemp.target);	
                    }

                    if (!string.IsNullOrWhiteSpace(kkTemp.lockedTarget)) {
                        targets.Add(kkTemp.lockedTarget);	
                    }
                }

                lbTemp = TestSetup.allGOs[m].GetComponent<LogicBranch>();
                if (lbTemp != null) {
                    if (!string.IsNullOrWhiteSpace(lbTemp.target)) {
                        targets.Add(lbTemp.target);	
                    }

                    if (!string.IsNullOrWhiteSpace(lbTemp.target2)) {
                        targets.Add(lbTemp.target2);	
                    }
                }

                lrTemp = TestSetup.allGOs[m].GetComponent<LogicRelay>();
                if (lrTemp != null) {
                    if (!string.IsNullOrWhiteSpace(lrTemp.target)) {
                        targets.Add(lrTemp.target);	
                    }
                }

                ltTemp = TestSetup.allGOs[m].GetComponent<LogicTimer>();
                if (ltTemp != null) {
                    if (!string.IsNullOrWhiteSpace(ltTemp.target)) {
                        targets.Add(ltTemp.target);	
                    }
                }

                pgpTemp = TestSetup.allGOs[m].GetComponent<PuzzleGridPuzzle>();
                if (pgpTemp != null) {
                    if (!string.IsNullOrWhiteSpace(pgpTemp.target)) {
                        targets.Add(pgpTemp.target);	
                    }
                }

                pwpTemp = TestSetup.allGOs[m].GetComponent<PuzzleWirePuzzle>();
                if (pwpTemp != null) {
                    if (!string.IsNullOrWhiteSpace(pwpTemp.target)) {
                        targets.Add(pwpTemp.target);	
                    }
                }

                qbrTemp = TestSetup.allGOs[m].GetComponent<QuestBitRelay>();
                if (qbrTemp != null) {
                    if (!string.IsNullOrWhiteSpace(qbrTemp.target)) {
                        targets.Add(qbrTemp.target);	
                    }

                    if (!string.IsNullOrWhiteSpace(qbrTemp.targetIfFalse)) {
                        targets.Add(qbrTemp.targetIfFalse);	
                    }
                }

                trgTemp = TestSetup.allGOs[m].GetComponent<Trigger>();
                if (trgTemp != null) {
                    if (!string.IsNullOrWhiteSpace(trgTemp.target)) {
                        targets.Add(trgTemp.target);	
                    }
                }

                trgcTemp = TestSetup.allGOs[m].GetComponent<TriggerCounter>();
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
        public void ConfirmAIControllersSetupProperly() {
            Assert.That(TestSetup.allGOs.Count > 1,"TestSetup.RunBeforeAnyTests failed to populate TestSetup.allGOs:" + TestSetup.allGOs.Count.ToString());
            int i=0;
		    string script = "AIController";

            // Run through all GameObjects and perform all tests
            for (i=0;i<TestSetup.allGOs.Count;i++) {
                AIController aic = TestSetup.allGOs[i].GetComponent<AIController>();
                if (aic != null) {
                    Assert.That(!BoundsError(script,TestSetup.allGOs[i],0,28,aic.index,"index"),aic.gameObject.name + " has issue with AIController.index out of bounds [0,28]");
                     Assert.That(!MissingComponent(script,TestSetup.allGOs[i],typeof(Rigidbody)));
                     Assert.That(!MissingComponent(script,TestSetup.allGOs[i],typeof(HealthManager)));
                    if (aic.walkPathOnStart) {
                        Assert.That(!(aic.walkWaypoints.Length < 1),script + " missing any walkWaypoints but walkPathOnStart is true.");
                        if (aic.walkWaypoints.Length > 0) {
                            for (k=0;k<aic.walkWaypoints.Length;k++) {
                                Assert.That(aic.walkWaypoints[k] != null,script + " missing walkWaypoints["+k.ToString()+"].");
                            }
                        }
                    }
                }
            }
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        //[UnityTest]
        //public IEnumerator TestGeneralSetupWithEnumeratorPasses() {
            //// Use the Assert class to test conditions.
            //// Use yield to skip a frame.
        //    yield return null;
        //}

        private bool MissingReference(string className, GameObject go, Component comp, string variableName) {
            string self = (go != null) ? go.name : "errname";
            string parent = (go.transform.parent != null) ? go.transform.parent.name : "none";
            if (comp == null) {
                Assert.That(false,"BUG: " + className + " is missing reference " + comp.ToString() + "(" + variableName + ") on GameObject " + self + " with parent of " + parent);
                return true;
            } else return false;
        }

        private bool MissingComponent(string className, GameObject go, System.Type type) {
            string self = (go != null) ? go.name : "errname";
            string parent = (go.transform.parent != null) ? go.transform.parent.name : "none";
            if (go.GetComponent(type) == null) {
                Assert.That(false,"BUG: " + className + " is missing component " + type.ToString() + " on GameObject " + self + " with parent of " + parent);
                return true;
            } else return false;
        }

        private bool BoundsError(string className, GameObject go, int expectedMin, int expectedMax, int foundVal, string valueName) {
            string self = (go != null) ? go.name : "errname";
            string parent = (go.transform.parent != null) ? go.transform.parent.name : "none";
            if (foundVal > expectedMax || foundVal < expectedMin) {
                Assert.That(false,"BUG: " + className + " has invalid value for "+ valueName +" on GameObject " + self + " with parent of " + parent + ". Expected value in range: "
                + expectedMin.ToString() + " to " + expectedMax.ToString() + "; Found value: " + foundVal.ToString());
                return true;
            } else return false;
        }

        private bool BoundsError(string className, GameObject go, float expectedMin, float expectedMax, float foundVal, string valueName) {
            string self = (go != null) ? go.name : "errname";
            string parent = (go.transform.parent != null) ? go.transform.parent.name : "none";
            if (foundVal > expectedMax || foundVal < expectedMin) {
                Assert.That(false,"BUG: " + className + " has invalid value for " + valueName + " on GameObject " + self + " with parent of " + parent + ". Expected value in range: "
                + expectedMin.ToString() + " to " + expectedMax.ToString() + "; Found value: " + foundVal.ToString());
                return true;
            } else return false;
        }
    }
}
