using UnityEngine;
using UnityEngine.UI;

public class MinigameCursor : MonoBehaviour {
    public float minigameMouseX;
    public float minigameMouseY;
    public RectTransform minigameCursor;
    private float panelWidth = 256f;
    private float offset = 128f;
    private float xmin = 22f/1366f;
    private float xmax = 282f/1366f;
    private float ymin = 6f/768f;
    private float ymax = 266f/768f;
    private float deltaX;
    private float deltaY;

    public static MinigameCursor a;

    void Awake() {
        a = this;
    }

    void Start() {
        deltaX = xmax - xmin;
        deltaY = ymax - ymin;
    }

    void Update() {
        minigameMouseX = MouseCursor.a.cursorPosition.x / Screen.width;
        if (minigameMouseX < xmin) minigameMouseX = xmin;
        if (minigameMouseX > xmax) minigameMouseX = xmax;
        minigameMouseX = (minigameMouseX - xmin) / deltaX;

        minigameMouseY = MouseCursor.a.cursorPosition.y / Screen.height;
        if (minigameMouseY < ymin) minigameMouseY = ymin;
        if (minigameMouseY > ymax) minigameMouseY = ymax;
        minigameMouseY = (minigameMouseY - ymin) / deltaY;

        minigameMouseX = (minigameMouseX * panelWidth) - offset;
        minigameMouseY = (minigameMouseY * panelWidth) - offset;
        minigameCursor.localPosition = new Vector3(minigameMouseX,minigameMouseY,0f);
    }

    void DoubleClick() {

    }
}
