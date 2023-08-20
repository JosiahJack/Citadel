using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
	public int index; // Global constindex
	public int numberToSpawn = 1;
	public int numberActive; // save
	public bool active = false; // save
	public Transform[] spawnLocations;
	private GameObject dynamicObjectsContainer;
	public bool NPCSpawner = true;
	public float minDelayBetweenSpawns = 15f; // default values for generic level spawners for repopulating hallways
	public float maxDelayBetweenSpawns = 45f;
	public float allSpawnedResetDelay = 120f;
	[HideInInspector]
	public float delayFinished; // save
	public bool alertEnemiesOnAwake;
	public bool countOnlySameIndex = false; // Not one of us.

	void Start() {
		delayFinished = PauseScript.a.relativeTime;
		if (Const.a.difficultyCombat == 1) {
			numberToSpawn = (int) Mathf.Floor(numberToSpawn*0.5f);
			if (numberToSpawn < 1) numberToSpawn = 1;
		}

		if (Const.a.difficultyCombat == 3) {
			numberToSpawn = (int) Mathf.Floor(numberToSpawn*1.5f);
			if (numberToSpawn < 1) numberToSpawn = 1;
		}

		if (Const.a.difficultyCombat > 3) {
			numberToSpawn = (int) Mathf.Floor(numberToSpawn*5f); // Hehe :)
		}
	}

	public void Activate(bool alertEnemies) {
		alertEnemiesOnAwake = alertEnemies;
		active = true;
		delayFinished = PauseScript.a.relativeTime;
	}

	void Update() {
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;
		if (!active) return;

		if (LevelManager.a.npcsm[LevelManager.a.currentLevel] == null) return;

		NPCSubManager subM = LevelManager.a.npcsm[LevelManager.a.currentLevel];
		int numNPCs = subM.childrenNPCsAICs.Length;		
		if (numNPCs > 300) return;

		int count = 0;
		for (int i=0;i<numNPCs;i++) {
			AIController aic = subM.childrenNPCsAICs[i];
			if (aic == null) {
				Debug.Log("subM.childrenNPCsAICs null!");
				continue;
			}

            if (!aic.gameObject.activeInHierarchy) continue;
            if (aic.healthManager.health <= 0) continue;
            if (aic.index != (index - 419) && countOnlySameIndex) continue;
            
			count++;
		}
		if (numberActive != count) numberActive = count;

		if (numberActive >= numberToSpawn) return;
		if (delayFinished >= PauseScript.a.relativeTime) return; // Not yet.

		delayFinished = PauseScript.a.relativeTime
						+ Random.Range(minDelayBetweenSpawns,
									   maxDelayBetweenSpawns);

		Spawn(index); // spawn then wait randomized amount of time
		count++;
		if (count >= numberToSpawn) {
			delayFinished = PauseScript.a.relativeTime + allSpawnedResetDelay;
		}
	}

	void Spawn(int index) {
		if (Const.a.difficultyCombat == 0) return; // Not on combat diff 0
		Debug.Log("Spawning new enemy...");
		dynamicObjectsContainer = LevelManager.a.GetCurrentDynamicContainer();
        if (dynamicObjectsContainer == null) return; // Didn't find current 
													 // level, can't spawn.

		Debug.Log("Found dynamic object container for spawning new enemy");
		Vector3 spot = GetRandomLocation();
		if (spot.x == 0 && spot.y == 0 && spot.z == 0) return;

		GameObject instGO = ConsoleEmulator.SpawnDynamicObject(
			index,LevelManager.a.currentLevel,false,null,-1
		);

		if (instGO == null) {
			Debug.Log("BUG: Could not spawn NPC index " + index.ToString());
		} else {
			instGO.transform.position = spot;
			AIController aic = instGO.GetComponent<AIController>();
			if (aic == null) return;

			if (!alertEnemiesOnAwake) {
				if (aic.index != 14) aic.wandering = true;
				return;
			}

			aic.SetEnemy(Const.a.player1Capsule,Const.a.player1TargettingPos);
		}
	}

	Vector3 GetRandomLocation() {
		int randpos;
		randpos = Random.Range(0,(spawnLocations.Length-1));
		Vector3 retval = spawnLocations[randpos].position;
		if (!AreaClear(retval)) return new Vector3(0,0,0);
		if (!AreaHidden(retval)) return new Vector3(0,0,0);
		return retval;
	}

	// CapsuleCast using largest NPC's bounding capsule to check area is clear.
	bool AreaClear(Vector3 spot) {
		RaycastHit hit = new RaycastHit();
		if (Physics.CapsuleCast(spot + new Vector3(0,0.52f,0),
								spot + new Vector3(0,-0.52f,0),0.48f,
								Const.a.vectorZero,out hit,0.02f,
								Const.a.layerMaskNPCCollision)) {
			return false;
		} else {
			return true;
		}
	}

	bool AreaHidden(Vector3 spot) {
		Vector3 plyPos = Const.a.player1Capsule.transform.position;
		float range = 50f;
		if (Vector3.Distance(plyPos,spot) > range) return true;

		int mask = Const.a.layerMaskNPCAttack;
		Vector3 ray = (plyPos - spot).normalized;
		RaycastHit tempHit;
		if (Physics.Raycast(spot,ray,out tempHit,range,mask)) {
			if (tempHit.collider.CompareTag("Player")) return false;
		}
		return true;
	}

	public static string Save(GameObject go) {
		SpawnManager sm = go.GetComponent<SpawnManager>();
		if (sm == null) {
			Debug.Log("SpawnManager missing on savetype of SpawnManager!  "
					  + "GameObject.name: " + go.name);

			return "0|0|0000.00000";
		}

		string line = System.String.Empty;
		line = Utils.BoolToString(sm.active,"SpawnManager.active");
		line += Utils.splitChar;
		line += sm.numberActive.ToString();
		line += Utils.splitChar;
		line += Utils.SaveRelativeTimeDifferential(sm.delayFinished);
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		SpawnManager sm = go.GetComponent<SpawnManager>();
		// Spawn martin.  Ok dang, this is such a vague reference it makes
		// vague references look specific.  See source code for a Quake mod
		// called vigil (the source code, not referenced as Martin in the mod
		// though that's what your friend's name is, requires decompiling to
		// see that it's called spawnmartin() as it isn't available separately,
		// goodness gracious!)
		if (sm == null) {
			Debug.Log("SpawnManager.Load failure, sm == null");
			return index + 3;
		}

		if (index < 0) {
			Debug.Log("SpawnManager.Load failure, index < 0");
			return index + 3;
		}

		if (entries == null) {
			Debug.Log("SpawnManager.Load failure, entries == null");
			return index + 3;
		}

		sm.active = Utils.GetBoolFromString(entries[index],
											"SpawnManager.active");
		index++;

		sm.numberActive = Utils.GetIntFromString(entries[index]);
		index++;

		sm.delayFinished = Utils.LoadRelativeTimeDifferential(entries[index]);
		index++;
		return index;
	}
}
