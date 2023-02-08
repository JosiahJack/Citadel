using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadPageGetSaveNames : MonoBehaviour {
	public Text[] loadButtonText;

	void Awake () {
		string readline; // variable to hold each string read in from the file
		for (int i=0;i<8;i++) {
			string sName = "sav" + i.ToString() + ".txt";
			string savP = Utils.SafePathCombine(Application.streamingAssetsPath,
												sName);

			Const.a.ConfirmExistsInStreamingAssetsMakeIfNot(sName);
			StreamReader sf = new StreamReader(savP);
			using (sf) {
				readline = sf.ReadLine();
				if (readline == null) break; // Just in case.

				loadButtonText[i].text = readline;
				if (i == 7) {
					// Override because we still want to keep it "- unused -" 
					// at first so we don't try quickloading a null save.
					loadButtonText[i].text = "quicksave";
				}

				sf.Close();
			}
		}	
	}
}
