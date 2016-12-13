using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TabButtonsScript : MonoBehaviour {
	[SerializeField] private LeftMFDTabs TabManager = null; // assign in the editor
	[SerializeField] private Button WeaponTabButton = null; // assign in the editor
	[SerializeField] private Button ItemTabButton = null; // assign in the editor
	[SerializeField] private Button AutomapTabButton = null; // assign in the editor
	[SerializeField] private Button TargetTabButton = null; // assign in the editor
	[SerializeField] private Button DataTabButton = null; // assign in the editor
	[SerializeField] private Sprite MFDSprite = null; // assign in the editor
	[SerializeField] private Sprite MFDSpriteSelected = null; // assign in the editor
	[SerializeField] private AudioSource TabSFX = null; // assign in the editor
	[SerializeField] private AudioClip TabSFXClip = null; // assign in the editor
	public int curTab = 0;
	public int lastTab = 0;

	public void SetCurrentAsLast () {
		lastTab = curTab;
	}

	public void ReturnToLastTab () {
		TabButtonClickSilent(lastTab,true);
	}

	public void TabButtonClick (int tabNum) {
		TabSFX.PlayOneShot(TabSFXClip);
		TabButtonClickSilent(tabNum,false);
	}

	public void TabButtonClickSilent (int tabNum,bool overrideToggling) {
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
