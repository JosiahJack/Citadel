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

	void Awake () {
		twm = mainPlayerParent.GetComponent<PlayerReferenceManager>().playerTextWarningManager.GetComponent<TextWarningsManager>();
		hm = GetComponent<HealthManager>();
		if (hm == null) Debug.Log("BUG: No HealthManager script found on player!!");
		painSoundFinished = Time.time;
		radSoundFinished = Time.time;
		radFXFinished = Time.time;
		lastHealth = hm.health;
	}

	void Update (){
		if (PlayerNoise.isPlaying)
			makingNoise = true;
		else
			makingNoise = false;

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

		if (radiated > 0) {
			if (radiationArea) twm.SendWarning(("Radiation Area"),0.1f,-2,TextWarningsManager.warningTextColor.white,radiationAreaWarningID);
			twm.SendWarning(("Radiation poisoning "+radiated.ToString()+" LBP"),0.1f,-2,TextWarningsManager.warningTextColor.red,radiationAmountWarningID);
			if (radFXFinished < Time.time) {
				radiationEffect.SetActive(true);
				radFXFinished = Time.time + Random.Range(0.4f,1f);
			}
		}

		if (radiated < 1) {
			radiationArea = false;
		}

		if (radiationBleedOffFinished < Time.time) {
			if (radiated > 0) {
				hm.health -= radiated*radiationHealthDamageRatio*radiationBleedOffTime; // apply health at rate of bleedoff time
				if (!radiationArea) {
					radiated -= radiationReductionAmount;  // bleed off the radiation over time
				} else {
					if (radSoundFinished < Time.time) {
						radSoundFinished = Time.time + Random.Range(0.5f,1.5f);
						PlayerNoise.PlayOneShot(RadiationClip);
					}
				}
				radiationBleedOffFinished = Time.time + radiationBleedOffTime;
			}
		}

		// Did we lose health?
		if (lastHealth > hm.health) {
			if (painSoundFinished < Time.time && !(radSoundFinished < Time.time)) {
				painSoundFinished = Time.time + Random.Range(0.5f,3f); // Don't spam pain sounds
				PlayerNoise.PlayOneShot(PainSFXClip);
			}
		}
		lastHealth = hm.health;
	}
	
	void PlayerDying (){
		timer += Time.deltaTime;
		
		if (timer >= resetAfterDeathTime) {
			hm.health = 0f;
			playerDead = true;
		}
	}
	
	void PlayerDead (){
		MouseLookScript mls = cameraObject.GetComponent<MouseLookScript>();
		mls.DropHeldItem();
		mls.ResetHeldItem ();
		mls.ResetCursor ();
		Cursor.lockState = CursorLockMode.None;
		if (LevelManager.a.ressurectionActive[LevelManager.a.currentLevel]) {
			// Ressurection
			bool ressurected = LevelManager.a.RessurectPlayer(mainPlayerParent);
			if (!ressurected) Debug.Log("ERROR: failed to ressurect player!");
		} else {
			// Death to Main Menu
			//gameObject.GetComponent<PlayerMovement>().enabled = false;
			//cameraObject.SetActive(false);
			//cameraObject.GetComponent<Camera>().enabled = false;
			//PauseScript.a.PauseEnable();
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

		//radiated -= suitReduction;
	}
}