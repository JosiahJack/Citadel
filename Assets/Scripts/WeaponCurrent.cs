using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponCurrent : MonoBehaviour {
	[SerializeField] public int weaponCurrent = new int();
	[SerializeField] public int weaponIndex = new int();
	[SerializeField] public bool weaponIsAlternateAmmo = new bool();
	public float[] weaponEnergySetting;
	public int[] currentMagazineAmount;
	public int[] currentMagazineAmount2;
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
	public bool justChangedWeap = true;
	private int lastIndex = 0;
	private AudioSource SFX;
	public AudioClip ReloadSFX;
	public AudioClip ReloadInStyleSFX;
	public bool bottomless = false; // don't use any ammo (and energy weapons no energy)
	public bool redbull = false; // don't use any energy
	[HideInInspector]
	public float reloadFinished;
	public GameObject reloadContainer;
	public Vector3 reloadContainerOrigin;
	[HideInInspector]
	public float reloadContainerDropAmount = 0.64f;
	[HideInInspector]
	public float reloadLerpValue;
	[HideInInspector]
	public float lerpStartTime;
	[HideInInspector]
	public float targetY;

	void Awake() {
		WepInstance = this;
		WepInstance.weaponCurrent = 0; // Current slot in the weapon inventory (7 slots)
		WepInstance.weaponIndex = 0; // Current index to the weapon look-up tables
		WepInstance.weaponIsAlternateAmmo = false;
		WepInstance.SFX = GetComponent<AudioSource> ();
		// Put energy settings to lowest energy level as default
		for (int i=0;i<16;i++) {
			WepInstance.weaponEnergySetting [i] = 0f; // zero out ones we don't use
			WepInstance.currentMagazineAmount[i] = 0;
			WepInstance.currentMagazineAmount2[i] = 0;
		}

        //Default power settings
		WepInstance.weaponEnergySetting [1] = 3f; // Blaster
		WepInstance.weaponEnergySetting [4] = 5f; // Ion Beam
		WepInstance.weaponEnergySetting [10] = 13f; // Plasma rifle
		WepInstance.weaponEnergySetting [14] = 2f; // Sparq Beam
		WepInstance.weaponEnergySetting [15] = 3f; // Stungun
		reloadFinished = Time.time;
		reloadContainerOrigin = reloadContainer.transform.localPosition;
		reloadLerpValue = 0;
	}

	void Update() {
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex(weaponIndex);

		if (justChangedWeap) {
			justChangedWeap = false;
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
			//ammoIndicatorHuns.SetActive(false);
			//ammoIndicatorTens.SetActive(false);
			//ammoIndicatorOnes.SetActive(false);
		}

		if (lastIndex != weaponIndex) {
			justChangedWeap = true;
			lastIndex = weaponIndex;
		}
		
		switch (weaponIndex) {
		case 36:
			ViewModelAssault.SetActive(true);
			ammoIndicatorHuns.SetActive(true);
			ammoIndicatorTens.SetActive(true);
			ammoIndicatorOnes.SetActive(true);
			if (unloadButton != null) unloadButton.SetActive(true);
			if (loadNormalAmmoButton != null) {
				loadNormalAmmoButton.SetActive(true);
				loadNormalAmmoButtonText.text = "LOAD MAGNESIUM";
			}
			if (loadAlternateAmmoButton != null) {
				loadAlternateAmmoButton.SetActive(true);
				loadAlternateAmmoButtonText.text = "LOAD PENETRATOR";
			}
			break;
		case 37:
			ViewModelBlaster.SetActive(true);
			ammoIndicatorHuns.SetActive(false);
			ammoIndicatorTens.SetActive(false);
			ammoIndicatorOnes.SetActive(false);
			if (energySlider != null) energySlider.SetActive(true);
			if (energyHeatTicks != null) energyHeatTicks.SetActive(true);
			if (overloadButton != null) overloadButton.SetActive(true);
			break;
		case 38:
			ViewModelDartgun.SetActive(true);
			ammoIndicatorHuns.SetActive(true);
			ammoIndicatorTens.SetActive(true);
			ammoIndicatorOnes.SetActive(true);
			if (unloadButton != null) unloadButton.SetActive(true);
			if (loadNormalAmmoButton != null) {
				loadNormalAmmoButton.SetActive(true);
				loadNormalAmmoButtonText.text = "LOAD NEEDLE";
			}
			if (loadAlternateAmmoButton != null) {
				loadAlternateAmmoButton.SetActive(true);
				loadAlternateAmmoButtonText.text = "LOAD TRANQ";
			}
			break;
		case 39:
			ViewModelFlechette.SetActive(true);
			ammoIndicatorHuns.SetActive(true);
			ammoIndicatorTens.SetActive(true);
			ammoIndicatorOnes.SetActive(true);
			if (unloadButton != null) unloadButton.SetActive(true);
			if (loadNormalAmmoButton != null) {
				loadNormalAmmoButton.SetActive(true);
				loadNormalAmmoButtonText.text = "LOAD HORNET";
			}
			if (loadAlternateAmmoButton != null) {
				loadAlternateAmmoButton.SetActive(true);
				loadAlternateAmmoButtonText.text = "LOAD SPLINTER";
			}
			break;
		case 40:
			ViewModelIon.SetActive(true);
			ammoIndicatorHuns.SetActive(false);
			ammoIndicatorTens.SetActive(false);
			ammoIndicatorOnes.SetActive(false);
			if (energySlider != null) energySlider.SetActive(true);
			if (energyHeatTicks != null) energyHeatTicks.SetActive(true);
			if (overloadButton != null) overloadButton.SetActive(true);
			break;
		case 41:
			ViewModelRapier.SetActive(true);
			ammoIndicatorHuns.SetActive(false);
			ammoIndicatorTens.SetActive(false);
			ammoIndicatorOnes.SetActive(false);
			break;
		case 42:
			ViewModelPipe.SetActive(true);
			ammoIndicatorHuns.SetActive(false);
			ammoIndicatorTens.SetActive(false);
			ammoIndicatorOnes.SetActive(false);
			break;
		case 43:
			ViewModelMagnum.SetActive(true);
			ammoIndicatorHuns.SetActive(true);
			ammoIndicatorTens.SetActive(true);
			ammoIndicatorOnes.SetActive(true);
			if (unloadButton != null) unloadButton.SetActive(true);
			if (loadNormalAmmoButton != null) {
				loadNormalAmmoButton.SetActive(true);
				loadNormalAmmoButtonText.text = "LOAD HOLLOW TIP";
			}
			if (loadAlternateAmmoButton != null) {
				loadAlternateAmmoButton.SetActive(true);
				loadAlternateAmmoButtonText.text = "LOAD HEAVY SLUG";
			}
			break;
		case 44:
			ViewModelMagpulse.SetActive(true);
			ammoIndicatorHuns.SetActive(true);
			ammoIndicatorTens.SetActive(true);
			ammoIndicatorOnes.SetActive(true);
			if (unloadButton != null) unloadButton.SetActive(true);
			if (loadNormalAmmoButton != null) {
				loadNormalAmmoButton.SetActive(true);
				loadNormalAmmoButtonText.text = "LOAD CARTRIDGE";
			}
			break;
		case 45:
			ViewModelPistol.SetActive(true);
			ammoIndicatorHuns.SetActive(true);
			ammoIndicatorTens.SetActive(true);
			ammoIndicatorOnes.SetActive(true);
			if (unloadButton != null) unloadButton.SetActive(true);
			if (loadNormalAmmoButton != null) {
				loadNormalAmmoButton.SetActive(true);
				loadNormalAmmoButtonText.text = "LOAD STANDARD";
			}
			if (loadAlternateAmmoButton != null) {
				loadAlternateAmmoButton.SetActive(true);
				loadAlternateAmmoButtonText.text = "LOAD TEFLON";
			}
			break;
		case 46:
			ViewModelPlasma.SetActive(true);
			ammoIndicatorHuns.SetActive(false);
			ammoIndicatorTens.SetActive(false);
			ammoIndicatorOnes.SetActive(false);
			if (energySlider != null) energySlider.SetActive(true);
			if (energyHeatTicks != null) energyHeatTicks.SetActive(true);
			if (overloadButton != null) overloadButton.SetActive(true);
			break;
		case 47:
			ViewModelRailgun.SetActive(true);
			ammoIndicatorHuns.SetActive(true);
			ammoIndicatorTens.SetActive(true);
			ammoIndicatorOnes.SetActive(true);
			if (unloadButton != null) unloadButton.SetActive(true);
			if (loadNormalAmmoButton != null) {
				loadNormalAmmoButton.SetActive(true);
				loadNormalAmmoButtonText.text = "LOAD RAIL CLIP";
			}
			break;
		case 48:
			ViewModelRiotgun.SetActive(true);
			ammoIndicatorHuns.SetActive(true);
			ammoIndicatorTens.SetActive(true);
			ammoIndicatorOnes.SetActive(true);
			if (unloadButton != null) unloadButton.SetActive(true);
			if (loadNormalAmmoButton != null) {
				loadNormalAmmoButton.SetActive(true);
				loadNormalAmmoButtonText.text = "LOAD RUBBER SLUG";
			}
			break;
		case 49:
			ViewModelSkorpion.SetActive(true);
			ammoIndicatorHuns.SetActive(true);
			ammoIndicatorTens.SetActive(true);
			ammoIndicatorOnes.SetActive(true);
			if (unloadButton != null) unloadButton.SetActive(true);
			if (loadNormalAmmoButton != null) {
				loadNormalAmmoButton.SetActive(true);
				loadNormalAmmoButtonText.text = "LOAD SLAG";
			}
			if (loadAlternateAmmoButton != null) {
				loadAlternateAmmoButton.SetActive(true);
				loadAlternateAmmoButtonText.text = "LOAD LARGE SLAG";
			}
			break;
		case 50:
			ViewModelSparq.SetActive(true);
			ammoIndicatorHuns.SetActive(false);
			ammoIndicatorTens.SetActive(false);
			ammoIndicatorOnes.SetActive(false);
			if (energySlider != null) energySlider.SetActive(true);
			if (energyHeatTicks != null) energyHeatTicks.SetActive(true);
			if (overloadButton != null) overloadButton.SetActive(true);
			break;
		case 51:
			ViewModelStungun.SetActive(true);
			ammoIndicatorHuns.SetActive(false);
			ammoIndicatorTens.SetActive(false);
			ammoIndicatorOnes.SetActive(false);
			if (energySlider != null) energySlider.SetActive(true);
			if (energyHeatTicks != null) energyHeatTicks.SetActive(true);
			if (overloadButton != null) overloadButton.SetActive(true);
			break;
		}


		if (wep16index == -1) return; // we don't have a weapon at all right now :)
		weaponIsAlternateAmmo = WeaponAmmo.a.wepLoadedWithAlternate[wep16index];

		if (weaponIsAlternateAmmo) {
			if (loadNormalAmmoButton != null) loadNormalAmmoButton.GetComponent<Image>().overrideSprite = ammoButtonDeHighlighted;
			if (loadAlternateAmmoButton != null) loadAlternateAmmoButton.GetComponent<Image>().overrideSprite = ammoButtonHighlighted;
		} else {
			if (loadNormalAmmoButton != null) loadNormalAmmoButton.GetComponent<Image>().overrideSprite = ammoButtonHighlighted;
			if (loadAlternateAmmoButton != null) loadAlternateAmmoButton.GetComponent<Image>().overrideSprite = ammoButtonDeHighlighted;
		}

		UpdateHUDAmmoCountsEither();
	}

	public void ChangeAmmoType() {
		if (weaponIsAlternateAmmo) {
			weaponIsAlternateAmmo = false;
			LoadPrimaryAmmoType(false);
		} else {
			weaponIsAlternateAmmo = true;
			LoadSecondaryAmmoType(false);
		}
	}

	public void LoadPrimaryAmmoType(bool isSilent) {
		Unload(true);
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex (weaponIndex);
		WeaponAmmo.a.wepLoadedWithAlternate[wep16index] = false;

		// Put bullets into the magazine
		if (WeaponAmmo.a.wepAmmo[wep16index] >= Const.a.magazinePitchCountForWeapon[wep16index]) {
			currentMagazineAmount[wep16index] = Const.a.magazinePitchCountForWeapon[wep16index];
		} else {
			currentMagazineAmount[wep16index] = WeaponAmmo.a.wepAmmo[wep16index];
		}

		// Take bullets out of the ammo stockpile
		WeaponAmmo.a.wepAmmo[wep16index] -= currentMagazineAmount[wep16index];

		if (!isSilent) {
			if (wep16index == 0 || wep16index == 3) {
				SFX.PlayOneShot (ReloadInStyleSFX);
			} else {
				SFX.PlayOneShot (ReloadSFX);
			}
		}

		// Update the counter on the HUD
		MFDManager.a.UpdateHUDAmmoCounts(currentMagazineAmount[wep16index]);
	}

	public void LoadSecondaryAmmoType(bool isSilent) {
		Unload(true);
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex (weaponIndex);
		WeaponAmmo.a.wepLoadedWithAlternate[wep16index] = true;

		// Put bullets into the magazine
		if (WeaponAmmo.a.wepAmmoSecondary[wep16index] >= Const.a.magazinePitchCountForWeapon2[wep16index]) {
			currentMagazineAmount2[wep16index] = Const.a.magazinePitchCountForWeapon2[wep16index];
		} else {
			currentMagazineAmount2[wep16index] = WeaponAmmo.a.wepAmmoSecondary[wep16index];
		}

		// Take bullets out of the ammo stockpile
		WeaponAmmo.a.wepAmmoSecondary[wep16index] -= currentMagazineAmount2[wep16index];

		if (!isSilent) {
			if (wep16index == 0 || wep16index == 3) {
				SFX.PlayOneShot (ReloadInStyleSFX);
			} else {
				SFX.PlayOneShot (ReloadSFX);
			}
		}

		// Update the counter on the HUD
		MFDManager.a.UpdateHUDAmmoCounts(currentMagazineAmount2[wep16index]);
	}

	public void UpdateHUDAmmoCountsEither() {
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex (weaponIndex);
		if (weaponIsAlternateAmmo) {
			MFDManager.a.UpdateHUDAmmoCounts(currentMagazineAmount2[wep16index]);
		} else {
			MFDManager.a.UpdateHUDAmmoCounts(currentMagazineAmount[wep16index]);
		}
	}

	public void Unload(bool isSilent) {
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex (weaponIndex);

		if (wep16index == -1) return; // we don't have a weapon at all right now :)
		weaponIsAlternateAmmo = WeaponAmmo.a.wepLoadedWithAlternate[wep16index];

		// Take bullets out of the clip, put them back into the ammo stockpile, then zero out the clip amount, did I say clip?  I mean magazine but whatever
		if (weaponIsAlternateAmmo) {
			WeaponAmmo.a.wepAmmoSecondary[wep16index] += currentMagazineAmount2[wep16index];
			currentMagazineAmount2[wep16index] = 0;

			// Update the counter on the HUD
			MFDManager.a.UpdateHUDAmmoCounts(currentMagazineAmount2[wep16index]);
		} else {
			WeaponAmmo.a.wepAmmo[wep16index] += currentMagazineAmount[wep16index];
			currentMagazineAmount[wep16index] = 0;

			// Update the counter on the HUD
			MFDManager.a.UpdateHUDAmmoCounts(currentMagazineAmount[wep16index]);
		}
		if (!isSilent) SFX.PlayOneShot (ReloadSFX);


	}

	public void Reload() {
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex (weaponIndex);
		reloadFinished = Time.time + Const.a.reloadTime[wep16index];
		lerpStartTime = Time.time;
		reloadContainer.transform.localPosition = reloadContainerOrigin; // pop it back to start to be sure

		if (weaponIsAlternateAmmo) {
			if (currentMagazineAmount2[wep16index] == Const.a.magazinePitchCountForWeapon2[wep16index]) {
				Const.sprint("Current weapon magazine already full.",owner);
				return;
			}

			if (WeaponAmmo.a.wepAmmoSecondary[wep16index] <= 0) {
				Const.sprint("No more of current ammo type to load.",owner);
				return;
			}
			LoadSecondaryAmmoType(false);
		} else {
			if (currentMagazineAmount[wep16index] == Const.a.magazinePitchCountForWeapon[wep16index]) {
				Const.sprint("Current weapon magazine already full.",owner);
				return;
			}

			if (WeaponAmmo.a.wepAmmo[wep16index] <= 0) {
				Const.sprint("No more of current ammo type to load.",owner);
				return;
			}
			LoadPrimaryAmmoType(false);
		}

	}
}