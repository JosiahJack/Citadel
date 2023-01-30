using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReverbZone : MonoBehaviour {
    void Start() {
        Const.a.AddToReverbRegister(gameObject);
    }
}