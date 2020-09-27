using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolPopulateByCopy : MonoBehaviour {
	public int numberOfCopies = 3;

	void Awake() {
		int i =0;
		while(i<numberOfCopies) {
			CreateCopy();
			i++;
		}
	}

    public void CreateCopy() {
		GameObject copy = Instantiate(transform.GetChild(0).gameObject,transform.position,Quaternion.identity) as GameObject; // create a copy of a pool object
		if (copy != null) {
			if (copy.GetComponent<RectTransform>() != null) {
				copy.GetComponent<RectTransform>().SetParent(transform,true);
				copy.GetComponent<RectTransform>().localScale = Vector3.one;
				copy.GetComponent<RectTransform>().localRotation = Quaternion.identity;
			} else {
				copy.transform.parent = transform;
			}
		}
	}
}
