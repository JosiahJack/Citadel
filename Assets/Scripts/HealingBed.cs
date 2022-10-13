using UnityEngine;
using System.Collections;

public class HealingBed : MonoBehaviour {

	public float amount = 170;  //default to 2/3 of 255, the total player can have
	//public float resetTime = 150; //150 seconds
	//public bool requireReset;
	public int minSecurityLevel = 0;
	public bool broken = false;
	public AudioClip SFX;
	//private float nextthink;
	private float give;

	private AudioSource SFXSource;

	void Awake () {
		SFXSource = GetComponent<AudioSource>();
	}

	public void Use (UseData ud) {
		if (LevelManager.a.GetCurrentLevelSecurity() <= minSecurityLevel) {
			if (!broken) {
				PlayerReferenceManager.a.playerCapsule.GetComponent<HealthManager>().HealingBed(amount,true);
				Const.sprint(Const.a.stringTable[23],ud.owner);
				SFXSource.PlayOneShot(SFX);
			} else {
				Const.sprint(Const.a.stringTable[24],ud.owner);
			}
		}
	}
}
