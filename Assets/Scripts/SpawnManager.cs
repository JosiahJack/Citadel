using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
	public int index;
	public int numberToSpawn = 1;
	public int numberActive; // save
	public bool active = false; // save
	public Transform[] spawnLocations;
	private GameObject dynamicObjectsContainer;
	public bool NPCSpawner = true;
	private bool alertEnemiesOnAwake = false;
	public bool spawnAllAtOnce = false;
	public float minDelayBetweenSpawns = 15f; // default values for generic level spawners for repopulating hallways
	public float maxDelayBetweenSpawns = 45f;
	[HideInInspector]
	public float delayFinished; // save

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
	}

	public void Activate(bool alertEnemies) {
		alertEnemiesOnAwake = alertEnemies;
		active = true;
		if (!spawnAllAtOnce) delayFinished = PauseScript.a.relativeTime + Random.Range(minDelayBetweenSpawns,maxDelayBetweenSpawns);
	}

	void Update() {
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;
		if (!active) return;

		if (LevelManager.a.npcsm[LevelManager.a.currentLevel] == null) return;

		int numNPCs = LevelManager.a.npcsm[LevelManager.a.currentLevel].childrenNPCsAICs.Length;
		if (numNPCs <= 0) return;
		
		numberActive = 0;
		for (int i=0;i<numNPCs;i++) {
			AIController aic = LevelManager.a.npcsm[LevelManager.a.currentLevel].childrenNPCsAICs[i];
            if (!aic.gameObject.activeInHierarchy) continue;
            if (aic.healthManager.health <= 0) continue;
            if (aic.index != index) return; // Not one of us.
            
			numberActive++;
		}

		if (numberActive >= numberToSpawn) return;

		if (spawnAllAtOnce) {
			Spawn(index); //spawn don't wait
			return;
		}

		if (delayFinished >= PauseScript.a.relativeTime) return; // Not yet.

		delayFinished = PauseScript.a.relativeTime
						+ Random.Range(minDelayBetweenSpawns,
									   maxDelayBetweenSpawns);

		Spawn(index); // spawn then wait randomized amount of time
	}

	void Spawn(int index) {
		if (Const.a.difficultyCombat == 0) return; // no spawns on combat difficulty 0
		Debug.Log("Spawning new enemy...");
		dynamicObjectsContainer = LevelManager.a.GetCurrentDynamicContainer();
        if (dynamicObjectsContainer == null) return; //didn't find current level, can't spawn
		Debug.Log("Found dynamic object container for spawning new enemy");
		GameObject instGO = ConsoleEmulator.SpawnDynamicObject(index,LevelManager.a.currentLevel,
													false,null,-1);

		if (instGO == null) Debug.Log("BUG: Could not spawn NPC index "
									  + index.ToString());
	}

	public void SpawneeJustDied() {
		numberActive--;
		if (numberActive < 0) numberActive = 0;
	}

	public void AwakeFromLoad(float health) {
		if (health > 0) {
			numberActive++;
		} else {
			numberActive--;
		}
		if (numberActive < 0) numberActive = 0;
	}

	Vector3 GetRandomLocation() {
		int randpos;
		for (int i=0;i<spawnLocations.Length;i++) {
			randpos = Random.Range(0,(spawnLocations.Length-1));
			if (AreaClear(spawnLocations[i].position)) return spawnLocations[i].position;
		}
		return new Vector3(0,0,0);
	}

	// CapsuleCast using largest NPC's bounding capsule to check if area is clear.
	bool AreaClear(Vector3 spot) {
		RaycastHit hit = new RaycastHit();
		if (Physics.CapsuleCast(spot + new Vector3(0,0.52f,0),spot + new Vector3(0,-0.52f,0),0.48f,Const.a.vectorZero,out hit,0.02f,Const.a.layerMaskNPCCollision)) {
			return false;
		} else {
			return true;
		}
	}

	public static string Save(GameObject go) {
		SpawnManager sm = go.GetComponent<SpawnManager>();
		if (sm == null) {
			Debug.Log("SpawnManager missing on savetype of SpawnManager!  GameObject.name: " + go.name);
			return "0|0|0000.00000";
		}

		string line = System.String.Empty;
		line = Utils.BoolToString(sm.active); // bool - is this enabled
		line += Utils.splitChar + sm.numberActive.ToString(); // int - number spawned
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(sm.delayFinished); // float - time that we need to spawn next
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		SpawnManager sm = go.GetComponent<SpawnManager>(); // Spawn martin.  Ok dang, this is such a vague reference it makes vague references look specific.  See source code for a Quake mod called vigil (the source code, not referenced as Martin in the mod though that's what your friend's name is, requires decompiling to see that it's called spawnmartin() as it isn't available separately, goodness gracious!)
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

		sm.active = Utils.GetBoolFromString(entries[index]); index++; // bool - is this enabled
		sm.numberActive = Utils.GetIntFromString(entries[index]); index++; // int - number spawned
		sm.delayFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float - time that we need to spawn next
		return index;
	}
}
