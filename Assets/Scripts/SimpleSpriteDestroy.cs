﻿using UnityEngine;
using System.Collections;

public class SimpleSpriteDestroy : MonoBehaviour {
	public void DestroySprite() {
		Utils.SafeDestroy(gameObject);
	}
}
