using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAnimationController : MonoBehaviour {
	public float currentClipPercentage;
	public float clipEndThreshold = 0.99f;

	private Animator anim;
	private AnimatorStateInfo anstinfo;
	private AIController aic;
	private bool dying;
	private bool dead;

	void Awake () {
		anim = GetComponent<Animator>();
		aic = GetComponent<AIController>();
		currentClipPercentage = 0f;
		dead = false;
	}

	void Update () {
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
		anim.Play("Idle");
	}

	void Run () {
		anim.Play("Run");
	}

	void Walk () {
		anim.Play("Walk");
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
		anim.Play("Death");
		if (currentClipPercentage > clipEndThreshold) {
			dying = false;
			dead = true;
		}
	}

	void Dead () {
		anim.Play("Death");
		anim.speed = 0f;
	}

	void Inspect () {
		anim.Play("Inspect");
	}

	void Interacting () {
		anim.Play("Interact");
	}
}
