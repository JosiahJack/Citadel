using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {
	public float health = 211f; //max is 255
	public float resetAfterDeathTime = 0.5f;
	public float timer;
	public static bool playerDead = false;
	public bool mediPatchActive = false;
	public bool detoxPatchActive = false;
	public AudioSource PainSFX;
	public AudioClip PainSFXClip;
	public GameObject cameraObject;
	public GameObject hardwareShield;
	private bool shieldOn = false;
	
	void Update (){
		if (health <= 0f) {
			if (!playerDead) {
				PlayerDying();
			} else {
				PlayerDead();
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
	
	public void TakeDamage ( float take  ){
		float shieldBlock = 0f;
		if (shieldOn) {
			//shieldBlock = hardwareShield.GetComponent<Shield>().GetShieldBlock();
		}
		take = Const.a.GetDamageTakeAmount(take,0,shieldBlock,Const.AttackType.None,false,0,0);
		health -= take;
		PainSFX.PlayOneShot(PainSFXClip);
		//print("Player Health: " + health.ToString());
	}
}