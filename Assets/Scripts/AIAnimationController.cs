using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAnimationController : MonoBehaviour {
	public float currentClipPercentage;
	public float clipEndThreshold = 0.99f;
	private SkinnedMeshRenderer smR;
	private Animator anim;
	private AnimatorStateInfo anstinfo;
	public AIController aic;
	private bool dying;
	private bool dead;
	public bool useDeadAnimForDeath = false;
	public bool playDeathAnim = true;
	public bool playDyingAnim = true;
	public float minWalkSpeedToAnimate = 0.32f;
	private bool checkVisWithSMR = false;

	void Start () {
		anim = GetComponent<Animator>();
		currentClipPercentage = 0;
		dead = false;
		checkVisWithSMR = false;
		smR = GetComponentInChildren<SkinnedMeshRenderer>();
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
			if (dead) {
				Dead();
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
				default: 						Idle(); 		break;
				}
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
		//alreadySetAnimation = false;
		if (gameObject.activeInHierarchy) {
			if (aic.actAsTurret) {
				anim.Play("Idle");
			} else {
				anim.Play("Run");
			}
		}
	}

	void Walk () {
		if (aic.rbody.velocity.magnitude > minWalkSpeedToAnimate) {
			if (aic.actAsTurret) {
				anim.Play("Idle");
			} else {
				anim.Play("Walk");
			}
		} else {
			anim.Play("Idle");
		}
	}

	void Attack1 () {
		if (gameObject.activeInHierarchy) anim.Play("Attack1");
	}

	void Attack2 () {
		//if (!alreadySetAnimation) {
		//	alreadySetAnimation = true;
			if (gameObject.activeInHierarchy) anim.Play("Attack2");
		//}
	}

	void Attack3 () {
		if (gameObject.activeInHierarchy) anim.Play("Attack3");
	}

	void Pain () {
		if (gameObject.activeInHierarchy) anim.Play("Pain");
	}

	void Dying () {
		dying = true;
		if (playDyingAnim && gameObject.activeInHierarchy) anim.Play("Death");
		if (currentClipPercentage > clipEndThreshold) {
			dying = false;
			dead = true;
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
