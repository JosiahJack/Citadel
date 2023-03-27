using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerEnergy : MonoBehaviour {
	// External references
	public Text drainText;
	public Text jpmText;
    public AudioClip SFXBatteryUse;
    public AudioClip SFXChargeStationUse;
    public AudioClip SFXPowerExhausted;
	public float energy = 54f; // save

	// Internal references
    private AudioSource SFX;
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
        SFX = GetComponent<AudioSource>();
		tempF = 0;
		drainJPM = 0;
		energy = 54f; //max is 255 
		tickFinished = PauseScript.a.relativeTime + tick + Random.value; // random offset seed to prevent ticks lining up and causing frame hiccups
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
					float dist = TargetID.GetTargetIDSensingRange(false);

					// Automatically lock onto nearby targets.
					if (LevelManager.a.npcsm[LevelManager.a.currentLevel] != null) {
						// Very specific variable names are good right ;)
						int numNPCsOnCurrentLevel = LevelManager.a.npcsm[LevelManager.a.currentLevel].childrenNPCsAICs.Length;
						if (numNPCsOnCurrentLevel > 0) {
							for (int i=0;i<numNPCsOnCurrentLevel;i++) {
								AIController aic = LevelManager.a.npcsm[LevelManager.a.currentLevel].childrenNPCsAICs[i];
								// if NPC is in range....
								if (Vector3.Distance(aic.transform.position,
													 transform.position) < dist) {
									if (!aic.hasTargetIDAttached) {
										WeaponFire.a.CreateTargetIDInstance(-1f,aic.healthManager);
									}
								}
							}
						}
					}
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

    public void TakeEnergy ( float take  ){
		float was = energy;
		if (energy == 0) return;
		if (WeaponCurrent.a.redbull) return; // No energy drain!

		energy -= take;
		if (energy <= 0f) {
			energy = 0f;
			Utils.PlayOneShotSavable(SFX,SFXPowerExhausted); // battery sound
			Const.sprint(Const.a.stringTable[314],WeaponCurrent.a.owner); //Power supply exhausted.
			DeactivateHardwareOnEnergyDepleted();
		}
	}

	public void GiveEnergy (float give, EnergyType type) {
		energy += give;
		if (energy > maxenergy) energy = maxenergy;
        if (type == EnergyType.Battery) {
            Utils.PlayOneShotSavable(SFX,SFXBatteryUse); // battery sound
        }
        if (type == EnergyType.ChargeStation) {
            Utils.PlayOneShotSavable(SFX,SFXChargeStationUse); // charging station sound
        }
    }

	public static string Save(GameObject go) {
		PlayerEnergy pe = go.GetComponent<PlayerEnergy>();
		if (pe == null) {
			Debug.Log("PlayerEnergy missing on savetype of Player!  GameObject.name: " + go.name);
			return "0000.00000|0000.00000";
		}

		string line = System.String.Empty;
		line = Utils.FloatToString(pe.energy); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pe.tickFinished); // float
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		PlayerEnergy pe = go.GetComponent<PlayerEnergy>();
		if (pe == null) {
			Debug.Log("PlayerEnergy.Load failure, pe == null");
			return index + 2;
		}

		if (index < 0) {
			Debug.Log("PlayerEnergy.Load failure, index < 0");
			return index + 2;
		}

		if (entries == null) {
			Debug.Log("PlayerEnergy.Load failure, entries == null");
			return index + 2;
		}

		pe.energy = Utils.GetFloatFromString(entries[index]); index++;
		pe.tickFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		return index;
	}
}
