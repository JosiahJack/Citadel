using UnityEngine;
using System.Collections;

public class PlayerEnergy : MonoBehaviour {
	public float energy = 54f; //max is 255
	public float resetAfterDeathTime = 0.5f;
	public float timer;

	public void TakeEnergy ( float take  ){
		energy -= take;
		if (energy <= 0f)
			energy = 0f;
		//print("Player Energy: " + energy.ToString());
	}
}