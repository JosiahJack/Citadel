using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponCurrent : MonoBehaviour {
	[SerializeField] public int weaponCurrent = new int(); // save
	[SerializeField] public int weaponIndex = new int(); // save
	public float[] weaponEnergySetting; // save
	public int[] currentMagazineAmount; // save
	public int[] currentMagazineAmount2; // save
	public static WeaponCurrent WepInstance;
	public WeaponMagazineCounter wepmagCounter;
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
	public bool justChangedWeap = true; // save
	[HideInInspector]
	public int lastIndex = 0; // save
	private AudioSource SFX;
	public AudioClip ReloadSFX;
	public AudioClip ReloadInStyleSFX;
	public bool bottomless = false; // don't use any ammo (and energy weapons no energy) // save
	public bool redbull = false; // don't use any energy // save
	[HideInInspector]
	public float reloadFinished; // save
	public GameObject reloadContainer;
	public Vector3 reloadContainerOrigin;
	[HideInInspector]
	public float reloadContainerDropAmount = 0.64f;
	[HideInInspector]
	public float reloadLerpValue; // save
	[HideInInspector]
	public float lerpStartTime; // save
	[HideInInspector]
	public float targetY; // save

	void Start() {
		WepInstance = this;
		WepInstance.weaponCurrent = 0; // Current slot in the weapon inventory (7 slots)
		WepInstance.weaponIndex = -1; // Current index to the weapon look-up tables
		WepInstance.SFX = GetComponent<AudioSource> ();
		// Put energy settings to lowest energy level as default
		for (int j=0;j<7;j++) {
			WepInstance.weaponEnergySetting[j] = 0f;
			WepInstance.currentMagazineAmount[j] = 0;
			WepInstance.currentMagazineAmount2[j] = 0;
		}
		reloadFinished = PauseScript.a.relativeTime;
		reloadContainerOrigin = reloadContainer.transform.localPosition;
		reloadLerpValue = 0;
	}

	public float GetDefaultEnergySettingForWeaponFrom16Index(int wep16Index) {
		switch (wep16Index) {
			case  1: return  3f; // Blaster
			case  4: return  5f; // Ion Beam
			case 10: return 13f; // Plasma rifle
			case 14: return  2f; // Sparq Beam
			case 15: return  3f; // Stungun
		}
		return 3f;
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

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			if (justChangedWeap) {
				justChangedWeap = false;
				SetAllViewModelsDeactive();
			}

			if (lastIndex != weaponIndex) {
				justChangedWeap = true;
				lastIndex = weaponIndex;
			}
			
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
					loadNormalAmmoButtonText.text = "LOAD MAGNESIUM";
				}
				if (loadAlternateAmmoButton != null) {
					if (!loadAlternateAmmoButton.activeSelf) loadAlternateAmmoButton.SetActive(true);
					loadAlternateAmmoButtonText.text = "LOAD PENETRATOR";
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
					loadNormalAmmoButtonText.text = "LOAD NEEDLE";
				}
				if (loadAlternateAmmoButton != null) {
					if (!loadAlternateAmmoButton.activeSelf) loadAlternateAmmoButton.SetActive(true);
					loadAlternateAmmoButtonText.text = "LOAD TRANQ";
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
					loadNormalAmmoButtonText.text = "LOAD HORNET";
				}
				if (loadAlternateAmmoButton != null) {
					if (!loadAlternateAmmoButton.activeSelf) loadAlternateAmmoButton.SetActive(true);
					loadAlternateAmmoButtonText.text = "LOAD SPLINTER";
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
					loadNormalAmmoButtonText.text = "LOAD HOLLOW TIP";
				}
				if (loadAlternateAmmoButton != null) {
					if (!loadAlternateAmmoButton.activeSelf) loadAlternateAmmoButton.SetActive(true);
					loadAlternateAmmoButtonText.text = "LOAD HEAVY SLUG";
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
					loadNormalAmmoButtonText.text = "LOAD CARTRIDGE";
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
					loadNormalAmmoButtonText.text = "LOAD STANDARD";
				}
				if (loadAlternateAmmoButton != null) {
					if (!loadAlternateAmmoButton.activeSelf) loadAlternateAmmoButton.SetActive(true);
					loadAlternateAmmoButtonText.text = "LOAD TEFLON";
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
					loadNormalAmmoButtonText.text = "LOAD RAIL CLIP";
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
					loadNormalAmmoButtonText.text = "LOAD RUBBER SLUG";
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
					loadNormalAmmoButtonText.text = "LOAD SLAG";
				}
				if (loadAlternateAmmoButton != null) {
					if (!loadAlternateAmmoButton.activeSelf) loadAlternateAmmoButton.SetActive(true);
					loadAlternateAmmoButtonText.text = "LOAD LARGE SLAG";
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
				if (WeaponAmmo.a.wepLoadedWithAlternate[weaponCurrent]) {
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
		if (wep16index == 5 || wep16index == 6) {
			Const.sprint(Const.a.stringTable[315],owner);
			return; // do nothing for pipe or rapier
		}

		if (wep16index == 1 || wep16index == 4 || wep16index == 10 || wep16index == 14 || wep16index == 15) {
			if (overloadButton.activeInHierarchy) overloadButton.GetComponent<EnergyOverloadButton>().OverloadEnergyClick();
		} else {
			//Unload(true);
			if (WeaponAmmo.a.wepLoadedWithAlternate[weaponCurrent]) {
				if (WeaponAmmo.a.wepAmmo[wep16index] > 0) {
				WeaponAmmo.a.wepLoadedWithAlternate[weaponCurrent] = false;
				ReloadSecret(true);
				//LoadPrimaryAmmoType(false);
				} else {
					Const.sprint(Const.a.stringTable[535],owner); //No more of ammo type to load.
				}
			} else {
				if (WeaponAmmo.a.wepAmmoSecondary[wep16index] > 0) {
					WeaponAmmo.a.wepLoadedWithAlternate[weaponCurrent] = true;
					ReloadSecret(true);
					//LoadSecondaryAmmoType(false);
				} else {
					Const.sprint(Const.a.stringTable[535],owner); //No more of ammo type to load.
				}
			}
		}
	}

	public void LoadPrimaryAmmoType(bool isSilent) {
		Unload(true);
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex (weaponIndex);
		WeaponAmmo.a.wepLoadedWithAlternate[weaponCurrent] = false;

		// Put bullets into the magazine
		if (WeaponAmmo.a.wepAmmo[wep16index] >= Const.a.magazinePitchCountForWeapon[wep16index]) {
			currentMagazineAmount[weaponCurrent] = Const.a.magazinePitchCountForWeapon[wep16index];
		} else {
			currentMagazineAmount[weaponCurrent] = WeaponAmmo.a.wepAmmo[wep16index];
		}

		// Take bullets out of the ammo stockpile
		WeaponAmmo.a.wepAmmo[wep16index] -= currentMagazineAmount[weaponCurrent];

		if (!isSilent) {
			if (wep16index == 0 || wep16index == 3) {
				SFX.PlayOneShot (ReloadInStyleSFX);
			} else {
				SFX.PlayOneShot (ReloadSFX);
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
		WeaponAmmo.a.wepLoadedWithAlternate[weaponCurrent] = true;

		// Put bullets into the magazine
		if (WeaponAmmo.a.wepAmmoSecondary[wep16index] >= Const.a.magazinePitchCountForWeapon2[wep16index]) {
			currentMagazineAmount2[weaponCurrent] = Const.a.magazinePitchCountForWeapon2[wep16index];
		} else {
			currentMagazineAmount2[weaponCurrent] = WeaponAmmo.a.wepAmmoSecondary[wep16index];
		}

		// Take bullets out of the ammo stockpile
		WeaponAmmo.a.wepAmmoSecondary[wep16index] -= currentMagazineAmount2[weaponCurrent];

		if (!isSilent) {
			if (wep16index == 0 || wep16index == 3) {
				SFX.PlayOneShot (ReloadInStyleSFX);
			} else {
				SFX.PlayOneShot (ReloadSFX);
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
			if (WeaponAmmo.a.wepLoadedWithAlternate[weaponCurrent]) {
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
		if (WeaponAmmo.a.wepLoadedWithAlternate[weaponCurrent]) {
			WeaponAmmo.a.wepAmmoSecondary[wep16index] += currentMagazineAmount2[weaponCurrent];
			currentMagazineAmount2[weaponCurrent] = 0;

			// Update the counter on the HUD
			MFDManager.a.UpdateHUDAmmoCounts(currentMagazineAmount2[weaponCurrent]);
		} else {
			WeaponAmmo.a.wepAmmo[wep16index] += currentMagazineAmount[weaponCurrent];
			currentMagazineAmount[weaponCurrent] = 0;

			// Update the counter on the HUD
			MFDManager.a.UpdateHUDAmmoCounts(currentMagazineAmount[weaponCurrent]);
		}
		if (!isSilent) SFX.PlayOneShot (ReloadSFX);
	}

	public void ReloadSecret(bool isSilent) {
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex (weaponIndex);
		if (wep16index == 5 || wep16index == 6) {
			Const.sprint(Const.a.stringTable[315],owner);
			return; // do nothing for pipe or rapier
		}

		if (wep16index == 1 || wep16index == 4 || wep16index == 10 || wep16index == 14 || wep16index == 15) {
			return; // do nothing for energy weapons
		}

		if (WeaponAmmo.a.wepLoadedWithAlternate[weaponCurrent]) {
			if (currentMagazineAmount2[weaponCurrent] == Const.a.magazinePitchCountForWeapon2[wep16index]) {
				Const.sprint(Const.a.stringTable[191],owner); //Current weapon magazine already full.
				return;
			}

			if (WeaponAmmo.a.wepAmmoSecondary[wep16index] <= 0) {
				if (WeaponAmmo.a.wepAmmo[wep16index] <= 0) {
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

			if (WeaponAmmo.a.wepAmmo[wep16index] <= 0) {
				if (WeaponAmmo.a.wepAmmoSecondary[wep16index] <= 0) {
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
}