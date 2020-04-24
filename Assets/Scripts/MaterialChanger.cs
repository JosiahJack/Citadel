using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour {
	//public bool disableImageSequence = true;
	//private ImageSequenceTextureArray ista;
	[HideInInspector]
	public bool alreadyDone = false;
	public int levelIndex = 0;

	void OnEnable() {
		ImageSequenceTextureArray ista = GetComponent<ImageSequenceTextureArray>();
		if (ista != null) ista.enabled = false;
		StartCoroutine(SetMaterialFromCode(levelIndex));
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
		// if (disableImageSequence) {
			// ista.enabled = false; // disable automatic texture changing
			// ista.screenDestroyed = true;
		// }
		// alreadyDone = true;
		// SetMaterialFromCode();
	}
}
