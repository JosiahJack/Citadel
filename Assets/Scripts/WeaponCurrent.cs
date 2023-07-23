using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponCurrent : MonoBehaviour {
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

	public AudioClip ReloadSFX;
	public AudioClip ReloadInStyleSFX;
	public AudioClip WeaponChangeSFX;
	public int weaponCurrent = new int(); // save
	public int weaponIndex = new int(); // save

	[HideInInspector] public bool justChangedWeap = true; // save
	[HideInInspector] public float[] weaponEnergySetting; // save
	[HideInInspector] public int[] currentMagazineAmount; // save
	[HideInInspector] public int[] currentMagazineAmount2; // save
	[HideInInspector] public int lastIndex = 0; // save
	[HideInInspector] public bool bottomless = false; // Don't use any ammo and
													  // energy weapons no
													  // energy, save
	[HideInInspector] public bool redbull = false; // No energy usage, save
	private AudioSource SFX;
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
		Utils.Deactivate(MFDManager.a.energySliderLH);
		Utils.Deactivate(MFDManager.a.energyHeatTicksLH);
		Utils.Deactivate(MFDManager.a.overloadButtonLH);
		Utils.Deactivate(MFDManager.a.unloadButtonLH);
		Utils.Deactivate(MFDManager.a.loadNormalAmmoButtonLH);
		Utils.Deactivate(MFDManager.a.loadAlternateAmmoButtonLH);

		Utils.Deactivate(MFDManager.a.energySliderRH);
		Utils.Deactivate(MFDManager.a.energyHeatTicksRH);
		Utils.Deactivate(MFDManager.a.overloadButtonRH);
		Utils.Deactivate(MFDManager.a.unloadButtonRH);
		Utils.Deactivate(MFDManager.a.loadNormalAmmoButtonRH);
		Utils.Deactivate(MFDManager.a.loadAlternateAmmoButtonRH);
	}

	public void RemoveWeapon(int weaponButton7Index) {
		WeaponButtonsManager wepbutMan = MFDManager.a.wepbutMan;
		WeaponButton wepbut = wepbutMan.wepButtonsScripts[0];
		if (weaponButton7Index != weaponCurrent) {
			if (weaponButton7Index > weaponCurrent) return; // No list shift.

			weaponCurrent--;
			return; // Don't continue down and change the weapon, keep current.
		}

		SetAllViewModelsDeactive();
		WeaponFire.a.reloadFinished = 0;
		int initialIndex = WeaponCurrent.a.weaponCurrent;
		if (initialIndex < 0) initialIndex = 0;
		if (initialIndex > 6) initialIndex = 0;
		int nextIndex = initialIndex - 1; // add 1 to get slot above this
		if (nextIndex < 0) nextIndex = 6; // wraparound to top
		int countCheck = 0;
		bool buttonNotValid = (Inventory.a.weaponInventoryIndices[nextIndex] == -1);
		while (buttonNotValid) {
			countCheck++;
			if (countCheck > 13) return; // no weapons!  don't runaway loop

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
		WeaponFire.a.StartWeaponDip(0);
		currentMagazineAmount[weaponButton7Index] = 0; // Zero out ammo
		currentMagazineAmount2[weaponButton7Index] = 0;
		MFDManager.a.UpdateHUDAmmoCountsEither();
		MFDManager.a.SetWepInfo(-1);
		MFDManager.a.OpenTab(0, true, TabMSG.Weapon, 0,Handedness.LH);
	}

	public void WeaponChange(int useableItemIndex, int buttonIndex) {
		if (WeaponFire.a.reloadFinished > PauseScript.a.relativeTime) return;
		if (useableItemIndex == -1 || buttonIndex > 6 || buttonIndex < 0) {
			MFDManager.a.SetAmmoIcons(-1,false); // Clear the ammo icons.
			Debug.Log("Early exit on WeaponChange() in WeaponCurrent.cs!");
			return;
		}

		Utils.PlayOneShotSavable(SFX,WeaponChangeSFX);
		if (buttonIndex == weaponCurrent) return; // Already there!

		int wep16index =  // Get index into the list of 16 weapons
			  WeaponFire.Get16WeaponIndexFromConstIndex(useableItemIndex);

		WeaponFire.a.StartWeaponDip(Const.a.reloadTime[wep16index]);
		weaponCurrentPending = buttonIndex;
		weaponIndexPending = useableItemIndex;
		MFDManager.a.SetWepInfo(-1);
		MFDManager.a.UpdateHUDAmmoCountsEither();
	}

	void Update() {
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;

		if (justChangedWeap) {
			justChangedWeap = false;
			MFDManager.a.HideAmmoAndEnergyItems();
			MFDManager.a.SetAmmoIcons(-1,false); // Clear it.
			SetAllViewModelsDeactive();
			UpdateWeaponViewModels();
		}

		// Compare weaponCurrent since we might have more of the same type.
		if (lastIndex != weaponCurrent) { 
			justChangedWeap = true;
			lastIndex = weaponCurrent;
		}
	}

	public void UpdateWeaponViewModels() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			int useableIndex = weaponIndex;
			int setWep = weaponIndex;
			if (weaponIndexPending >= 0) {
				setWep = weaponIndexPending;
				useableIndex = -1;
			}

			switch (setWep) {
				case 36: // "LOAD MAGNESIUM", "LOAD PENETRATOR"
					MFDManager.a.ShowAmmoItems(539,540);
					Utils.Activate(ViewModelAssault);
					break;
				case 37:
					MFDManager.a.ShowEnergyItems();
					Utils.Activate(ViewModelBlaster);
					break;
				case 38: // "LOAD NEEDLE", "LOAD TRANQ"
					MFDManager.a.ShowAmmoItems(541,542);
					Utils.Activate(ViewModelDartgun);
					break;
				case 39: // "LOAD HORNET", "LOAD SPLINTER"
					MFDManager.a.ShowAmmoItems(543,544);
					Utils.Activate(ViewModelFlechette);
					break;
				case 40:
					MFDManager.a.ShowEnergyItems();
					Utils.Activate(ViewModelIon);
					break;
				case 41:
					Utils.Activate(ViewModelRapier);
					MFDManager.a.HideAmmoAndEnergyItems();
					break;
				case 42:
					Utils.Activate(ViewModelPipe);
					MFDManager.a.HideAmmoAndEnergyItems();
					break;
				case 43: // "LOAD HOLLOW TIP", "LOAD HEAVY SLUG"
					MFDManager.a.ShowAmmoItems(545,546);
					Utils.Activate(ViewModelMagnum);
					break;
				case 44: // "LOAD CARTRIDGE"
					MFDManager.a.ShowAmmoItems(547,-1);
					Utils.Activate(ViewModelMagpulse);
					MFDManager.a.HideAlternateAmmoButton();
					break;
				case 45: // "LOAD STANDARD", "LOAD TEFLON"
					MFDManager.a.ShowAmmoItems(548,549);
					Utils.Activate(ViewModelPistol);
					break;
				case 46:
					MFDManager.a.ShowEnergyItems();
					Utils.Activate(ViewModelPlasma);
					break;
				case 47: // "LOAD RAIL CLIP"
					MFDManager.a.ShowAmmoItems(550,-1);
					Utils.Activate(ViewModelRailgun);
					MFDManager.a.HideAlternateAmmoButton();
					break;
				case 48:  // "LOAD RUBBER SLUG"
					MFDManager.a.ShowAmmoItems(551,-1);
					Utils.Activate(ViewModelRiotgun);
					MFDManager.a.HideAlternateAmmoButton();
					break;
				case 49: // "LOAD SLAG", "LOAD LARGE SLAG"
					MFDManager.a.ShowAmmoItems(552,553);
					Utils.Activate(ViewModelSkorpion);
					break;
				case 50:
					MFDManager.a.ShowEnergyItems();
					Utils.Activate(ViewModelSparq);
					break;
				case 51:
					MFDManager.a.ShowEnergyItems();
					Utils.Activate(ViewModelStungun);
					break;
				default: MFDManager.a.HideAmmoAndEnergyItems(); break;
			}
		}
	}

	public void ChangeAmmoType() {
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex(weaponIndex);
		if (wep16index < 0) return;
		if (wep16index == 5 || wep16index == 6) {
			Const.sprint(Const.a.stringTable[315],owner);
			return; // Do nothing for pipe or rapier.
		}

		if (wep16index == 1 || wep16index == 4 || wep16index == 10 || wep16index == 14 || wep16index == 15) {
			if (MFDManager.a.overloadButtonLH.activeInHierarchy) {
				MFDManager.a.overloadButtonLH.GetComponent<EnergyOverloadButton>().OverloadButtonAction();
			}

			if (MFDManager.a.overloadButtonRH.activeInHierarchy) {
				MFDManager.a.overloadButtonRH.GetComponent<EnergyOverloadButton>().OverloadButtonAction();
			}
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
		WeaponFire.a.StartWeaponDip(Const.a.reloadTime[wep16index]);

		// Pop it back to start to be sure
		WeaponFire.a.reloadContainer.localPosition =
			WeaponFire.a.reloadContainerHome;
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
		WeaponFire.a.StartWeaponDip(Const.a.reloadTime[wep16index]);

		// Pop it back to start to be sure
		WeaponFire.a.reloadContainer.localPosition =
			WeaponFire.a.reloadContainerHome;
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
		line += Utils.splitChar + Utils.UintToString(wc.weaponCurrentPending);
		line += Utils.splitChar + Utils.UintToString(wc.weaponIndexPending);
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
		wc.weaponCurrentPending = Utils.GetIntFromString(entries[index] ); index++;
		wc.weaponIndexPending = Utils.GetIntFromString(entries[index] ); index++;
		wc.justChangedWeap = true;
		return index;
	}
}