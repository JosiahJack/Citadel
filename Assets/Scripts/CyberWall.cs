using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CyberWall : MonoBehaviour {
	private Material cyberwall;
	public MeshRenderer mr;
	List<GameObject> currentCollisions = new List<GameObject>();
	//private bool wasTouching;
	private float conwayFinished; // Visual only, Time.time controlled
	private float conwayTime = 0.5f;
	private float centerAlphaMinimum = 0.02f;
	private float centerAlphaMaximum = 1f;
	private float centerAlphaCurrent = 0.02f;
	public float tick = 0.05f;
	private float tickFinished; // Visual only, Time.time controlled

	void Start() {
		cyberwall = mr.material;
		centerAlphaCurrent = centerAlphaMinimum;
		cyberwall.SetFloat("_CenterAlpha",centerAlphaCurrent);
		tickFinished = Time.time + 2f;
		//Const.a.AddCyberPanelToRegistry(this);
		//wasTouching = false;
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			if (tickFinished < Time.time) {
				if (centerAlphaCurrent > centerAlphaMinimum) {
					centerAlphaCurrent -= 0.05f;
					if (centerAlphaCurrent < centerAlphaMinimum) centerAlphaCurrent = centerAlphaMinimum;
					cyberwall.SetFloat("_CenterAlpha",centerAlphaCurrent);
				}

				// if (currentCollisions.Any()) {
					// if (mr.material != cyberwallTouching) mr.material = cyberwallTouching;
					// wasTouching = true;
					// conwayFinished = Time.time + conwayTime; // keep resetting timer so it's fresh for if an object stops touching us
				// } else {
					//See if we were just touched and the conway timer is up so that touch material is active for conwayTime seconds
					// if (wasTouching && conwayFinished < Time.time) {
						// wasTouching = false; // reset bit so we don't spam Conway's Game of Life
						// Const.a.ConwayGameEntry(this,transform.position); // keep spreading life!
					// }
					// if (mr.material != cyberwall) mr.material = cyberwall;
				// }
				tickFinished = Time.time + tick;
			}

		}
	}

	// Input conway signal to keep spreading life!
	public void ConwaySignal() {
		conwayFinished = Time.time + conwayTime;
		//wasTouching = true;
	}

	void OnCollisionEnter (Collision other) {
		centerAlphaCurrent = centerAlphaMaximum;
		//currentCollisions.Add(other.gameObject);
	}

    // void OnCollisionExit (Collision other) {
		// currentCollisions.Remove(other.gameObject);
	// }
}
