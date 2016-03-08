using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextureScrollingFX : MonoBehaviour {
	public float interval = 0.1f;
	public float scrollSpeedX = 0.5f;
	public float scrollSpeedY = 0f;
	public float rotate;
	private RawImage image;
	private Vector2 offset;
	private float updateTime;
	private Texture2D texture;
	private Rect tempRect;
	//private Rect actRect;

	void Awake () {
		//texture = GetComponent<MeshRenderer>().material.mainTexture;
		image = GetComponent<RawImage>();
		//actRect = image.uvRect;
		//FillTexture();
		updateTime = Time.time + interval;
	}

	void OnEnable () {
		//texture = GetComponent<MeshRenderer>().material.mainTexture;
		image = GetComponent<RawImage>();
		//actRect = image.uvRect;
		//FillTexture();
		updateTime = Time.time + interval;
	}

	void FillTexture () {
		//texture.filterMode = FilterMode.Point;
		offset.x += (Time.deltaTime*scrollSpeedX)/10.0f;
		offset.y += (Time.deltaTime*scrollSpeedY)/10.0f;
		tempRect.x += offset.x;
		tempRect.y += offset.y;
		//actRect = tempRect;
		//texture.
		//texture.Apply();
	}

	void Update () {
		if (updateTime < Time.time) {
			updateTime = (Time.time + interval);
			FillTexture();
		}
	}
}
