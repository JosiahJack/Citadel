using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtonHighlight : MonoBehaviour {
	public MenuArrowKeyControls pageController;
	public int menuItemIndex;
	public Text subtext;
	public GameObject pad;
	public Sprite padlit;
	public Sprite paddrk;

	void DeHighlight () {
		Color tempcol = subtext.color;
		tempcol.a = 0.5f;
		subtext.color = tempcol;
		pad.GetComponent<Image>().overrideSprite = paddrk;
	}

	void Highlight () {
		Color tempcol = subtext.color;
		tempcol.a = 1.0f;
		subtext.color = tempcol;
		pad.GetComponent<Image>().overrideSprite = padlit;
	}

	public void CursorHighlight () {
		Highlight();
		pageController.SetIndex(menuItemIndex);
	}

	public void CursorDeHighlight () {
		DeHighlight();
		pageController.currentIndex = 0;
	}
}
