#pragma strict
var lanternState : float = 0;
var lanternVersion : float = 1;	//TODO: Set to 0
var lanternSetting : float = 1;

function Update () {
	if (Input.GetButtonDown("Lantern")) {
		switch(lanternState) {
		case 0:
			GetComponent.<Light>().intensity = 2;
			lanternState = 1;
			break;
		default:
			GetComponent.<Light>().intensity = 0;
			lanternState = 0;
		}
	}
}