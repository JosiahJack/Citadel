using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerEnergy : MonoBehaviour {
	public float energy = 54f; //max is 255 // save
	public float resetAfterDeathTime = 0.5f;
	public float timer; // save
    private AudioSource SFX;
    public AudioClip SFXBatteryUse;
    public AudioClip SFXChargeStationUse;
    public AudioClip SFXPowerExhausted;
	public WeaponCurrent wepCur;
	public HardwareInvCurrent hwc;
	public HardwareInventory hwi;
	private float tick = 0.1f;
	[HideInInspector]
	public float tickFinished; // save
	private float tempF;
	[HideInInspector]
	public float maxenergy = 255f;
	public int drainJPM = 0;
	public Text drainText;
	public Text jpmText;
	private string jpm = " J/min";
	public WeaponFire wepFire;

    public void Start() {
        SFX = GetComponent<AudioSource>();
		tempF = 0;
		drainJPM = 0;
		tickFinished = PauseScript.a.relativeTime + tick + Random.value; // random offset seed to prevent ticks lining up and causing frame hiccups
    }

	void Update () {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			tempF = 1f;
			bool activeEnergyDrainers = false;
			if (tickFinished < PauseScript.a.relativeTime) {
				drainJPM = 0;
				// 0 System Analyzer doesn't take energy

				// 1 = Navigation Unit doesn't take energy

				// 2 = Datareader doesn't take energy

				// 3 Drain sensaround
				if (hwc.hardwareIsActive[3]) {
					switch (hwi.hardwareVersionSetting[3]) {
						case 0: tempF = 0.08533f; drainJPM += 50; break; // takes about 300s to drain full energy
						case 1: tempF = 0.08533f; drainJPM += 50; break; // takes about 300s to drain full energy
						case 2: tempF = 0.10666f; drainJPM += 64; break; // takes about 240s to drain full energy
					}
					activeEnergyDrainers = true;
					TakeEnergy(tempF);
				}

				// 4 = Target Identifier doesn't take energy
				if (HardwareInventory.a.hasHardware[4]) {
					float sensingRange = 10f;
					switch (HardwareInventory.a.hardwareVersion[4]) {
						case 1: sensingRange = 10f; break;
						case 2: sensingRange = 20f; break;
						case 3: sensingRange = 30f; break;
						case 4: sensingRange = 40f; break;
					}
					if (HardwareInventory.a.hardwareVersion[4] > 3) {
						if (LevelManager.a.npcsm[LevelManager.a.currentLevel] != null) {
							int numNPCsOnCurrentLevel = LevelManager.a.npcsm[LevelManager.a.currentLevel].childrenNPCsAICs.Length; // very specific variable names are good right
							if (numNPCsOnCurrentLevel > 0) {
								for (int i=0;i<numNPCsOnCurrentLevel;i++) {
									// if NPC is in range....
									if (Vector3.Distance(LevelManager.a.npcsm[LevelManager.a.currentLevel].childrenNPCsAICs[i].transform.position,transform.position) < sensingRange) {
										if (LevelManager.a.npcsm[LevelManager.a.currentLevel].childrenNPCsAICs[i].healthManager.health > 0f && !LevelManager.a.npcsm[LevelManager.a.currentLevel].childrenNPCsAICs[i].hasTargetIDAttached) {
											bool createdAnIDInstance = wepFire.CreateTargetIDInstance(System.String.Empty,LevelManager.a.npcsm[LevelManager.a.currentLevel].childrenNPCsAICs[i].transform,LevelManager.a.npcsm[LevelManager.a.currentLevel].childrenNPCsAICs[i].healthManager,transform,sensingRange,true,true,true,true);
											if (!createdAnIDInstance) break;
										}
									}
								}
							}
						}
					}
				}

				// 5 = Energy Shield - handled by HealthManager

				// 6 = Biomonitor
				if (hwc.hardwareIsActive[6]) {
					switch (hwi.hardwareVersionSetting[6]) {
						case 0: tempF = 0.08533f; drainJPM += 50;  activeEnergyDrainers = true; break; // takes about 300s to drain full energy
						case 1: tempF = 0; break; // doesn't take energy
					}
					if (tempF > 0) TakeEnergy(tempF);
				}

				// 7 = Head Mounted Lantern
				if (hwc.hardwareIsActive[7]) {
					switch (hwi.hardwareVersionSetting[7]) {
						case 0: tempF = 0.1422f; drainJPM += 85; break;// takes about 180s to drain full energy
						case 1: tempF = 0.21f; drainJPM += 125; break; // takes about 120s to drain full energy
						case 2: tempF = 0.28f; drainJPM += 170; break; // takes about 90s to drain full energy
					}
					activeEnergyDrainers = true;
					TakeEnergy(tempF);
				}

				// 8 Envirosuit - handled by HealthManager for radiation checks

				// 9 = Turbo Motion Booster - done in PlayerMovement since we only use energy on boost, no drain with skates

				// 10 Drain jump jet boots - done in PlayerMovement since we only drain while jumping

				// 11 Drain nightsight
				if (hwc.hardwareIsActive [11]) {
					tempF = 0.21f; drainJPM += 125; // takes about 120s to drain full energy
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
		hwc.hardwareIsActive [3] = false;
		if (hwc.hardwareIsActive [3]) hwc.hardwareButtons[1].SensaroundOff(); //sensaround
		if (hwc.hardwareIsActive [6] && hwi.hardwareVersionSetting[6] == 0) hwc.hardwareButtons[0].BioOff(); // biomonitor, but only on v1, v2 doesn't use power
		if (hwc.hardwareIsActive [5]) hwc.hardwareButtons[3].ShieldOff(); // shield
		if (hwc.hardwareIsActive [7]) hwc.hardwareButtons[2].LanternOff(); // lantern
		if (hwc.hardwareIsActive [9]) hwc.hardwareButtons[6].BoosterOff(); // turbo motion booster
		if (hwc.hardwareIsActive [11]) hwc.hardwareButtons[4].InfraredOff(); // infrared
	}

    public void TakeEnergy ( float take  ){
		float was = energy;
		if (energy == 0) return;
		if (wepCur != null) {
			if (wepCur.redbull) return; // no energy drain!
		}

		energy -= take;
		if (energy <= 0f) {
			energy = 0f;
			if (SFX != null && SFXPowerExhausted != null) SFX.PlayOneShot(SFXPowerExhausted); // battery sound
			Const.sprint(Const.a.stringTable[314],wepCur.owner); //Power supply exhausted.
			DeactivateHardwareOnEnergyDepleted();
		}
		//print("Player Energy: " + energy.ToString());
		//Debug.Log(take.ToString("F4") +" player energy taken.  Was: " + was.ToString() +", Now: " + energy.ToString());
	}

	public void GiveEnergy (float give, int type) {
		energy += give;
		if (energy > maxenergy) {
			energy = maxenergy;
		}

        if (type == 0) {
            if (SFX != null && SFXBatteryUse != null) SFX.PlayOneShot(SFXBatteryUse); // battery sound
        }
        if (type == 1) {
            if (SFX != null && SFXChargeStationUse != null) SFX.PlayOneShot(SFXChargeStationUse); // charging station sound
        }
    }
}