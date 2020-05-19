using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CenterTabButtons : MonoBehaviour {
	[SerializeField] private CenterMFDTabs TabManager = null; // assign in the editor
	[SerializeField] private Button MainTabButton = null; // assign in the editor
	[SerializeField] private Button HardwareTabButton = null; // assign in the editor
	[SerializeField] private Button GeneralTabButton = null; // assign in the editor
	[SerializeField] private Button SoftwareTabButton = null; // assign in the editor

	[SerializeField] private Sprite MFDSprite = null; // assign in the editor
	[SerializeField] private Sprite MFDSpriteSelected = null; // assign in the editor
	[SerializeField] private Sprite MFDSpriteNotification = null; // assign in the editor
	[SerializeField] private AudioSource TabSFX = null; // assign in the editor
	[SerializeField] private AudioClip TabSFXClip = null; // assign in the editor
	[SerializeField] private int curTab = 0;
	[SerializeField] private float tickTime = 0.5f;
	[SerializeField] private int numTicks = 14;
	private bool[] tabNotified;
	private float tickFinished;
	private bool[] highlightStatus;
	private int[] highlightTickCount;

	void Start () {
		tabNotified = new bool[] {false, false, false, false};
		tickFinished = PauseScript.a.relativeTime;
		tabNotified = new bool[] {false, false, false, false};
		highlightStatus = new bool[] {false, false, false, false};
		highlightTickCount = new int[] {0,0,0,0};
	}

	void Update () {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			if (tickFinished < PauseScript.a.relativeTime) {
				for (int i=0;i<4;i++) {
					if (tabNotified[i]) {
						ToggleHighlightOnButton(i);
					}
				}
				tickFinished = PauseScript.a.relativeTime + tickTime;
			}
		}
	}

	void ToggleHighlightOnButton (int buttonIndex) {
		Image buttonImage = null;
		switch (buttonIndex) {
			case 0: if (buttonImage != MainTabButton.image) buttonImage = MainTabButton.image; break;
			case 1: if (buttonImage != HardwareTabButton.image) buttonImage = HardwareTabButton.image; break;
			case 2: if (buttonImage != GeneralTabButton.image) buttonImage = GeneralTabButton.image; break;
			case 3: if (buttonImage != SoftwareTabButton.image) buttonImage = SoftwareTabButton.image; break;
		}

		if (buttonImage == null) return;
		if (highlightStatus[buttonIndex]) {
			if (buttonImage.overrideSprite != MFDSpriteNotification) buttonImage.overrideSprite = MFDSpriteNotification;
		} else {
			if (curTab == buttonIndex) {
				if (buttonImage.overrideSprite != MFDSpriteSelected) buttonImage.overrideSprite = MFDSpriteSelected;
			} else {
				if (buttonImage.overrideSprite != MFDSprite) buttonImage.overrideSprite = MFDSprite;
			}
		}

		highlightTickCount[buttonIndex]++;
		highlightStatus[buttonIndex] = (!highlightStatus[buttonIndex]);

		if (highlightTickCount[buttonIndex] >= numTicks) {
			highlightStatus[buttonIndex] = false;
			highlightTickCount[buttonIndex] = 0;
			tabNotified[buttonIndex] = false; // stop blinking
			if (curTab == buttonIndex) {
				if (buttonImage.overrideSprite != MFDSpriteSelected) buttonImage.overrideSprite = MFDSpriteSelected; // If we are on this tab, return to selected
			} else {
				if (buttonImage.overrideSprite != MFDSprite) buttonImage.overrideSprite = MFDSprite; // Return to normal
			}
		}
	}

	public void NotifyToCenterTab(int tabNum) {
		//if (curTab == tabNum) return;
		tabNotified[tabNum] = true;
		tickFinished = PauseScript.a.relativeTime + tickTime;
		ToggleHighlightOnButton (tabNum);
	}

	public void TabButtonClick (int tabNum) {
		TabSFX.PlayOneShot(TabSFXClip);
		TabButtonClickSilent(tabNum,false);
	}

	public void TabButtonClickSilent (int tabNum, bool forceOn) {
		bool wasActive = false;

		switch (tabNum) {
		case 0:
			wasActive = TabManager.MainTab.activeInHierarchy;
			TabManager.DisableAllTabs();
			if (curTab == 0) {
				if (wasActive && !forceOn) {
					break;
				} else {
					TabManager.MainTab.SetActive(true);
					break;
				}
			}
			MainTabButton.image.overrideSprite = MFDSpriteSelected;
			TabManager.DisableAllTabs();
			TabManager.MainTab.SetActive(true);
			HardwareTabButton.image.overrideSprite = MFDSprite;
			GeneralTabButton.image.overrideSprite = MFDSprite;
			SoftwareTabButton.image.overrideSprite = MFDSprite;
			curTab = 0;
			break;
		case 1:
			wasActive = TabManager.HardwareTab.activeInHierarchy;
			TabManager.DisableAllTabs();
			if (curTab == 1) {
				if (wasActive && !forceOn) {
					break;
				} else {
					TabManager.HardwareTab.SetActive(true);
					break;
				}
			}
			HardwareTabButton.image.overrideSprite = MFDSpriteSelected;
			TabManager.DisableAllTabs();
			TabManager.HardwareTab.SetActive(true);
			MainTabButton.image.overrideSprite = MFDSprite;
			GeneralTabButton.image.overrideSprite = MFDSprite;
			SoftwareTabButton.image.overrideSprite = MFDSprite;
			curTab = 1;
			break;
		case 2:
			wasActive = TabManager.GeneralTab.activeInHierarchy;
			TabManager.DisableAllTabs();
			if (curTab == 2) {
				if (wasActive && !forceOn) {
					break;
				} else {
					TabManager.GeneralTab.SetActive(true);
					break;
				}
			}
			GeneralTabButton.image.overrideSprite = MFDSpriteSelected;
			TabManager.DisableAllTabs();
			TabManager.GeneralTab.SetActive(true);
			MainTabButton.image.overrideSprite = MFDSprite;
			HardwareTabButton.image.overrideSprite = MFDSprite;
			SoftwareTabButton.image.overrideSprite = MFDSprite;
			curTab = 2;
			break;
		case 3:
			wasActive = TabManager.SoftwareTab.activeInHierarchy;
			TabManager.DisableAllTabs();
			if (curTab == 3) {
				if (wasActive && !forceOn) {
					break;
				} else {
					TabManager.SoftwareTab.SetActive(true);
					break;
				}
			}
			SoftwareTabButton.image.overrideSprite = MFDSpriteSelected;
			TabManager.DisableAllTabs();
			TabManager.SoftwareTab.SetActive(true);
			MainTabButton.image.overrideSprite = MFDSprite;
			HardwareTabButton.image.overrideSprite = MFDSprite;
			GeneralTabButton.image.overrideSprite = MFDSprite;
			curTab = 3;
			break;
		case 4:
			TabManager.DisableAllTabs();
			TabManager.DataReaderContentTab.SetActive(true);
			TabManager.DataReaderContentTab.GetComponent<MultiMediaTabManager>().OpenLogTableContents();
			MainTabButton.image.overrideSprite = MFDSprite;
			HardwareTabButton.image.overrideSprite = MFDSprite;
			GeneralTabButton.image.overrideSprite = MFDSprite;
			SoftwareTabButton.image.overrideSprite = MFDSprite;
			curTab = 4;
			break;
		}
	}
}
