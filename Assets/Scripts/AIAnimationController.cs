using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAnimationController : MonoBehaviour, IBatchUpdate {
	public float currentClipPercentage;
	public float clipEndThreshold = 0.99f;

	private Animator anim;
	private AnimatorStateInfo anstinfo;
	public AIController aic;
	private bool dying;
	private bool dead;
	public bool useDeadAnimForDeath = false;
	private bool alreadySetAnimation = false;
	public bool playDeathAnim = true;
	public bool playDyingAnim = true;

	void Start () {
		anim = GetComponent<Animator>();
		//aic = GetComponent<AIController>();
		currentClipPercentage = 0;
		dead = false;
		alreadySetAnimation = false;
		UpdateManager.Instance.RegisterSlicedUpdate(this, UpdateManager.UpdateMode.Always);
	}

	public void BatchUpdate () {
		if (PauseScript.a != null && PauseScript.a.Paused()) {
			anim.speed = 0;
			alreadySetAnimation = false;
			return;
		} else {
			anim.speed = 1f;
		}

		if (dying) {
			Dying();
		} else {
			if (dead) {
				Dead();
			} else {
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

		anstinfo = anim.GetCurrentAnimatorStateInfo(0);
		currentClipPercentage = anstinfo.normalizedTime % 1;
	}

	void Idle () {
		if (gameObject.activeInHierarchy) anim.Play("Idle");
	}

	void Run () {
		alreadySetAnimation = false;
		if (gameObject.activeInHierarchy) anim.Play("Run");
	}

	void Walk () {
		if (gameObject.activeInHierarchy) anim.Play("Walk");
	}

	void Attack1 () {
		if (gameObject.activeInHierarchy) anim.Play("Attack1");
	}

	void Attack2 () {
		if (!alreadySetAnimation) {
			alreadySetAnimation = true;
			if (gameObject.activeInHierarchy) anim.Play("Attack2");
		}
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
			if (playDeathAnim && gameObject.activeInHierarchy) anim.Play("Dead");
		} else {
			if (playDeathAnim && gameObject.activeInHierarchy) anim.Play("Death");
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
