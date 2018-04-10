using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EReaderSectionsButtons : MonoBehaviour {
	public MultiMediaTabManager mmtm;
	public EReaderSectionsButtonHighlight ersbh0;
	public EReaderSectionsButtonHighlight ersbh1;
	public EReaderSectionsButtonHighlight ersbh2;
	public int index;

	public void OnClick() {
		switch (index) {
		case 0: mmtm.OpenEmailTableContents(); ersbh0.Highlight(); ersbh1.DeHighlight(); ersbh2.DeHighlight(); break;
		case 1: mmtm.OpenLogTableContents(); ersbh0.DeHighlight(); ersbh1.Highlight(); ersbh2.DeHighlight(); break;
		case 2: mmtm.OpenDataTableContents(); ersbh0.DeHighlight(); ersbh1.DeHighlight(); ersbh2.Highlight(); break;
		}
	}
}
