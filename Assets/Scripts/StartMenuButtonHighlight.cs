using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuButtonHighlight : MonoBehaviour {
	/*[DTValidator.Optional] */public MenuArrowKeyControls pageController; //not used on sub-buttons within the start menu page for new game difficulty settings
	public int menuItemIndex;
	public Text text;
	public Shadow textshadow;
	/*[DTValidator.Optional] */public Outline outlineGlow; // Only used on the difficulty number buttons, looked odd on the difficulty names.
	public Color lit;
	public Color dark;
	public Color darkshadow;
	public Color litshadow;

	public void DeHighlight () {
		if (textshadow != null) {
			textshadow.effectColor = darkshadow;
			// textshadow.enabled = true;
		}
		if (outlineGlow != null) outlineGlow.enabled = false;
		if (text != null) text.color = dark;
	}

	public void Highlight () {
		if (textshadow != null) {
			textshadow.effectColor = litshadow;
			// textshadow.enabled = false;
		}
		if (outlineGlow != null) outlineGlow.enabled = true;
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
