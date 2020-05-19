using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EReaderSectionsButtons : MonoBehaviour {
	public MultiMediaTabManager mmtm;
	public EReaderSectionsButtonHighlight ersbh0;
	public EReaderSectionsButtonHighlight ersbh1;
	public EReaderSectionsButtonHighlight ersbh2;

	public void OnClick(int index) {
		SetEReaderSectionsButtonsHighlights(index);
		switch (index) {
			case 0: mmtm.OpenEmailTableContents(); break;
			case 1: mmtm.OpenLogTableContents(); break;
			case 2: mmtm.OpenDataTableContents(); break;
		}
	}

	public void SetEReaderSectionsButtonsHighlights(int index) {
		switch (index) {
			case 0: ersbh0.Highlight(); ersbh1.DeHighlight(); ersbh2.DeHighlight(); break;
			case 1: ersbh0.DeHighlight(); ersbh1.Highlight(); ersbh2.DeHighlight(); break;
			case 2: ersbh0.DeHighlight(); ersbh1.DeHighlight(); ersbh2.Highlight(); break;
		}
	}
}
