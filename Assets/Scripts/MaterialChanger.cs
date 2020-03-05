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
	public string linktarget;
	public string argvalue;
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

	public void LinkTargetted(UseData ud) {
		Debug.Log("MaterialChanger was link targetted!");
		ista.enabled = false;
		selectedDigit = ud.mainIndex;
		selectedTexture = ud.texture;
		gameObject.GetComponent<MeshRenderer>().material.mainTexture = selectedTexture; // set texture to new texture
	}

	public void Targetted (UseData ud) {
		Debug.Log("MaterialChanger was targetted!");
		ista.enabled = false; // disable automatic texture changing
		if (ud.mainIndex != -1) {
			selectedDigit = ud.mainIndex;
			selectedTexture = newTextureRandom[selectedDigit];
		}

		gameObject.GetComponent<MeshRenderer> ().material.mainTexture = selectedTexture; // set texture to new texture

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
			Debug.Log("Self-destruct code is now: " + Const.a.questData.lev1SecCode.ToString() + Const.a.questData.lev2SecCode.ToString() + Const.a.questData.lev3SecCode.ToString() + Const.a.questData.lev4SecCode.ToString() + Const.a.questData.lev5SecCode.ToString() + Const.a.questData.lev6SecCode.ToString() + ".");
		}

		if (linkSelectedDigit) {
			if (linktarget == null || linktarget == "" || linktarget == " " || linktarget == "  ") {
				Debug.Log("WARNING: MaterialChanger attempting to linktarget nothing");
				return; // no target, do nothing
			}
			ud.mainIndex = selectedDigit;
			ud.texture = selectedTexture;
			ud.argvalue = argvalue;
			TargetIO tio = GetComponent<TargetIO>();
			if (tio != null) {
				ud.SetBits(tio);
			} else {
				Debug.Log("BUG: no TargetIO.cs found on an object with a MaterialChanger.cs script!  Trying to run linktarget without parameters!");
			}
			Const.a.UseTargets(ud,linktarget);
		}
	}
}
