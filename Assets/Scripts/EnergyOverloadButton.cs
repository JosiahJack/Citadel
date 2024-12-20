﻿using System.Collections;
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
    public Text buttonText;
    public Text energySettingText;
    private Image buttonSprite;
    private float clickFinished;

    private void Awake() {
        buttonSprite = GetComponent<Image>();
        buttonSprite.overrideSprite = normalButtonSprite;
        buttonText.color = textClickableColor;
    }

    void Start() {
        GetComponent<Button>().onClick.AddListener(() => { OverloadEnergyClick(); });
    }

    public void OverloadEnergyClick() {
		MFDManager.a.mouseClickHeldOverGUI = true;
        OverloadButtonAction();
    }

    public void OverloadButtonAction() {
        if (clickFinished >= Time.time) return;

        clickFinished = Time.time + 0.4f;
        if (Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] > 25f) {
            Const.sprint(Const.a.stringTable[12]);
            return;
        }

        if (WeaponFire.a.overloadEnabled) {
            Const.sprint(Const.a.stringTable[13]);
            WeaponFire.a.overloadEnabled = false;
            buttonSprite.overrideSprite = normalButtonSprite;
            buttonText.color = textClickableColor;
            energySettingText.color = textEnergySetting;
            energySettingText.text = Const.a.stringTable[16];
        } else { 
            Const.sprint(Const.a.stringTable[17]);
            WeaponFire.a.overloadEnabled = true;
            buttonSprite.overrideSprite = overloadButtonSprite;
            buttonText.color = textOverloadColor;
            energySettingText.color = textEnergyOverloaded;
            energySettingText.text = Const.a.stringTable[18];
        }
    }

    public void OverloadFired() {
        buttonSprite.overrideSprite = normalButtonSprite;
        buttonText.color = textDisabledColor;
    }
}
