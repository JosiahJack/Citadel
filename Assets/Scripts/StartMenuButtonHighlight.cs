using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuButtonHighlight : MonoBehaviour {
	public Text text;
	public Shadow textshadow;
	public Color lit;
	public Color dark;
	public Color darkshadow;
	public Color litshadow;

	void DeHighlight () {
		if (textshadow != null) textshadow.effectColor = darkshadow;
		if (text != null) text.color = dark;
	}
	void Highlight () {
		if (textshadow != null) textshadow.effectColor = litshadow;
		if (text != null) text.color = lit;
	}
}
