using UnityEngine;
using System.Collections;

public class WeaponCurrent : MonoBehaviour {
	[SerializeField] public int weaponCurrent = new int();
	[SerializeField] public int weaponIndex = new int();
	[SerializeField] public bool weaponIsAlternateAmmo = new bool();
	public static WeaponCurrent WepInstance;
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
	private bool justChangedWeap = true;
	private int lastIndex = 0;

	void Awake() {
		WepInstance = this;
		WepInstance.weaponCurrent = 0; // Current slot in the weapon inventory (7 slots)
		WepInstance.weaponIndex = 0; // Current index to the weapon look-up tables
		WepInstance.weaponIsAlternateAmmo = false;
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
			break;
		case 42:
			ViewModelPipe.SetActive(true);
			break;
		}
	}
}