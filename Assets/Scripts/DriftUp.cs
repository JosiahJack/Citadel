using UnityEngine;
using UnityEngine.UI;

public class DriftUp : MonoBehaviour {
    public float startY;
    public float endY;
    public float rate = 0.5f;
    public float fadeRate = 0.1f;
    public float tickFinished;
    public bool fadeImage;
    public Image img;
    public float startFade = 1f;
    public float endFade = 0f;

    void OnEnable() {
        transform.position = new Vector3(transform.position.x,
                                         startY,
                                         transform.position.z);

        if (fadeImage && img != null) {
            img.color = new Color(img.color.r,img.color.g,img.color.b,startFade);
        }

        tickFinished = PauseScript.a.relativeTime;
    }

    void Update() {
        if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;
		if (tickFinished >= PauseScript.a.relativeTime) return;

        float delta = (1f / 60f);
		tickFinished = PauseScript.a.relativeTime + delta;
		float drift = transform.localPosition.y;
        drift += rate;
        if (drift > endY) drift = endY;
		transform.localPosition = new Vector3(transform.localPosition.x,
                                              drift,
                                              transform.localPosition.z);
        drift = img.color.a;
        drift -= fadeRate;
        if (drift < endFade) drift = endFade;
        img.color = new Color(img.color.r,img.color.g,img.color.b,drift);
    }
}
