using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageSequenceTextureArrayUI : MonoBehaviour {
	//An array of Objects that stores the results of the Resources.LoadAll() method
	private Object[] objects;
	//Each returned object is converted to a Texture and stored in this array
	private Sprite[] sprites;
	//With this Material object, a reference to the game object Material can be stored
	private Image goImage;
	//An integer to advance frames
	private int frameCounter = 0;
	public string resourceFolder;
	public float frameDelay = 0.35f;
	public bool stopAtEnd = false;
	private bool playDone = false;
	public bool replayOnEnable = false;
	
	void Awake() {
		//Get a reference to the Material of the game object this script is attached to.
		this.goImage = this.GetComponent<Image>();
		//gameObject.SetActive (false);
	}
	
	void Start () {
		//Load all textures found on the Sequence folder, that is placed inside the resources folder
		this.objects = Resources.LoadAll(resourceFolder, typeof(Sprite));
		//Initialize the array of textures with the same size as the objects array
		this.sprites = new Sprite[objects.Length];
		//Cast each Object to Texture and store the result inside the Textures array
		for(int i=0; i < objects.Length;i++) {
			this.sprites[i] = (Sprite)this.objects[i];
		}
	}

	void OnEnable() {
		if (replayOnEnable) {
			playDone = false;
			frameCounter = 0;
		}
	}
	
	void Update () {
		//Call the 'PlayLoop' method as a coroutine with a float delay
		if (stopAtEnd && playDone) return;
		if (stopAtEnd && !playDone) {
			StartCoroutine("Play", frameDelay);
		} else {
			StartCoroutine("PlayLoop", frameDelay);
		}
		//Set the material's texture to the current value of the frameCounter variable
		goImage.overrideSprite = sprites[frameCounter];
	}
	
	//The following methods return a IEnumerator so they can be yielded:
	//A method to play the animation in a loop
	IEnumerator PlayLoop(float delay) {
		//Wait for the time defined at the delay parameter
		yield return new WaitForSeconds(delay);  
		//Advance one frame
		frameCounter = (++frameCounter)%sprites.Length;
		//Stop this coroutine
		StopCoroutine("PlayLoop");
	}  

	//A method to play the animation just once
	IEnumerator Play(float delay) {
		//Wait for the time defined at the delay parameter
		yield return new WaitForSeconds(delay);  
		
		//If the frame counter isn't at the last frame
		if(frameCounter < sprites.Length-1) {
			//Advance one frame
			++frameCounter;
			if (frameCounter >= sprites.Length) playDone = true;
		} else {
			playDone = true;
		}
		//Stop this coroutine
		StopCoroutine("Play");
	} 
}