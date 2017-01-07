using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuDifficultyButton : MonoBehaviour {
	public GameObject[] otherControllers;
	public GameObject controller;
	public int difficultyValue;

	public void OnDiffButtonClick () {
		controller.GetComponent<StartMenuDifficultyController>().SetDifficulty(difficultyValue);
		controller.SendMessage("Highlight",SendMessageOptions.DontRequireReceiver);

		if (otherControllers.Length <= 0) return;

		for (int i=0; i<(otherControllers.Length-1);i++) {
			otherControllers[i].SendMessage("DeHighlight",SendMessageOptions.DontRequireReceiver);
		}
	}
}
