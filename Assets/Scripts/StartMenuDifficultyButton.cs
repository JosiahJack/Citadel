using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuDifficultyButton : MonoBehaviour {
	public GameObject[] otherControllers;
	public StartMenuDifficultyController controller;
	public StartMenuButtonHighlight controllerHighlighter;
	public int difficultyValue;

	public void OnDiffButtonClick () {
		controller.SetDifficulty(difficultyValue);
		controller.HighlightUpdate();
		controllerHighlighter.Highlight();
		DeHighlightNeighbors();
	}

	public void DeHighlightNeighbors() {
		for (int i=0;i<otherControllers.Length;i++) {
			if (otherControllers[i] != null) otherControllers[i].SendMessage("DeHighlight",SendMessageOptions.DontRequireReceiver);
		}
	}
}
