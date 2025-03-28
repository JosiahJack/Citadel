using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to change gravity lift from red to green.
public class MaterialChanger : MonoBehaviour {
	[HideInInspector] public bool alreadyDone = false;
	public int levelIndex = 0;

	IEnumerator SetMaterialFromCode(int index){
        yield return new WaitForSeconds(0.2f); // give Const a time to populate it's questdata
		switch (index) {
			case 1: GetComponent<MeshRenderer> ().material = (Const.a.screenCodes[Const.a.questData.lev1SecCode]); break;
			case 2: GetComponent<MeshRenderer> ().material = (Const.a.screenCodes[Const.a.questData.lev2SecCode]); break;
			case 3: GetComponent<MeshRenderer> ().material = (Const.a.screenCodes[Const.a.questData.lev3SecCode]); break;
			case 4: GetComponent<MeshRenderer> ().material = (Const.a.screenCodes[Const.a.questData.lev4SecCode]); break;
			case 5: GetComponent<MeshRenderer> ().material = (Const.a.screenCodes[Const.a.questData.lev5SecCode]); break;
			case 6: GetComponent<MeshRenderer> ().material = (Const.a.screenCodes[Const.a.questData.lev6SecCode]); break;
		}
		alreadyDone = true;
	}

	public void Targetted (UseData ud) {
		if (alreadyDone) return; // Gravity lift is already on.

		ImageSequenceTextureArray ista = GetComponent<ImageSequenceTextureArray>();
		alreadyDone = true;
		StartCoroutine(SetMaterialFromCode(levelIndex));
	}

	public static string Save(GameObject go) {
		MaterialChanger mch = go.GetComponent<MaterialChanger>();
		return Utils.BoolToString(mch.alreadyDone,"alreadyDone"); // and much already yet remaining
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		MaterialChanger mch = go.GetComponent<MaterialChanger>(); // ... ado about nothing
		mch.alreadyDone = Utils.GetBoolFromString(entries[index],"alreadyDone"); index++;
		if (mch.alreadyDone) {
			ImageSequenceTextureArray ista = mch.GetComponent<ImageSequenceTextureArray>();
			if (ista != null) ista.enabled = false;
			mch.StartCoroutine(mch.SetMaterialFromCode(mch.levelIndex));
		} else {
			ImageSequenceTextureArray ista = mch.GetComponent<ImageSequenceTextureArray>();
			if (ista != null) ista.enabled = true;
			mch.StopCoroutine("SetMaterialFromCode");
		}
		return index;
	}
}
