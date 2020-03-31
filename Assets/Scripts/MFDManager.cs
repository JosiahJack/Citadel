using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MFDManager : MonoBehaviour  {
	public TabButtons leftTC;
	public TabButtons rightTC;
	public CenterTabButtons ctb;
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
	public enum TabMSG {None,Search,AudioLog,Keypad,Elevator,GridPuzzle,WirePuzzle,EReader,Weapon};
	public static MFDManager a;
	public MouseLookScript playerMLook;
	private bool isRH;

	// External to gameObject, assigned in Inspector
	public WeaponMagazineCounter wepmagCounterLH;
	public WeaponMagazineCounter wepmagCounterRH;
	public GameObject logReaderContainer;
	public GameObject multiMediaTab;
	private float logFinished;
	private bool logActive;

	public void Awake () {
		a = this;
		a.logFinished = Time.time;
		a.logActive = false;
	}

	void Update () {
		if (logActive) {
			if (logFinished < Time.time) {
				logActive = false;
				ReturnToLastTab(lastLogSideRH);
				//ReturnToLastTab(lastLogSideSecondaryRH);  UPDATE add back in when LH MFD is done
				if (ctb != null) ctb.TabButtonClickSilent(0,false);  // UPDATE add memory for last center tab as well at some point
			}
		}
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
			// LH LEFT HAND MFD
			leftTC.SetCurrentAsLast();
			leftTC.TabButtonClickSilent(index,overrideToggling);
			if (type == TabMSG.Weapon) {
				dataTabLH.Reset();
				
			}

			if (type == TabMSG.AudioLog) {
				dataTabLH.Reset();
				dataTabLH.audioLogContainer.SetActive(true);
				dataTabLH.audioLogContainer.GetComponent<LogDataTabContainerManager>().SendLogData(intdata1);
			}

			if (type == TabMSG.Keypad) {
				dataTabLH.Reset();
				dataTabLH.keycodeUIControl.SetActive(true);
				playerMLook.ForceInventoryMode();
			}

			if (type == TabMSG.Elevator) {
				dataTabLH.Reset();
				dataTabLH.elevatorUIControl.SetActive(true);
				playerMLook.ForceInventoryMode();
			}

			if (type == TabMSG.GridPuzzle) {
				dataTabLH.Reset();
				dataTabLH.puzzleGrid.SetActive(true);
				playerMLook.ForceInventoryMode();
			}

			if (type == TabMSG.WirePuzzle) {
				dataTabLH.Reset();
				dataTabLH.puzzleWire.SetActive(true);
				playerMLook.ForceInventoryMode();
			}

			if (type == TabMSG.EReader) {
				//itemTabLH.Reset();
				itemTabLH.EReaderSectionSContainerOpen();
				playerMLook.ForceInventoryMode();
			}
		} else {
			// RH RIGHT HAND MFD
			rightTC.SetCurrentAsLast();
			rightTC.TabButtonClickSilent(index,overrideToggling);
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
				dataTabRH.Reset();
				dataTabRH.puzzleGrid.SetActive(true);
			}

			if (type == TabMSG.WirePuzzle) {
				dataTabRH.Reset();
				dataTabRH.puzzleWire.SetActive(true);
				playerMLook.ForceInventoryMode();
			}

			if (type == TabMSG.EReader) {
				//itemTabRH.Reset();
				itemTabRH.EReaderSectionSContainerOpen();
				playerMLook.ForceInventoryMode();
			}
		}
	}

	public void SendInfoToItemTab(int index) {
		if (itemTabRH != null) itemTabRH.SendItemDataToItemTab(index);
		if (itemTabLH != null) itemTabLH.SendItemDataToItemTab(index);
	}

	public void SendSearchToDataTab (string name, int contentCount, int[] resultContents, int[] resultsIndices, Vector3 searchPosition) {
		// Enable search box scaling effect
		if (lastSearchSideRH) {
			dataTabRH.Reset();
			dataTabRH.Search(name,contentCount,resultContents,resultsIndices,searchPosition);
			OpenTab(4,true,TabMSG.Search,0,handedness.RightHand);
			SearchFXRH.SetActive(true);
		} else {
			dataTabLH.Reset();
			dataTabLH.Search(name,contentCount,resultContents,resultsIndices,searchPosition);
			OpenTab(4,true,TabMSG.Search,0,handedness.LeftHand);
			SearchFXLH.SetActive(true);
		}
	}

	public void SendGridPuzzleToDataTab (bool[] states, PuzzleGrid.CellType[] types, PuzzleGrid.GridType gtype, int start, int end, int width, int height, PuzzleGrid.GridColorTheme colors, string t1, UseData ud, Vector3 tetherPoint) {
		if (lastDataSideRH) {
			// Send to RH tab
			dataTabRH.Reset();
			dataTabRH.GridPuzzle(states,types,gtype,start,end, width, height,colors,t1,ud);
			dataTabRH.objectInUsePos = tetherPoint;
			dataTabRH.usingObject = true;
			OpenTab(4,true,TabMSG.GridPuzzle,0,handedness.RightHand);
			SearchFXRH.SetActive(true);
		} else {
			// Send to LH tab
			dataTabLH.Reset();
			dataTabLH.GridPuzzle(states,types,gtype,start,end, width, height,colors,t1,ud);
			dataTabLH.objectInUsePos = tetherPoint;
			dataTabLH.usingObject = true;
			OpenTab(4,true,TabMSG.GridPuzzle,0,handedness.LeftHand);
			SearchFXLH.SetActive(true);
		}
	}

	public void SendWirePuzzleToDataTab(bool[] sentWiresOn, bool[] sentNodeRowsActive, int[] sentCurrentPositionsLeft, int[] sentCurrentPositionsRight, int[] sentTargetsLeft, int[] sentTargetsRight, PuzzleWire.WireColorTheme theme, PuzzleWire.WireColor[] wireColors, string t1, string a1, UseData udSent,Vector3 tetherPoint) {
		if (lastDataSideRH) {
			// Send to RH tab
			dataTabRH.Reset();
			dataTabRH.WirePuzzle(sentWiresOn,sentNodeRowsActive,sentCurrentPositionsLeft,sentCurrentPositionsRight,sentTargetsLeft,sentTargetsRight,theme,wireColors,t1,a1,udSent);
			dataTabRH.objectInUsePos = tetherPoint;
			dataTabRH.usingObject = true;
			OpenTab(4,true,TabMSG.WirePuzzle,0,handedness.RightHand);
			SearchFXRH.SetActive(true);
		} else {
			// Send to LH tab
			dataTabLH.Reset();
			dataTabLH.WirePuzzle(sentWiresOn,sentNodeRowsActive,sentCurrentPositionsLeft,sentCurrentPositionsRight,sentTargetsLeft,sentTargetsRight,theme,wireColors,t1,a1,udSent);
			dataTabLH.objectInUsePos = tetherPoint;
			dataTabLH.usingObject = true;
			OpenTab(4,true,TabMSG.WirePuzzle,0,handedness.LeftHand);
			SearchFXLH.SetActive(true);
		}
	}

	public void SendPaperLogToDataTab(int index) {
		if (lastDataSideRH) {
			// Send to RH tab
			dataTabRH.Reset();
			OpenTab(4,true,TabMSG.AudioLog,index,handedness.RightHand);
			SearchFXRH.SetActive(true);
		} else {
			// Send to LH tab
			dataTabLH.Reset();
			OpenTab(4,true,TabMSG.AudioLog,index,handedness.LeftHand);
			SearchFXLH.SetActive(true);
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
		if (Const.a.audioLogs[index] != null) logFinished = Time.time + Const.a.audioLogs[index].length + 0.1f; //add slight delay after log is finished playing to make sure we don't cut off audio in case there's a frame delay for audio start
		logActive = true;
	}

	public void OpenEReaderInItemsTab() {
		if (lastItemSideRH) {
			//itemTabRH.Reset();  done later in OpenTab
			OpenTab(1,true,TabMSG.EReader,-1,handedness.RightHand);
		} else {
			//itemTabLH.Reset();  done later in OpenTab
			OpenTab(1,true,TabMSG.EReader,-1,handedness.LeftHand);
		}
		if (ctb != null) ctb.TabButtonClickSilent(4,false);
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

	public void BlockedBySecurity(Vector3 tetherPoint) {
		if (lastDataSideRH) {
			dataTabRH.Reset();
			OpenTab(4,true,MFDManager.TabMSG.None,0,MFDManager.handedness.RightHand);
			dataTabRH.blockedBySecurity.SetActive(true);
			dataTabRH.objectInUsePos = tetherPoint;
			dataTabRH.usingObject = true;
		} else {
			dataTabLH.Reset();
			OpenTab(4,true,MFDManager.TabMSG.None,0,MFDManager.handedness.LeftHand);
			dataTabLH.blockedBySecurity.SetActive(true);
			dataTabLH.objectInUsePos = tetherPoint;
			dataTabLH.usingObject = true;
		}
	}

	public void SendKeypadKeycodeToDataTab(int keycode, Vector3 tetherPoint, KeypadKeycode keypad, bool alreadySolved) {
		if (lastDataSideRH) {
			dataTabRH.Reset();
			OpenTab(4,true,MFDManager.TabMSG.Keypad,0,MFDManager.handedness.RightHand);
			dataTabRH.keycodeUIControl.SetActive(true);
			dataTabRH.keycodeUIControl.GetComponent<KeypadKeycodeButtons>().keycode = keycode;
			dataTabRH.keycodeUIControl.GetComponent<KeypadKeycodeButtons>().keypad = keypad;
			dataTabRH.keycodeUIControl.GetComponent<KeypadKeycodeButtons>().done = alreadySolved;
			dataTabRH.objectInUsePos = tetherPoint;
			dataTabRH.usingObject = true;
		} else {
			dataTabLH.Reset();
			OpenTab(4,true,MFDManager.TabMSG.Keypad,0,MFDManager.handedness.LeftHand);
			dataTabLH.keycodeUIControl.SetActive(true);
			dataTabLH.keycodeUIControl.GetComponent<KeypadKeycodeButtons>().keycode = keycode;
			dataTabLH.keycodeUIControl.GetComponent<KeypadKeycodeButtons>().keypad = keypad;
			dataTabLH.keycodeUIControl.GetComponent<KeypadKeycodeButtons>().done = alreadySolved;
			dataTabLH.objectInUsePos = tetherPoint;
			dataTabLH.usingObject = true;
		}
	}

	public void UpdateHUDAmmoCounts(int amount) {
		if (wepmagCounterLH != null) wepmagCounterLH.UpdateDigits(amount);
		if (wepmagCounterRH != null) wepmagCounterRH.UpdateDigits(amount);
	}

	public void ReturnToLastTab(bool isRightHand) {
		if (isRightHand) {
			rightTC.ReturnToLastTab();
		} else {
			leftTC.ReturnToLastTab();
		}
	}
}