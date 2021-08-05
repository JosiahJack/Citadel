using UnityEngine;
#if UNITY_EDITOR 
	using UnityEditor;
#endif
using System.Collections;

public class PlayerHealth : MonoBehaviour {
	//public float health = 211f; //max is 255
	public float radiated = 0f; // save
	public float resetAfterDeathTime = 0.5f;
	public float timer; // save
	public bool playerDead = false; // save
	public bool mediPatchActive = false; // save
	public float mediPatchPulseTime = 1f;
	public float mediPatchHealAmount = 10f;
	public bool detoxPatchActive = false; // save
	public AudioSource PlayerNoise;
	public AudioClip PainSFXClip;
	public AudioClip RadiationClip;
	public AudioClip ShieldClip;
	public AudioClip CyberMineSFXClip;
	public GameObject cameraObject;
	public GameObject radiationEffect;
	public GameObject shieldEffect;
	public bool radiationArea = false; // save
	private float radiationBleedOffFinished = 0f;
	public float radiationBleedOffTime = 1f;
	public float radiationReductionAmount = 1f;
	public float radiationHealthDamageRatio = 0.2f;
	public GameObject mainPlayerParent;
	public int radiationAmountWarningID = 323;
	public int radiationAreaWarningID = 322;
	public float mediPatchPulseFinished = 0f; // save
	public int mediPatchPulseCount = 0; // save
	public bool makingNoise = false; // save
	public HealthTickManager playerHealthTicks;
	public HealthTickManager playerCyberHealthTicks;
	[HideInInspector]
	public HealthManager hm;

	[HideInInspector]
	public float lastHealth; // save
	[HideInInspector]
	public float painSoundFinished; // save
	[HideInInspector]
	public float radSoundFinished; // save
	[HideInInspector]
	public float radFXFinished; // save
	private TextWarningsManager twm;
	private PlayerEnergy pe;
	private float radAdjust;
	private float initialRadiation;
	[HideInInspector]
	public float noiseFinished;
	public PlayerPatch pp;

	void Start () {
		twm = mainPlayerParent.GetComponent<PlayerReferenceManager>().playerTextWarningManager.GetComponent<TextWarningsManager>();
		if (twm == null) Debug.Log("BUG: No TextWarningManager script found on player (sent from PlayerHealth.Awake)");
		hm = GetComponent<HealthManager>();
		if (hm == null) Debug.Log("BUG: No HealthManager script found on player (sent from PlayerHealth.Awake)");
		pe = GetComponent<PlayerEnergy>();
		if (pe == null) Debug.Log("BUG: No PlayerEnergy script found on player (sent from PlayerHealth.Awake)");
		painSoundFinished = PauseScript.a.relativeTime;
		radSoundFinished = PauseScript.a.relativeTime;
		radFXFinished = PauseScript.a.relativeTime;
		noiseFinished = PauseScript.a.relativeTime;
		lastHealth = hm.health;
		radAdjust = 0f;
		initialRadiation = 0f;
	}

	void Update (){
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			//if (PlayerNoise.isPlaying)
				//makingNoise = true;
			//else
				//makingNoise = false;
			if (noiseFinished < PauseScript.a.relativeTime) makingNoise = false;

			if (hm.health <= 0f) {
				if (!playerDead) {
					PlayerDying();
					return;
				} else {
					PlayerDead();
					return;
				}
			}

			if (mediPatchActive) {
				if (mediPatchPulseFinished == 0) mediPatchPulseCount = 0;
				if (mediPatchPulseFinished < PauseScript.a.relativeTime) {
					float timePulse = mediPatchPulseTime;
					hm.HealingBed(mediPatchHealAmount,false);
					playerHealthTicks.DrawTicks();
					//hm.health += mediPatchHealAmount;
					timePulse += (mediPatchPulseCount*0.5f);
					mediPatchPulseFinished = PauseScript.a.relativeTime + timePulse;
					mediPatchPulseCount++;
				}
			} else {
				mediPatchPulseFinished = 0;
				mediPatchPulseCount = 0;
			}

			if (detoxPatchActive) {
				radiated = 0f;
			}

			//Debug.Log("Radiation level is " + radiated.ToString());
			if (radiated > 1) {
				if (radiationArea) twm.SendWarning((Const.a.stringTable[184]),0.1f,-2,TextWarningsManager.warningTextColor.white,radiationAreaWarningID); // Radiation area
				if (HardwareInventory.a.hasHardware[8]) {
					// Suit absorbs some radiation, say it.  Envirosuit absorbed ##, Radiation poisoning ## LBP
					twm.SendWarning((Const.a.stringTable[280]+radAdjust.ToString()+Const.a.stringTable[281] + Const.a.stringTable[185] + radiated.ToString()+Const.a.stringTable[186]),0.1f,-2,TextWarningsManager.warningTextColor.red,radiationAmountWarningID); // Envirosuit absorbed ##LBP, Radiation poisoning ##LBP
				} else {
					// Radiation poisoning ## LBP
					twm.SendWarning((Const.a.stringTable[185]+radiated.ToString()+Const.a.stringTable[186]),0.1f,-2,TextWarningsManager.warningTextColor.red,radiationAmountWarningID); // Radiation poisoning ##LBP
				}
				if (radFXFinished < PauseScript.a.relativeTime) {
					radiationEffect.SetActive(true);
					float minT = 0.5f;
					if (radiated > 50) minT = 0.25f;
					radFXFinished = PauseScript.a.relativeTime + Random.Range(minT,1f);
				}
			} else {
				radiationArea = false;
				radiated = 0;
			}

			if (radiationBleedOffFinished < PauseScript.a.relativeTime) {
				if (!radiationArea) radiated -= radiationReductionAmount;  // bleed off the radiation over time
				if (radiated < 0) radiated = 0;
				radiationBleedOffFinished = PauseScript.a.relativeTime + radiationBleedOffTime;

				if (radiated > 0) {
					if (!hm.god) {
						hm.health -= radiated*radiationHealthDamageRatio*radiationBleedOffTime; // apply health at rate of bleedoff time
						playerHealthTicks.DrawTicks();
					}
					if (radSoundFinished < PauseScript.a.relativeTime) {
						radSoundFinished = PauseScript.a.relativeTime + Random.Range(1f,3f);
						PlayerNoise.PlayOneShot(RadiationClip);
					}
				}
			}

			// Did we lose health?
			if (lastHealth > hm.health) {
				if (painSoundFinished < PauseScript.a.relativeTime && !(radSoundFinished < PauseScript.a.relativeTime)) {
					painSoundFinished = PauseScript.a.relativeTime + Random.Range(0.25f,3f); // Don't spam pain sounds
					PlayerNoise.PlayOneShot(PainSFXClip);
				}
			}
			lastHealth = hm.health;
		}
	}
	
	void PlayerDying (){
		timer += Time.deltaTime;
		makingNoise = false;
		playerHealthTicks.DrawTicks();
		if (timer >= resetAfterDeathTime) {
			hm.health = 0f;
			playerDead = true;
		}
	}
	
	void PlayerDead (){
		MouseLookScript mls = cameraObject.GetComponent<MouseLookScript>();
		if (mls == null) { Debug.Log("BUG: No mouselookscript for PlayerDead"); return; }

		if (mls.heldObjectIndex != -1) {
			mls.DropHeldItem();
			mls.ResetHeldItem ();
			mls.ResetCursor ();
			Cursor.lockState = CursorLockMode.None;
		}	
		int lindex = 0;
		if (LevelManager.a.currentLevel != -1) {
			lindex = LevelManager.a.currentLevel;
		} else {
			lindex = 0;
			//LevelManager.a.ressurectionActive[lindex] = true; // oopsies, debug time!
		}
		if (LevelManager.a.ressurectionActive[lindex]) {
			// Ressurection
			PlayerRessurect();
		} else {
			// Game Over
			PlayerDeathToMenu(mls);
		}
	}

	public void PlayerRessurect() {
		bool ressurected = LevelManager.a.RessurectPlayer(mainPlayerParent);
		if (!ressurected) Debug.Log("ERROR: failed to ressurect player!");
		hm.health = 211f;
		playerHealthTicks.DrawTicks();
		radiationArea = false;
		radiated = 0;
		playerDead = false;
		mediPatchActive = false;
		detoxPatchActive = false;
		pp.DisableAllPatches();
		pp.playerMovementScript.fatigue = 0f;
	}

	public void PlayerDeathToMenu(MouseLookScript mls) {
		hm.pstatic.Deactivate();
		// Death to Main Menu
		if (mls.inventoryMode == false) {
			mls.ToggleInventoryMode();
			mls.ToggleAudioPause();
		}
		PauseScript.a.mainMenu.SetActive(true);
		MainMenuHandler.a.returnToPause = false;
		hm.health = 211f;
		playerHealthTicks.DrawTicks();
		radiationArea = false;
		radiated = 0;
		playerDead = false;
		mediPatchActive = false;
		detoxPatchActive = false;
		pp.DisableAllPatches();
		pp.playerMovementScript.fatigue = 0f;
		/*
		#if UNITY_EDITOR
		if (Application.isEditor) {
			EditorApplication.isPlaying = false;
			return;
		}
		#endif
		*/
	}

	public void GiveRadiation (float rad) {
		if (playerDead) return;
		if (radiated < rad)
			radiated = rad;

		// Check for envirosuit and apply reduction based on version
		if (HardwareInventory.a.hasHardware[8] && pe.energy > 0) {
			radAdjust = radiated;
			float enerTake = 0.25f;
			switch (HardwareInventory.a.hardwareVersion[8]) {
				case 1: radAdjust *= 0.17f; enerTake = 0.25f*(radiated - radAdjust); break;
				case 2: radAdjust *= 0.15f; enerTake = 0.16f*(radiated - radAdjust); break;
				case 3: radAdjust *= 0.12f; enerTake = 0.11f*(radiated - radAdjust); break;
			}
			radiated *= radAdjust;
			radAdjust = initialRadiation - radiated;
			pe.TakeEnergy(enerTake);
		} else {
			radAdjust = 0f;
		}
		initialRadiation = radiated;
		//radiated -= suitReduction;
	}
}