#pragma strict

var playerObject : GameObject;
var playerHealth : PlayerHealth;
var healthTicks : GameObject[];

function Update () {
	var h : int = 0;
	var tickcnt : int = 0;
	
	// Clear health ticks
	for (var i=0;i<24;i++) {
		healthTicks[i].SetActive(false);
	}
	
	// Keep drawing ticks out until playerHealth is met
	while (h < playerHealth.health) {
		healthTicks[tickcnt].SetActive(true);
		tickcnt++;
		h = h + 11;		
	}
}