using UnityEngine;
using System.Collections;

public class ImageSequenceTextureArray : MonoBehaviour {
	//An array of Objects that stores the results of the Resources.LoadAll() method
	private Object[] objects;
	private Object[] glowobjects;
	//Each returned object is converted to a Texture and stored in this array
	private Texture[] textures;
	private Texture[] glowtextures;
	//With this Material object, a reference to the game object Material can be stored
	private Material goMaterial;
	//An integer to advance frames
	private int frameCounter = 0;
	public string resourceFolder;
	public string glowResourceFolder;
	public float frameDelay = 0.35f;
	public bool animateGlow = false;

	private float tick;
	private float tickFinished;
	
	void Awake() {
		//Get a reference to the Material of the game object this script is attached to.
		this.goMaterial = this.GetComponent<Renderer>().material;
		//gameObject.SetActive (false);
	}
	
	void Start () {
		//Load all textures found on the Sequence folder, that is placed inside the resources folder
		this.objects = Resources.LoadAll(resourceFolder, typeof(Texture));
		this.glowobjects = Resources.LoadAll(glowResourceFolder, typeof(Texture));
		//Initialize the array of textures with the same size as the objects array
		this.textures = new Texture[objects.Length];
		this.glowtextures = new Texture[glowobjects.Length];
		//Cast each Object to Texture and store the result inside the Textures array
		for(int i=0; i < objects.Length;i++) {
			this.textures[i] = (Texture)this.objects[i];
		}
		for(int i=0; i < glowobjects.Length;i++) {
			this.glowtextures[i] = (Texture)this.glowobjects[i];
		}
		tick = frameDelay;
		tickFinished = Time.time + tick;
	}

	void Update () {
		if (tickFinished < Time.time) {
			Think();
			tickFinished = Time.time + tick;
		}
	}

	void Think () {
		frameCounter = (++frameCounter)%textures.Length;

		//Set the material's texture to the current value of the frameCounter variable
		goMaterial.mainTexture = textures[frameCounter];
		if (animateGlow) {
			goMaterial.SetTexture("_EmissionMap", glowtextures[frameCounter]);
		}
	}
}