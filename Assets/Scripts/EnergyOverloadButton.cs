using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyOverloadButton : MonoBehaviour {
    public Color textClickableColor;
    public Color textDisabledColor;
    public Color textOverloadColor;
    public Color textEnergySetting;
    public Color textEnergyOverloaded;
    public Sprite normalButtonSprite;
    public Sprite overloadButtonSprite;
    public WeaponFire wf;
    public Text buttonText;
    public Text energySettingText;
    private Image buttonSprite;

    private void Awake() {
        buttonSprite = GetComponent<Image>();
        buttonSprite.overrideSprite = normalButtonSprite;
        buttonText.color = textClickableColor;
    }

    public void OverloadEnergyClick() {
        if (Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.WepInstance.weaponCurrent] > 25f) {
            Const.sprint(Const.a.stringTable[12]);
            return;
        }

        if (wf.overloadEnabled) {
            Const.sprint(Const.a.stringTable[13]);
            wf.overloadEnabled = false;
            buttonSprite.overrideSprite = normalButtonSprite;
            buttonText.color = textClickableColor;
            energySettingText.color = textEnergySetting;
            energySettingText.text = "ENERGY SETTING";
        } else { 
            Const.sprint(Const.a.stringTable[17]);
            wf.overloadEnabled = true;
            buttonSprite.overrideSprite = overloadButtonSprite;
            buttonText.color = textOverloadColor;
            energySettingText.color = textEnergyOverloaded;
            energySettingText.text = "OVERLOAD ENABLED";
        }
    }

    public void OverloadFired() {
        buttonSprite.overrideSprite = normalButtonSprite;
        buttonText.color = textDisabledColor;
    }
}
