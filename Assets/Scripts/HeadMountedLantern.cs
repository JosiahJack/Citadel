using UnityEngine;
using System.Collections;

public class HeadMountedLantern : MonoBehaviour {
	public int lanternState = 0;
	//public int lanternVersion = 0;
	//public float lanternSetting = 1;
	public float lanternVersion1Brightness = 2.5f;
	public float lanternVersion2Brightness = 4;
	public float lanternVersion3Brightness = 5;

	[HideInInspector]
	public Light headlight;

	void Awake () {
		headlight = GetComponent<Light>();
	}
	
	/*void  Update (){
		if (GetInput.a != null && GetInput.a.Lantern()) {
			float brightness = 0f;
			switch(lanternVersion) {
				case 1: brightness = lanternVersion1Brightness; break;
				case 2: brightness = lanternVersion2Brightness; break;
				case 3: brightness = lanternVersion3Brightness; break;
				default: brightness = 0f; break;
			}
				
			switch(lanternState) {
			case 0:
				headlight.intensity = brightness;
				lanternState = 1;
				break;
			default:
				headlight.intensity = 0f;
				lanternState = 0;
				break;
			}
		}
	}*/
}