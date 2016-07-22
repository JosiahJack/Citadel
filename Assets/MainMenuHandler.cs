using UnityEngine;
using UnityStandardAssets.ImageEffects;
using System.Collections;

public class MainMenuHandler : MonoBehaviour {
	public GameObject Button1;
	public GameObject Button2;
	public GameObject Button3;
	public GameObject Button4;
	public GameObject startFXObject;
	public AudioClip StartGameSFX;
	public GameObject saltTheFries;
	public GameObject mainCamera;
	private AudioSource StartSFX;

	void Awake () {
		StartSFX = startFXObject.GetComponent<AudioSource>();
	}

	public void StartGame () {
		StartSFX.PlayOneShot(StartGameSFX);
		mainCamera.GetComponent<Antialiasing>().enabled = false;
		this.gameObject.SetActive(false);
	}

	public void GoToOptions () {
		
	}

	public void Quit () {
		StartCoroutine(quitFunction());
	}

	IEnumerator quitFunction () {
		saltTheFries.SetActive(true);
		yield return new WaitForSeconds(1f);
		Application.Quit();
	}
}
