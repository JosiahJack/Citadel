using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class HardwareButton : MonoBehaviour {
	//[SerializeField] private GameObject iconman;
	[SerializeField] private AudioSource SFX = null; // assign in the editor
	[SerializeField] private AudioClip SFXClip = null; // assign in the editor
	[SerializeField] private AudioClip SFXClipDeactivate = null; // assign in the editor
	//public CenterTabButtons ctb;
	public Sprite buttonDeactive;
	public Sprite buttonActive1;
	public Sprite buttonActive2;
	public Sprite buttonActive3;
	public Sprite buttonActive4;
	public HardwareInventory hwi;
	public HardwareInvCurrent hwc;
	public int referenceIndex;
	public int ref14Index;
	private bool toggleState = false;
	private Button butn;
	private float defaultZero = 0f;
	private float brightness = 0f;
	public GameObject sensaroundCenter;
	public GameObject sensaroundLH;
	public GameObject sensaroundRH;
	public GameObject sensaroundCenterCamera;
	public GameObject sensaroundLHCamera;
	public GameObject sensaroundRHCamera;
	public GameObject bioMonitorContainer;
	public Light infraredLight;
	public GameObject playerCamera;
	public GameObject gunCamera;
	public HeadMountedLantern hml;
	public PlayerEnergy pe;
	private float blinkFinished;
	private float blinkTick = 1f;
	public EmailContentsButtonsManager ecbm;
	private float beepFinished;
	private float beepTick = 3f;
	public AudioClip beepSFX;
	private int beepCount = 0;
	public GameObject ShieldActivateFX;
	public GameObject ShieldDeactivateFX;

	void Awake () {
		butn = GetComponent<Button>();
		blinkFinished = blinkTick + PauseScript.a.relativeTime;
		beepFinished = beepTick + PauseScript.a.relativeTime;
	}

	public void PtrEnter () {
		GUIState.a.isBlocking = true;
	}

	public void PtrExit () {
		GUIState.a.isBlocking = false;
	}

	void Update () {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			ListenForHardwareHotkeys();
			if (ref14Index == 2) {
				if (ecbm != null) {
					bool foundsome = false;
					for (int i=0;i<ecbm.mmLBs.Length;i++) {
						if (LogInventory.a.hasLog[ecbm.mmLBs[i].logReferenceIndex] && !LogInventory.a.readLog[ecbm.mmLBs[i].logReferenceIndex])
							foundsome = true;
					}
					if (foundsome) {
						// You've got mail!
						if (blinkFinished < PauseScript.a.relativeTime) {
							blinkFinished = blinkTick + PauseScript.a.relativeTime;
							toggleState = !toggleState;
							if (toggleState) {
								butn.image.overrideSprite = buttonActive1;
							} else {
								butn.image.overrideSprite = buttonDeactive;
							}
						}
						if (beepFinished < PauseScript.a.relativeTime && LogInventory.a.beepDone) {
							beepFinished = beepTick + PauseScript.a.relativeTime;
							beepCount++;
							if (beepCount >= 3) {
								LogInventory.a.beepDone = false;
								beepCount = 0;
							}
							if (SFX != null && beepSFX != null) SFX.PlayOneShot(beepSFX);
						}
					} else {
						butn.image.overrideSprite = buttonDeactive;
					}
				}
			}
		}
	}

	public void ChangeHardwareVersion (int ind, int verz) {
		SetVersionIconForButton (true, verz);
		switch (ind) {
		case 0:
			break;
		case 1:
			break;
		case 2:
			break;
		case 3:
			break;
		case 4:
			break;
		case 5:
			break;
		case 6:
			// Bio
			break;
		case 7:
			// Lantern
			// figure out which brightness setting to use depending on version again
			switch(verz) {
				case 0: brightness = hml.lanternVersion1Brightness; break;
				case 1: brightness = hml.lanternVersion2Brightness; break;
				case 2: brightness = hml.lanternVersion3Brightness; break;
				default: brightness = defaultZero; break;
			}

			hml.headlight.intensity = brightness; // set the light intensity per version
			break;
		case 8:
			break;
		case 9:
			break;
		case 10:
			break;
		case 11:
			break;
		case 12:
			break;
		case 13:
			break;
		}
	}

	public void ListenForHardwareHotkeys () {
		// Bio
		if (ref14Index == 6 && GetInput.a != null && HardwareInventory.a.hasHardware[6] &&  GetInput.a.Biomonitor()) {
			BioClick ();
		}

		// Sensaround
		if (ref14Index == 3 && GetInput.a != null && HardwareInventory.a.hasHardware[3] &&  GetInput.a.Sensaround()) {
			SensaroundClick ();
		}

		// Shield
		if (ref14Index == 5 && GetInput.a != null && HardwareInventory.a.hasHardware[5] &&  GetInput.a.Shield()) {
			ShieldClick ();
		}

		// Lantern
		if (ref14Index == 7 && GetInput.a != null && HardwareInventory.a.hasHardware[7] &&  GetInput.a.Lantern()) {
			LanternClick ();
		}

		// Infrared
		if (ref14Index == 11 && GetInput.a != null && HardwareInventory.a.hasHardware[11] && GetInput.a.Infrared()) {
			InfraredClick ();
		}

		// Ereader
		if (ref14Index == 2 && GetInput.a != null && HardwareInventory.a.hasHardware[2] && GetInput.a.Email()) {
			EReaderClick ();
		}

		// Booster
		if (ref14Index == 9 && GetInput.a != null && HardwareInventory.a.hasHardware[9] && GetInput.a.Booster()) {
			BoosterClick ();
		}

		// JumpJets
		if (ref14Index == 10 && GetInput.a != null && HardwareInventory.a.hasHardware[10] && GetInput.a.Jumpjets()) {
			JumpJetsClick ();
		}
	}

	public void SetVersionIconForButton(bool isOn, int verz) {
		if (verz > 3 || verz < 0 || butn == null) { Debug.Log("BUG: in SetVersionIconForButton"); return; }
		if (isOn) {
			switch (verz) {
			case 0:
				butn.image.overrideSprite = buttonActive1;
				break;
			case 1:
				butn.image.overrideSprite = buttonActive2;
				break;
			case 2:
				butn.image.overrideSprite = buttonActive3;
				break;
			case 3:
				butn.image.overrideSprite = buttonActive4;
				break;
			}
		} else {
			butn.image.overrideSprite = buttonDeactive;
		}
	}

	public void BioClick() {
		if (hwi.hardwareVersionSetting[ref14Index] == 0 && pe.energy <=0) { Const.sprint(Const.a.stringTable[314],pe.wepCur.owner); return; }
		if (toggleState) {
			SFX.PlayOneShot (SFXClipDeactivate);
			bioMonitorContainer.SetActive(false);
		} else {
			SFX.PlayOneShot (SFXClip);
			bioMonitorContainer.SetActive(true);
		}

		toggleState = !toggleState;
		hwc.hardwareIsActive [6] = toggleState;
		SetVersionIconForButton (toggleState, hwi.hardwareVersionSetting[ref14Index]);
	}

	// called by PlayerEnergy when exhausted energy to 0
	public void BioOff() {
		toggleState = false;
		hwc.hardwareIsActive [6] = toggleState;
		SetVersionIconForButton (toggleState, hwi.hardwareVersionSetting[ref14Index]);
		bioMonitorContainer.SetActive(false);
	}

	public void SensaroundClick() {
		if (pe.energy <=0) { Const.sprint(Const.a.stringTable[314],pe.wepCur.owner); return; }
		if (toggleState) {
			SFX.PlayOneShot (SFXClipDeactivate);
		} else {
			SFX.PlayOneShot (SFXClip);
		}

		toggleState = !toggleState;
		hwc.hardwareIsActive [3] = toggleState;
		SetVersionIconForButton (toggleState, hwi.hardwareVersionSetting[ref14Index]);
		if (toggleState) {
			switch (hwi.hardwareVersion [ref14Index]) {
			case 1:
				if (sensaroundCenterCamera != null) sensaroundCenterCamera.SetActive (true);
				if (sensaroundCenter != null) sensaroundCenter.SetActive (true);
				break;
			case 2:
				if (sensaroundCenterCamera != null) sensaroundCenterCamera.SetActive (true);
				if (sensaroundCenter != null) sensaroundCenter.SetActive (true);
				if (sensaroundLHCamera != null) sensaroundLHCamera.SetActive (true);
				if (sensaroundLH != null) sensaroundLH.SetActive (true);
				if (sensaroundRHCamera != null) sensaroundRHCamera.SetActive (true);
				if (sensaroundRH != null) sensaroundRH.SetActive (true);
				break;
			case 3:
				if (sensaroundCenterCamera != null) sensaroundCenterCamera.SetActive (true);
				if (sensaroundCenter != null) sensaroundCenter.SetActive (true);
				if (sensaroundLHCamera != null) sensaroundLHCamera.SetActive (true);
				if (sensaroundLH != null) sensaroundLH.SetActive (true);
				if (sensaroundRHCamera != null) sensaroundRHCamera.SetActive (true);
				if (sensaroundRH != null) sensaroundRH.SetActive (true);
				break;
			case 4:
				if (sensaroundCenterCamera != null) sensaroundCenterCamera.SetActive (true);
				if (sensaroundCenter != null) sensaroundCenter.SetActive (true);
				if (sensaroundLHCamera != null) sensaroundLHCamera.SetActive (true);
				if (sensaroundLH != null) sensaroundLH.SetActive (true);
				if (sensaroundRHCamera != null) sensaroundRHCamera.SetActive (true);
				if (sensaroundRH != null) sensaroundRH.SetActive (true);
				break;
			}
		} else {
			if (sensaroundCenterCamera != null) sensaroundCenterCamera.SetActive (false);
			if (sensaroundCenter != null) sensaroundCenter.SetActive (false);
			if (sensaroundLHCamera != null) sensaroundLHCamera.SetActive (false);
			if (sensaroundLH != null) sensaroundLH.SetActive (false);
			if (sensaroundRHCamera != null) sensaroundRHCamera.SetActive (false);
			if (sensaroundRH != null) sensaroundRH.SetActive (false);
		}
	}

	// called by PlayerEnergy when exhausted energy to 0
	public void SensaroundOff() {
		toggleState = false;
		hwc.hardwareIsActive [3] = toggleState;
		SetVersionIconForButton (toggleState, hwi.hardwareVersionSetting[ref14Index]);
		if (sensaroundCenterCamera != null) sensaroundCenterCamera.SetActive (false);
		if (sensaroundCenter != null) sensaroundCenter.SetActive (false);
		if (sensaroundLHCamera != null) sensaroundLHCamera.SetActive (false);
		if (sensaroundLH != null) sensaroundLH.SetActive (false);
		if (sensaroundRHCamera != null) sensaroundRHCamera.SetActive (false);
		if (sensaroundRH != null) sensaroundRH.SetActive (false);
	}

	public void ShieldClick() {
		if (pe.energy <=0) { Const.sprint(Const.a.stringTable[314],pe.wepCur.owner); return; }
		if (toggleState) {
			SFX.PlayOneShot (SFXClipDeactivate);
			ShieldDeactivateFX.SetActive(true);
			ShieldActivateFX.SetActive(false);
		} else {
			SFX.PlayOneShot (SFXClip);
			ShieldDeactivateFX.SetActive(false);
			ShieldActivateFX.SetActive(true);
		}

		toggleState = !toggleState;
		hwc.hardwareIsActive [5] = toggleState;
		SetVersionIconForButton (toggleState, hwi.hardwareVersionSetting[ref14Index]);
	}

	// called by PlayerEnergy when exhausted energy to 0
	public void ShieldOff() {
		//Debug.Log("ShieldOff");
		toggleState = false;
		hwc.hardwareIsActive [5] = toggleState;
		SetVersionIconForButton (toggleState, hwi.hardwareVersionSetting[ref14Index]);
		ShieldDeactivateFX.SetActive(true);
		ShieldActivateFX.SetActive(false);
	}

	public void LanternClick() {
		if (pe.energy <=0) { Const.sprint(Const.a.stringTable[314],pe.wepCur.owner); return; }
		if (toggleState) {
			SFX.PlayOneShot (SFXClipDeactivate);
		} else {
			SFX.PlayOneShot (SFXClip);
		}

		toggleState = !toggleState;
		hwc.hardwareIsActive [7] = toggleState;
		SetVersionIconForButton (toggleState, hwi.hardwareVersionSetting[ref14Index]);

		// figure out which brightness setting to use depending on version
		switch(hwi.hardwareVersionSetting[ref14Index]) {
			case 0: brightness = hml.lanternVersion1Brightness; break;
			case 1: brightness = hml.lanternVersion2Brightness; break;
			case 2: brightness = hml.lanternVersion3Brightness; break;
			default: brightness = defaultZero; break;
		}

		if (hwc.hardwareIsActive [ref14Index]) {
			hml.headlight.intensity = brightness; // set the light intensity per version
		} else {
			hml.headlight.intensity = defaultZero; // turn the light off
		}
	}

	// called by PlayerEnergy when exhausted energy to 0
	public void LanternOff() {
		toggleState = false;
		hwc.hardwareIsActive [7] = toggleState;
		SetVersionIconForButton (toggleState, hwi.hardwareVersionSetting[ref14Index]);
		hml.headlight.intensity = defaultZero; // turn the light off
	}

	public void InfraredClick() {
		if (pe.energy <=0) { Const.sprint(Const.a.stringTable[314],pe.wepCur.owner); return; }
		if (infraredLight == null) return;
		if (toggleState) {
			SFX.PlayOneShot (SFXClipDeactivate);
		} else {
			SFX.PlayOneShot (SFXClip);
		}

		toggleState = !toggleState;
		hwc.hardwareIsActive [11] = toggleState;
		SetVersionIconForButton (toggleState, hwi.hardwareVersionSetting[ref14Index]);
		if (toggleState) {
			infraredLight.enabled = true;
			playerCamera.GetComponent<Grayscale>().enabled = true;
			gunCamera.GetComponent<Grayscale>().enabled = true;
		} else {
			infraredLight.enabled = false;
			playerCamera.GetComponent<Grayscale>().enabled = false;
			gunCamera.GetComponent<Grayscale>().enabled = false;
		}
	}

	// called by PlayerMovement when exhausted energy to < 11f
	public void InfraredOff() {
		toggleState = false;
		hwc.hardwareIsActive [11] = toggleState;
		SetVersionIconForButton (toggleState, hwi.hardwareVersionSetting[ref14Index]);
		infraredLight.enabled = false;
		playerCamera.GetComponent<Grayscale>().enabled = false;
		gunCamera.GetComponent<Grayscale>().enabled = false;
	}

	public void EReaderClick () {
		toggleState = true;
		if (SFXClip != null) SFX.PlayOneShot (SFXClip);
		hwc.hardwareIsActive [2] = toggleState;
		//if (ctb != null) ctb.TabButtonClickSilent(4,false);  Moved to MFDManager
		MFDManager.a.OpenEReaderInItemsTab();
	}

	public void BoosterClick() {
		if (hwi.hardwareVersionSetting[ref14Index] == 1 && pe.energy <=0) { Const.sprint(Const.a.stringTable[314],pe.wepCur.owner); return; }
		if (toggleState) {
			SFX.PlayOneShot (SFXClipDeactivate);
		} else {
			SFX.PlayOneShot (SFXClip);
		}

		toggleState = !toggleState;
		hwc.hardwareIsActive [9] = toggleState;
		SetVersionIconForButton (toggleState, hwi.hardwareVersionSetting[ref14Index]);
	}

	// called by PlayerMovement when exhausted energy to < 11f
	public void BoosterOff() {
		toggleState = false;
		hwc.hardwareIsActive [9] = toggleState;
		SetVersionIconForButton (toggleState, hwi.hardwareVersionSetting[ref14Index]);
	}

	public void JumpJetsClick() {
		if (pe.energy <=0) { Const.sprint(Const.a.stringTable[314],pe.wepCur.owner); return; }
		if (toggleState) {
			SFX.PlayOneShot (SFXClipDeactivate);
		} else {
			SFX.PlayOneShot (SFXClip);
		}

		toggleState = !toggleState;
		hwc.hardwareIsActive [10] = toggleState;
		SetVersionIconForButton (toggleState, hwi.hardwareVersionSetting[ref14Index]);
	}

	// called by PlayerMovement when exhausted energy to < 11f
	public void JumpJetsOff() {
		toggleState = false;
		hwc.hardwareIsActive [10] = toggleState;
		SetVersionIconForButton (toggleState, hwi.hardwareVersionSetting[ref14Index]);
	}

	public void UpdateIconForVersionUpgrade() {
		toggleState = hwc.hardwareIsActive [ref14Index];
		SetVersionIconForButton (toggleState, hwi.hardwareVersionSetting[ref14Index]);
	}
}
