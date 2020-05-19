using UnityEngine;
using System.Collections;

public class TextureStaticNoise : MonoBehaviour {
	public int resolution = 64;
	public float interval = 0.15f;
	private float updateTime;
	private Texture2D texture;

	void Awake () {
		texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
		texture.name = "ProceduralStatic";
		GetComponent<MeshRenderer>().material.mainTexture = texture;
		FillTexture();
		updateTime = PauseScript.a.relativeTime + interval;
	}

	void OnEnable () {
		texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
		texture.name = "ProceduralStatic";
		GetComponent<MeshRenderer>().material.mainTexture = texture;
		FillTexture();
		updateTime = PauseScript.a.relativeTime + interval;
	}

	void FillTexture () {
		if (texture.width != resolution)
			texture.Resize(resolution, resolution);

		//Vector3 point00 = new Vector3(-0.5f,-0.5f);
		//Vector3 point10 = new Vector3( 0.5f,-0.5f);
		//Vector3 point01 = new Vector3(-0.5f, 0.5f);
		//Vector3 point11 = new Vector3( 0.5f, 0.5f);
		//float stepSize = 1f / resolution;

		for (int y=0; y<resolution; y++) {
			//Vector3 point0 = Vector3.Lerp(point00, point01, (y+0.5f) * stepSize);
			//Vector3 point1 = Vector3.Lerp(point10, point11, (y+0.5f) * stepSize);
			for (int x=0; x<resolution; x++) {
				//Vector3 point = Vector3.Lerp(point0, point1, (y+0.5f) * stepSize);
				texture.SetPixel(x, y, Color.white * Random.value);
			}
		}
		texture.filterMode = FilterMode.Point;
		texture.Apply();
	}

	void Update () {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			if (updateTime < PauseScript.a.relativeTime) {
				updateTime = (PauseScript.a.relativeTime + interval);
				FillTexture();
			}
		}
		//updateTime += Time.deltaTime;
	}
}
