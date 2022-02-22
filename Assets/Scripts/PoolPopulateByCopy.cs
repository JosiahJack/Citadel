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
		GameObject copy = Instantiate(transform.GetChild(0).gameObject,transform.position,Const.a.quaternionIdentity) as GameObject; // create a copy of a pool object
		if (copy != null) {
			copy.SetActive(false); // Ensure it is in fact, "empty" and available to return.
			if (copy.GetComponent<RectTransform>() != null) {
				copy.GetComponent<RectTransform>().SetParent(transform,true);
				copy.GetComponent<RectTransform>().localScale = Const.a.vectorOne;
				copy.GetComponent<RectTransform>().localRotation = Const.a.quaternionIdentity;
			} else {
				copy.transform.parent = transform;
			}
		}
	}
}
