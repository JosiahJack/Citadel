using UnityEngine;
using System.Collections;

public class CodeScreen : MonoBehaviour {
    public int level;
    private MeshRenderer mr;
    private int matIndex = 0;
    private float tickFinished;
    
    void Start() {
        mr = GetComponent<MeshRenderer>();
        tickFinished = PauseScript.a.relativeTime + 0.3f;
    }
    
    void Update() {
        if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;

		if (tickFinished - PauseScript.a.relativeTime > 0.3f) {
			tickFinished = PauseScript.a.relativeTime + 0.3f;
		}

        if (tickFinished > PauseScript.a.relativeTime) return;
        
        tickFinished = PauseScript.a.relativeTime + 0.3f;

        // Integer overload is maximum exclusive.  Confirmed maximum return
		// value is 9 and not 10 for Random.Range's below.
        switch (level) {
			case 1:
			    if (!Const.a.questData.lev1SecCodeLocked) {
			        Const.a.questData.lev1SecCode = UnityEngine.Random.Range(0,10);
			    }
			    
			    matIndex = Const.a.questData.lev1SecCode;
			    break;
			case 2:
			    if (!Const.a.questData.lev2SecCodeLocked) {
			        Const.a.questData.lev2SecCode = UnityEngine.Random.Range(0,10);
			    }
			    
			    matIndex = Const.a.questData.lev2SecCode;
			    break;
			case 3:
			    if (!Const.a.questData.lev3SecCodeLocked) {
			        Const.a.questData.lev3SecCode = UnityEngine.Random.Range(0,10);
			    }
			    
			    matIndex = Const.a.questData.lev3SecCode;
			    break;
			case 4:
			    if (!Const.a.questData.lev4SecCodeLocked) {
			        Const.a.questData.lev4SecCode = UnityEngine.Random.Range(0,10);
			    }
			    
			    matIndex = Const.a.questData.lev4SecCode;
			    break;
			case 5:
			    if (!Const.a.questData.lev5SecCodeLocked) {
			        Const.a.questData.lev5SecCode = UnityEngine.Random.Range(0,10);
			    }
			    
			    matIndex = Const.a.questData.lev5SecCode;
			    break;
			case 6:
			    if (!Const.a.questData.lev6SecCodeLocked) {
			        Const.a.questData.lev6SecCode = UnityEngine.Random.Range(0,10);
			    }
			    
			    matIndex = Const.a.questData.lev6SecCode;
			    break;
		}
		
		mr.material = (Const.a.screenCodes[matIndex]);
    }
}
