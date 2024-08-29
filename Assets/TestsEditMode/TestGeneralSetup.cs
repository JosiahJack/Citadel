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
        private List<TargetIO> allTIOs;
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
                MissingComponent(script,allGOs[i],typeof(TargetIO));
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

                if (aic.searchColliderGO != null) {
                    msg = "search layer not Corpse";
                    check = aic.searchColliderGO.layer == 13; // Corpse
                    Assert.That(check,FailMessage(script,allGOs[i],msg));
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

                    if (hm.teleportOnDeath) {
                        msg = "teleportOnDeath set but is missing teleportEffect";
                        check = hm.teleportEffect != null;
                        Assert.That(check,FailMessage(script,allGOs[i],msg));
                    }
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
                    
                    ImageSequenceTextureArray ista =
                        allGOs[i].GetComponent<ImageSequenceTextureArray>();
                    
                    
                    msg = "isScreen and is marked as destroyed but has health";
                    check = ista.screenDestroyed ? hm.health <= 0f: true;
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

                    check = !hm.gibsGetVelocity;
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
                check = (num_MouseLookScript == 1);
                Assert.That(check,FailMessage(script,allGOs[i],msg));
                if (!check) continue;

                MissingReference(script,allGOs[i],mls.player,"player");
                MissingReference(script,allGOs[i],mls.canvasContainer,
                                 "canvasContainer");

                MissingReference(script,allGOs[i],mls.compassContainer,
                                 "compassContainer");

                MissingReference(script,allGOs[i],mls.automapContainerLH,
                                 "automapContainerLH");

                MissingReference(script,allGOs[i],mls.automapContainerRH,
                                 "automapContainerRH");

                MissingReference(script,allGOs[i],mls.compassMidpoints,
                                 "compassMidpoints");

                MissingReference(script,allGOs[i],mls.compassLargeTicks,
                                 "compassLargeTicks");

                MissingReference(script,allGOs[i],mls.compassSmallTicks,
                                 "compassSmallTicks");

                MissingReference(script,allGOs[i],mls.dataTabNoItemsText,
                                 "dataTabNoItemsText");

                MissingReference(script,allGOs[i],mls.logContentsManager,
                                 "logContentsManager");

                MissingReference(script,allGOs[i],mls.SFXSource,"SFXSource");
                MissingReference(script,allGOs[i],mls.hardwareButtons,
                                 "hardwareButtons");

                MissingReference(script,allGOs[i],mls.puzzleWire,"puzzleWire");
                MissingReference(script,allGOs[i],mls.puzzleGrid,"puzzleGrid");
                MissingReference(script,allGOs[i],mls.shootModeButton,
                                 "shootModeButton");

                MissingReference(script,allGOs[i],mls.hm,"hm");
                MissingReference(script,allGOs[i],
                                 mls.playerRadiationTreatmentFlash,
                                 "playerRadiationTreatmentFlash");
            }
        }

        [UnityTest]
        public IEnumerator SearchableItemsSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "SearchableItem";
            bool check = true;

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
                SearchableItem sitem = allGOs[i].GetComponent<SearchableItem>();
                if (sitem == null) continue;

                for (int k=0;k<4;k++) {
                    check = !(sitem.contents[k] < -1
                              || sitem.contents[k] > 110);
                    msg = "invalid contents index of "
                          + sitem.contents[k].ToString()
                          + " for content " + k.ToString();

                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                }

                for (int k=0;k<4;k++) {
                    check = !(sitem.customIndex[k] < -1
                              || sitem.customIndex[k] > 110);
                    msg = "invalid customIndex of "
                          + sitem.customIndex[k].ToString()
                          + " for content " + k.ToString();

                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                }

                msg = "has empty objectName";
                check = (!string.IsNullOrWhiteSpace(sitem.objectName));
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                if (!sitem.generateContents) continue;

                // All of the following only apply when randomly generating.
                msg = "set to generate contents but randomItem list empty";
                check = !(sitem.randomItem.Length <= 0);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "randomItem, randomItemCustomIndex list lengths differ";
                check = sitem.randomItem.Length ==
                            sitem.randomItemCustomIndex.Length;

                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "randomItem, randomItemDropChance list lengths differ";
                check = sitem.randomItem.Length ==
                            sitem.randomItemDropChance.Length;

                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "too many maxRandomItems of "
                      + sitem.maxRandomItems.ToString();

                check = !(sitem.maxRandomItems > 4);
                Assert.That(check,FailMessage(script,allGOs[i],msg));
            }
        }

        [UnityTest]
        public IEnumerator ButtonSwitchesSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "ButtonSwitch";
            bool check = true;

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
                ButtonSwitch bsw = allGOs[i].GetComponent<ButtonSwitch>();
                if (bsw == null) continue;

                MissingComponent(script,allGOs[i],typeof(TargetIO));
                if (string.IsNullOrWhiteSpace(bsw.target) && !bsw.locked) {
                    msg = "has no target";
                    check = false;
                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                }

                if (bsw.changeMatOnActive || bsw.blinkWhenActive) {
                    MissingComponent(script,allGOs[i],typeof(MeshRenderer));
                    MissingReference(script,allGOs[i],bsw.mainSwitchMaterial,
                                     "mainSwitchMaterial");

                    MissingReference(script,allGOs[i],
                                     bsw.alternateSwitchMaterial,
                                     "alternateSwitchMaterial");
                }
            }
        }

        [UnityTest]
        public IEnumerator ChargeStationsSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "ChargeStation";
            bool check = true;

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
                ChargeStation chst = allGOs[i].GetComponent<ChargeStation>();
                if (chst == null) continue;

                MissingComponent(script,allGOs[i],typeof(TargetIO));
                if (chst.requireReset) {
                    msg = "no resetTime set";
                    check = chst.resetTime > 0.01f;
                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                }

                msg = "unstranslated string rechargeMsg";
                check = string.IsNullOrWhiteSpace(chst.rechargeMsg);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "unstranslated string usedMsg";
                check = string.IsNullOrWhiteSpace(chst.usedMsg);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "charge station amount " + chst.amount.ToString()
                      + "too low to detect, less than 11.0";

                check = chst.amount >= 11f;
                Assert.That(check,FailMessage(script,allGOs[i],msg));
            }
        }

        [UnityTest]
        public IEnumerator CyberAccessesSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "CyberAccess";

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
                CyberAccess cyba = allGOs[i].GetComponent<CyberAccess>();
                if (cyba == null) continue;

                MissingComponent(script,allGOs[i],typeof(TargetIO));
                MissingReference(script,allGOs[i],cyba.entryPosition,
                                 "entryPosition");
            }
        }

        [UnityTest]
        public IEnumerator CyberSwitchSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "CyberSwitch";
            bool check = true;

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
                CyberSwitch cybsw = allGOs[i].GetComponent<CyberSwitch>();
                if (cybsw == null) continue;

                MissingComponent(script,allGOs[i],typeof(TargetIO));
                msg = "has no target";
                check = !string.IsNullOrWhiteSpace(cybsw.target);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

            }
        }

        [UnityTest]
        public IEnumerator DoorsSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "Door";
            bool check = true;

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
                Door dor = allGOs[i].GetComponent<Door>();
                if (dor == null) continue;

                MissingComponent(script,allGOs[i],typeof(TargetIO));
                if (dor.ajar) {
                    msg = "set to ajar but no ajar percent";
                    check = dor.ajarPercentage > 0f && dor.ajarPercentage < 1f;
                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                }

                msg = "marked as keycard already used before game starts!";
                check = !dor.accessCardUsedByPlayer;
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "untranslated string lockedMessage";
                check = string.IsNullOrWhiteSpace(dor.lockedMessage);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                if (dor.toggleLasers) {
                    msg = "door toggles lasers but has none assigned";
                    check = dor.laserLines.Length > 0;
                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                }
            }
        }

        [UnityTest]
        public IEnumerator InteractablePanelsSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "InteractablePanel";
            bool check = true;

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
			    InteractablePanel intap =
                    allGOs[i].GetComponent<InteractablePanel>();

			    if (intap == null) continue;

                MissingComponent(script,allGOs[i],typeof(AudioSource));

                // Don't need to check for these 2 as it is used for the relay
                // panels on level 3 to display messages about systems.
                //msg = "panel starts open before game start";
                //check = !intap.open;
                //Assert.That(check,FailMessage(script,allGOs[i],msg));
                //
                //msg = "panel starts installed before game start";
                //check = !intap.installed;
                //Assert.That(check,FailMessage(script,allGOs[i],msg));

                if (intap.requiredIndex > -1) {
                    if (!intap.open) {
                        MissingComponent(script,allGOs[i],typeof(Animator));

                        msg = "panel missing openMessageLingdex";
                        check = intap.openMessageLingdex >= 0;
                        Assert.That(check,FailMessage(script,allGOs[i],msg));
                    }

                    if (!intap.installed) {
                        msg = "panel missing installedMessageLingdex";
                        check = intap.installedMessageLingdex >= 0;
                        Assert.That(check,FailMessage(script,allGOs[i],msg));
                    }

                    if (!intap.installed) {
                        msg = "panel missing wrongItemMessageLingdex";
                        check = intap.wrongItemMessageLingdex >= 0;
                        Assert.That(check,FailMessage(script,allGOs[i],msg));
                    }

                    msg = "panel missing alreadyInstalledMessageLingdex";
                    check = intap.alreadyInstalledMessageLingdex >= 0;
                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                }
            }
        }

        [UnityTest]
        public IEnumerator FuncWallsSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "FuncWall";
            bool check = true;

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
                FuncWall fw = allGOs[i].GetComponent<FuncWall>();
                if (fw == null) continue;

                MissingComponent(script,allGOs[i],typeof(TargetIO));

                msg = "no speed set, was " + fw.speed.ToString();
                check = fw.speed > 0;
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                MissingReference(script,allGOs[i],fw.targetPosition,
                                 "targetPosition");                

                if (fw.targetPosition != null) {
                    Vector3 pos = fw.targetPosition.transform.localPosition;

                    msg = "targetPosition.x nonzero at " + pos.x.ToString();
                    check = pos.x == 0;
                    Assert.That(check,FailMessage(script,allGOs[i],msg));

                    msg = "targetPosition.z nonzero at " + pos.z.ToString();
                    check = pos.z == 0;
                    Assert.That(check,FailMessage(script,allGOs[i],msg));

                    msg = "targetPosition.y zero (" + pos.y.ToString() + ")";
                    check = pos.y != 0;
                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                }

                msg = "no chunks assigned";
                check = fw.chunkIDs.Length > 0;
                Assert.That(check,FailMessage(script,allGOs[i],msg));
            }
        }

        [UnityTest]
        public IEnumerator KeypadKeycodesSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "KeypadKeycode";
            bool check = true;

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
                KeypadKeycode keyco = allGOs[i].GetComponent<KeypadKeycode>();
                if (keyco == null) continue;


                msg = "no target for keycode keypad";
                check = !string.IsNullOrWhiteSpace(keyco.target);
                Assert.That(check,FailMessage(script,allGOs[i],msg));
                MissingComponent(script,allGOs[i],typeof(TargetIO));
                MissingComponent(script,allGOs[i],typeof(AudioSource));

                msg = "untranslated string successMessage";
                check = string.IsNullOrWhiteSpace(keyco.successMessage);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "untranslated string lockedMessage";
                check = string.IsNullOrWhiteSpace(keyco.lockedMessage);
                Assert.That(check,FailMessage(script,allGOs[i],msg));
            }
        }

        [UnityTest]
        public IEnumerator LogicBranchesSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "LogicBranch";
            bool check = true;

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
			LogicBranch logb = allGOs[i].GetComponent<LogicBranch>();
			    if (logb == null) continue;

				msg = "no target";
                check = !string.IsNullOrWhiteSpace(logb.target);
                Assert.That(check,FailMessage(script,allGOs[i],msg));
                MissingComponent(script,allGOs[i],typeof(TargetIO));

                TargetIO tio = allGOs[i].GetComponent<TargetIO>();
                if ((!logb.relayEnabled
                    || logb.autoFlipOnTarget) && tio != null) {

                    msg = "no targetname when not enabled";
                    check = !string.IsNullOrWhiteSpace(tio.targetname);
                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                }
            }
        }

        [UnityTest]
        public IEnumerator LogicRelaysSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "LogicRelay";
            bool check = true;

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
			    LogicRelay leray = allGOs[i].GetComponent<LogicRelay>();
			    if (leray == null) continue;

				msg = "no target";
                check = !string.IsNullOrWhiteSpace(leray.target);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                MissingComponent(script,allGOs[i],typeof(TargetIO));

				msg = "no targetname";
                TargetIO tio = allGOs[i].GetComponent<TargetIO>();
                if (tio != null) {
                    check = !string.IsNullOrWhiteSpace(tio.targetname);
                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                }
			}
        }

        [UnityTest]
        public IEnumerator LogicTimersSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "LogicTimer";
            bool check = true;

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
                LogicTimer limer = allGOs[i].GetComponent<LogicTimer>();
                if (limer == null) continue;

 				msg = "no target";
                check = !string.IsNullOrWhiteSpace(limer.target);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                MissingComponent(script,allGOs[i],typeof(TargetIO));
                if (!limer.active) {
                    msg = "no targetname for inactive timer";
                    TargetIO tio = allGOs[i].GetComponent<TargetIO>();
                    if (tio != null) {
                        check = !string.IsNullOrWhiteSpace(tio.targetname);
                        Assert.That(check,FailMessage(script,allGOs[i],msg));
                    }
                }

 				msg = "no timer value";
                check = limer.timeInterval > 0f;
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                if (limer.useRandomTimes) {
                    msg = "randomMin is not less than randomMax";
                    check = limer.randomMin < limer.randomMax;
                    Assert.That(check,FailMessage(script,allGOs[i],msg));

                    msg = "randomMin is negative";
                    check = limer.randomMin >= 0f;
                    Assert.That(check,FailMessage(script,allGOs[i],msg));

                    msg = "randomMax is zero or negative";
                    check = limer.randomMax > 0f;
                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                }
			}
        }

        [UnityTest]
        public IEnumerator PuzzleGridPuzzlesSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "PuzzleGridPuzzle";
            bool check = true;

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
			PuzzleGridPuzzle puzg = allGOs[i].GetComponent<PuzzleGridPuzzle>();
			    if (puzg == null) continue;

                msg = "no target";
                check = !string.IsNullOrWhiteSpace(puzg.target);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                MissingComponent(script,allGOs[i],typeof(TargetIO));

                msg = "no grid[]";
                check = puzg.grid.Length > 0;
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "no cellType[]";
                check = puzg.cellType.Length > 0;
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "puzzle solved before game start";
                check = !puzg.puzzleSolved;
                Assert.That(check,FailMessage(script,allGOs[i],msg));
			}
        }

        [UnityTest]
        public IEnumerator PuzzleWirePuzzlesSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "PuzzleWirePuzzle";
            bool check = true;

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
			PuzzleWirePuzzle puzw = allGOs[i].GetComponent<PuzzleWirePuzzle>();
			    if (puzw == null) continue;

                msg = "no target";
                check = !string.IsNullOrWhiteSpace(puzw.target);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                MissingComponent(script,allGOs[i],typeof(TargetIO));

                msg = "no wiresOn[]";
                check = puzw.wiresOn.Length > 0;
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "no rowsActive[]";
                check = puzw.rowsActive.Length > 0;
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "no currentPositionsLeft[]";
                check = puzw.currentPositionsLeft.Length > 0;
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "no currentPositionsRight[]";
                check = puzw.currentPositionsRight.Length > 0;
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "no solutionPositionsLeft[]";
                check = puzw.solutionPositionsLeft.Length > 0;
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "no solutionPositionsRight[]";
                check = puzw.solutionPositionsRight.Length > 0;
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                int k = 0;
                int j = 0;

                // Check that wires aren't shared for left side:
                for (k=0;k<puzw.currentPositionsLeft.Length;k++) {
                    for (j=0;j<puzw.currentPositionsLeft.Length;j++) {
                        if (k == j) continue;
                        if (!puzw.wiresOn[k] || !puzw.wiresOn[j]) continue;

                        msg = "puzzle left position " + k.ToString()
                              + " shared with position " + j.ToString();

                        check = puzw.currentPositionsLeft[k]
                                != puzw.currentPositionsLeft[j];

                        Assert.That(check,FailMessage(script,allGOs[i],msg));
                    }
                }

                // Check that wires aren't shared for right side:
                for (k=0;k<puzw.currentPositionsRight.Length;k++) {
                    for (j=0;j<puzw.currentPositionsRight.Length;j++) {
                        if (k == j) continue;
                        if (!puzw.wiresOn[k] || !puzw.wiresOn[j]) continue;

                        msg = "puzzle left position " + k.ToString()
                              + " shared with position " + j.ToString();

                        check = puzw.currentPositionsRight[k]
                                != puzw.currentPositionsRight[j];

                        Assert.That(check,FailMessage(script,allGOs[i],msg));
                    }
                }

                msg = "puzzle solved before game start";
                check = !puzw.puzzleSolved;
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                msg = "puzzle in use before game start";
                check = !puzw.inUse;
                Assert.That(check,FailMessage(script,allGOs[i],msg));
			}
        }

        [UnityTest]
        public IEnumerator QuestBitRelaysSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "QuestBitRelay";

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
			    QuestBitRelay qbr = allGOs[i].GetComponent<QuestBitRelay>();
			    if (qbr == null) continue;

                MissingComponent(script,allGOs[i],typeof(TargetIO));
            }
        }
        
        [UnityTest]
        public IEnumerator TriggersSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "Trigger";
		    bool check = true;

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
			    Trigger trig = allGOs[i].GetComponent<Trigger>();
			    if (trig == null) continue;
			    
				msg = "no target";
                check = !string.IsNullOrWhiteSpace(trig.target);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

                MissingComponent(script,allGOs[i],typeof(TargetIO));
			}
        }
        
        [UnityTest]
        public IEnumerator TriggerCountersSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "TriggerCounter";
		    bool check = true;

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
			    TriggerCounter trigc = allGOs[i].GetComponent<TriggerCounter>();
		    	if (trigc == null) continue;

                MissingComponent(script,allGOs[i],typeof(TargetIO));
                
                msg = "count not set, was " +trigc.countToTrigger.ToString();
                check = trigc.countToTrigger > 1;
                Assert.That(check,FailMessage(script,allGOs[i],msg));
			}
        }
        
        [UnityTest]
        public IEnumerator MeshRenderersSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();
            bool check = true;

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "MeshRenderer";

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
		    	MeshRenderer mrend = allGOs[i].GetComponent<MeshRenderer>();
		    	if (mrend == null) continue;
			
				msg = "has 0 materials";
                check = (mrend.sharedMaterials.Length >= 1);
                Assert.That(check,FailMessage(script,allGOs[i],msg));

				if (mrend.sharedMaterials.Length > 0) {
					for (int j=0; j < mrend.sharedMaterials.Length;j++) {
					    msg = "has missing material " + j.ToString();
                        check = (mrend.sharedMaterials[j] != null);
                        Assert.That(check,FailMessage(script,allGOs[i],msg));
					}
				}
			}
        }
        
        [UnityTest]
        public IEnumerator BioMonitorSetupProperly() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
		    string script = "BioMonitor";

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
		    	BioMonitor bio = allGOs[i].GetComponent<BioMonitor>();
		    	if (bio == null) continue;
			
			    MissingReference(script,allGOs[i],bio.heartRate,"heartRate");
			    
			    MissingReference(script,allGOs[i],bio.patchEffects,
			                     "patchEffects");
			                     
			    MissingReference(script,allGOs[i],bio.heartRateText,
			                     "heartRateText");
			                     
			    MissingReference(script,allGOs[i],bio.header,"header");
			    
			    MissingReference(script,allGOs[i],bio.patchesActiveText,
			                     "patchesActiveText");
			                     
			    MissingReference(script,allGOs[i],bio.bpmText,"bpmText");
			    
			    MissingReference(script,allGOs[i],bio.heartRate,"heartRate");
			    
			    MissingReference(script,allGOs[i],bio.fatigueDetailText,
			                     "fatigueDetailText");
			                     
			    MissingReference(script,allGOs[i],bio.fatigue,"fatigue");
            }
        }

        [UnityTest]
        public IEnumerator DetermineIfAllSaveIDsAreUnique() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());

            SaveObject so;
            List<GameObject> allParents =
                SceneManager.GetActiveScene().GetRootGameObjects().ToList();

            List<int> saveIDsFound = new List<int>();
            for (int i=0;i<allParents.Count;i++) {
                // find all SaveObject components, including inactive
                // (hence the true at the end)
                Component[] compArray = 
                    allParents[i].GetComponentsInChildren(typeof(SaveObject),
                                                        true);

                for (int k=0;k<compArray.Length;k++) {
                    so = compArray[k].gameObject.GetComponent<SaveObject>();
                    saveIDsFound.Add(so.SaveID);
                }
            }

            // Check for duplicate SaveIDs
            List<int> duplicateSaveIDs = saveIDsFound
                .GroupBy(id => id)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            // Assert that there are no duplicate SaveIDs
            Assert.That(duplicateSaveIDs.Count == 0,
                        "Duplicate SaveIDs found: "
                        + string.Join(", ", duplicateSaveIDs));
        }

        [UnityTest]
        public IEnumerator ConfirmChunksHaveTextureArrayAssigned() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();
            bool check = true;

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());
            string script = "PrefabIdentifier";
            MeshFilter mf;
            Material mat;

            // Run through all GameObjects and perform all tests
            for (int i=0;i<allGOs.Count;i++) {
                PrefabIdentifier pid =
                    allGOs[i].GetComponent<PrefabIdentifier>();

                if (pid == null) continue;
                if (pid.constIndex == 22) {//pid.constIndex < 2 || pid.constIndex > 304
                //    || pid.constIndex == 20 || pid.constIndex == 21
                //    || pid.constIndex == 22 || pid.constIndex == 279
                //    || pid.constIndex == 112 || pid.constIndex == 79
                //    || pid.constIndex == 78 || pid.constIndex == 93) {
                    continue;
                }

                // Have prefab gameobject now get its goods.
                Component[] mfArray = allGOs[i].GetComponentsInChildren(
                                            typeof(MeshFilter),true);

                for (int k=0;k<mfArray.Length;k++) {
                    mat = mfArray[k].gameObject.GetComponent<Material>();
                    if (mat == null) continue;
                    if (mat != Const.a.genericMaterials[48]) continue; // chunk

                    mf = mfArray[k].gameObject.GetComponent<MeshFilter>();
                    Color[] vertexColors = mf.sharedMesh.colors;
                    msg = "has no vertex colors";
                    check = vertexColors.Length > 0;
                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                    //if (!check) {
                    //    UnityEngine.Debug.Log(FailMessage(script,allGOs[i],
                    //                                      msg));
                    //}

                    if (!check) continue;

                    msg = "has some vertices whose vertex color.r is 0";
                    check = true;
                    for (int j=0;j<vertexColors.Length;j++) {
                        if (vertexColors[j].r == 0f) {
                            check = false;
                            break;
                        }
                    }
                    //if (!check) {
                    //    UnityEngine.Debug.Log(FailMessage(script,allGOs[i],
                    //                          msg));
                    //}
                    Assert.That(check,FailMessage(script,allGOs[i],msg));
                }
            }
        }

        [UnityTest]
        public IEnumerator ConfirmLevelManagerInScene() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            GameObject levMan = GameObject.Find("LevelManager");
            string msg = "No LevelManager found in Scene!";
            Assert.That(levMan != null,msg);
        }

        [UnityTest]
        public IEnumerator DiagnosticReport() {
            RunBeforeAnyTests();
            yield return new WaitWhile(() => SceneLoaded() == false);
            SetupTests();

            string msg = "RunBeforeAnyTests failed to populate allGOs: ";
            Assert.That(allGOs.Count > 1,msg + allGOs.Count.ToString());

            // Run through all GameObjects and perform all tests
            //for (int i=0;i<allGOs.Count;i++) {
            //    TargetIO tio = allGOs[i].GetComponent<TargetIO>();
            //    if (tio == null) continue;

 			//	if (tio.radiationTreatment) {
            //        UnityEngine.Debug.Log(allGOs[i].name
            //                              + " radiation treatment");
            //    }
			//}
        }

        // Utility Functions for messages and common checks
        // --------------------------------------------------------------------

        private string ParentChain(GameObject go) {
            if (go.transform.parent == null) return "none";

            StringBuilder s1 = new StringBuilder();
            s1.Clear();
            Transform tr = go.transform;
            while(tr.parent != null) {
                s1.Insert(0,"->");
                s1.Insert(0,tr.parent.name);
                tr = tr.parent;
            }

            return s1.ToString();
        }

        private string FailMessage(string className,GameObject go,string msg) {
            string self = (go != null) ? go.name : "errname";
            string parent = ParentChain(go);

            return ("BUG: " + className + " " + msg + " on GameObject " + self
                    + " with parent of " + parent);
        }

        private bool MissingReference(string className, GameObject go,
                                      GameObject[] elms, string variableName) {
            if (elms == null) {
                string msg = "missing reference " + elms.ToString() + "("
                             + variableName + ")";

                Assert.That(false,FailMessage(className,go,msg));
                return true;
            } else return false;
        }

        private bool MissingReference(string className, GameObject go,
                                      GameObject elem, string variableName) {
            if (elem == null) {
                string msg = "missing reference " + elem.ToString() + "("
                             + variableName + ")";

                Assert.That(false,FailMessage(className,go,msg));
                return true;
            } else return false;
        }

        private bool MissingReference(string className, GameObject go,
                                      Material mat, string variableName) {
            if (mat == null) {
                string msg = "missing reference " + mat.ToString() + "("
                             + variableName + ")";

                Assert.That(false,FailMessage(className,go,msg));
                return true;
            } else return false;
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
