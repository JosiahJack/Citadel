using UnityEngine;
using UnityEngine.EventSystems;
[RequireComponent(typeof(UnityEngine.UI.AspectRatioFitter))]
public class MobileInputController : MonoBehaviour,IBeginDragHandler,
                                     IDragHandler,IEndDragHandler,
                                     IPointerDownHandler,IPointerUpHandler {
    public RectTransform Background;
    public RectTransform Knob;
    public Camera uiCamera;
    public bool left = true;

    [Header("Input Values")]
    public float Horizontal = 0;
    public float Vertical = 0;
    public float offset;

    Vector2 PointPosition;

    // 264,328
    // 1241,328
    // 1503,753

    public void OnBeginDrag(PointerEventData eventData) { }

    public void OnDrag(PointerEventData eventData) {
        float yRatio = 0.4355f;
        float xRatioLeft = 0.1756f;
        float xRatioRight = 0.8256f;
        float xpos = eventData.position.x / Screen.width;
        float ypos = eventData.position.y / Screen.height;

        ypos = ypos - yRatio;
        if (left) {
            xpos = xpos - xRatioLeft;
        } else {
            xpos = xpos - xRatioRight;
        }

        Debug.Log("PointPosition: " + PointPosition.ToString()
                  + ", eventData.position: " + eventData.position.ToString());

        ypos = (ypos * Screen.height) / (Background.rect.size.x / 2f);
        xpos = (xpos * Screen.width) / (Background.rect.size.x / 2f);
        if (ypos > 1f) ypos = 1f;
        if (ypos < -1f) ypos = -1f;
        if (xpos > 1f) xpos = 1f;
        if (xpos < -1f) xpos = -1f;
        PointPosition = new Vector2(xpos,ypos);

        Knob.transform.position = new Vector2(eventData.position.x,eventData.position.y);
//(PointPosition.x 
//            *((Background.rect.size.x-Knob.rect.size.x)/2)*offset)
//            + Background.position.x, (PointPosition.y
//            * ((Background.rect.size.y-Knob.rect.size.y)/2) *offset)
//            + Background.position.y);

        if (!left) MouseLookScript.a.Mouselook();
    }

    public void OnEndDrag(PointerEventData eventData) {
        PointPosition = new Vector2(0f,0f);
        Knob.transform.position = Background.position;
    }

    public void OnPointerDown(PointerEventData eventData) {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData) {
        OnEndDrag(eventData);
    }
   
	// Update is called once per frame
	void Update () {
        Horizontal = PointPosition.x;
        Vertical = PointPosition.y;
    }

    public Vector2 Coordinate() {
        return new Vector2(Horizontal,Vertical);
    }
}
