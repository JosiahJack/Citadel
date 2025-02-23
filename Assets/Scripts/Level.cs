using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Execute prior to LevelManager
public class Level : MonoBehaviour {
	public GameObject geometryContainer;
	public GameObject dynamicObjectsContainer;
	public GameObject lightsStaticSaveable;
	public GameObject lightsStaticImmutable;
	public GameObject doorsStaticSaveable;
	public GameObject staticObjectsSaveable;
	public GameObject staticObjectsImmutable;
	public GameObject NPCsSaveableInstantiated;

// 	public void Awake() {
// 		geometryContainer = transform.GetChild(0).gameObject;
// 		dynamicObjectsContainer = transform.GetChild(1).gameObject;
// 		lightsStaticSaveable = transform.GetChild(2).gameObject;
// 		lightsStaticImmutable = transform.GetChild(3).gameObject;
// 		doorsStaticSaveable = transform.GetChild(4).gameObject;
// 		staticObjectsSaveable = transform.GetChild(5).gameObject;
// 		staticObjectsImmutable = transform.GetChild(6).gameObject;
// 		NPCsSaveableInstantiated = transform.GetChild(7).gameObject;
// 	}
}
