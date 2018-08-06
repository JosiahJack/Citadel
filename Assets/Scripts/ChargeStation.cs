using UnityEngine;
using System.Collections;

public class ChargeStation : MonoBehaviour {

	public float amount = 170;  //default to 2/3 of 255, the total energy player can have
	public float resetTime = 150; //150 seconds
	public bool requireReset;
	public float minSecurityLevel = 0;
	public AudioClip SFX;
	private float nextthink;
	private float give;
	private AudioSource SFXSource;
	
	void Awake () {
		SFXSource = GetComponent<AudioSource>();
		nextthink = Time.time;
	}

	void Use (GameObject owner) {
		if (LevelManager.a.GetCurrentLevelSecurity () > minSecurityLevel) {
			MFDManager.a.BlockedBySecurity ();
			return;
		}
		
		if (nextthink < Time.time) {
			give = owner.GetComponent<PlayerReferenceManager>().playerCapsule.GetComponent<PlayerEnergy>().energy + amount;
			if (give > 255f)
				give = 255f;
			owner.GetComponent<PlayerReferenceManager>().playerCapsule.GetComponent<PlayerEnergy>().energy = give;
			Const.sprint("Energy drawn from Power Station.", owner);
			SFXSource.PlayOneShot(SFX);
			if (requireReset) {
				nextthink = Time.time + resetTime;
			}
		} else {
			Const.sprint("Power Station is recharging\n", owner);
		}
	}
}
