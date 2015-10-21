using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AmmoIconManager : MonoBehaviour {
	[SerializeField] public Sprite[] ammIcons;
	[SerializeField] public GameObject clipBox;

	public void SetAmmoIcon (int index, bool alternateAmmo) {
		if (index >= 0) {
			GetComponent<Image>().enabled = true;
			clipBox.GetComponent<Image>().enabled = true; 
			switch (index) {
			case 0: if (alternateAmmo) {
					GetComponent<Image>().overrideSprite = ammIcons[8];
					} else {
					GetComponent<Image>().overrideSprite = ammIcons[7];
					}
				break;
			case 1: GetComponent<Image>().enabled = false;  // Uses energy
				break;
			case 2: if (alternateAmmo) {
					GetComponent<Image>().overrideSprite = ammIcons[1];
					} else {
					GetComponent<Image>().overrideSprite = ammIcons[0];
					}
				break;
			case 3: if (alternateAmmo) {
					GetComponent<Image>().overrideSprite = ammIcons[10];
					} else {
					GetComponent<Image>().overrideSprite = ammIcons[9];
					}
				break;
			case 4: GetComponent<Image>().enabled = false;  // Uses energy
				clipBox.GetComponent<Image>().enabled = false;
				break;
			case 5: GetComponent<Image>().enabled = false;  // Rapier, no ammo used
				clipBox.GetComponent<Image>().enabled = false;
				break;
			case 6: GetComponent<Image>().enabled = false;  // Pipe, no ammo used
				clipBox.GetComponent<Image>().enabled = false;
				break;
			case 7: if (alternateAmmo) {
					GetComponent<Image>().overrideSprite = ammIcons[6];
					} else {
					GetComponent<Image>().overrideSprite = ammIcons[5];
					}
				break;
			case 8: GetComponent<Image>().overrideSprite = ammIcons[11];
				break;
			case 9: if (alternateAmmo) {
					GetComponent<Image>().overrideSprite = ammIcons[3];
					} else {
					GetComponent<Image>().overrideSprite = ammIcons[2];
					}
				break;
			case 10: GetComponent<Image>().enabled = false;  // Uses energy
				clipBox.GetComponent<Image>().enabled = false;
				break;
			case 11: GetComponent<Image>().overrideSprite = ammIcons[14];
				break;
			case 12: GetComponent<Image>().overrideSprite = ammIcons[4];
				break;
			case 13: if (alternateAmmo) {
					GetComponent<Image>().overrideSprite = ammIcons[13];
					} else {
					GetComponent<Image>().overrideSprite = ammIcons[12];
					}
				break;
			case 14: GetComponent<Image>().enabled = false;  // Uses energy
				clipBox.GetComponent<Image>().enabled = false;
				break;
			case 15: GetComponent<Image>().enabled = false;  // Uses energy
				clipBox.GetComponent<Image>().enabled = false;
				break;
			}
		}
	}
}
