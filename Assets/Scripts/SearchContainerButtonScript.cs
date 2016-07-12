using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SearchContainerButtonScript : MonoBehaviour {
    public GameObject playerCamera;
	public int refIndex = 0;

	public void PtrEnter () {
		GUIState.isBlocking = true;
        playerCamera.GetComponent<MouseLookScript>().overButton = true;
        playerCamera.GetComponent<MouseLookScript>().overButtonType = 4; //generic button code
        playerCamera.GetComponent<MouseLookScript>().currentButton = gameObject;
    }

	public void PtrExit () {
		GUIState.isBlocking = false;
        playerCamera.GetComponent<MouseLookScript>().overButton = false;
        playerCamera.GetComponent<MouseLookScript>().overButtonType = -1;
    }
}
