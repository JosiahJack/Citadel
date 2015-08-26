using UnityEngine;
using System.Collections;

public class HealthTickManager : MonoBehaviour {
	public GameObject playerObject;
	public PlayerHealth playerHealth;
	public GameObject[] healthTicks;
	
	void  Update (){
		int h = 0;
		int tickcnt = 0;
		
		// Clear health ticks
		for (int i=0;i<24;i++) {
			healthTicks[i].SetActive(false);
		}
		
		// Keep drawing ticks out until playerHealth is met
		while (h < playerHealth.health) {
			healthTicks[tickcnt].SetActive(true);
			tickcnt++;
			h = h + 11;		
		}
	}
}