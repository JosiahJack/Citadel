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
        if (wf.GetHeatForCurrentWeapon() > 25f) {
            Const.sprint("Weapon too hot for overload", Const.a.allPlayers);
            return;
        }

		//Debug.Log("OverloadEnergyClick()ed");
        if (wf.overloadEnabled) {
            Const.sprint("Weapon normal setting", Const.a.allPlayers);
            wf.overloadEnabled = false;
            buttonSprite.overrideSprite = normalButtonSprite;
            buttonText.color = textClickableColor;
            energySettingText.color = textEnergySetting;
            energySettingText.text = "ENERGY SETTING";
        } else { 
            Const.sprint("Weapon overload enabled", Const.a.allPlayers);
            wf.overloadEnabled = true;
            buttonSprite.overrideSprite = overloadButtonSprite;
            buttonText.color = textOverloadColor;
            energySettingText.color = textEnergyOverloaded;
            energySettingText.text = "OVERLOAD ENABLED";
        }
    }

    public void OverloadFired() {
		//Const.sprint("Overload fired!",Const.a.allPlayers);
        buttonSprite.overrideSprite = normalButtonSprite;
        buttonText.color = textDisabledColor;
    }

    //void Start() {
    //    GetComponent<Button>().onClick.AddListener(() => { OverloadEnergyClick(); });
    //}
}
