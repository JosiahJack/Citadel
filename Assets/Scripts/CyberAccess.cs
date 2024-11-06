using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberAccess : MonoBehaviour {
	public string target;
	public string argvalue;

	// Entry Positions:
	// ======================
	// 	LR 210.6834 2.812 -24.378
	// 	L1 195.42 -13.44 33.28
	// 	L2A 157.1608 -15.53 47.331
	// 	L2B 256.0416 -0.716 62.48789
	// 	L5 126.43 29.56733 34.24
	// 	L6 177.612 3.294942 108.7725
	// 	L8 244.735 41.99257 -19.695
	// 	L9 185.161 84.502 -46.04246
	
    public void Use (UseData ud) {
		ud.argvalue = argvalue;
		Const.a.UseTargets(gameObject,ud,target);
		Const.sprint(Const.a.stringTable[441]); // Entering Cyberspace!
		Vector3 entryPosition = new Vector3( 195.42000f, -13.44000f,  33.28000f);
		switch(LevelManager.a.currentLevel) {
			case 0: entryPosition = new Vector3( 210.68340f,   2.81200f, -24.37800f); break;
			case 1: entryPosition = new Vector3( 195.42000f, -13.44000f,  33.28000f); break;
			case 2: 
				if (transform.localPosition.x < -26f ) {
					// Keycard room port at localPosition -34.53611 -27.76395 2.5696
					entryPosition = new Vector3( 157.16080f, -15.53000f,  47.33100f);
				} else {
					// Library port
					entryPosition = new Vector3( 256.04160f,  -0.71600f,  62.48789f);
				}
				
				break;
			case 5: entryPosition = new Vector3( 126.43000f,  29.56733f,  34.24000f); break;
			case 6: entryPosition = new Vector3( 177.61200f,   3.29494f, 108.77250f); break;
			case 8: entryPosition = new Vector3( 244.73500f,  41.99257f, -19.69500f); break;
			case 9: entryPosition = new Vector3( 185.16100f,  84.50200f, -46.04246f); break;
		}
		MouseLookScript.a.EnterCyberspace(entryPosition);
	}
}
