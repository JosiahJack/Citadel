using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using UnityStandardAssets.ImageEffects;

public class Const : MonoBehaviour {
	[SerializeField] public GameObject[] useableItems;
	[SerializeField] public Texture2D[] useableItemsFrobIcons;
    [SerializeField] public Sprite[] useableItemsIcons;
    [SerializeField] public string[] useableItemsNameText;
	[SerializeField] public Sprite[] searchItemIconSprites;
	[SerializeField] public string[] audiologNames;
	[SerializeField] public string[] audiologSenders;
	[SerializeField] public string[] audiologSubjects;
	[SerializeField] public AudioClip[] audioLogs;
	[SerializeField] public int[] audioLogType;  // 0 = text only, 1 = normal, 2 = email, 3 = vmail
	[SerializeField] public string[] audioLogSpeech2Text;
	[SerializeField] public int[] audioLogLevelFound;
	[SerializeField] public bool[] isFullAutoForWeapon;
	[SerializeField] public float[] delayBetweenShotsForWeapon;
	[SerializeField] public float[] delayBetweenShotsForWeapon2;
	[SerializeField] public float[] damagePerHitForWeapon;
	[SerializeField] public float[] damagePerHitForWeapon2;
	[SerializeField] public float[] damageOverloadForWeapon;
	[SerializeField] public float[] energyDrainLowForWeapon;
	[SerializeField] public float[] energyDrainHiForWeapon;
	[SerializeField] public float[] energyDrainOverloadForWeapon;
	[SerializeField] public float[] penetrationForWeapon;
	[SerializeField] public float[] penetrationForWeapon2;
	[SerializeField] public float[] offenseForWeapon;
	[SerializeField] public float[] offenseForWeapon2;
	[SerializeField] public float[] rangeForWeapon;
	[SerializeField] public int[] magazinePitchCountForWeapon;
	[SerializeField] public int[] magazinePitchCountForWeapon2;
	[SerializeField] public string[] genericText;
	public enum AttackType{None,Melee,EnergyBeam,Magnetic,Projectile,ProjectileEnergyBeam};
	[SerializeField] public AttackType[] attackTypeForWeapon;
	public float doubleClickTime = 0.500f;
	public float frobDistance = 5f;
	public enum PoolType{DartImpacts,SparqImpacts,CameraExplosions,ProjEnemShot2,Sec2BotRotMuzBursts,Sec2BotMuzBursts,LaserLines};
	public GameObject Pool_DartImpacts;
	public GameObject Pool_SparqImpacts;
	public GameObject Pool_CameraExplosions;
	public GameObject Pool_ProjectilesEnemShot2;
	public GameObject Pool_Sec2BotRotaryMuzzleBursts;
	public GameObject Pool_Sec2BotMuzzleBursts;
	public GameObject Pool_LaserLines;
	public GameObject statusBar;
    public static Const a;
	public int difficultyCombat;
	public int difficultyMission;
	public int difficultyPuzzle;
	public int difficultyCyber;
	public string playerName;
	public AudioSource mainmenuMusic;
	public int GraphicsResWidth;
	public int GraphicsResHeight;
	public bool GraphicsFullscreen;
	public bool GraphicsSSAO;
	public bool GraphicsBloom;
	public int GraphicsFOV;
	public int GraphicsGamma;
	public int AudioSpeakerMode;
	public bool AudioReverb;
	public int AudioVolumeMaster;
	public int AudioVolumeMusic;
	public int AudioVolumeMessage;
	public int AudioVolumeEffects;
	public int AudioLanguage;
	public bool AudioSubtitles;
	public int[] InputCodeSettings;
	public string[] InputCodes;
	public string[] InputValues;
	public string[] InputConfigNames;
	public bool InputInvertLook;
	public bool InputInvertCyberspaceLook;
	public bool InputInvertInventoryCycling;
	public bool InputQuickItemPickup;
	public bool InputQuickReloadWeapons;
	public GameObject playerCamera;

	// Instantiate it so that it can be accessed globally. MOST IMPORTANT PART!!
	// =========================================================================
	void Awake() {
		Application.targetFrameRate = 60;
		a = this;
		//for (int i=0;i<Display.displays.Length;i++) {
		//	Display.displays[i].Activate();
		//}
	}
	// =========================================================================
	void Start() {
		LoadConfig();
		LoadAudioLogMetaData();
		LoadItemNamesData();
		LoadDamageTablesData();
	}

	private void LoadConfig() {
		// Graphics Configurations
		GraphicsResWidth = AssignConfigInt("Graphics","ResolutionWidth");
		GraphicsResHeight = AssignConfigInt("Graphics","ResolutionHeight");
		GraphicsFullscreen = AssignConfigBool("Graphics","Fullscreen");
		GraphicsSSAO = AssignConfigBool("Graphics","SSAO");
		GraphicsBloom = AssignConfigBool("Graphics","Bloom");
		GraphicsFOV = AssignConfigInt("Graphics","FOV");
		GraphicsGamma = AssignConfigInt("Graphics","Gamma");

		// Audio Configurations
		AudioSpeakerMode = AssignConfigInt("Audio","SpeakerMode");
		AudioReverb = AssignConfigBool("Audio","Reverb");
		AudioVolumeMaster = AssignConfigInt("Audio","VolumeMaster");
		AudioVolumeMusic = AssignConfigInt("Audio","VolumeMusic");
		AudioVolumeMessage = AssignConfigInt("Audio","VolumeMessage");
		AudioVolumeEffects = AssignConfigInt("Audio","VolumeEffects");
		AudioLanguage = AssignConfigInt("Audio","Language");  // defaults to 0 = english
		AudioSubtitles = AssignConfigBool("Audio","Subtitles");

		// Input Configurations
		for (int i=0;i<40;i++) {
			string inputCapture = INIWorker.IniReadValue("Input",InputCodes[i]);
			for (int j=0;j<159;j++) {
				if (InputValues[j] == inputCapture) {
					InputCodeSettings[i] = j;
				}
			}
		}
		InputInvertLook = AssignConfigBool("Input","InvertLook");
		InputInvertCyberspaceLook = AssignConfigBool("Input","InvertCyberspaceLook");
		InputInvertInventoryCycling = AssignConfigBool("Input","InvertInventoryCycling");
		InputQuickItemPickup = AssignConfigBool("Input","QuickItemPickup");
		InputQuickReloadWeapons = AssignConfigBool("Input","QuickReloadWeapons");

		SetVolume();
		Screen.SetResolution(GraphicsResWidth,GraphicsResHeight,true);
		Screen.fullScreen = Const.a.GraphicsFullscreen;
	}

	public void WriteConfig() {
		INIWorker.IniWriteValue("Graphics","ResolutionWidth",GraphicsResWidth.ToString());
		INIWorker.IniWriteValue("Graphics","ResolutionHeight",GraphicsResHeight.ToString());
		INIWorker.IniWriteValue("Graphics","Fullscreen",GetBoolAsString(GraphicsFullscreen));
		INIWorker.IniWriteValue("Graphics","SSAO",GetBoolAsString(GraphicsSSAO));
		INIWorker.IniWriteValue("Graphics","Bloom",GetBoolAsString(GraphicsBloom));
		INIWorker.IniWriteValue("Graphics","FOV",GraphicsFOV.ToString());
		INIWorker.IniWriteValue("Graphics","Gamma",GraphicsGamma.ToString());
		INIWorker.IniWriteValue("Audio","SpeakerMode",AudioSpeakerMode.ToString());
		INIWorker.IniWriteValue("Audio","Reverb",GetBoolAsString(AudioReverb));
		INIWorker.IniWriteValue("Audio","VolumeMaster",AudioVolumeMaster.ToString());
		INIWorker.IniWriteValue("Audio","VolumeMusic",AudioVolumeMusic.ToString());
		INIWorker.IniWriteValue("Audio","VolumeMessage",AudioVolumeMessage.ToString());
		INIWorker.IniWriteValue("Audio","VolumeEffects",AudioVolumeEffects.ToString());
		INIWorker.IniWriteValue("Audio","Language",AudioLanguage.ToString());
		INIWorker.IniWriteValue("Audio","Subtitles",GetBoolAsString(AudioSubtitles));
		for (int i=0;i<40;i++) {
			INIWorker.IniWriteValue("Input",InputCodes[i],InputValues[InputCodeSettings[i]]);
		}
		INIWorker.IniWriteValue("Input","InvertLook",GetBoolAsString(InputInvertLook));
		INIWorker.IniWriteValue("Input","InvertCyberspaceLook",GetBoolAsString(InputInvertCyberspaceLook));
		INIWorker.IniWriteValue("Input","InvertInventoryCycling",GetBoolAsString(InputInvertInventoryCycling));
		INIWorker.IniWriteValue("Input","QuickItemPickup",GetBoolAsString(InputQuickItemPickup));
		INIWorker.IniWriteValue("Input","QuickReloadWeapons",GetBoolAsString(InputQuickReloadWeapons));
		SetBloom();
		SetSSAO();
	}
		
	// Check if particular bit is 1 (ON/TRUE) in binary format of given integer
	public bool CheckFlags (int checkInt, int flag) {
		if ((checkInt & flag) != 0)
			return true;

		return false;
	}

	private void EnterLogMetaDataToArrays (int index, string name, string sender, string subject, int ltype, int levelFoundOn, string text) {
		audiologNames[index] = name;
		audiologSenders[index] = sender;
		audiologSubjects[index] = subject;
		audioLogType[index] = ltype;
		audioLogLevelFound[index] = levelFoundOn;
		audioLogSpeech2Text[index] = text;
	}

	private void LoadAudioLogMetaData () {
		// The following to be assigned to the arrays in the Unity Const data structure
		int readIndexOfLog; // look-up index for assigning the following data on the line in the file to the arrays
		string readNameOfLog; // loaded into string audiologNames[]
		string readSenderOfLog; // loaded into string audiologSenders[]
		string readSubjectOfLog; // loaded into string audiologSubjects[]
		int readLogType; // loaded into bool audioLogType[]
		int readLevelFoundOn; // loaded into int audioLogLevelFound[]
		string readLogText; // loaded into string audioLogSpeech2Text[]

		string readline; // variable to hold each string read in from the file
		int currentline = 0;

		StreamReader dataReader = new StreamReader(Application.dataPath + "/Resources/logs_text.txt",Encoding.Default);
		using (dataReader) {
			do {
				// Read the next line
				readline = dataReader.ReadLine();
				if (readline == null) break; // just in case
			
				//char[] delimiters = new char[] {','};
				string[] entries = readline.Split(',');
				bool parsed = Int32.TryParse(entries[0],out readIndexOfLog);
				if (!parsed) sprint("BUG: Could not parse into integer readIndexOfLog from log_text.txt file on line " + currentline.ToString());
				readNameOfLog = entries[1];
				readSenderOfLog = entries[2];
				readSubjectOfLog = entries[3];
				parsed = Int32.TryParse(entries[4],out readLogType);
				if (!parsed) sprint("BUG: Could not parse into integer readLogType from log_text.txt file on line " + currentline.ToString());
				parsed = Int32.TryParse(entries[5],out readLevelFoundOn);
				if (!parsed) sprint("BUG: Could not parse into integer readLevelFoundOn from log_text.txt file on line " + currentline.ToString());
				readLogText = entries[6];

				// handle extra commas within the body text and append remaining portions of the line
				if (entries.Length > 7) {
					for (int i=7;i<entries.Length;i++) {
						readLogText = (readLogText +"," + entries[i]);  // combine remaining portions of text after other commas and add comma back
					}
				}

				// Send this line of data to all the correct arrays
				EnterLogMetaDataToArrays(readIndexOfLog,readNameOfLog,readSenderOfLog,readSubjectOfLog,readLogType,readLevelFoundOn,readLogText);
				currentline++;
			} while (!dataReader.EndOfStream);
			dataReader.Close();
			return;
		}
	}

	private void LoadItemNamesData () {
		string readline; // variable to hold each string read in from the file
		int currentline = 0;

		StreamReader dataReader = new StreamReader(Application.dataPath + "/Resources/item_names.txt",Encoding.Default);
		using (dataReader) {
			do {
				// Read the next line
				readline = dataReader.ReadLine();
				if (readline == null) break; // just in case
				useableItemsNameText[currentline] = readline;
				currentline++;
			} while (!dataReader.EndOfStream);
			dataReader.Close();
			return;
		}
	}

	private void LoadDamageTablesData () {
		string readline; // variable to hold each string read in from the file
		int currentline = 0;
		bool readBool = false; // it gives you wings!
		int readInt = 0;
		float readFloat = 0f;
		AttackType readAttType = AttackType.None;

		StreamReader dataReader = new StreamReader(Application.dataPath + "/Resources/damage_tables.txt",Encoding.Default);
		using (dataReader) {
			do {
				// Read the next line
				readline = dataReader.ReadLine();
				//char[] delimiters = new char[] {','};
				string[] entries = readline.Split(',');
				bool parsed = Int32.TryParse(entries[0],out readInt);
				if (parsed) {
					if (readInt == 1) readBool = true; else readBool = false;
					isFullAutoForWeapon[currentline] = readBool;
				} else { sprint("BUG: Could not parse into bool isFullAutoForWeapon from damage_tables.txt file on line " + currentline.ToString()); }

				parsed = Single.TryParse(entries[1],out readFloat);
				if (parsed) { delayBetweenShotsForWeapon[currentline] = readFloat;
				} else { sprint("BUG: Could not parse into float delayBetweenShotsForWeapon from damage_tables.txt file on line " + currentline.ToString()); }

				parsed = Single.TryParse(entries[2],out readFloat);
				if (parsed) { delayBetweenShotsForWeapon2[currentline] = readFloat;
				} else { sprint("BUG: Could not parse into float delayBetweenShotsForWeapon2 from damage_tables.txt file on line " + currentline.ToString()); }

				parsed = Int32.TryParse(entries[3],out readInt);
				if (parsed) { damagePerHitForWeapon[currentline] = (float)readInt;
				} else { sprint("BUG: Could not parse into float damagePerHitForWeapon from damage_tables.txt file on line " + currentline.ToString()); }

				parsed = Int32.TryParse(entries[4],out readInt);
				if (parsed) { damagePerHitForWeapon2[currentline] = (float)readInt;
				} else { sprint("BUG: Could not parse into float damagePerHitForWeapon2 from damage_tables.txt file on line " + currentline.ToString()); }

				parsed = Int32.TryParse(entries[5],out readInt);
				if (parsed) { damageOverloadForWeapon[currentline] = (float)readInt;
				} else { sprint("BUG: Could not parse into float damageOverloadForWeapon from damage_tables.txt file on line " + currentline.ToString()); }

				parsed = Int32.TryParse(entries[6],out readInt);
				if (parsed) { energyDrainLowForWeapon[currentline] = (float)readInt;
				} else { sprint("BUG: Could not parse into float energyDrainLowForWeapon from damage_tables.txt file on line " + currentline.ToString()); }

				parsed = Int32.TryParse(entries[7],out readInt);
				if (parsed) { energyDrainHiForWeapon[currentline] = (float)readInt;
				} else { sprint("BUG: Could not parse into float energyDrainHiForWeapon from damage_tables.txt file on line " + currentline.ToString()); }

				parsed = Int32.TryParse(entries[8],out readInt);
				if (parsed) { energyDrainOverloadForWeapon[currentline] = (float)readInt;
				} else { sprint("BUG: Could not parse into float energyDrainOverloadForWeapon from damage_tables.txt file on line " + currentline.ToString()); }

				parsed = Int32.TryParse(entries[9],out readInt);
				if (parsed) { penetrationForWeapon[currentline] = (float)readInt;
				} else { sprint("BUG: Could not parse into float penetrationForWeapon from damage_tables.txt file on line " + currentline.ToString()); }

				parsed = Int32.TryParse(entries[10],out readInt);
				if (parsed) { penetrationForWeapon2[currentline] = (float)readInt;
				} else { sprint("BUG: Could not parse into float penetrationForWeapon2 from damage_tables.txt file on line " + currentline.ToString()); }

				parsed = Int32.TryParse(entries[11],out readInt);
				if (parsed) { offenseForWeapon[currentline] = (float)readInt;
				} else { sprint("BUG: Could not parse into float offenseForWeapon from damage_tables.txt file on line " + currentline.ToString()); }

				parsed = Int32.TryParse(entries[12],out readInt);
				if (parsed) { offenseForWeapon2[currentline] = (float)readInt;
				} else { sprint("BUG: Could not parse into float offenseForWeapon2 from damage_tables.txt file on line " + currentline.ToString()); }

				parsed = Int32.TryParse(entries[13],out readInt);
				if (parsed) { rangeForWeapon[currentline] = (float)readInt;
				} else { sprint("BUG: Could not parse into float rangeForWeapon from damage_tables.txt file on line " + currentline.ToString()); }

				parsed = Int32.TryParse(entries[14],out readInt);
				if (parsed) {
					switch (readInt) {
					case 0: readAttType = AttackType.None;
						break;
					case 1: readAttType = AttackType.Melee;
						break;
					case 2: readAttType = AttackType.EnergyBeam;
						break;
					case 3: readAttType = AttackType.Magnetic;
						break;
					case 4: readAttType = AttackType.Projectile;
						break;
					case 5: readAttType = AttackType.ProjectileEnergyBeam;
						break;
					}
					attackTypeForWeapon[currentline] = readAttType;
				} else { sprint("BUG: Could not parse into AttackType(enum) attackTypeForWeapon from damage_tables.txt file on line " + currentline.ToString()); }

				currentline++;
			} while (!dataReader.EndOfStream);
			dataReader.Close();
			return;
		}
	}

	// StatusBar Print
	public static void sprint (string input) {
		print(input);  // print to console
		if (a != null) {
			if (a.statusBar != null)
				a.statusBar.GetComponent<StatusBarTextDecay>().SendText(input);
		}
	}

	public GameObject GetObjectFromPool(PoolType pool) {
		GameObject poolContainer = Pool_DartImpacts;
		string poolName = " ";

		switch (pool) {
		case PoolType.DartImpacts: 
			poolContainer = Pool_DartImpacts;
			poolName = "DartImpacts ";
			break;
		case PoolType.SparqImpacts:
			poolContainer = Pool_SparqImpacts;
			poolName = "SparqImpacts ";
			break;
		case PoolType.CameraExplosions:
			poolContainer = Pool_CameraExplosions;
			poolName = "CameraExplosions ";
			break;
		case PoolType.ProjEnemShot2:
			poolContainer = Pool_ProjectilesEnemShot2;
			poolName = "ProjectilesEnemShot2 ";
			break;
		case PoolType.Sec2BotRotMuzBursts:
			poolContainer = Pool_Sec2BotRotaryMuzzleBursts;
			poolName = "Sec2BotRotaryMuzzleBursts ";
			break;
		case PoolType.Sec2BotMuzBursts:
			poolContainer = Pool_Sec2BotMuzzleBursts;
			poolName = "Sec2BotMuzzleBursts ";
			break;
		case PoolType.LaserLines:
			poolContainer = Pool_LaserLines;
			poolName = "LaserLines ";
			break;
		}

		if (poolContainer == null) {
			sprint("Cannot find " + poolName + "pool");
			return null;
		}

		for (int i=0;i<poolContainer.transform.childCount;i++) {
			Transform child = poolContainer.transform.GetChild(i);
			if (child.gameObject.activeInHierarchy == false) {
				//sprint("Found a free object in pool: " + poolName.ToString());
				return child.gameObject;
			}
		}

		return null;
	}

	// ========================DAMAGE SYSTEM===========================
	// 0. First checks against whether the entity is damageable (i.e. not the world)
	// 1. Armor absorption (see ICE Breaker Guide for all of 4 these)
	// 2. Weapon vulnerabilities based on attack type and the a_att_type bits stored in the npc
	// 3. Critical hits, chance for critical hit damage based on defense and offense of attack and target
	// 4. Random Factor, +/- 10% damage for randomness
	// 5. Apply velocity for damage, this is after all the above because otherwise the damage multipliers wouldn't affect velocity
	// 6. Return the damage to original TakeDamage() function

	public float GetDamageTakeAmount (float a_damage, float a_offense, float a_penetration, AttackType a_att_type, bool a_notclient, float o_armorvalue, float o_defense) {
		float take = 0f;
		float chance = 0f;
		float f = 0f;
		// 1. Armor Absorption
		if (o_armorvalue > a_penetration) {
			take = (a_damage - a_penetration);
		} else {
			take = a_damage;
		}

		// 2. Weapon Vulnerabilities
		if (a_att_type != AttackType.None) {

		}

		// 3. Critical Hits (NPCs only)
		if (a_notclient) {
			f = (a_offense - o_defense);
			float crit = f;
			if (f > 0) {
				// 71% success with 5/6  5 = f, 6 = max offense or defense value
				// 62% success with 4/6
				// 50% success with 3/6
				// 24% success with 2/6
				// 10% success with 1/6
				chance = (f/6); // 5/6|4/6|3/6|2/6|1/6 = .833|.666|.5|.333|.166
				if (f == 1)
					chance = 0.280f; //anything less than 0.25, 0.1666 in this case taken from 1/6, will fail

				chance = (chance * UnityEngine.Random.Range(0f,1f) * 2);
				if (chance > 0.5f) {
					// SUCCESS! Applying critical hit.
					crit = (take * f);  //How many extra damages we add to damage that we will take
					take = (take + crit); // Maximum extra is 5X + 1X Damage
				}
			}
		}

		// 4. Random Factor +/- 10% (aka 0.10 damage)
		chance = (0.1f * UnityEngine.Random.Range(0f,1f));

		// 50% chance of being positive or negative
		f = UnityEngine.Random.Range(0f,1f);
		if (f > 0.5f) {
			chance = (chance * (-1)); // Make negative
		}
		chance = (chance * take);
		take = (take + chance); // Add the random factor, anywhere up to +/- 10%
		if (take <= 0f)
			return take;

		// 5. Apply Velocity for Damage Amount

		// 6. Return the Damage 
		return take;
	}

	/*
	public static void drawDebugLine(Vector3 start , Vector3 end, Color color,float duration = 0.2f){
		StartCoroutine( drawLine(start, end, color, duration));
	}

	IEnumerator drawLine(Vector3 start , Vector3 end, Color color,float duration = 0.2f){
		GameObject myLine = new GameObject ();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer> ();
		LineRenderer lr = myLine.GetComponent<LineRenderer> ();
		lr.material = new Material (Shader.Find ("Particles/Additive"));
		lr.startColor = color;
		lr.endColor = color;
		lr.startWidth = 0.1f;
		lr.endWidth = 0.1f;
		lr.SetPosition (0, start);
		lr.SetPosition (1, end);
		yield return new WaitForSeconds(duration);
		GameObject.Destroy (myLine);
	}*/


	// Save the Game
	// ============================================================================
	public void Save(int saveFileIndex) {
		string[] saveData = new string[4096];
		string line;
		int i,j;
		int index = 0;
		Transform tr;
		List<GameObject> playerGameObjects = new List<GameObject>();
		List<GameObject> saveableGameObjects = new List<GameObject>();

		// Indicate we are saving
		sprint("Saving...");

		// Header
		// -----------------------------------------------------
		// Save Name
		saveData[index] = "TODO: SAVEGAME NAME ENTRY";
		index++;

		// temp string to hold global states TODO: actually pull in the global states to this string
		string states = "00000000|00000000|";

		// Global states and Difficulties
		saveData[index] = (states + difficultyCombat.ToString() + "|" + difficultyMission.ToString() + "|" + difficultyPuzzle.ToString() + "|" + difficultyCyber.ToString());
		index++;

		// Find all gameobjects with SaveObject script attached
		GameObject[] getAllGameObjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));
		foreach (GameObject gob in getAllGameObjects) {
			if (gob.GetComponentInChildren<PlayerMovement>() != null) {
				playerGameObjects.Add(gob);
			}

			if (gob.GetComponent<SaveObject>() != null) {
				saveableGameObjects.Add(gob);
			}
				
			// TODO: Do I need to save anything else in any other gameobject outside of Const?
		}

		// Save all the players' data
		for (i=0;i<playerGameObjects.Count;i++) {
			line = playerGameObjects[i].GetInstanceID().ToString();
			line += "|" + Const.a.playerName;
			line += "|" + playerGameObjects[i].GetComponentInChildren<PlayerHealth>().health.ToString("0000.00000");
			line += "|" + playerGameObjects[i].GetComponentInChildren<PlayerEnergy>().energy.ToString("0000.00000");
			line += "|" + playerGameObjects[i].GetComponentInChildren<PlayerMovement>().currentCrouchRatio.ToString("0000.00000");
			line += "|" + playerGameObjects[i].GetComponentInChildren<PlayerMovement>().bodyState.ToString();
			line += "|" + playerGameObjects[i].GetComponentInChildren<PlayerMovement>().ladderState.ToString();
			line += "|" + playerGameObjects[i].GetComponentInChildren<PlayerMovement>().gravliftState.ToString();
			line += "|" + playerGameObjects[i].GetComponentInChildren<PlayerPatchScript>().patchActive.ToString();
			line += "|" + playerGameObjects[i].GetComponentInChildren<PlayerPatchScript>().berserkIncrement.ToString();
			line += "|" + playerGameObjects[i].GetComponentInChildren<PlayerPatchScript>().sightFinishedTime.ToString("0000.00000");
			line += "|" + playerGameObjects[i].GetComponentInChildren<PlayerPatchScript>().staminupFinishedTime.ToString("0000.00000");
			tr = playerGameObjects[i].GetComponentInChildren<PlayerMovement>().gameObject.transform;
			line += "|" + (tr.localPosition.x.ToString("0000.00000") + "|" + tr.localPosition.y.ToString("0000.00000") + "|" + tr.localPosition.z.ToString("0000.00000"));
			line += "|" + (tr.localRotation.x.ToString("0000.00000") + "|" + tr.localRotation.y.ToString("0000.00000") + "|" + tr.localRotation.z.ToString("0000.00000") + "|" + tr.localRotation.w.ToString("0000.00000"));
			line += "|" + (tr.localScale.x.ToString("0000.00000") + "|" + tr.localScale.y.ToString("0000.00000") + "|" + tr.localScale.z.ToString("0000.00000"));
			line += "|" + playerGameObjects[i].GetComponentInChildren<MouseLookScript>().inventoryMode.ToString();
			line += "|" + playerGameObjects[i].GetComponentInChildren<MouseLookScript>().holdingObject.ToString();
			line += "|" + playerGameObjects[i].GetComponentInChildren<MouseLookScript>().heldObjectIndex.ToString();
			line += "|" + playerGameObjects[i].GetComponentInChildren<MouseLookScript>().heldObjectCustomIndex.ToString();
			line += "|" + playerGameObjects[i].GetComponentInChildren<MouseLookScript>().overButtonType.ToString();
			line += "|" + playerGameObjects[i].GetComponentInChildren<MouseLookScript>().overButton.ToString();
			line += "|" + playerGameObjects[i].GetComponentInChildren<MouseLookScript>().geniusActive.ToString();
			for (j=0;j<7;j++) {
				line += "|" + playerGameObjects[i].GetComponentInChildren<WeaponInventory>().weaponInventoryIndices[j].ToString();
			}
			for (j=0;j<7;j++) {
				line += "|" + playerGameObjects[i].GetComponentInChildren<WeaponInventory>().weaponInventoryAmmoIndices[j].ToString();
			}
			for (j=0;j<16;j++) {
				line += "|" + playerGameObjects[i].GetComponentInChildren<WeaponInventory>().hasWeapon[j].ToString();
			}
			for (j=0;j<16;j++) {
				line += "|" + playerGameObjects[i].GetComponentInChildren<WeaponAmmo>().wepAmmo[j].ToString();
			}
			line += "|" + playerGameObjects[i].GetComponentInChildren<WeaponCurrent>().weaponCurrent.ToString();
			line += "|" + playerGameObjects[i].GetComponentInChildren<WeaponCurrent>().weaponIndex.ToString();
			line += "|" + playerGameObjects[i].GetComponentInChildren<GrenadeCurrent>().grenadeCurrent.ToString();
			line += "|" + playerGameObjects[i].GetComponentInChildren<GrenadeCurrent>().grenadeIndex.ToString();
			for (j=0;j<7;j++) {
				line += "|" + playerGameObjects[i].GetComponentInChildren<GrenadeInventory>().grenAmmo[j].ToString();
			}
			line += "|" + playerGameObjects[i].GetComponentInChildren<PatchCurrent>().patchCurrent.ToString();
			line += "|" + playerGameObjects[i].GetComponentInChildren<PatchCurrent>().patchIndex.ToString();
			for (j=0;j<7;j++) {
				line += "|" + playerGameObjects[i].GetComponentInChildren<PatchInventory>().patchCounts[j].ToString();
			}
			for (j=0;j<128;j++) {
				line += "|" + playerGameObjects[i].GetComponentInChildren<LogInventory>().hasLog[j].ToString();
			}
			for (j=0;j<128;j++) {
				line += "|" + playerGameObjects[i].GetComponentInChildren<LogInventory>().readLog[j].ToString();
			}
			for (j=0;j<10;j++) {
				line += "|" + playerGameObjects[i].GetComponentInChildren<LogInventory>().numLogsFromLevel[j].ToString();
			}
			line += "|" + playerGameObjects[i].GetComponentInChildren<LogInventory>().lastAddedIndex.ToString();
			for (j=0;j<12;j++) {
				line += "|" + playerGameObjects[i].GetComponentInChildren<HardwareInventory>().hasHardware[j].ToString();
			}
			for (j=0;j<12;j++) {
				line += "|" + playerGameObjects[i].GetComponentInChildren<HardwareInventory>().hardwareVersion[j].ToString();
			}
			saveData[index] = line;
			index++;
		}

		// Save all the objects data
		for (i=0;i<saveableGameObjects.Count;i++) {
			line = saveableGameObjects[i].GetComponent<SaveObject>().SaveID.ToString();
			line += "|" + saveableGameObjects[i].activeInHierarchy.ToString();
			tr = saveableGameObjects[i].GetComponent<Transform>();
			line += "|" + (tr.localPosition.x.ToString("0000.00000") + "|" + tr.localPosition.y.ToString("0000.00000") + "|" + tr.localPosition.z.ToString("0000.00000"));
			line += "|" + (tr.localRotation.x.ToString("0000.00000") + "|" + tr.localRotation.y.ToString("0000.00000") + "|" + tr.localRotation.z.ToString("0000.00000") + "|" + tr.localRotation.w.ToString("0000.00000"));
			line += "|" + (tr.localScale.x.ToString("0000.00000") + "|" + tr.localScale.y.ToString("0000.00000") + "|" + tr.localScale.z.ToString("0000.00000"));
			saveData[index] = line;
			index++;
		}

		// Write to file
		StreamWriter sw = new StreamWriter(Application.streamingAssetsPath + "/sav"+saveFileIndex.ToString()+".txt");
		if (sw != null) {
			using (sw) {
				for (j=0;j<saveData.Length;j++) {
					sw.WriteLine(saveData[j]);
				}
				sw.Close();
			}
		}

		// Make "Done!" appear at the end of the line after "Saving..." is finished, stole this from Halo
		sprint("Saving...Done!");
	}

	public bool GetBoolFromString(string val) {
		if (val.ToLower() == "true") return true; else return false;
	}

	public int GetIntFromString(string val, int currentline) {
		bool parsed;
		int readInt;
		parsed = Int32.TryParse(val,out readInt);
		if (!parsed) {
			sprint("BUG: Could not parse int from save file on line " + currentline.ToString());
			return 0;
		}
		return readInt;
	}

	public float GetFloatFromString(string val, int currentline) {
		bool parsed;
		float readFloat;
		parsed = Single.TryParse(val,out readFloat);
		if (!parsed) {
			sprint("BUG: Could not parse float from save file on line " + currentline.ToString());
			return 0.0f;
		}
		return readFloat;
	}

	void LoadPlayerDataToPlayer(GameObject currentPlayer, string[] entries, int currentline) {
		int index = 1;
		int j;
		float readFloatx;
		float readFloaty;
		float readFloatz;
		float readFloatw;

		Const.a.playerName = entries[index]; index++;
		currentPlayer.GetComponentInChildren<PlayerHealth>().health = GetFloatFromString(entries[index],currentline); index++;
		currentPlayer.GetComponentInChildren<PlayerEnergy>().energy = GetFloatFromString(entries[index],currentline); index++;
		currentPlayer.GetComponentInChildren<PlayerMovement>().currentCrouchRatio = GetFloatFromString(entries[index],currentline); index++;
		currentPlayer.GetComponentInChildren<PlayerMovement>().bodyState = GetIntFromString(entries[index],currentline); index++;
		currentPlayer.GetComponentInChildren<PlayerMovement>().ladderState = GetBoolFromString(entries[index]); index++;
		currentPlayer.GetComponentInChildren<PlayerMovement>().gravliftState = GetBoolFromString(entries[index]); index++;
		currentPlayer.GetComponentInChildren<PlayerPatchScript>().patchActive = GetIntFromString(entries[index],currentline); index++;
		currentPlayer.GetComponentInChildren<PlayerPatchScript>().berserkIncrement = GetIntFromString(entries[index],currentline); index++;
		currentPlayer.GetComponentInChildren<PlayerPatchScript>().sightFinishedTime = GetFloatFromString(entries[index],currentline); index++;
		currentPlayer.GetComponentInChildren<PlayerPatchScript>().staminupFinishedTime = GetFloatFromString(entries[index],currentline); index++;
		readFloatx = GetFloatFromString(entries[index],currentline); index++;
		readFloaty = GetFloatFromString(entries[index],currentline); index++;
		readFloatz = GetFloatFromString(entries[index],currentline); index++;
		currentPlayer.GetComponentInChildren<PlayerMovement>().gameObject.transform.localPosition = new Vector3(readFloatx,readFloaty,readFloatz);
		readFloatx = GetFloatFromString(entries[index],currentline); index++;
		readFloaty = GetFloatFromString(entries[index],currentline); index++;
		readFloatz = GetFloatFromString(entries[index],currentline); index++;
		readFloatw = GetFloatFromString(entries[index],currentline); index++;
		currentPlayer.GetComponentInChildren<PlayerMovement>().gameObject.transform.localRotation = new Quaternion(readFloatx,readFloaty,readFloatz,readFloatw);
		readFloatx = GetFloatFromString(entries[index],currentline); index++;
		readFloaty = GetFloatFromString(entries[index],currentline); index++;
		readFloatz = GetFloatFromString(entries[index],currentline); index++;
		currentPlayer.GetComponentInChildren<PlayerMovement>().gameObject.transform.localScale = new Vector3(readFloatx,readFloaty,readFloatz);
		currentPlayer.GetComponentInChildren<MouseLookScript>().inventoryMode = GetBoolFromString(entries[index]); index++;
		currentPlayer.GetComponentInChildren<MouseLookScript>().holdingObject = GetBoolFromString(entries[index]); index++;
		currentPlayer.GetComponentInChildren<MouseLookScript>().heldObjectIndex = GetIntFromString(entries[index],currentline); index++;
		currentPlayer.GetComponentInChildren<MouseLookScript>().heldObjectCustomIndex = GetIntFromString(entries[index],currentline); index++;
		currentPlayer.GetComponentInChildren<MouseLookScript>().overButtonType = GetIntFromString(entries[index],currentline); index++;
		currentPlayer.GetComponentInChildren<MouseLookScript>().overButton = GetBoolFromString(entries[index]); index++;
		currentPlayer.GetComponentInChildren<MouseLookScript>().geniusActive = GetBoolFromString(entries[index]); index++;
		for (j=0;j<7;j++) {
			currentPlayer.GetComponentInChildren<WeaponInventory>().weaponInventoryIndices[j] = GetIntFromString(entries[index],currentline); index++;
		}
		for (j=0;j<7;j++) {
			currentPlayer.GetComponentInChildren<WeaponInventory>().weaponInventoryAmmoIndices[j] = GetIntFromString(entries[index],currentline); index++;
		}
		for (j=0;j<16;j++) {
			currentPlayer.GetComponentInChildren<WeaponInventory>().hasWeapon[j] = GetBoolFromString(entries[index]); index++;
		}
		for (j=0;j<16;j++) {
			currentPlayer.GetComponentInChildren<WeaponAmmo>().wepAmmo[j] = GetIntFromString(entries[index],currentline); index++;
		}
		currentPlayer.GetComponentInChildren<WeaponCurrent>().weaponCurrent = GetIntFromString(entries[index],currentline); index++;
		currentPlayer.GetComponentInChildren<WeaponCurrent>().weaponIndex = GetIntFromString(entries[index],currentline); index++;
		currentPlayer.GetComponentInChildren<GrenadeCurrent>().grenadeCurrent = GetIntFromString(entries[index],currentline); index++;
		currentPlayer.GetComponentInChildren<GrenadeCurrent>().grenadeIndex = GetIntFromString(entries[index],currentline); index++;
		for (j=0;j<7;j++) {
			currentPlayer.GetComponentInChildren<GrenadeInventory>().grenAmmo[j] = GetIntFromString(entries[index],currentline); index++;
		}
		currentPlayer.GetComponentInChildren<PatchCurrent>().patchCurrent = GetIntFromString(entries[index],currentline); index++;
		currentPlayer.GetComponentInChildren<PatchCurrent>().patchIndex = GetIntFromString(entries[index],currentline); index++;
		for (j=0;j<7;j++) {
			currentPlayer.GetComponentInChildren<PatchInventory>().patchCounts[j] = GetIntFromString(entries[index],currentline); index++;
		}
		for (j=0;j<128;j++) {
			currentPlayer.GetComponentInChildren<LogInventory>().hasLog[j] = GetBoolFromString(entries[index]); index++;
		}
		for (j=0;j<128;j++) {
			currentPlayer.GetComponentInChildren<LogInventory>().readLog[j] = GetBoolFromString(entries[index]); index++;
		}
		for (j=0;j<10;j++) {
			currentPlayer.GetComponentInChildren<LogInventory>().numLogsFromLevel[j] = GetIntFromString(entries[index],currentline); index++;
		}
		currentPlayer.GetComponentInChildren<LogInventory>().lastAddedIndex = GetIntFromString(entries[index],currentline); index++;
		for (j=0;j<12;j++) {
			currentPlayer.GetComponentInChildren<HardwareInventory>().hasHardware[j] = GetBoolFromString(entries[index]); index++;
		}
		for (j=0;j<12;j++) {
			currentPlayer.GetComponentInChildren<HardwareInventory>().hardwareVersion[j] = GetIntFromString(entries[index],currentline); index++;
		}
	}

	void LoadObjectDataToObject(GameObject currentGameObject, string[] entries, int currentline) {
		int index = 1;
		float readFloatx;
		float readFloaty;
		float readFloatz;
		float readFloatw;
		bool tempb = false;
		bool parsed = false;
		Vector3 tempvec;

		// Set active state of GameObject in Hierarchy
		if (entries[index].ToLower() == "true") tempb = true; else tempb = false; index++;
		currentGameObject.SetActive(tempb);

		// Get transform
		parsed = Single.TryParse(entries[2],out readFloatx);
		if (!parsed) sprint("BUG: Could not parse float from save file on line " + currentline.ToString());

		parsed = Single.TryParse(entries[3],out readFloaty);
		if (!parsed) sprint("BUG: Could not parse float from save file on line " + currentline.ToString());

		parsed = Single.TryParse(entries[4],out readFloatz);
		if (!parsed) sprint("BUG: Could not parse float from save file on line " + currentline.ToString());

		tempvec = new Vector3(readFloatx,readFloaty,readFloatz);
		currentGameObject.transform.localPosition = tempvec;

		readFloatx = 0;
		readFloaty = 0;
		readFloatz = 0;
		readFloatw = 0;
		tempvec = Vector3.zero;

		// Get rotation
		parsed = Single.TryParse(entries[5],out readFloatx);
		if (!parsed) sprint("BUG: Could not parse float from save file on line " + currentline.ToString());

		parsed = Single.TryParse(entries[6],out readFloaty);
		if (!parsed) sprint("BUG: Could not parse float from save file on line " + currentline.ToString());

		parsed = Single.TryParse(entries[7],out readFloatz);
		if (!parsed) sprint("BUG: Could not parse float from save file on line " + currentline.ToString());

		parsed = Single.TryParse(entries[8],out readFloatw);
		if (!parsed) sprint("BUG: Could not parse float from save file on line " + currentline.ToString());

		Quaternion tempquat = new Quaternion(readFloatx,readFloaty,readFloatz,readFloatw);
		currentGameObject.transform.localRotation = tempquat;

		readFloatx = 0;
		readFloaty = 0;
		readFloatz = 0;
		readFloatw = 0;
		tempvec = Vector3.zero;

		// Get scale
		parsed = Single.TryParse(entries[9],out readFloatx);
		if (!parsed) sprint("BUG: Could not parse float from save file on line " + currentline.ToString());

		parsed = Single.TryParse(entries[10],out readFloaty);
		if (!parsed) sprint("BUG: Could not parse float from save file on line " + currentline.ToString());

		parsed = Single.TryParse(entries[11],out readFloatz);
		if (!parsed) sprint("BUG: Could not parse float from save file on line " + currentline.ToString());

		tempvec = new Vector3(readFloatx,readFloaty,readFloatz);
		currentGameObject.transform.localScale = tempvec;

		readFloatx = 0;
		readFloaty = 0;
		readFloatz = 0;
		readFloatw = 0;
		tempvec = Vector3.zero;
	}

	public void Load(int saveFileIndex) {
		sprint("Loading...");
		List<GameObject> playerGameObjects = new List<GameObject>();
		List<GameObject> saveableGameObjects = new List<GameObject>();
		// Find all gameobjects with SaveObject script attached
		GameObject[] getAllGameObjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));
		foreach (GameObject gob in getAllGameObjects) {
			if (gob.GetComponentInChildren<PlayerMovement>() != null) {
				playerGameObjects.Add(gob);
			}

			if (gob.GetComponent<SaveObject>() != null) {
				saveableGameObjects.Add(gob);
			}
		}

		string readline;

		int currentline = 0;

		StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/sav"+saveFileIndex.ToString()+".txt");
		if (sr != null) {
			using (sr) {
				do {
					readline = sr.ReadLine();
					if (readline == null) continue; // skip blank lines
					string[] entries = readline.Split('|');  // delimited by | character, aka the vertical bar, pipe, obelisk, etc.
					if (entries.Length <= 1) continue; // Skip save name

					foreach (GameObject pl in playerGameObjects) {
						if (entries[0] == playerGameObjects[0].GetInstanceID().ToString()) {
							LoadPlayerDataToPlayer(pl,entries,currentline);
							break;
						}
					}

					foreach (GameObject ob in saveableGameObjects) {
						if (entries[0] == ob.GetComponent<SaveObject>().SaveID.ToString()) {
							LoadObjectDataToObject(ob,entries,currentline);
							break;
						}
					}
					currentline++;
				} while (!sr.EndOfStream);
				sr.Close();
			}
		}
		sprint("Loading...Done!");
	}

	public void SetFOV() {
		playerCamera.GetComponent<Camera>().fieldOfView = GraphicsFOV;
	}

	public void SetBloom() {
		playerCamera.GetComponent<Bloom>().enabled = GraphicsBloom;
	}

	public void SetSSAO() {
		playerCamera.GetComponent<ScreenSpaceAmbientObscurance>().enabled = GraphicsSSAO;
	}

	public void SetBrightness() {
		float tempf = Const.a.GraphicsGamma;
		if (tempf < 1) tempf = 0;
		else tempf = tempf/100;
		playerCamera.GetComponent<ColorCurvesManager>().Factor = tempf;
	}

	public void SetVolume() {
		AudioListener.volume = (AudioVolumeMaster/100f);
		mainmenuMusic.volume = (AudioVolumeMusic/100f);
	}

	private int AssignConfigInt(string section, string keyname) {
		int inputInt = -1;
		string inputCapture = "";
		inputCapture = INIWorker.IniReadValue(section,keyname);
		if (inputCapture == null) inputCapture = "NULL";
		bool parsed = Int32.TryParse(inputCapture, out inputInt);
		if (parsed) return inputInt; else sprint("Warning: Could not parse config key " + keyname + " as integer: " + inputCapture);
		return 0;
	}

	private bool AssignConfigBool(string section, string keyname) {
		int inputInt = -1;
		string inputCapture = "";
		inputCapture = INIWorker.IniReadValue(section,keyname);
		if (inputCapture == null) inputCapture = "NULL";
		bool parsed = Int32.TryParse(inputCapture, out inputInt);
		if (parsed) {
			if (inputInt > 0) return true; else return false;
		} else sprint("Warning: Could not parse config key " + keyname + " as bool: " + inputCapture);
		return false;
	}

	public string GetBoolAsString(bool inputValue) {
		if (inputValue) return "1";
		return "0";
	}
}
