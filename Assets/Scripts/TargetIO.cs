using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Add this script to anything that should be able to be targetted
public class TargetIO : MonoBehaviour {
	public string targetname;

	// Action bits.  What do we want our target to do, e.g. turn on a light or close a door or activate force bridge
	// Using multiple bools to allow for multiple actions to be attempted on all the targets
	public bool tripTrigger; // force activate a trigger
	public bool doorOpen; // force opens the door
	public bool doorOpenIfUnlocked; // open a door only if it isn't locked
	public bool doorClose; // force closes the door
	public bool doorLock; // locks door, argvalue sets the locked message
	public bool doorUnlock; // unlocks door
	public bool switchTrigger; // force use a switch
	public bool chargeStationRecharge; // force recharge a charging station
	public bool enemyAlert; // alert an enemy and pass owner as the new enemy
	public bool forceBridgeActivate; // activate a force bridge
	public bool forceBridgeDeactivate; // deactivate a force bridge
	public bool forceBridgeToggle; // toggle a force bridge
	public bool gravityLiftToggle; // activate a gravity lift
	public bool textureChangeToggle; // toggle a texture on something
	public bool lightOn; // turn on the light
	public bool lightOff; // turn out that light!
	public bool lightToggle; // flip the switch
	public bool funcwallMove; // target a moving wall
	public bool missionBitOn; // turn a mission quest bit on
	public bool missionBitOff; // turn a mission quest bit off, wait why?? because shield deactivated is possible
	public bool missionBitToggle; // toggle mission bit
	public bool sendEmail; // send all players an email
	public bool switchLockToggle; // toggle locked state of a ButtonSwitch
	public bool lockCodeToScreenMaterialChanger; // set the code on a screen after CPUs are destroyed
	public bool spawnerActivate; // activate a SpawnManager
	public bool spawnerActivateAlerted; // activate a SpawnManager and notify all enemies of the player's location
	public bool cyborgConversionToggle; // toggle cyborg conversion so player can respawn on current level
	public bool GOSetActive; // turn a gameObject on
	public bool GOSetDeactive; // turn a gameObject off
	public bool GOToggleActive; // toggle gameObject on/off
	public bool toggleRadiationTrigger; // toggle radiation on/off for a radiation trigger
	public bool disableThisGOOnAwake = false; // disable this gameobject so that it can be enabled later
	public bool toggleRelayEnabled; // toggle logic relay enabled state
	public bool togglePuzzlePanelLocked; // toggle whether a puzzle panel is locked or not
	public bool testQuestBitIsOn; // run target if a certain quest bit is on
	public bool testQuestBitIsOff; // run target if a certain quest bit is off
	public bool playSoundOnce; // play a sound effect
	public bool stopSound; // play a sound effect
	public bool sendSprintMessage; // sprint to the status bar
	public bool radiationTreatment; // flash radiation treatment static on player's screen who used the treatment
	public bool startFlashingMaterials; // enable flashing of materials blink blink blink blink blink!
	public bool stopFlashingMaterials; // disable flashing
	public bool unlockElevatorPad; // unlock elevator keypad
	public bool unlockKeycodePad; // unlock elevator keypad
	public bool unlockPuzzlePad; // unlock puzzle pad, grid or wire
	public bool screenShake; // shake the screen/earthquake
	public bool awakeSleepingEnemy; // awaken a sleeping enemy, e.g. the sec-2 bots that are in repair sleep on level 8
	public bool branchFlip; // flip logic_branchs
	public bool branchFlipOnly; // only flip the branch, not flip and fire
	public bool doorAccessCardOverrideToggle; // set that access card has already been used
	private UseData tempUD;

	void Start() {
		if (!string.IsNullOrWhiteSpace(targetname)) {
			RegisterToConst();
			if (disableThisGOOnAwake) {
				SaveObject so = GetComponent<SaveObject>();
				if (so == null) {
					gameObject.AddComponent(typeof(SaveObject));
					so = GetComponent<SaveObject>();
					if (so != null) {
						so.saveType = SaveObject.SaveableType.Transform;
						so.saveableType = "Transform";
					} else {
						Debug.Log("BUG: failed to add SaveObject to a disabled on awake gameobject with TargetIO.cs");
					}
				}
				this.gameObject.SetActive(false);
			}
		} else {
			if (disableThisGOOnAwake) Debug.Log("BUG: Trying to disable a gameObject with a TargetIO.cs attached but no valid targetname!");
		}
	}

	void RegisterToConst() {
		Const.a.AddToTargetRegister(this.gameObject, targetname);
	}

	// comes from Const.a.UseTargets - already checked that target matched targetname of this interaction
	public void Targetted(UseData ud) {
		tempUD = ud; // prevent overwrites in the stack
		//Debug.Log("Entering Targetted() on a TargetIO.cs script, with targetname: " + targetname);
		
		// Whatever else happens, try to access a LogicRelay and keep the messages going
		LogicRelay lr = GetComponent<LogicRelay>();
		if (lr != null && lr.relayEnabled) lr.Targetted(tempUD);

		GameEnd gend = GetComponent<GameEnd>();
		if (gend != null) gend.Targetted(tempUD);

		// or a LogicBranch since it also carries logic along
		if (!tempUD.branchFlipOnly) {
			LogicBranch lb = GetComponent<LogicBranch>();
			if (lb != null && lb.relayEnabled) lb.Targetted(tempUD);
		}
		if (tempUD.branchFlip || tempUD.branchFlipOnly) {
			// or a LogicBranch since it also carries logic along
			LogicBranch lbr = GetComponent<LogicBranch>();
			if (lbr != null) lbr.FlipTrackSwitch();
		}

		if (tempUD.tripTrigger) {
			Trigger trig = GetComponent<Trigger>();
			if (trig != null) trig.Targetted(tempUD);

			TriggerCounter trigcnt = GetComponent<TriggerCounter>();
			if (trigcnt != null) trigcnt.Targetted(tempUD);
		}

		if (tempUD.doorOpen) {
			Door dr = GetComponent<Door>();
			if (dr != null) dr.ForceOpen();
			//Debug.Log("opening door!");
		}

		if (tempUD.doorOpenIfUnlocked) {
			Door dr = GetComponent<Door>();
			if (dr != null) {
				if (dr.locked == false && dr.accessCardUsedByPlayer) dr.ForceOpen();
			}
		}

		if (tempUD.doorClose) {
			Door dr = GetComponent<Door>();
			if (dr != null) dr.ForceClose();
		}

		if (tempUD.doorLock) {
			Door dr = GetComponent<Door>();
			if (dr != null) dr.Lock(tempUD.argvalue);
		}

		if (tempUD.doorUnlock) {
			Door dr = GetComponent<Door>();
			if (dr != null) {
				dr.Unlock();
				dr.accessCardUsedByPlayer = true;
			}
		}

		if (tempUD.doorAccessCardOverrideToggle) {
			Door dr = GetComponent<Door>();
			if (dr != null) {
				dr.accessCardUsedByPlayer = !dr.accessCardUsedByPlayer;
			}
		}

		if (tempUD.switchTrigger) {
			ButtonSwitch bs = GetComponent<ButtonSwitch>();
			if (bs != null) bs.Targetted(tempUD);
		}

		if (tempUD.chargeStationRecharge) {
			ChargeStation chst = GetComponent<ChargeStation>();
			if (chst != null) chst.ForceRecharge();
		}

		if (tempUD.enemyAlert) {
			AIController aic = GetComponent<AIController>();
			if (aic != null) aic.Alert(tempUD);
		}

		if (tempUD.forceBridgeActivate) {
			ForceBridge fb = GetComponent<ForceBridge>();
			//Debug.Log("Activating force bridge");
			if (fb != null) fb.Activate(false,false);
		}

		if (tempUD.forceBridgeDeactivate) {
			ForceBridge fb = GetComponent<ForceBridge>();
			//Debug.Log("Deactivating force bridge");
			if (fb != null) fb.Deactivate(false,false);
		}

		if (tempUD.forceBridgeToggle) {
			//Debug.Log("Toggling force bridge");
			ForceBridge fb = GetComponent<ForceBridge>();
			if (fb != null) fb.Toggle();
		}

		if (tempUD.gravityLiftToggle) {
			GravityLift gl = GetComponent<GravityLift>();
			if (gl != null) gl.Toggle();
		}

		if (tempUD.textureChangeToggle) {
			TextureChanger tch = GetComponent<TextureChanger>();
			if (tch != null) tch.Toggle();
		}

		if (tempUD.lightOn) {
			LightAnimation lam = GetComponent<LightAnimation>();
			if (lam != null) lam.TurnOn();
		}

		if (tempUD.lightOff) {
			LightAnimation lam = GetComponent<LightAnimation>();
			if (lam != null) lam.TurnOff();
		}

		if (tempUD.lightToggle) {
			LightAnimation lam = GetComponent<LightAnimation>();
			if (lam != null) lam.Toggle();
		}

		if (tempUD.funcwallMove) {
			//Debug.Log("FuncWall move activated!");
			FuncWall fw = GetComponent<FuncWall>();
			if (fw != null) fw.Targetted(tempUD);
		}

		if (tempUD.missionBitOn) {
			QuestBitRelay qbr = GetComponent<QuestBitRelay>();
			Const.a.DebugQuestBitShoutOut();
			if (qbr != null) qbr.EnableBits();
		}

		if (tempUD.missionBitOff) {
			QuestBitRelay qbr = GetComponent<QuestBitRelay>();
			Const.a.DebugQuestBitShoutOut();
			if (qbr != null) qbr.DisableBits();
		}

		if (tempUD.missionBitToggle) {
			QuestBitRelay qbr = GetComponent<QuestBitRelay>();
			Const.a.DebugQuestBitShoutOut();
			if (qbr != null) qbr.ToggleBits();
		}

		if (tempUD.sendEmail) {
			Debug.Log("sendEmail was true for Targetted() with targetname: " + targetname);
			Email msg = GetComponent<Email>();
			if (msg != null) {
				Debug.Log("sendEmail was true and msg was found for Targetted() with targetname: " + targetname);
				msg.Targetted();
			}
		}

		if (tempUD.switchLockToggle) {
			ButtonSwitch btsw = GetComponent<ButtonSwitch>();
			if (btsw != null) btsw.ToggleLocked();
		}

		if (tempUD.lockCodeToScreenMaterialChanger) {
			MaterialChanger matchg = GetComponent<MaterialChanger>();
			if (matchg != null) matchg.Targetted(tempUD);
		}

		if (tempUD.spawnerActivate) {
			SpawnManager spwnmgr = GetComponent<SpawnManager>();
			if (spwnmgr != null) spwnmgr.Activate(false);
		}

		if (tempUD.spawnerActivateAlerted) {
			SpawnManager spwnmgr = GetComponent<SpawnManager>();
			if (spwnmgr != null) spwnmgr.Activate(true);
		}

		if (tempUD.cyborgConversionToggle) {
			LevelManager.a.CyborgConversionToggleForCurrentLevel();
			CyborgConversionToggle cctog = GetComponent<CyborgConversionToggle>();
			if (cctog != null) cctog.PlayVoxMessage();
		}

		if (tempUD.toggleRadiationTrigger) {
			Radiation rad = GetComponent<Radiation>();
			if (rad != null) rad.isEnabled = !rad.isEnabled;
		}

		if (tempUD.toggleRelayEnabled) {
			LogicRelay logrel = GetComponent<LogicRelay>();
			if (logrel != null) logrel.relayEnabled = !logrel.relayEnabled;
		}

		if (tempUD.togglePuzzlePanelLocked) {
			PuzzleGridPuzzle pgp = GetComponent<PuzzleGridPuzzle>();
			if (pgp != null) pgp.locked = !pgp.locked;

			PuzzleWirePuzzle pwp = GetComponent<PuzzleWirePuzzle>();
			if (pwp != null) pwp.locked = !pwp.locked;
		}

		if (tempUD.testQuestBitIsOn) {
			QuestBitRelay qbr = GetComponent<QuestBitRelay>();
			if (qbr != null) qbr.TestBits(true,tempUD,this);
		}

		if (tempUD.testQuestBitIsOff) {
			QuestBitRelay qbr = GetComponent<QuestBitRelay>();
			if (qbr != null) qbr.TestBits(false,tempUD,this);
		}

		if (tempUD.playSoundOnce) {
			PlaySoundTriggered pst = GetComponent<PlaySoundTriggered>();
			if (pst != null) pst.PlaySoundEffect();
		}

		if (tempUD.stopSound) {
			PlaySoundTriggered pst = GetComponent<PlaySoundTriggered>();
			if (pst != null) {
				pst.SFX.Stop();
				pst.currentlyPlaying = false;
			}
		}

		if (tempUD.sendSprintMessage) {
			TriggeredSprintMessage tsm = GetComponent<TriggeredSprintMessage>();
			if (tsm != null) Const.sprint(tsm.messageToDisplay,tempUD.owner);
		}

		if (tempUD.radiationTreatment) {
			PlayerReferenceManager prefman = tempUD.owner.GetComponent<PlayerReferenceManager>();
			if (prefman != null) prefman.playerRadiationTreatmentFlash.SetActive(true);
		}

		if (tempUD.startFlashingMaterials) {
			MaterialFlash mflash = GetComponent<MaterialFlash>();
			if (mflash != null) mflash.StartFlashing();
		}

		if (tempUD.stopFlashingMaterials) {
			MaterialFlash mflash = GetComponent<MaterialFlash>();
			if (mflash != null) mflash.StopFlashing();
		}

		if (tempUD.unlockElevatorPad) {
			KeypadElevator kelv = GetComponent<KeypadElevator>();
			if (kelv != null) kelv.locked = false;
		}

		if (tempUD.unlockKeycodePad) {
			KeypadKeycode keyk = GetComponent<KeypadKeycode>();
			if (keyk != null) keyk.locked = false;
		}

		if (tempUD.unlockPuzzlePad) {
			PuzzleGridPuzzle pgp = GetComponent<PuzzleGridPuzzle>();
			if (pgp != null) pgp.locked = false;

			PuzzleWirePuzzle pwp = GetComponent<PuzzleWirePuzzle>();
			if (pwp != null) pwp.locked = false;
		}

		if (tempUD.screenShake) {
			EffectScreenShake efsh = GetComponent<EffectScreenShake>();
			if (efsh != null) efsh.Shake();
		}

		if (tempUD.awakeSleepingEnemy) {
			AIController aic = GetComponent<AIController>();
			if (aic != null) aic.AwakeFromSleep(tempUD);
		}
	}
}

public class UseData {
	public GameObject owner = null; // pass main GameObject that contains the script PlayerReferenceManager
	public int mainIndex = -1; // master index value for lookup in the Const tables
	public int customIndex = -1;
	public string argvalue = "";
	public bool bitsSet = false;
	public Texture2D texture;

	// Action bits.  What do we want our target to do, e.g. turn on a light or close a door or activate force bridge
	// Using multiple bools to allow for multiple actions to be attempted on all the targets
	public bool tripTrigger; // force activate a trigger
	public bool doorOpen; // force opens the door
	public bool doorOpenIfUnlocked; // open a door only if it isn't locked
	public bool doorClose; // force closes the door
	public bool doorLock; // locks door, argvalue sets the locked message
	public bool doorUnlock; // unlocks door
	public bool switchTrigger; // force use a switch
	public bool chargeStationRecharge; // force recharge a charging station
	public bool enemyAlert; // alert an enemy and pass owner as the new enemy
	public bool forceBridgeActivate; // activate a force bridge
	public bool forceBridgeDeactivate; // deactivate a force bridge
	public bool forceBridgeToggle; // toggle a force bridge
	public bool gravityLiftToggle; // activate a gravity lift
	public bool textureChangeToggle; // toggle a texture on something
	public bool lightOn; // turn on the light
	public bool lightOff; // turn out that light!
	public bool lightToggle; // flip the switch
	public bool funcwallMove; // target a moving wall
	public bool missionBitOn; // turn a mission quest bit on
	public bool missionBitOff; // turn a mission quest bit off
	public bool missionBitToggle; // toggle mission bit
	public bool transferToLogicRelay; // send on to any relays to allow for special extra bits
	public bool sendEmail; // send all players an email
	public bool switchLockToggle; // toggle locked state of a ButtonSwitch
	public bool lockCodeToScreenMaterialChanger; // set the code on a screen after CPUs are destroyed
	public bool spawnerActivate; // activate a SpawnManager
	public bool spawnerActivateAlerted; // activate a SpawnManager and notify all enemies of the player's location
	public bool cyborgConversionToggle; // toggle cyborg conversion so player can respawn on current level
	public bool GOSetActive; // turn a gameObject on
	public bool GOSetDeactive; // turn a gameObject off
	public bool GOToggleActive; // toggle gameObject on/off
	public bool toggleRadiationTrigger; // toggle radiation on/off for a radiation trigger
	public bool toggleRelayEnabled; // toggle logic relay enabled state
	public bool togglePuzzlePanelLocked; // toggle whether a puzzle panel is locked or not
	public bool testQuestBitIsOn; // run target if a certain quest bit is on
	public bool testQuestBitIsOff; // run target if a certain quest bit is off
	public bool playSoundOnce; // play a sound effect
	public bool stopSound; // play a sound effect
	public bool sendSprintMessage; // sprint to the status bar
	public bool radiationTreatment; // flash radiation treatment static on player's screen who used the treatment
	public bool startFlashingMaterials; // enable flashing of materials blink blink blink blink blink!
	public bool stopFlashingMaterials; // disable flashing
	public bool unlockElevatorPad; // unlock elevator pad
	public bool unlockKeycodePad; // unlock elevator keypad
	public bool unlockPuzzlePad; // unlock puzzle pad, grid or wire
	public bool screenShake; // shake the screen/earthquake
	public bool awakeSleepingEnemy; // awaken a sleeping enemy, e.g. the sec-2 bots that are in repair sleep on level 8
	public bool branchFlip; // flip logic_branchs
	public bool branchFlipOnly; // only flip the branch, not flip and fire
	public bool doorAccessCardOverrideToggle; // set that access card has already been used

	// function for reseting all data if needed
	public void Reset (UseData ud) {
		ud.owner = null;
		ud.mainIndex = -1;
		ud.customIndex = -1;
		ud.argvalue = "";
	}

	public void SetBits(TargetIO tio) {
		tripTrigger = tio.tripTrigger;
		doorOpen = tio.doorOpen;
		doorOpenIfUnlocked = tio.doorOpenIfUnlocked;
		doorClose = tio.doorClose;
		doorLock = tio.doorLock;
		doorUnlock = tio.doorUnlock;
		switchTrigger = tio.switchTrigger;
		chargeStationRecharge = tio.chargeStationRecharge;
		enemyAlert = tio.enemyAlert;
		forceBridgeActivate = tio.forceBridgeActivate;
		forceBridgeDeactivate = tio.forceBridgeDeactivate;
		forceBridgeToggle = tio.forceBridgeToggle;
		gravityLiftToggle = tio.gravityLiftToggle;
		textureChangeToggle = tio.textureChangeToggle;
		lightOn = tio.lightOn;
		lightOff = tio.lightOff;
		lightToggle = tio.lightToggle;
		funcwallMove = tio.funcwallMove;
		missionBitOn = tio.missionBitOn;
		missionBitOff = tio.missionBitOff;
		missionBitToggle = tio.missionBitToggle;
		sendEmail = tio.sendEmail;
		switchLockToggle = tio.switchLockToggle;
		lockCodeToScreenMaterialChanger = tio.lockCodeToScreenMaterialChanger;
		spawnerActivate = tio.spawnerActivate;
		spawnerActivateAlerted = tio.spawnerActivateAlerted;
		cyborgConversionToggle = tio.cyborgConversionToggle;
		GOSetActive = tio.GOSetActive;
		GOSetDeactive = tio.GOSetDeactive;
		GOToggleActive = tio.GOToggleActive;
		toggleRadiationTrigger = tio.toggleRadiationTrigger;
		toggleRelayEnabled = tio.toggleRelayEnabled;
		togglePuzzlePanelLocked = tio.togglePuzzlePanelLocked;
		testQuestBitIsOn = tio.testQuestBitIsOn;
		testQuestBitIsOff = tio.testQuestBitIsOff;
		playSoundOnce = tio.playSoundOnce;
		stopSound = tio.stopSound;
		sendSprintMessage = tio.sendSprintMessage;
		radiationTreatment = tio.radiationTreatment;
		startFlashingMaterials = tio.startFlashingMaterials;
		stopFlashingMaterials = tio.stopFlashingMaterials;
		unlockElevatorPad = tio.unlockElevatorPad;
		unlockKeycodePad = tio.unlockKeycodePad;
		unlockPuzzlePad = tio.unlockPuzzlePad;
		screenShake = tio.screenShake;
		awakeSleepingEnemy = tio.awakeSleepingEnemy;
		branchFlip = tio.branchFlip;
		branchFlipOnly = tio.branchFlipOnly;
		doorAccessCardOverrideToggle = tio.doorAccessCardOverrideToggle;
		bitsSet = true;
	}

	public void CopyBitsFromUseData(UseData tio) {
		tripTrigger = tio.tripTrigger;
		doorOpen = tio.doorOpen;
		doorOpenIfUnlocked = tio.doorOpenIfUnlocked;
		doorClose = tio.doorClose;
		doorLock = tio.doorLock;
		doorUnlock = tio.doorUnlock;
		switchTrigger = tio.switchTrigger;
		chargeStationRecharge = tio.chargeStationRecharge;
		enemyAlert = tio.enemyAlert;
		forceBridgeActivate = tio.forceBridgeActivate;
		forceBridgeDeactivate = tio.forceBridgeDeactivate;
		forceBridgeToggle = tio.forceBridgeToggle;
		gravityLiftToggle = tio.gravityLiftToggle;
		textureChangeToggle = tio.textureChangeToggle;
		lightOn = tio.lightOn;
		lightOff = tio.lightOff;
		lightToggle = tio.lightToggle;
		funcwallMove = tio.funcwallMove;
		missionBitOn = tio.missionBitOn;
		missionBitOff = tio.missionBitOff;
		missionBitToggle = tio.missionBitToggle;
		sendEmail = tio.sendEmail;
		switchLockToggle = tio.switchLockToggle;
		lockCodeToScreenMaterialChanger = tio.lockCodeToScreenMaterialChanger;
		spawnerActivate = tio.spawnerActivate;
		spawnerActivateAlerted = tio.spawnerActivateAlerted;
		cyborgConversionToggle = tio.cyborgConversionToggle;
		GOSetActive = tio.GOSetActive;
		GOSetDeactive = tio.GOSetDeactive;
		GOToggleActive = tio.GOToggleActive;
		toggleRadiationTrigger = tio.toggleRadiationTrigger;
		toggleRelayEnabled = tio.toggleRelayEnabled;
		togglePuzzlePanelLocked = tio.togglePuzzlePanelLocked;
		testQuestBitIsOn = tio.testQuestBitIsOn;
		testQuestBitIsOff = tio.testQuestBitIsOff;
		playSoundOnce = tio.playSoundOnce;
		stopSound = tio.stopSound;
		sendSprintMessage = tio.sendSprintMessage;
		radiationTreatment = tio.radiationTreatment;
		startFlashingMaterials = tio.startFlashingMaterials;
		stopFlashingMaterials = tio.stopFlashingMaterials;
		unlockElevatorPad = tio.unlockElevatorPad;
		unlockKeycodePad = tio.unlockKeycodePad;
		unlockPuzzlePad = tio.unlockPuzzlePad;
		screenShake = tio.screenShake;
		awakeSleepingEnemy = tio.awakeSleepingEnemy;
		branchFlip = tio.branchFlip;
		branchFlipOnly = tio.branchFlipOnly;
		doorAccessCardOverrideToggle = tio.doorAccessCardOverrideToggle;
		bitsSet = true;
	}
}