using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuncWall : MonoBehaviour {
	// Externally set in inspector per instance
	public float speed = 0.64f;
	public GameObject targetPosition;
	public FuncStates startState;
	public float percentAjar = 0;
	public AudioClip SFXMoving;
	public AudioClip SFXStop;
	public AudioSource SFXSource;
	public FuncStates currentState; // save

	public int[] chunkIDs;
	[HideInInspector] public Vector3 startPosition; // save
	private Vector3 goalPosition;
	private Vector3 tempVec;
	private Rigidbody rbody;
	private bool stopSoundPlayed;
	private float dist;
	private float distanceLeft;
	private float startTime;

	void Awake () {
		currentState = startState; // set door position to picked state
		startPosition = transform.position;
		if (currentState == FuncStates.AjarMovingStart
			|| currentState == FuncStates.AjarMovingTarget) {
			tempVec = (transform.position - targetPosition.transform.position);
			distanceLeft = Vector3.Distance(transform.position,
											targetPosition.transform.position);
			tempVec = (tempVec.normalized * (distanceLeft * percentAjar * -1));
			tempVec += transform.position;
			transform.position = tempVec;
		}
		rbody = GetComponent<Rigidbody>();
		rbody.isKinematic = true;
		rbody.useGravity = false;
		rbody.collisionDetectionMode =
		  CollisionDetectionMode.ContinuousSpeculative;

		if (SFXSource == null) SFXSource = GetComponent<AudioSource>();
		stopSoundPlayed = false;
		dist = distanceLeft = 0;
		startTime = PauseScript.a.relativeTime;
	}
		
	public void Targetted (UseData ud) {
		switch (currentState) {
			case FuncStates.Start:
				MoveTarget();
				break;
			case FuncStates.Target:
				MoveStart();
				break;
			case FuncStates.MovingStart:
				MoveTarget ();
				break;
			case FuncStates.MovingTarget:
				MoveStart ();
				break;
			case FuncStates.AjarMovingStart:
				MoveStart();
				break;
			case FuncStates.AjarMovingTarget:
				MoveTarget();
				break;
		}
		if (SFXSource != null) SFXSource.clip = SFXMoving;
		if (SFXSource != null) SFXSource.loop = true;
		if (SFXSource != null) SFXSource.Play();
		stopSoundPlayed = false;
	}

	void MoveStart() {
		currentState = FuncStates.MovingStart;
		startTime = PauseScript.a.relativeTime + 10f;
	}

	void MoveTarget() {
		currentState = FuncStates.MovingTarget;
		startTime = PauseScript.a.relativeTime + 10f;
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeSelf) {
			switch (currentState) {
				case FuncStates.Start:
					transform.position = startPosition;
					if (rbody.velocity.sqrMagnitude > 0) {
						rbody.velocity = Const.a.vectorZero;
					}
					break;
				case FuncStates.Target:
					transform.position = targetPosition.transform.position;
					if (rbody.velocity.sqrMagnitude > 0) {
						rbody.velocity = Const.a.vectorZero;
					}
					break;
				case FuncStates.MovingStart:
					goalPosition = startPosition;
					rbody.WakeUp();
					dist = speed * Time.deltaTime;
					tempVec = (transform.position - goalPosition).normalized;
					tempVec = (tempVec * dist * -1) + transform.position;
					rbody.MovePosition(tempVec);
					distanceLeft = Vector3.Distance(transform.position,
													goalPosition);
					if (distanceLeft <= 0.04f
						|| startTime < PauseScript.a.relativeTime) {
						currentState = FuncStates.Start;
						if (SFXSource != null) {
							SFXSource.Stop ();
							SFXSource.loop = false;
							if (!stopSoundPlayed) {
								Utils.PlayOneShotSavable(SFXSource,SFXStop);
								stopSoundPlayed = true;
							}
						}
					}
					break;
				case FuncStates.MovingTarget:
					goalPosition = targetPosition.transform.position;
					rbody.WakeUp();
					dist = speed * Time.deltaTime;
					tempVec = (transform.position - goalPosition).normalized;
					tempVec = (tempVec * dist * -1) + transform.position;
					rbody.MovePosition(tempVec);
					distanceLeft = Vector3.Distance(transform.position,
													goalPosition);
					if (distanceLeft <= 0.04f
						|| startTime < PauseScript.a.relativeTime) {
						currentState = FuncStates.Target;
						if (SFXSource != null) {
							SFXSource.Stop ();
							SFXSource.loop = false;
							if (!stopSoundPlayed) {
								Utils.PlayOneShotSavable(SFXSource,SFXStop);
								stopSoundPlayed = true;
							}
						}
					}
					break;
			}
		}
	}

	// Only need to save state here.  Transform position is saved higher up in
	// the saving hierarchy and that is the only other thing needed for these.
	public static string Save(GameObject go) {
		FuncWall fw = go.GetComponent<FuncWall>();
		if (fw == null) {
			Debug.Log("FuncWall missing on savetype of FuncWall!  "
					  + "GameObject.name: " + go.name);
			return Utils.DTypeWordToSaveString("uffffffffff");
		}

		string line = System.String.Empty;
		switch (fw.currentState) {
			case FuncStates.Start: line = "0"; break;
			case FuncStates.Target: line = "1"; break;
			case FuncStates.MovingStart: line = "2"; break;
			case FuncStates.MovingTarget: line = "3"; break;
			case FuncStates.AjarMovingStart: line = "4"; break;
			case FuncStates.AjarMovingTarget: line = "5"; break;
		}
		line += Utils.splitChar + Utils.Vector3ToString(fw.startPosition);

		// The mover_target transform and position was saved by SaveObject.Save
		// prior to that function calling this function, so only save the
		// parent transform here.
		line += Utils.splitChar + Utils.SaveTransform(go.transform.parent.transform);
		line += Utils.splitChar + Utils.UintToString(fw.chunkIDs.Length);
		for (int i=0;i<go.transform.childCount; i++) {
			line += Utils.splitChar + Utils.UintToString(fw.chunkIDs[i]);
			line += Utils.splitChar + Utils.SaveChildGOState(go,i);
		}
		return line;
	}

	// Load data to a freshly instantiated blank func_wall prefab.
	// Prefab hierarchy is assumed to be in this structure:
	// func_wall             This is the main parent which is merely a container
	// ->mover_target        This is the GameObject with FuncWall as a component.
	// ->->chunk_somechunk   These are the walls or floors that comprise the
	// ->->chunk_somechunk2    visible and physical collisions.  These will not
	// ->->chunk_somechunk3    exist yet on a freshly instantiated prefab.
	// ->->etc. etc.
	// ->info_target         This is a relative offset position creator
	public static int Load(GameObject go, ref string[] entries, int index) {
		float readFloatx, readFloaty, readFloatz;
		FuncWall fw = go.GetComponent<FuncWall>(); // Fairweather we are having.
												   // Vague Quake mapper reference
		if (fw == null || index < 0 || entries == null) return index + 1;

		int state = Utils.GetIntFromString(entries[index]); index++;
		switch (state) {
			case 0: fw.currentState = FuncStates.Start; break;
			case 1: fw.currentState = FuncStates.Target; break;
			case 2: fw.currentState = FuncStates.MovingStart; break;
			case 3: fw.currentState = FuncStates.MovingTarget; break;
			case 4: fw.currentState = FuncStates.AjarMovingStart; break;
			case 5: fw.currentState = FuncStates.AjarMovingTarget; break;
		}
		readFloatx = Utils.GetFloatFromString(entries[index]); index++; // float
		readFloaty = Utils.GetFloatFromString(entries[index]); index++; // float
		readFloatz = Utils.GetFloatFromString(entries[index]); index++; // float
		fw.startPosition = new Vector3(readFloatx,readFloaty,readFloatz);

		index = Utils.LoadTransform(go.transform.parent.transform,ref entries,
									index);
		int numChildren = Utils.GetIntFromString(entries[index]); index++;
		int chunkdex = 0;
		for (int i=0; i<numChildren; i++) {
			// Get the index of the chunk prefab
			chunkdex = Utils.GetIntFromString(entries[index]); index++;

			// Assumption here is that we are loading to a freshly instantiated
			// func_wall prefab and that there are no children chunks on the
			// mover_target GameObject yet.
			GameObject childGO = Instantiate(Const.a.chunkPrefabs[chunkdex],
						go.transform.localPosition, // 0's, transform loaded below
						Const.a.quaternionIdentity) as GameObject;
			childGO.transform.SetParent(go.transform); // Always set parent prior
													   // to loading transform.
			index = Utils.LoadChildGOState(childGO,ref entries,index);
		}
		return index;
	}
}
