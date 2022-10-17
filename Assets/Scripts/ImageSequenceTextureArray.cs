using UnityEngine;
using System.Collections;

public class ImageSequenceTextureArray : MonoBehaviour {
	public string resourceFolder;
	public string glowResourceFolder;
	public float frameDelay = 0.35f;
	public int initialIndexOffset = 0;
	public bool animateGlow = false;
	public bool randomFrame = false; // randomly pick a frame instead of sequential
	public bool reverseSequence = false;
	public bool glowOnly = false;
	public bool screenDestroyed = false;
	public int[] constArrayLookup;
	public int[] constArrayLookupGlow;
	public int[] constArrayDestroyed;
	/*[DTValidator.Optional] */public AudioClip SFXClip;
	/*[DTValidator.Optional] */public GameObject lightContainer;

	private AudioSource SFX;
	private float tick;
	private float tickFinished; // save....except handled purely through HealthManager
	private bool screenDestroyedDone = false; // Delay ending animation for a few destroy frames.
	private bool screenDestroyFirstFrame = true;
	private MeshRenderer mR;
	private Material goMaterial;
	private Light lit;
	private int frameCounter = 0; //An integer to advance frames
	private int frameCounterGlow = 0;

	void Awake() {
		//Get a reference to the Material of the game object this script is attached to.
		mR = GetComponent<MeshRenderer>();
		if (mR == null) { this.gameObject.SetActive(false); return; }
		this.goMaterial = this.GetComponent<Renderer>().material;
		SFX = GetComponent<AudioSource>();
		if (lightContainer != null) {
			lit = lightContainer.GetComponent<Light>();
			if (lit != null) {
				if ((transform.localScale.x < 1.0f) || (transform.localScale.y < 1.0f) || (transform.localScale.z < 1.0f)) {
					float factor = Mathf.Min(transform.localScale.x, transform.localScale.y, transform.localScale.z);
					lit.range *= factor;
					if (lit.range < 2.0f) lit.range = 2.0f;
				}
			}
		}
	}

	// called by HealthManager.cs's ScreenDeath
	public void Destroy() {
		if (SFX != null) {
			if (SFXClip != null) SFX.PlayOneShot(SFXClip);
		}
		if (lightContainer != null) lightContainer.SetActive(false);
		screenDestroyed = true; // if not already dead, say so
	}

	public void AwakeFromLoad(float health) {
		if (health > 0) {
			screenDestroyed = screenDestroyedDone = false;
			if (lightContainer != null) lightContainer.SetActive(true);
			tickFinished = PauseScript.a.relativeTime + tick;
			SetFrameIndices();
		} else {
			if (lightContainer != null) lightContainer.SetActive(false);
			screenDestroyed = true;
			goMaterial.SetTexture("_EmissionMap", Const.a.sequenceTextures[5]);
			goMaterial.mainTexture = Const.a.sequenceTextures[5]; // End frame of destroyed texture
		}
	}

	void SetFrameIndices() {
		if (reverseSequence) {
			if (constArrayLookup != null) frameCounter = constArrayLookup.Length - 1; // Start counting down from end.
			if (constArrayLookupGlow != null) frameCounterGlow = constArrayLookupGlow.Length - 1;
		} else {
			frameCounter = frameCounterGlow = 0; // Start counting up from 0.
		}

		if (constArrayLookupGlow != null) {
			if (initialIndexOffset < constArrayLookupGlow.Length  && initialIndexOffset > 0) frameCounterGlow = initialIndexOffset;
		}
		if (constArrayLookup != null) {
			if (initialIndexOffset < constArrayLookup.Length && initialIndexOffset > 0) frameCounter = initialIndexOffset;
		}
	}

	void Start () {
		if (PauseScript.a == null) { this.enabled = false; return; }

		//Load all textures found on the Sequence folder, that is placed inside the resources folder
		if (string.IsNullOrWhiteSpace(resourceFolder)) resourceFolder = glowResourceFolder;
		if (string.IsNullOrWhiteSpace(resourceFolder)) return;

		tick = frameDelay;
		tickFinished = PauseScript.a.relativeTime + tick;

		// New method...long, but reduces overall memory load from duplicate frames and reduces startup time by over 8 seconds.
		if (resourceFolder == "Bridge11") {
			// Normal
			constArrayLookup = new int[8];
			constArrayLookup[0] = 6;
			constArrayLookup[1] = 7;
			constArrayLookup[2] = 8;
			constArrayLookup[3] = 9;
			constArrayLookup[4] = 9;
			constArrayLookup[5] = 8;
			constArrayLookup[6] = 7;
			constArrayLookup[7] = 6;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "BrokenClock") {
			// Normal
			constArrayLookup = new int[2];
			constArrayLookup[0] = 10;
			constArrayLookup[1] = 11;
			// Glow
			constArrayLookupGlow = new int[2];
			constArrayLookupGlow[0] = 12;
			constArrayLookupGlow[1] = 13;
		} else if (resourceFolder == "EnergMine") {
			// Normal
			constArrayLookup = new int[7];
			constArrayLookup[0] = 14;
			constArrayLookup[1] = 15;
			constArrayLookup[2] = 16;
			constArrayLookup[3] = 17;
			constArrayLookup[4] = 18;
			constArrayLookup[5] = 19;
			constArrayLookup[6] = 20;
			// Glow
			constArrayLookupGlow = new int[8];
			constArrayLookupGlow[0] = 21;
			constArrayLookupGlow[1] = 22;
			constArrayLookupGlow[2] = 23;
			constArrayLookupGlow[3] = 24;
			constArrayLookupGlow[4] = 25;
			constArrayLookupGlow[5] = 26;
			constArrayLookupGlow[6] = 27;
			constArrayLookupGlow[7] = 28;
		} else if (resourceFolder == "EngScreen1") {
			// Normal
			constArrayLookup = new int[43];
			constArrayLookup[0] = 29;
			constArrayLookup[1] = 30;
			constArrayLookup[2] = 31;
			constArrayLookup[3] = 32;
			constArrayLookup[4] = 45;
			constArrayLookup[5] = 45;
			constArrayLookup[6] = 45;
			constArrayLookup[7] = 29;
			constArrayLookup[8] = 30;
			constArrayLookup[9] = 31;
			constArrayLookup[10] = 32;
			constArrayLookup[11] = 29;
			constArrayLookup[12] = 30;
			constArrayLookup[13] = 31;
			constArrayLookup[14] = 32;
			constArrayLookup[15] = 29;
			constArrayLookup[16] = 30;
			constArrayLookup[17] = 31;
			constArrayLookup[18] = 32;
			constArrayLookup[19] = 29;
			constArrayLookup[20] = 45;
			constArrayLookup[21] = 45;
			constArrayLookup[22] = 45;
			constArrayLookup[23] = 29;
			constArrayLookup[24] = 30;
			constArrayLookup[25] = 31;
			constArrayLookup[26] = 32;
			constArrayLookup[27] = 29;
			constArrayLookup[28] = 30;
			constArrayLookup[29] = 31;
			constArrayLookup[30] = 32;
			constArrayLookup[31] = 29;
			constArrayLookup[32] = 30;
			constArrayLookup[33] = 31;
			constArrayLookup[34] = 32;
			constArrayLookup[35] = 29;
			constArrayLookup[36] = 30;
			constArrayLookup[37] = 31;
			constArrayLookup[38] = 32;
			constArrayLookup[39] = 29;
			constArrayLookup[40] = 45;
			constArrayLookup[41] = 45;
			constArrayLookup[42] = 45;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "EngScreen2") {
			// Normal
			constArrayLookup = new int[8];
			constArrayLookup[0] = 52;
			constArrayLookup[1] = 51;
			constArrayLookup[2] = 50;
			constArrayLookup[3] = 49;
			constArrayLookup[4] = 49;
			constArrayLookup[5] = 50;
			constArrayLookup[6] = 51;
			constArrayLookup[7] = 52;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "ExecScreen1") {
			// Normal
			constArrayLookup = new int[13];
			constArrayLookup[0] = 29;
			constArrayLookup[1] = 30;
			constArrayLookup[2] = 31;
			constArrayLookup[3] = 32;
			constArrayLookup[4] = 45;
			constArrayLookup[5] = 29;
			constArrayLookup[6] = 30;
			constArrayLookup[7] = 31;
			constArrayLookup[8] = 45;
			constArrayLookup[9] = 29;
			constArrayLookup[10] = 30;
			constArrayLookup[11] = 31;
			constArrayLookup[12] = 45;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "ExecScreen2") {
			// Normal
			constArrayLookup = new int[21];
			constArrayLookup[0] = 83;
			constArrayLookup[1] = 84;
			constArrayLookup[2] = 85;
			constArrayLookup[3] = 86;
			constArrayLookup[4] = 83;
			constArrayLookup[5] = 83;
			constArrayLookup[6] = 86;
			constArrayLookup[7] = 85;
			constArrayLookup[8] = 84;
			constArrayLookup[9] = 83;
			constArrayLookup[10] = 103; // 711...yep!
			constArrayLookup[11] = 83;
			constArrayLookup[12] = 84;
			constArrayLookup[13] = 85;
			constArrayLookup[14] = 86;
			constArrayLookup[15] = 83;
			constArrayLookup[16] = 83;
			constArrayLookup[17] = 86;
			constArrayLookup[18] = 85;
			constArrayLookup[19] = 84;
			constArrayLookup[20] = 83;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "ExecScreen3") {
			// Normal
			constArrayLookup = new int[3];
			constArrayLookup[0] = 115;
			constArrayLookup[1] = 115;
			constArrayLookup[2] = 117;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "ExecScreen4") {
			// Normal
			constArrayLookup = new int[7];
			constArrayLookup[0] = 115;
			constArrayLookup[1] = 115;
			constArrayLookup[2] = 115;
			constArrayLookup[3] = 115;
			constArrayLookup[4] = 116;
			constArrayLookup[5] = 117;
			constArrayLookup[6] = 118;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MagCartridge") {
			// Normal
			constArrayLookup = new int[11];
			constArrayLookup[0] = 123;
			constArrayLookup[1] = 124;
			constArrayLookup[2] = 125;
			constArrayLookup[3] = 126;
			constArrayLookup[4] = 127;
			constArrayLookup[5] = 128;
			constArrayLookup[6] = 129;
			constArrayLookup[7] = 130;
			constArrayLookup[8] = 131;
			constArrayLookup[9] = 132;
			constArrayLookup[10] = 133;
			// Glow
			constArrayLookupGlow = new int[11];
			constArrayLookupGlow[0] = 134;
			constArrayLookupGlow[1] = 135;
			constArrayLookupGlow[2] = 136;
			constArrayLookupGlow[3] = 137;
			constArrayLookupGlow[4] = 138;
			constArrayLookupGlow[5] = 139;
			constArrayLookupGlow[6] = 140;
			constArrayLookupGlow[7] = 141;
			constArrayLookupGlow[8] = 142;
			constArrayLookupGlow[9] = 143;
			constArrayLookupGlow[10] = 144;
		} else if (resourceFolder == "MaintScreen1") {
			// Normal
			constArrayLookup = new int[5];
			constArrayLookup[0] = 29;
			constArrayLookup[1] = 30;
			constArrayLookup[2] = 31;
			constArrayLookup[3] = 32;
			constArrayLookup[4] = 37;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MaintScreen2") {
			// Normal
			constArrayLookup = new int[6];
			constArrayLookup[0] = 33;
			constArrayLookup[1] = 34;
			constArrayLookup[2] = 35;
			constArrayLookup[3] = 36;
			constArrayLookup[4] = 32;
			constArrayLookup[5] = 29;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MedicalBed") {
			// Normal
			constArrayLookup = new int[10];
			constArrayLookup[0] = 145;
			constArrayLookup[1] = 146;
			constArrayLookup[2] = 147;
			constArrayLookup[3] = 148;
			constArrayLookup[4] = 149;
			constArrayLookup[5] = 150;
			constArrayLookup[6] = 151;
			constArrayLookup[7] = 152;
			constArrayLookup[8] = 153;
			constArrayLookup[9] = 154;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MedScreen1") {
			// Normal
			constArrayLookup = new int[6];
			constArrayLookup[0] = 54;
			constArrayLookup[1] = 59;
			constArrayLookup[2] = 118;
			constArrayLookup[3] = 116;
			constArrayLookup[4] = 118;
			constArrayLookup[5] = 59;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MedScreen2") {
			// Normal
			constArrayLookup = new int[4];
			constArrayLookup[0] = 79;
			constArrayLookup[1] = 80;
			constArrayLookup[2] = 81;
			constArrayLookup[3] = 82;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MedScreen3") {
			// Normal
			constArrayLookup = new int[4];
			constArrayLookup[0] = 99;
			constArrayLookup[1] = 98;
			constArrayLookup[2] = 97;
			constArrayLookup[3] = 92;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MedScreen4") {
			// Normal
			constArrayLookup = new int[4];
			constArrayLookup[0] = 29;
			constArrayLookup[1] = 30;
			constArrayLookup[2] = 31;
			constArrayLookup[3] = 36;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MedScreen5") {
			// Normal
			constArrayLookup = new int[8];
			constArrayLookup[0] = 56;
			constArrayLookup[1] = 55;
			constArrayLookup[2] = 54;
			constArrayLookup[3] = 59;
			constArrayLookup[4] = 59;
			constArrayLookup[5] = 54;
			constArrayLookup[6] = 55;
			constArrayLookup[7] = 56;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MedScreen6") {
			// Normal
			constArrayLookup = new int[12];
			constArrayLookup[0] = 61;
			constArrayLookup[1] = 61;
			constArrayLookup[2] = 62;
			constArrayLookup[3] = 62;
			constArrayLookup[4] = 61;
			constArrayLookup[5] = 61;
			constArrayLookup[6] = 212;
			constArrayLookup[7] = 213;
			constArrayLookup[8] = 214;
			constArrayLookup[9] = 215;
			constArrayLookup[10]= 216;
			constArrayLookup[11]= 217;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MedScreen7") {
			// Normal
			constArrayLookup = new int[4];
			constArrayLookup[0] = 119;
			constArrayLookup[1] = 120;
			constArrayLookup[2] = 121;
			constArrayLookup[3] = 122;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MedScreen8") {
			// Normal
			constArrayLookup = new int[4];
			constArrayLookup[0] = 59;
			constArrayLookup[1] = 54;
			constArrayLookup[2] = 55;
			constArrayLookup[3] = 56;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MedScreen9") {
			// Normal
			constArrayLookup = new int[8];
			constArrayLookup[0] = 37;
			constArrayLookup[1] = 38;
			constArrayLookup[2] = 39;
			constArrayLookup[3] = 40;
			constArrayLookup[4] = 41;
			constArrayLookup[5] = 42;
			constArrayLookup[6] = 43;
			constArrayLookup[7] = 44;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MedScreen10") {
			// Normal
			constArrayLookup = new int[10];
			constArrayLookup[0] = 83;
			constArrayLookup[1] = 84;
			constArrayLookup[2] = 85;
			constArrayLookup[3] = 86;
			constArrayLookup[4] = 83;
			constArrayLookup[5] = 83;
			constArrayLookup[6] = 86;
			constArrayLookup[7] = 85;
			constArrayLookup[8] = 84;
			constArrayLookup[9] = 83;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MedScreen11") {
			// Normal
			constArrayLookup = new int[8];
			constArrayLookup[0] = 67;
			constArrayLookup[1] = 66;
			constArrayLookup[2] = 66;
			constArrayLookup[3] = 67;
			constArrayLookup[4] = 79;
			constArrayLookup[5] = 80;
			constArrayLookup[6] = 80;
			constArrayLookup[7] = 79;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MedScreen13") {
			// Normal
			constArrayLookup = new int[12];
			constArrayLookup[0] = 218;
			constArrayLookup[1] = 219;
			constArrayLookup[2] = 220;
			constArrayLookup[3] = 221;
			constArrayLookup[4] = 222;
			constArrayLookup[5] = 223;
			constArrayLookup[6] = 224;
			constArrayLookup[7] = 225;
			constArrayLookup[8] = 226;
			constArrayLookup[9] = 227;
			constArrayLookup[10]= 228;
			constArrayLookup[11]= 229;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MedScreen16") {
			// Normal
			constArrayLookup = new int[8];
			constArrayLookup[0] = 79;
			constArrayLookup[1] = 80;
			constArrayLookup[2] = 81;
			constArrayLookup[3] = 82;
			constArrayLookup[4] = 82;
			constArrayLookup[5] = 81;
			constArrayLookup[6] = 80;
			constArrayLookup[7] = 79;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MedScreen18") {
			// Normal
			constArrayLookup = new int[6];
			constArrayLookup[0] = 73;
			constArrayLookup[1] = 74;
			constArrayLookup[2] = 75;
			constArrayLookup[3] = 76;
			constArrayLookup[4] = 77;
			constArrayLookup[5] = 78;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MedScreen22") {
			// Normal
			constArrayLookup = new int[8];
			constArrayLookup[0] = 73;
			constArrayLookup[1] = 74;
			constArrayLookup[2] = 76;
			constArrayLookup[3] = 75;
			constArrayLookup[4] = 77;
			constArrayLookup[5] = 76;
			constArrayLookup[6] = 78;
			constArrayLookup[7] = 73;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MedScreen23") {
			// Normal
			constArrayLookup = new int[4];
			constArrayLookup[0] = 29;
			constArrayLookup[1] = 30;
			constArrayLookup[2] = 31;
			constArrayLookup[3] = 32;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MedScreen24") {
			// Normal
			constArrayLookup = new int[8];
			constArrayLookup[0] = 230;
			constArrayLookup[1] = 231;
			constArrayLookup[2] = 232;
			constArrayLookup[3] = 233;
			constArrayLookup[4] = 234;
			constArrayLookup[5] = 235;
			constArrayLookup[6] = 236;
			constArrayLookup[7] = 237;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MedScreen25") {
			// Normal
			constArrayLookup = new int[4];
			constArrayLookup[0] = 92;
			constArrayLookup[1] = 93;
			constArrayLookup[2] = 94;
			constArrayLookup[3] = 95;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MedScreen27") {
			// Normal
			constArrayLookup = new int[8];
			constArrayLookup[0] = 238;
			constArrayLookup[1] = 239;
			constArrayLookup[2] = 240;
			constArrayLookup[3] = 241;
			constArrayLookup[4] = 242;
			constArrayLookup[5] = 243;
			constArrayLookup[6] = 244;
			constArrayLookup[7] = 245;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "MedScreen29") {
			// Normal
			constArrayLookup = new int[5];
			constArrayLookup[0] = 64;
			constArrayLookup[1] = 65;
			constArrayLookup[2] = 66;
			constArrayLookup[3] = 67;
			constArrayLookup[4] = 68;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "Rad1_1") {
			// Normal
			constArrayLookup = new int[5];
			constArrayLookup[0] = 155;
			constArrayLookup[1] = 156;
			constArrayLookup[2] = 157;
			constArrayLookup[3] = 158;
			constArrayLookup[4] = 159;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "ReacScreen4") {
			// Normal
			constArrayLookup = new int[4];
			constArrayLookup[0] = 61;
			constArrayLookup[1] = 61;
			constArrayLookup[2] = 62;
			constArrayLookup[3] = 62;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "SciScreen1") {
			// Normal
			constArrayLookup = new int[3];
			constArrayLookup[0] = 62;
			constArrayLookup[1] = 61;
			constArrayLookup[2] = 60;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "SciScreen2") {
			// Normal
			constArrayLookup = new int[7];
			constArrayLookup[0] = 107;
			constArrayLookup[1] = 108;
			constArrayLookup[2] = 109;
			constArrayLookup[3] = 111;
			constArrayLookup[4] = 112;
			constArrayLookup[5] = 113;
			constArrayLookup[6] = 114;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "SciScreen3") {
			// Normal
			constArrayLookup = new int[4];
			constArrayLookup[0] = 33;
			constArrayLookup[1] = 34;
			constArrayLookup[2] = 35;
			constArrayLookup[3] = 36;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "SciScreen4") {
			// Normal
			constArrayLookup = new int[4];
			constArrayLookup[0] = 188;
			constArrayLookup[1] = 189;
			constArrayLookup[2] = 113;
			constArrayLookup[3] = 112;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "SciScreen5") {
			// Normal
			constArrayLookup = new int[9];
			constArrayLookup[0] = 79;
			constArrayLookup[1] = 80;
			constArrayLookup[2] = 80;
			constArrayLookup[3] = 79;
			constArrayLookup[4] = 73;
			constArrayLookup[5] = 74;
			constArrayLookup[6] = 76;
			constArrayLookup[7] = 77;
			constArrayLookup[8] = 75;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "ScreenDestroyed") {
			// Normal
			constArrayLookup = new int[6];
			constArrayLookup[0] = 0;
			constArrayLookup[1] = 1;
			constArrayLookup[2] = 2;
			constArrayLookup[3] = 3;
			constArrayLookup[4] = 4;
			constArrayLookup[5] = 5;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "ScreenCodeRandom") {
			// Normal
			constArrayLookup = new int[10];
			constArrayLookup[0] = 160;
			constArrayLookup[1] = 161;
			constArrayLookup[2] = 162;
			constArrayLookup[3] = 163;
			constArrayLookup[4] = 164;
			constArrayLookup[5] = 165;
			constArrayLookup[6] = 166;
			constArrayLookup[7] = 167;
			constArrayLookup[8] = 168;
			constArrayLookup[9] = 169;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "SecScreen4") {
			// Normal
			constArrayLookup = new int[3];
			constArrayLookup[0] = 29;
			constArrayLookup[1] = 30;
			constArrayLookup[2] = 31;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "ShodanStatic") {
			// Normal
			constArrayLookup = new int[60];
			constArrayLookup[0] = 170;
			constArrayLookup[1] = 171;
			constArrayLookup[2] = 172;
			constArrayLookup[3] = 173;
			constArrayLookup[4] = 174;
			constArrayLookup[5] = 175;
			constArrayLookup[6] = 176;
			constArrayLookup[7] = 177;
			constArrayLookup[8] = 178;
			constArrayLookup[9] = 179;
			constArrayLookup[10]= 180;
			constArrayLookup[11]= 181;
			constArrayLookup[12]= 182;
			constArrayLookup[13]= 183;
			constArrayLookup[14]= 184;
			constArrayLookup[15]= 185;
			constArrayLookup[16]= 186;
			constArrayLookup[17]= 187;
			constArrayLookup[18]= 188;
			constArrayLookup[19]= 189;
			constArrayLookup[20]= 190;
			constArrayLookup[21]= 191;
			constArrayLookup[22]= 192;
			constArrayLookup[23]= 193;
			constArrayLookup[24]= 194;
			constArrayLookup[25]= 195;
			constArrayLookup[26]= 196;
			constArrayLookup[27]= 197;
			constArrayLookup[28]= 198;
			constArrayLookup[29]= 199;
			constArrayLookup[30]= 200;
			constArrayLookup[31]= 201;
			constArrayLookup[32]= 202;
			constArrayLookup[33]= 203;
			constArrayLookup[34]= 204;
			constArrayLookup[35]= 205;
			constArrayLookup[36]= 206;
			constArrayLookup[37]= 203;
			constArrayLookup[38]= 204;
			constArrayLookup[39]= 205;
			constArrayLookup[40]= 206;
			constArrayLookup[41]= 205;
			constArrayLookup[42]= 203;
			constArrayLookup[43]= 204;
			constArrayLookup[44]= 206;
			constArrayLookup[45]= 205;
			constArrayLookup[46]= 204;
			constArrayLookup[47]= 203;
			constArrayLookup[48]= 206;
			constArrayLookup[49]= 205;
			constArrayLookup[50]= 203;
			constArrayLookup[51]= 204;
			constArrayLookup[52]= 205;
			constArrayLookup[53]= 206;
			constArrayLookup[54]= 203;
			constArrayLookup[55]= 206;
			constArrayLookup[56]= 205;
			constArrayLookup[57]= 203;
			constArrayLookup[58]= 204;
			constArrayLookup[59]= 203;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "Static") {
			// Normal
			constArrayLookup = new int[17];
			constArrayLookup[0] = 203;
			constArrayLookup[1] = 204;
			constArrayLookup[2] = 205;
			constArrayLookup[3] = 206;
			constArrayLookup[4] = 203;
			constArrayLookup[5] = 206;
			constArrayLookup[6] = 205;
			constArrayLookup[7] = 203;
			constArrayLookup[8] = 204;
			constArrayLookup[9] = 205;
			constArrayLookup[10]= 206;
			constArrayLookup[11]= 203;
			constArrayLookup[12]= 206;
			constArrayLookup[13]= 205;
			constArrayLookup[14]= 203;
			constArrayLookup[15]= 204;
			constArrayLookup[16]= 203;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "Telepad") {
			// Normal
			constArrayLookup = new int[4];
			constArrayLookup[0] = 207;
			constArrayLookup[1] = 208;
			constArrayLookup[2] = 209;
			constArrayLookup[3] = 210;
			// Glow
			constArrayLookupGlow = new int[4];
			constArrayLookupGlow[0] = 207;
			constArrayLookupGlow[1] = 208;
			constArrayLookupGlow[2] = 209;
			constArrayLookupGlow[3] = 210;
		} else if (resourceFolder == "XDoor1") {
			// Normal
			constArrayLookup = new int[3];
			constArrayLookup[0] = 299;
			constArrayLookup[1] = 300;
			constArrayLookup[2] = 301;
			// Glow
			constArrayLookupGlow = null;
		} else if (resourceFolder == "ZeroGMutant") {
			// Normal
			constArrayLookup = new int[53];
			constArrayLookup[0] = 246;
			constArrayLookup[1] = 247;
			constArrayLookup[2] = 248;
			constArrayLookup[3] = 249;
			constArrayLookup[4] = 250;
			constArrayLookup[5] = 251;
			constArrayLookup[6] = 252;
			constArrayLookup[7] = 253;
			constArrayLookup[8] = 254;
			constArrayLookup[9] = 255;
			constArrayLookup[10]= 256;
			constArrayLookup[11]= 257;
			constArrayLookup[12]= 258;
			constArrayLookup[13]= 259;
			constArrayLookup[14]= 260;
			constArrayLookup[15]= 261;
			constArrayLookup[16]= 262;
			constArrayLookup[17]= 263;
			constArrayLookup[18]= 264;
			constArrayLookup[19]= 265;
			constArrayLookup[20]= 266;
			constArrayLookup[21]= 267;
			constArrayLookup[22]= 268;
			constArrayLookup[23]= 269;
			constArrayLookup[24]= 270;
			constArrayLookup[25]= 271;
			constArrayLookup[26]= 272;
			constArrayLookup[27]= 273;
			constArrayLookup[28]= 274;
			constArrayLookup[29]= 275;
			constArrayLookup[30]= 276;
			constArrayLookup[31]= 277;
			constArrayLookup[32]= 278;
			constArrayLookup[33]= 279;
			constArrayLookup[34]= 280;
			constArrayLookup[35]= 281;
			constArrayLookup[36]= 282;
			constArrayLookup[37]= 283;
			constArrayLookup[38]= 284;
			constArrayLookup[39]= 285;
			constArrayLookup[40]= 286;
			constArrayLookup[41]= 287;
			constArrayLookup[42]= 288;
			constArrayLookup[43]= 289;
			constArrayLookup[44]= 290;
			constArrayLookup[45]= 291;
			constArrayLookup[46]= 292;
			constArrayLookup[47]= 293;
			constArrayLookup[48]= 294;
			constArrayLookup[49]= 295;
			constArrayLookup[50]= 296;
			constArrayLookup[51]= 297;
			constArrayLookup[52]= 298;
			// Glow
			constArrayLookupGlow = null;
		}
		SetFrameIndices();
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			if (string.IsNullOrWhiteSpace(resourceFolder)) { this.enabled = false; return; }

			if (mR.isVisible) {
				if (tickFinished < PauseScript.a.relativeTime) {
					Think();
					tickFinished = PauseScript.a.relativeTime + tick;
				}
			}
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
			if (frameCounter > 5) {
				screenDestroyedDone = true; // stop continuing to increment counter, all done counting
				return; // we are done, no need to continue animating frames, destruction complete, unit lost, unit ready, wait is this a Command and Conquer reference?  Yes.  Yes it is.
			}

			//Set the material's texture to the current value of the frameCounter variable
			if (frameCounter >= 0 && frameCounter <= 5) goMaterial.mainTexture = Const.a.sequenceTextures[frameCounter]; // 0 thru 5
			if (frameCounter >= 0 && frameCounter <= 5) goMaterial.SetTexture("_EmissionMap", Const.a.sequenceTextures[frameCounter]);
			return;
		}

		// Oh hey we aren't destroyed yet, so let's get to actual animating!

		// Flip it, on the back. -Nathan Drake
		if (reverseSequence) {
			if (constArrayLookup != null) frameCounter = (--frameCounter) % constArrayLookup.Length-1;
			if (constArrayLookupGlow != null) frameCounterGlow = (--frameCounterGlow) % constArrayLookup.Length-1;
		} else {
			if (constArrayLookup != null) {
				frameCounter++;
				if (frameCounter > (constArrayLookup.Length - 1)) frameCounter = 0;
			}
			if (constArrayLookupGlow != null) {
				frameCounterGlow++;
				if (frameCounterGlow > (constArrayLookupGlow.Length - 1)) frameCounterGlow = 0;
			}
		}

		// We don't know where we are going, or when.
		if (randomFrame) {
			if (constArrayLookup != null) frameCounter = Random.Range(0, constArrayLookup.Length-1);
			if (constArrayLookupGlow != null) {
				if (frameCounter < constArrayLookupGlow.Length) frameCounterGlow = frameCounter; // Match when it makes sense.
				else frameCounterGlow = Random.Range(0, constArrayLookupGlow.Length-1); // Otherwise randomize it.
			}
		}

		if (constArrayLookupGlow != null) {
			if (constArrayLookupGlow.Length > 0) {
				if (frameCounterGlow < constArrayLookupGlow.Length) {
					if (constArrayLookupGlow[frameCounterGlow] < Const.a.sequenceTextures.Length && constArrayLookupGlow[frameCounterGlow] >= 0) {
						goMaterial.SetTexture("_EmissionMap", Const.a.sequenceTextures[constArrayLookupGlow[frameCounterGlow]]);
					}
				}
			}
		}

		if (glowOnly) return;

		if (constArrayLookup != null) {
			if (constArrayLookup.Length > 0 && frameCounter < constArrayLookup.Length) {
				if (constArrayLookup[frameCounter] < Const.a.sequenceTextures.Length && constArrayLookup[frameCounter] >= 0) {
					if (goMaterial.mainTexture != Const.a.sequenceTextures[constArrayLookup[frameCounter]]) goMaterial.mainTexture = Const.a.sequenceTextures[constArrayLookup[frameCounter]];
				}
			}
		}
	}
}
