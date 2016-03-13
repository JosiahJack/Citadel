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
			case 36: if (alternateAmmo) {
					GetComponent<Image>().overrideSprite = ammIcons[8]; // penetrator, MK3
					} else {
					GetComponent<Image>().overrideSprite = ammIcons[7]; // magnesium, MK3
					}
				break;
			case 37: GetComponent<Image>().enabled = false;  // Uses energy, blaster
				break;
			case 38: if (alternateAmmo) {
					GetComponent<Image>().overrideSprite = ammIcons[1]; // tranq darts, dartgun
					} else {
					GetComponent<Image>().overrideSprite = ammIcons[0]; // needle darts, dartgun
					}
				break;
			case 39: if (alternateAmmo) {
					GetComponent<Image>().overrideSprite = ammIcons[10]; // splinter, flechette
					} else {
					GetComponent<Image>().overrideSprite = ammIcons[9]; // hornet, flechette
					}
				break;
			case 40: GetComponent<Image>().enabled = false;  // Uses energy, ion beam
				clipBox.GetComponent<Image>().enabled = false;
				break;
			case 41: GetComponent<Image>().enabled = false;  // Rapier, no ammo used
				clipBox.GetComponent<Image>().enabled = false;
				break;
			case 42: GetComponent<Image>().enabled = false;  // Pipe, no ammo used
				clipBox.GetComponent<Image>().enabled = false;
				break;
			case 43: if (alternateAmmo) {
					GetComponent<Image>().overrideSprite = ammIcons[6]; // slug, magnum
					} else {
					GetComponent<Image>().overrideSprite = ammIcons[5]; // hollow, magnum
					}
				break;
			case 44: GetComponent<Image>().overrideSprite = ammIcons[11]; // magcart, magpulse
				break;
			case 45: if (alternateAmmo) {
					GetComponent<Image>().overrideSprite = ammIcons[3]; // teflon, pistol
					} else {
					GetComponent<Image>().overrideSprite = ammIcons[2]; // standard, pistol
					}
				break;
			case 46: GetComponent<Image>().enabled = false;  // Uses energy, plasma rifle
				clipBox.GetComponent<Image>().enabled = false;
				break;
			case 47: GetComponent<Image>().overrideSprite = ammIcons[14]; // rail round, railgun
				break;
			case 48: GetComponent<Image>().overrideSprite = ammIcons[4]; // rubber, riotgun
				break;
			case 49: if (alternateAmmo) {
					GetComponent<Image>().overrideSprite = ammIcons[13]; // large slag, skorpion
					} else {
					GetComponent<Image>().overrideSprite = ammIcons[12]; // slag, skorpion
					}
				break;
			case 50: GetComponent<Image>().enabled = false;  // Uses energy, sparq beam
				clipBox.GetComponent<Image>().enabled = false;
				break;
			case 51: GetComponent<Image>().enabled = false;  // Uses energy, stungun
				clipBox.GetComponent<Image>().enabled = false;
				break;
			}
		}
	}
}
