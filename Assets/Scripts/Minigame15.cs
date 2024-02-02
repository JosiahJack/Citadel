using UnityEngine;
using UnityEngine.UI;

public class Minigame15 : MonoBehaviour {
    public Image[] tileImage;
    public Text[] numText;
    public int[] curNum;

    void OnEnable() {
        for (int i=1;i<=16;i++) {
            Utils.AssignImageOverride(tileImage[i],Const.a.useableItemsIcons[0]);
            numText[i].text = i.ToString();
            if (i == 16) numText[i].text = "";
            curNum[i] = i;
        }
    }

    void Update() {
        for (int i=1;i<=16;i++) {
            if (curNum[i] == 16) tileImage[i].color = Color.gray;
            else tileImage[i].color = Color.white;
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
        Debug.Log("index: " + index.ToString());
        Slide(index,EmptyNeighbor(index));
    }
}
