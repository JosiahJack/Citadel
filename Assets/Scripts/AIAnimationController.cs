using System.Collections;
using System.Collections.Generic;
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

	// Derived or temporary variables, unsaved
	[HideInInspector] public Animator anim;
	private SkinnedMeshRenderer smR; // Optional, used for performance
	private AnimatorStateInfo anstinfo;
	private bool checkVisWithSMR = false;
	private bool pauseStateUpdated = false;

	void Start () {
		anim = GetComponent<Animator>();
		smR = GetComponentInChildren<SkinnedMeshRenderer>(true);
		if (smR != null) checkVisWithSMR = true;
		else checkVisWithSMR = false;

		animSwapFinished = PauseScript.a.relativeTime;
		currentClipPercentage = 0;
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
				if (!smR.isVisible) return;
			}
		}
		if (dying) {
			anstinfo = anim.GetCurrentAnimatorStateInfo(0);
			currentClipPercentage = anstinfo.normalizedTime % 1;
			Dying();
		} else {
			if (aic.asleep) {
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
	}

	void Idle () {
		if (aic.asleep) {
			if (anim.speed > 0) anim.speed = 0;
		} else {
			if (anim.speed != 1f) anim.speed = 1f;
			if (UnityEngine.Random.Range(0,1f) < 0.5f) anim.Play("Idle");
		}
	}

	void Run () {
		if (aic.actAsTurret) {
			anim.Play("Idle");
		} else {
			anim.Play("Run");
		}
	}

	void Walk () {
		if (aic.rbody.velocity.sqrMagnitude >
			(minWalkSpeedToAnimate * minWalkSpeedToAnimate)) {
			if (aic.actAsTurret) {
				anim.Play("Idle");
			} else {
				anim.Play("Walk");
			}
		} else {
			 // Prevent flickering by using a delay timer.
			if (animSwapFinished < PauseScript.a.relativeTime) {
				animSwapFinished = PauseScript.a.relativeTime + 0.5f;
				anim.Play("Idle");
			}
		}
	}

	void Attack1 () {
		anim.Play("Attack1");
	}

	void Attack2 () {
		anim.Play("Attack2");
	}

	void Attack3 () {
		anim.Play("Attack3");
	}

	void Pain () {
		anim.Play("Pain");
	}

	void Dying () {
		dying = true;
		if (playDyingAnim) anim.Play("Death");
		if (currentClipPercentage > 0.99f) dying = false;
	}

	void Dead () {
		if (playDeathAnim) {
			if (useDeadAnimForDeath) {
				anim.Play("Dead",0,1.0f);
			} else {
				anim.Play("Death",0,1.0f);
			}
		}
		if (anim.speed > 0f) anim.speed = 0f;
	}

	void Inspect () {
		anim.Play("Inspect");
	}

	void Interacting () {
		anim.Play("Interact");
	}

	public static string Save(GameObject go) {
		AIAnimationController aiac = go.GetComponentInChildren<AIAnimationController>(true);
		// No debug warn, cyber enemies don't have one.
		if (aiac == null) return Utils.DTypeWordToSaveString("fbf");

		string line = System.String.Empty;
		line = Utils.FloatToString(aiac.currentClipPercentage);
		line += Utils.splitChar + Utils.BoolToString(aiac.dying);
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(aiac.animSwapFinished);
		line += Utils.splitChar + Utils.BoolToString(aiac.useDeadAnimForDeath);
		line += Utils.splitChar + Utils.BoolToString(aiac.playDeathAnim);
		line += Utils.splitChar + Utils.BoolToString(aiac.playDyingAnim);
		line += Utils.splitChar + Utils.FloatToString(aiac.minWalkSpeedToAnimate);
		if (aiac.anim != null) line += Utils.splitChar + Utils.FloatToString(aiac.anim.speed);
		else line += Utils.splitChar + Utils.FloatToString(1.0f);

		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		AIAnimationController aiac = go.GetComponentInChildren<AIAnimationController>(true);
		if (aiac == null) {
			AIController aic = go.GetComponentInChildren<AIController>(true);
			if (aic != null) {
				if (Const.a.moveTypeForNPC[aic.index] != AIMoveType.Cyber
					&& aic.index != 20 && aic.index != 0) {
					Debug.Log("AIAnimationController.Load failure, aiac == "
							  + "null on " + go.name + " at location "
							  + go.transform.localPosition.ToString());
				}
			} else {
				Debug.Log("AIAnimationController.Load failure, aic == null "
						  + "on " + go.name + " with parent of "
						  + go.transform.parent.gameObject.name);
			}
			return index + 3;
		}

		if (index < 0) {
			Debug.Log("AIAnimationController.Load failure, index < 0");
			return index + 3;
		}

		if (entries == null) {
			Debug.Log("AIAnimationController.Load failure, entries == null");
			return index + 3;
		}

		aiac.currentClipPercentage = Utils.GetFloatFromString(entries[index]); index++;
		aiac.dying = Utils.GetBoolFromString(entries[index]); index++;
		aiac.animSwapFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		aiac.useDeadAnimForDeath = Utils.GetBoolFromString(entries[index]); index++;
		aiac.playDeathAnim = Utils.GetBoolFromString(entries[index]); index++;
		aiac.playDyingAnim = Utils.GetBoolFromString(entries[index]); index++;
		aiac.minWalkSpeedToAnimate = Utils.GetFloatFromString(entries[index]); index++;
		if (!aiac.aic.ai_dead) {
			float setSpeed = Utils.GetFloatFromString(entries[index]); index++;
			if (setSpeed < 0f || setSpeed > 100f) setSpeed = 1f;
			if (aiac.anim != null) aiac.anim.speed = setSpeed;
		}
		return index;
	}
}
