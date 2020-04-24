using UnityEngine;
#if UNITY_EDITOR 
	using UnityEditor;
#endif
using System.Collections;

public class PlayerHealth : MonoBehaviour {
	//public float health = 211f; //max is 255
	public float radiated = 0f;
	public float resetAfterDeathTime = 0.5f;
	public float timer;
	public static bool playerDead = false;
	public bool mediPatchActive = false;
	public float mediPatchPulseTime = 1f;
	public float mediPatchHealAmount = 10f;
	public bool detoxPatchActive = false;
	public AudioSource PlayerNoise;
	public AudioClip PainSFXClip;
	public AudioClip RadiationClip;
	public AudioClip ShieldClip;
	public GameObject cameraObject;
	public GameObject hardwareShield;
	public GameObject radiationEffect;
	public GameObject shieldEffect;
	public bool radiationArea = false;
	private float radiationBleedOffFinished = 0f;
	public float radiationBleedOffTime = 1f;
	public float radiationReductionAmount = 1f;
	public float radiationHealthDamageRatio = 0.2f;
	public GameObject mainPlayerParent;
	public int radiationAmountWarningID = 323;
	public int radiationAreaWarningID = 322;
	public float mediPatchPulseFinished = 0f;
	public int mediPatchPulseCount = 0;
	public bool makingNoise = false;
	[HideInInspector]
	public HealthManager hm;

	private float lastHealth;
	private float painSoundFinished;
	private float radSoundFinished;
	private float radFXFinished;
	private TextWarningsManager twm;
	private PlayerEnergy pe;
	private float radAdjust;
	private float initialRadiation;
	[HideInInspector]
	public float noiseFinished;

	void Awake () {
		twm = mainPlayerParent.GetComponent<PlayerReferenceManager>().playerTextWarningManager.GetComponent<TextWarningsManager>();
		if (twm == null) Debug.Log("BUG: No TextWarningManager script found on player (sent from PlayerHealth.Awake)");
		hm = GetComponent<HealthManager>();
		if (hm == null) Debug.Log("BUG: No HealthManager script found on player (sent from PlayerHealth.Awake)");
		pe = GetComponent<PlayerEnergy>();
		if (pe == null) Debug.Log("BUG: No PlayerEnergy script found on player (sent from PlayerHealth.Awake)");
		painSoundFinished = Time.time;
		radSoundFinished = Time.time;
		radFXFinished = Time.time;
		noiseFinished = Time.time;
		lastHealth = hm.health;
		radAdjust = 0f;
		initialRadiation = 0f;
	}

	void Update (){
		//if (PlayerNoise.isPlaying)
			//makingNoise = true;
		//else
			//makingNoise = false;
		if (noiseFinished < Time.time) makingNoise = false;

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
			if (mediPatchPulseFinished < Time.time) {
				float timePulse = mediPatchPulseTime;
				hm.HealingBed(mediPatchHealAmount,false);
				//hm.health += mediPatchHealAmount;
				timePulse += (mediPatchPulseCount*0.5f);
				mediPatchPulseFinished = Time.time + timePulse;
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
			if (radFXFinished < Time.time) {
				radiationEffect.SetActive(true);
				float minT = 0.5f;
				if (radiated > 50) minT = 0.25f;
				radFXFinished = Time.time + Random.Range(minT,1f);
			}
		} else {
			radiationArea = false;
			radiated = 0;
		}

		if (radiationBleedOffFinished < Time.time) {
			if (!radiationArea) radiated -= radiationReductionAmount;  // bleed off the radiation over time
			if (radiated < 0) radiated = 0;
			radiationBleedOffFinished = Time.time + radiationBleedOffTime;

			if (radiated > 0) {
				if (!hm.god) hm.health -= radiated*radiationHealthDamageRatio*radiationBleedOffTime; // apply health at rate of bleedoff time
				if (radSoundFinished < Time.time) {
					radSoundFinished = Time.time + Random.Range(1f,3f);
					PlayerNoise.PlayOneShot(RadiationClip);
				}
			}
		}

		// Did we lose health?
		if (lastHealth > hm.health) {
			if (painSoundFinished < Time.time && !(radSoundFinished < Time.time)) {
				painSoundFinished = Time.time + Random.Range(0.25f,3f); // Don't spam pain sounds
				PlayerNoise.PlayOneShot(PainSFXClip);
			}
		}
		lastHealth = hm.health;
	}
	
	void PlayerDying (){
		timer += Time.deltaTime;
		makingNoise = false;
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
			bool ressurected = LevelManager.a.RessurectPlayer(mainPlayerParent);
			if (!ressurected) Debug.Log("ERROR: failed to ressurect player!");
			hm.health = 211f;
			playerDead = false;
		} else {
			// Death to Main Menu
			if (mls.inventoryMode == false) {
				mls.ToggleInventoryMode();
				mls.ToggleAudioPause();
			}
			mls.mainMenu.SetActive(true);
			/*
			#if UNITY_EDITOR
			if (Application.isEditor) {
				EditorApplication.isPlaying = false;
				return;
			}
			#endif
			*/
		}
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