using UnityEngine;
using System.Collections;

public class HealingBed : MonoBehaviour {

	public float amount = 170;  //default to 2/3 of 255, the total energy player can have
	//public float resetTime = 150; //150 seconds
	//public bool requireReset;
	//public float minSecurityLevel = 0;
	public bool broken = false;
	public AudioClip SFX;
	//private float nextthink;
	private float give;
	public GameObject healingFXFlash;
	private AudioSource SFXSource;
	
	void Awake () {
		SFXSource = GetComponent<AudioSource>();
	}

	void Use (GameObject owner) {
		//if (security<minSecurityLevel) {
			if (!broken) {
				give = owner.transform.parent.GetComponent<PlayerHealth>().health + amount;
				if (give > 255f)
					give = 255f;
				owner.transform.parent.GetComponent<PlayerHealth>().health = give;
				Const.sprint("Automatic healing process activated.");
				if (healingFXFlash != null) {
					healingFXFlash.SetActive(true);
				}
				SFXSource.PlayOneShot(SFX);
			} else {
				print("Healing bed is broken beyond repair\n");
			}
		//}
	}
}
