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
	public Light headlight;
	public EmailContentsButtonsManager ecbm;
	public GameObject ShieldActivateFX;
	public GameObject ShieldDeactivateFX;

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
	private const float defaultZero = 0f;
	private float brightness = 0f;
	private const float lanternVersion1Brightness = 2.5f;
	private const float lanternVersion2Brightness = 4;
	private const float lanternVersion3Brightness = 5;
	private Grayscale gsc;
	private Grayscale gscSensaCenter;
	private Grayscale gscSensaLH;
	private Grayscale gscSensaRH;

	void Awake () {
		SFX = GetComponent<AudioSource>();
		gsc = playerCamera.GetComponent<Grayscale>();
		gscSensaCenter = sensaroundCenterCamera.GetComponent<Grayscale>();
		gscSensaLH = sensaroundLHCamera.GetComponent<Grayscale>();
		gscSensaRH = sensaroundRHCamera.GetComponent<Grayscale>();
	}

	public void ListenForHardwareHotkeys () {
		if (Inventory.a.hasHardware[2] && GetInput.a.Email())      EReaderAction();
		if (Inventory.a.hasHardware[3] && GetInput.a.Sensaround()) SensaroundAction();
		if (Inventory.a.hasHardware[5] && GetInput.a.Shield())     ShieldAction();
		if (Inventory.a.hasHardware[6] && GetInput.a.Biomonitor()) BioAction();
		if (Inventory.a.hasHardware[7] && GetInput.a.Lantern())    LanternAction();
		if (Inventory.a.hasHardware[9] && GetInput.a.Booster())    BoosterAction();
		if (Inventory.a.hasHardware[10]&& GetInput.a.Jumpjets())   JumpJetsAction();
		if (Inventory.a.hasHardware[11]&& GetInput.a.Infrared())   InfraredAction();
	}

	// 0 = bio, 1 = sen, 2 = lan, 3 = shi, 4 = nig, 5 = ere, 6 = boo, 7 = jum
	// verz must come from Inventory.a.hardwareVersionSetting[] as this value has already subtracted 1 since the version number on prefabs is 1 based but the one needed for images is 0 based.
	public void SetVersionIconForButton(bool isOn, int verz, int button8Index) {
// 		Debug.Log("SetVersionIconForButton with version " + verz.ToString() + ", and button8Index of " + button8Index.ToString());
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
		MFDManager.a.mouseClickHeldOverGUI = true;
		BioAction();
	}

	public void BioAction() {
		if (Inventory.a.BioMonitorVersion() == 0 && PlayerEnergy.a.energy <= 0) {
			Const.sprint(Const.a.stringTable[314],WeaponCurrent.a.owner);
			return;
		}

		Utils.PlayUIOneShotSavable(78);
		if (Inventory.a.BioMonitorActive()) {
			BioOff();
		} else {
			BioOn();
		}
	}

	// Called by PlayerEnergy when exhausted energy to 0 so mustn't play sound.
	public void BioOff() {
		Inventory.a.hardwareIsActive[6] = false;
		SetVersionIconForButton(Inventory.a.hardwareIsActive[6],Inventory.a.hardwareVersionSetting[6],0);
		
		if (MFDManager.a.FPS.activeInHierarchy) return;
		if (BiomonitorGraphSystem.a != null) {
			BiomonitorGraphSystem.a.ClearGraphs();
		}

		Utils.Deactivate(bioMonitorContainer);
	}

	public void BioOn() {
		Inventory.a.hardwareIsActive[6] = true;
		SetVersionIconForButton(Inventory.a.BioMonitorActive(),Inventory.a.hardwareVersionSetting[6],0);
		Utils.Activate(bioMonitorContainer);
	}

	public void ActivateSensaroundCenter() {
		MFDManager.a.DisableAllCenterTabs();
		Utils.Activate(sensaroundCenterCamera);
		Utils.Activate(sensaroundCenter);
	}

	public void ActivateSensaroundSides() {
		MFDManager.a.TabReset(true); // right
		MFDManager.a.TabReset(false); // left
		if (sensaroundLHCamera != null) sensaroundLHCamera.SetActive (true);
		if (sensaroundLH != null) sensaroundLH.SetActive (true);
		if (sensaroundRHCamera != null) sensaroundRHCamera.SetActive (true);
		if (sensaroundRH != null) sensaroundRH.SetActive (true);
	}
	
	public void HideSensaround() {
		if (sensaroundCenterCamera != null) sensaroundCenterCamera.SetActive(false);
		if (sensaroundCenter != null) sensaroundCenter.SetActive(false);
		if (sensaroundLHCamera != null) sensaroundLHCamera.SetActive(false);
		if (sensaroundLH != null) sensaroundLH.SetActive(false);
		if (sensaroundRHCamera != null) sensaroundRHCamera.SetActive(false);
		if (sensaroundRH != null) sensaroundRH.SetActive(false);
	}
	
	public void UnhideSensaround() {
		if (!Inventory.a.hardwareIsActive[3]) return;
		
		if (Inventory.a.hardwareVersion[3] == 1) {
			ActivateSensaroundCenter(); // Only center on version 1.
		} else {
			ActivateSensaroundCenter();
			ActivateSensaroundSides();
		}
	}

	public void DeactivateSensaroundCameras() {
		HideSensaround();
		MFDManager.a.CenterTabButtonClickSilent(MFDManager.a.curCenterTab,true);
		MFDManager.a.TabReset(true); // right
		MFDManager.a.TabReset(false); // left
		MFDManager.a.ReturnToLastTab(true);
		MFDManager.a.ReturnToLastTab(false);
	}

	public void SensaroundOn() {
		Inventory.a.hardwareIsActive[3] = true;
		SetVersionIconForButton(Inventory.a.hardwareIsActive[3], Inventory.a.hardwareVersionSetting[3],1);
		UnhideSensaround();
	}

	public void SensaroundClick() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		SensaroundAction();
	}

	public void SensaroundAction() {
		if (PlayerEnergy.a.energy <=0) { Const.sprint(Const.a.stringTable[314],WeaponCurrent.a.owner); return; }

		if (Inventory.a.hardwareIsActive[3]) {
			Utils.PlayUIOneShotSavable(82);
			SensaroundOff();
		} else {
			Utils.PlayUIOneShotSavable(93);
			SensaroundOn();
		}
	}

	// called by PlayerEnergy when exhausted energy to 0
	public void SensaroundOff() {
		Inventory.a.hardwareIsActive[3] = false;
		SetVersionIconForButton(Inventory.a.hardwareIsActive[3],Inventory.a.hardwareVersionSetting[3],1);
		DeactivateSensaroundCameras();
	}

	public void ShieldClick() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		ShieldAction();
	}
	
	public void ShieldOff() {
		Inventory.a.hardwareIsActive[5] = false;
		SetVersionIconForButton(Inventory.a.hardwareIsActive[5],Inventory.a.hardwareVersionSetting[5],3);
	}
	
	public void ShieldOn() {
		Inventory.a.hardwareIsActive[5] = true;
		SetVersionIconForButton(Inventory.a.hardwareIsActive[5],Inventory.a.hardwareVersionSetting[5],3);
	}

	public void ShieldAction() {
		if (PlayerEnergy.a.energy <=0) { Const.sprint(Const.a.stringTable[314],WeaponCurrent.a.owner); return; }
		if (Inventory.a.hardwareIsActive[5]) {
			Utils.PlayUIOneShotSavable(95);
			ShieldOffWithEffects();
		} else {
			Utils.PlayUIOneShotSavable(96);
			ShieldDeactivateFX.SetActive(false);
			ShieldActivateFX.SetActive(true);
			ShieldOn();
			
		}
	}

	// Called by PlayerEnergy when exhausted energy to 0.
	public void ShieldOffWithEffects() {
		ShieldOff();
		ShieldDeactivateFX.SetActive(true);
		ShieldActivateFX.SetActive(false);
	}

	public void LanternClick() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		LanternAction();
	}

	public void LanternAction() {
		if (PlayerEnergy.a.energy <=0) { Const.sprint(Const.a.stringTable[314],WeaponCurrent.a.owner); return; }
		Utils.PlayUIOneShotSavable(78);
		if (Inventory.a.hardwareIsActive[7]) {
			LanternOff();
		} else {
			LanternOn();
		}
	}

	public void LanternOn() {
		Inventory.a.hardwareIsActive[7] = true;
		SetVersionIconForButton(Inventory.a.LanternActive(), Inventory.a.hardwareVersionSetting[7],2);

		// Figure out which brightness setting to use depending on version.
		switch(Inventory.a.hardwareVersionSetting[7]) {
			case 0: brightness = lanternVersion1Brightness; break;
			case 1: brightness = lanternVersion2Brightness; break;
			case 2: brightness = lanternVersion3Brightness; break;
			default: brightness = defaultZero; break;
		}

		Utils.EnableLight(headlight);
		headlight.intensity = brightness; // Set the light intensity per version.
	}
	
	// Called by PlayerEnergy when exhausted energy to 0.
	public void LanternOff() {
		Inventory.a.hardwareIsActive[7] = false;
		SetVersionIconForButton(Inventory.a.LanternActive(), Inventory.a.hardwareVersionSetting[7],2);
		Utils.DisableLight(headlight);
		headlight.intensity = defaultZero; // Turn the light off.
	}

	public void InfraredClick() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		InfraredAction();
	}

	public void InfraredAction() {
		if (PlayerEnergy.a.energy <=0) { Const.sprint(Const.a.stringTable[314],WeaponCurrent.a.owner); return; }
		if (Inventory.a.hardwareIsActive[11]) {
			Utils.PlayUIOneShotSavable(82);
		} else {
			Utils.PlayUIOneShotSavable(98);
		}
		Inventory.a.hardwareIsActive[11] = !Inventory.a.hardwareIsActive[11];
		SetVersionIconForButton(Inventory.a.hardwareIsActive[11], Inventory.a.hardwareVersionSetting[11],4);
		if (Inventory.a.hardwareIsActive[11]) {
			InfraredOn();
		} else {
			InfraredOff();
		}
	}

	public void InfraredOn() {
		Utils.EnableLight(infraredLight);
		Utils.EnableGrayscale(gsc);
		Utils.EnableGrayscale(gscSensaCenter);
		Utils.EnableGrayscale(gscSensaLH);
		Utils.EnableGrayscale(gscSensaRH);
	}

	// called by PlayerMovement when exhausted energy to < 11f
	public void InfraredOff() {
		Inventory.a.hardwareIsActive[11] = false;
		Utils.DisableLight(infraredLight);
		Utils.DisableGrayscale(gsc);
		Utils.DisableGrayscale(gscSensaCenter);
		Utils.DisableGrayscale(gscSensaLH);
		Utils.DisableGrayscale(gscSensaRH);
		SetVersionIconForButton(false,Inventory.a.hardwareVersionSetting[11],4);
	}

	public void EReaderClick () {
		MFDManager.a.mouseClickHeldOverGUI = true;
		EReaderAction();
	}
	
	public void EReaderOn() {
		Inventory.a.hardwareIsActive[2] = true;
		MFDManager.a.OpenEReaderInItemsTab();
	}

	public void EReaderAction() {
		Utils.PlayUIOneShotSavable(97);
		EReaderOn();
	}


	public void BoosterClick() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		BoosterAction();
	}

	public void BoosterAction() {
		if (Inventory.a.BoosterSetToBoost() && PlayerEnergy.a.energy <= 0) {
			Const.sprint(Const.a.stringTable[314],WeaponCurrent.a.owner);
			return;
		}

		Utils.PlayUIOneShotSavable(78);
		if (Inventory.a.hardwareIsActive[9]) {
			BoosterOff();
		} else {
			BoosterOn();
		}
	}
	
	public void BoosterOn() {
		Inventory.a.hardwareIsActive[9] = true;
		SetVersionIconForButton(Inventory.a.hardwareIsActive[9],Inventory.a.hardwareVersionSetting[9],6);
	}

	// called by PlayerMovement when exhausted energy to < 11f
	public void BoosterOff() {
		Inventory.a.hardwareIsActive[9] = false;
		SetVersionIconForButton(Inventory.a.hardwareIsActive[9],Inventory.a.hardwareVersionSetting[9],6);
	}

	public void JumpJetsClick() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		JumpJetsAction();
	}

	public void JumpJetsAction() {
		if (PlayerEnergy.a.energy <= 0) {
			Const.sprint(Const.a.stringTable[314],WeaponCurrent.a.owner);
			return;
		}

		Utils.PlayUIOneShotSavable(78);
		Inventory.a.JumpJetsToggle();
		if (Inventory.a.JumpJetsActive()) {
			JumpJetsOn();
		} else {
			JumpJetsOff();
		}
	}
	
	public void JumpJetsOn() {
		Inventory.a.hardwareIsActive[10] = true;
		SetVersionIconForButton(Inventory.a.JumpJetsActive(),Inventory.a.hardwareVersionSetting[10],7);
	}

	// called by PlayerMovement when exhausted energy to < 11f
	public void JumpJetsOff() {
		Inventory.a.hardwareIsActive[10] = false;
		SetVersionIconForButton(Inventory.a.JumpJetsActive(),Inventory.a.hardwareVersionSetting[10],7);
	}
}
