using UnityEngine;
using System.Collections;

public class HeadMountedLantern : MonoBehaviour {
	public int lanternState = 0;
	public float lanternVersion = 1;	//TODO: Set to 0
	public float lanternSetting = 1;
	public float lanternVersion1Brightness = 2.5f;
	public float lanternVersion2Brightness = 4;
	public float lanternVersion3Brightness = 5;
	
	void  Update (){
		if (GetInput.a != null && GetInput.a.Lantern()) {
			switch(lanternState) {
			case 0:
				GetComponent<Light>().intensity = lanternVersion1Brightness;
				lanternState = 1;
				break;
			default:
				GetComponent<Light>().intensity = 0;
				lanternState = 0;
				break;
			}
		}
	}
}