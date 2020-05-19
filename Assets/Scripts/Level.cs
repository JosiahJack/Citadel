using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {
	public GameObject dynamicObjectsContainer;
	public LevelLeafData[] leaves;

	//private float tick = 0.1f;
	private float tickFinished;
	
	void Start() {
		tickFinished = PauseScript.a.relativeTime + UnityEngine.Random.Range(0.5f, 2f);
	}

	//void pdate() {
	//	if (tickFinished < PauseScript.a.relativeTime) {
	//		if (LevelManager.a.currentLeaf < 0) { UnCullAll(); return; }

	//		DisplaySeenLeafsFrom(LevelManager.a.currentLeaf);
	//		tickFinished = PauseScript.a.relativeTime + tick;
	//	}
	//}

	void UnCullAll() {
		for (int i=0;i<leaves.Length;i++) {
			leaves[i].ShowMeshRenderers();
			leaves[i].ShowLights();
		}
	}

	void DisplaySeenLeafsFrom(int c) {
		Debug.Log("Inside leaf: " + leaves[c].leafname);
		for (int i=0;i<leaves.Length;i++) {
			if (i == c || leaves[c].visibleLeaves[i]) {
				Debug.Log("Saw leaf: " + leaves[i].leafname);
				leaves[i].ShowMeshRenderers();
				leaves[i].ShowLights();
			} else {
				leaves[i].HideMeshRenderers();
				leaves[i].HideLights();
			}
		}
	}
}
