using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
	public int index;
	public int numberToSpawn = 1;
	public GameObject[] activeSpawned;
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
		if (activeSpawned.Length > 0) {
			for (int i=0;i<activeSpawned.Length;i++) {
				numberActive++;
			}
		}
		delayFinished = PauseScript.a.relativeTime;
	}

	public void Activate(bool alertEnemies) {
		alertEnemiesOnAwake = alertEnemies;
		active = true;
		if (!spawnAllAtOnce) delayFinished = PauseScript.a.relativeTime + Random.Range(minDelayBetweenSpawns,maxDelayBetweenSpawns);
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			if (active) {
				if (numberActive < numberToSpawn) {
					if (spawnAllAtOnce) {
						Spawn(index); //spawn don't wait
					} else {
						if (delayFinished < PauseScript.a.relativeTime) {
							delayFinished = PauseScript.a.relativeTime + Random.Range(minDelayBetweenSpawns,maxDelayBetweenSpawns);
							Spawn(index); // spawn then wait randomized amount of time
						}
					}
				}
			}
		}
	}

	void Spawn(int index) {
		Debug.Log("Spawning new enemy...");
		dynamicObjectsContainer = LevelManager.a.GetCurrentLevelDynamicContainer();
        if (dynamicObjectsContainer == null) return; //didn't find current level, can't spawn
		Debug.Log("Found dynamic object container for spawning new enemy");
		if (NPCSpawner) {
			GameObject spawnee = (GameObject) Instantiate(Const.a.npcPrefabs[index], new Vector3(0,0,0),  Quaternion.identity);
			if (spawnee !=null) {
				spawnee.GetComponent<HealthManager>().spawnMother = this;
				spawnee.transform.position = GetRandomLocation();
				if (spawnee.transform.position.x == 0 && spawnee.transform.position.y == 0  && spawnee.transform.position.z == 0 ) Debug.Log("BUG: Spawned enemy at 0 0 0!");
				if (alertEnemiesOnAwake) {
					AIController aic = spawnee.GetComponent<AIController>();
					if (aic != null) aic.enemy = Const.a.player1;
				}
				numberActive++;
				Debug.Log("Number spawned enemies: " + numberActive.ToString());
				SaveObject so = spawnee.GetComponent<SaveObject>();
				if (so != null) {
					so.levelParentID = LevelManager.a.currentLevel;
					so.Start();
					Debug.Log("Spawned enemy SaveObject setup");
				}
			}
		}

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
		//if (numberActive > numberToSpawn) numberActive = numberToSpawn;
	}

	Vector3 GetRandomLocation() {
		int randpos;
		for (int i=0;i<spawnLocations.Length;i++) {
			randpos = Random.Range(0,(spawnLocations.Length-1));
			if (AreaClear(spawnLocations[i].position)) return spawnLocations[i].position;
		}
		return new Vector3(0,0,0);
	}

	bool AreaClear(Vector3 spot) {
		int layMask = LayerMask.GetMask("Default","Water","Geometry","NPC","Corpse","Door","InterDebris","Player");
		RaycastHit hit = new RaycastHit();
		if (Physics.CapsuleCast(spot + new Vector3(0,0.52f,0),spot + new Vector3(0,-0.52f,0),0.48f,Vector3.zero,out hit,0.02f,layMask)) {
			return false;
		} else {
			return true;
		}
	}
}
