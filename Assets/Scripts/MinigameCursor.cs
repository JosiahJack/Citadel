using UnityEngine;
using UnityEngine.UI;

public class MinigameCursor : MonoBehaviour {
    public float minigameMouseX;
    public float minigameMouseY;
    public RectTransform minigameCursor;
    private float xmin = 22f/1366f;
    private float xmax = 282f/1366f;
    private float ymin = 6f/768f;
    private float ymax = 266f/768f;

    void Update() {
        minigameMouseX = MouseCursor.a.cursorPosition.x / Screen.width;
        if (minigameMouseX < xmin) minigameMouseX = xmin;
        if (minigameMouseX > xmax) minigameMouseX = xmax;

        minigameMouseY = MouseCursor.a.cursorPosition.y / Screen.height;
        if (minigameMouseY < ymin) minigameMouseY = ymin;
        if (minigameMouseY > ymax) minigameMouseY = ymax;

        minigameMouseX = (minigameMouseX * 256f) - 128f;
        minigameMouseY = (minigameMouseY * 256f) - 128f;
        minigameCursor.localPosition = new Vector3(minigameMouseX,minigameMouseY,0f);
    }

    void DoubleClick() {

    }
}
