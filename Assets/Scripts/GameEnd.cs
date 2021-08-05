using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEnd : MonoBehaviour {
	public void Targetted(UseData ud) {
		Const.a.gameFinished = true; // YAY WE DID IT!!!!
		PauseScript.a.PauseEnable(); // Pauses game, no more to do
		PauseScript.a.NoSavePauseQuit(); // quit to and enable main menu (exits the game to menu)
		MainMenuHandler.a.PlayCredits(); // Play credits and set page in menu handler
	}
}