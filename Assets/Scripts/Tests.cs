using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class Tests : MonoBehaviour {
	public GameObject[] lightContainers; // Can't use LevelManager's since
										 // there is no instance unless in Play
										 // mode.
	public GameObject[] dynamicObjectContainers;
	public GameObject gameObjectToSave;
	public int levelToOutputFrom = 0;
	public LevelManager lm;

	private bool getValparsed;
	private bool[] levelDataLoaded;
	private int getValreadInt;
	private float getValreadFloat;

	[HideInInspector] public string buttonLabel = "Run Tests";

	public void RunUnits() {
		Stopwatch testTimer = new Stopwatch();
		testTimer.Start();
		GameObject go = new GameObject();
		ActivateButton ab = go.AddComponent<ActivateButton>();
		UnityEngine.Object.DestroyImmediate(ab);
		UnityEngine.Object.DestroyImmediate(go);
		testTimer.Stop();
		UnityEngine.Debug.Log("All unit tests completed in "
							  + testTimer.Elapsed.ToString());
	}

	public void Run() {
		#if UNITY_EDITOR
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
		/* MouseLookScript */ int issueCount_MouseLookScript = 0; int num_MouseLookScript = 0;

		/* MFDManager
		if (leftTC == null) Debug.Log("BUG: MFDManager missing manually assigned reference for leftTC");
		if (rightTC == null) Debug.Log("BUG: MFDManager missing manually assigned reference for rightTC");
		if (itemTabLH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for itemTabLH");
		if (itemTabRH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for itemTabRH");
		if (SearchFXRH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for SearchFXRH");
		if (SearchFXLH == null) Debug.Log("BUG: MFDManager missing manually assigned reference for SearchFXLH");
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
		/* TargetIO */ int issueCount_TargetIO = 0; int num_TargetIO = 0;
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
					// if (hm.linkedOverlay == null) { UnityEngine.Debug.Log(script + " is marked as an security camera and has no linkedOverlay for gameObject " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); issueCount_HealthManager++; }
					if (hm.securityAffected == SecurityType.None) { UnityEngine.Debug.Log(script + " is marked as an security camera and securityAffected is marked as None for gameObject " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); issueCount_HealthManager++; }
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

				if (string.IsNullOrWhiteSpace(hm.targetOnDeath)) { 
					//UnityEngine.Debug.Log(script + " has no target on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == hm.targetOnDeath) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) { UnityEngine.Debug.Log(script + " has no matching targets for " + hm.targetOnDeath + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); }
				}
			}

			script = "ElevatorButton";
			ElevatorButton evb = allGOs[i].GetComponent<ElevatorButton>();
			if (evb != null) {
				num_ElevatorButton++;
				if (evb.GetComponentInChildren<Text>() == null) { UnityEngine.Debug.Log(script + " is missing childText Text component."); issueCount_ElevatorButton++; }
			}

			script = "MouseLookScript";
			MouseLookScript mls = allGOs[i].GetComponent<MouseLookScript>();
			if (mls != null) {
				num_MouseLookScript++;
				if (num_MouseLookScript > 1) { UnityEngine.Debug.Log(script + " has more than one instance!!"); issueCount_MouseLookScript++; }
				if (mls.player == null) { UnityEngine.Debug.Log(script + " is missing player."); issueCount_MouseLookScript++; }
				if (mls.canvasContainer == null) { UnityEngine.Debug.Log(script + " is missing canvasContainer."); issueCount_MouseLookScript++; }
				if (mls.compassContainer == null) { UnityEngine.Debug.Log(script + " is missing compassContainer."); issueCount_MouseLookScript++; }
				if (mls.automapContainerLH == null) { UnityEngine.Debug.Log(script + " is missing automapContainerLH."); issueCount_MouseLookScript++; }
				if (mls.automapContainerRH == null) { UnityEngine.Debug.Log(script + " is missing automapContainerRH."); issueCount_MouseLookScript++; }
				if (mls.compassMidpoints == null) { UnityEngine.Debug.Log(script + " is missing compassMidpoints."); issueCount_MouseLookScript++; }
				if (mls.compassLargeTicks == null) { UnityEngine.Debug.Log(script + " is missing compassLargeTicks."); issueCount_MouseLookScript++; }
				if (mls.compassSmallTicks == null) { UnityEngine.Debug.Log(script + " is missing compassSmallTicks."); issueCount_MouseLookScript++; }
				if (mls.tabControl == null) { UnityEngine.Debug.Log(script +  " is missing tabControl."); issueCount_MouseLookScript++; }
				if (mls.dataTabNoItemsText == null) { UnityEngine.Debug.Log(script + " is missing dataTabNoItemsText."); issueCount_MouseLookScript++; }
				if (mls.logContentsManager == null) { UnityEngine.Debug.Log(script + " is missing logContentsManager."); issueCount_MouseLookScript++; }
				if (mls.SFXSource == null) { UnityEngine.Debug.Log(script + " is missing SFXSource."); issueCount_MouseLookScript++; }
				if (mls.hardwareButtons == null) { UnityEngine.Debug.Log(script + " is missing hardwareButtons."); issueCount_MouseLookScript++; }
				if (mls.puzzleWire == null) { UnityEngine.Debug.Log(script + " is missing puzzleWire."); issueCount_MouseLookScript++; }
				if (mls.puzzleGrid == null) { UnityEngine.Debug.Log(script + " is missing puzzleGrid."); issueCount_MouseLookScript++; }
				if (mls.shootModeButton == null) { UnityEngine.Debug.Log(script + " is missing shootModeButton."); issueCount_MouseLookScript++; }
				if (mls.hm == null) { UnityEngine.Debug.Log(script + " is missing hm."); issueCount_MouseLookScript++; }
				if (mls.playerRadiationTreatmentFlash == null) { UnityEngine.Debug.Log(script + " is missing playerRadiationTreatmentFlash."); issueCount_MouseLookScript++; }

			}

			script = "SearchableItem";
			SearchableItem sitem = allGOs[i].GetComponent<SearchableItem>();
			if (sitem != null) {
				num_SearchableItem++;
				if (sitem.contents[0] < -1 || sitem.contents[0] > 110) { UnityEngine.Debug.Log(script + " has invalid item index of " + sitem.contents[0].ToString() + " for content 0, with parent of " + allGOs[i].transform.parent.name); issueCount_SearchableItem++; }
				if (sitem.contents[1] < -1 || sitem.contents[1] > 110) { UnityEngine.Debug.Log(script + " has invalid item index of " + sitem.contents[1].ToString() + " for content 1, with parent of " + allGOs[i].transform.parent.name); issueCount_SearchableItem++; }
				if (sitem.contents[2] < -1 || sitem.contents[2] > 110) { UnityEngine.Debug.Log(script + " has invalid item index of " + sitem.contents[2].ToString() + " for content 2, with parent of " + allGOs[i].transform.parent.name); issueCount_SearchableItem++; }
				if (sitem.contents[3] < -1 || sitem.contents[3] > 110) { UnityEngine.Debug.Log(script + " has invalid item index of " + sitem.contents[3].ToString() + " for content 3, with parent of " + allGOs[i].transform.parent.name); issueCount_SearchableItem++; }
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

			script = "ButtonSwitch";
			ButtonSwitch bsw = allGOs[i].GetComponent<ButtonSwitch>();
			if (bsw != null) {
				if (string.IsNullOrWhiteSpace(bsw.target) && !bsw.locked) { 
					UnityEngine.Debug.Log(script + " has no target on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == bsw.target) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) { UnityEngine.Debug.Log(script + " has no matching targets for " + bsw.target + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); }
				}

				if (bsw.GetComponent<MeshRenderer>() == null
					&& (bsw.changeMatOnActive || bsw.blinkWhenActive)) {
					UnityEngine.Debug.Log("BUG: ButtonSwitch missing component for mRenderer, "
							  			  + "name: " + bsw.gameObject.name);
				}

				if (bsw.mainSwitchMaterial == null
					&& (bsw.changeMatOnActive || bsw.blinkWhenActive)) {
					UnityEngine.Debug.Log("BUG: ButtonSwitch missing manually assigned reference"
										  + "for mainSwitchMaterial, name: " + bsw.gameObject.name);
				}

				if (bsw.alternateSwitchMaterial == null
					&& (bsw.changeMatOnActive || bsw.blinkWhenActive)) {
					UnityEngine.Debug.Log("BUG: ButtonSwitch missing manually assigned reference "
										  + "for alternateSwitchMaterial, name: "
										  + bsw.gameObject.name);
				}
			}

			script = "ChargeStation";
			ChargeStation chst = allGOs[i].GetComponent<ChargeStation>();
			if (chst != null) {
				if (string.IsNullOrWhiteSpace(chst.target)) { 
					// Not primary purpose here.  No need for this: UnityEngine.Debug.Log(script + " has no target on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == chst.target) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) { UnityEngine.Debug.Log(script + " has no matching targets for " + chst.target + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); }
				}
			}

			script = "CyberAccess";
			CyberAccess cyba = allGOs[i].GetComponent<CyberAccess>();
			if (cyba != null) {
				if (string.IsNullOrWhiteSpace(cyba.target)) { 
					// UnityEngine.Debug.Log(script + " has no target on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == cyba.target) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) { UnityEngine.Debug.Log(script + " has no matching targets for " + cyba.target + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); }
				}
			}

			script = "CyberSwitch";
			CyberSwitch cybsw = allGOs[i].GetComponent<CyberSwitch>();
			if (cybsw != null) {
				if (string.IsNullOrWhiteSpace(cybsw.target)) { 
					UnityEngine.Debug.Log(script + " has no target on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == cybsw.target) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) { UnityEngine.Debug.Log(script + " has no matching targets for " + cybsw.target + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); }
				}
			}

			script = "Door";
			Door dor = allGOs[i].GetComponent<Door>();
			if (dor != null) {
				if (string.IsNullOrWhiteSpace(dor.target)) { 
					//UnityEngine.Debug.Log(script + " has no target on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == dor.target) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) { UnityEngine.Debug.Log(script + " has no matching targets for " + dor.target + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); }
				}
			}

			script = "InteractablePanel";
			InteractablePanel intap = allGOs[i].GetComponent<InteractablePanel>();
			if (intap != null) {
				if (string.IsNullOrWhiteSpace(intap.target)) { 
					//UnityEngine.Debug.Log(script + " has no target on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == intap.target) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) { UnityEngine.Debug.Log(script + " has no matching targets for " + intap.target + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); }
				}
			}

			script = "KeypadKeycode";
			KeypadKeycode keyco = allGOs[i].GetComponent<KeypadKeycode>();
			if (keyco != null) {
				if (string.IsNullOrWhiteSpace(keyco.target)) { 
					UnityEngine.Debug.Log(script + " has no target on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == keyco.target) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) { UnityEngine.Debug.Log(script + " has no matching targets for " + keyco.target + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); }
				}
			}

			script = "LogicBranch";
			LogicBranch logb = allGOs[i].GetComponent<LogicBranch>();
			if (logb != null) {
				if (string.IsNullOrWhiteSpace(logb.target)) { 
					UnityEngine.Debug.Log(script + " has no target on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == logb.target) numtargetsfound++;
							if (!string.IsNullOrWhiteSpace(logb.target2)) {
								if (tioTemp.targetname == logb.target2) numtargetsfound++;
							}
						}
					}
					if (numtargetsfound < 1) { UnityEngine.Debug.Log(script + " has no matching targets for " + logb.target + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); }
				}
			}

			script = "LogicRelay";
			LogicRelay leray = allGOs[i].GetComponent<LogicRelay>();
			if (leray != null) {
				if (string.IsNullOrWhiteSpace(leray.target)) { 
					UnityEngine.Debug.Log(script + " has no target on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == leray.target) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) { UnityEngine.Debug.Log(script + " has no matching targets for " + leray.target + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); }
				}
			}

			script = "LogicTimer";
			LogicTimer limer = allGOs[i].GetComponent<LogicTimer>();
			if (limer != null) {
				if (string.IsNullOrWhiteSpace(limer.target)) { 
					UnityEngine.Debug.Log(script + " has no target on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == limer.target) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) { UnityEngine.Debug.Log(script + " has no matching targets for " + limer.target + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); }
				}
			}

			script = "PuzzleGridPuzzle";
			PuzzleGridPuzzle puzg = allGOs[i].GetComponent<PuzzleGridPuzzle>();
			if (puzg != null) {
				if (string.IsNullOrWhiteSpace(puzg.target)) { 
					UnityEngine.Debug.Log(script + " has no target on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == puzg.target) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) { UnityEngine.Debug.Log(script + " has no matching targets for " + puzg.target + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); }
				}
			}

			script = "PuzzleWirePuzzle";
			PuzzleWirePuzzle puzw = allGOs[i].GetComponent<PuzzleWirePuzzle>();
			if (puzw != null) {
				if (string.IsNullOrWhiteSpace(puzw.target)) { 
					UnityEngine.Debug.Log(script + " has no target on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == puzw.target) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) { UnityEngine.Debug.Log(script + " has no matching targets for " + puzw.target + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); }
				}
			}

			script = "QuestBitRelay";
			QuestBitRelay qbr = allGOs[i].GetComponent<QuestBitRelay>();
			if (qbr != null) {
				if (string.IsNullOrWhiteSpace(qbr.target)) { 
					//UnityEngine.Debug.Log(script + " has no target on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == qbr.target) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) { UnityEngine.Debug.Log(script + " has no matching targets for " + qbr.target + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); }
				}
			}

			script = "Trigger";
			Trigger trig = allGOs[i].GetComponent<Trigger>();
			if (trig != null) {
				if (string.IsNullOrWhiteSpace(trig.target)) {
					UnityEngine.Debug.Log(script + " has no target on "
										  + allGOs[i].name + " with parent of "
										  + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == trig.target) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) {
						UnityEngine.Debug.Log(script
											  + " has no matching targets for "
											  + trig.target + " on "
											  + allGOs[i].name
											  + " with parent of "
											  + allGOs[i].transform.parent.name);
					}
				}
			}

			script = "TriggerCounter";
			TriggerCounter trigc = allGOs[i].GetComponent<TriggerCounter>();
			if (trigc != null) {
				if (string.IsNullOrWhiteSpace(trigc.target)) { 
					UnityEngine.Debug.Log(script + " has no target on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == trigc.target) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) { UnityEngine.Debug.Log(script + " has no matching targets for " + trigc.target + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); }
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

			script = "BioMonitor";
			BioMonitor bio = allGOs[i].GetComponent<BioMonitor>();
			if (bio != null) {
				if (bio.heartRate == null) UnityEngine.Debug.Log("BUG: BioMonitor missing manually assigned reference for heartRate");
				if (bio.patchEffects == null) UnityEngine.Debug.Log("BUG: BioMonitor missing manually assigned reference for patchEffects");
				if (bio.heartRateText == null) UnityEngine.Debug.Log("BUG: BioMonitor missing manually assigned reference for heartRateText");
				if (bio.header == null) UnityEngine.Debug.Log("BUG: BioMonitor missing manually assigned reference for header");
				if (bio.patchesActiveText == null) UnityEngine.Debug.Log("BUG: BioMonitor missing manually assigned reference for patchesActiveText");
				if (bio.bpmText == null) UnityEngine.Debug.Log("BUG: BioMonitor missing manually assigned reference for bpmText");
				if (bio.fatigueDetailText == null) UnityEngine.Debug.Log("BUG: BioMonitor missing manually assigned reference for fatigueDetailText");
				if (bio.fatigue == null) UnityEngine.Debug.Log("BUG: BioMonitor missing manually assigned reference for fatigue");
			}

			//script = "Transform";
			//Transform tfm = allGOs[i].GetComponent<Transform>();
			//if (tfm.localScale.x < Mathf.Epsilon) { UnityEngine.Debug.Log(script + " on " + allGOs[i].name + " has negative x scale " + tfm.localScale.x.ToString() + " with parent of " + allGOs[i].transform.parent.name); }
			//if (tfm.localScale.y < Mathf.Epsilon) { UnityEngine.Debug.Log(script + " on " + allGOs[i].name + " has negative y scale " + tfm.localScale.y.ToString() + " with parent of " + allGOs[i].transform.parent.name); }
			//if (tfm.localScale.z < Mathf.Epsilon) { UnityEngine.Debug.Log(script + " on " + allGOs[i].name + " has negative z scale " + tfm.localScale.z.ToString() + " with parent of " + allGOs[i].transform.parent.name); }
		}

		// Ok now print result tallies
		PrintTally("AIController",				issueCount_AIController,				num_AIController);
		PrintTally("AIAnimationController",		issueCount_AIAnimationController,		num_AIAnimationController);
		PrintTally("HealthManager",				issueCount_HealthManager,				num_HealthManagerController);
		PrintTally("ElevatorButton",			issueCount_ElevatorButton,				num_ElevatorButton);
		PrintTally("SearchableItem",			issueCount_SearchableItem,				num_SearchableItem);
		PrintTally("MeshRenderer",				issueCount_MeshRenderer,				num_MeshRenderer);
		PrintTally("TargetIO",					issueCount_TargetIO,					num_TargetIO);
		testTimer.Stop();
		UnityEngine.Debug.Log("All tests completed in " + testTimer.Elapsed.ToString());
		buttonLabel = "Run Tests (Last was: " + testTimer.Elapsed.ToString() + ")";
		#endif
	}

	public struct LightGOData {
		public Vector3 position;
		public Vector3 rotation;
		public Color color;
		public float intensity;
		public bool isSpotlight;
	}

    //private List<GameObject> GetAllObjectsOnlyInScene() {
    //    List<GameObject> objectsInScene = new List<GameObject>();
    //    foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[]) {
    //        if (!EditorUtility.IsPersistent(go.transform.root.gameObject)
	//			&& !(go.hideFlags == HideFlags.NotEditable
	//			     || go.hideFlags == HideFlags.HideAndDontSave)) objectsInScene.Add(go);
    //    }

    //    return objectsInScene;
    //}

	public void LoadLevelLights() {
		lm.LoadLevelLights(levelToOutputFrom);
	}

	public void UnloadLevelLights() {
		lm.UnloadLevelLights(levelToOutputFrom);
	}

	public void GenerateLightsDataFile() {
		UnityEngine.Debug.Log("Outputting all lights to StreamingAssets/CitadelScene_lights_level" + levelToOutputFrom.ToString() + ".dat");
		StringBuilder s1 = new StringBuilder();
		List<GameObject> allLights = new List<GameObject>();
		Component[] compArray = lightContainers[levelToOutputFrom].GetComponentsInChildren(typeof(Light),true);
		for (int i=0;i<compArray.Length;i++) {
			if (compArray[i].gameObject.GetComponent<LightAnimation>() != null) {
				UnityEngine.Debug.Log("Skipping light with LightAnimation");
				continue;
			}
			if (compArray[i].gameObject.GetComponent<TargetIO>() != null) {
				UnityEngine.Debug.Log("Skipping light with TargetIO");
				continue;
			}
			allLights.Add(compArray[i].gameObject);
		}

		UnityEngine.Debug.Log("Found " + allLights.Count + " lights in level " + levelToOutputFrom.ToString());

		string lName = "CitadelScene_lights_level"
					   + levelToOutputFrom.ToString() + ".dat";

		string lP = Utils.SafePathCombine(Application.streamingAssetsPath,
										  lName);

		StreamWriter sw = new StreamWriter(lP,false,Encoding.ASCII);
		if (sw == null) {
			UnityEngine.Debug.Log("Lights output file path invalid");
			return;
		}

		using (sw) {
			for (int i=0;i<allLights.Count;i++) {
				s1.Clear();
				Transform tr = allLights[i].transform;
				s1.Append(Utils.SaveTransform(allLights[i].transform));
				s1.Append(Utils.splitChar);
				Light lit = allLights[i].GetComponent<Light>();
				s1.Append(Utils.FloatToString(lit.intensity));
				s1.Append(Utils.splitChar);
				s1.Append(Utils.FloatToString(lit.range));
				s1.Append(Utils.splitChar);
				s1.Append(lit.type.ToString());
				s1.Append(Utils.splitChar);
				s1.Append(Utils.FloatToString(lit.color.r));
				s1.Append(Utils.splitChar);
				s1.Append(Utils.FloatToString(lit.color.g));
				s1.Append(Utils.splitChar);
				s1.Append(Utils.FloatToString(lit.color.b));
				s1.Append(Utils.splitChar);
				s1.Append(Utils.FloatToString(lit.color.a));
				s1.Append(Utils.splitChar);
				s1.Append(Utils.FloatToString(lit.spotAngle));
				s1.Append(Utils.splitChar);
				s1.Append(lit.shadows.ToString());
				s1.Append(Utils.splitChar);
				s1.Append(Utils.FloatToString(lit.shadowStrength));
				s1.Append(Utils.splitChar);
				s1.Append(lit.shadowResolution);
				s1.Append(Utils.splitChar);
				s1.Append(Utils.FloatToString(lit.shadowBias));
				s1.Append(Utils.splitChar);
				s1.Append(Utils.FloatToString(lit.shadowNormalBias));
				s1.Append(Utils.splitChar);
				s1.Append(Utils.FloatToString(lit.shadowNearPlane));
				s1.Append(Utils.splitChar);
				s1.Append(lit.cullingMask.ToString());
				//UnityEngine.Debug.Log(s1.ToString());
				sw.Write(s1.ToString());
				sw.Write(Environment.NewLine);
			}
			sw.Close();
		}
	}

	public void LoadLevelDynamicObjects() {
		lm.LoadLevelDynamicObjects(levelToOutputFrom,dynamicObjectContainers[levelToOutputFrom]);
	}

	public void UnloadLevelDynamicObjects() {
		lm.UnloadLevelDynamicObjects(levelToOutputFrom);
	}

	public void GenerateDynamicObjectsDataFile() {
		UnityEngine.Debug.Log("Outputting all dynamic objects to StreamingAssets/CitadelScene_dynamics_level" + levelToOutputFrom.ToString() + ".dat");
		StringBuilder s1 = new StringBuilder();
		List<GameObject> allDynamicObjects = new List<GameObject>();
		Component[] compArray = dynamicObjectContainers[levelToOutputFrom].GetComponentsInChildren(typeof(SaveObject),true);
		for (int i=0;i<compArray.Length;i++) {
			allDynamicObjects.Add(compArray[i].gameObject);
		}

		UnityEngine.Debug.Log("Found " + allDynamicObjects.Count
							  + " dynamic objects in level "
							  + levelToOutputFrom.ToString());

		string dynName = "CitadelScene_dynamics_level"
					   + levelToOutputFrom.ToString() + ".dat";

		string dynP = Utils.SafePathCombine(Application.streamingAssetsPath,
										    dynName);

		StreamWriter sw = new StreamWriter(dynP,false,Encoding.ASCII);
		if (sw == null) {
			UnityEngine.Debug.Log("Lights output file path invalid");
			return;
		}

		using (sw) {
			for (int i=0;i<allDynamicObjects.Count;i++) {
				sw.Write(SaveObject.Save(allDynamicObjects[i]));
				sw.Write(Environment.NewLine);
			}
			sw.Close();
		}
	}

	public void SaveSelectedObject() {
		string line = SaveObject.Save(gameObjectToSave);
		string sName = "saving_unit_test.dat";
		string sP = Utils.SafePathCombine(Application.streamingAssetsPath,
										  sName);
		StreamWriter sw = new StreamWriter(sP,false,Encoding.ASCII);
		if (sw == null) {
			UnityEngine.Debug.Log("Save unit test output file path invalid");
			return;
		}

		using (sw) {;
			sw.Write(line);
			sw.Write(Environment.NewLine);
			sw.Close();
		}
		UnityEngine.Debug.Log("Saved data for " + gameObjectToSave.name);
	}

	public void LoadSelectedObject() {
		string lName = "saving_unit_test.dat";
		string lP = Utils.SafePathCombine(Application.streamingAssetsPath,
										  lName);

		Const.a.ConfirmExistsInStreamingAssetsMakeIfNot(lName);
		StreamReader sf = new StreamReader(lP);
		if (sf == null) {
			UnityEngine.Debug.Log("Save unit test input file path invalid");
			return;
		}

		string readline;
		List<string> readFileList = new List<string>();
		using (sf) {
			do {
				readline = sf.ReadLine();
				if (readline != null) {
					readFileList.Add(readline);
				}
			} while (!sf.EndOfStream);
			sf.Close();
		}

		string[] entries = readFileList[0].Split('|');
		int index = 5;
		index = SaveObject.Load(gameObjectToSave,ref entries);
		UnityEngine.Debug.Log("Loaded data for " + gameObjectToSave.name + ", contained " + index.ToString() + " entries on the line.");
	}

	public void TEMP_SetFunc_WallChunkIDs() {
		List<GameObject> allParents = SceneManager.GetActiveScene().GetRootGameObjects().ToList();
		for (int i=0;i<allParents.Count;i++) {
			Component[] compArray = allParents[i].GetComponentsInChildren(typeof(FuncWall),true); // Find all FuncWall components, including inactive (hence the true here at the end)
			for (int k=0;k<compArray.Length;k++) {
				TEMP_Func_Wall_SetChunkIDs(compArray[k].gameObject);
			}
		}
	}

	public void SetStaticSaveableIDs() {
		#if UNITY_EDITOR
			int idInc = 1000000;
			SaveObject so;
			List<GameObject> allParents = SceneManager.GetActiveScene().GetRootGameObjects().ToList();
			for (int i=0;i<allParents.Count;i++) {
				Component[] compArray = allParents[i].GetComponentsInChildren(typeof(SaveObject),true); // find all SaveObject components, including inactive (hence the true here at the end)
				for (int k=0;k<compArray.Length;k++) {
					so = compArray[k].gameObject.GetComponent<SaveObject>();
					so.SaveID = idInc; //add the gameObject associated with all SaveObject components in the scene
					//EditorUtility.SetDirty(so as Object);
					PrefabUtility.RecordPrefabInstancePropertyModifications(so);
					idInc++;
				}
			}
			Scene sc = SceneManager.GetActiveScene();
			//EditorSceneManager.MarkSceneDirty(sc);
			EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
		#endif
	}

	// Before: 15006, After 8000, for some reason it didn't work for all of them.
	public void RevertAll_m_CastShadows() {
		//List<GameObject> allParents = SceneManager.GetActiveScene().GetRootGameObjects().ToList();
		//for (int i=0;i<allParents.Count;i++) {
		//	Component[] compArray = allParents[i].GetComponentsInChildren(typeof(Transform),true);
		//	for (int k=0;k<compArray.Length;k++) {
		//		GameObject go = compArray[k].gameObject;
		//		if (!PrefabUtility.IsPartOfPrefabInstance(go)) continue;
		//		if (!PrefabUtility.HasPrefabInstanceAnyOverrides(go, false)) continue;

		//		List<ObjectOverride> ovides = PrefabUtility.GetObjectOverrides(go,false);
		//		for (int j=0; j < ovides.Count; j++) {
		//			UnityEngine.Object ob = ovides[j].instanceObject;
		//			SerializedObject sob = new UnityEditor.SerializedObject(ob);
		//			SerializedProperty prop = sob.FindProperty("m_CastShadows");
		//			if (prop == null) continue;
		//			if (!prop.prefabOverride) continue;

		//			UnityEngine.Debug.Log("Reverting m_CastShadows");
		//			PrefabUtility.RevertPropertyOverride(prop,InteractionMode.AutomatedAction);
		//		}


		//	}
		//}
	}
	// m_CastShadows
	// m_MotionVectors Before 19057
	// m_LightProbeUsage

	void TEMP_Func_Wall_SetChunkIDs(GameObject go) {
		UnityEngine.Debug.Log("go.name: " + go.name);
		FuncWall fw = go.GetComponent<FuncWall>();
		fw.chunkIDs = new int[go.transform.childCount];
		GameObject childGO;
		for (int i = 0; i < go.transform.childCount; i++) {
			childGO = go.transform.GetChild(i).gameObject;
			UnityEngine.Debug.Log("childGO.name: " + childGO.name);
			PrefabIdentifier pid = childGO.GetComponent<PrefabIdentifier>();
			if (pid == null) {
				UnityEngine.Debug.Log("ERROR: FuncWall child missing PrefabIdentifier");
				continue;
			}
			fw.chunkIDs[i] = pid.constIndex;
		}
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
