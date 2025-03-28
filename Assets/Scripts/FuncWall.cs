using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;

public class FuncWall : MonoBehaviour {
	// Externally set in inspector per instance
	public float speed = 0.64f; // save
	public GameObject targetPosition;
	public float targetPositionY;
	public FuncStates startState; // save
	public float percentAjar = 0; // save
	public float percentMoved = 0; // save
	private AudioSource SFXSource;
	public FuncStates currentState; // save
	public int[] chunkIDs; // save

	[HideInInspector] public float startTime; // save
	[HideInInspector] public Vector3 startPosition; // save
	[HideInInspector] public Rigidbody rbody;
	[HideInInspector] public bool stopSoundPlayed; // save
	private Vector3 tempVec;
	private float dist;         // Only ever used right away, not saved.
	private float distanceLeft; // Only ever used right away, not saved.
	private static StringBuilder s1 = new StringBuilder();

	public void InitializeFromLoad() {
		rbody = GetComponent<Rigidbody>();
		SFXSource = GetComponent<AudioSource>();
		rbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		rbody.isKinematic = true;
		rbody.useGravity = false;
		PauseRigidbody pr = GetComponent<PauseRigidbody>();
		pr.previousVelocity = Vector3.zero;
		pr.previousUseGravity = false;
		pr.previousKinematic = true;
		pr.previouscolDetMode = CollisionDetectionMode.ContinuousSpeculative;
		pr.previousSet = true;
		targetPositionY = targetPosition.transform.localPosition.y;
		tempVec = (transform.position - targetPosition.transform.position);
		float distTotal = Vector3.Distance(startPosition,
										   targetPosition.transform.position);

		tempVec = -tempVec.normalized;
		if (currentState == FuncStates.AjarMovingTarget) {
			tempVec *= (distTotal * percentAjar);
		} else if (currentState == FuncStates.AjarMovingStart) {
			tempVec *= (distTotal * (1f - percentAjar));
		} else if (currentState == FuncStates.MovingStart) {
			tempVec *= (distTotal * (1f - percentMoved));
		} else {
			tempVec *= (distTotal * percentMoved);
		}

		if (tempVec.x >  10000f) tempVec.x =  10000f;
		if (tempVec.x < -10000f) tempVec.x = -10000f;
		if (float.IsNaN(tempVec.x)) tempVec.x = 0f;
		if (tempVec.y >  10000f) tempVec.y =  10000f;
		if (tempVec.y < -10000f) tempVec.y = -10000f;
		if (float.IsNaN(tempVec.y)) tempVec.y = 0f;
		if (tempVec.z >  10000f) tempVec.z =  10000f;
		if (tempVec.z < -10000f) tempVec.z = -10000f;
		if (float.IsNaN(tempVec.z)) tempVec.z = 0f;
		tempVec += transform.position;
		transform.position = tempVec;
	}

	public void Targetted (UseData ud) {
		switch (currentState) {
			case FuncStates.Start:            MoveTarget(); break;
			case FuncStates.Target:           MoveStart();  break;
			case FuncStates.MovingStart:      MoveTarget(); break;
			case FuncStates.MovingTarget:     MoveStart();  break;
			case FuncStates.AjarMovingStart:  MoveStart();  break;
			case FuncStates.AjarMovingTarget: MoveTarget(); break;
		}

		if (SFXSource != null) {
			SFXSource.clip = Const.a.sounds[76]; // doorwall_move
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
		float distTotal = Mathf.Abs(targetPositionY);
		percentMoved = (distTotal - distanceLeft) / distTotal;
		if (float.IsNaN(percentMoved)) percentMoved = 0f;
		if (percentMoved > 1.0f) percentMoved = 1.0f;
		if (percentMoved < 0f) percentMoved = 0f;
		if (distanceLeft <= 0.04f || startTime < PauseScript.a.relativeTime) {
			currentState = newState;
			if (SFXSource != null) {
				SFXSource.Stop ();
				SFXSource.loop = false;
				if (!stopSoundPlayed) {
					Utils.PlayOneShotSavable(SFXSource,Const.a.sounds[77]);
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
		s1.Clear();
		s1.Append(Utils.IntToString(Utils.FuncStatesToInt(fw.currentState),"FuncWall.currentState"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(Utils.FuncStatesToInt(fw.startState),"startState"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(fw.stopSoundPlayed,"stopSoundPlayed"));
		s1.Append(Utils.splitChar);
        s1.Append(Utils.FloatToString(fw.startPosition.x,"startPosition.x"));
		s1.Append(Utils.splitChar);
        s1.Append(Utils.FloatToString(fw.startPosition.y,"startPosition.y"));
		s1.Append(Utils.splitChar);
        s1.Append(Utils.FloatToString(fw.startPosition.z,"startPosition.z"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(fw.speed,"speed"));
		s1.Append(Utils.splitChar );
		s1.Append(Utils.FloatToString(fw.percentAjar,"percentAjar"));
		s1.Append(Utils.splitChar );
		s1.Append(Utils.FloatToString(fw.percentMoved,"percentMoved"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(fw.startTime,"startTime"));

		// The mover_target transform and position was saved by SaveObject.Save
		// prior to that function calling this function, so only save the
		// parent transform here.
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveTransform(go.transform.parent.transform));
		Transform info_target = go.transform.parent.transform.GetChild(1);
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveTransform(info_target));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveAudioSource(go));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(fw.chunkIDs.Length,"chunkIDs.Length"));
		for (int i=0;i<go.transform.childCount; i++) {
			s1.Append(Utils.splitChar);
			s1.Append(Utils.UintToString(fw.chunkIDs[i],"chunkIDs["+ i.ToString() + "]"));
			s1.Append(Utils.splitChar);
			s1.Append(Utils.SaveChildGOState(go,i));
		}
		return s1.ToString();
	}

	// Load data to a freshly instantiated blank func_wall prefab.
	// Prefab hierarchy is assumed to be in this structure:
	// func_wall            This is the main parent which is merely a container
	// ->mover_target       This is the GameObject with FuncWall as a component
	// ->->chunk_somechunk  These are the walls or floors that comprise the
	// ->->chunk_somechunk2   visible and physical collisions.  These will not
	// ->->chunk_somechunk3   exist yet on a freshly instantiated prefab.
	// ->->etc. etc.
	// ->info_target         This is a relative offset position creator
	public static int Load(GameObject go, ref string[] entries, int index) {
		float readFloatx, readFloaty, readFloatz;
		FuncWall fw = go.GetComponent<FuncWall>(); // Fairweather we are
												   // having. Vague Quake
												   // mapper reference.
		
		fw.currentState = Utils.GetFuncStatesFromInt(Utils.GetIntFromString(entries[index],"FuncWall.currentState")); index++;
		fw.startState = Utils.GetFuncStatesFromInt(Utils.GetIntFromString(entries[index],"startState")); index++;
		fw.stopSoundPlayed = Utils.GetBoolFromString(entries[index],"stopSoundPlayed"); index++;
		readFloatx = Utils.GetFloatFromString(entries[index],"startPosition.x"); index++;
		readFloaty = Utils.GetFloatFromString(entries[index],"startPosition.y"); index++;
		readFloatz = Utils.GetFloatFromString(entries[index],"startPosition.z"); index++;
		fw.startPosition = new Vector3(readFloatx,readFloaty,readFloatz);
		fw.speed = Utils.GetFloatFromString(entries[index],"speed"); index++;
		fw.percentAjar = Utils.GetFloatFromString(entries[index],"percentAjar"); index++;
		fw.percentMoved = Utils.GetFloatFromString(entries[index],"percentMoved"); index++;
		fw.startTime = Utils.LoadRelativeTimeDifferential(entries[index],"startTime"); index++;
		Transform parentTR = go.transform.parent.transform;
		index = Utils.LoadTransform(parentTR,ref entries,index);
		Transform info_target = go.transform.parent.transform.GetChild(1);
		index = Utils.LoadTransform(info_target,ref entries,index);
		index = Utils.LoadAudioSource(go,ref entries,index);
		fw.transform.localPosition = new Vector3(0f,0f,0f);
		int numChildren = Utils.GetIntFromString(entries[index],"chunkIDs.Length"); index++;
		fw.chunkIDs = new int[numChildren];
		int chunkdex = 0;
		for (int i=0; i<numChildren; i++) {
			// Get the index of the chunk prefab
			chunkdex = Utils.GetIntFromString(entries[index],"chunkIDs[" + i.ToString() + "]"); index++;
			if (chunkdex < 0 || chunkdex > 306) continue;
			
			fw.chunkIDs[i] = chunkdex;

			// Assumption here is that we are loading to a freshly instantiated
			// func_wall prefab and that there are no children chunks on the
			// mover_target GameObject yet.
			GameObject childGO = Instantiate(Const.a.GetPrefab(chunkdex),
						go.transform.localPosition, // 0's, transform is below
						Const.a.quaternionIdentity) as GameObject;
			childGO.transform.SetParent(go.transform); // Set parent prior
													   // to loading transform.
			#if UNITY_EDITOR
				childGO.isStatic = false; // EDITOR ONLY!!!!!!!!!!!!
			#endif
			childGO.layer = 18; // Door
			for (int j=0;j<childGO.transform.childCount;j++) {
				Transform subtr = childGO.transform.GetChild(j);
				if (subtr == null) continue;
				
				MeshCollider mcol = subtr.GetComponent<MeshCollider>();
				if (mcol == null) continue;
				
				if (!mcol.convex) mcol.enabled = false;
			}
			index = Utils.LoadSubActivatedGOState(childGO,ref entries,index);
		}
		fw.InitializeFromLoad();

		return index;
	}
}
