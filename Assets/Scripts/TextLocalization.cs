using UnityEngine;
using UnityEngine.UI;
using System;

public class TextLocalization : MonoBehaviour {
    public int lingdex = 0;
    public TextMesh tM;

    // Register with localization
    void Awake() {
        Const.a.AddToTextLocalizationRegister(this);
    }

    // Update to match new string table contents.
    public void UpdateText() {
        if (lingdex < 0) return;

        tM.text = Const.a.stringTable[lingdex];
    }
}
