using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuncWall : MonoBehaviour {
	// Externally set in inspector per instance
	public float speed = 0.64f; // save
	public GameObject targetPosition;
	public FuncStates startState; // save
	public float percentAjar = 0; // save
	public AudioClip SFXMoving;
	public AudioClip SFXStop;
	public AudioSource SFXSource;
	public FuncStates currentState; // save
	public int[] chunkIDs; // save

	[HideInInspector] public float startTime; // save
	[HideInInspector] public Vector3 startPosition; // save
	[HideInInspector] public Rigidbody rbody;
	[HideInInspector] public bool stopSoundPlayed; // save
	private Vector3 tempVec;
	private float dist;         // Only ever used right away, not saved.
	private float distanceLeft; // Only ever used right away, not saved.

	public void Awake () {
		currentState = startState; // set door position to picked state
		startPosition = transform.position;
		rbody = GetComponent<Rigidbody>();
		if (SFXSource == null) SFXSource = GetComponent<AudioSource>();
		Initialize();
		dist = distanceLeft = 0;
		startTime = PauseScript.a.relativeTime;
	}

	public void Initialize() {
		stopSoundPlayed = false;
		if (currentState == FuncStates.AjarMovingStart
			|| currentState == FuncStates.AjarMovingTarget) {
			tempVec = (transform.position - targetPosition.transform.position);
			distanceLeft = Vector3.Distance(transform.position,
											targetPosition.transform.position);
			tempVec = (tempVec.normalized * (distanceLeft * percentAjar * -1));
			tempVec += transform.position;
			transform.position = tempVec;
		}
		rbody.isKinematic = true;
		rbody.useGravity = false;
		rbody.collisionDetectionMode =
		  CollisionDetectionMode.ContinuousSpeculative;

		// Ensure children are non-static.  Necessary to move them with parent!
		GameObject childGO;
		for (int i = 0; i < transform.childCount; i++) {
			childGO = transform.GetChild(i).gameObject;
			childGO.isStatic = false;
			childGO.layer = 18; // Door
		}
	}

	public void Targetted (UseData ud) {
		UnityEngine.Debug.Log("FuncWall was targetted");
		switch (currentState) {
			case FuncStates.Start:            MoveTarget(); break;
			case FuncStates.Target:           MoveStart();  break;
			case FuncStates.MovingStart:      MoveTarget(); break;
			case FuncStates.MovingTarget:     MoveStart();  break;
			case FuncStates.AjarMovingStart:  MoveStart();  break;
			case FuncStates.AjarMovingTarget: MoveTarget(); break;
		}

		if (SFXSource != null) {
			SFXSource.clip = SFXMoving;
			SFXSource.loop = true;
			SFXSource.Play();
		}

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

	void MoveToPosition(Vector3 goalPosition, FuncStates newState) {
		rbody.WakeUp();
		dist = speed * Time.deltaTime;
		tempVec = (transform.position - goalPosition).normalized; // Relative
		tempVec = (tempVec * dist * -1) + transform.position; // Absolute
		rbody.MovePosition(tempVec);
		distanceLeft = Vector3.Distance(transform.position, goalPosition);
		if (distanceLeft <= 0.04f || startTime < PauseScript.a.relativeTime) {
			currentState = newState;
			if (SFXSource != null) {
				SFXSource.Stop ();
				SFXSource.loop = false;
				if (!stopSoundPlayed) {
					Utils.PlayOneShotSavable(SFXSource,SFXStop);
					stopSoundPlayed = true;
				}
			}
		}
	}

	void FixedUpdate() {
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.mainMenu.activeSelf) return;

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
				MoveToPosition(startPosition, FuncStates.Start);
				break;
			case FuncStates.MovingTarget:
				MoveToPosition(targetPosition.transform.position,
								FuncStates.Target);
				break;
		}
	}

	// Only need to save state here.  Transform position is saved higher up in
	// the saving hierarchy and that is the only other thing needed for these.
	public static string Save(GameObject go) {
		FuncWall fw = go.GetComponent<FuncWall>();
		if (fw == null) {
			Debug.Log("FuncWall missing on savetype of FuncWall!  "
					  + "GameObject.name: " + go.name);
			return Utils.DTypeWordToSaveString("uufffffffffffffff");
		}

		string line = System.String.Empty;
		line = Utils.IntToString(Utils.FuncStatesToInt(fw.currentState));
		line += Utils.splitChar + Utils.IntToString(Utils.FuncStatesToInt(fw.startState));
		line += Utils.splitChar + Utils.Vector3ToString(fw.startPosition);
		line += Utils.splitChar + Utils.FloatToString(fw.speed);
		line += Utils.splitChar + Utils.FloatToString(fw.percentAjar);
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(fw.startTime);

		// The mover_target transform and position was saved by SaveObject.Save
		// prior to that function calling this function, so only save the
		// parent transform here.
		line += Utils.splitChar + Utils.SaveTransform(go.transform.parent.transform);
		line += Utils.splitChar + Utils.UintToString(fw.chunkIDs.Length);
		for (int i=0;i<go.transform.childCount; i++) {
			line += Utils.splitChar + Utils.UintToString(fw.chunkIDs[i]);
			line += Utils.splitChar + Utils.SaveChildGOState(go,i);
		}
		Transform info_target = go.transform.parent.transform.GetChild(1);
		line += Utils.splitChar + Utils.SaveTransform(info_target);
		line += Utils.splitChar + Utils.SaveAudioSource(go);
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
		if (fw == null || index < 0 || entries == null) return index + 17;

		int state = Utils.GetIntFromString(entries[index]); index++;
		fw.currentState = Utils.GetFuncStatesFromInt(state);
		state = Utils.GetIntFromString(entries[index]); index++;
		fw.startState = Utils.GetFuncStatesFromInt(state);
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		fw.startPosition = new Vector3(readFloatx,readFloaty,readFloatz);
		fw.speed = Utils.GetFloatFromString(entries[index]); index++;
		fw.percentAjar = Utils.GetFloatFromString(entries[index]); index++;
		fw.startTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		Transform parentTR = go.transform.parent.transform;
		index = Utils.LoadTransform(parentTR,ref entries,index);
		int numChildren = Utils.GetIntFromString(entries[index]); index++;
		fw.chunkIDs = new int[numChildren];
		int chunkdex = 0;
		for (int i=0; i<numChildren; i++) {
			// Get the index of the chunk prefab
			chunkdex = Utils.GetIntFromString(entries[index]); index++;
			fw.chunkIDs[i] = chunkdex;

			// Assumption here is that we are loading to a freshly instantiated
			// func_wall prefab and that there are no children chunks on the
			// mover_target GameObject yet.
			GameObject childGO = Instantiate(Const.a.chunkPrefabs[chunkdex],
						go.transform.localPosition, // 0's, transform loaded below
						Const.a.quaternionIdentity) as GameObject;
			childGO.transform.SetParent(go.transform); // Always set parent prior
													   // to loading transform.
			childGO.isStatic = false;
			childGO.layer = 18; // Door
			index = Utils.LoadSubActivatedGOState(childGO,ref entries,index);
		}
		Transform info_target = go.transform.parent.transform.GetChild(1);
		index = Utils.LoadTransform(info_target,ref entries,index);
		index = Utils.LoadAudioSource(go,ref entries,index);
		fw.Initialize();

		return index;
	}
}
