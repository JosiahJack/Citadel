using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Add this script to anything that should be able to be targetted
public class TargetIO : MonoBehaviour {
	public string targetname;
	//public string[] target;
	// Action bits.  What do we want our target to do, e.g. turn on a light or close a door or activate force bridge
	// Using multiple bools to allow for multiple actions to be attempted on all the targets
	public bool tripTrigger; // force activate a trigger
	public bool doorOpen; // force opens the door
	public bool doorOpenIfUnlocked; // open a door only if it isn't locked
	public bool doorClose; // force closes the door
	public bool doorLock; // locks door, argvalue sets the locked message TODO: set the lock message in Door.cs
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

	void Start() {
		if (targetname != "" && targetname != " " & targetname != "  ") {
			RegisterToConst();
			if (disableThisGOOnAwake) this.gameObject.SetActive(false);
		} else {
			if (disableThisGOOnAwake) Debug.Log("BUG: Trying to disable a gameObject with a TargetIO.cs attached but no valid targetname!");
		}
	}

	void RegisterToConst() {
		Const.a.AddToTargetRegister(this.gameObject, targetname);
	}

	// comes from Const.a.UseTargets - already checked that target matched targetname of this interaction
	public void Targetted(UseData ud) {
		//Debug.Log("Entering Targetted() on a TargetIO.cs script!, called by Const.a.UseTargets! We made it!");

		// Whatever else happens, try to access a LogicRelay and keep the messages going
		LogicRelay lr = GetComponent<LogicRelay>();
		if (lr != null && lr.relayEnabled) lr.Targetted(ud);

		if (ud.tripTrigger) {
			Trigger trig = GetComponent<Trigger>();
			if (trig != null) trig.Targetted(ud);

			TriggerCounter trigcnt = GetComponent<TriggerCounter>();
			if (trigcnt != null) trigcnt.Targetted(ud);
		}

		if (ud.doorOpen) {
			Door dr = GetComponent<Door>();
			if (dr != null) dr.ForceOpen();
			Debug.Log("opening door!");
		}

		if (ud.doorOpenIfUnlocked) {
			Door dr = GetComponent<Door>();
			if (dr != null) {
				if (dr.locked == false && dr.accessCardUsedByPlayer) dr.ForceOpen();
			}
		}

		if (ud.doorClose) {
			Door dr = GetComponent<Door>();
			if (dr != null) dr.ForceClose();
		}

		if (ud.doorLock) {
			Door dr = GetComponent<Door>();
			if (dr != null) dr.Lock(ud.argvalue);
		}

		if (ud.doorUnlock) {
			Door dr = GetComponent<Door>();
			if (dr != null) {
				dr.Unlock();
				dr.accessCardUsedByPlayer = true;
			}
		}

		if (ud.switchTrigger) {
			ButtonSwitch bs = GetComponent<ButtonSwitch>();
			if (bs != null) bs.Targetted(ud);
		}

		if (ud.chargeStationRecharge) {
			ChargeStation chst = GetComponent<ChargeStation>();
			if (chst != null) chst.ForceRecharge();
		}

		if (ud.enemyAlert) {
			AIController aic = GetComponent<AIController>();
			NPC_Hopper nph = GetComponent<NPC_Hopper>();
			if (aic != null) aic.Alert(ud);
			if (nph != null) nph.Alert(ud);
		}

		if (ud.forceBridgeActivate) {
			ForceBridge fb = GetComponent<ForceBridge>();
			if (fb != null) fb.Activate();
		}

		if (ud.forceBridgeDeactivate) {
			ForceBridge fb = GetComponent<ForceBridge>();
			if (fb != null) fb.Deactivate();
		}

		if (ud.forceBridgeToggle) {
			ForceBridge fb = GetComponent<ForceBridge>();
			if (fb != null) fb.Toggle();
		}

		if (ud.gravityLiftToggle) {
			GravityLift gl = GetComponent<GravityLift>();
			if (gl != null) gl.Toggle();
		}

		if (ud.textureChangeToggle) {
			TextureChanger tch = GetComponent<TextureChanger>();
			if (tch != null) tch.Toggle();
		}

		if (ud.lightOn) {
			LightAnimation lam = GetComponent<LightAnimation>();
			if (lam != null) lam.TurnOn();
		}

		if (ud.lightOff) {
			LightAnimation lam = GetComponent<LightAnimation>();
			if (lam != null) lam.TurnOff();
		}

		if (ud.lightToggle) {
			LightAnimation lam = GetComponent<LightAnimation>();
			if (lam != null) lam.Toggle();
		}

		if (ud.funcwallMove) {
			FuncWall fw = GetComponent<FuncWall>();
			if (fw != null) fw.Targetted(ud);
		}

		if (ud.missionBitOn) {
			QuestBitRelay qbr = GetComponent<QuestBitRelay>();
			Debug.Log("EnablingBits on QBR!");
			if (qbr != null) qbr.EnableBits();
		}

		if (ud.missionBitOff) {
			QuestBitRelay qbr = GetComponent<QuestBitRelay>();
			if (qbr != null) qbr.DisableBits();
		}

		if (ud.missionBitToggle) {
			QuestBitRelay qbr = GetComponent<QuestBitRelay>();
			if (qbr != null) qbr.ToggleBits();
		}

		if (ud.sendEmail) {
			Email msg = GetComponent<Email>();
			if (msg != null) msg.Targetted();
		}

		if (ud.switchLockToggle) {
			ButtonSwitch btsw = GetComponent<ButtonSwitch>();
			if (btsw != null) btsw.ToggleLocked();
		}

		if (ud.lockCodeToScreenMaterialChanger) {
			MaterialChanger matchg = GetComponent<MaterialChanger>();
			if (matchg != null) matchg.Targetted(ud);
		}

		if (ud.spawnerActivate) {
			SpawnManager spwnmgr = GetComponent<SpawnManager>();
			if (spwnmgr != null) spwnmgr.Activate(false);
		}

		if (ud.spawnerActivateAlerted) {
			SpawnManager spwnmgr = GetComponent<SpawnManager>();
			if (spwnmgr != null) spwnmgr.Activate(true);
		}

		if (ud.cyborgConversionToggle) {
			LevelManager.a.CyborgConversionToggleForCurrentLevel();
			CyborgConversionToggle cctog = GetComponent<CyborgConversionToggle>();
			if (cctog != null) cctog.PlayVoxMessage();
		}

		if (ud.toggleRadiationTrigger) {
			Radiation rad = GetComponent<Radiation>();
			if (rad != null) rad.isEnabled = !rad.isEnabled;
		}

		if (ud.toggleRelayEnabled) {
			LogicRelay logrel = GetComponent<LogicRelay>();
			if (logrel != null) logrel.relayEnabled = !logrel.relayEnabled;
		}

		if (ud.togglePuzzlePanelLocked) {
			PuzzleGridPuzzle pgp = GetComponent<PuzzleGridPuzzle>();
			if (pgp != null) pgp.locked = !pgp.locked;

			PuzzleWirePuzzle pwp = GetComponent<PuzzleWirePuzzle>();
			if (pwp != null) pwp.locked = !pwp.locked;
		}

		if (ud.testQuestBitIsOn) {
			QuestBitRelay qbr = GetComponent<QuestBitRelay>();
			if (qbr != null) qbr.TestBits(true,ud,this);
		}

		if (ud.testQuestBitIsOff) {
			QuestBitRelay qbr = GetComponent<QuestBitRelay>();
			if (qbr != null) qbr.TestBits(false,ud,this);
		}

		if (ud.playSoundOnce) {
			PlaySoundTriggered pst = GetComponent<PlaySoundTriggered>();
			if (pst != null) pst.PlaySoundEffect();
		}

		if (ud.stopSound) {
			PlaySoundTriggered pst = GetComponent<PlaySoundTriggered>();
			if (pst != null) {
				pst.SFX.Stop();
				pst.currentlyPlaying = false;
			}
		}

		if (ud.sendSprintMessage) {
			TriggeredSprintMessage tsm = GetComponent<TriggeredSprintMessage>();
			if (tsm != null) Const.sprint(tsm.messageToDisplay,ud.owner);
		}

		if (ud.radiationTreatment) {
			PlayerReferenceManager prefman = ud.owner.GetComponent<PlayerReferenceManager>();
			if (prefman != null) prefman.playerRadiationTreatmentFlash.SetActive(true);
		}

		if (ud.startFlashingMaterials) {
			MaterialFlash mflash = GetComponent<MaterialFlash>();
			if (mflash != null) mflash.StartFlashing();
		}

		if (ud.stopFlashingMaterials) {
			MaterialFlash mflash = GetComponent<MaterialFlash>();
			if (mflash != null) mflash.StopFlashing();
		}

		if (ud.unlockElevatorPad) {
			KeypadElevator kelv = GetComponent<KeypadElevator>();
			if (kelv != null) kelv.locked = false;
		}

		if (ud.unlockKeycodePad) {
			KeypadKeycode keyk = GetComponent<KeypadKeycode>();
			if (keyk != null) keyk.locked = false;
		}
	}
}

public class UseData {
	public GameObject owner = null; // pass main GameObject that contains the script PlayerReferenceManager
	public int mainIndex = -1; // master index value for lookup in the Const tables
	public int customIndex = -1;
	public string argvalue = "";
	public bool bitsSet = false;

	// Action bits.  What do we want our target to do, e.g. turn on a light or close a door or activate force bridge
	// Using multiple bools to allow for multiple actions to be attempted on all the targets
	public bool tripTrigger; // force activate a trigger
	public bool doorOpen; // force opens the door
	public bool doorOpenIfUnlocked; // open a door only if it isn't locked
	public bool doorClose; // force closes the door
	public bool doorLock; // locks door, argvalue sets the locked message TODO: set the lock message in Door.cs
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
		bitsSet = true;
	}
}