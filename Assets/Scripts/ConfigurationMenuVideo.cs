using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationMenuVideo : MonoBehaviour {
	public Resolution[] resolutions;
	private Dropdown resSelector;

	void Awake () {
		//List<int> widths = new List<int>();
		//List<int> heights = new List<int>();
		resolutions = Screen.resolutions;
		resSelector = GetComponent<Dropdown>();
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
