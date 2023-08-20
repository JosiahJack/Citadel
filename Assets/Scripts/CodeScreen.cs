using UnityEngine;
using System.Collections;

public class CodeScreen : MonoBehaviour {
    public int level;
    private MeshRenderer mr;
    private int matIndex = 0;
    
    void Start() {
        mr = GetComponent<MeshRenderer>();
    }
    
    void Update() {
        switch (level) {
			case 1: matIndex = Const.a.questData.lev1SecCode; break;
			case 2: matIndex = Const.a.questData.lev2SecCode; break;
			case 3: matIndex = Const.a.questData.lev3SecCode; break;
			case 4: matIndex = Const.a.questData.lev4SecCode; break;
			case 5: matIndex = Const.a.questData.lev5SecCode; break;
			case 6: matIndex = Const.a.questData.lev6SecCode; break;
		}
		
		mr.material = (Const.a.screenCodes[matIndex]);
    }
}
