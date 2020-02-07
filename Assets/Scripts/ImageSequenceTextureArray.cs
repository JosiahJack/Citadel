using UnityEngine;
using System.Collections;

public class ImageSequenceTextureArray : MonoBehaviour {
	//An array of Objects that stores the results of the Resources.LoadAll() method
	private Object[] objects;
	private Object[] glowobjects;
	private Object[] destroyedobjects;
	//Each returned object is converted to a Texture and stored in this array
	private Texture[] textures;
	private Texture[] glowtextures;
	private Texture[] destroyedTextures;
	//With this Material object, a reference to the game object Material can be stored
	private Material goMaterial;
	//An integer to advance frames
	private int frameCounter = 0;
	public string resourceFolder;
	public string glowResourceFolder;
	public float frameDelay = 0.35f;
	public int initialIndexOffset = 0;
	public bool animateGlow = false;
	public bool randomFrame = false; // randomly pick a frame instead of sequential
	public bool reverseSequence = false;
	public bool glowOnly = false;
	public bool screenDestroyed = false;
	private bool screenDestroyedDone = false;
	private bool screenDestroyFirstFrame = true;
	public string destroyedScreenFolder = "ScreenDestroyed";

	private float tick;
	private float tickFinished;
	
	void Awake() {
		//Get a reference to the Material of the game object this script is attached to.
		this.goMaterial = this.GetComponent<Renderer>().material;
		//gameObject.SetActive (false);
	}
	
	void Start () {
		//Load all textures found on the Sequence folder, that is placed inside the resources folder
		if (resourceFolder == "" && !glowOnly) {
			return;
		}

		if (reverseSequence) {
			frameCounter = (textures.Length - 1);
		} else {
			frameCounter = 0;
		}

		tick = frameDelay;
		tickFinished = Time.time + tick;

		if (animateGlow) {
			this.glowobjects = Resources.LoadAll(glowResourceFolder, typeof(Texture));
			this.glowtextures = new Texture[glowobjects.Length];
			for(int i=0; i < glowobjects.Length;i++) {
				this.glowtextures[i] = (Texture)this.glowobjects[i];
			}
			if (glowOnly) {
				if (initialIndexOffset < glowobjects.Length  && initialIndexOffset > 0) frameCounter = initialIndexOffset;
				return; // don't continue on to the normal part, setup glow only
			}
		}

		// Setup normal animated texture sequence
		this.objects = Resources.LoadAll(resourceFolder, typeof(Texture));
		//Initialize the array of textures with the same size as the objects array
		this.textures = new Texture[objects.Length];
		
		//Cast each Object to Texture and store the result inside the Textures array
		for(int i=0; i < objects.Length;i++) {
			this.textures[i] = (Texture)this.objects[i];
		}


		// Load in screen textures for when this screen gets destroyed
		if (destroyedScreenFolder != "") {
			this.destroyedobjects = Resources.LoadAll(destroyedScreenFolder, typeof(Texture));
			this.destroyedTextures = new Texture[destroyedobjects.Length];

			//Cast each Object to Texture and store the result inside the Textures array
			for(int i=0; i < destroyedobjects.Length;i++) {
				this.destroyedTextures[i] = (Texture)this.destroyedobjects[i];
			}
		}

		if (initialIndexOffset < objects.Length && initialIndexOffset > 0) frameCounter = initialIndexOffset;
	}

	// called by HealthManager.cs's ScreenDeath
	public void Destroy() {
		screenDestroyed = true; // if not already dead, say so
	}

	void Update () {
		if (resourceFolder == "" && !glowOnly) {
			return;
		}

		if (tickFinished < Time.time) {
			Think();
			tickFinished = Time.time + tick;
		}
	}

	void Think () {
		// animate through the screen destruction textures once then stop once frameCounter reaches the end
		if (screenDestroyed) {
			if (screenDestroyedDone) return; // all done, yoohoo see ya bye bye

			if (screenDestroyFirstFrame) {
				frameCounter = 0; // set destroyed frame to 0 for the first frame, we were just destroyed but let's keep using same frameCounter but reset it first
				screenDestroyFirstFrame = false; // flip bit so we don't keep setting current frame to 0 endlessly
			}

			frameCounter++;
			if (frameCounter >= destroyedTextures.Length) {
				screenDestroyedDone = true; // stop continuing to increment counter, all done counting
				return; // we are done, no need to continue animating frames, destruction complete, unit lost, unit ready, wait is this a Command and Conquer reference?  Yes.  Yes it is.
			}

			//Set the material's texture to the current value of the frameCounter variable
			goMaterial.mainTexture = destroyedTextures[frameCounter]; // using destroyed textures since we are a dead screen now
			return;
		}

		// Oh hey we aren't destroyed yet, so let's get animating!

		// flip it, on the back
		if (reverseSequence) {
			frameCounter = (--frameCounter) % textures.Length-1;
		} else {
			frameCounter = (++frameCounter) % textures.Length;
		}

		// we don't know where we are going, or when
		if (randomFrame) {
			frameCounter = Random.Range (0, textures.Length-1);
		}

		// careful! let's match the glow texture index to the maintexture index by using the same frameCounter...make sure the glow textures array is the exact same length!!
		if (animateGlow) {
			if (frameCounter >= glowtextures.Length) {
				Debug.Log("ERROR: frameCounter got out of bounds relative to the glowtextures array on ImageSequenceTextureArray, check that glowtextures.Length = textures.Length!");
			} else {
				goMaterial.SetTexture("_EmissionMap", glowtextures[frameCounter]);
				if (glowOnly) return; // set glow only
			}
		}

		//Set the material's texture to the current value of the frameCounter variable
		goMaterial.mainTexture = textures[frameCounter];
	}
}