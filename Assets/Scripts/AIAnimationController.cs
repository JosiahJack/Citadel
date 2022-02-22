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
				case Const.aiState.Idle: 		Idle(); 		break;
				case Const.aiState.Walk:	 	Walk(); 		break;
				case Const.aiState.Run: 		Run(); 			break;
				case Const.aiState.Attack1: 	Attack1(); 		break;
				case Const.aiState.Attack2: 	Attack2(); 		break;
				case Const.aiState.Attack3: 	Attack3(); 		break;
				case Const.aiState.Pain: 		Pain();			break;
				case Const.aiState.Dying: 		Dying(); 		break;
				case Const.aiState.Dead:		Dead();			break;
				default: 						Idle(); 		break;
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
				animSwapFinished = PauseScript.a.relativeTime + animSwapFinishedDelay; // prevent flickering
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
}
