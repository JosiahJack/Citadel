using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MultiMediaTabManager : MonoBehaviour {
	public GameObject startingSubTab;
	public GameObject secondaryTab1;
	public GameObject secondaryTab2;
	public GameObject emailTab;
	public GameObject dataTab;
	public GameObject notesTab;
	public GameObject headerLabel;
	public EReaderSectionsButtons ersbLH;
	public EReaderSectionsButtons ersbRH;
	[HideInInspector]
	public int lastOpened = 1; // default to logs.  0 = email table, 1 = log table, 2 = data table

	public void OpenLastTab() {
		//Debug.Log("Last opened tab index: " + lastOpened.ToString());
		switch (lastOpened) {
			case 0: OpenEmailTableContents(); break;
			case 1: OpenLogTableContents(); break;
			case 2: OpenDataTableContents(); break;
			case 3: OpenNotesTableContents(); break;
		}
	}

	public void ResetTabs() {
		startingSubTab.SetActive(false);
		secondaryTab1.SetActive(false);
		secondaryTab2.SetActive(false);
		emailTab.SetActive(false);
		ersbLH.SetEReaderSectionsButtonsHighlights(lastOpened);
		ersbRH.SetEReaderSectionsButtonsHighlights(lastOpened);
		dataTab.SetActive(false);
		headerLabel.GetComponent<Text>().text = System.String.Empty;
	}

	public void OpenLogTableContents() {
		ResetTabs();
		startingSubTab.SetActive(true);
		headerLabel.GetComponent<Text>().text = "LOGS";
		lastOpened = 1;
		ersbLH.SetEReaderSectionsButtonsHighlights(1);
		ersbRH.SetEReaderSectionsButtonsHighlights(1);
	}

	public void OpenLogsLevelFolder(int curlevel) {
		ResetTabs();
		secondaryTab1.SetActive(true);
		headerLabel.GetComponent<Text>().text = "Level " + curlevel.ToString() + " Logs";
		secondaryTab1.GetComponent<LogContentsButtonsManager>().currentLevelFolder = curlevel;
		secondaryTab1.GetComponent<LogContentsButtonsManager>().InitializeLogsFromLevelIntoFolder();
	}

	public void OpenLogTextReader() {
		ResetTabs();
		secondaryTab2.SetActive(true);
	}

	public void OpenEmailTableContents() {
		ResetTabs();
		emailTab.SetActive(true);
		headerLabel.GetComponent<Text>().text = "EMAIL";
		lastOpened = 0;
		ersbLH.SetEReaderSectionsButtonsHighlights(0);
		ersbRH.SetEReaderSectionsButtonsHighlights(0);
	}

	public void OpenDataTableContents() {
		ResetTabs();
		dataTab.SetActive(true);
		headerLabel.GetComponent<Text>().text = "DATA";
		lastOpened = 2;
		ersbLH.SetEReaderSectionsButtonsHighlights(2);
		ersbRH.SetEReaderSectionsButtonsHighlights(2);
	}

	public void OpenNotesTableContents() {
		ResetTabs();
		notesTab.SetActive(true);
		headerLabel.GetComponent<Text>().text = "NOTES";
		lastOpened = 3;
		ersbLH.SetEReaderSectionsButtonsHighlights(3);
		ersbRH.SetEReaderSectionsButtonsHighlights(3);
	}
}
