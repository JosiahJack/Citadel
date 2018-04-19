using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsManager : MonoBehaviour {
	private GameObject[] lightChildrenGOs;
	private Light[] lightChildren;
	private int childCount;
	public float distanceToDisableLightsFromPlayer = 10f;

	void Start () {
		childCount = transform.childCount;
		lightChildren = new Light[childCount];
		lightChildrenGOs = new GameObject[childCount];
		for (int i=0;i<childCount;i++) {
			lightChildrenGOs[i] = transform.GetChild(i).gameObject;
			lightChildren[i] = lightChildrenGOs[i].GetComponent<Light>();
		}
	}

	void Update () {
		for (int i=0;i<lightChildren.Length;i++) {
			if (lightChildrenGOs[i] != null) {
				if ((Vector3.Distance(Const.a.player1.transform.position,lightChildrenGOs[i].transform.position)) > distanceToDisableLightsFromPlayer) {
					lightChildrenGOs[i].SetActive(false);
				} else {
					lightChildrenGOs[i].SetActive(true);
				}
			}
		}
	}
}
