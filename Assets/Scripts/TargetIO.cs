using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;

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
	public bool unlockSwitch; // unlock a ButtonSwitch
	public bool lockElevatorPad; // lock elevator keypad
	public bool alreadyDisabledThisGOOnceEver = false;
	public bool doorToggle;

	private UseData tempUD;
	private bool startInitialized = false;

	private void Start() {
		RemoteStart(this.gameObject,"self Start()");
	}
	
	public void RemoteStart(GameObject sender,string sourcefunc) {
// 		Debug.Log("TargetIO remote Start() by " + sender.name + "'s "
// 				  + sourcefunc);
		
		Const.a.AddToTargetRegister(this); // Always, since on load we need to refill register.
		Initialize();
	}
	
	public void Initialize() {
		if (startInitialized) return;

		if (!string.IsNullOrWhiteSpace(targetname)) {
			if (disableThisGOOnAwake && !alreadyDisabledThisGOOnceEver) {
				SaveObject so = GetComponent<SaveObject>();
				if (so == null) {
					Debug.Log("Adding SaveObject to a "
							  + "disabled on awake gameobject "
							  + gameObject.name + " with TargetIO.cs");

					gameObject.AddComponent(typeof(SaveObject));
					so = GetComponent<SaveObject>();
					if (so != null) {
						if (!so.initialized) so.Start();
						//so.saveType = SaveableType.Transform;
						//so.saveableType = "Transform";
					} else {
						Debug.Log("BUG: failed to add SaveObject to a "
								  + "disabled on awake gameobject "
								  + gameObject.name + " with TargetIO.cs");
					}
				}
				this.gameObject.SetActive(false);
			}
		} else {
			if (disableThisGOOnAwake && !alreadyDisabledThisGOOnceEver) {
				Debug.Log("BUG: Trying to disable a gameObject with a "
						  + "TargetIO.cs attached but no valid targetname!");
			}
		}
		startInitialized = true;
	}

	// Comes from Const.a.UseTargets - already checked that target matched
	// targetname of this interaction.
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

		if (tempUD.doorUnlock) {
			Door dr = GetComponent<Door>();
			if (dr != null) {
				dr.Unlock();
				dr.accessCardUsedByPlayer = true;
			}
		} // Unlock before open or toggle
		
		if (tempUD.doorOpen) {
			Door dr = GetComponent<Door>();
			if (dr != null) {
				dr.ForceOpen();
			}
			//Debug.Log("opening door!");
		}
		
		if (tempUD.doorOpenIfUnlocked) {
			Door dr = GetComponent<Door>();
			if (dr != null) {
				if (dr.locked == false && dr.accessCardUsedByPlayer) dr.ForceOpen();
			}
		}
		
		if (tempUD.doorToggle) {
			Door dr = GetComponent<Door>();
			if (dr != null) {
				if (dr.locked == false && dr.accessCardUsedByPlayer) dr.DoorActuate();
			}
			//Debug.Log("opening door!");
		}
		
		if (tempUD.doorClose) {
			Door dr = GetComponent<Door>();
			if (dr != null) dr.ForceClose();
		}
		
		if (tempUD.doorLock) { // Lock after forcing door into a position.
			Door dr = GetComponent<Door>();
			if (dr != null) dr.Lock(tempUD.argvalue);
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
			if (fb != null) fb.Activate(false);
		}

		if (tempUD.forceBridgeDeactivate) {
			ForceBridge fb = GetComponent<ForceBridge>();
			//Debug.Log("Deactivating force bridge");
			if (fb != null) fb.Deactivate(false);
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
			if (qbr != null) qbr.EnableBits();
		}

		if (tempUD.missionBitOff) {
			QuestBitRelay qbr = GetComponent<QuestBitRelay>();
			if (qbr != null) qbr.DisableBits();
		}

		if (tempUD.missionBitToggle) {
			QuestBitRelay qbr = GetComponent<QuestBitRelay>();
			if (qbr != null) qbr.ToggleBits();
		}

		if (tempUD.sendEmail) {
			//Debug.Log("sendEmail was true for Targetted() with targetname: " + targetname);
			Email msg = GetComponent<Email>();
			if (msg != null) {
				//Debug.Log("sendEmail was true and msg was found for Targetted() with targetname: " + targetname);
				msg.Targetted();
			}
		}

		if (tempUD.switchLockToggle) {
			ButtonSwitch btsw = GetComponent<ButtonSwitch>();
			if (btsw != null) btsw.ToggleLocked();
		}

		if (tempUD.unlockSwitch) {
			ButtonSwitch btsw = GetComponent<ButtonSwitch>();
			if (btsw != null) btsw.locked = false;
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
			if (rad != null) rad.enabled = !rad.enabled;
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
			if (PlayerReferenceManager.a != null) {
				PlayerReferenceManager.a.playerRadiationTreatmentFlash.SetActive(true);
				PlayerHealth.a.radiated = 0;
			}
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

		if (tempUD.lockElevatorPad) {
			KeypadElevator kelv = GetComponent<KeypadElevator>();
			if (kelv != null) kelv.locked = true;
		}
	}

	// Save searchable data
	public static string Save(GameObject go, PrefabIdentifier prefID) {
		TargetIO tio;
		if (go.name.Contains("se_corpse_eaten")) tio = go.transform.GetChild(0).GetComponent<TargetIO>(); // se_corpse_eaten
		else tio = go.GetComponent<TargetIO>(); 
		
		if (tio == null) {
			Transform par = go.transform.parent;
			string parname;
			if (par == null) parname = "-";
			else parname = par.gameObject.name;
			
			Debug.LogError("BUG: Missing TargetIO on " + go.name + " with parent of " + parname);
		}
		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		s1.Append(Utils.SaveString(tio.targetname,"targetname"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.tripTrigger,"tripTrigger"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.doorOpen,"doorOpen"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.doorOpenIfUnlocked,"doorOpenIfUnlocked"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.doorClose,"doorClose"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.doorLock,"doorLock"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.doorUnlock,"doorUnlock"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.switchTrigger,"switchTrigger"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.chargeStationRecharge,"chargeStationRecharge"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.enemyAlert,"enemyAlert"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.forceBridgeActivate,"forceBridgeActivate"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.forceBridgeDeactivate,"forceBridgeDeactivate"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.forceBridgeToggle,"forceBridgeToggle"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.gravityLiftToggle,"gravityLiftToggle"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.textureChangeToggle,"textureChangeToggle"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.lightOn,"lightOn"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.lightOff,"lightOff"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.lightToggle,"lightToggle"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.funcwallMove,"funcwallMove"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.missionBitOn,"missionBitOn"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.missionBitOff,"missionBitOff"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.missionBitToggle,"missionBitToggle"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.sendEmail,"sendEmail"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.switchLockToggle,"switchLockToggle"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.lockCodeToScreenMaterialChanger,"lockCodeToScreenMaterialChanger"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.spawnerActivate,"spawnerActivate"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.spawnerActivateAlerted,"spawnerActivateAlerted"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.cyborgConversionToggle,"cyborgConversionToggle"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.GOSetActive,"GOSetActive"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.GOSetDeactive,"GOSetDeactive"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.GOToggleActive,"GOToggleActive"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.toggleRadiationTrigger,"toggleRadiationTrigger"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.disableThisGOOnAwake,"disableThisGOOnAwake"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.toggleRelayEnabled,"toggleRelayEnabled"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.togglePuzzlePanelLocked,"togglePuzzlePanelLocked"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.testQuestBitIsOn,"testQuestBitIsOn"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.testQuestBitIsOff,"testQuestBitIsOff"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.playSoundOnce,"playSoundOnce"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.stopSound,"stopSound"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.sendSprintMessage,"sendSprintMessage"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.radiationTreatment,"radiationTreatment"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.startFlashingMaterials,"startFlashingMaterials"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.stopFlashingMaterials,"stopFlashingMaterials"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.unlockElevatorPad,"unlockElevatorPad"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.unlockKeycodePad,"unlockKeycodePad"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.unlockPuzzlePad,"unlockPuzzlePad"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.screenShake,"screenShake"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.awakeSleepingEnemy,"awakeSleepingEnemy"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.branchFlip,"branchFlip"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.branchFlipOnly,"branchFlipOnly"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.doorAccessCardOverrideToggle,"doorAccessCardOverrideToggle"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.unlockSwitch,"unlockSwitch"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.lockElevatorPad,"lockElevatorPad"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.alreadyDisabledThisGOOnceEver,"alreadyDisabledThisGOOnceEver"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.doorToggle,"doorToggle"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tio.gameObject.activeInHierarchy,"gameObject.activeInHierarchy"));
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index,
						   bool instantiated, PrefabIdentifier prefID) {
		TargetIO tio;
		if (go.name.Contains("se_corpse_eaten")) tio = go.transform.GetChild(0).GetComponent<TargetIO>(); // se_corpse_eaten
		else tio = go.GetComponent<TargetIO>(); 

		tio.targetname = Utils.LoadString(entries[index],"targetname"); index++;
		tio.tripTrigger = Utils.GetBoolFromString(entries[index],"tripTrigger"); index++;
		tio.doorOpen = Utils.GetBoolFromString(entries[index],"doorOpen"); index++;
		tio.doorOpenIfUnlocked = Utils.GetBoolFromString(entries[index],"doorOpenIfUnlocked"); index++;
		tio.doorClose = Utils.GetBoolFromString(entries[index],"doorClose"); index++;
		tio.doorLock = Utils.GetBoolFromString(entries[index],"doorLock"); index++;
		tio.doorUnlock = Utils.GetBoolFromString(entries[index],"doorUnlock"); index++;
		tio.switchTrigger = Utils.GetBoolFromString(entries[index],"switchTrigger"); index++;
		tio.chargeStationRecharge = Utils.GetBoolFromString(entries[index],"chargeStationRecharge"); index++;
		tio.enemyAlert = Utils.GetBoolFromString(entries[index],"enemyAlert"); index++;
		tio.forceBridgeActivate = Utils.GetBoolFromString(entries[index],"forceBridgeActivate"); index++;
		tio.forceBridgeDeactivate = Utils.GetBoolFromString(entries[index],"forceBridgeDeactivate"); index++;
		tio.forceBridgeToggle = Utils.GetBoolFromString(entries[index],"forceBridgeToggle"); index++;
		tio.gravityLiftToggle = Utils.GetBoolFromString(entries[index],"gravityLiftToggle"); index++;
		tio.textureChangeToggle = Utils.GetBoolFromString(entries[index],"textureChangeToggle"); index++;
		tio.lightOn = Utils.GetBoolFromString(entries[index],"lightOn"); index++;
		tio.lightOff = Utils.GetBoolFromString(entries[index],"lightOff"); index++;
		tio.lightToggle = Utils.GetBoolFromString(entries[index],"lightToggle"); index++;
		tio.funcwallMove = Utils.GetBoolFromString(entries[index],"funcwallMove"); index++;
		tio.missionBitOn = Utils.GetBoolFromString(entries[index],"missionBitOn"); index++;
		tio.missionBitOff = Utils.GetBoolFromString(entries[index],"missionBitOff"); index++;
		tio.missionBitToggle = Utils.GetBoolFromString(entries[index],"missionBitToggle"); index++;
		tio.sendEmail = Utils.GetBoolFromString(entries[index],"sendEmail"); index++;
		tio.switchLockToggle = Utils.GetBoolFromString(entries[index],"switchLockToggle"); index++;
		tio.lockCodeToScreenMaterialChanger = Utils.GetBoolFromString(entries[index],"lockCodeToScreenMaterialChanger"); index++;
		tio.spawnerActivate = Utils.GetBoolFromString(entries[index],"spawnerActivate"); index++;
		tio.spawnerActivateAlerted = Utils.GetBoolFromString(entries[index],"spawnerActivateAlerted"); index++;
		tio.cyborgConversionToggle = Utils.GetBoolFromString(entries[index],"cyborgConversionToggle"); index++;
		tio.GOSetActive = Utils.GetBoolFromString(entries[index],"GOSetActive"); index++;
		tio.GOSetDeactive = Utils.GetBoolFromString(entries[index],"GOSetDeactive"); index++;
		tio.GOToggleActive = Utils.GetBoolFromString(entries[index],"GOToggleActive"); index++;
		tio.toggleRadiationTrigger = Utils.GetBoolFromString(entries[index],"toggleRadiationTrigger"); index++;
		tio.disableThisGOOnAwake = Utils.GetBoolFromString(entries[index],"disableThisGOOnAwake"); index++;
		tio.toggleRelayEnabled = Utils.GetBoolFromString(entries[index],"toggleRelayEnabled"); index++;
		tio.togglePuzzlePanelLocked = Utils.GetBoolFromString(entries[index],"togglePuzzlePanelLocked"); index++;
		tio.testQuestBitIsOn = Utils.GetBoolFromString(entries[index],"testQuestBitIsOn"); index++;
		tio.testQuestBitIsOff = Utils.GetBoolFromString(entries[index],"testQuestBitIsOff"); index++;
		tio.playSoundOnce = Utils.GetBoolFromString(entries[index],"playSoundOnce"); index++;
		tio.stopSound = Utils.GetBoolFromString(entries[index],"stopSound"); index++;
		tio.sendSprintMessage = Utils.GetBoolFromString(entries[index],"sendSprintMessage"); index++;
		tio.radiationTreatment = Utils.GetBoolFromString(entries[index],"radiationTreatment"); index++;
		tio.startFlashingMaterials = Utils.GetBoolFromString(entries[index],"startFlashingMaterials"); index++;
		tio.stopFlashingMaterials = Utils.GetBoolFromString(entries[index],"stopFlashingMaterials"); index++;
		tio.unlockElevatorPad = Utils.GetBoolFromString(entries[index],"unlockElevatorPad"); index++;
		tio.unlockKeycodePad = Utils.GetBoolFromString(entries[index],"unlockKeycodePad"); index++;
		tio.unlockPuzzlePad = Utils.GetBoolFromString(entries[index],"unlockPuzzlePad"); index++;
		tio.screenShake = Utils.GetBoolFromString(entries[index],"screenShake"); index++;
		tio.awakeSleepingEnemy = Utils.GetBoolFromString(entries[index],"awakeSleepingEnemy"); index++;
		tio.branchFlip = Utils.GetBoolFromString(entries[index],"branchFlip"); index++;
		tio.branchFlipOnly = Utils.GetBoolFromString(entries[index],"branchFlipOnly"); index++;
		tio.doorAccessCardOverrideToggle = Utils.GetBoolFromString(entries[index],"doorAccessCardOverrideToggle"); index++;
		tio.unlockSwitch = Utils.GetBoolFromString(entries[index],"unlockSwitch"); index++;
		tio.lockElevatorPad = Utils.GetBoolFromString(entries[index],"lockElevatorPad"); index++;
		tio.alreadyDisabledThisGOOnceEver = Utils.GetBoolFromString(entries[index],"alreadyDisabledThisGOOnceEver"); index++;
		tio.doorToggle = Utils.GetBoolFromString(entries[index],"doorToggle"); index++;
		bool wasActive = Utils.GetBoolFromString(entries[index],"gameObject.activeInHierarchy");
		tio.Initialize();
		if (wasActive) Utils.Activate(tio.gameObject); index++;
		return index;
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
	public bool unlockSwitch; // unlock a ButtonSwitch
	public bool lockElevatorPad; // lock elevator pad
	public bool doorToggle; // Actuate door, similar to use to open/close

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
		unlockSwitch = tio.unlockSwitch;
		lockElevatorPad = tio.lockElevatorPad;
		doorToggle = tio.doorToggle;
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
		unlockSwitch = tio.unlockSwitch;
		lockElevatorPad = tio.lockElevatorPad;
		doorToggle = tio.doorToggle;
		bitsSet = true;
	}
}
