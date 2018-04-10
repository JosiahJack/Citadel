using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MFDManager : MonoBehaviour  {
	public TabButtons leftTC;
	public TabButtons rightTC;
	public ItemTabManager itemTabLH;
	public ItemTabManager itemTabRH;
	public enum handedness {LeftHand,RightHand};
	public DataTab dataTabLH;
	public DataTab dataTabRH;
	public bool lastWeaponSideRH;
	public bool lastItemSideRH;
	public bool lastAutomapSideRH;
	public bool lastTargetSideRH;
	public bool lastDataSideRH;
	public bool lastSearchSideRH;
	public bool lastLogSideRH;
	public bool lastLogSecondarySideRH;
	public bool lastMinigameSideRH;
	public GameObject SearchFXRH;
	public GameObject SearchFXLH;
	public enum TabMSG {None,Search,AudioLog,Keypad,Elevator,GridPuzzle,WirePuzzle,EReader};
	public static MFDManager a;

	private bool isRH;

	// External to gameObject, assigned in Inspector
	public WeaponMagazineCounter wepmagCounterLH;
	public WeaponMagazineCounter wepmagCounterRH;
	public GameObject logReaderContainer;
	public GameObject multiMediaTab;

	public void Awake () {
		a = this;
	}

	public void OpenTab(int index, bool overrideToggling,TabMSG type,int intdata1, handedness side) {
		if (side == handedness.LeftHand) {
			isRH = false;
		} else {
			isRH = true;
		}
		switch (index) {
			case 0: isRH = lastWeaponSideRH; break;
			case 1: isRH = lastItemSideRH; break;
			case 2: isRH = lastAutomapSideRH; break;
			case 3: isRH = lastTargetSideRH; break;
			case 4: isRH = lastDataSideRH; break;
		}
		if(!isRH) {
			// RH MFD
			leftTC.TabButtonClickSilent(index,overrideToggling);
			leftTC.SetCurrentAsLast();
			if (type == TabMSG.AudioLog) {
				dataTabLH.Reset();
				dataTabLH.audioLogContainer.SetActive(true);
				dataTabLH.audioLogContainer.GetComponent<LogDataTabContainerManager>().SendLogData(intdata1);
			}

			if (type == TabMSG.Keypad) {
				dataTabLH.Reset();
				dataTabLH.keycodeUIControl.SetActive(true);
			}

			if (type == TabMSG.Elevator) {
				dataTabLH.Reset();
				dataTabLH.elevatorUIControl.SetActive(true);
			}

			if (type == TabMSG.GridPuzzle) {
				dataTabLH.Reset();
				dataTabLH.puzzleGrid.SetActive(true);
			}

			if (type == TabMSG.EReader) {
				//itemTabLH.Reset();
				itemTabLH.EReaderSectionSContainerOpen();
			}
		} else {
			// LH MFD
			rightTC.TabButtonClickSilent(index,overrideToggling);
			rightTC.SetCurrentAsLast();
			if (type == TabMSG.AudioLog) {
				dataTabRH.Reset();
				dataTabRH.audioLogContainer.SetActive(true);
				dataTabRH.audioLogContainer.GetComponent<LogDataTabContainerManager>().SendLogData(intdata1);
			}

			if (type == TabMSG.Keypad) {
				dataTabRH.Reset();
				dataTabRH.keycodeUIControl.SetActive(true);
			}

			if (type == TabMSG.Elevator) {
				dataTabRH.Reset();
				dataTabRH.elevatorUIControl.SetActive(true);
			}

			if (type == TabMSG.GridPuzzle) {
				dataTabLH.Reset();
				dataTabLH.puzzleGrid.SetActive(true);
			}

			if (type == TabMSG.EReader) {
				//itemTabRH.Reset();
				itemTabRH.EReaderSectionSContainerOpen();
			}
		}
	}

	public void SendSearchToDataTab (string name, int contentCount, int[] resultContents, int[] resultsIndices) {
		// Enable search box scaling effect
		if (lastSearchSideRH) {
			dataTabRH.Reset();
			dataTabRH.Search(name,contentCount,resultContents,resultsIndices);
			OpenTab(4,true,TabMSG.Search,0,handedness.RightHand);
			SearchFXRH.SetActive(true);
		} else {
			dataTabLH.Reset();
			dataTabLH.Search(name,contentCount,resultContents,resultsIndices);
			OpenTab(4,true,TabMSG.Search,0,handedness.LeftHand);
			SearchFXLH.SetActive(true);
		}
	}

	public void SendGridPuzzleToDataTab (bool[] states, PuzzleGrid.CellType[] types, PuzzleGrid.GridType gtype, int start, int end, int width, int height, PuzzleGrid.GridColorTheme colors) {
		if (lastDataSideRH) {
			// Send to RH tab
			dataTabRH.Reset();
			dataTabRH.GridPuzzle(states,types,gtype,start,end, width, height,colors);
			OpenTab(4,true,TabMSG.GridPuzzle,0,handedness.RightHand);
		} else {
			// Send to LH tab
			dataTabLH.Reset();
			dataTabLH.GridPuzzle(states,types,gtype,start,end, width, height,colors);
			OpenTab(4,true,TabMSG.GridPuzzle,0,handedness.LeftHand);
		}
	}

	public void SendPaperLogToDataTab(int index) {
		if (lastDataSideRH) {
			// Send to RH tab
			dataTabRH.Reset();
			OpenTab(4,true,TabMSG.AudioLog,index,handedness.RightHand);
		} else {
			// Send to LH tab
			dataTabLH.Reset();
			OpenTab(4,true,TabMSG.AudioLog,index,handedness.LeftHand);
		}
		multiMediaTab.GetComponent<MultiMediaTabManager>().OpenLogTextReader();
		logReaderContainer.GetComponent<LogTextReaderManager>().SendTextToReader(index);
	}

	public void SendAudioLogToDataTab(int index) {
		if (lastDataSideRH) {
			// Send to RH tab
			dataTabRH.Reset();
			OpenTab(4,true,TabMSG.AudioLog,index,handedness.RightHand);
		} else {
			// Send to LH tab
			dataTabLH.Reset();
			OpenTab(4,true,TabMSG.AudioLog,index,handedness.LeftHand);
		}
		multiMediaTab.GetComponent<MultiMediaTabManager>().OpenLogTextReader();
		logReaderContainer.GetComponent<LogTextReaderManager>().SendTextToReader(index);
	}

	public void OpenEReaderInItemsTab() {
		if (lastItemSideRH) {
			OpenTab(1,true,TabMSG.EReader,-1,handedness.RightHand);
		} else {
			OpenTab(1,true,TabMSG.EReader,-1,handedness.LeftHand);
		}
	}

	public void ClearDataTab() {
		if (lastDataSideRH) {
			dataTabRH.Reset();
		} else {
			dataTabLH.Reset();
		}
	}

	public void TurnOffKeypad() {
		if (lastDataSideRH) {
			dataTabRH.keycodeUIControl.SetActive(false);
		} else {
			dataTabLH.keycodeUIControl.SetActive(false);
		}
	}

	public void TurnOffElevatorPad() {
		if (lastDataSideRH) {
			dataTabRH.elevatorUIControl.SetActive(false);
		} else {
			dataTabLH.elevatorUIControl.SetActive(false);
		}
	}

	public bool GetElevatorControlActiveState() {
		if (lastDataSideRH) {
			return dataTabRH.elevatorUIControl.activeInHierarchy;
		} else {
			return dataTabLH.elevatorUIControl.activeInHierarchy;
		}
	}

	public void BlockedBySecurity() {
		if (lastDataSideRH) {
			dataTabRH.blockedBySecurity.SetActive(true);
		} else {
			dataTabLH.blockedBySecurity.SetActive(true);
		}
	}

	public void UpdateHUDAmmoCounts(int amount) {
		if (wepmagCounterLH != null) wepmagCounterLH.UpdateDigits(amount);
		if (wepmagCounterRH != null) wepmagCounterRH.UpdateDigits(amount);
	}
}