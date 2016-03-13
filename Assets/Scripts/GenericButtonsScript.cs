using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GenericButtonsScript : MonoBehaviour {
    public GameObject playerCamera;

	public void PtrEnter () {
		GUIState.isBlocking = true;
        playerCamera.GetComponent<MouseLookScript>().overButton = true;
        playerCamera.GetComponent<MouseLookScript>().overButtonType = 77; //generic button code
        playerCamera.GetComponent<MouseLookScript>().currentButton = gameObject;
    }

	public void PtrExit () {
		GUIState.isBlocking = false;
        playerCamera.GetComponent<MouseLookScript>().overButton = false;
        playerCamera.GetComponent<MouseLookScript>().overButtonType = -1;
    }
}
