﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextGeometry : MonoBehaviour {
	void Start () {
		Text txt = GetComponent<Text>();
		if (txt != null) {
			if (txt.font != null) {
				if (txt.font.material != null) {
					if (txt.font.material.mainTexture != null) {
						txt.font.material.mainTexture.filterMode = FilterMode.Point;
					}
				}
			}
		}
	}
}
