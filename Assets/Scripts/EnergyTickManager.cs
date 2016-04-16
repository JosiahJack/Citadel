using UnityEngine;
using System.Collections;

public class EnergyTickManager : MonoBehaviour {
	public GameObject playerObject;
	public PlayerEnergy playerEnergy;
	public GameObject[] energyTicks;
	
	void  Update (){
		int h = 0;
		int tickcnt = 0;
		
		// Clear health ticks
		for (int i=0;i<24;i++) {
			energyTicks[i].SetActive(false);
		}
		
		// Keep drawing ticks out until playerHealth is met
		while (h <= playerEnergy.energy) {
			energyTicks[tickcnt].SetActive(true);
			tickcnt++;
			h = h + 11;		
		}
	}
}