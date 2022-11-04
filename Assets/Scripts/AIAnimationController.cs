using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAnimationController : MonoBehaviour {
	// External manually assigned references
	public AIController aic;

	// Externall set values per instance
	public bool useDeadAnimForDeath = false;
	public bool playDeathAnim = true;
	public bool playDyingAnim = true;
	public float minWalkSpeedToAnimate = 0.32f;

	// Internal
	[HideInInspector] public float currentClipPercentage; // save
	[HideInInspector] public float clipEndThreshold = 0.99f;
	[HideInInspector] public Animator anim;
	[HideInInspector] public bool dying; // save
	[HideInInspector] public float animSwapFinished; // save
	private SkinnedMeshRenderer smR; // Optional
	private AnimatorStateInfo anstinfo;
	private bool checkVisWithSMR = false;
	private float animSwapFinishedDelay = 0.5f;
	private bool pauseStateUpdated = false;

	void Start () {
		anim = GetComponent<Animator>();
		smR = GetComponentInChildren<SkinnedMeshRenderer>();
		animSwapFinished = PauseScript.a.relativeTime;
		if (smR != null) checkVisWithSMR = true;
		else checkVisWithSMR = false;
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
		if (aic.rbody.velocity.sqrMagnitude > (minWalkSpeedToAnimate * minWalkSpeedToAnimate)) {
			if (aic.actAsTurret) {
				anim.Play("Idle");
			} else {
				anim.Play("Walk");
			}
		} else {
			if (animSwapFinished < PauseScript.a.relativeTime) {
				animSwapFinished = PauseScript.a.relativeTime + animSwapFinishedDelay; // Prevent flickering.
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
		if (currentClipPercentage > clipEndThreshold) dying = false;
	}

	void Dead () {
		if (playDeathAnim) {
			if (useDeadAnimForDeath) {
				anim.Play("Dead",0,1.0f);
			} else {
				anim.Play("Death",0,1.0f);
			}
		}
		if (anim.speed > 0) anim.speed = 0f;
	}

	void Inspect () {
		anim.Play("Inspect");
	}

	void Interacting () {
		anim.Play("Interact");
	}

	public static string Save(GameObject go) {
		AIAnimationController aiac = go.GetComponentInChildren<AIAnimationController>();
		if (aiac == null) {
			return "0000.00000|0|0000.00000"; // No warn, cyber enemies don't have one.
		}

		string line = System.String.Empty;
		line = Utils.FloatToString(aiac.currentClipPercentage);
		line += Utils.splitChar;
		line += Utils.BoolToString(aiac.dying); // bool
		line += Utils.splitChar;
		line += Utils.SaveRelativeTimeDifferential(aiac.animSwapFinished);
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		AIAnimationController aiac = go.GetComponentInChildren<AIAnimationController>();
		if (aiac == null || index < 0 || entries == null) return index + 3;

		aiac.currentClipPercentage = Utils.GetFloatFromString(entries[index]); index++; // float
		aiac.dying = Utils.GetBoolFromString(entries[index]); index++; // bool
		aiac.animSwapFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
		if (!aiac.aic.ai_dead) {
			if (aiac.anim != null) aiac.anim.speed = 1f;
		}
		return index;
	}
}
