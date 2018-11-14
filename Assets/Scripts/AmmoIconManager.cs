using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AmmoIconManager : MonoBehaviour {
	[SerializeField] public Sprite[] ammIcons;
	[SerializeField] public GameObject clipBox;
    [SerializeField] public GameObject energySlider;
    [SerializeField] public GameObject energyHeatTicks;
    [SerializeField] public GameObject energyOverloadButton;
    private Image border;
    private Image icon;

    public void Awake() {
        icon = GetComponent<Image>();
        border = clipBox.GetComponent<Image>();
    }

    public void SetAmmoIcon (int index, bool alternateAmmo) {
		if (index >= 0) {
			icon.enabled = true;
			border.enabled = true;
            energySlider.SetActive(false);
            energyHeatTicks.SetActive(false);
            energyOverloadButton.SetActive(false);
            switch (index) {
			case 36:
                if (alternateAmmo) {
                    icon.overrideSprite = ammIcons[8]; // penetrator, MK3
				} else {
                    icon.overrideSprite = ammIcons[7]; // magnesium, MK3
				}
				break;
			case 37:
                icon.enabled = false;  // Uses energy, blaster
                border.enabled = false;
                energySlider.SetActive(true);
                energyHeatTicks.SetActive(true);
                energyOverloadButton.SetActive(true);
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
                icon.enabled = false;  // Uses energy, ion beam
				border.enabled = false;
                energySlider.SetActive(true);
                energyHeatTicks.SetActive(true);
                energyOverloadButton.SetActive(true);
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
                icon.enabled = false;  // Uses energy, plasma rifle
				border.enabled = false;
                energySlider.SetActive(true);
                energyHeatTicks.SetActive(true);
                energyOverloadButton.SetActive(true);
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
                icon.enabled = false;  // Uses energy, sparq beam
				border.enabled = false;
                energySlider.SetActive(true);
                energyHeatTicks.SetActive(true);
                energyOverloadButton.SetActive(true);
                break;
                case 51:
                icon.enabled = false;  // Uses energy, stungun
				border.enabled = false;
                energySlider.SetActive(true);
                energyHeatTicks.SetActive(true);
                energyOverloadButton.SetActive(true);
                break;
            }
		}
	}
}
