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

    void DisableAll() {
        Utils.DisableImage(icon);
        Utils.DisableImage(border);
        Utils.Deactivate(energySlider);
        Utils.Deactivate(energyHeatTicks);
        Utils.Deactivate(energyOverloadButton);
    }

    void Energy() {
        Utils.DisableImage(icon);
        Utils.DisableImage(border);
        Utils.Activate(energySlider);
        Utils.Activate(energyHeatTicks);
        Utils.Activate(energyOverloadButton);
    }

    void Standard(bool alternateAmmo, int norm, int alt) {
        Utils.EnableImage(icon);
        Utils.EnableImage(border);
        Utils.Activate(border.gameObject);
        Utils.Activate(icon.gameObject);
        Utils.Deactivate(energySlider);
        Utils.Deactivate(energyHeatTicks);
        Utils.Deactivate(energyOverloadButton);
        if (norm < 0 || norm > ammIcons.Length) norm = 0;
        if ( alt < 0 ||  alt > ammIcons.Length)  alt = 0;
        if (alternateAmmo) {
            icon.overrideSprite = ammIcons[alt];
        } else {
            icon.overrideSprite = ammIcons[norm];
        }
    }

    public void SetAmmoIcon (int index, bool alt) {
        Debug.Log("Set ammo icon");
        switch (index) {
			case 36: Standard(alt,7,8);    break; // MK3 Magnesium, Penetrator
			case 37: Energy();             break; // Uses energy, Blaster
			case 38: Standard(alt,0,1);    break; // Dartgun Needle, Tranq
            case 39: Standard(alt,9,10);   break;//Flechetter Hornette,Splinter
			case 40: Energy();             break; // Uses energy, Ion Beam
            case 41: DisableAll();         break; // Rapier, no ammo used
			case 42: DisableAll();         break; // Pipe, no ammo used
			case 43: Standard(alt,5,6);    break; // Magnum Hollow, Slug
			case 44: Standard(false,11,-1);break; // Magpulse Magcart, N/A
			case 45: Standard(alt,2,3);    break; // Pistorl Standard, Teflon
			case 46: Energy();             break; // Uses energy, plasma rifle
            case 47: Standard(false,14,-1);break; // Railgun Rail Round
			case 48: Standard(false,4,-1); break; // Riotgun Rubber, N/A
			case 49: Standard(alt,12,13);  break; // Skorpion Slag, Large Slag
			case 50: Energy();             break; // Uses energy, Sparq Beam
            case 51: Energy();             break; // Uses energy, Stungun
            default: DisableAll();         break; // - no weapon -
		}
	}
}
