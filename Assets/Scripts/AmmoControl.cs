using UnityEngine;

public class AmmoControl : MonoBehaviour {
	public GameObject noAmmoIcon;
	public GameObject curcntDigitOnes;
	public GameObject curcntDigitTens;
	public GameObject curcntDigitHuns;
	public float currentAmmo;

	void Awake() {
		if (noAmmoIcon == null) Debug.Log("BUG: AmmoControl missing manually assigned reference for noAmmoIcon");
		if (curcntDigitOnes == null) Debug.Log("BUG: AmmoControl missing manually assigned reference for curcntDigitOnes");
		if (curcntDigitTens == null) Debug.Log("BUG: AmmoControl missing manually assigned reference for curcntDigitTens");
		if (curcntDigitHuns == null) Debug.Log("BUG: AmmoControl missing manually assigned reference for curcntDigitHuns");
	}
}
