using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CenterTabButtonsScript : MonoBehaviour {
	[SerializeField] private CenterMFDTabs TabManager = null; // assign in the editor
	[SerializeField] private Button MainTabButton = null; // assign in the editor
	[SerializeField] private Button HardwareTabButton = null; // assign in the editor
	[SerializeField] private Button GeneralTabButton = null; // assign in the editor
	[SerializeField] private Button SoftwareTabButton = null; // assign in the editor
	[SerializeField] private Sprite MFDSprite = null; // assign in the editor
	[SerializeField] private Sprite MFDSpriteSelected = null; // assign in the editor
	[SerializeField] private AudioSource TabSFX = null; // assign in the editor
	[SerializeField] private AudioClip TabSFXClip = null; // assign in the editor
	private int curTab = 0;
	
	public void TabButtonClick (int tabNum) {
		TabSFX.PlayOneShot(TabSFXClip);
		switch (tabNum) {
		case 0:
			if (curTab == 0) {
				if (TabManager.MainTab.activeSelf == true) {
					TabManager.MainTab.SetActive(false);
					break;
				} else {
					TabManager.MainTab.SetActive(true);
					break;
				}
			}
			MainTabButton.image.overrideSprite = MFDSpriteSelected;
			TabManager.MainTab.SetActive(true);
			TabManager.HardwareTab.SetActive(false);
			TabManager.GeneralTab.SetActive(false);
			TabManager.SoftwareTab.SetActive(false);
			TabManager.EmailTab.SetActive(false);
			TabManager.DataReaderContentTab.SetActive(false);
			HardwareTabButton.image.overrideSprite = MFDSprite;
			GeneralTabButton.image.overrideSprite = MFDSprite;
			SoftwareTabButton.image.overrideSprite = MFDSprite;
			curTab = 0;
			break;
		case 1:
			if (curTab == 1) {
				if (TabManager.HardwareTab.activeSelf == true) {
					TabManager.HardwareTab.SetActive(false);
					break;
				} else {
					TabManager.HardwareTab.SetActive(true);
					break;
				}
			}
			HardwareTabButton.image.overrideSprite = MFDSpriteSelected;
			TabManager.MainTab.SetActive(false);
			TabManager.HardwareTab.SetActive(true);
			TabManager.GeneralTab.SetActive(false);
			TabManager.SoftwareTab.SetActive(false);
			TabManager.EmailTab.SetActive(false);
			TabManager.DataReaderContentTab.SetActive(false);
			MainTabButton.image.overrideSprite = MFDSprite;
			GeneralTabButton.image.overrideSprite = MFDSprite;
			SoftwareTabButton.image.overrideSprite = MFDSprite;
			curTab = 1;
			break;
		case 2:
			if (curTab == 2) {
				if (TabManager.GeneralTab.activeSelf == true) {
					TabManager.GeneralTab.SetActive(false);
					break;
				} else {
					TabManager.GeneralTab.SetActive(true);
					break;
				}
			}
			GeneralTabButton.image.overrideSprite = MFDSpriteSelected;
			TabManager.MainTab.SetActive(false);
			TabManager.HardwareTab.SetActive(false);
			TabManager.GeneralTab.SetActive(true);
			TabManager.SoftwareTab.SetActive(false);
			TabManager.EmailTab.SetActive(false);
			TabManager.DataReaderContentTab.SetActive(false);
			MainTabButton.image.overrideSprite = MFDSprite;
			HardwareTabButton.image.overrideSprite = MFDSprite;
			SoftwareTabButton.image.overrideSprite = MFDSprite;
			curTab = 2;
			break;
		case 3:
			if (curTab == 3) {
				if (TabManager.SoftwareTab.activeSelf == true) {
					TabManager.SoftwareTab.SetActive(false);
					break;
				} else {
					TabManager.SoftwareTab.SetActive(true);
					break;
				}
			}
			SoftwareTabButton.image.overrideSprite = MFDSpriteSelected;
			TabManager.MainTab.SetActive(false);
			TabManager.HardwareTab.SetActive(false);
			TabManager.GeneralTab.SetActive(false);
			TabManager.SoftwareTab.SetActive(true);
			TabManager.EmailTab.SetActive(false);
			TabManager.DataReaderContentTab.SetActive(false);
			MainTabButton.image.overrideSprite = MFDSprite;
			HardwareTabButton.image.overrideSprite = MFDSprite;
			GeneralTabButton.image.overrideSprite = MFDSprite;
			curTab = 3;
			break;
		case 4:
			if (curTab == 4) {
				if (TabManager.EmailTab.activeSelf == true) {
					TabManager.EmailTab.SetActive(false);
					break;
				} else {
					TabManager.EmailTab.SetActive(true);
					break;
				}
			}
			SoftwareTabButton.image.overrideSprite = MFDSpriteSelected;
			TabManager.MainTab.SetActive(false);
			TabManager.HardwareTab.SetActive(false);
			TabManager.GeneralTab.SetActive(false);
			TabManager.SoftwareTab.SetActive(false);
			TabManager.EmailTab.SetActive(true);
			TabManager.DataReaderContentTab.SetActive(false);
			MainTabButton.image.overrideSprite = MFDSprite;
			HardwareTabButton.image.overrideSprite = MFDSprite;
			GeneralTabButton.image.overrideSprite = MFDSprite;
			curTab = 4;
			break;
		case 5:
			if (curTab == 5) {
				if (TabManager.DataReaderContentTab.activeSelf == true) {
					TabManager.DataReaderContentTab.SetActive(false);
					break;
				} else {
					TabManager.DataReaderContentTab.SetActive(true);
					break;
				}
			}
			SoftwareTabButton.image.overrideSprite = MFDSpriteSelected;
			TabManager.MainTab.SetActive(false);
			TabManager.HardwareTab.SetActive(false);
			TabManager.GeneralTab.SetActive(false);
			TabManager.SoftwareTab.SetActive(false);
			TabManager.DataReaderContentTab.SetActive(true);
			TabManager.EmailTab.SetActive(false);
			MainTabButton.image.overrideSprite = MFDSprite;
			HardwareTabButton.image.overrideSprite = MFDSprite;
			GeneralTabButton.image.overrideSprite = MFDSprite;
			curTab = 5;
			break;
		}
	}
}
