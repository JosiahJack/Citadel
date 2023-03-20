using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour {
	//public bool disableImageSequence = true;
	//private ImageSequenceTextureArray ista;
	[HideInInspector] public bool alreadyDone = false;
	public int levelIndex = 0;

	void OnEnable() {
		if (alreadyDone) return;


	}

	IEnumerator SetMaterialFromCode(int index){
        yield return new WaitForSeconds(0.2f); // give Const a time to populate it's questdata
		switch (index) {
			case 1:
				GetComponent<MeshRenderer> ().material = (Const.a.screenCodes[Const.a.questData.lev1SecCode]); break;
			case 2:
				GetComponent<MeshRenderer> ().material = (Const.a.screenCodes[Const.a.questData.lev2SecCode]); break;
			case 3:
				GetComponent<MeshRenderer> ().material = (Const.a.screenCodes[Const.a.questData.lev3SecCode]); break;
			case 4:
				GetComponent<MeshRenderer> ().material = (Const.a.screenCodes[Const.a.questData.lev4SecCode]); break;
			case 5:
				GetComponent<MeshRenderer> ().material = (Const.a.screenCodes[Const.a.questData.lev5SecCode]); break;
			case 6:
				GetComponent<MeshRenderer> ().material = (Const.a.screenCodes[Const.a.questData.lev6SecCode]); break;
		}
		alreadyDone = true;
	}

	public void Targetted (UseData ud) {
		if (alreadyDone) return;

		ImageSequenceTextureArray ista = GetComponent<ImageSequenceTextureArray>();
		alreadyDone = true;
		StartCoroutine(SetMaterialFromCode(levelIndex));
	}

	public static string Save(GameObject go) {
		MaterialChanger mch = go.GetComponent<MaterialChanger>();
		if (mch == null) {
			Debug.Log("MaterialChanger missing on savetype of MaterialChanger!  GameObject.name: " + go.name);
			return "1";
		}

		string line = System.String.Empty;
		line = Utils.BoolToString(mch.alreadyDone); // bool - is this gravlift on?  Much is already done.
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		MaterialChanger mch = go.GetComponent<MaterialChanger>(); // ... ado about nothing
		if (mch == null) {
			Debug.Log("MaterialChanger.Load failure, mch == null");
			return index + 1;
		}

		if (index < 0) {
			Debug.Log("MaterialChanger.Load failure, index < 0");
			return index + 1;
		}

		if (entries == null) {
			Debug.Log("MaterialChanger.Load failure, entries == null");
			return index + 1;
		}

		mch.alreadyDone = Utils.GetBoolFromString(entries[index]); index++; // bool - is this gravlift on?

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
