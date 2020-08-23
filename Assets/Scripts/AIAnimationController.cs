using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAnimationController : MonoBehaviour {
	public float currentClipPercentage; // save
	public float clipEndThreshold = 0.99f;
	private SkinnedMeshRenderer smR;
	[HideInInspector]
	public Animator anim;
	private AnimatorStateInfo anstinfo;
	public AIController aic;
	public bool dying; // save
	public bool useDeadAnimForDeath = false;
	public bool playDeathAnim = true;
	public bool playDyingAnim = true;
	public float minWalkSpeedToAnimate = 0.32f;
	private bool checkVisWithSMR = false;
	private float animSwapFinished;
	private float animSwapFinishedDelay = 0.5f;

	void Start () {
		anim = GetComponent<Animator>();
		currentClipPercentage = 0;
		checkVisWithSMR = false;
		smR = GetComponentInChildren<SkinnedMeshRenderer>();
		animSwapFinished = PauseScript.a.relativeTime;
		if (smR != null) checkVisWithSMR = true;
	}

	void Update() {
		if (PauseScript.a.Paused() || PauseScript.a.mainMenu.activeInHierarchy) {
			if (anim.speed != 0) anim.speed = 0;
			return;
		} else {
			if (anim.speed != 1f) anim.speed = 1f;
		}

		if (checkVisWithSMR) {
			if (!smR.isVisible) return;
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
			case Const.aiState.Inspect: 	Inspect(); 		break;
			case Const.aiState.Interacting: Interacting();	break;
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
		if (aic.rbody.velocity.magnitude > minWalkSpeedToAnimate) {
			if (aic.actAsTurret) {
				anim.Play("Idle");
			} else {
				//if (animSwapFinished < PauseScript.a.relativeTime) {
				//	animSwapFinished = PauseScript.a.relativeTime + animSwapFinishedDelay; // prevent flickering
					anim.Play("Walk");
				//}
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
		if (currentClipPercentage > clipEndThreshold) {
			dying = false;
		}
	}

	void Dead () {
		if (useDeadAnimForDeath) {
			if (playDeathAnim && gameObject.activeInHierarchy) anim.Play("Dead",0,1.0f);
		} else {
			if (playDeathAnim && gameObject.activeInHierarchy) anim.Play("Death",0,1.0f);
		}
		anim.speed = 0f;
	}

	void Inspect () {
		if (gameObject.activeInHierarchy) anim.Play("Inspect");
	}

	void Interacting () {
		if (gameObject.activeInHierarchy) anim.Play("Interact");
	}
}
