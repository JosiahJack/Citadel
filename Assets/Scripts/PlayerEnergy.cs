using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;

public class PlayerEnergy : MonoBehaviour {
	// External references
	public Text drainText;
	public Text jpmText;
	public float energy = 54f; // save

	// Internal references
	private float tick = 0.1f;
	[HideInInspector] public float tickFinished; // save
	private float tempF;
	[HideInInspector] public float maxenergy = 255f;
	[HideInInspector] public int drainJPM = 0;
	private string jpm = " J/min";

	public static PlayerEnergy a;

	public void Awake() {
		a = this;
	}

    public void Start() {
		tempF = 0;
		drainJPM = 0;
		energy = 54f; //max is 255 
		tickFinished = PauseScript.a.relativeTime + tick + Random.value; // random offset seed to prevent ticks lining up and causing frame hiccups
    }
    
    void TargetIdentifierSenseTargets() {
		// Automatically lock onto nearby targets.
		if (LevelManager.a.npcsm[LevelManager.a.currentLevel] == null) return;
		
		// Very specific variable names are good right ;)
		int lev = LevelManager.a.currentLevel;
		int numNPCs = LevelManager.a.npcsm[lev].childrenNPCsAICs.Length;
		if (numNPCs <= 0) return;
		
		for (int i=0;i<numNPCs;i++) {
			AIController aic = LevelManager.a.npcsm[lev].childrenNPCsAICs[i];
            if (!aic.gameObject.activeInHierarchy) continue;
            if (aic.healthManager.health <= 0) continue;
            if (aic.hasTargetIDAttached) continue;
            
			// if NPC is in range....
			float far = Vector3.Distance(aic.transform.position,
			                             transform.position);
			if (far > TargetID.GetTargetIDSensingRange(false)) continue;
			
			WeaponFire.a.CreateTargetIDInstance(-1f,aic.healthManager);
		}
    }

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			tempF = 1f;
			bool activeEnergyDrainers = false;
			if (tickFinished < PauseScript.a.relativeTime) {
				drainJPM = 0;
				// 0 System Analyzer doesn't take energy

				// 1 = Navigation Unit doesn't take energy

				// 2 = Datareader doesn't take energy

				// 3 Drain sensaround
				if (Inventory.a.hardwareIsActive[3]) {
					switch (Inventory.a.hardwareVersion[3]) {
						case 0: tempF = 0.01535f; drainJPM += 9; break; // takes about 300s to drain full energy
						case 1: tempF = 0.03413f; drainJPM += 20; break; // takes about 300s to drain full energy
						case 2: tempF = 0.02559f; drainJPM += 15; break; // takes about 240s to drain full energy
					}
					activeEnergyDrainers = true;
					TakeEnergy(tempF);
				}

				// 4 = Target Identifier doesn't take energy
				if (Inventory.a.hasHardware[4]) {
				    TargetIdentifierSenseTargets();
				}

				// 5 = Energy Shield - handled by HealthManager
				if (Inventory.a.hardwareIsActive[5]) {
					switch (Inventory.a.hardwareVersionSetting[5]) {
						case 0: tempF = 0.04096f; drainJPM += 24; break;
						case 1: tempF = 0.10239f; drainJPM += 60; break;
						case 2: tempF = 0.17919f; drainJPM += 105; break;
						case 3: tempF = 0.05119f; drainJPM += 30; break;
					}
					activeEnergyDrainers = true;
					TakeEnergy(tempF);
				}

				// 6 = Biomonitor
				if (Inventory.a.hardwareIsActive[6]) {
					switch (Inventory.a.hardwareVersionSetting[6]) {
						case 0: tempF = 0.001706f; drainJPM += 1;  activeEnergyDrainers = true; break;
						case 1: tempF = 0; break; // doesn't take energy
					}
					if (tempF > 0) TakeEnergy(tempF);
				}

				// 7 = Head Mounted Lantern
				if (Inventory.a.hardwareIsActive[7]) {
					switch (Inventory.a.hardwareVersionSetting[7]) {
						case 0: tempF = 0.02559f; drainJPM += 15; break;// takes about 180s to drain full energy
						case 1: tempF = 0.04266f; drainJPM += 25; break; // takes about 120s to drain full energy
						case 2: tempF = 0.05119f; drainJPM += 30; break; // takes about 90s to drain full energy
					}
					activeEnergyDrainers = true;
					TakeEnergy(tempF);
				}

				// 8 Envirosuit - handled by HealthManager for radiation checks

				// 9 = Turbo Motion Booster - done in PlayerMovement since we only use energy on boost, no drain with skates
				if (Inventory.a.hardwareIsActive[9]) {
					switch (Inventory.a.hardwareVersionSetting[9]) {
						case 0: tempF = 0f; break;
						case 1: tempF = 0.02f; drainJPM += 16; break; // takes about 120s to drain full energy
						case 2: tempF = 0.015f; drainJPM += 12; break; // takes about 90s to drain full energy
					}
					activeEnergyDrainers = true;
					if (tempF > 0) TakeEnergy(tempF);
				}

				// 10 Jump Jet Boots - done in PlayerMovement since we only drain while jumping

				// 11 Drain nightsight
				if (Inventory.a.hardwareIsActive [11]) {
					tempF = 0.08533f; drainJPM += 50; // takes about 120s to drain full energy
					activeEnergyDrainers = true;
					TakeEnergy(tempF);
				}
				tickFinished = PauseScript.a.relativeTime + tick;
			}

			// Turn everything off when we are out of energy
			if (activeEnergyDrainers && energy == 0) {
				DeactivateHardwareOnEnergyDepleted();
				activeEnergyDrainers = false;
				 drainJPM = 0;
			}
			if (drainJPM > 0) {
				drainText.text = drainJPM.ToString();
				jpmText.text = jpm;
			} else {
				drainText.text = System.String.Empty;
				jpmText.text = System.String.Empty;
			}
		}
	}

	void DeactivateHardwareOnEnergyDepleted() {
		Inventory.a.hardwareIsActive[3] = false;
		Inventory.a.hardwareButtonManager.SensaroundOff(); //sensaround
		if (Inventory.a.hardwareIsActive [6] && Inventory.a.hardwareVersionSetting[6] == 0) Inventory.a.hardwareButtonManager.BioOff(); // biomonitor, but only on v1, v2 doesn't use power
		if (Inventory.a.hardwareIsActive [5]) Inventory.a.hardwareButtonManager.ShieldOff(); // shield
		if (Inventory.a.hardwareIsActive [7]) Inventory.a.hardwareButtonManager.LanternOff(); // lantern
		if (Inventory.a.hardwareIsActive [9]) Inventory.a.hardwareButtonManager.BoosterOff(); // turbo motion booster
		if (Inventory.a.hardwareIsActive [11]) Inventory.a.hardwareButtonManager.InfraredOff(); // infrared
	}

    public void TakeEnergy(float take) {
		float was = energy;
		if (energy == 0) return;
		if (WeaponCurrent.a.redbull) return; // No energy drain!

		energy -= take;
		if (energy <= 0f) {
			energy = 0f;
			Utils.PlayUIOneShotSavable(84); // energy_gone
			Const.sprint(314); //Power supply exhausted.
			DeactivateHardwareOnEnergyDepleted();
		}
	}

	public void GiveEnergy(float give, EnergyType type) {
		energy += give;
		if (energy > maxenergy) energy = maxenergy;
        if (type == EnergyType.Battery) {
            Utils.PlayUIOneShotSavable(79); // batteryuse
        }
        if (type == EnergyType.ChargeStation) {
            Utils.PlayUIOneShotSavable(100); // chargingstation
        }
    }

	public static string Save(GameObject go) {
		PlayerEnergy pe = go.GetComponent<PlayerEnergy>();
		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		s1.Append(Utils.FloatToString(pe.energy,"energy"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pe.tickFinished,"tickFinished"));
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		PlayerEnergy pe = go.GetComponent<PlayerEnergy>();
		pe.energy = Utils.GetFloatFromString(entries[index],"energy"); index++;
		pe.tickFinished = Utils.LoadRelativeTimeDifferential(entries[index],"tickFinished"); index++;
		return index;
	}
}
