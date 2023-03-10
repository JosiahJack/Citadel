using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponCurrent : MonoBehaviour {
	public AmmoIconManager ammoIconManLH;
	public AmmoIconManager ammoIconManRH;
	public GameObject ammoIndicatorHuns;
	public GameObject ammoIndicatorTens;
	public GameObject ammoIndicatorOnes;
	public GameObject ViewModelAssault;
	public GameObject ViewModelBlaster;
	public GameObject ViewModelDartgun;
	public GameObject ViewModelFlechette;
	public GameObject ViewModelIon;
	public GameObject ViewModelRapier;
	public GameObject ViewModelPipe;
	public GameObject ViewModelMagnum;
	public GameObject ViewModelMagpulse;
	public GameObject ViewModelPistol;
	public GameObject ViewModelPlasma;
	public GameObject ViewModelRailgun;
	public GameObject ViewModelRiotgun;
	public GameObject ViewModelSkorpion;
	public GameObject ViewModelSparq;
	public GameObject ViewModelStungun;
	public GameObject owner;
	public GameObject energySlider;
	public GameObject energyHeatTicks;
	public GameObject overloadButton;
	public GameObject unloadButton;
	public GameObject loadNormalAmmoButton;
	public Text loadNormalAmmoButtonText;
	public GameObject loadAlternateAmmoButton;
	public Sprite ammoButtonHighlighted;
	public Sprite ammoButtonDeHighlighted;
	public Text loadAlternateAmmoButtonText;
	public AudioClip ReloadSFX;
	public AudioClip ReloadInStyleSFX;
	public AudioClip WeaponChangeSFX;
	public GameObject reloadContainer;

	[HideInInspector] public Vector3 reloadContainerOrigin; // save
	[HideInInspector] public bool justChangedWeap = true; // save
	public int weaponCurrent = new int(); // save
	public int weaponIndex = new int(); // save
	[HideInInspector] public float[] weaponEnergySetting; // save
	[HideInInspector] public int[] currentMagazineAmount; // save
	[HideInInspector] public int[] currentMagazineAmount2; // save
	[HideInInspector] public int lastIndex = 0; // save
	private AudioSource SFX;
	[HideInInspector] public bool bottomless = false; // Don't use any ammo and
													  // energy weapons no
													  // energy, save
	[HideInInspector] public bool redbull = false; // No energy usage, save
	public float reloadFinished; // save
	[HideInInspector] public float reloadContainerDropAmount = 0.66f;
	public float reloadLerpValue; // save
	public float lerpStartTime; // save
	[HideInInspector] public float targetY; // save
	public int weaponCurrentPending; // save
	public int weaponIndexPending; // save

	public static WeaponCurrent a;

	void Start() {
		a = this;
		a.weaponCurrent = 0; // Current slot in the weapon inventory (7 slots)
		a.weaponIndex = -1; // Current index to the weapon look-up tables
		a.SFX = GetComponent<AudioSource> ();

		// Put energy settings to lowest energy level as default
		for (int j=0;j<7;j++) {
			a.weaponEnergySetting[j] = 0f;
			a.currentMagazineAmount[j] = 0;
			a.currentMagazineAmount2[j] = 0;
		}
		reloadFinished = PauseScript.a.relativeTime;
		reloadContainerOrigin = reloadContainer.transform.localPosition;
		reloadLerpValue = 0;
		weaponCurrentPending = -1;
		weaponIndexPending = -1;
	}

	public void SetAllViewModelsDeactive() {
		Utils.Deactivate(ViewModelAssault);
		Utils.Deactivate(ViewModelBlaster);
		Utils.Deactivate(ViewModelDartgun);
		Utils.Deactivate(ViewModelFlechette);
		Utils.Deactivate(ViewModelIon);
		Utils.Deactivate(ViewModelRapier);
		Utils.Deactivate(ViewModelPipe);
		Utils.Deactivate(ViewModelMagnum);
		Utils.Deactivate(ViewModelMagpulse);
		Utils.Deactivate(ViewModelPistol);
		Utils.Deactivate(ViewModelPlasma);
		Utils.Deactivate(ViewModelRailgun);
		Utils.Deactivate(ViewModelRiotgun);
		Utils.Deactivate(ViewModelSkorpion);
		Utils.Deactivate(ViewModelSparq);
		Utils.Deactivate(ViewModelStungun);
		Utils.Deactivate(energySlider);
		Utils.Deactivate(energyHeatTicks);
		Utils.Deactivate(overloadButton);
		Utils.Deactivate(unloadButton);
		Utils.Deactivate(loadNormalAmmoButton);
		Utils.Deactivate(loadAlternateAmmoButton);
	}

	public void RemoveWeapon(int weaponButton7Index) {
		WeaponButtonsManager wepbutMan = MFDManager.a.wepbutMan;
		WeaponButton wepbut = wepbutMan.wepButtonsScripts[0];
		if (weaponButton7Index != weaponCurrent) {
			if (weaponButton7Index > weaponCurrent) return; // No list shift.

			//int numweps = 0;
			//for (int i=0;i<7;i++) {
			//	WeaponButton wepbut = wepbutMan.wepButtonsScripts[i];
			//	if (wepbut.gameObject.activeSelf) numweps++;
			//}

			//if (numweps == 1) {
			//	currentMagazineAmount[weaponButton7Index] = 0; // Zero out ammo
			//	currentMagazineAmount2[weaponButton7Index] = 0;
			//	MFDManager.a.SetWepInfo(-1);
			//	return;
			//}

			weaponCurrent--;

			//// Shift the indices down on each button from the one above it.
			//for (int i=weaponButton7Index;i<6;i++) {
			//	WeaponButton wepbut = wepbutMan.wepButtonsScripts[i];
			//	WeaponButton wepbutNext = wepbutMan.wepButtonsScripts[i + 1];
			//	wepbut.useableItemIndex = 
			//}

			//// Deactivate the topmost button
			//for (int i=0;i<7;i++) {
			//	WeaponButton wepbut = wepbutMan.wepButtonsScripts[i];
			//	if (wepbut.gameObject.activeSelf) numweps++;
			//}
			return; // Don't continue down and change the weapon, keep current.
		}

		SetAllViewModelsDeactive();
		reloadFinished = 0;

		int initialIndex = WeaponCurrent.a.weaponCurrent;
		if (initialIndex < 0) initialIndex = 0;
		if (initialIndex > 6) initialIndex = 0;
		int nextIndex = initialIndex - 1; // add 1 to get slot above this
		if (nextIndex < 0) nextIndex = 6; // wraparound to top
		int countCheck = 0;
		bool buttonNotValid = (Inventory.a.weaponInventoryIndices[nextIndex] == -1);
		while (buttonNotValid) {
			countCheck++;
			if (countCheck > 13) {
				return; // no weapons!  don't runaway loop
			}
			nextIndex--;
			if (nextIndex < 0) nextIndex = 6;
			buttonNotValid = (Inventory.a.weaponInventoryIndices[nextIndex] == -1);
		}

		wepbut = wepbutMan.wepButtonsScripts[nextIndex];
		if (!wepbut.gameObject.activeSelf) {
			wepbut = wepbutMan.wepButtonsScripts[0];
		}

		if (wepbut.gameObject.activeSelf && nextIndex != initialIndex) {
			weaponIndexPending = wepbut.useableItemIndex;
			weaponCurrentPending = wepbut.WepButtonIndex;
		} else {
			weaponCurrentPending = 0;
			weaponIndexPending = -1;
		}

		lastIndex = weaponCurrent;
		weaponCurrent = -1;
		weaponIndex = -1;
		reloadFinished = PauseScript.a.relativeTime;
		lerpStartTime = PauseScript.a.relativeTime;
		currentMagazineAmount[weaponButton7Index] = 0; // Zero out ammo
		currentMagazineAmount2[weaponButton7Index] = 0;
		UpdateHUDAmmoCountsEither();
		MFDManager.a.SetWepInfo(-1);
		MFDManager.a.OpenTab(0, true, TabMSG.Weapon, 0,Handedness.LH);
	}

	public void WeaponChange(int useableItemIndex, int WepButtonIndex) {
		if (reloadFinished > PauseScript.a.relativeTime) return;
		if (useableItemIndex == -1 || WepButtonIndex > 6
			|| WepButtonIndex < 0) {
			// Clear the ammo icons.
			ammoIconManLH.SetAmmoIcon(-1,false);
			ammoIconManRH.SetAmmoIcon(-1,false);
			Debug.Log("Early exit on WeaponChange() in WeaponCurrent.cs!");
			return;
		}

		Utils.PlayOneShotSavable(SFX,WeaponChangeSFX);
		if (WepButtonIndex == weaponCurrent) return; // Already there!

		int wep16index =  // Get index into the list of 16 weapons
			  WeaponFire.Get16WeaponIndexFromConstIndex(useableItemIndex);
		float delay = Const.a.reloadTime[wep16index];
		reloadFinished = PauseScript.a.relativeTime + delay;
		lerpStartTime = PauseScript.a.relativeTime;
		weaponCurrentPending = WepButtonIndex;
		weaponIndexPending = useableItemIndex;
		MFDManager.a.SetWepInfo(-1);
		UpdateHUDAmmoCountsEither();
	}

	void Update() {
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;

		if (justChangedWeap) {
			justChangedWeap = false;
			Inventory.a.UpdateAmmoText();
			SetAllViewModelsDeactive();
			UpdateWeaponViewModels();
		}

		UpdateAmmoAndLoadButtons();

		if (lastIndex != weaponCurrent) { // Compare weaponCurrent since we
										  // might have more of the same type.
			justChangedWeap = true;
			lastIndex = weaponCurrent;
		}
	}

	void ShowAmmoItems() {
		Utils.Activate(ammoIndicatorHuns);
		Utils.Activate(ammoIndicatorTens);
		Utils.Activate(ammoIndicatorOnes);
		Utils.Activate(unloadButton);
		Utils.Activate(loadNormalAmmoButton);
		Utils.Activate(loadAlternateAmmoButton);
		Utils.Deactivate(energySlider);
		Utils.Deactivate(energyHeatTicks);
		Utils.Deactivate(overloadButton);
	}

	void ShowEnergyItems() {
		Utils.Activate(energySlider);
		Utils.Activate(energyHeatTicks);
		Utils.Activate(overloadButton);
		Utils.Deactivate(ammoIndicatorHuns);
		Utils.Deactivate(ammoIndicatorTens);
		Utils.Deactivate(ammoIndicatorOnes);
		Utils.Deactivate(loadNormalAmmoButton);
		Utils.Deactivate(loadAlternateAmmoButton);
	}

	void HideAll() {
		Utils.Deactivate(ammoIndicatorHuns);
		Utils.Deactivate(ammoIndicatorTens);
		Utils.Deactivate(ammoIndicatorOnes);
		Utils.Deactivate(loadNormalAmmoButton);
		Utils.Deactivate(loadAlternateAmmoButton);
		Utils.Deactivate(energySlider);
		Utils.Deactivate(energyHeatTicks);
		Utils.Deactivate(overloadButton);
	}

	public void UpdateWeaponViewModels() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			int useableIndex = weaponIndex;
			int setWep = weaponIndex;
			if (weaponIndexPending >= 0) {
				setWep = weaponIndexPending;
				useableIndex = -1;
			}

			Debug.Log("setWep = " + setWep.ToString());
			switch (setWep) {
				case 36:
					if (!ViewModelAssault.activeSelf) ViewModelAssault.SetActive(true);
					ShowAmmoItems();
					if (loadNormalAmmoButtonText != null) {
						loadNormalAmmoButtonText.text = Const.a.stringTable[539]; // "LOAD MAGNESIUM"
					}
					if (loadAlternateAmmoButtonText != null) {
						loadAlternateAmmoButtonText.text = Const.a.stringTable[540]; // "LOAD PENETRATOR"
					}
					break;
				case 37: ViewModelBlaster.SetActive(true); ShowEnergyItems(); break;
				case 38:
					if (!ViewModelDartgun.activeSelf) ViewModelDartgun.SetActive(true);
					ShowAmmoItems();
					if (loadNormalAmmoButtonText != null) {
						loadNormalAmmoButtonText.text = Const.a.stringTable[541]; // "LOAD NEEDLE";
					}
					if (loadAlternateAmmoButtonText != null) {
						loadAlternateAmmoButtonText.text = Const.a.stringTable[542]; // "LOAD TRANQ";
					}
					break;
				case 39:
					ViewModelFlechette.SetActive(true);
					ShowAmmoItems();
					if (loadNormalAmmoButtonText != null) {
						loadNormalAmmoButtonText.text = Const.a.stringTable[543]; // "LOAD HORNET";
					}
					if (loadAlternateAmmoButtonText != null) {
						loadAlternateAmmoButtonText.text = Const.a.stringTable[544]; // "LOAD SPLINTER";
					}
					break;
				case 40: ViewModelIon.SetActive(true); ShowEnergyItems(); break;
				case 41: ViewModelRapier.SetActive(true); HideAll(); break;
				case 42: ViewModelPipe.SetActive(true); HideAll(); break;
				case 43:
					ViewModelMagnum.SetActive(true);
					ShowAmmoItems();
					if (loadNormalAmmoButtonText != null) {
						loadNormalAmmoButtonText.text = Const.a.stringTable[545]; // "LOAD HOLLOW TIP";
					}
					if (loadAlternateAmmoButtonText != null) {
						loadAlternateAmmoButtonText.text = Const.a.stringTable[546]; // "LOAD HEAVY SLUG";
					}
					break;
				case 44:
					ViewModelMagpulse.SetActive(true);
					ShowAmmoItems();
					if (loadNormalAmmoButtonText != null) {
						loadNormalAmmoButtonText.text = Const.a.stringTable[547]; // "LOAD CARTRIDGE";
					}
					Utils.Deactivate(loadAlternateAmmoButton);
					break;
				case 45:
					ViewModelPistol.SetActive(true);
					ShowAmmoItems();
					if (loadNormalAmmoButtonText != null) {
						loadNormalAmmoButtonText.text = Const.a.stringTable[548]; // "LOAD STANDARD";
					}
					if (loadAlternateAmmoButtonText != null) {
						loadAlternateAmmoButtonText.text = Const.a.stringTable[549]; // "LOAD TEFLON";
					}
					break;
				case 46: ViewModelPlasma.SetActive(true); ShowEnergyItems(); break;
				case 47:
					ViewModelRailgun.SetActive(true);
					ShowAmmoItems();
					if (loadNormalAmmoButtonText != null) {
						loadNormalAmmoButtonText.text = Const.a.stringTable[550]; // "LOAD RAIL CLIP";
					}
					Utils.Deactivate(loadAlternateAmmoButton);
					break;
				case 48:
					ViewModelRiotgun.SetActive(true);
					ShowAmmoItems();
					if (loadNormalAmmoButtonText != null) {
						loadNormalAmmoButtonText.text = Const.a.stringTable[551]; // "LOAD RUBBER SLUG";
					}
					Utils.Deactivate(loadAlternateAmmoButton);
					break;
				case 49:
					ViewModelSkorpion.SetActive(true);
					ShowAmmoItems();
					if (loadNormalAmmoButtonText != null) {
						loadNormalAmmoButtonText.text = Const.a.stringTable[552]; // "LOAD SLAG";
					}
					if (loadAlternateAmmoButtonText != null) {
						loadAlternateAmmoButtonText.text = Const.a.stringTable[553]; // "LOAD LARGE SLAG";
					}
					break;
				case 50: ViewModelSparq.SetActive(true); ShowEnergyItems(); break;
				case 51: ViewModelStungun.SetActive(true); ShowEnergyItems(); break;
				default: HideAll(); break;
			}
		}
	}

	void UpdateAmmoAndLoadButtons() {
		if (weaponCurrent < 0 || weaponCurrentPending >= 0) {
			Utils.Deactivate(ammoIndicatorHuns);
			Utils.Deactivate(ammoIndicatorTens);
			Utils.Deactivate(ammoIndicatorOnes);
			Utils.Deactivate(energySlider);
			Utils.Deactivate(energyHeatTicks);
			Utils.Deactivate(overloadButton);
			Utils.Deactivate(unloadButton);
			Utils.Deactivate(loadNormalAmmoButton);
			Utils.Deactivate(loadAlternateAmmoButton);
			return;
		}

		UpdateHUDAmmoCountsEither();
		if (loadNormalAmmoButton == null
			|| loadAlternateAmmoButton == null) {
			return;
		}

		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex(weaponIndex);
		if (wep16index == 1 || wep16index == 4 || wep16index == 10
			|| wep16index == 14 || wep16index == 15) {
			return; // Already hidden.
		}

		Image norm = loadNormalAmmoButton.GetComponent<Image>();
		Image anorm = loadAlternateAmmoButton.GetComponent<Image>();
		if (Inventory.a.wepLoadedWithAlternate[weaponCurrent]) {
			norm.overrideSprite = ammoButtonDeHighlighted;
			if (currentMagazineAmount2[weaponCurrent] > 0) {
				anorm.overrideSprite = ammoButtonHighlighted;
			} else {
				anorm.overrideSprite = ammoButtonDeHighlighted;
			}
		} else {
			anorm.overrideSprite = ammoButtonDeHighlighted;
			if (currentMagazineAmount[weaponCurrent] > 0) {
				norm.overrideSprite = ammoButtonHighlighted;
			} else {
				norm.overrideSprite = ammoButtonDeHighlighted;
			}
		}
	}

	public void ChangeAmmoType() {
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex(weaponIndex);
		if (wep16index == 5 || wep16index == 6) { Const.sprint(Const.a.stringTable[315],owner); return; } // Do nothing for pipe or rapier.

		if (wep16index == 1 || wep16index == 4 || wep16index == 10 || wep16index == 14 || wep16index == 15) {
			if (overloadButton.activeInHierarchy) overloadButton.GetComponent<EnergyOverloadButton>().OverloadButtonAction();
		} else {
			if (Inventory.a.wepLoadedWithAlternate[weaponCurrent]) {
				if (Inventory.a.wepAmmo[wep16index] > 0) {
				Inventory.a.wepLoadedWithAlternate[weaponCurrent] = false;
				// Take bullets out of the clip, put them back into the ammo stockpile, then zero out the clip amount, did I say clip?  I mean magazine but whatever
				Inventory.a.wepAmmoSecondary[wep16index] += currentMagazineAmount2[weaponCurrent];
				currentMagazineAmount2[weaponCurrent] = 0;
				LoadPrimaryAmmoType(false);
				} else {
					Const.sprint(Const.a.stringTable[535],owner); //No more of ammo type to load.
				}
			} else {
				if (Inventory.a.wepAmmoSecondary[wep16index] > 0) {
					Inventory.a.wepLoadedWithAlternate[weaponCurrent] = true;
					Inventory.a.wepAmmo[wep16index] += currentMagazineAmount[weaponCurrent];
					currentMagazineAmount[weaponCurrent] = 0;
					LoadSecondaryAmmoType(false);
				} else {
					Const.sprint(Const.a.stringTable[535],owner); //No more of ammo type to load.
				}
			}
		}
	}

	public void LoadPrimaryAmmoType(bool isSilent) {
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex(weaponIndex);
		if (!Inventory.a.wepLoadedWithAlternate[weaponCurrent]) { // Already loaded with normal.
			if (currentMagazineAmount[weaponCurrent] == Const.a.magazinePitchCountForWeapon[wep16index]) {
				Const.sprint(Const.a.stringTable[191],owner); //Current weapon magazine already full.
				return;
			}
			
			if (currentMagazineAmount[weaponCurrent] == Inventory.a.wepAmmo[wep16index]) {
				Const.sprint("No more of ammo type to load");
				return;
			}
		}

		Unload(true);
		Inventory.a.wepLoadedWithAlternate[weaponCurrent] = false;

		// Put bullets into the magazine
		if (Inventory.a.wepAmmo[wep16index] >= Const.a.magazinePitchCountForWeapon[wep16index]) {
			currentMagazineAmount[weaponCurrent] = Const.a.magazinePitchCountForWeapon[wep16index];
		} else {
			currentMagazineAmount[weaponCurrent] = Inventory.a.wepAmmo[wep16index];
		}

		// Take bullets out of the ammo stockpile
		Inventory.a.wepAmmo[wep16index] -= currentMagazineAmount[weaponCurrent];

		if (!isSilent) {
			if (wep16index == 0 || wep16index == 3) {
				Utils.PlayOneShotSavable(SFX,ReloadInStyleSFX);
			} else {
				Utils.PlayOneShotSavable(SFX,ReloadSFX);
			}
		}

		// Update the counter on the HUD
		MFDManager.a.UpdateHUDAmmoCounts(currentMagazineAmount[weaponCurrent]);

		reloadFinished = PauseScript.a.relativeTime + Const.a.reloadTime[wep16index];
		lerpStartTime = PauseScript.a.relativeTime;
		reloadContainer.transform.localPosition = reloadContainerOrigin; // pop it back to start to be sure
	}

	public void LoadSecondaryAmmoType(bool isSilent) {
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex(weaponIndex);
		if (Inventory.a.wepLoadedWithAlternate[weaponCurrent]) { // Already loaded with alternate
			if (currentMagazineAmount2[weaponCurrent] == Const.a.magazinePitchCountForWeapon2[wep16index]) {
				Const.sprint(Const.a.stringTable[191],owner); //Current weapon magazine already full.
				return;
			}
			
			if (currentMagazineAmount2[weaponCurrent] == Inventory.a.wepAmmoSecondary[wep16index]) {
				Const.sprint("No more of ammo type to load");
				return;
			}
		}

		Unload(true);
		Inventory.a.wepLoadedWithAlternate[weaponCurrent] = true;

		// Put bullets into the magazine
		if (Inventory.a.wepAmmoSecondary[wep16index] >= Const.a.magazinePitchCountForWeapon2[wep16index]) {
			currentMagazineAmount2[weaponCurrent] = Const.a.magazinePitchCountForWeapon2[wep16index];
		} else {
			currentMagazineAmount2[weaponCurrent] = Inventory.a.wepAmmoSecondary[wep16index];
		}

		// Take bullets out of the ammo stockpile
		Inventory.a.wepAmmoSecondary[wep16index] -= currentMagazineAmount2[weaponCurrent];

		if (!isSilent) {
			if (wep16index == 0 || wep16index == 3) {
				Utils.PlayOneShotSavable(SFX,ReloadInStyleSFX);
			} else {
				Utils.PlayOneShotSavable(SFX,ReloadSFX);
			}
		}

		// Update the counter on the HUD
		MFDManager.a.UpdateHUDAmmoCounts(currentMagazineAmount2[weaponCurrent]);

		reloadFinished = PauseScript.a.relativeTime + Const.a.reloadTime[wep16index];
		lerpStartTime = PauseScript.a.relativeTime;
		reloadContainer.transform.localPosition = reloadContainerOrigin; // pop it back to start to be sure
	}

	public void UpdateHUDAmmoCountsEither() {
		if (weaponCurrent >= 0) {
			if (Inventory.a.wepLoadedWithAlternate[weaponCurrent]) {
				MFDManager.a.UpdateHUDAmmoCounts(currentMagazineAmount2[weaponCurrent]);
			} else {
				MFDManager.a.UpdateHUDAmmoCounts(currentMagazineAmount[weaponCurrent]);
			}
		}
	}

	public void Unload(bool isSilent) {
		if (weaponIndex < 0) return;
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex (weaponIndex);
		if (wep16index == 5 || wep16index == 6) {
			return; // do nothing for pipe or rapier
		}

		if (wep16index == -1) return; // we don't have a weapon at all right now :)

		// Take bullets out of the clip, put them back into the ammo stockpile, then zero out the clip amount, did I say clip?  I mean magazine but whatever
		if (Inventory.a.wepLoadedWithAlternate[weaponCurrent]) {
			Inventory.a.wepAmmoSecondary[wep16index] += currentMagazineAmount2[weaponCurrent];
			currentMagazineAmount2[weaponCurrent] = 0;

			// Update the counter on the HUD
			MFDManager.a.UpdateHUDAmmoCounts(currentMagazineAmount2[weaponCurrent]);
		} else {
			Inventory.a.wepAmmo[wep16index] += currentMagazineAmount[weaponCurrent];
			currentMagazineAmount[weaponCurrent] = 0;

			// Update the counter on the HUD
			MFDManager.a.UpdateHUDAmmoCounts(currentMagazineAmount[weaponCurrent]);
		}
		if (!isSilent) Utils.PlayOneShotSavable(SFX,ReloadSFX);
	}

	public void ReloadSecret(bool isSilent) {
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex(weaponIndex);
		if (wep16index < 0) return;

		if (wep16index == 5 || wep16index == 6) {
			Const.sprint(Const.a.stringTable[315],owner); // Weapon does not need reloaded.
			return; // do nothing for pipe or rapier
		}

		if (wep16index == 1 || wep16index == 4 || wep16index == 10 || wep16index == 14 || wep16index == 15) {
			Const.sprint(Const.a.stringTable[538],owner); // We
			return; // do nothing for energy weapons
		}

		if (weaponCurrent < 0) return;

		if (Inventory.a.wepLoadedWithAlternate[weaponCurrent]) {
			if (currentMagazineAmount2[weaponCurrent] == Const.a.magazinePitchCountForWeapon2[wep16index]) {
				Const.sprint(Const.a.stringTable[191],owner); //Current weapon magazine already full.
				return;
			}

			if (Inventory.a.wepAmmoSecondary[wep16index] <= 0) {
				if (Inventory.a.wepAmmo[wep16index] <= 0) {
					Const.sprint(Const.a.stringTable[305],owner); //No more of any ammo type to load.
					return;
				} else {
					Const.sprint(Const.a.stringTable[192],owner); //No more of current ammo type to load, loading with alternate.
					LoadPrimaryAmmoType(isSilent);
					return;
				}
			}
			LoadSecondaryAmmoType(isSilent);
		} else {
			if (currentMagazineAmount[weaponCurrent] == Const.a.magazinePitchCountForWeapon[wep16index]) {
				Const.sprint(Const.a.stringTable[191],owner); //Current weapon magazine already full.
				return;
			}

			if (Inventory.a.wepAmmo[wep16index] <= 0) {
				if (Inventory.a.wepAmmoSecondary[wep16index] <= 0) {
					Const.sprint(Const.a.stringTable[305],owner); //No more of any ammo type to load.
					return;
				} else {
					Const.sprint(Const.a.stringTable[192],owner); //No more of current ammo type to load, loading with alternate.
					LoadSecondaryAmmoType(isSilent);
					return;
				}
			}
			LoadPrimaryAmmoType(isSilent);
		}
	}

	public void Reload() {
		ReloadSecret(false);
	}

	public static string Save(GameObject go) {
		WeaponCurrent wc = go.GetComponent<WeaponCurrent>();
		if (wc == null) {
			Debug.Log("WeaponCurrent missing on Player!  GameObject.name: " + go.name);
			return "0|0|0";
		}

		int j =0;
		string line = System.String.Empty;
		line = Utils.UintToString(wc.weaponCurrent);
		line += Utils.splitChar + Utils.UintToString(wc.weaponIndex);
		for (j=0;j<7;j++) { line += Utils.splitChar + Utils.FloatToString(wc.weaponEnergySetting[j]); }
		for (j=0;j<7;j++) { line += Utils.splitChar + wc.currentMagazineAmount[j].ToString(); }
		for (j=0;j<7;j++) { line += Utils.splitChar + wc.currentMagazineAmount2[j].ToString(); }
		line += Utils.splitChar + Utils.UintToString(wc.lastIndex);
		line += Utils.splitChar + Utils.BoolToString(wc.bottomless);
		line += Utils.splitChar + Utils.BoolToString(wc.redbull);
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(wc.reloadFinished);
		line += Utils.splitChar + Utils.FloatToString(wc.reloadLerpValue);
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(wc.lerpStartTime);
		line += Utils.splitChar + Utils.FloatToString(wc.targetY);
		line += Utils.splitChar + Utils.UintToString(wc.weaponCurrentPending);
		line += Utils.splitChar + Utils.UintToString(wc.weaponIndexPending);
		line += Utils.splitChar + Utils.SaveTransform(wc.reloadContainer.transform);
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		WeaponCurrent wc = go.GetComponent<WeaponCurrent>();
		if (wc == null) {
			Debug.Log("WeaponCurrent.Load failure, wc == null");
			return index + 31;
		}

		if (index < 0) {
			Debug.Log("WeaponCurrent.Load failure, index < 0");
			return index + 31;
		}

		if (entries == null) {
			Debug.Log("WeaponCurrent.Load failure, entries == null");
			return index + 31;
		}

		int j =0;
		wc.weaponCurrent = Utils.GetIntFromString(entries[index] ); index++;
		wc.weaponIndex = Utils.GetIntFromString(entries[index] ); index++;
		for (j=0;j<7;j++) { wc.weaponEnergySetting[j] = Utils.GetFloatFromString(entries[index]); index++; }
		for (j=0;j<7;j++) { wc.currentMagazineAmount[j] = Utils.GetIntFromString(entries[index] ); index++; }
		for (j=0;j<7;j++) { wc.currentMagazineAmount2[j] = Utils.GetIntFromString(entries[index] ); index++; }
		wc.SetAllViewModelsDeactive();
		wc.lastIndex = Utils.GetIntFromString(entries[index] ); index++;
		wc.bottomless = Utils.GetBoolFromString(entries[index]); index++;
		wc.redbull = Utils.GetBoolFromString(entries[index]); index++;
		wc.reloadFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		wc.reloadLerpValue = Utils.GetFloatFromString(entries[index]); index++; // %
		wc.lerpStartTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		wc.targetY = Utils.GetFloatFromString(entries[index]); index++;
		wc.weaponCurrentPending = Utils.GetIntFromString(entries[index] ); index++;
		wc.weaponIndexPending = Utils.GetIntFromString(entries[index] ); index++;
		index = Utils.LoadTransform(wc.reloadContainer.transform,ref entries,index);
		wc.justChangedWeap = true;
		return index;
	}
}