using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MFDManager : MonoBehaviour  {
	public TabButtonsScript leftTC;
	public TabButtonsScript rightTC;
	public ItemTabManager itemTabLH;
	public ItemTabManager itemTabRH;
	public DataTab dataTabLH;
	public DataTab dataTabRH;
	public bool lastWeaponSideRH;
	public bool lastItemSideRH;
	public bool lastAutomapSideRH;
	public bool lastTargetSideRH;
	public bool lastDataSideRH;
	public bool lastLogSideRH;
	public bool lastLogSecondarySideRH;
	public bool lastMinigameSideRH;
	public enum TabMSG {None,Search,AudioLog};

	public void OpenTab(int index, bool overrideToggling,TabMSG type,int intdata1) {
		bool isRH = false;
		switch (index) {
			case 0: isRH = lastWeaponSideRH; break;
			case 1: isRH = lastItemSideRH; break;
			case 2: isRH = lastAutomapSideRH; break;
			case 3: isRH = lastTargetSideRH; break;
			case 4: isRH = lastDataSideRH; break;
		}
		if(isRH) {
			// RH MFD
			leftTC.TabButtonClickSilent(index,overrideToggling);
			if (type == TabMSG.AudioLog) {
				dataTabLH.Reset();
				dataTabLH.audioLogContainer.SetActive(true);
				dataTabLH.audioLogContainer.GetComponent<LogDataTabContainerManager>().SendLogData(intdata1);
			}
		} else {
			// LH MFD
			rightTC.TabButtonClickSilent(index,overrideToggling);
			if (type == TabMSG.AudioLog) {
				dataTabRH.Reset();
				dataTabRH.audioLogContainer.SetActive(true);
				dataTabRH.audioLogContainer.GetComponent<LogDataTabContainerManager>().SendLogData(intdata1);
			}
		}
	}
}