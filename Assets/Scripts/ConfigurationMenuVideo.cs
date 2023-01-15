using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ConfigurationMenuVideo : MonoBehaviour {
	// Internal references
	[HideInInspector] public Resolution[] resolutions;
	private Dropdown resSelector;

	void Start () {
		RepopulateResolutionsDropdown();
	}

	void OnEnable() {
		RepopulateResolutionsDropdown();
	}

	public void RepopulateResolutionsDropdown() {
		resolutions = GetResolutions().ToArray();//Screen.resolutions.Where(resolutions => resolutions.refreshRate >= 60).ToArray();
		if (resSelector == null) resSelector = GetComponent<Dropdown>();
		if (resSelector == null) Debug.Log("BUG: ConfigurationMenuVideo missing component for resSelector.");
		List<string> resList = new List<string>();
		int refindex = 0;
		string paddingWidth = "";
		string paddingHeight = "";
		for (int i=0;i<resolutions.Length;i++) {
			if (resolutions[i].width < 1000) paddingWidth = " ";
			else paddingWidth = "";
			if (resolutions[i].height < 1000) paddingHeight = " ";
			else paddingHeight = "";
			resList.Add(paddingWidth + resolutions[i].width.ToString() + "x" + paddingHeight + resolutions[i].height.ToString() +  " " + resolutions[i].refreshRate.ToString() + "Hz");
			if (resolutions[i].width == Screen.width) refindex = i;
		}
		resSelector.ClearOptions();
		resSelector.AddOptions(resList);
		resSelector.value = refindex;
	}

	public static List<Resolution> GetResolutions() {
		//Filters out all resolutions with low refresh rate:
		Resolution[] resolutions = Screen.resolutions;
		int[] uniqueResolutionWidths = new int[50];
		int[] uniqueResolutionHeights = new int[50];
		int[] refreshRates = new int[50];
		int maxRefreshRate = 30;
		for (int i = 0; i < resolutions.Length; i++) {
			if (resolutions[i].refreshRate > maxRefreshRate) {
				maxRefreshRate = resolutions[i].refreshRate;
			}
		}

		List<Resolution> uniqueResolutionsList = new List<Resolution>();
		for (int i = 0; i < resolutions.Length; i++) {
			if (resolutions[i].refreshRate == maxRefreshRate) {
				uniqueResolutionsList.Add(resolutions[i]);
			}
		}

		return uniqueResolutionsList;
	}
}
