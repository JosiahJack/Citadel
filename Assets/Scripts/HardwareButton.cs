using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class HardwareButton : MonoBehaviour {
	//[SerializeField] private GameObject iconman;
	[SerializeField] private AudioSource SFX = null; // assign in the editor
	[SerializeField] private AudioClip SFXClip = null; // assign in the editor
	[SerializeField] private AudioClip SFXClipDeactivate = null; // assign in the editor
	public CenterTabButtons ctb;
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
	public Light infraredLight;
	public GameObject playerCamera;
	public HeadMountedLantern hml;

	void Awake () {
		butn = GetComponent<Button>();
	}

	public void PtrEnter () {
		GUIState.a.isBlocking = true;
	}

	public void PtrExit () {
		GUIState.a.isBlocking = false;
	}

	void Update () {
		ListenForHardwareHotkeys();
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
				case 1: brightness = hml.lanternVersion1Brightness; break;
				case 2: brightness = hml.lanternVersion2Brightness; break;
				case 3: brightness = hml.lanternVersion3Brightness; break;
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
		if (GetInput.a != null && HardwareInventory.a.hasHardware[6] &&  GetInput.a.Biomonitor()) {
			BioClick ();
		}

		// Sensaround
		if (GetInput.a != null && HardwareInventory.a.hasHardware[3] &&  GetInput.a.Sensaround()) {
			SensaroundClick ();
		}

		// Shield
		if (GetInput.a != null && HardwareInventory.a.hasHardware[5] &&  GetInput.a.Shield()) {
			ShieldClick ();
		}

		// Lantern
		if (GetInput.a != null && HardwareInventory.a.hasHardware[7] &&  GetInput.a.Lantern()) {
			LanternClick ();
		}

		// Infrared
		if (GetInput.a != null && HardwareInventory.a.hasHardware[11] && GetInput.a.Infrared()) {
			InfraredClick ();
		}

		// Ereader
		if (GetInput.a != null && HardwareInventory.a.hasHardware[2] && GetInput.a.Email()) {
			EReaderClick ();
		}

		// Booster
		if (GetInput.a != null && HardwareInventory.a.hasHardware[9] && GetInput.a.Booster()) {
			BoosterClick ();
		}

		// JumpJets
		if (GetInput.a != null && HardwareInventory.a.hasHardware[10] && GetInput.a.Jumpjets()) {
			JumpJetsClick ();
		}
	}

	public void SetVersionIconForButton(bool isOn, int verz) {
		if (isOn) {
			switch (verz) {
			case 0:
				Const.sprint ("ERROR: Hardware version set to 0!! for " + Const.a.useableItemsNameText [referenceIndex], Const.a.allPlayers);
				break;
			case 1:
				butn.image.overrideSprite = buttonActive1;
				break;
			case 2:
				butn.image.overrideSprite = buttonActive2;
				break;
			case 3:
				butn.image.overrideSprite = buttonActive3;
				break;
			case 4:
				butn.image.overrideSprite = buttonActive4;
				break;
			}
		} else {
			butn.image.overrideSprite = buttonDeactive;
		}
	}

	public void BioClick() {
		if (toggleState) {
			SFX.PlayOneShot (SFXClipDeactivate);
		} else {
			SFX.PlayOneShot (SFXClip);
		}

		toggleState = !toggleState;
		hwc.hardwareIsActive [6] = toggleState;
		SetVersionIconForButton (toggleState, hwi.hardwareVersionSetting[ref14Index]);
	}

	public void SensaroundClick() {
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
				sensaroundCenterCamera.SetActive (true);
				sensaroundCenter.SetActive (true);
				break;
			case 2:
				sensaroundCenterCamera.SetActive (true);
				sensaroundCenter.SetActive (true);
				sensaroundLHCamera.SetActive (true);
				sensaroundLH.SetActive (true);
				sensaroundRHCamera.SetActive (true);
				sensaroundRH.SetActive (true);
				break;
			case 3:
				sensaroundCenterCamera.SetActive (true);
				sensaroundCenter.SetActive (true);
				sensaroundLHCamera.SetActive (true);
				sensaroundLH.SetActive (true);
				sensaroundRHCamera.SetActive (true);
				sensaroundRH.SetActive (true);
				break;
			case 4:
				sensaroundCenterCamera.SetActive (true);
				sensaroundCenter.SetActive (true);
				sensaroundLHCamera.SetActive (true);
				sensaroundLH.SetActive (true);
				sensaroundRHCamera.SetActive (true);
				sensaroundRH.SetActive (true);
				break;
			}
		} else {
			sensaroundCenterCamera.SetActive (false);
			sensaroundCenter.SetActive (false);
			sensaroundLHCamera.SetActive (false);
			sensaroundLH.SetActive (false);
			sensaroundRHCamera.SetActive (false);
			sensaroundRH.SetActive (false);
		}
	}

	public void ShieldClick() {
		if (toggleState) {
			SFX.PlayOneShot (SFXClipDeactivate);
		} else {
			SFX.PlayOneShot (SFXClip);
		}

		toggleState = !toggleState;
		hwc.hardwareIsActive [5] = toggleState;
		SetVersionIconForButton (toggleState, hwi.hardwareVersionSetting[ref14Index]);
	}

	public void LanternClick() {
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
		case 1: brightness = hml.lanternVersion1Brightness; break;
		case 2: brightness = hml.lanternVersion2Brightness; break;
		case 3: brightness = hml.lanternVersion3Brightness; break;
		default: brightness = defaultZero; break;
		}

		if (hwc.hardwareIsActive [ref14Index]) {
			hml.headlight.intensity = brightness; // set the light intensity per version
		} else {
			hml.headlight.intensity = defaultZero; // turn the light off
		}
	}

	public void InfraredClick() {
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
		} else {
			infraredLight.enabled = false;
			playerCamera.GetComponent<Grayscale>().enabled = false;
		}
	}

	public void EReaderClick () {
		if (toggleState) {
			SFX.PlayOneShot (SFXClipDeactivate);
		} else {
			SFX.PlayOneShot (SFXClip);
		}

		toggleState = !toggleState;
		hwc.hardwareIsActive [2] = toggleState;
		if (toggleState) {
			butn.image.overrideSprite = buttonActive1;
		} else {
			butn.image.overrideSprite = buttonDeactive;
		}
		if (ctb != null) ctb.TabButtonClickSilent(4);
		MFDManager.a.OpenEReaderInItemsTab();
	}

	public void BoosterClick() {
		if (toggleState) {
			SFX.PlayOneShot (SFXClipDeactivate);
		} else {
			SFX.PlayOneShot (SFXClip);
		}

		toggleState = !toggleState;
		hwc.hardwareIsActive [9] = toggleState;
		SetVersionIconForButton (toggleState, hwi.hardwareVersionSetting[ref14Index]);
	}

	public void JumpJetsClick() {
		if (toggleState) {
			SFX.PlayOneShot (SFXClipDeactivate);
		} else {
			SFX.PlayOneShot (SFXClip);
		}

		toggleState = !toggleState;
		hwc.hardwareIsActive [10] = toggleState;
		SetVersionIconForButton (toggleState, hwi.hardwareVersionSetting[ref14Index]);
	}
}
