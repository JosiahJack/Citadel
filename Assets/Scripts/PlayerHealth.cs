using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {
	public float health = 255f;
	public float resetAfterDeathTime = 1f;
	public float timer;
	public static bool  playerDead;
	public AudioSource PainSFX;
	public AudioClip PainSFXClip;
	public GameObject cameraObject;
	
	void Update (){
		if (health <= 0) {
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
		health -= take;
		PainSFX.PlayOneShot(PainSFXClip);
		print("Player Health: " + health.ToString());
	}
}