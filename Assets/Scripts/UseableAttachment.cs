using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseableAttachment : MonoBehaviour {
	public GameObject[] destructables; // used by plastique
	public GameObject explosion; // used by plastique
	public GameObject simpleActivationIndicator; // object to enable when activated
	public GameObject targettedObject;
	public float timerFinished;
	[Tooltip("Time till explode, minimum 1 second (= 1.0)")]
	public float timeTillPlastiqueExplode; // minimum of 1f
	public bool activated = false;

	void Awake () {
		activated = false;
		timerFinished = -1f;
		if (timeTillPlastiqueExplode < 1f) {
			timeTillPlastiqueExplode = 1f;
		}
	}

	// Use this to attach an item such as isotope-x22, isolinear chipset, interface demodulator, or a plastique
	public void Use (UseData ud) {
		if (ud.mainIndex != 56 || ud.mainIndex != 57 || ud.mainIndex != 61 || ud.mainIndex != 64) return;

		switch (ud.mainIndex) {
		case 56:
				// Z-44 Plastique Explosive
				activated = true;
				targettedObject.SendMessageUpwards("Use", ud);
				timerFinished = Time.time + timeTillPlastiqueExplode;
				break;
			case 57:
				// Interface Demodulator
				activated = true;
				targettedObject.SendMessageUpwards("Use", ud);
				simpleActivationIndicator.SetActive(true);
				break;
			case 61:
				// Isotope-X22
				activated = true;
				targettedObject.SendMessageUpwards("Use", ud);
				simpleActivationIndicator.SetActive(true);
				break;
			case 64:
				// Isolinear Chipset
				activated = true;
				targettedObject.SendMessageUpwards("Use", ud);
				simpleActivationIndicator.SetActive(true);
				break;
		}
	}

	void Update () {
		if (activated) {

			// Plastique delayed effect (to give player time to escape of course)
			if (timerFinished != -1f) {
				if (timerFinished < Time.time) {
					explosion.SetActive (true);
					for (int i = 0; i < destructables.Length; i++) {
						destructables [i].SetActive (false); // blow up the walls and floor
					}
					timerFinished = -1f;
					this.gameObject.SetActive (false); // blow up the panel as well
				}
			}
		}
	}
}
