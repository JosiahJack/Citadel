using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class HardwareButton : MonoBehaviour {
	public Button[] buttons;
	public Sprite[] buttonDeactive;
	public Sprite[] buttonActive1;
	public Sprite[] buttonActive2;
	public Sprite[] buttonActive3;
	public Sprite[] buttonActive4;
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
	public Light headlight;
	public PlayerEnergy pe;
	public EmailContentsButtonsManager ecbm;
	public AudioClip beepSFX;
	public GameObject ShieldActivateFX;
	public GameObject ShieldDeactivateFX;
	public AudioClip[] SFXClip;
	public AudioClip[] SFXClipDeactivate;

	// Hw referenceIndex, ref14Index, button index
	// Bio 27,6, 0
	// Sen 24,3, 1
	// Lan 28,7, 2
	// Shi 26,5, 3
	// Nig 32,11,4
	// Ere 23,2, 5
	// Boo 30,9, 6
	// Jum 31,10,7

	[HideInInspector] public AudioSource SFX;
	private float defaultZero = 0f;
	private float brightness = 0f;
	private float lanternVersion1Brightness = 2.5f;
	private float lanternVersion2Brightness = 4;
	private float lanternVersion3Brightness = 5;

	void Awake () {
		SFX = GetComponent<AudioSource>();
		infraredLight.enabled = false;
		playerCamera.GetComponent<Grayscale>().enabled = false;
		gunCamera.GetComponent<Grayscale>().enabled = false;
		sensaroundCenterCamera.GetComponent<Grayscale>().enabled = false;
		sensaroundLHCamera.GetComponent<Grayscale>().enabled = false;
		sensaroundRHCamera.GetComponent<Grayscale>().enabled = false;
	}

	public void ListenForHardwareHotkeys () {
		if (Inventory.a.hasHardware[6] && GetInput.a.Biomonitor()) BioClick();
		if (Inventory.a.hasHardware[3] && GetInput.a.Sensaround()) SensaroundClick();
		if (Inventory.a.hasHardware[5] && GetInput.a.Shield())     ShieldClick();
		if (Inventory.a.hasHardware[7] && GetInput.a.Lantern())    LanternClick();
		if (Inventory.a.hasHardware[11]&& GetInput.a.Infrared())   InfraredClick();
		if (Inventory.a.hasHardware[2] && GetInput.a.Email())      EReaderClick();
		if (Inventory.a.hasHardware[9] && GetInput.a.Booster())    BoosterClick();
		if (Inventory.a.hasHardware[10]&& GetInput.a.Jumpjets())   JumpJetsClick();
	}

	// 0 = bio, 1 = sen, 2 = lan, 3 = shi, 4 = nig, 5 = ere, 6 = boo, 7 = jum
	public void SetVersionIconForButton(bool isOn, int verz, int button8Index) {
		if (button8Index < 0 || button8Index > 7) button8Index = 0;
		if (isOn) {
			switch (verz) {
			case 0:
				buttons[button8Index].image.overrideSprite = buttonActive1[button8Index];
				break;
			case 1:
				buttons[button8Index].image.overrideSprite = buttonActive2[button8Index];
				break;
			case 2:
				buttons[button8Index].image.overrideSprite = buttonActive3[button8Index];
				break;
			case 3:
				buttons[button8Index].image.overrideSprite = buttonActive4[button8Index];
				break;
			default:
				buttons[button8Index].image.overrideSprite = buttonActive4[button8Index];
				break;
			}
		} else {
			buttons[button8Index].image.overrideSprite = buttonDeactive[button8Index];
		}
	}

	public void BioClick() {
		if (Inventory.a.hardwareVersionSetting[6] == 0 && pe.energy <=0) { Const.sprint(Const.a.stringTable[314],pe.wepCur.owner); return; }
		if (Inventory.a.hardwareIsActive[6]) {
			SFX.PlayOneShot (SFXClipDeactivate[0]);
			bioMonitorContainer.SetActive(false);
		} else {
			SFX.PlayOneShot (SFXClip[0]);
			bioMonitorContainer.SetActive(true);
		}
		Inventory.a.hardwareIsActive[6] = !Inventory.a.hardwareIsActive[6];
		SetVersionIconForButton(Inventory.a.hardwareIsActive[6], Inventory.a.hardwareVersionSetting[6],0);
	}

	// called by PlayerEnergy when exhausted energy to 0
	public void BioOff() {
		Inventory.a.hardwareIsActive[6] = false;
		SetVersionIconForButton(Inventory.a.hardwareIsActive[6], Inventory.a.hardwareVersionSetting[6],0);
		bioMonitorContainer.SetActive(false);
	}

	public void ActivateSensaroundCenter() {
		if (sensaroundCenterCamera != null) sensaroundCenterCamera.SetActive (true);
		if (sensaroundCenter != null) sensaroundCenter.SetActive (true);
	}

	public void ActivateSensaroundSides() {
		if (sensaroundLHCamera != null) sensaroundLHCamera.SetActive (true);
		if (sensaroundLH != null) sensaroundLH.SetActive (true);
		if (sensaroundRHCamera != null) sensaroundRHCamera.SetActive (true);
		if (sensaroundRH != null) sensaroundRH.SetActive (true);
	}

	public void DeactivateSensaround() {
		if (sensaroundCenterCamera != null) sensaroundCenterCamera.SetActive (false);
		if (sensaroundCenter != null) sensaroundCenter.SetActive (false);
		if (sensaroundLHCamera != null) sensaroundLHCamera.SetActive (false);
		if (sensaroundLH != null) sensaroundLH.SetActive (false);
		if (sensaroundRHCamera != null) sensaroundRHCamera.SetActive (false);
		if (sensaroundRH != null) sensaroundRH.SetActive (false);
	}

	public void SensaroundClick() {
		if (pe.energy <=0) { Const.sprint(Const.a.stringTable[314],pe.wepCur.owner); return; }
		if (Inventory.a.hardwareIsActive[3]) {
			SFX.PlayOneShot (SFXClipDeactivate[1]);
		} else {
			SFX.PlayOneShot (SFXClip[1]);
		}
		Inventory.a.hardwareIsActive[3] = !Inventory.a.hardwareIsActive[3];
		SetVersionIconForButton(Inventory.a.hardwareIsActive[3], Inventory.a.hardwareVersionSetting[3],1);
		if (Inventory.a.hardwareIsActive[3]) {
			if (Inventory.a.hardwareVersion[3] == 1) {
				ActivateSensaroundCenter(); // Only center on version 1.
			} else {
				ActivateSensaroundCenter();
				ActivateSensaroundSides();
			}
		} else {
			DeactivateSensaround();
		}
	}

	// called by PlayerEnergy when exhausted energy to 0
	public void SensaroundOff() {
		Inventory.a.hardwareIsActive[3] = false;
		SetVersionIconForButton(Inventory.a.hardwareIsActive[3], Inventory.a.hardwareVersionSetting[3],1);
		DeactivateSensaround();
	}

	public void ShieldClick() {
		if (pe.energy <=0) { Const.sprint(Const.a.stringTable[314],pe.wepCur.owner); return; }
		if (Inventory.a.hardwareIsActive[5]) {
			SFX.PlayOneShot (SFXClipDeactivate[3]);
			ShieldDeactivateFX.SetActive(true);
			ShieldActivateFX.SetActive(false);
		} else {
			SFX.PlayOneShot (SFXClip[3]);
			ShieldDeactivateFX.SetActive(false);
			ShieldActivateFX.SetActive(true);
		}
		Inventory.a.hardwareIsActive[5] = !Inventory.a.hardwareIsActive[5];
		SetVersionIconForButton(Inventory.a.hardwareIsActive[5], Inventory.a.hardwareVersionSetting[5],3);
	}

	// Called by PlayerEnergy when exhausted energy to 0.
	public void ShieldOff() {
		Inventory.a.hardwareIsActive[5] = false;
		SetVersionIconForButton(Inventory.a.hardwareIsActive[5], Inventory.a.hardwareVersionSetting[5],3);
		ShieldDeactivateFX.SetActive(true);
		ShieldActivateFX.SetActive(false);
	}

	public void LanternClick() {
		if (pe.energy <=0) { Const.sprint(Const.a.stringTable[314],pe.wepCur.owner); return; }
		if (Inventory.a.hardwareIsActive[7]) {
			SFX.PlayOneShot (SFXClipDeactivate[2]);
		} else {
			SFX.PlayOneShot (SFXClip[2]);
		}
		Inventory.a.hardwareIsActive[7] = !Inventory.a.hardwareIsActive[7];
		SetVersionIconForButton(Inventory.a.hardwareIsActive[7], Inventory.a.hardwareVersionSetting[7],2);

		// Figure out which brightness setting to use depending on version.
		switch(Inventory.a.hardwareVersionSetting[7]) {
			case 0: brightness = lanternVersion1Brightness; break;
			case 1: brightness = lanternVersion2Brightness; break;
			case 2: brightness = lanternVersion3Brightness; break;
			default: brightness = defaultZero; break;
		}

		if (Inventory.a.hardwareIsActive[7]) {
			headlight.intensity = brightness; // Set the light intensity per version.
		} else {
			headlight.intensity = defaultZero; // Turn the light off.
		}
	}

	// Called by PlayerEnergy when exhausted energy to 0.
	public void LanternOff() {
		Inventory.a.hardwareIsActive[7] = false;
		SetVersionIconForButton(Inventory.a.hardwareIsActive[7], Inventory.a.hardwareVersionSetting[7],2);
		headlight.intensity = defaultZero; // Turn the light off.
	}

	public void InfraredClick() {
		if (pe.energy <=0) { Const.sprint(Const.a.stringTable[314],pe.wepCur.owner); return; }
		if (Inventory.a.hardwareIsActive[11]) {
			SFX.PlayOneShot (SFXClipDeactivate[4]);
		} else {
			SFX.PlayOneShot (SFXClip[4]);
		}
		Inventory.a.hardwareIsActive[11] = !Inventory.a.hardwareIsActive[11];
		SetVersionIconForButton(Inventory.a.hardwareIsActive[11], Inventory.a.hardwareVersionSetting[11],4);
		if (Inventory.a.hardwareIsActive[11]) {
			infraredLight.enabled = true;
			playerCamera.GetComponent<Grayscale>().enabled = true;
			gunCamera.GetComponent<Grayscale>().enabled = true;
			sensaroundCenterCamera.GetComponent<Grayscale>().enabled = true;
			sensaroundLHCamera.GetComponent<Grayscale>().enabled = true;
			sensaroundRHCamera.GetComponent<Grayscale>().enabled = true;
		} else {
			infraredLight.enabled = false;
			playerCamera.GetComponent<Grayscale>().enabled = false;
			gunCamera.GetComponent<Grayscale>().enabled = false;
			sensaroundCenterCamera.GetComponent<Grayscale>().enabled = false;
			sensaroundLHCamera.GetComponent<Grayscale>().enabled = false;
			sensaroundRHCamera.GetComponent<Grayscale>().enabled = false;
		}
	}

	// called by PlayerMovement when exhausted energy to < 11f
	public void InfraredOff() {
		Inventory.a.hardwareIsActive[11] = false;
		SetVersionIconForButton(Inventory.a.hardwareIsActive[11], Inventory.a.hardwareVersionSetting[11],4);
		infraredLight.enabled = false;
		playerCamera.GetComponent<Grayscale>().enabled = false;
		gunCamera.GetComponent<Grayscale>().enabled = false;
	}

	public void EReaderClick () {
		if (SFXClip != null) SFX.PlayOneShot (SFXClip[5]);
		Inventory.a.hardwareIsActive[2] = true;
		MFDManager.a.OpenEReaderInItemsTab();
	}

	public void BoosterClick() {
		if (Inventory.a.hardwareVersionSetting[6] == 1 && pe.energy <=0) { Const.sprint(Const.a.stringTable[314],pe.wepCur.owner); return; }
		if (Inventory.a.hardwareIsActive[9]) {
			SFX.PlayOneShot (SFXClipDeactivate[6]);
		} else {
			SFX.PlayOneShot (SFXClip[6]);
		}
		Inventory.a.hardwareIsActive[9] = !Inventory.a.hardwareIsActive[9];
		SetVersionIconForButton(Inventory.a.hardwareIsActive[9], Inventory.a.hardwareVersionSetting[9],6);
	}

	// called by PlayerMovement when exhausted energy to < 11f
	public void BoosterOff() {
		Inventory.a.hardwareIsActive[9] = false;
		SetVersionIconForButton(Inventory.a.hardwareIsActive[9], Inventory.a.hardwareVersionSetting[9],6);
	}

	public void JumpJetsClick() {
		if (pe.energy <=0) { Const.sprint(Const.a.stringTable[314],pe.wepCur.owner); return; }
		if (Inventory.a.hardwareIsActive[10]) {
			SFX.PlayOneShot (SFXClipDeactivate[7]);
		} else {
			SFX.PlayOneShot (SFXClip[7]);
		}
		Inventory.a.hardwareIsActive[10] = !Inventory.a.hardwareIsActive[10];
		SetVersionIconForButton(Inventory.a.hardwareIsActive[10], Inventory.a.hardwareVersionSetting[10],7);
	}

	// called by PlayerMovement when exhausted energy to < 11f
	public void JumpJetsOff() {
		Inventory.a.hardwareIsActive[10] = false;
		SetVersionIconForButton(Inventory.a.hardwareIsActive[10], Inventory.a.hardwareVersionSetting[10],7);
	}
}
