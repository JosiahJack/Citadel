using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {
	// External references, required
	public AudioSource SFX;
	public AudioClip PainSFXClip;
	public AudioClip RadiationClip;
	public AudioClip ShieldClip;
	public AudioClip CyberMineSFXClip;
	public GameObject cameraObject;
	public GameObject radiationEffect;
	public GameObject shieldEffect;
	public GameObject mainPlayerParent;

	// Internal references
	[HideInInspector] public float radiated = 0f; // save
	private float resetAfterDeathTime = 0.5f;
	[HideInInspector] public float timer; // save
	[HideInInspector] public bool playerDead = false; // save
	private float mediPatchPulseTime = 0.5f;
	private float mediPatchHealAmount = 8f;
	[HideInInspector] public bool radiationArea = false; // save
	private float radiationBleedOffFinished = 0f;
	private float radiationBleedOffTime = 1.8f;
	private float radiationReductionAmount = 1f;
	private float radiationHealthDamageRatio = 0.1f;
	private int radiationAmountWarningID = 323;
	private int radiationAreaWarningID = 322;
	[HideInInspector] public float mediPatchPulseFinished = 0f; // save
	[HideInInspector] public int mediPatchPulseCount = 0; // save, Used to incrementally increase the time between health increases by 0.5s every n+0.5s. Saved so we don't use quick load to cheat health faster.
	public bool makingNoise = false; // save
	[HideInInspector] public HealthManager hm;
	[HideInInspector] public float lastHealth; // save
	[HideInInspector] public float painSoundFinished; // save
	[HideInInspector] public float radSoundFinished; // save
	[HideInInspector] public float radFXFinished; // save
	private float radAdjust;
	private float initialRadiation;
	[HideInInspector] public float noiseFinished;

	public static PlayerHealth a;

	void Awake() {
		a = this;
	}

	void Start () {
		hm = GetComponent<HealthManager>();
		if (hm == null) Debug.Log("BUG: No HealthManager script found on player (sent from PlayerHealth.Awake)");
		painSoundFinished = PauseScript.a.relativeTime;
		radSoundFinished = PauseScript.a.relativeTime;
		radFXFinished = PauseScript.a.relativeTime;
		noiseFinished = PauseScript.a.relativeTime;
		lastHealth = hm.health;
		radAdjust = 0f;
		initialRadiation = 0f;
	}

	void Update() {
		if (PauseScript.a.Paused() || PauseScript.a.MenuActive()) return;

		if (noiseFinished < PauseScript.a.relativeTime) makingNoise = false;
		if (hm.health <= 0f) {
			if (!playerDead) PlayerDying();
			else PlayerDead();
			return;
		}

		if (Utils.CheckFlags(PlayerPatch.a.patchActive, PlayerPatch.a.PATCH_MEDI)) {
			if (mediPatchPulseFinished == 0) mediPatchPulseCount = 0;
			if (mediPatchPulseFinished < PauseScript.a.relativeTime) {
				hm.HealingBed(mediPatchHealAmount,false);
				MFDManager.a.DrawTicks(true);
				mediPatchPulseFinished = PauseScript.a.relativeTime + (mediPatchPulseTime + (mediPatchPulseCount * 0.5f));
				mediPatchPulseCount++;
			}
		} else {
			mediPatchPulseFinished = 0;
			mediPatchPulseCount = 0;
		}
		if (Utils.CheckFlags(PlayerPatch.a.patchActive, PlayerPatch.a.PATCH_DETOX)) radiated = 0f;
		if (radiated > 1f) {
			if (radiationArea) {
				// Radiation area
				PlayerMovement.a.twm.SendWarning((Const.a.stringTable[184]),
												  0.1f,-2,HUDColor.White,
												  radiationAreaWarningID);
			}

			if (!EnvirosuitApply()) {
				// Radiation poisoning ##LBP
				PlayerMovement.a.twm.SendWarning((Const.a.stringTable[185]
												  + radiated.ToString()
												  +Const.a.stringTable[186]),
												 0.1f,-2,HUDColor.Red,
												 radiationAmountWarningID);
			}

			if (radFXFinished < PauseScript.a.relativeTime) {
				radiationEffect.SetActive(true);
				float minT = 0.5f;
				if (radiated > 50f) minT = 0.25f;
				radFXFinished = PauseScript.a.relativeTime + Random.Range(minT,1f);
			}
		} else {
			radiationArea = false;
			if (radiated < 0) radiated = 0;
		}

		if (radiationBleedOffFinished < PauseScript.a.relativeTime) {
			if (!radiationArea) radiated -= radiationReductionAmount;  // Bleed off the radiation over time.
			if (radiated < 0) radiated = 0;
			radiationBleedOffFinished = PauseScript.a.relativeTime + radiationBleedOffTime;
			if (radiated > 0) {
				if (!hm.god) {
					hm.health -= radiated*radiationHealthDamageRatio; // Apply health at rate of bleedoff time.
					MFDManager.a.DrawTicks(true);
				}
				if (radSoundFinished < PauseScript.a.relativeTime) {
					radSoundFinished = PauseScript.a.relativeTime + Random.Range(1f,3f);
					Utils.PlayOneShotSavable(SFX,RadiationClip);
				}
			}
		}
		if (lastHealth > hm.health) { // Did we lose health?
			if (painSoundFinished < PauseScript.a.relativeTime && !(radSoundFinished < PauseScript.a.relativeTime)) {
				painSoundFinished = PauseScript.a.relativeTime + Random.Range(0.25f,3f); // Don't spam pain sounds
				Utils.PlayOneShotSavable(SFX,PainSFXClip);
			}
		}
		lastHealth = hm.health;
	}
	
	void PlayerDying (){
		timer += Time.deltaTime;
		makingNoise = false;
		MFDManager.a.DrawTicks(true);
		if (timer >= resetAfterDeathTime) {
			hm.health = 0f;
			playerDead = true;
		}
	}
	
	void PlayerDead (){
		if (MouseLookScript.a.heldObjectIndex != -1) {
			MouseLookScript.a.DropHeldItem();
			MouseLookScript.a.ForceInventoryMode();
		}	
		int lindex = LevelManager.a.currentLevel != -1 ? LevelManager.a.currentLevel : 0;
		hm.ClearOverlays();
		if (LevelManager.a.ressurectionActive[lindex])
			PlayerRessurect(); // Ressurection
		else
			PlayerDeathToMenu(); // Game Over
	}

	public void PlayerRessurect() {
		bool ressurected = LevelManager.a.RessurectPlayer();
		if (!ressurected) Debug.Log("ERROR: failed to ressurect player!");
		hm.health = 211f;
		MFDManager.a.DrawTicks(true);
		radiationArea = false;
		radiated = 0;
		playerDead = false;
		PlayerPatch.a.DisableAllPatches();
		PlayerMovement.a.fatigue = 0f;
	}

	public void PlayerDeathToMenu() {
		Const.a.loadingScreen.SetActive(true);

		// Death to Main Menu
		if (MouseLookScript.a.inventoryMode == false) {
			MouseLookScript.a.ToggleInventoryMode();
			PauseScript.a.ToggleAudioPause();
		}

		GameObject newGameIndicator = GameObject.Find("NewGameIndicator");
		GameObject loadGameIndicator = GameObject.Find("LoadGameIndicator");
		GameObject freshGame = GameObject.Find("GameNotYetStarted");
		if (newGameIndicator != null) Utils.SafeDestroy(newGameIndicator);
		if (loadGameIndicator != null) Utils.SafeDestroy(loadGameIndicator);
		if (freshGame != null) Utils.SafeDestroy(freshGame);
		PauseScript.a.mainMenu.SetActive(true);
		MainMenuHandler.a.returnToPause = false;
		MainMenuHandler.a.GoToFrontPage();
		hm.health = 211f;
		MFDManager.a.DrawTicks(true);
		radiationArea = false;
		radiated = 0;
		playerDead = false;
		PlayerPatch.a.DisableAllPatches();
		PlayerMovement.a.fatigue = 0f;
	}

	// Check for envirosuit and apply reduction based on version
	bool EnvirosuitApply() {
		radAdjust = 0f;
		if (!Inventory.a.hasHardware[8]) return false;
		if (PlayerEnergy.a.energy <= 0) return false;

		float enerTake = 0f;
		float frac = 0.12f;
		float energCost = 0.11f;
		switch (Inventory.a.hardwareVersion[8]) {
			case 1: frac = 0.17f; energCost = 0.25f; break;
			case 2: frac = 0.15f; energCost = 0.16f; break;
			case 3: frac = 0.12f; energCost = 0.11f; break;
		}

		radAdjust = radiated * frac;
		float diff = radiated - radAdjust;
		radiated = radAdjust; // After calculating difference.
		if (radiated < 0) radiated = 0;
		if (diff < 0) diff = 0; // Prevent underflow.
		enerTake = (energCost * diff);
		if (enerTake < 0) enerTake = 0;
		radAdjust = initialRadiation - radiated;
		if (radAdjust < 0) radAdjust = 0;
		Debug.Log("Taking energy for envirosuit: " + enerTake.ToString());

		// Suit absorbs some radiation, say it.
		// Envirosuit absorbed ##LBP, Radiation poisoning ##LBP
		PlayerMovement.a.twm.SendWarning((Const.a.stringTable[280]
											+ radAdjust.ToString()
											+ Const.a.stringTable[281]
											+ Const.a.stringTable[185]
											+ radiated.ToString()
											+ Const.a.stringTable[186]),
											0.1f,-2,HUDColor.Red,
											radiationAmountWarningID);

		PlayerEnergy.a.TakeEnergy(enerTake);
		if (BiomonitorGraphSystem.a != null) {
			BiomonitorGraphSystem.a.EnergyPulse(enerTake);
		}
		return true;
	}

	public void GiveRadiation(float rad) {
		if (playerDead) return;

		if (radiated < rad) radiated = rad;
		else return;

		EnvirosuitApply();
		initialRadiation = radiated;
	}

	public static string Save(GameObject go) {
		PlayerHealth ph = go.GetComponent<PlayerHealth>();
		if (ph == null) {
			Debug.Log("PlayerHealth missing on savetype of Player!  GameObject.name: " + go.name);
			return "0000.00000|0000.00000|0|0|0000.00000|-1|0|211.00000|0000.00000|0000.00000|0000.00000";
		}

		string line = System.String.Empty;
		line = Utils.FloatToString(ph.radiated); // float
		line += Utils.splitChar + Utils.FloatToString(ph.timer); // float, not relative timer
		line += Utils.splitChar + Utils.BoolToString(ph.playerDead); // bool
		line += Utils.splitChar + Utils.BoolToString(ph.radiationArea); // bool
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(ph.mediPatchPulseFinished); // float
		line += Utils.splitChar + ph.mediPatchPulseCount.ToString(); // int
		line += Utils.splitChar + Utils.BoolToString(ph.makingNoise); // bool
		line += Utils.splitChar + Utils.FloatToString(ph.lastHealth); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(ph.painSoundFinished); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(ph.radSoundFinished); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(ph.radFXFinished); // float
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		PlayerHealth ph = go.GetComponent<PlayerHealth>();
		if (ph == null) {
			Debug.Log("PlayerHealth.Load failure, ph == null");
			return index + 11;
		}

		if (index < 0) {
			Debug.Log("PlayerHealth.Load failure, index < 0");
			return index + 11;
		}

		if (entries == null) {
			Debug.Log("PlayerHealth.Load failure, entries == null");
			return index + 11;
		}

		ph.radiated = Utils.GetFloatFromString(entries[index]); index++;
		ph.timer = Utils.GetFloatFromString(entries[index]); index++; // Not relative time
		ph.playerDead = Utils.GetBoolFromString(entries[index]); index++;
		ph.radiationArea = Utils.GetBoolFromString(entries[index]); index++;
		ph.mediPatchPulseFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		ph.mediPatchPulseCount = Utils.GetIntFromString(entries[index] ); index++;
		ph.makingNoise = Utils.GetBoolFromString(entries[index]); index++;
		ph.lastHealth = Utils.GetFloatFromString(entries[index]); index++;
		ph.painSoundFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		ph.radSoundFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		ph.radFXFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		return index;
	}
}