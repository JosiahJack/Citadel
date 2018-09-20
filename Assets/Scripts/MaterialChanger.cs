using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour {
	public Texture2D newTexture;
	public Texture2D[] newTextureRandom;
	public bool disableImageSequence = true;
	public bool useRandom = false;
	public int selectedDigit;
	public bool sendSelectedDigit;
	public bool linkSelectedDigit;
	public GameObject target;
	private ImageSequenceTextureArray ista;
	private Texture2D selectedTexture;

	void Awake () {
		ista = GetComponent<ImageSequenceTextureArray>();
		if (useRandom) {
			selectedDigit = Random.Range (0, newTextureRandom.Length);
			selectedTexture = newTextureRandom[selectedDigit];
		} else {
			selectedTexture = newTexture;
		}
	}

	public void Targetted (UseData ud) {
		ista.enabled = false; // disable automatic texture changing
		if (ud.mainIndex != -1) {
			selectedDigit = ud.mainIndex;
			selectedTexture = newTextureRandom[selectedDigit];
		}

		gameObject.GetComponent<MeshRenderer> ().material.mainTexture = selectedTexture; // set texture to new texture

		if (linkSelectedDigit) {
			if (target != null) {
				ud.mainIndex = selectedDigit;
				target.SendMessageUpwards ("Targetted", ud);
			}
		}

		if (sendSelectedDigit) {
			switch (LevelManager.a.currentLevel) {
				case 1:
					Const.a.questData.lev1SecCode = selectedDigit;
					break;
				case 2:
					Const.a.questData.lev2SecCode = selectedDigit;
					break;
				case 3:
					Const.a.questData.lev3SecCode = selectedDigit;
					break;
				case 4:
					Const.a.questData.lev4SecCode = selectedDigit;
					break;
				case 5:
					Const.a.questData.lev5SecCode = selectedDigit;
					break;
				case 6:
					Const.a.questData.lev6SecCode = selectedDigit;
					break;
			}
		}
	}
}
