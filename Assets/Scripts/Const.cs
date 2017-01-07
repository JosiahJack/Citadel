using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
using System.Collections;
using System;

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

	// Instantiate it so that it can be accessed globally. MOST IMPORTANT PART!!
	// =========================================================================
	void Awake() {
		Application.targetFrameRate = 60;
		a = this;
	}
	// =========================================================================
	void Start() {
		LoadAudioLogMetaData();
		LoadItemNamesData();
		LoadDamageTablesData();
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
}
