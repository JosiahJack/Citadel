using UnityEngine;
using System.Collections;

public class HealingBed : MonoBehaviour {

	public float amount = 170;  //default to 2/3 of 255, the total energy player can have
	//public float resetTime = 150; //150 seconds
	//public bool requireReset;
	public int minSecurityLevel = 0;
	public bool broken = false;
	public AudioClip SFX;
	//private float nextthink;
	private float give;
	public GameObject healingFXFlash;
	private AudioSource SFXSource;

	void Awake () {
		SFXSource = GetComponent<AudioSource>();
	}

	void Use (UseData ud) {
		if (LevelManager.a.GetCurrentLevelSecurity() < minSecurityLevel) {
			if (!broken) {
				give = ud.owner.GetComponent<PlayerReferenceManager>().playerCapsule.GetComponent<PlayerHealth>().hm.health + amount;
				if (give > 255f)
					give = 255f;
				ud.owner.GetComponent<PlayerReferenceManager>().playerCapsule.GetComponent<PlayerHealth>().hm.health = give;
				Const.sprint("Automatic healing process activated.",ud.owner);
				if (healingFXFlash != null) {
					healingFXFlash.SetActive(true);
				}
				SFXSource.PlayOneShot(SFX);
			} else {
				Const.sprint("Healing bed is broken beyond repair",ud.owner);
			}
		}
	}
}
