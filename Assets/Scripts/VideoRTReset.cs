using UnityEngine;
using UnityEngine.UI;

public class VideoRTReset : MonoBehaviour {
	public RawImage rawImage;
	public Texture2D blackTex;

	void OnDisable() {
		rawImage.texture = blackTex;
	}
}
