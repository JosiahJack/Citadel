using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TabButtons : MonoBehaviour {
	// Externally assigned, required
	public LeftMFDTabs TabManager;
	public Button WeaponTabButton;
	public Button ItemTabButton;
	public Button AutomapTabButton;
	public Button TargetTabButton;
	public Button DataTabButton;
	public Sprite MFDSprite;
	public Sprite MFDSpriteSelected;
	public bool isRH; // Assign in the editors

	public int curTab = 0;
	public int lastTab = 0;

	void Start() {
		TurnAllTabsOff();
		ItemTabButton.image.overrideSprite = MFDSprite;
		AutomapTabButton.image.overrideSprite = MFDSprite;
		TargetTabButton.image.overrideSprite = MFDSprite;
		DataTabButton.image.overrideSprite = MFDSprite;
	}

	public void TurnAllTabsOff() {
		TabManager.WeaponTab.SetActive(false);
		TabManager.ItemTab.SetActive(false);
		TabManager.AutomapTab.SetActive(false);
		TabManager.TargetTab.SetActive(false);
		TabManager.DataTab.SetActive(false);
	}

	public void SetCurrentAsLast() {
		lastTab = curTab;
	}

	public void ReturnToLastTab() {
		TabButtonClickSilent(lastTab,true);
	}

	// Only setting these from manual user actions.
	private void SetMFDLasts(int tabNum) {
		SetCurrentAsLast();
		switch (tabNum) {
			case 0: // Weapon
				MFDManager.a.lastWeaponSideRH = isRH;
				break;
			case 1: // Item
				MFDManager.a.lastItemSideRH = isRH;
				break;
			case 2: // Automap
				MFDManager.a.lastAutomapSideRH = isRH;
				break;
			case 3: // Target
				MFDManager.a.lastTargetSideRH = isRH;
				break;
			case 4: // Data
				if (MFDManager.a.tetheredSearchable != null) {
					if (isRH) MFDManager.a.lastSearchSideRH = true;
					else MFDManager.a.lastSearchSideRH = false;
				}
				MFDManager.a.lastDataSideRH = isRH;
				break;
		}
	}

	public void TabButtonClick(int tabNum) { // For click events.
		MFDManager.a.mouseClickHeldOverGUI = true;
		TabButtonAction(tabNum);
		SetMFDLasts(tabNum);
	}

	public void TabButtonAction(int tabNum) { // For keyboard events.
		Utils.PlayUIOneShotSavable(97);
		TabButtonClickSilent(tabNum,false);
		SetMFDLasts(tabNum);
	}

	// For automatic internal events not directly from player input.
	public void TabButtonClickSilent(int tabNum,bool overrideToggling) {
		switch (tabNum) {
		case 0:
			if (curTab == 0) {
				if (TabManager.WeaponTab.activeSelf == true && !overrideToggling) {
					TabManager.WeaponTab.SetActive(false);
					break;
				} else {
					TabManager.WeaponTab.SetActive(true);
					break;
				}
			}
			WeaponTabButton.image.overrideSprite = MFDSpriteSelected;
			TabManager.WeaponTab.SetActive(true);
			TabManager.ItemTab.SetActive(false);
			TabManager.AutomapTab.SetActive(false);
			TabManager.TargetTab.SetActive(false);
			TabManager.DataTab.SetActive(false);
			ItemTabButton.image.overrideSprite = MFDSprite;
			AutomapTabButton.image.overrideSprite = MFDSprite;
			TargetTabButton.image.overrideSprite = MFDSprite;
			DataTabButton.image.overrideSprite = MFDSprite;
			curTab = 0;
			break;
		case 1:
			if (curTab == 1) {
				if (TabManager.ItemTab.activeSelf == true && !overrideToggling) {
					TabManager.ItemTab.SetActive(false);
					break;
				} else {
					TabManager.ItemTab.SetActive(true);
					break;
				}
			}
			ItemTabButton.image.overrideSprite = MFDSpriteSelected;
			TabManager.WeaponTab.SetActive(false);
			TabManager.ItemTab.SetActive(true);
			TabManager.AutomapTab.SetActive(false);
			TabManager.TargetTab.SetActive(false);
			TabManager.DataTab.SetActive(false);
			WeaponTabButton.image.overrideSprite = MFDSprite;
			AutomapTabButton.image.overrideSprite = MFDSprite;
			TargetTabButton.image.overrideSprite = MFDSprite;
			DataTabButton.image.overrideSprite = MFDSprite;
			curTab = 1;
			break;
		case 2:
			if (curTab == 2) {
				if (TabManager.AutomapTab.activeSelf == true && !overrideToggling) {
					TabManager.AutomapTab.SetActive(false);
					break;
				} else {
					TabManager.AutomapTab.SetActive(true);
					break;
				}
			}
			AutomapTabButton.image.overrideSprite = MFDSpriteSelected;
			TabManager.WeaponTab.SetActive(false);
			TabManager.ItemTab.SetActive(false);
			TabManager.AutomapTab.SetActive(true);
			TabManager.TargetTab.SetActive(false);
			TabManager.DataTab.SetActive(false);
			WeaponTabButton.image.overrideSprite = MFDSprite;
			ItemTabButton.image.overrideSprite = MFDSprite;
			TargetTabButton.image.overrideSprite = MFDSprite;
			DataTabButton.image.overrideSprite = MFDSprite;
			curTab = 2;
			break;
		case 3:
			if (curTab == 3) {
				if (TabManager.TargetTab.activeSelf == true && !overrideToggling) {
					TabManager.TargetTab.SetActive(false);
					break;
				} else {
					TabManager.TargetTab.SetActive(true);
					break;
				}
			}
			TargetTabButton.image.overrideSprite = MFDSpriteSelected;
			TabManager.WeaponTab.SetActive(false);
			TabManager.ItemTab.SetActive(false);
			TabManager.AutomapTab.SetActive(false);
			TabManager.TargetTab.SetActive(true);
			TabManager.DataTab.SetActive(false);
			WeaponTabButton.image.overrideSprite = MFDSprite;
			ItemTabButton.image.overrideSprite = MFDSprite;
			AutomapTabButton.image.overrideSprite = MFDSprite;
			DataTabButton.image.overrideSprite = MFDSprite;
			curTab = 3;
			break;
		case 4:
			if (curTab == 4) {
				if (TabManager.DataTab.activeSelf == true && !overrideToggling) {
					TabManager.DataTab.SetActive(false);
					break;
				} else {
					TabManager.DataTab.SetActive(true);
					break;
				}
			}
			DataTabButton.image.overrideSprite = MFDSpriteSelected;
			TabManager.WeaponTab.SetActive(false);
			TabManager.ItemTab.SetActive(false);
			TabManager.AutomapTab.SetActive(false);
			TabManager.TargetTab.SetActive(false);
			TabManager.DataTab.SetActive(true);
			WeaponTabButton.image.overrideSprite = MFDSprite;
			ItemTabButton.image.overrideSprite = MFDSprite;
			AutomapTabButton.image.overrideSprite = MFDSprite;
			TargetTabButton.image.overrideSprite = MFDSprite;
			curTab = 4;
			break;
		}
	}
}
