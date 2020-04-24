using UnityEngine;
using System.Collections;

public class TouchEnergyDrain : MonoBehaviour {
	public float drainage = 1; // assign in the editor
	public float tick = 0.1f;
	private float tickFinished;

	void Awake() {
		tickFinished = Time.time + UnityEngine.Random.Range(1f,2f);
	}

	void  OnCollisionEnter (Collision col) {
		if (tickFinished < Time.time) {
			if (col.gameObject.tag == "Player") {
				PlayerEnergy pe = col.gameObject.GetComponent<PlayerEnergy>();
				if (pe != null) pe.TakeEnergy(drainage);
			}
			tickFinished = Time.time + tick;
		}
	}

}