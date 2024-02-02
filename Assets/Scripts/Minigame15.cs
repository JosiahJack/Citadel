using UnityEngine;
using UnityEngine.UI;

public class Minigame15 : MonoBehaviour {
    public Image[] tileImage;
    public Text[] numText;
    public int[] curNum;
    public GameObject gameOver;
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

    void OnEnable() {
        Reset();
    }

    public void Reset() {
        SetAlignments();

        // TODO: Pick image, set size, hide numbers if using image.

        for (int i=1;i<=16;i++) curNum[i] = i;
        int sixteenIndex = 16;
        int shuffleIter = Const.a.difficultyPuzzle * 4;
        while (shuffleIter > 0) {
            int randIter = 32;
            while (randIter > 0) { // Find cell next to empty slot
                int randint = Random.Range(1,17); // Top end exclusive, [1,16]
                int emp = EmptyNeighbor(randint);
                if (emp != 0) {
                    Slide(randint,emp);
                    sixteenIndex = randint;
                    break;
                }
                randIter--;
            }
            shuffleIter--;
        }

        for (int i=1;i<=16;i++) {
            numText[i].text = curNum[i].ToString();
            if (curNum[i] == 16) numText[i].text = "";
        }
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
    }

    bool Slide(int from, int to) {
        if (from < 1 || to < 1) return false;
        if (from > 16 || to > 16) return false;

        int fromNum = curNum[from];
        int toNum = curNum[to];
        curNum[to] = fromNum;
        curNum[from] = toNum;
        numText[to].text = curNum[to].ToString();
        numText[from].text = "";
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

    public void ButtonClick(int index) {
        Slide(index,EmptyNeighbor(index));
    }

    void BtnCheck(int index) {
        for (int i=1;i<=16;i++) {
            if (curNum[i] == 16) {
                tileImage[i].color = noTileColor;
            } else {
                if (i == index) tileImage[i].color = overColor;
                else            tileImage[i].color = plainColor;
            }
        }

		if (Input.GetMouseButtonDown(0)) {
            Slide(index,EmptyNeighbor(index));
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
