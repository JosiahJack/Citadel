using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEnd : MonoBehaviour {
	public void Targetted(UseData ud) {
		Const.a.gameFinished = true;
		PauseScript.a.PauseEnable();
		PauseScript.a.NoSavePauseQuit();
		PauseScript.a.mainMenu.GetComponent<MainMenuHandler>().creditsPage.SetActive(true);
	}
}