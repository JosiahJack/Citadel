using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageSequenceTextureArrayUI : MonoBehaviour {
	private Object[] objects;
	private Sprite[] sprites;
	private Image goImage;
	private int frameCounter = 0;
	public string resourceFolder;
	public float frameDelay = 0.35f; // frameDelay = 0.09f; for Vmail
	public bool stopAtEnd = false; // True for Vmail
	private bool playDone = false;
	public bool replayOnEnable = false; // True for Vmail
	public bool playOnMenu = false; // False for Vmail
	public bool deactivateAtEnd = false;
	
	void Awake() {
		goImage = this.GetComponent<Image>();
	}
	
	void Start () {
		objects = Resources.LoadAll(resourceFolder, typeof(Sprite)); //Load all textures found on the Sequence folder, that is placed inside the resources folder
		if (resourceFolder == "AAOutro") Debug.Log("objects.Length: " + objects.Length.ToString());
		sprites = new Sprite[objects.Length]; //Initialize the array of textures with the same size as the objects array

		//Cast each Object to Texture and store the result inside the Textures array
		for(int i=0; i < objects.Length;i++) {
			this.sprites[i] = (Sprite)this.objects[i];
		}
	}

	void OnEnable() {
		Start();
		if (replayOnEnable) {
			playDone = false;
			frameCounter = 0;
		}
	}
	
	void Update() {
		if (!PauseScript.a.Paused() || playOnMenu) {
			if (!PauseScript.a.MenuActive() || playOnMenu) {
				if (deactivateAtEnd && playDone) gameObject.SetActive(false);
				if (stopAtEnd && playDone) return;

				if (stopAtEnd && !playDone) {
					StartCoroutine("Play", frameDelay);
				} else {
					StartCoroutine("PlayLoop", frameDelay); // Call the 'PlayLoop' method as a coroutine with a float delay.
				}

				// Set the material's texture to current value of frameCounter.
				if (frameCounter < sprites.Length && frameCounter >= 0) {
					goImage.overrideSprite = sprites[frameCounter];
				}
			}
		}
	}

	IEnumerator PlayLoop(float delay) {
		yield return new WaitForSeconds(delay); // Wait for the time defined at the delay parameter.
TryAgain:
		if (PauseScript.a.Paused() && !playOnMenu) {
			yield return null;
			goto TryAgain;
		}
		
		frameCounter = (++frameCounter)%sprites.Length; // Advance one frame
		StopCoroutine("PlayLoop"); // Stop this coroutine
	}  

	IEnumerator Play(float delay) {
		yield return new WaitForSeconds(delay); // Wait for the time defined at the delay parameter.
		
		// If the frame counter isn't at the last frame.
		if(frameCounter < sprites.Length-1) {
			++frameCounter; // Advance one frame.
			if (frameCounter >= sprites.Length) playDone = true;
		} else {
			playDone = true;
		}
		StopCoroutine("Play"); //Stop this coroutine
	} 
	
	void OnDestroy() {
		objects = null;
		sprites = null;
	}
}
