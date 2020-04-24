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
		copy.transform.parent = transform;
	}
}
