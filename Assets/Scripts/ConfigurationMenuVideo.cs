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
		for (int i=0;i<resolutions.Length;i++) {
			//widths.Add(resolutions[i].width);
			resList.Add(resolutions[i].width.ToString() + "x" + resolutions[i].height);
		}
		resSelector.ClearOptions();
		resSelector.AddOptions(resList);
	}
}
