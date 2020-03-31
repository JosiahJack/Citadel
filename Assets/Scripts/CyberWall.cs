using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CyberWall : MonoBehaviour {
	public Material cyberwallTouching;
	public Material cyberwall;
	public MeshRenderer mr;
	public MeshRenderer mrbackface;
	List<GameObject> currentCollisions = new List<GameObject>();
	private bool wasTouching;
	private float conwayFinished;
	private float conwayTime = 0.5f;

	void Start() {
		Const.a.AddCyberPanelToRegistry(this);
		wasTouching = false;
	}

	void Update() {
		if (currentCollisions.Any()) {
			if (mr.material != cyberwallTouching) mr.material = cyberwallTouching;
			if (mrbackface.material != cyberwallTouching) mrbackface.material = cyberwallTouching;
			wasTouching = true;
			conwayFinished = Time.time + conwayTime; // keep resetting timer so it's fresh for if an object stops touching us
		} else {
			// See if we were just touched and the conway timer is up so that touch material is active for conwayTime seconds
			if (wasTouching && conwayFinished < Time.time) {
				wasTouching = false; // reset bit so we don't spam Conway's Game of Life
				Const.a.ConwayGameEntry(this,transform.position); // keep spreading life!
			}
			if (mr.material != cyberwall) mr.material = cyberwall;
			if (mrbackface.material != cyberwall) mrbackface.material = cyberwall;
		}
	}

	// Input conway signal to keep spreading life!
	public void ConwaySignal() {
		conwayFinished = Time.time + conwayTime;
		wasTouching = true;
	}

	void OnCollisionEnter (Collision other) {
		currentCollisions.Add(other.gameObject);
	}

    void OnCollisionExit (Collision other) {
		currentCollisions.Remove(other.gameObject);
	}
}
