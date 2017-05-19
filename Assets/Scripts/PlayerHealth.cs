using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {
	public float health = 211f; //max is 255
	public float radiated = 0f;
	public float resetAfterDeathTime = 0.5f;
	public float timer;
	public static bool playerDead = false;
	public bool mediPatchActive = false;
	public float mediPatchPulseTime = 1f;
	public float mediPatchHealAmount = 10f;
	public bool detoxPatchActive = false;
	public AudioSource PainSFX;
	public AudioClip PainSFXClip;
	public GameObject cameraObject;
	public GameObject hardwareShield;
	private bool shieldOn = false;
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
	
	void Update (){
		if (health <= 0f) {
			if (!playerDead) {
				PlayerDying();
			} else {
				PlayerDead();
			}
		}

		if (mediPatchActive) {
			if (mediPatchPulseFinished == 0) mediPatchPulseCount = 0;
			if (mediPatchPulseFinished < Time.time) {
				float timePulse = mediPatchPulseTime;
				health += mediPatchHealAmount;
				if (health > 255f) health = 255f;
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
			TextWarningsManager twm = mainPlayerParent.GetComponent<PlayerReferenceManager>().playerTextWarningManager.GetComponent<TextWarningsManager>();
			if (radiationArea) twm.SendWarning(("Radiation Area"),0.1f,-2,TextWarningsManager.warningTextColor.white,radiationAreaWarningID);
			twm.SendWarning(("Radiation poisoning "+radiated.ToString()+" LBP"),0.1f,-2,TextWarningsManager.warningTextColor.red,radiationAmountWarningID);
		}

		if (radiated < 1) {
			radiationArea = false;
		}

		if (radiationBleedOffFinished < Time.time) {
			if (radiated > 0) {
				health -= radiated*radiationHealthDamageRatio*radiationBleedOffTime; // apply health at rate of bleedoff time
				if (!radiationArea) radiated -= radiationReductionAmount;  // bleed off the radiation over time
				radiationBleedOffFinished = Time.time + radiationBleedOffTime;
			}
		}
	}
	
	void PlayerDying (){
		timer += Time.deltaTime;
		
		if (timer >= resetAfterDeathTime) {
			health = 0f;
			playerDead = true;
		}
	}
	
	void PlayerDead (){
		//gameObject.GetComponent<PlayerMovement>().enabled = false;
		//cameraObject.SetActive(false);
		cameraObject.GetComponent<Camera>().enabled = false;
		Cursor.lockState = CursorLockMode.None;
	}
	
	public void TakeDamage (DamageData dd){
		float shieldBlock = 0f;
		if (shieldOn) {
			//shieldBlock = hardwareShield.GetComponent<Shield>().GetShieldBlock();
		}
		dd.armorvalue = shieldBlock;
		dd.defense = 0f;
		float take = Const.a.GetDamageTakeAmount(dd);
		health -= take;
		PainSFX.PlayOneShot(PainSFXClip);
		//Debug.Log("Player Health: " + health.ToString());
	}

	public void GiveRadiation (float rad) {
		if (radiated < rad)
			radiated = rad;

		//radiated -= suitReduction;
	}
}