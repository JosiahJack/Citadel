using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigInputLabels : MonoBehaviour {
	public Text[] labels;

	void Start () {
		for(int i=0;i<labels.Length;i++) {
			labels[i].text = Const.a.InputCodes[i];
		}
	}
}
