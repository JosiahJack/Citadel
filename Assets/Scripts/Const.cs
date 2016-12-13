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
	public float doubleClickTime = 0.500f;
	public float frobDistance = 5f;
	public enum PoolType{DartImpacts};
	public enum AttackType{None,Melee,EnergyBeam,Magnetic,Projectile,ProjectileEnergyBeam};
	public GameObject Pool_DartImpacts;
	public GameObject statusBar;
    public static Const a;
	public TextAsset logTextFile;

	// Instantiate it so that it can be accessed globally. MOST IMPORTANT PART!!
	// =========================================================================
	void Awake() { a = this; }
	// =========================================================================
	void Start() {
		LoadAudioLogMetaData();
		LoadItemNamesData();
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

		StreamReader dataReader = new StreamReader(Application.dataPath + "/Scripts/logs_text.txt",Encoding.Default);
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

		StreamReader dataReader = new StreamReader(Application.dataPath + "/Scripts/item_names.txt",Encoding.Default);
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

	// StatusBar Print
	public static void sprint (string input) {
		print(input);  // print to console
		if (a != null) {
			if (a.statusBar != null)
				a.statusBar.GetComponent<StatusBarTextDecay>().SendText(input);
		}
	}

	public GameObject GetObjectFromPool(PoolType pool) {
		if (Pool_DartImpacts == null) {
			sprint("Cannot find pool of type PoolType.DartImpacts");
			return null;
		}

		switch (pool) {
		case PoolType.DartImpacts: 
			for (int i=0;i<Pool_DartImpacts.transform.childCount;i++) {
				Transform child = Pool_DartImpacts.transform.GetChild(i);
				if (child.gameObject.activeInHierarchy == false) {
					//sprint("Found a DartImpact!");
					return child.gameObject;
				}
			}
			sprint("Warning: No free objects in DartImpacts pool");
			return null;
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
