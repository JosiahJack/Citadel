using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureChanger : MonoBehaviour {
	public Texture2D mainTexture;
	public Texture2D mainTexture2;
	public Texture2D mainTextureGlow;
	public Texture2D mainTextureGlow2;
	public bool startAlternate = false;
	[HideInInspector] public bool currentTexture = false;
	public Renderer rMainLod0;
	/*[DTValidator.Optional] */public Renderer rMainLod1;
	/*[DTValidator.Optional] */public Renderer rMainLod2;
	public bool useGlow;

	public void Awake() {
		if (startAlternate) {
			currentTexture = true;
			if (rMainLod0 != null) { rMainLod0.material.mainTexture = mainTexture2; if (useGlow) rMainLod0.material.SetTexture("_EmissionMap", mainTextureGlow2); }
			if (rMainLod1 != null) { rMainLod1.material.mainTexture = mainTexture2; if (useGlow) rMainLod1.material.SetTexture("_EmissionMap", mainTextureGlow2); }
			if (rMainLod2 != null) { rMainLod2.material.mainTexture = mainTexture2; if (useGlow) rMainLod2.material.SetTexture("_EmissionMap", mainTextureGlow2); }
		}
	}

    public void Toggle() {
		if (currentTexture) {
			if (rMainLod0 != null) { rMainLod0.material.mainTexture = mainTexture; if (useGlow) rMainLod0.material.SetTexture("_EmissionMap", mainTextureGlow); }
			if (rMainLod1 != null) { rMainLod1.material.mainTexture = mainTexture; if (useGlow) rMainLod1.material.SetTexture("_EmissionMap", mainTextureGlow); }
			if (rMainLod2 != null) { rMainLod2.material.mainTexture = mainTexture; if (useGlow) rMainLod2.material.SetTexture("_EmissionMap", mainTextureGlow); }
		} else {
			if (rMainLod0 != null) { rMainLod0.material.mainTexture = mainTexture2; if (useGlow) rMainLod0.material.SetTexture("_EmissionMap", mainTextureGlow2); }
			if (rMainLod1 != null) { rMainLod1.material.mainTexture = mainTexture2; if (useGlow) rMainLod1.material.SetTexture("_EmissionMap", mainTextureGlow2); }
			if (rMainLod2 != null) { rMainLod2.material.mainTexture = mainTexture2; if (useGlow) rMainLod2.material.SetTexture("_EmissionMap", mainTextureGlow2); }
		}

		currentTexture = !currentTexture;
	}

	public static string Save(GameObject go) {
		TextureChanger tex = go.GetComponent<TextureChanger>();
		if (tex == null) {
			UnityEngine.Debug.Log("TextureChanger missing on savetype of TextureChanger!  GameObject.name: " + go.name);
			return "1";
		}

		string line = System.String.Empty;
		line = Utils.BoolToString(tex.currentTexture); // bool - is this gravlift on?
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		TextureChanger tex = go.GetComponent<TextureChanger>();
		if (tex == null || index < 0 || entries == null) return index + 1;

		tex.currentTexture = Utils.GetBoolFromString(entries[index]); index++; // is this gravlift on?
		tex.currentTexture = !tex.currentTexture; // gets done again in Toggle()
		tex.Toggle(); // set it again since this does other stuff than just change the bool
		return index;
	}
}
