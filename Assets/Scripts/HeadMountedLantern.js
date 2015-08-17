#pragma strict
var lanternState : float = 0;
var lanternVersion : float = 1;	//TODO: Set to 0
var lanternSetting : float = 1;
var lanternVersion1Brightness : float = 2.5;
var lanternVersion2Brightness : float = 4;
var lanternVersion3Brightness : float = 5;


function Update () {
	if (Input.GetButtonDown("Lantern")) {
		switch(lanternState) {
		case 0:
			GetComponent.<Light>().intensity = lanternVersion1Brightness;
			lanternState = 1;
			break;
		default:
			GetComponent.<Light>().intensity = 0;
			lanternState = 0;
		}
	}
}