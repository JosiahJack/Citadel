using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuButtonHighlight : MonoBehaviour {
	public MenuArrowKeyControls pageController; //not used on sub-buttons within the start menu page for new game difficulty settings
	public int menuItemIndex;
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

	public void CursorHighlight () {
		Highlight();
		if (pageController != null) pageController.SetIndex(menuItemIndex);
	}

	public void CursorDeHighlight () {
		DeHighlight();
		if (pageController != null) pageController.currentIndex = 0;
	}
}
