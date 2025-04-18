﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AIAnimationController : MonoBehaviour {
	// External manually assigned references, unsaved, depends on prefab
	public AIController aic;

	// Externall set values per instance
	public bool useDeadAnimForDeath = false; // save
	public bool playDeathAnim = true; // save
	public bool playDyingAnim = true; // save
	public float minWalkSpeedToAnimate = 0.32f; // save

	// Internal
	[HideInInspector] public float currentClipPercentage; // save
	[HideInInspector] public bool dying; // save
	[HideInInspector] public float animSwapFinished; // save
	[HideInInspector] public string clipName;

	// Derived or temporary variables, unsaved
	[HideInInspector] public Animator anim;
	private SkinnedMeshRenderer smR; // Optional, used for performance
	private AnimatorStateInfo anstinfo;
	private bool checkVisWithSMR = false;
	private bool pauseStateUpdated = false;
	private bool firstUpdateAfterLoad = false;
	private string loadedClipName;
	private int loadedClipIndex;
	private float loadedAnimatorPlaybackTime;
	private float loadedSetSpeed;
	private bool initialized = false;
	private bool doneDidDead = false;
	private static StringBuilder s1 = new StringBuilder();

	public void Start () {
	    if (initialized) return;
	    
	    animSwapFinished = PauseScript.a.relativeTime;
		anim = GetComponent<Animator>();
		smR = GetComponentInChildren<SkinnedMeshRenderer>(true);
		if (smR != null) checkVisWithSMR = true;
		else checkVisWithSMR = false;
		
		switch (aic.currentState) {
			case AIState.Idle: 		clipName = "Idle";	  break;
			case AIState.Walk:	 	clipName = "Walk"; 	  break;
			case AIState.Run: 		clipName = "Run";	  break;
			case AIState.Attack1: 	clipName = "Attack1"; break;
			case AIState.Attack2: 	clipName = "Attack2"; break;
			case AIState.Attack3: 	clipName = "Attack3"; break;
			case AIState.Pain: 		clipName = "Pain";	break;
			case AIState.Dying: 	clipName = "Death";	break;
			case AIState.Dead:
				if (useDeadAnimForDeath) clipName = "Dead";
				else clipName = "Death";

				break;
			default: 				clipName = "Idle";	break;
		}
		
		initialized = true;
	}

	void OnEnable() {
		doneDidDead = false; // Ensure death anim is set to proper frame on enable.
	}

	void Update() {
		if (PauseScript.a.Paused() || PauseScript.a.MenuActive()) {
			if (!pauseStateUpdated) {
				if (anim.speed != 0) anim.speed = 0;
				pauseStateUpdated = true;
			}
			return;
		} else {
			if (pauseStateUpdated) {
				if (anim.speed != 1f) anim.speed = 1f;
				pauseStateUpdated = false;
			}
		}

		if (checkVisWithSMR) {
			if (smR != null) {
				if (!smR.isVisible && aic.withinPVS) return;
			}
		}
		
		if (firstUpdateAfterLoad) { SetAnimAfterLoad(); firstUpdateAfterLoad = false; return; }
		
		if (aic.asleep && aic.currentState != AIState.Dying
			&& aic.currentState != AIState.Dead) {
			Idle();
			return;
		}
		
		if (aic.currentState == AIState.Run && aic.tranquilizeFinished >= PauseScript.a.relativeTime) {
			Idle();
			return;
		}

		switch (aic.currentState) {
			case AIState.Idle: 		Idle(); 	break;
			case AIState.Walk:	 	Walk(); 	break;
			case AIState.Run: 		Run(); 		break;
			case AIState.Attack1: 	Attack1(); 	break;
			case AIState.Attack2: 	Attack2(); 	break;
			case AIState.Attack3: 	Attack3(); 	break;
			case AIState.Pain: 		Pain();		break;
			case AIState.Dying: 	Dying(); 	break;
			case AIState.Dead:		Dead();		break;
			default: 				Idle(); 	break;
		}
	}

	void Idle () {
		if (aic.asleep || aic.tranquilizeFinished >= PauseScript.a.relativeTime) {
			if (anim.speed > 0) anim.speed = 0;
		} else {
			if (anim.speed != 1f) anim.speed = 1f;
			if (UnityEngine.Random.Range(0,1f) < 0.5f) {
			    anim.Play("Idle");
			    clipName = "Idle";
			}
		}
	}

	void Run () {
		if (anim.speed != 1f) anim.speed = 1f;
		if (aic.actAsTurret) {
			anim.Play("Idle");
			clipName = "Idle";
		} else {
			anim.Play("Run");
			clipName = "Run";
		}
	}

	void Walk () {
		if (anim.speed != 1f) anim.speed = 1f;
		if (aic.rbody.velocity.sqrMagnitude >
			(minWalkSpeedToAnimate * minWalkSpeedToAnimate)) {
			if (aic.actAsTurret) {
				anim.Play("Idle");
				clipName = "Idle";
			} else {
				anim.Play("Walk");
				clipName = "Walk";
			}
		} else {
			 // Prevent flickering by using a delay timer.
			if (animSwapFinished < PauseScript.a.relativeTime) {
				animSwapFinished = PauseScript.a.relativeTime + 0.5f;
				anim.Play("Idle");
				clipName = "Idle";
			}
		}
	}

	void Attack1 () {
		if (anim.speed != 1f) anim.speed = 1f;
		anim.Play("Attack1");
		clipName = "Attack1";
	}

	void Attack2 () {
		if (anim.speed != 1f) anim.speed = 1f;
		anim.Play("Attack2");
		clipName = "Attack2";
	}

	void Attack3 () {
		if (anim.speed != 1f) anim.speed = 1f;
		anim.Play("Attack3");
		clipName = "Attack3";
	}

	void Pain () {
		if (anim.speed != 1f) anim.speed = 1f;
		anim.Play("Pain");
		clipName = "Pain";
	}

	void Dying () {
		if (anim.speed != 1f) anim.speed = 1f;
		aic.asleep = false;
		Utils.Deactivate(aic.sleepingCables);
		if (anim.speed != 1f) anim.speed = 1f;
		anstinfo = anim.GetCurrentAnimatorStateInfo(0);
		currentClipPercentage = anstinfo.normalizedTime % 1;
		dying = true;
		if (playDyingAnim) {
		    anim.Play("Death");
		    clipName = "Death";
		}
		if (currentClipPercentage > 0.99f) dying = false;
	}

	void Dead () {
		if (playDeathAnim) {
			if (useDeadAnimForDeath) {
				if (!doneDidDead) anim.Play("Dead",0,1.0f);
				doneDidDead = true;
				clipName = "Dead";
			} else {
				if (!doneDidDead) anim.Play("Death",0,1.0f);
				doneDidDead = true;
				clipName = "Death";
			}
		}

		if (anim.speed > 0f) anim.speed = 0f;
	}

	void Inspect () {
		if (anim.speed != 1f) anim.speed = 1f;
		anim.Play("Inspect");
		clipName = "Inspect";
	}

	void Interacting () {
		if (anim.speed != 1f) anim.speed = 1f;
		anim.Play("Interact");
		clipName = "Interact";
	}
	
	void SetAnimAfterLoad() {
		firstUpdateAfterLoad = false;
		anim.speed = loadedSetSpeed;
		if (anim.speed == 0) anim.speed = 1f;
		if (string.IsNullOrWhiteSpace(loadedClipName)) loadedClipName = "Idle";
		if (loadedClipIndex < 0) loadedClipIndex = 0;
		anim.Play(loadedClipName,loadedClipIndex,loadedAnimatorPlaybackTime);
	}
	
	public void SetAnimFromLoad(string n, int i, float t, float sp) {
		firstUpdateAfterLoad = true;
		loadedClipName = n;
		loadedClipIndex = i;
		loadedAnimatorPlaybackTime = t;
		loadedSetSpeed = sp;
		anim.speed = loadedSetSpeed;
	}

	public static string Save(GameObject go) {
		AIAnimationController aiac = go.GetComponentInChildren<AIAnimationController>(true);
		s1.Clear();
		if (aiac == null) { // Keep!  Cyber enemies don't have an AIAC!
			return "clipName:|currentClipPercentage:0000.00000|dying:0|animSwapFinished:0000.00000|useDeadAnimForDeath:0|playDeathAnim:0|playDyingAnim:0|minWalkSpeedToAnimate:0000.00000|anim.speed:0000.00000";
		}
		
		s1.Append(Utils.SaveString(aiac.clipName,"clipName"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(aiac.currentClipPercentage,"currentClipPercentage"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aiac.dying,"dying"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aiac.animSwapFinished,"animSwapFinished"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aiac.useDeadAnimForDeath,"useDeadAnimForDeath"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aiac.playDeathAnim,"playDeathAnim"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aiac.playDyingAnim,"playDyingAnim"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(aiac.minWalkSpeedToAnimate,"minWalkSpeedToAnimate"));
		s1.Append(Utils.splitChar);
		if (aiac.anim != null) {
			s1.Append(Utils.FloatToString(aiac.anim.speed,"anim.speed"));
		} else {
			s1.Append(Utils.FloatToString(1.0f,"anim.speed"));
		}

		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		AIAnimationController aiac = go.GetComponentInChildren<AIAnimationController>(true);

		if (aiac == null) {
			AIController aic = go.GetComponentInChildren<AIController>(true);
			if (aic == null) {
				if (Const.a.moveTypeForNPC[aic.index] != AIMoveType.Cyber
					&& aic.index != 20 && aic.index != 0) {
					
					Debug.LogError("AIAnimationController.Load failure, aiac == "
							  + "null on " + go.name + " at location "
							  + go.transform.localPosition.ToString());
				} else {
					return index + 9; // Keep!  Cyber enemies don't have an AIAC!
				}
			} else {
				return index + 9;
			}
		}
		
		aiac.initialized = false;
		aiac.Start();
        aiac.clipName = Utils.LoadString(entries[index],"clipName"); index++;
		aiac.currentClipPercentage = Utils.GetFloatFromString(entries[index],"currentClipPercentage"); index++;
		aiac.dying = Utils.GetBoolFromString(entries[index],"dying"); index++;
		aiac.animSwapFinished = Utils.LoadRelativeTimeDifferential(entries[index],"animSwapFinished"); index++;
		aiac.useDeadAnimForDeath = Utils.GetBoolFromString(entries[index],"useDeadAnimForDeath"); index++;
		aiac.playDeathAnim = Utils.GetBoolFromString(entries[index],"playDeathAnim"); index++;
		aiac.playDyingAnim = Utils.GetBoolFromString(entries[index],"playDyingAnim"); index++;
		aiac.minWalkSpeedToAnimate = Utils.GetFloatFromString(entries[index],"minWalkSpeedToAnimate"); index++;
        float setSpeed = 1f;
		if (!aiac.aic.HasHealth(aiac.aic.healthManager)) {
			setSpeed = 0f;
		} else {
			setSpeed = Utils.GetFloatFromString(entries[index],"anim.speed");
			if (setSpeed < 0f || setSpeed > 100f) setSpeed = 1f;
		}

		aiac.SetAnimFromLoad(aiac.clipName,0,aiac.currentClipPercentage,setSpeed); index++;
		return index;
	}
	
	void OnDestroy() {
		aic = null;
		clipName = null;
		anim = null;
		smR = null;
	}
}
