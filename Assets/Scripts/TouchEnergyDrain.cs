using UnityEngine;
using System.Collections;

public class TouchEnergyDrain : MonoBehaviour {
	public float drainage = 1; // assign in the editor

	void  OnCollisionEnter (Collision col) {
		if (col.gameObject.tag == "Player") {
			col.gameObject.GetComponent<PlayerEnergy>().TakeEnergy(drainage);
		}
	}

}