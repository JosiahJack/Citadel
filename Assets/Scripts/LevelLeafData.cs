using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLeafData : MonoBehaviour {
	public int leafIndex;
	public string leafname;
	public bool[] visibleLeaves;
	[HideInInspector]
	public BoxCollider leafBox;
	public GameObject[] lightGOs;
	public MeshRenderer[] meshes;

	void Awake() {
		leafBox = GetComponent<BoxCollider>();
		if (leafBox == null) Debug.Log("BUG: Missing box collider on LevelLeafData for leaf " + leafname);
	}

	public void HideMeshRenderers() {
		for (int i=0;i<meshes.Length;i++) {
			if (meshes[i] != null) {
				if (meshes[i].enabled == true) meshes[i].enabled = false;
			}
		}
	}

	public void ShowMeshRenderers() {
		for (int i=0;i<meshes.Length;i++) {
			if (meshes[i] != null) {
				if (meshes[i].enabled == false) meshes[i].enabled = true;
			}
		}
	}

	public void HideLights() {
		for (int i=0;i<lightGOs.Length;i++) {
			if (lightGOs[i] != null) {
				if (lightGOs[i].activeInHierarchy) lightGOs[i].SetActive(false);
			}
		}
	}

	public void ShowLights() {
		for (int i=0;i<lightGOs.Length;i++) {
			if (lightGOs[i] != null) {
				if (!lightGOs[i].activeInHierarchy) lightGOs[i].SetActive(true);
			}
		}
	}
}
