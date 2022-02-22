using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigInputLabels : MonoBehaviour {
	// Externally references, required
	public Text[] labels;

	void Start () {
		if (labels.Length < 40) Debug.Log("BUG: ConfigInputLabels has less than 40 assigned values.");
		for(int i=0;i<labels.Length;i++) {
			labels[i].text = Const.a.InputCodes[i];
		}
	}
}
