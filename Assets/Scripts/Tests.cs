﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tests : MonoBehaviour {
	public void RunUnits() {
		Stopwatch testTimer = new Stopwatch();
		testTimer.Start();

		GameObject go = new GameObject();


		
		ActivateButton ab = go.AddComponent<ActivateButton>();
		UnityEngine.Object.DestroyImmediate(ab);

		UnityEngine.Object.DestroyImmediate(go);

		testTimer.Stop();
		UnityEngine.Debug.Log("All unit tests completed in " + testTimer.Elapsed.ToString());
	}

	public void Run() {
		Stopwatch testTimer = new Stopwatch();
		testTimer.Start();

		int i=0;
		int k=0;
		List<GameObject> allGOs = new List<GameObject>();
		List<GameObject> allParents = SceneManager.GetActiveScene().GetRootGameObjects().ToList();
		for (i=0;i<allParents.Count;i++) {
			Component[] compArray = allParents[i].GetComponentsInChildren(typeof(Transform),true);
			for (k=0;k<compArray.Length;k++) {
				allGOs.Add(compArray[k].gameObject); // Add to full list, separate so we don't infinite loop
			}
		}

		// Declare and initialize cache values for all scripts
		/* ActivateButton */
		/* AIAnimationController */ int issueCount_AIAnimationController = 0; int num_AIAnimationController = 0;
		/* AIController */ int issueCount_AIController = 0; int num_AIController = 0;
		/* AIMeleeDamageCollider */
		/* AmbientRegistration */
		/* AmmoControl */
		/* AmmoIconManager */
		/* AnimatorDelayedStop */
		/* AutomapPlayerIcon */
		/* Billboard */
		/* BioMonitor */
		/* BiomonitorGraphSystem */
		/* ButtonSwitch */
		/* CameraView */
		/* CatchTray */
		/* ChargeStation */
		/* ClassDamageData */
		/* ConfigInputLabels */
		/* ConfigKeybindButton
		/* ConfigSlider */
		/* ConfigSliderValueDisplay */
		/* ConfigToggles */
		/* ConfigurationMenuAAApply */
		/* ConfigurationMenuVideo */
		/* ConfigurationMenuVideoApply */
		/* Const */
		/* CreditsScroll */
		/* CyberAccess */
		/* CyberBullet */
		/* CyberDataFragment */
		/* CyberDecoy */
		/* CyberExit */
		/* HealthManager */ int issueCount_HealthManager = 0; int num_HealthManagerController = 0;

		/* ElevatorButton */ int issueCount_ElevatorButton = 0; int num_ElevatorButton = 0;
		/* MouseLookScript */ //int issueCount_MouseLookScript = 0; int num_MouseLookScript = 0;

		/* MFDManager
		if (leftTC == null) Debug.Log("BUG: MFDManager missing manually assigned reference for leftTC");
		if (rightTC == null) Debug.Log("BUG: MFDManager missing manually assigned reference for rightTC");
		if (itemTabLH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for itemTabLH");
		if (itemTabRH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for itemTabRH");
		if (SearchFXRH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for SearchFXRH");
		if (SearchFXLH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for SearchFXLH");
		if (playerMLook == null) Debug.Log("BUG: MFDManager missing manually assigned reference for playerMLook");
		if (playerCapsuleTransform == null) Debug.Log("BUG: MFDManager missing manually assigned reference for playerCapsuleTransform");
		if (wepmagCounterLH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for wepmagCounterLH");
		if (wepmagCounterRH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for wepmagCounterRH");
		if (logReaderContainer == null) Debug.Log("BUG: MFDManager missing manually assigned reference for logReaderContainer");
		if (logTable == null) Debug.Log("BUG: MFDManager missing manually assigned reference for logTable");
		if (logLevelsFolder == null) Debug.Log("BUG: MFDManager missing manually assigned reference for logLevelsFolder");
		if (wepbutMan == null) Debug.Log("BUG: MFDManager missing manually assigned reference for wepbutMan");
		if (iconLH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for iconLH");
		if (iconRH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for iconRH");
		if (weptextLH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for weptextLH");
		if (weptextRH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for weptextRH");
		if (headerTextLH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for headerTextLH");
		if (headerTextRH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for headerTextRH");
		if (headerText_textLH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for headerText_textLH");
		if (headerText_textRH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for headerText_textRH");
		if (noItemsTextLH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for noItemsTextLH");
		if (noItemsTextRH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for noItemsTextRH");
		if (blockedBySecurityLH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for blockedBySecurityLH");
		if (blockedBySecurityRH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for blockedBySecurityRH");
		if (elevatorUIControlLH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for elevatorUIControlLH");
		if (elevatorUIControlRH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for elevatorUIControlRH");
		if (keycodeUIControlLH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for keycodeUIControlLH");
		if (keycodeUIControlRH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for keycodeUIControlRH");
		if (searchItemImagesLH.Length < 4) Debug.Log("BUG: MFDManager searchItemImagesLH[] incorrect length, needs to be 4.");
		if (searchItemImagesRH.Length < 4) Debug.Log("BUG: MFDManager searchItemImagesLH[] incorrect length, needs to be 4.");
		if (searchCloseButtonLH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for searchCloseButtonLH");
		if (searchCloseButtonRH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for searchCloseButtonRH");
		if (audioLogContainerLH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for audioLogContainerLH");
		if (audioLogContainerRH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for audioLogContainerRH");
		if (puzzleGridLH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for puzzleGridLH");
		if (puzzleGridRH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for puzzleGridRH");
		if (puzzleWireLH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for puzzleWireLH");
		if (puzzleWireRH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for puzzleWireRH");
		if (searchContainerLH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for searchContainerLH");
		if (searchContainerRH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for searchContainerRH");
		if (sysAnalyzerLH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for sysAnalyzerLH");
		if (sysAnalyzerRH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for sysAnalyzerRH");
		if (viewWeaponsContainer == null) Debug.Log("BUG: MFDManager missing manually assigned reference for viewWeaponsContainer");
		if (hardwareButtonsContainer == null) Debug.Log("BUG: MFDManager missing manually assigned reference for hardwareButtonsContainer");
		if (ctbButtonMain == null) Debug.Log("BUG: MFDManager missing manually assigned reference for ctbButtonMain");
		if (ctbButtonHardware == null) Debug.Log("BUG: MFDManager missing manually assigned reference for ctbButtonHardware");
		if (ctbButtonGeneral == null) Debug.Log("BUG: MFDManager missing manually assigned reference for ctbButtonGeneral");
		if (tabButtonsLHButtons == null) Debug.Log("BUG: MFDManager missing manually assigned reference for tabButtonsLHButtons");
		if (tabButtonsRHButtons == null) Debug.Log("BUG: MFDManager missing manually assigned reference for tabButtonsRHButtons");
		if (energyTickPanel == null) Debug.Log("BUG: MFDManager missing manually assigned reference for energyTickPanel");
		if (energyIndicator == null) Debug.Log("BUG: MFDManager missing manually assigned reference for energyIndicator");
		if (healthIndicator == null) Debug.Log("BUG: MFDManager missing manually assigned reference for healthIndicator");
		if (cyberHealthIndicator == null) Debug.Log("BUG: MFDManager missing manually assigned reference for cyberHealthIndicator");
		if (cyberTimerT == null) Debug.Log("BUG: MFDManager missing manually assigned reference for cyberTimerT");
		if (cyberTimer == null) Debug.Log("BUG: MFDManager missing manually assigned reference for cyberTimer");
		if (cyberSprintContainer == null) Debug.Log("BUG: MFDManager missing manually assigned reference for cyberSprintContainer");
		if (cyberSprintText == null) Debug.Log("BUG: MFDManager missing manually assigned reference for cyberSprintText");
		if (automapFull == null) Debug.Log("BUG: MFDManager missing manually assigned reference for automapFull");
		if (pm == null) Debug.Log("BUG: MFDManager missing manually assigned reference for pm");
		if (biograph == null) Debug.Log("BUG: MFDManager missing manually assigned reference for biograph");
		if (pe == null) Debug.Log("BUG: MFDManager missing manually assigned reference for pe");
		if (pp == null) Debug.Log("BUG: MFDManager missing manually assigned reference for pp");
		if (autoMapCamera == null) Debug.Log("BUG: MFDManager missing manually assigned reference for autoMapCamera");
		if (MainTabButton == null) Debug.Log("BUG: MFDManager missing manually assigned reference for MainTabButton.");
		if (HardwareTabButton == null) Debug.Log("BUG: MFDManager missing manually assigned reference for HardwareTabButton.");
		if (GeneralTabButton == null) Debug.Log("BUG: MFDManager missing manually assigned reference for GeneralTabButton.");
		if (SoftwareTabButton == null) Debug.Log("BUG: MFDManager missing manually assigned reference for SoftwareTabButton.");
		if (MFDSprite == null) Debug.Log("BUG: MFDManager missing manually assigned reference for MFDSprite.");
		if (MFDSpriteSelected == null) Debug.Log("BUG: MFDManager missing manually assigned reference for MFDSpriteSelected.");
		if (MFDSpriteNotification == null) Debug.Log("BUG: MFDManager missing manually assigned reference for MFDSpriteNotification.");
		if (TabSFX == null) Debug.Log("BUG: MFDManager missing manually assigned reference for TabSFX.");
		if (TabSFXClip == null) Debug.Log("BUG: MFDManager missing manually assigned reference for TabSFXClip.");
		if (MainTab == null) Debug.Log("BUG: MFDManager missing manually assigned reference for MainTab.");
		if (HardwareTab == null) Debug.Log("BUG: MFDManager missing manually assigned reference for HardwareTab.");
		if (GeneralTab == null) Debug.Log("BUG: MFDManager missing manually assigned reference for GeneralTab.");
		if (SoftwareTab == null) Debug.Log("BUG: MFDManager missing manually assigned reference for SoftwareTab.");
		if (DataReaderContentTab == null) Debug.Log("BUG: MFDManager missing manually assigned reference for DataReaderContentTab.");
		*/

		/* SearchableItem */ int issueCount_SearchableItem = 0; int num_SearchableItem = 0;
		/* MeshRenderer */ int issueCount_MeshRenderer = 0; int num_MeshRenderer = 0;

		string script = "null";

		// Run through all GameObjects and perform all tests
		for (i=0;i<allGOs.Count;i++) {
			
			script = "AIController";
			AIController aic = allGOs[i].GetComponent<AIController>();
			if (aic != null) {
				num_AIController++;
				if (BoundsError(script,allGOs[i],0,28,aic.index,"index"))				issueCount_AIController++;
				if (MissingComponent(script,allGOs[i],typeof(Rigidbody)))				issueCount_AIController++;
				if (MissingComponent(script,allGOs[i],typeof(HealthManager)))			issueCount_AIController++;
				if (aic.walkPathOnStart) {
					if (aic.walkWaypoints.Length < 1) {
						UnityEngine.Debug.Log(script + " missing any walkWaypoints but walkPathOnStart is true."); issueCount_AIController++;
					} else {
						for (k=0;k<aic.walkWaypoints.Length;k++) {
							if (aic.walkWaypoints[k] == null) UnityEngine.Debug.Log(script + " missing walkWaypoints["+k.ToString()+"]."); issueCount_AIController++;
						}
					}
				}
				if (aic.attack1Type == Const.AttackType.ProjectileLaunched && aic.attack1ProjectileLaunchedType == Const.PoolType.None) { UnityEngine.Debug.Log(script + " has no projectile set for attack1 but is intended to be projectile launched."); issueCount_AIController++; }
				if (aic.attack2Type == Const.AttackType.ProjectileLaunched && aic.attack2ProjectileLaunchedType == Const.PoolType.None) { UnityEngine.Debug.Log(script + " has no projectile set for attack2 but is intended to be projectile launched."); issueCount_AIController++; }
				if (aic.attack3Type == Const.AttackType.ProjectileLaunched && aic.attack3ProjectileLaunchedType == Const.PoolType.None) { UnityEngine.Debug.Log(script + " has no projectile set for attack3 but is intended to be projectile launched."); issueCount_AIController++; }
				if (BoundsError(script,allGOs[i],0f,1.0f,aic.deathBurstTimer,"deathBurstTimer")) issueCount_AIController++;
			}

			script = "AIAnimationController";
			AIAnimationController aiac = allGOs[i].GetComponent<AIAnimationController>();
			if (aiac != null) {
				num_AIAnimationController++;
				if (BoundsError(script,allGOs[i],0,10,aiac.minWalkSpeedToAnimate,"minWalkSpeedToAnimate")) { issueCount_AIAnimationController++; }
				if (MissingComponent(script,allGOs[i],typeof(Animator))) { issueCount_AIAnimationController++; }
				if (MissingReference(script,allGOs[i],aiac.aic,"aic")) { issueCount_AIAnimationController++; }
			}

			script = "HealthManager";
			HealthManager hm = allGOs[i].GetComponent<HealthManager>();
			if (hm != null) {
				num_HealthManagerController++;
				if (hm.isNPC) {
					if (MissingComponent(script,allGOs[i],typeof(AIController))) { issueCount_HealthManager++; }
					if (hm.isSecCamera || hm.isPlayer || hm.isObject || hm.isIce || hm.isScreen || hm.isGrenade) { UnityEngine.Debug.Log(script + " is marked as an NPC and another object as well."); issueCount_HealthManager++; }
					if (hm.teleportOnDeath && hm.teleportEffect == null) { UnityEngine.Debug.Log(script + "has teleportOnDeath set but is missing teleportEffect."); issueCount_HealthManager++; }
				} else if (hm.teleportOnDeath) { UnityEngine.Debug.Log(script + "has teleportOnDeath set but is not marked as an NPC."); issueCount_HealthManager++; }
				if (hm.isSecCamera) {
					if (BoundsError(script,allGOs[i],0,13,hm.levelIndex,"levelIndex")) { issueCount_HealthManager++; }
					if (!hm.isObject) { UnityEngine.Debug.Log(script + " is marked as an security camera but isn't also marked as isObject."); issueCount_HealthManager++; }
					if (hm.isNPC || hm.isPlayer || hm.isIce || hm.isScreen || hm.isGrenade) { UnityEngine.Debug.Log(script + " is marked as an security camera and another object as well."); issueCount_HealthManager++; }
					// if (hm.linkedCameraOverlay == null) { UnityEngine.Debug.Log(script + " is marked as an security camera and has no linkedCameraOverlay for gameObject " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); issueCount_HealthManager++; }
					if (hm.securityAffected == LevelManager.SecurityType.None) { UnityEngine.Debug.Log(script + " is marked as an security camera and securityAffected is marked as None for gameObject " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); issueCount_HealthManager++; }
				}
				if (hm.isPlayer) {
					if (hm.isNPC || hm.isSecCamera || hm.isObject || hm.isIce || hm.isScreen || hm.isGrenade) { UnityEngine.Debug.Log(script + " is marked as a player and another object as well."); issueCount_HealthManager++; }
				}
				if (hm.isGrenade) {
					if (MissingComponent(script,allGOs[i],typeof(GrenadeActivate))) { issueCount_HealthManager++; }
					if (hm.isNPC || hm.isSecCamera || hm.isObject || hm.isIce || hm.isScreen || hm.isPlayer) { UnityEngine.Debug.Log(script + " is marked as a grenade and another object as well."); issueCount_HealthManager++; }
				}
				if (hm.isObject) {
					if (hm.isNPC || hm.isGrenade || hm.isIce || hm.isScreen || hm.isPlayer) { UnityEngine.Debug.Log(script + " is marked as an object and another object as well."); issueCount_HealthManager++; }  // Can also be security camera (isSecCamera)
				}
				if (hm.isScreen) {
					if (hm.backupDeathSound == null) { UnityEngine.Debug.Log(script + " missing backupDeathSound when isScreen on gameObject " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); issueCount_HealthManager++; }
					if (MissingComponent(script,allGOs[i],typeof(ImageSequenceTextureArray))) { issueCount_HealthManager++; }
					if (hm.isNPC || hm.isSecCamera || hm.isGrenade || hm.isIce || hm.isObject || hm.isPlayer) { UnityEngine.Debug.Log(script + " is marked as a screen and another object as well."); issueCount_HealthManager++; }
				}
				if (hm.gibOnDeath) {
					if (hm.gibObjects.Length < 1 && !hm.dropItemsOnGib) { UnityEngine.Debug.Log(script + " no gibObjects when gibOnDeath is true on gameObject " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); issueCount_HealthManager++; }
					for (k=0;k<hm.gibObjects.Length;k++) {
						if (hm.gibObjects[k] == null && !hm.dropItemsOnGib) { UnityEngine.Debug.Log(script + "missing gibObjects["+k.ToString()+"]."); issueCount_HealthManager++; }
					} // Exception for dropItemsOnGib since some objects need to drop their searchables.
					if (hm.gibsGetVelocity && hm.gibVelocityBoost.magnitude < Mathf.Epsilon) { UnityEngine.Debug.Log(script + " has gibsGetVelocity set to true but it's gibVelocityBoost is zero."); issueCount_HealthManager++; }
				} else {
					if (hm.gibsGetVelocity) { UnityEngine.Debug.Log(script + " has gibsGetVelocity set to true but isn't also set to gibOnDeath."); issueCount_HealthManager++; }
				}
			}

			script = "ElevatorButton";
			ElevatorButton evb = allGOs[i].GetComponent<ElevatorButton>();
			if (evb != null) {
				num_ElevatorButton++;
				if (evb.GetComponentInChildren<Text>() == null) { UnityEngine.Debug.Log(script + " is missing childText Text component."); issueCount_ElevatorButton++; }
			}

			// script = "MouseLookScript";
			// MouseLookScript mls = allGOs[i].GetComponent<MouseLookScript>();
			// if (mls != null) {
				// num_MouseLookScript++;
				// if (num_MouseLookScript > 1) { UnityEngine.Debug.Log(script + " has more than one instance!!"); issueCount_MouseLookScript; }
				// if (mls.player == null) { UnityEngine.Debug.Log(script + " is missing player Text component."); issueCount_MouseLookScript++; }
			// }

			script = "SearchableItem";
			SearchableItem sitem = allGOs[i].GetComponent<SearchableItem>();
			if (sitem != null) {
				num_SearchableItem++;
				if (sitem.contents[0] < -1 || sitem.contents[0] > 94) { UnityEngine.Debug.Log(script + " has invalid item index of " + sitem.contents[0].ToString() + " for content 0, with parent of " + allGOs[i].transform.parent.name); issueCount_SearchableItem++; }
				if (sitem.contents[1] < -1 || sitem.contents[1] > 94) { UnityEngine.Debug.Log(script + " has invalid item index of " + sitem.contents[1].ToString() + " for content 1, with parent of " + allGOs[i].transform.parent.name); issueCount_SearchableItem++; }
				if (sitem.contents[2] < -1 || sitem.contents[2] > 94) { UnityEngine.Debug.Log(script + " has invalid item index of " + sitem.contents[2].ToString() + " for content 2, with parent of " + allGOs[i].transform.parent.name); issueCount_SearchableItem++; }
				if (sitem.contents[3] < -1 || sitem.contents[3] > 94) { UnityEngine.Debug.Log(script + " has invalid item index of " + sitem.contents[3].ToString() + " for content 3, with parent of " + allGOs[i].transform.parent.name); issueCount_SearchableItem++; }
				if (sitem.customIndex[0] < -1) { UnityEngine.Debug.Log(script + " has invalid customIndex of " + sitem.customIndex[0].ToString() + " for content 0, with parent of " + allGOs[i].transform.parent.name); issueCount_SearchableItem++; }
				if (sitem.customIndex[1] < -1) { UnityEngine.Debug.Log(script + " has invalid customIndex of " + sitem.customIndex[1].ToString() + " for content 1, with parent of " + allGOs[i].transform.parent.name); issueCount_SearchableItem++; }
				if (sitem.customIndex[2] < -1) { UnityEngine.Debug.Log(script + " has invalid customIndex of " + sitem.customIndex[2].ToString() + " for content 2, with parent of " + allGOs[i].transform.parent.name); issueCount_SearchableItem++; }
				if (sitem.customIndex[3] < -1) { UnityEngine.Debug.Log(script + " has invalid customIndex of " + sitem.customIndex[3].ToString() + " for content 3, with parent of " + allGOs[i].transform.parent.name); issueCount_SearchableItem++; }
				if (string.IsNullOrWhiteSpace(sitem.objectName)) { UnityEngine.Debug.Log(script + " has empty objectName of with parent of " + allGOs[i].transform.parent.name); issueCount_SearchableItem++; }
				if (sitem.generateContents) {
					if (sitem.randomItem.Length <= 0) { UnityEngine.Debug.Log(script + " is set to generate random contents but randomItem list to choose from is empty with parent of " + allGOs[i].transform.parent.name); issueCount_SearchableItem++; }
					if (sitem.randomItem.Length != sitem.randomItemCustomIndex.Length) { UnityEngine.Debug.Log(script + " has different length randomItem list versus randomItemCustomIndex list with parent of " + allGOs[i].transform.parent.name); issueCount_SearchableItem++; }
					if (sitem.randomItem.Length != sitem.randomItemDropChance.Length) { UnityEngine.Debug.Log(script + " has different length randomItem list versus randomItemDropChance list with parent of " + allGOs[i].transform.parent.name); issueCount_SearchableItem++; }
					if (sitem.maxRandomItems > 4) { UnityEngine.Debug.Log(script + " has too many maxRandomItems of " + sitem.maxRandomItems.ToString() + " with parent of " + allGOs[i].transform.parent.name); issueCount_SearchableItem++; }
				}
			}

			 script = "MeshRenderer";
			 MeshRenderer mrend = allGOs[i].GetComponent<MeshRenderer>();
			 if (mrend != null) {
				num_MeshRenderer++;
				if (mrend.sharedMaterials.Length < 1) { UnityEngine.Debug.Log(script + " on " + allGOs[i].name + " has 0 materials with parent of " + allGOs[i].transform.parent.name); issueCount_MeshRenderer++; }
				else {
					for (int j=0; j < mrend.sharedMaterials.Length;j++) {
						if (mrend.sharedMaterials[j] == null) { UnityEngine.Debug.Log(script + " on " + allGOs[i].name + " has missing material " + j.ToString() + " with parent of " + allGOs[i].transform.parent.name); issueCount_MeshRenderer++; }
					}
				}
			 }
		}

		// Ok now print result tallies
		PrintTally("AIController",				issueCount_AIController,				num_AIController);
		PrintTally("AIAnimationController",		issueCount_AIAnimationController,		num_AIAnimationController);
		PrintTally("HealthManager",				issueCount_HealthManager,				num_HealthManagerController);
		PrintTally("ElevatorButton",			issueCount_ElevatorButton,				num_ElevatorButton);
		PrintTally("SearchableItem",			issueCount_SearchableItem,				num_SearchableItem);
		PrintTally("MeshRenderer",				issueCount_MeshRenderer,				num_MeshRenderer);
		testTimer.Stop();
		UnityEngine.Debug.Log("All tests completed in " + testTimer.Elapsed.ToString());
	}

	private void PrintTally(string className, int issueCount, int objCount) {
		UnityEngine.Debug.Log(issueCount.ToString() + " " + className + " issues found on " + objCount.ToString() + " gameobjects");
	}

	private bool MissingReference(string className, GameObject go, Component comp, string variableName) {
		string self = (go != null) ? go.name : "errname";
		string parent = (go.transform.parent != null) ? go.transform.parent.name : "none";
		if (comp == null) {
			UnityEngine.Debug.Log("BUG: " + className + " is missing reference " + comp.ToString() + "(" + variableName + ") on GameObject " + self + " with parent of " + parent);
			return true;
		} else return false;
	}

	private bool MissingComponent(string className, GameObject go, System.Type type) {
		string self = (go != null) ? go.name : "errname";
		string parent = (go.transform.parent != null) ? go.transform.parent.name : "none";
		if (go.GetComponent(type) == null) {
			UnityEngine.Debug.Log("BUG: " + className + " is missing component " + type.ToString() + " on GameObject " + self + " with parent of " + parent);
			return true;
		} else return false;
	}

	private bool BoundsError(string className, GameObject go, int expectedMin, int expectedMax, int foundVal, string valueName) {
		string self = (go != null) ? go.name : "errname";
		string parent = (go.transform.parent != null) ? go.transform.parent.name : "none";
		if (foundVal > expectedMax || foundVal < expectedMin) {
			UnityEngine.Debug.Log("BUG: " + className + " has invalid value for "+ valueName +" on GameObject " + self + " with parent of " + parent + ". Expected value in range: "
			+ expectedMin.ToString() + " to " + expectedMax.ToString() + "; Found value: " + foundVal.ToString());
			return true;
		} else return false;
	}

	private bool BoundsError(string className, GameObject go, float expectedMin, float expectedMax, float foundVal, string valueName) {
		string self = (go != null) ? go.name : "errname";
		string parent = (go.transform.parent != null) ? go.transform.parent.name : "none";
		if (foundVal > expectedMax || foundVal < expectedMin) {
			UnityEngine.Debug.Log("BUG: " + className + " has invalid value for " + valueName + " on GameObject " + self + " with parent of " + parent + ". Expected value in range: "
			+ expectedMin.ToString() + " to " + expectedMax.ToString() + "; Found value: " + foundVal.ToString());
			return true;
		} else return false;
	}
}