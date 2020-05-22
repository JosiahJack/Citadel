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
	[HideInInspector]
	public Vector3 objectInUsePos;
	[HideInInspector]
	public PuzzleGridPuzzle tetheredPGP = null;
	[HideInInspector]
	public PuzzleWirePuzzle tetheredPWP = null;
	[HideInInspector]
	public SearchableItem tetheredSearchable = null;
	[HideInInspector]
	public KeypadElevator tetheredKeypadElevator = null;
	[HideInInspector]
	public KeypadKeycode tetheredKeypadKeycode = null;
	[HideInInspector]
	public bool paperLogInUse = false;
	[HideInInspector]
	public bool usingObject = false;
	public Transform playerCapsuleTransform;

	// External to gameObject, assigned in Inspector
	public WeaponMagazineCounter wepmagCounterLH;
	public WeaponMagazineCounter wepmagCounterRH;
	public GameObject logReaderContainer;
	public GameObject multiMediaTab;
	public GameObject logTable;
	public GameObject logLevelsFolder;
	public WeaponButtonsManager wepbutMan;
	[HideInInspector]
	public float logFinished;
	[HideInInspector]
	public bool logActive;
	[HideInInspector]
	public int logType = 0;

	//Handedness inspector assignments
	//	SearchButton.isRH found on SearchContentsContainer under DataTab

	public GameObject headerTextLH;
	public GameObject headerTextRH;
	public Text headerText_textLH;
	public Text headerText_textRH;
	public GameObject noItemsTextLH;
	public GameObject noItemsTextRH;
	public GameObject blockedBySecurityLH;
	public GameObject blockedBySecurityRH;
	public GameObject elevatorUIControlLH;
	public GameObject elevatorUIControlRH;
	[HideInInspector]
	public Door linkedElevatorDoor;
	public GameObject keycodeUIControlLH;
	public GameObject keycodeUIControlRH;
	public GameObject[] searchItemImagesLH;
	public GameObject[] searchItemImagesRH;
	public GameObject audioLogContainerLH;
	public GameObject audioLogContainerRH;
	public GameObject puzzleGridLH;
	public GameObject puzzleGridRH;
	public GameObject puzzleWireLH;
	public GameObject puzzleWireRH;
	public SearchButton searchContainerLH;
	public SearchButton searchContainerRH;

	public void Start () {
		a = this;
		a.logFinished = PauseScript.a.relativeTime;
		a.logActive = false;
		//a.TabReset(true);
		a.TabReset(false);
	}


	public void TabReset(bool isRH) {
		if (isRH) {
			headerTextRH.SetActive(false);
			headerText_textRH.text = System.String.Empty;
			noItemsTextRH.SetActive(false);
			blockedBySecurityRH.SetActive(false);
			elevatorUIControlRH.SetActive(false);
			keycodeUIControlRH.SetActive(false);
			puzzleGridRH.SetActive(false);
			puzzleWireRH.SetActive(false);
			audioLogContainerRH.SetActive(false);
			for (int i=0; i<=3;i++) {
				searchItemImagesRH[i].SetActive(false);
			}
		} else {
			headerTextLH.SetActive(false);
			headerText_textLH.text = System.String.Empty;
			noItemsTextLH.SetActive(false);
			blockedBySecurityLH.SetActive(false);
			elevatorUIControlLH.SetActive(false);
			keycodeUIControlLH.SetActive(false);
			puzzleGridLH.SetActive(false);
			puzzleWireLH.SetActive(false);
			audioLogContainerLH.SetActive(false);
			for (int i=0; i<=3;i++) {
				searchItemImagesLH[i].SetActive(false);
			}
		}
	}

	void Update () {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			if (logActive) {
				if (logFinished < PauseScript.a.relativeTime && logType != 3 && logType != 0) {
					logActive = false;
					ReturnToLastTab(lastLogSideRH);
					//ReturnToLastTab(lastLogSideSecondaryRH);  UPDATE add back in when LH MFD is done
					if (ctb != null) ctb.TabButtonClickSilent(0,true);  // UPDATE add memory for last center tab as well at some point
				}
			}

			WeaponButtonsManagerUpdate();

			if (GetInput.a.WeaponCycUp ()) {
				if (Const.a.InputInvertInventoryCycling) {
					wepbutMan.WeaponCycleDown();
				} else {
					wepbutMan.WeaponCycleUp();
				}
			}

			if (GetInput.a.WeaponCycDown ()) {
				if (Const.a.InputInvertInventoryCycling) {
					wepbutMan.WeaponCycleUp();
				} else {
					wepbutMan.WeaponCycleDown();
				}
			}

			if (usingObject) {
				if (Vector3.Distance(playerCapsuleTransform.position, objectInUsePos) > (Const.a.frobDistance + 0.16f)) {
					if (tetheredPGP != null) {
						if (lastDataSideRH) {
							PuzzleGrid pg = puzzleGridRH.GetComponent<PuzzleGrid>();
							tetheredPGP.SendDataBackToPanel(pg);
							pg.Reset();
						} else {
							PuzzleGrid pg = puzzleGridLH.GetComponent<PuzzleGrid>();
							tetheredPGP.SendDataBackToPanel(pg);
							pg.Reset();
						}
						tetheredPGP = null;
					}

					if (tetheredPWP != null) {
						if (lastDataSideRH) {
							PuzzleWire pw = puzzleWireRH.GetComponent<PuzzleWire>();
							tetheredPWP.SendDataBackToPanel(pw);
							pw.Reset();
						} else {
							PuzzleWire pw = puzzleWireLH.GetComponent<PuzzleWire>();
							tetheredPWP.SendDataBackToPanel(pw);
							pw.Reset();
						}
						tetheredPWP = null;
					}

					if (tetheredKeypadElevator != null) {
						tetheredKeypadElevator.SendDataBackToPanel();
						TurnOffElevatorPad();
						tetheredKeypadElevator = null;
						linkedElevatorDoor = null;
					}

					if (tetheredSearchable != null) {
						tetheredSearchable.ResetSearchable(false);
						tetheredSearchable = null;
					}
					if (paperLogInUse && ctb != null) {
						ctb.TabButtonClickSilent(0,false);  // UPDATE add memory for last center tab as well at some point
					}

					TabReset(lastDataSideRH);
					usingObject = false;
					logTable.SetActive(false);
					logLevelsFolder.SetActive(false);
					logReaderContainer.SetActive(false);
					ReturnToLastTab(lastDataSideRH);
				}
			}
		}
	}

	void WeaponButtonsManagerUpdate() {
		for (int i=0; i<7; i++) {
			if (WeaponInventory.WepInventoryInstance.weaponInventoryIndices[i] > 0) {
				if (!wepbutMan.wepButtons[i].activeInHierarchy) wepbutMan.wepButtons[i].SetActive(true);
				wepbutMan.wepButtonsScripts[i].useableItemIndex = WeaponInventory.WepInventoryInstance.weaponInventoryIndices[i];
				if (!wepbutMan.wepCountsText[i].activeInHierarchy) wepbutMan.wepCountsText[i].SetActive(true);
			} else {
				if (wepbutMan.wepButtons[i].activeInHierarchy) wepbutMan.wepButtons[i].SetActive(false);
				wepbutMan.wepButtonsScripts[i].useableItemIndex = -1;
				if (wepbutMan.wepCountsText[i].activeInHierarchy) wepbutMan.wepCountsText[i].SetActive(false);
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
				TabReset(false);
			}

			if (type == TabMSG.AudioLog) {
				TabReset(false);
				audioLogContainerLH.SetActive(true);
				audioLogContainerLH.GetComponent<LogDataTabContainerManager>().SendLogData(intdata1);
			}

			if (type == TabMSG.Keypad) {
				TabReset(false);
				keycodeUIControlLH.SetActive(true);
				playerMLook.ForceInventoryMode();
			}

			if (type == TabMSG.Elevator) {
				TabReset(false);
				elevatorUIControlLH.SetActive(true);
				playerMLook.ForceInventoryMode();
			}

			if (type == TabMSG.GridPuzzle) {
				TabReset(false);
				puzzleGridLH.SetActive(true);
				playerMLook.ForceInventoryMode();
			}

			if (type == TabMSG.WirePuzzle) {
				TabReset(false);
				puzzleWireLH.SetActive(true);
				playerMLook.ForceInventoryMode();
			}

			if (type == TabMSG.EReader) {
				//TabReset(false);
				itemTabLH.EReaderSectionSContainerOpen();
				playerMLook.ForceInventoryMode();
			}
		} else {
			// RH RIGHT HAND MFD
			rightTC.SetCurrentAsLast();
			rightTC.TabButtonClickSilent(index,overrideToggling);
			if (type == TabMSG.AudioLog) {
				TabReset(true);
				audioLogContainerRH.SetActive(true);
				audioLogContainerRH.GetComponent<LogDataTabContainerManager>().SendLogData(intdata1);
			}

			if (type == TabMSG.Keypad) {
				TabReset(true);
				keycodeUIControlRH.SetActive(true);
			}

			if (type == TabMSG.Elevator) {
				TabReset(true);
				elevatorUIControlRH.SetActive(true);
			}

			if (type == TabMSG.GridPuzzle) {
				TabReset(true);
				puzzleGridRH.SetActive(true);
			}

			if (type == TabMSG.WirePuzzle) {
				TabReset(true);
				puzzleWireRH.SetActive(true);
				playerMLook.ForceInventoryMode();
			}

			if (type == TabMSG.EReader) {
				//TabReset(true);
				itemTabRH.EReaderSectionSContainerOpen();
				playerMLook.ForceInventoryMode();
			}
		}
	}

	public void SendInfoToItemTab(int index) {
		if (itemTabRH != null) itemTabRH.SendItemDataToItemTab(index);
		if (itemTabLH != null) itemTabLH.SendItemDataToItemTab(index);
	}

	public void Search(bool isRH, string head, int numberFoundContents, int[] contents, int[] customIndex) {
		if (isRH) {
			headerTextRH.SetActive(true);
			headerText_textRH.enabled = true;
			headerText_textRH.text = head;

			if (numberFoundContents <= 0) {
				noItemsTextRH.SetActive(true);
				noItemsTextRH.GetComponent<Text>().enabled = true;
				return;
			}

			for (int i=0;i<4;i++) {
				if (contents[i] > -1) {
					searchItemImagesRH[i].SetActive(true);
					searchItemImagesRH[i].GetComponent<Image>().overrideSprite = Const.a.searchItemIconSprites[contents[i]];
					searchContainerRH.contents[i] = contents[i];
					searchContainerRH.customIndex[i] = customIndex[i];
				}
			}
		} else {
			headerTextLH.SetActive(true);
			headerText_textLH.enabled = true;
			headerText_textLH.text = head;

			if (numberFoundContents <= 0) {
				noItemsTextLH.SetActive(true);
				noItemsTextLH.GetComponent<Text>().enabled = true;
				return;
			}

			for (int i=0;i<4;i++) {
				if (contents[i] > -1) {
					searchItemImagesLH[i].SetActive(true);
					searchItemImagesLH[i].GetComponent<Image>().overrideSprite = Const.a.searchItemIconSprites[contents[i]];
					searchContainerLH.contents[i] = contents[i];
					searchContainerLH.customIndex[i] = customIndex[i];
				}
			}
		}
	}

	public void SendSearchToDataTab (string name, int contentCount, int[] resultContents, int[] resultsIndices, Vector3 searchPosition, SearchableItem si) {
		// Enable search box scaling effect
		if (lastSearchSideRH) {
			TabReset(true);
			Search(true,name,contentCount,resultContents,resultsIndices);
			OpenTab(4,true,TabMSG.Search,0,handedness.RightHand);
			SearchFXRH.SetActive(true);
		} else {
			TabReset(false);
			Search(false,name,contentCount,resultContents,resultsIndices);
			OpenTab(4,true,TabMSG.Search,0,handedness.LeftHand);
			SearchFXLH.SetActive(true);
		}
		if (tetheredSearchable != null) tetheredSearchable.ResetSearchable(false);
		tetheredSearchable = si;
		objectInUsePos = searchPosition;
		usingObject = true;
	}

	public void SendGridPuzzleToDataTab (bool[] states, PuzzleGrid.CellType[] types, PuzzleGrid.GridType gtype, int start, int end, int width, int height, PuzzleGrid.GridColorTheme colors, string t1, UseData ud, Vector3 tetherPoint, PuzzleGridPuzzle pgp) {
		if (lastDataSideRH) {
			// Send to RH tab
			TabReset(true);
			puzzleGridRH.GetComponent<PuzzleGrid>().SendGrid(states,types,gtype,start,end, width, height,colors,t1,ud,pgp);
			OpenTab(4,true,TabMSG.GridPuzzle,0,handedness.RightHand);
			SearchFXRH.SetActive(true);
		} else {
			// Send to LH tab
			TabReset(false);
			puzzleGridLH.GetComponent<PuzzleGrid>().SendGrid(states,types,gtype,start,end, width, height,colors,t1,ud,pgp);
			OpenTab(4,true,TabMSG.GridPuzzle,0,handedness.LeftHand);
			SearchFXLH.SetActive(true);
		}
		objectInUsePos = tetherPoint;
		tetheredPGP = pgp;
		usingObject = true;
	}

	public void SendWirePuzzleToDataTab(bool[] sentWiresOn, bool[] sentNodeRowsActive, int[] sentCurrentPositionsLeft, int[] sentCurrentPositionsRight, int[] sentTargetsLeft, int[] sentTargetsRight, PuzzleWire.WireColorTheme theme, PuzzleWire.WireColor[] wireColors, string t1, string a1, UseData udSent,Vector3 tetherPoint, PuzzleWirePuzzle pwp) {
		TabReset(lastDataSideRH);
		if (lastDataSideRH) {
			// Send to RH tab
			puzzleWireRH.GetComponent<PuzzleWire>().SendWirePuzzleData(sentWiresOn,sentNodeRowsActive,sentCurrentPositionsLeft,sentCurrentPositionsRight,sentTargetsLeft,sentTargetsRight,theme,wireColors,t1,a1,udSent,pwp);
			OpenTab(4,true,TabMSG.WirePuzzle,0,handedness.RightHand);
			SearchFXRH.SetActive(true);
		} else {
			// Send to LH tab
			puzzleWireLH.GetComponent<PuzzleWire>().SendWirePuzzleData(sentWiresOn,sentNodeRowsActive,sentCurrentPositionsLeft,sentCurrentPositionsRight,sentTargetsLeft,sentTargetsRight,theme,wireColors,t1,a1,udSent,pwp);
			OpenTab(4,true,TabMSG.WirePuzzle,0,handedness.LeftHand);
			SearchFXLH.SetActive(true);
		}
		objectInUsePos = tetherPoint;
		tetheredPWP = pwp;
		usingObject = true;
	}

	public void SendPaperLogToDataTab(int index,Vector3 tetherPoint) {
		TabReset(lastDataSideRH);
		if (lastDataSideRH) {
			// Send to RH tab
			OpenTab(4,true,TabMSG.AudioLog,index,handedness.RightHand);
			SearchFXRH.SetActive(true);
		} else {
			// Send to LH tab
			OpenTab(4,true,TabMSG.AudioLog,index,handedness.LeftHand);
			SearchFXLH.SetActive(true);
		}
		objectInUsePos = tetherPoint;
		usingObject = true;
		multiMediaTab.GetComponent<MultiMediaTabManager>().OpenLogTextReader();
		multiMediaTab.SetActive(true);
		logReaderContainer.SetActive(true);
		logReaderContainer.GetComponent<LogTextReaderManager>().SendTextToReader(index);
		logTable.SetActive(false);
		logLevelsFolder.SetActive(false);
	}

	public void SendAudioLogToDataTab(int index) {
		TabReset(lastDataSideRH);
		if (lastDataSideRH) {
			// Send to RH tab
			OpenTab(4,true,TabMSG.AudioLog,index,handedness.RightHand);
		} else {
			// Send to LH tab
			OpenTab(4,true,TabMSG.AudioLog,index,handedness.LeftHand);
		}
		ctb.TabButtonClickSilent(4,true);	
		if (tetheredSearchable != null) {
			tetheredSearchable.searchableInUse = false;
		}
		multiMediaTab.GetComponent<MultiMediaTabManager>().OpenLogTextReader();
		multiMediaTab.SetActive(true);
		logReaderContainer.SetActive(true);
		logReaderContainer.GetComponent<LogTextReaderManager>().SendTextToReader(index);
		logTable.SetActive(false);
		logLevelsFolder.SetActive(false);
		if (Const.a.audioLogs[index] != null) logFinished = PauseScript.a.relativeTime + Const.a.audioLogs[index].length + 0.1f; //add slight delay after log is finished playing to make sure we don't cut off audio in case there's a frame delay for audio start
		logActive = true;
		logType = Const.a.audioLogType[index];
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
		if (tetheredSearchable != null) {
			tetheredSearchable.searchableInUse = false;
		}
		logTable.SetActive(false);
		logLevelsFolder.SetActive(false);
		logReaderContainer.SetActive(false);
		multiMediaTab.GetComponent<MultiMediaTabManager>().OpenLastTab();
	}

	public void ClearDataTab() {
		TabReset(lastDataSideRH);
	}

	public void TurnOffKeypad() {
		if (lastDataSideRH) {
			keycodeUIControlRH.SetActive(false);
		} else {
			keycodeUIControlLH.SetActive(false);
		}
	}

	public void TurnOffElevatorPad() {
		if (lastDataSideRH) {
			elevatorUIControlRH.SetActive(false);
		} else {
			elevatorUIControlLH.SetActive(false);
		}
	}

	public bool GetElevatorControlActiveState() {
		if (lastDataSideRH) {
			return elevatorUIControlRH.activeInHierarchy;
		} else {
			return elevatorUIControlLH.activeInHierarchy;
		}
	}

	public void BlockedBySecurity(Vector3 tetherPoint, UseData ud) {
		TabReset(lastDataSideRH);
		if (lastDataSideRH) {
			OpenTab(4,true,MFDManager.TabMSG.None,0,MFDManager.handedness.RightHand);
			blockedBySecurityRH.SetActive(true);
		} else {
			OpenTab(4,true,MFDManager.TabMSG.None,0,MFDManager.handedness.LeftHand);
			blockedBySecurityLH.SetActive(true);
		}
		Const.sprint(Const.a.stringTable[25],ud.owner);
		objectInUsePos = tetherPoint;
		usingObject = true;
	}

	public void SendKeypadKeycodeToDataTab(int keycode, Vector3 tetherPoint, KeypadKeycode keypad, bool alreadySolved) {
		TabReset(lastDataSideRH);
		if (lastDataSideRH) {
			OpenTab(4,true,MFDManager.TabMSG.Keypad,0,MFDManager.handedness.RightHand);
			keycodeUIControlRH.SetActive(true);
			KeypadKeycodeButtons kkb = keycodeUIControlRH.GetComponent<KeypadKeycodeButtons>();
			kkb.keycode = keycode;
			kkb.keypad = keypad;
			kkb.ResetEntry();
			kkb.done = alreadySolved;
		} else {
			OpenTab(4,true,MFDManager.TabMSG.Keypad,0,MFDManager.handedness.LeftHand);
			keycodeUIControlLH.SetActive(true);
			KeypadKeycodeButtons kkb = keycodeUIControlLH.GetComponent<KeypadKeycodeButtons>();
			kkb.keycode = keycode;
			kkb.keypad = keypad;
			kkb.ResetEntry();
			kkb.done = alreadySolved;
		}
		objectInUsePos = tetherPoint;
		usingObject = true;
	}

	public void SendElevatorKeypadToDataTab(KeypadElevator ke, bool[] buttonsEnabled, bool[] buttonsDarkened, string[] buttonText,GameObject[] targetDestination,Vector3 tetherPoint,Door linkedDoor,int currentFloor) {
		TabReset(lastDataSideRH);
		ElevatorKeypad elevatorKeypad;
		if (lastDataSideRH) {
			elevatorKeypad = elevatorUIControlRH.GetComponent<ElevatorKeypad>();
		} else {
			elevatorKeypad = elevatorUIControlLH.GetComponent<ElevatorKeypad>();
		}
		for (int i=0;i<8;i++) {
			elevatorKeypad.buttonsEnabled[i] = buttonsEnabled[i];
			elevatorKeypad.buttonsDarkened[i] = buttonsDarkened[i];
			elevatorKeypad.buttonText[i] = buttonText[i];
			elevatorKeypad.targetDestination[i] = targetDestination[i];

			if (elevatorKeypad.buttonsEnabled[i]) {
				if (!elevatorKeypad.buttons[i].activeSelf) elevatorKeypad.buttons[i].SetActive(true);
				elevatorKeypad.buttonTextHolders[i].text = elevatorKeypad.buttonText[i];

				if (elevatorKeypad.buttonsDarkened[i]) {
					elevatorKeypad.buttonSprites[i].overrideSprite = elevatorKeypad.buttonDarkened;
					elevatorKeypad.buttonTextHolders[i].color = elevatorKeypad.textDarkenedColor;
					elevatorKeypad.buttonHandlers[i].GetComponent<ElevatorButton>().floorAccessible = false;
				} else {
					elevatorKeypad.buttonSprites[i].overrideSprite = elevatorKeypad.buttonNormal;
					elevatorKeypad.buttonSprites[i].overrideSprite = elevatorKeypad.buttonNormal;
					elevatorKeypad.buttonTextHolders[i].color = elevatorKeypad.textEnabledColor;
					elevatorKeypad.buttonHandlers[i].GetComponent<ElevatorButton>().floorAccessible = true;
					elevatorKeypad.buttonHandlers[i].GetComponent<ElevatorButton>().targetDestination = elevatorKeypad.targetDestination[i];
				}
			} else {
				if (elevatorKeypad.buttons[i].activeSelf) elevatorKeypad.buttons[i].SetActive(false);
			}
		}
		elevatorKeypad.currentFloor = currentFloor;
		elevatorKeypad.activeKeypad = ke.gameObject;
		elevatorKeypad.SetCurrentFloor();
		if (lastDataSideRH) {
			MFDManager.a.OpenTab(4,true,TabMSG.Elevator,0,handedness.RightHand);
		} else {
			MFDManager.a.OpenTab(4,true,TabMSG.Elevator,0,handedness.LeftHand);
		}
		linkedElevatorDoor = linkedDoor;
		objectInUsePos = tetherPoint;
		tetheredKeypadElevator = ke;
		usingObject = true;
	}

	public void OpenFullMap() {

	}

	public void UpdateHUDAmmoCounts(int amount) {
		if (wepmagCounterLH != null) wepmagCounterLH.UpdateDigits(amount);
		if (wepmagCounterRH != null) wepmagCounterRH.UpdateDigits(amount);
	}

	public void DisableSearchItemImage(int index) {
		MFDManager.a.searchItemImagesLH[index].SetActive(false);
		//MFDManager.a.searchItemImagesRH[index].SetActive(false);
	}

	public void NotifySearchThatSearableWasDestroyed() {
		if (tetheredSearchable != null) {
			tetheredSearchable.ResetSearchable(true); // reset the actual object
			// reset the HUD contents
			// if (headerTextRH.activeSelf) {
				// headerTextRH.SetActive(false);
				// headerText_textRH.enabled = false;
				// headerText_textRH.text = System.String.Empty;
				// noItemsTextRH.SetActive(false);
				// noItemsTextRH.GetComponent<Text>().enabled = false;
				// for (int i=0;i<4;i++) {
					// searchItemImagesRH[i].SetActive(false);
					// searchItemImagesRH[i].GetComponent<Image>().overrideSprite = Const.a.searchItemIconSprites[200];
					// searchContainerRH.contents[i] = -1;
					// searchContainerRH.customIndex[i] = -1;
				// }
			// }
			if (headerTextLH.activeSelf) {
				headerTextLH.SetActive(false);
				headerText_textLH.enabled = false;
				headerText_textLH.text = System.String.Empty;
				noItemsTextLH.SetActive(false);
				noItemsTextLH.GetComponent<Text>().enabled = false;
				for (int i=0;i<4;i++) {
					searchItemImagesLH[i].SetActive(false);
					searchItemImagesLH[i].GetComponent<Image>().overrideSprite = Const.a.searchItemIconSprites[200];
					searchContainerLH.contents[i] = -1;
					searchContainerLH.customIndex[i] = -1;
				}
			}
			tetheredSearchable = null;
		}
	}

	public void ReturnToLastTab(bool isRightHand) {
		usingObject = false;
		objectInUsePos = new Vector3(999f,999f,999f); // out of bounds
		if (isRightHand) {
			rightTC.ReturnToLastTab();
		} else {
			leftTC.ReturnToLastTab();
		}
	}
}