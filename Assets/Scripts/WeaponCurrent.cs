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
	[HideInInspector] public int weaponCurrent = new int(); // save
	[HideInInspector] public int weaponIndex = new int(); // save
	[HideInInspector] public float[] weaponEnergySetting; // save
	[HideInInspector] public int[] currentMagazineAmount; // save
	[HideInInspector] public int[] currentMagazineAmount2; // save
	[HideInInspector] public int lastIndex = 0; // save
	private AudioSource SFX;
	[HideInInspector] public bool bottomless = false; // don't use any ammo (and energy weapons no energy) // save
	[HideInInspector] public bool redbull = false; // don't use any energy // save
	[HideInInspector] public float reloadFinished; // save
	[HideInInspector] public float reloadContainerDropAmount = 0.66f;
	[HideInInspector] public float reloadLerpValue; // save
	[HideInInspector] public float lerpStartTime; // save
	[HideInInspector] public float targetY; // save
	[HideInInspector] public int weaponCurrentPending; // save
	[HideInInspector] public int weaponIndexPending; // save

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
		if (ViewModelAssault != null) ViewModelAssault.SetActive(false);
		if (ViewModelBlaster != null) ViewModelBlaster.SetActive(false);
		if (ViewModelDartgun != null) ViewModelDartgun.SetActive(false);
		if (ViewModelFlechette != null) ViewModelFlechette.SetActive(false);
		if (ViewModelIon != null) ViewModelIon.SetActive(false);
		if (ViewModelRapier != null) ViewModelRapier.SetActive(false);
		if (ViewModelPipe != null) ViewModelPipe.SetActive(false);
		if (ViewModelMagnum != null) ViewModelMagnum.SetActive(false);
		if (ViewModelMagpulse != null) ViewModelMagpulse.SetActive(false);
		if (ViewModelPistol != null) ViewModelPistol.SetActive(false);
		if (ViewModelPlasma != null) ViewModelPlasma.SetActive(false);
		if (ViewModelRailgun != null) ViewModelRailgun.SetActive(false);
		if (ViewModelRiotgun != null) ViewModelRiotgun.SetActive(false);
		if (ViewModelSkorpion != null) ViewModelSkorpion.SetActive(false);
		if (ViewModelSparq != null) ViewModelSparq.SetActive(false);
		if (ViewModelStungun != null) ViewModelStungun.SetActive(false);
		if (energySlider != null) energySlider.SetActive(false);
		if (energyHeatTicks != null) energyHeatTicks.SetActive(false);
		if (overloadButton != null) overloadButton.SetActive(false);
		if (unloadButton != null) unloadButton.SetActive(false);
		if (loadNormalAmmoButton != null) loadNormalAmmoButton.SetActive(false);
		if (loadAlternateAmmoButton != null) loadAlternateAmmoButton.SetActive(false);
	}

	public void WeaponChange(int useableItemIndex, int WepButtonIndex) {
		if (useableItemIndex == -1 || WepButtonIndex > 6
			|| WepButtonIndex < 0) {
			Debug.Log("Early exit on WeaponChange() in WeaponCurrent.cs!");
			return;
		}

		Utils.PlayOneShotSavable(SFX,WeaponChangeSFX);
		int wep16index =  // Get index into the list of 16 weapons
			  WeaponFire.Get16WeaponIndexFromConstIndex(useableItemIndex);
		float delay = Const.a.reloadTime[wep16index];
		reloadFinished = PauseScript.a.relativeTime + delay;
		lerpStartTime = PauseScript.a.relativeTime;
		weaponCurrentPending = WepButtonIndex;
		weaponIndexPending = useableItemIndex;
		UpdateHUDAmmoCountsEither();
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			if (justChangedWeap) {
				justChangedWeap = false;
				Inventory.a.UpdateAmmoText();
				SetAllViewModelsDeactive();
				UpdateWeaponViewModels();
				if (weaponCurrent >= 0) {
					// Update the ammo icons.
					ammoIconManLH.SetAmmoIcon(weaponIndex,Inventory.a.wepLoadedWithAlternate[weaponCurrent]);
					ammoIconManRH.SetAmmoIcon(weaponIndex,Inventory.a.wepLoadedWithAlternate[weaponCurrent]);
				} else {
					// Clear the ammo icons.
					ammoIconManLH.SetAmmoIcon(-1,false);
					ammoIconManRH.SetAmmoIcon(-1,false);
				}
			}

			if (lastIndex != weaponIndex) {
				justChangedWeap = true;
				lastIndex = weaponIndex;
			}
		}
	}

	public void UpdateWeaponViewModels() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			switch (weaponIndex) {
			case 36:
				if (!ViewModelAssault.activeSelf) ViewModelAssault.SetActive(true);
				if (!ammoIndicatorHuns.activeSelf) ammoIndicatorHuns.SetActive(true);
				if (!ammoIndicatorTens.activeSelf) ammoIndicatorTens.SetActive(true);
				if (!ammoIndicatorOnes.activeSelf) ammoIndicatorOnes.SetActive(true);
				if (energySlider != null && energySlider.activeSelf) energySlider.SetActive(false);
				if (energyHeatTicks != null && energyHeatTicks.activeSelf) energyHeatTicks.SetActive(false);
				if (overloadButton != null && overloadButton.activeSelf) overloadButton.SetActive(false);
				if (unloadButton != null) unloadButton.SetActive(true);
				if (loadNormalAmmoButton != null) {
					if (!loadNormalAmmoButton.activeSelf) loadNormalAmmoButton.SetActive(true);
					loadNormalAmmoButtonText.text = Const.a.stringTable[539]; // "LOAD MAGNESIUM"
				}
				if (loadAlternateAmmoButton != null) {
					if (!loadAlternateAmmoButton.activeSelf) loadAlternateAmmoButton.SetActive(true);
					loadAlternateAmmoButtonText.text = Const.a.stringTable[540]; // "LOAD PENETRATOR"
				}
				break;
			case 37:
				if (!ViewModelBlaster.activeSelf) ViewModelBlaster.SetActive(true);
				if (ammoIndicatorHuns.activeSelf) ammoIndicatorHuns.SetActive(false);
				if (ammoIndicatorTens.activeSelf) ammoIndicatorTens.SetActive(false);
				if (ammoIndicatorOnes.activeSelf) ammoIndicatorOnes.SetActive(false);
				if (energySlider != null && !energySlider.activeSelf) energySlider.SetActive(true);
				if (energyHeatTicks != null && !energyHeatTicks.activeSelf) energyHeatTicks.SetActive(true);
				if (overloadButton != null && !overloadButton.activeSelf) overloadButton.SetActive(true);
				if (loadNormalAmmoButton != null) { if (loadNormalAmmoButton.activeSelf) loadNormalAmmoButton.SetActive(false); }
				if (loadAlternateAmmoButton != null) { if (loadAlternateAmmoButton.activeSelf) loadAlternateAmmoButton.SetActive(false); }
				break;
			case 38:
				if (!ViewModelDartgun.activeSelf) ViewModelDartgun.SetActive(true);
				if (!ammoIndicatorHuns.activeSelf) ammoIndicatorHuns.SetActive(true);
				if (!ammoIndicatorTens.activeSelf) ammoIndicatorTens.SetActive(true);
				if (!ammoIndicatorOnes.activeSelf) ammoIndicatorOnes.SetActive(true);
				if (energySlider != null && energySlider.activeSelf) energySlider.SetActive(false);
				if (energyHeatTicks != null && energyHeatTicks.activeSelf) energyHeatTicks.SetActive(false);
				if (overloadButton != null && overloadButton.activeSelf) overloadButton.SetActive(false);
				if (unloadButton != null) unloadButton.SetActive(true);
				if (loadNormalAmmoButton != null) {
					if (!loadNormalAmmoButton.activeSelf) loadNormalAmmoButton.SetActive(true);
					loadNormalAmmoButtonText.text = Const.a.stringTable[541]; // "LOAD NEEDLE";
				}
				if (loadAlternateAmmoButton != null) {
					if (!loadAlternateAmmoButton.activeSelf) loadAlternateAmmoButton.SetActive(true);
					loadAlternateAmmoButtonText.text = Const.a.stringTable[542]; // "LOAD TRANQ";
				}
				break;
			case 39:
				ViewModelFlechette.SetActive(true);
				if (!ammoIndicatorHuns.activeSelf) ammoIndicatorHuns.SetActive(true);
				if (!ammoIndicatorTens.activeSelf) ammoIndicatorTens.SetActive(true);
				if (!ammoIndicatorOnes.activeSelf) ammoIndicatorOnes.SetActive(true);
				if (energySlider != null && energySlider.activeSelf) energySlider.SetActive(false);
				if (energyHeatTicks != null && energyHeatTicks.activeSelf) energyHeatTicks.SetActive(false);
				if (overloadButton != null && overloadButton.activeSelf) overloadButton.SetActive(false);
				if (unloadButton != null) unloadButton.SetActive(true);
				if (loadNormalAmmoButton != null) {
					if (!loadNormalAmmoButton.activeSelf) loadNormalAmmoButton.SetActive(true);
					loadNormalAmmoButtonText.text = Const.a.stringTable[543]; // "LOAD HORNET";
				}
				if (loadAlternateAmmoButton != null) {
					if (!loadAlternateAmmoButton.activeSelf) loadAlternateAmmoButton.SetActive(true);
					loadAlternateAmmoButtonText.text = Const.a.stringTable[544]; // "LOAD SPLINTER";
				}
				break;
			case 40:
				ViewModelIon.SetActive(true);
				if (ammoIndicatorHuns.activeSelf) ammoIndicatorHuns.SetActive(false);
				if (ammoIndicatorTens.activeSelf) ammoIndicatorTens.SetActive(false);
				if (ammoIndicatorOnes.activeSelf) ammoIndicatorOnes.SetActive(false);
				if (energySlider != null && !energySlider.activeSelf) energySlider.SetActive(true);
				if (energyHeatTicks != null && !energyHeatTicks.activeSelf) energyHeatTicks.SetActive(true);
				if (overloadButton != null && !overloadButton.activeSelf) overloadButton.SetActive(true);
				if (loadNormalAmmoButton != null) { if (loadNormalAmmoButton.activeSelf) loadNormalAmmoButton.SetActive(false); }
				if (loadAlternateAmmoButton != null) { if (loadAlternateAmmoButton.activeSelf) loadAlternateAmmoButton.SetActive(false); }
				break;
			case 41:
				ViewModelRapier.SetActive(true);
				if (ammoIndicatorHuns.activeSelf) ammoIndicatorHuns.SetActive(false);
				if (ammoIndicatorTens.activeSelf) ammoIndicatorTens.SetActive(false);
				if (ammoIndicatorOnes.activeSelf) ammoIndicatorOnes.SetActive(false);
				if (energySlider != null && energySlider.activeSelf) energySlider.SetActive(false);
				if (energyHeatTicks != null && energyHeatTicks.activeSelf) energyHeatTicks.SetActive(false);
				if (overloadButton != null && overloadButton.activeSelf) overloadButton.SetActive(false);
				if (loadNormalAmmoButton != null) { if (loadNormalAmmoButton.activeSelf) loadNormalAmmoButton.SetActive(false); }
				if (loadAlternateAmmoButton != null) { if (loadAlternateAmmoButton.activeSelf) loadAlternateAmmoButton.SetActive(false); }
				break;
			case 42:
				ViewModelPipe.SetActive(true);
				if (ammoIndicatorHuns.activeSelf) ammoIndicatorHuns.SetActive(false);
				if (ammoIndicatorTens.activeSelf) ammoIndicatorTens.SetActive(false);
				if (ammoIndicatorOnes.activeSelf) ammoIndicatorOnes.SetActive(false);
				if (energySlider != null && energySlider.activeSelf) energySlider.SetActive(false);
				if (energyHeatTicks != null && energyHeatTicks.activeSelf) energyHeatTicks.SetActive(false);
				if (overloadButton != null && overloadButton.activeSelf) overloadButton.SetActive(false);
				if (loadNormalAmmoButton != null) { if (loadNormalAmmoButton.activeSelf) loadNormalAmmoButton.SetActive(false); }
				if (loadAlternateAmmoButton != null) { if (loadAlternateAmmoButton.activeSelf) loadAlternateAmmoButton.SetActive(false); }
				break;
			case 43:
				ViewModelMagnum.SetActive(true);
				if (!ammoIndicatorHuns.activeSelf) ammoIndicatorHuns.SetActive(true);
				if (!ammoIndicatorTens.activeSelf) ammoIndicatorTens.SetActive(true);
				if (!ammoIndicatorOnes.activeSelf) ammoIndicatorOnes.SetActive(true);
				if (unloadButton != null) unloadButton.SetActive(true);
				if (loadNormalAmmoButton != null) {
					if (!loadNormalAmmoButton.activeSelf) loadNormalAmmoButton.SetActive(true);
					loadNormalAmmoButtonText.text = Const.a.stringTable[545]; // "LOAD HOLLOW TIP";
				}
				if (loadAlternateAmmoButton != null) {
					if (!loadAlternateAmmoButton.activeSelf) loadAlternateAmmoButton.SetActive(true);
					loadAlternateAmmoButtonText.text = Const.a.stringTable[546]; // "LOAD HEAVY SLUG";
				}
				break;
			case 44:
				ViewModelMagpulse.SetActive(true);
				if (!ammoIndicatorHuns.activeSelf) ammoIndicatorHuns.SetActive(true);
				if (!ammoIndicatorTens.activeSelf) ammoIndicatorTens.SetActive(true);
				if (!ammoIndicatorOnes.activeSelf) ammoIndicatorOnes.SetActive(true);
				if (energySlider != null && energySlider.activeSelf) energySlider.SetActive(false);
				if (energyHeatTicks != null && energyHeatTicks.activeSelf) energyHeatTicks.SetActive(false);
				if (overloadButton != null && overloadButton.activeSelf) overloadButton.SetActive(false);
				if (unloadButton != null) unloadButton.SetActive(true);
				if (loadNormalAmmoButton != null) {
					if (!loadNormalAmmoButton.activeSelf) loadNormalAmmoButton.SetActive(true);
					loadNormalAmmoButtonText.text = Const.a.stringTable[547]; // "LOAD CARTRIDGE";
				}
				break;
			case 45:
				ViewModelPistol.SetActive(true);
				if (!ammoIndicatorHuns.activeSelf) ammoIndicatorHuns.SetActive(true);
				if (!ammoIndicatorTens.activeSelf) ammoIndicatorTens.SetActive(true);
				if (!ammoIndicatorOnes.activeSelf) ammoIndicatorOnes.SetActive(true);
				if (energySlider != null && energySlider.activeSelf) energySlider.SetActive(false);
				if (energyHeatTicks != null && energyHeatTicks.activeSelf) energyHeatTicks.SetActive(false);
				if (overloadButton != null && overloadButton.activeSelf) overloadButton.SetActive(false);
				if (unloadButton != null) unloadButton.SetActive(true);
				if (loadNormalAmmoButton != null) {
					if (!loadNormalAmmoButton.activeSelf) loadNormalAmmoButton.SetActive(true);
					loadNormalAmmoButtonText.text = Const.a.stringTable[548]; // "LOAD STANDARD";
				}
				if (loadAlternateAmmoButton != null) {
					if (!loadAlternateAmmoButton.activeSelf) loadAlternateAmmoButton.SetActive(true);
					loadAlternateAmmoButtonText.text = Const.a.stringTable[549]; // "LOAD TEFLON";
				}
				break;
			case 46:
				ViewModelPlasma.SetActive(true);
				if (ammoIndicatorHuns.activeSelf) ammoIndicatorHuns.SetActive(false);
				if (ammoIndicatorTens.activeSelf) ammoIndicatorTens.SetActive(false);
				if (ammoIndicatorOnes.activeSelf) ammoIndicatorOnes.SetActive(false);
				if (energySlider != null && !energySlider.activeSelf) energySlider.SetActive(true);
				if (energyHeatTicks != null && !energyHeatTicks.activeSelf) energyHeatTicks.SetActive(true);
				if (overloadButton != null && !overloadButton.activeSelf) overloadButton.SetActive(true);
				if (loadNormalAmmoButton != null) { if (loadNormalAmmoButton.activeSelf) loadNormalAmmoButton.SetActive(false); }
				if (loadAlternateAmmoButton != null) { if (loadAlternateAmmoButton.activeSelf) loadAlternateAmmoButton.SetActive(false); }
				break;
			case 47:
				ViewModelRailgun.SetActive(true);
				if (!ammoIndicatorHuns.activeSelf) ammoIndicatorHuns.SetActive(true);
				if (!ammoIndicatorTens.activeSelf) ammoIndicatorTens.SetActive(true);
				if (!ammoIndicatorOnes.activeSelf) ammoIndicatorOnes.SetActive(true);
				if (energySlider != null && energySlider.activeSelf) energySlider.SetActive(false);
				if (energyHeatTicks != null && energyHeatTicks.activeSelf) energyHeatTicks.SetActive(false);
				if (overloadButton != null && overloadButton.activeSelf) overloadButton.SetActive(false);
				if (unloadButton != null) unloadButton.SetActive(true);
				if (loadNormalAmmoButton != null) {
					if (!loadNormalAmmoButton.activeSelf) loadNormalAmmoButton.SetActive(true);
					loadNormalAmmoButtonText.text = Const.a.stringTable[550]; // "LOAD RAIL CLIP";
				}
				break;
			case 48:
				ViewModelRiotgun.SetActive(true);
				if (!ammoIndicatorHuns.activeSelf) ammoIndicatorHuns.SetActive(true);
				if (!ammoIndicatorTens.activeSelf) ammoIndicatorTens.SetActive(true);
				if (!ammoIndicatorOnes.activeSelf) ammoIndicatorOnes.SetActive(true);
				if (energySlider != null && energySlider.activeSelf) energySlider.SetActive(false);
				if (energyHeatTicks != null && energyHeatTicks.activeSelf) energyHeatTicks.SetActive(false);
				if (overloadButton != null && overloadButton.activeSelf) overloadButton.SetActive(false);
				if (unloadButton != null) unloadButton.SetActive(true);
				if (loadNormalAmmoButton != null) {
					if (!loadNormalAmmoButton.activeSelf) loadNormalAmmoButton.SetActive(true);
					loadNormalAmmoButtonText.text = Const.a.stringTable[551]; // "LOAD RUBBER SLUG";
				}
				break;
			case 49:
				ViewModelSkorpion.SetActive(true);
				if (!ammoIndicatorHuns.activeSelf) ammoIndicatorHuns.SetActive(true);
				if (!ammoIndicatorTens.activeSelf) ammoIndicatorTens.SetActive(true);
				if (!ammoIndicatorOnes.activeSelf) ammoIndicatorOnes.SetActive(true);
				if (energySlider != null && energySlider.activeSelf) energySlider.SetActive(false);
				if (energyHeatTicks != null && energyHeatTicks.activeSelf) energyHeatTicks.SetActive(false);
				if (overloadButton != null && overloadButton.activeSelf) overloadButton.SetActive(false);
				if (unloadButton != null) unloadButton.SetActive(true);
				if (loadNormalAmmoButton != null) {
					if (!loadNormalAmmoButton.activeSelf) loadNormalAmmoButton.SetActive(true);
					loadNormalAmmoButtonText.text = Const.a.stringTable[552]; // "LOAD SLAG";
				}
				if (loadAlternateAmmoButton != null) {
					if (!loadAlternateAmmoButton.activeSelf) loadAlternateAmmoButton.SetActive(true);
					loadAlternateAmmoButtonText.text = Const.a.stringTable[553]; // "LOAD LARGE SLAG";
				}
				break;
			case 50:
				ViewModelSparq.SetActive(true);
				if (ammoIndicatorHuns.activeSelf) ammoIndicatorHuns.SetActive(false);
				if (ammoIndicatorTens.activeSelf) ammoIndicatorTens.SetActive(false);
				if (ammoIndicatorOnes.activeSelf) ammoIndicatorOnes.SetActive(false);
				if (energySlider != null && !energySlider.activeSelf) energySlider.SetActive(true);
				if (energyHeatTicks != null && !energyHeatTicks.activeSelf) energyHeatTicks.SetActive(true);
				if (overloadButton != null && !overloadButton.activeSelf) overloadButton.SetActive(true);
				if (loadNormalAmmoButton != null) { if (loadNormalAmmoButton.activeSelf) loadNormalAmmoButton.SetActive(false); }
				if (loadAlternateAmmoButton != null) { if (loadAlternateAmmoButton.activeSelf) loadAlternateAmmoButton.SetActive(false); }
				break;
			case 51:
				ViewModelStungun.SetActive(true);
				if (ammoIndicatorHuns.activeSelf) ammoIndicatorHuns.SetActive(false);
				if (ammoIndicatorTens.activeSelf) ammoIndicatorTens.SetActive(false);
				if (ammoIndicatorOnes.activeSelf) ammoIndicatorOnes.SetActive(false);
				if (energySlider != null && !energySlider.activeSelf) energySlider.SetActive(true);
				if (energyHeatTicks != null && !energyHeatTicks.activeSelf) energyHeatTicks.SetActive(true);
				if (overloadButton != null && !overloadButton.activeSelf) overloadButton.SetActive(true);
				if (loadNormalAmmoButton != null) { if (loadNormalAmmoButton.activeSelf) loadNormalAmmoButton.SetActive(false); }
				if (loadAlternateAmmoButton != null) { if (loadAlternateAmmoButton.activeSelf) loadAlternateAmmoButton.SetActive(false); }
				break;
			}

			if (weaponCurrent >= 0) {
				if (Inventory.a.wepLoadedWithAlternate[weaponCurrent]) {
					if (loadNormalAmmoButton != null && loadNormalAmmoButton.GetComponent<Image>().overrideSprite != ammoButtonDeHighlighted) loadNormalAmmoButton.GetComponent<Image>().overrideSprite = ammoButtonDeHighlighted;
					if (loadAlternateAmmoButton != null && loadAlternateAmmoButton.GetComponent<Image>().overrideSprite != ammoButtonHighlighted) loadAlternateAmmoButton.GetComponent<Image>().overrideSprite = ammoButtonHighlighted;
				} else {
					if (loadNormalAmmoButton != null && loadNormalAmmoButton.GetComponent<Image>().overrideSprite != ammoButtonHighlighted) loadNormalAmmoButton.GetComponent<Image>().overrideSprite = ammoButtonHighlighted;
					if (loadAlternateAmmoButton != null && loadAlternateAmmoButton.GetComponent<Image>().overrideSprite != ammoButtonDeHighlighted) loadAlternateAmmoButton.GetComponent<Image>().overrideSprite = ammoButtonDeHighlighted;
				}
				UpdateHUDAmmoCountsEither();
			}
		}
	}

	public void ChangeAmmoType() {
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex(weaponIndex);
		if (wep16index == 5 || wep16index == 6) { Const.sprint(Const.a.stringTable[315],owner); return; } // Do nothing for pipe or rapier.

		if (wep16index == 1 || wep16index == 4 || wep16index == 10 || wep16index == 14 || wep16index == 15) {
			if (overloadButton.activeInHierarchy) overloadButton.GetComponent<EnergyOverloadButton>().OverloadEnergyClick();
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
		Unload(true);
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex (weaponIndex);
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
		Unload(true);
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex (weaponIndex);
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
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex (weaponIndex);
		if (wep16index == 5 || wep16index == 6) {
			Const.sprint(Const.a.stringTable[315],owner); // Weapon does not need reloaded.
			return; // do nothing for pipe or rapier
		}

		if (wep16index == 1 || wep16index == 4 || wep16index == 10 || wep16index == 14 || wep16index == 15) {
			Const.sprint(Const.a.stringTable[538],owner); // We
			return; // do nothing for energy weapons
		}

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
		if (wc == null || index < 0 || entries == null) return index + 31;

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