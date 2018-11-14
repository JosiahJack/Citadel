using UnityEngine;
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
	private bool justChangedWeap = true;
	private int lastIndex = 0;
	private AudioSource SFX;
	public AudioClip ReloadSFX;
	public AudioClip ReloadInStyleSFX;

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
	}

	void Update() {
		if (justChangedWeap) {
			justChangedWeap = false;
			if (ViewModelPipe != null) ViewModelPipe.SetActive(false);
			if (ViewModelRapier != null) ViewModelRapier.SetActive(false);
		}

		if (lastIndex != weaponIndex) {
			justChangedWeap = true;
			lastIndex = weaponIndex;
		}
		
		switch (weaponIndex) {
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
		default:
			ammoIndicatorHuns.SetActive(true);
			ammoIndicatorTens.SetActive(true);
			ammoIndicatorOnes.SetActive(true);
			break;
		}
	}

	public void ChangeAmmoType() {
		if (weaponIsAlternateAmmo) {
			weaponIsAlternateAmmo = false;
			Unload(true);
			LoadPrimaryAmmoType(false);
		} else {
			weaponIsAlternateAmmo = true;
			Unload(true);
			LoadSecondaryAmmoType(false);
		}
	}

	public void LoadPrimaryAmmoType(bool isSilent) {
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex (weaponIndex);

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
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex (weaponIndex);

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
		MFDManager.a.UpdateHUDAmmoCounts(currentMagazineAmount[wep16index]);
	}

	public void Unload(bool isSilent) {
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex (weaponIndex);

		// Take bullets out of the clip, put them back into the ammo stockpile, then zero out the clip amount, did I say clip?  I mean magazine but whatever
		if (weaponIsAlternateAmmo) {
			WeaponAmmo.a.wepAmmoSecondary[wep16index] += currentMagazineAmount2[wep16index];
			currentMagazineAmount2[wep16index] = 0;
		} else {
			WeaponAmmo.a.wepAmmo[wep16index] += currentMagazineAmount[wep16index];
			currentMagazineAmount[wep16index] = 0;
		}
		if (!isSilent) SFX.PlayOneShot (ReloadSFX);

		// Update the counter on the HUD
		MFDManager.a.UpdateHUDAmmoCounts(currentMagazineAmount[wep16index]);
	}

	public void Reload() {
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex (weaponIndex);

		if (weaponIsAlternateAmmo) {
			if (currentMagazineAmount2[wep16index] == Const.a.magazinePitchCountForWeapon2[wep16index]) {
				Const.sprint("Current weapon magazine already full.",owner);
				return;
			}

			if (WeaponAmmo.a.wepAmmoSecondary[wep16index] <= 0) {
				Const.sprint("No more of current ammo type to load.",owner);
				return;
			}
			Unload(true);
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

			Unload(true);
			LoadPrimaryAmmoType(false);
		}

	}
}