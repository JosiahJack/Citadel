using UnityEngine;
using UnityEngine.UI;

public class Minigame15 : MonoBehaviour {
    public Image[] tileImage;
    public Text[] numText;
    public int[] curNum;
    public GameObject gameOver;
    public RectTransform sliderButton;
    public Text sliderButtonText;
    public Text winHeader;
    public Text winText;
    public int btnWidth = 28;
    public int btnWidthH = 14; // Buttons are 28x28 pixels.
    public int btnGap = 4;
    public int farLeft = -62;
    public int farUp = 62;
    public Color plainColor = new Color(0.8f,0.75f,0.72f,1f);
    public Color overColor = new Color (1.0f,0.6f,0.5f,1f);
    public Color noTileColor = new Color(0.5f,0.4f,0.4f,1f);
    public int col1Left;
    public int col1Right;
    public int col2Left;
    public int col2Right;
    public int col3Left;
    public int col3Right;
    public int col4Left;
    public int col4Right;
    public int row1Up;
    public int row1Dn;
    public int row2Up;
    public int row2Dn;
    public int row3Up;
    public int row3Dn;
    public int row4Up;
    public int row4Dn;
    public bool[] sliding = new bool[17];
    public Vector2[] slideDir = new Vector2[17];
    private Vector3 pos;
    public Vector3[] position = new Vector3[17];
    private float slideTickFinished;

    void OnEnable() {
        Reset();
    }

    public void Reset() {
        SetAlignments();

        // TODO: Pick image, set size, hide numbers if using image.

        for (int i=1;i<=16;i++) { curNum[i] = i; sliding[i] = false; }
        int sixteenIndex = 16;
        int shuffleIter = Const.a.difficultyPuzzle * 4;
        while (shuffleIter > 0) {
            int randIter = 32;
            while (randIter > 0) { // Find cell next to empty slot
                int randint = Random.Range(1,17); // Top end exclusive, [1,16]
                int emp = EmptyNeighbor(randint);
                if (emp != 0) {
                    Slide(randint,emp,true);
                    sixteenIndex = randint;
                    break;
                }
                randIter--;
            }
            shuffleIter--;
        }

        position[1]  = new Vector3(-48f, 48f,-1f);
        position[2]  = new Vector3(-16f, 48f,-1f);
        position[3]  = new Vector3( 16f, 48f,-1f);
        position[4]  = new Vector3( 48f, 48f,-1f);
        position[5]  = new Vector3(-48f, 16f,-1f);
        position[6]  = new Vector3(-16f, 16f,-1f);
        position[7]  = new Vector3( 16f, 16f,-1f);
        position[8]  = new Vector3( 48f, 16f,-1f);
        position[9]  = new Vector3(-48f,-16f,-1f);
        position[10] = new Vector3(-16f,-16f,-1f);
        position[11] = new Vector3( 16f,-16f,-1f);
        position[12] = new Vector3( 48f,-16f,-1f);
        position[13] = new Vector3(-48f,-48f,-1f);
        position[14] = new Vector3(-16f,-48f,-1f);
        position[15] = new Vector3( 16f,-48f,-1f);
        position[16] = new Vector3( 48f,-48f,-1f);

        for (int i=1;i<=16;i++) {
            numText[i].text = curNum[i].ToString();
            if (curNum[i] == 16) numText[i].text = "";
            tileImage[i].rectTransform.localPosition =
                new Vector3(position[i].x,position[i].y,0f);
        }

        slideTickFinished = PauseScript.a.relativeTime;
    }

    private void SetAlignments() {
        col1Left  = farLeft;
        col1Right = col1Left  + btnWidth;
        col2Left  = col1Right + btnGap;
        col2Right = col2Left  + btnWidth;
        col3Left  = col2Right + btnGap;
        col3Right = col3Left  + btnWidth;
        col4Left  = col3Right + btnGap;
        col4Right = col4Left  + btnWidth;
        row1Up = farUp;
        row1Dn = farUp - btnWidth;
        row2Up = row1Dn - btnGap;
        row2Dn = row2Up - btnWidth;
        row3Up = row2Dn - btnGap;
        row3Dn = row3Up - btnWidth;
        row4Up = row3Dn - btnGap;
        row4Dn = row4Up - btnWidth;
    }

    bool AABBCursorCheck(int left, int right, int up, int down) {
        if (MinigameCursor.a.minigameMouseX < left)  return false;
        if (MinigameCursor.a.minigameMouseX > right) return false;
        if (MinigameCursor.a.minigameMouseY > up)    return false;
        if (MinigameCursor.a.minigameMouseY < down)  return false;
        return true;
    }

    void Update() {
        if (PauseScript.a.Paused()) return;
        if (PauseScript.a.MenuActive()) return;

        if      (AABBCursorCheck(col1Left,col1Right,row1Up,row1Dn)) BtnCheck(1);
        else if (AABBCursorCheck(col2Left,col2Right,row1Up,row1Dn)) BtnCheck(2);
        else if (AABBCursorCheck(col3Left,col3Right,row1Up,row1Dn)) BtnCheck(3);
        else if (AABBCursorCheck(col4Left,col4Right,row1Up,row1Dn)) BtnCheck(4);

        else if (AABBCursorCheck(col1Left,col1Right,row2Up,row2Dn)) BtnCheck(5);
        else if (AABBCursorCheck(col2Left,col2Right,row2Up,row2Dn)) BtnCheck(6);
        else if (AABBCursorCheck(col3Left,col3Right,row2Up,row2Dn)) BtnCheck(7);
        else if (AABBCursorCheck(col4Left,col4Right,row2Up,row2Dn)) BtnCheck(8);

        else if (AABBCursorCheck(col1Left,col1Right,row3Up,row3Dn)) BtnCheck(9);
        else if (AABBCursorCheck(col2Left,col2Right,row3Up,row3Dn)) BtnCheck(10);
        else if (AABBCursorCheck(col3Left,col3Right,row3Up,row3Dn)) BtnCheck(11);
        else if (AABBCursorCheck(col4Left,col4Right,row3Up,row3Dn)) BtnCheck(12);

        else if (AABBCursorCheck(col1Left,col1Right,row4Up,row4Dn)) BtnCheck(13);
        else if (AABBCursorCheck(col2Left,col2Right,row4Up,row4Dn)) BtnCheck(14);
        else if (AABBCursorCheck(col3Left,col3Right,row4Up,row4Dn)) BtnCheck(15);
        else if (AABBCursorCheck(col4Left,col4Right,row4Up,row4Dn)) BtnCheck(16);
        else {
            for (int i=1;i<=16;i++) {
                if (curNum[i] == 16) tileImage[i].color = noTileColor;
                else tileImage[i].color = plainColor;
            }
        }

        if (slideTickFinished < PauseScript.a.relativeTime) {
            slideTickFinished = PauseScript.a.relativeTime + 0.1f;
            for (int i=1;i<=16;i++) {
                if (!sliding[i]) continue;

                pos = tileImage[i].rectTransform.localPosition;
                float x = pos.x;
                float y = pos.y;
                Debug.Log("slideDir: " + slideDir[i].ToString());
                bool slidingLeft = slideDir[i].x < 0;
                bool slidingRight = slideDir[i].x > 0;
                if (slidingRight) x = pos.x + 2f;
                else if (slidingLeft) x = pos.x + -2f;

                bool slidingDown = slideDir[i].y < 0;
                bool slidingUp = slideDir[i].y > 0;
                if (slidingUp) y = pos.y + 2f;
                else if (slidingDown) y = pos.y + -2f;

                Debug.Log("bef sliderButton.localPosition: " + sliderButton.localPosition.ToString());
                sliderButton.localPosition = new Vector3(x,y,-1f);
                Debug.Log("aft sliderButton.localPosition: " + sliderButton.localPosition.ToString());
                float curx = sliderButton.localPosition.x;
                float cury = sliderButton.localPosition.y;
                if (   (slidingLeft  && curx <= position[i].x)
                    || (slidingRight && curx >= position[i].x)
                    || (slidingUp    && cury >= position[i].y)
                    || (slidingDown  && cury <= position[i].y)) {

                    sliderButton.gameObject.SetActive(false);
                    sliderButtonText.text = "";
                    tileImage[i].color = plainColor;
                    numText[i].text = curNum[i].ToString();
                    sliding[i] = false;
                    slideDir[i] = Vector2.zero;
                }

                break;
            }
        }
    }

    bool Slide(int from, int to, bool resetting) {
        if (from < 1 || to < 1) return false;
        if (from > 16 || to > 16) return false;

        int fromNum = curNum[from];
        int toNum = curNum[to];
        if (!resetting) {
            sliderButton.gameObject.SetActive(true);
            sliderButton.localPosition = position[from];
            sliding[to] = true;
            float xdiff = position[to].x - position[from].x;
            xdiff = Mathf.Abs(xdiff) > 2f ? xdiff : 0f;
            float ydiff = position[to].y - position[from].y;
            ydiff = Mathf.Abs(ydiff) > 2f ? ydiff : 0f;
            Vector2 sliddirBefore = new Vector2(xdiff,ydiff);
            slideDir[to] = new Vector2(Utils.Sign(xdiff),Utils.Sign(ydiff));
            slideTickFinished = PauseScript.a.relativeTime + 0.1f;
        }

        curNum[to] = fromNum;
        curNum[from] = toNum;
        numText[to].text = "";
        tileImage[to].color = plainColor;
        numText[from].text = "";
        tileImage[from].color = noTileColor;
        if (!resetting) sliderButtonText.text = curNum[to].ToString();
        return true;
    }

    int EmptyNeighbor(int curdex) {
        int up = curdex - 4; // Index of neighbors
        int dn = curdex + 4;
        int rt = curdex + 1;
        int lt = curdex - 1;
        int lti = 0; // Indices at neighboring cell
        int rti = 0;
        int upi = 0;
        int dni = 0;
        if (lt > 0 && lt < 17) lti = curNum[lt];
        if (rt > 0 && rt < 17) rti = curNum[rt];
        if (up > 0 && up < 17) upi = curNum[up];
        if (dn > 0 && dn < 17) dni = curNum[dn];
        if (lti == 16) return lt;
        if (rti == 16) return rt;
        if (upi == 16) return up;
        if (dni == 16) return dn;
        return 0;
    }

    void BtnCheck(int index) {
        for (int i=1;i<=16;i++) { if (sliding[i]) return; }

        for (int i=1;i<=16;i++) {
            if (curNum[i] == 16) {
                tileImage[i].color = noTileColor;
            } else {
                if (i == index) tileImage[i].color = overColor;
                else            tileImage[i].color = plainColor;
            }
        }

		if (Input.GetMouseButtonDown(0)) {
            Slide(index,EmptyNeighbor(index),false);
            Evaluate();
        }
    }

    void Evaluate() {
        if (   curNum[1] == 1
            && curNum[2] == 2
            && curNum[3] == 3
            && curNum[4] == 4
            && curNum[5] == 5
            && curNum[6] == 6
            && curNum[7] == 7
            && curNum[8] == 8
            && curNum[9] == 9
            && curNum[10] == 10
            && curNum[11] == 11
            && curNum[12] == 12
            && curNum[13] == 13
            && curNum[14] == 14
            && curNum[15] == 15) {

            gameOver.SetActive(true);
            winHeader.text = "PUZZLE SOLVED!";
            winText.text = "CLICK TO PLAY AGAIN";
        }
    }
}
