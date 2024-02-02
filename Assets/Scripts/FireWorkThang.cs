using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FireWorkThang : MonoBehaviour {
    public float minScale;
    public float maxScale;
    public float changeFracPerSecond = 0.5f;
    public float waitTimeFull = 0.8f;
    public float waitTimeMinMin = 0.8f;
    public float waitTimeMinMax = 3f;
    private Image img;
    private float tickFinished;
    private float curScale;
    private bool waitAtFull;
    private bool waitAtMin;

    void Awake() {
        img = GetComponent<Image>();
    }

    void OnEnable() {
        tickFinished = PauseScript.a.relativeTime;
        curScale = Random.Range(minScale,maxScale);
        if (changeFracPerSecond < 0.001f) changeFracPerSecond = 0.5f; // 2 secs
        waitAtFull = false;
        if (minScale >= maxScale) {
            minScale = 0f;
            maxScale = 1f;
            Debug.LogWarning("FireWorkThang maxScale not set higher than min");
        }
    }

    void Update() {
        if (PauseScript.a.Paused()) return;
        if (PauseScript.a.MenuActive()) return;
        if (tickFinished >= PauseScript.a.relativeTime) return;

        float delta = (1f / 60f);
        tickFinished = PauseScript.a.relativeTime + delta;
        if (waitAtFull) {
            waitAtFull = false;
            waitAtMin = false;
            curScale = minScale;
            waitAtMin = true;
            tickFinished = PauseScript.a.relativeTime
                           + Random.Range(waitTimeMinMin,waitTimeMinMax);
        } else if (waitAtMin) {
            waitAtMin = false;
            waitAtFull = false;
            curScale += (maxScale - minScale) * 0.333f;
        } else {
            curScale += (changeFracPerSecond * (maxScale - minScale)) / 60f;
        }

        if (curScale > maxScale) {
            curScale = maxScale;
            waitAtFull = true;
            tickFinished = PauseScript.a.relativeTime + waitTimeFull;
        }

        transform.localScale = new Vector3(curScale,curScale,curScale);
    }
}
