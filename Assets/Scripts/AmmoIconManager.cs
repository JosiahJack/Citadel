using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AmmoIconManager : MonoBehaviour {
	// External references, required
	public Sprite[] ammIcons;
	public GameObject clipBox;
    public GameObject energySlider;
    public GameObject energyHeatTicks;
    public GameObject energyOverloadButton;
    public Image border;
    public Image icon;

    public void SetAmmoIcon (int index, bool alternateAmmo) {
        switch (index) {
			case 36:
                if (alternateAmmo) {
                    icon.overrideSprite = ammIcons[8]; // penetrator, MK3
				} else {
                    icon.overrideSprite = ammIcons[7]; // magnesium, MK3
				}
				break;
			case 37:
				if (icon.enabled) icon.enabled = false;  // Uses energy, blaster
				if (border.enabled) border.enabled = false;
                if (!energySlider.activeSelf) energySlider.SetActive(true);
                if (!energyHeatTicks.activeSelf) energyHeatTicks.SetActive(true);
                if (!energyOverloadButton.activeSelf) energyOverloadButton.SetActive(true);
                break;
			case 38:
                if (alternateAmmo) {
                    icon.overrideSprite = ammIcons[1]; // tranq darts, dartgun
				} else {
                    icon.overrideSprite = ammIcons[0]; // needle darts, dartgun
				}
				break;
            case 39:
                if (alternateAmmo) {
                    icon.overrideSprite = ammIcons[10]; // splinter, flechette
				} else {
                    icon.overrideSprite = ammIcons[9]; // hornet, flechette
				}
				break;
			case 40:
				if (icon.enabled) icon.enabled = false;  // Uses energy, ion beam
				if (border.enabled) border.enabled = false;
                if (!energySlider.activeSelf) energySlider.SetActive(true);
                if (!energyHeatTicks.activeSelf) energyHeatTicks.SetActive(true);
                if (!energyOverloadButton.activeSelf) energyOverloadButton.SetActive(true);
                break;
                case 41:
                icon.enabled = false;  // Rapier, no ammo used
				border.enabled = false;
				break;
			case 42:
                icon.enabled = false;  // Pipe, no ammo used
				border.enabled = false;
				break;
			case 43:
                if (alternateAmmo) {
                    icon.overrideSprite = ammIcons[6]; // slug, magnum
				} else {
                    icon.overrideSprite = ammIcons[5]; // hollow, magnum
				}
				break;
			case 44:
                icon.overrideSprite = ammIcons[11]; // magcart, magpulse
				break;
			case 45:
                if (alternateAmmo) {
                    icon.overrideSprite = ammIcons[3]; // teflon, pistol
				} else {
                    icon.overrideSprite = ammIcons[2]; // standard, pistol
				}
				break;
			case 46:
                if (icon.enabled) icon.enabled = false;  // Uses energy, plasma rifle
				if (border.enabled) border.enabled = false;
                if (!energySlider.activeSelf) energySlider.SetActive(true);
                if (!energyHeatTicks.activeSelf) energyHeatTicks.SetActive(true);
                if (!energyOverloadButton.activeSelf) energyOverloadButton.SetActive(true);
                break;
            case 47:
				icon.overrideSprite = ammIcons[14]; // rail round, railgun
				break;
			case 48:
				icon.overrideSprite = ammIcons[4]; // rubber, riotgun
				break;
			case 49:
                if (alternateAmmo) {
                    icon.overrideSprite = ammIcons[13]; // large slag, skorpion
				} else {
                    icon.overrideSprite = ammIcons[12]; // slag, skorpion
				}
				break;
			case 50:
                if (icon.enabled) icon.enabled = false;  // Uses energy, sparq beam
				if (border.enabled) border.enabled = false;
                if (!energySlider.activeSelf) energySlider.SetActive(true);
                if (!energyHeatTicks.activeSelf) energyHeatTicks.SetActive(true);
                if (!energyOverloadButton.activeSelf) energyOverloadButton.SetActive(true);
                break;
            case 51:
                if (icon.enabled) icon.enabled = false;  // Uses energy, stungun
				if (border.enabled) border.enabled = false;
                if (!energySlider.activeSelf) energySlider.SetActive(true);
                if (!energyHeatTicks.activeSelf) energyHeatTicks.SetActive(true);
                if (!energyOverloadButton.activeSelf) energyOverloadButton.SetActive(true);
                break;
            default:
                if (icon.enabled) icon.enabled = false;
                if (border.enabled) border.enabled = false;
                if (energySlider.activeSelf) energySlider.SetActive(false);
                if (energyHeatTicks.activeSelf) energyHeatTicks.SetActive(false);
                if (energyOverloadButton.activeSelf) energyOverloadButton.SetActive(false);
                break;
		}
	}
}
