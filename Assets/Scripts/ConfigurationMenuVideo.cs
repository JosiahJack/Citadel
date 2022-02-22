using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationMenuVideo : MonoBehaviour {
	// Internal references
	[HideInInspector] public Resolution[] resolutions;
	private Dropdown resSelector;

	void Awake () {
		resolutions = Screen.resolutions;
		resSelector = GetComponent<Dropdown>();
		if (resSelector == null) Debug.Log("BUG: ConfigurationMenuVideo missing component for resSelector.");
		List<string> resList = new List<string>();
		int refindex = 0;
		for (int i=0;i<resolutions.Length;i++) {
			resList.Add(resolutions[i].width.ToString() + "x" + resolutions[i].height);
			if (resolutions[i].width == 1366) refindex = i;
		}
		resSelector.ClearOptions();
		resSelector.AddOptions(resList);
		resSelector.value = refindex;
	}
}
