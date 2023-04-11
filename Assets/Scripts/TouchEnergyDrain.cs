using UnityEngine;
using System.Collections;

public class TouchEnergyDrain : MonoBehaviour {
	public float drainage = 1; // assign in the editor
	public float tick = 0.1f;
	private float tickFinished;

	void Awake() {
		tickFinished = PauseScript.a.relativeTime + UnityEngine.Random.Range(1f,2f);
	}

	void  OnCollisionEnter (Collision col) {
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;

		if (tickFinished < PauseScript.a.relativeTime) {
			if (col.gameObject.CompareTag("Player")) {
				PlayerEnergy pe = col.gameObject.GetComponent<PlayerEnergy>();
				if (pe != null) {
					pe.TakeEnergy(drainage);
					if (BiomonitorGraphSystem.a != null) {
						BiomonitorGraphSystem.a.EnergyPulse(drainage);
					}
				}
			}
			tickFinished = PauseScript.a.relativeTime + tick;
		}
	}

}