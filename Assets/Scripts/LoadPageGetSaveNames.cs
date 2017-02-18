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
			StreamReader sf = new StreamReader(Application.streamingAssetsPath + "/sav"+i.ToString()+".txt");
			if (sf != null) {
				using (sf) {
					readline = sf.ReadLine();
					if (readline == null) break; // just in case
					loadButtonText[i].text = readline;
					sf.Close();
				}
			}
		}	
	}
}
