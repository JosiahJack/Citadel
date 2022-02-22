using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EReaderSectionsButtons : MonoBehaviour {
	public EReaderSectionsButtonHighlight ersbh0;
	public EReaderSectionsButtonHighlight ersbh1;
	public EReaderSectionsButtonHighlight ersbh2;
	public EReaderSectionsButtonHighlight ersbh3;

	void OnEnable() {
		if (Const.a.difficultyMission == 0) ersbh3.gameObject.SetActive(false);
	}

	public void OnClick(int index) {
		SetEReaderSectionsButtonsHighlights(index);
		switch (index) {
			case 0: MFDManager.a.OpenEmailTableContents(); break;
			case 1: MFDManager.a.OpenLogTableContents(); break;
			case 2: MFDManager.a.OpenDataTableContents(); break;
			case 3: MFDManager.a.OpenNotesTableContents(); break;
		}
	}

	public void SetEReaderSectionsButtonsHighlights(int index) {
		switch (index) {
			case 0: ersbh0.Highlight();   ersbh1.DeHighlight(); ersbh2.DeHighlight(); ersbh3.DeHighlight(); break;
			case 1: ersbh0.DeHighlight(); ersbh1.Highlight();   ersbh2.DeHighlight(); ersbh3.DeHighlight(); break;
			case 2: ersbh0.DeHighlight(); ersbh1.DeHighlight(); ersbh2.Highlight();   ersbh3.DeHighlight(); break;
			case 3: ersbh0.DeHighlight(); ersbh1.DeHighlight(); ersbh2.DeHighlight(); ersbh3.Highlight();   break;
		}
	}
}
