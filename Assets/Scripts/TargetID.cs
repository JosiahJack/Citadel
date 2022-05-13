using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetID : MonoBehaviour {
	public TextMesh text;
	public string currentText;
	public bool useLife;
	public float lifetime;
	[HideInInspector] public float lifetimeFinished;
	public ParticleSystem partSys;
	/*[DTValidator.Optional] */public Transform parent;
	/*[DTValidator.Optional] */public HealthManager linkedHM;
	public Transform playerCapsuleTransform;
	public float playerLinkDistance = 10f;
	public bool displayHealth = false;
	public TextMesh secondaryText;
	public bool displayRange = false;
	public string comma = ", ";
	public string rangeMetersM = "M";
	private string secondaryDisplayString;
	public bool displayAttitude = false;
	public bool displayName = false;
	public TextMesh nameText;

    void Start() {
		lifetimeFinished = PauseScript.a.relativeTime;
		secondaryDisplayString = System.String.Empty;
		nameText.text = System.String.Empty;
		text.text = System.String.Empty;
    }

	void FixedUpdate() {
		if (parent != null) transform.position = parent.position;
	}

	public void SendDamageReceive(float damage) {
		if (linkedHM == null || (Inventory.a.hardwareVersion[4] < 3 && (damage > 0f))) return;

		if (damage > linkedHM.maxhealth * 0.75f) {
			// Severe Damage
			currentText = Const.a.stringTable[514];
		}
		if (damage > linkedHM.maxhealth * 0.50f) {
			// Major Damage
			currentText = Const.a.stringTable[515];
		}
		if (damage > linkedHM.maxhealth * 0.25f) {
			// Normal Damage
			currentText = Const.a.stringTable[513];
		}

		if (damage > 0f) {
			// Minor Damage
			currentText = Const.a.stringTable[512];
			return;
		}

		currentText = Const.a.stringTable[511]; // NO DAMAGE
		lifetime = 1f;
		lifetimeFinished = PauseScript.a.relativeTime + lifetime;
		text.text = currentText;
	}

    void Update() {
		if ((Vector3.Distance(transform.position,playerCapsuleTransform.position) > playerLinkDistance) || (linkedHM.health <= 0f) || parent == null || playerCapsuleTransform == null) {
			secondaryText.text = System.String.Empty; // blank out text
			text.text = System.String.Empty; // blank out text
			linkedHM.linkedTargetID = null;
			linkedHM.aic.hasTargetIDAttached = false;
			linkedHM = null;
			gameObject.SetActive(false); // put back into pool
		}
		if (displayName) {
			if (nameText != null && linkedHM != null) {
				if (linkedHM.aic != null) nameText.text = linkedHM.aic.targetID; 
			}
		}
		if (displayHealth && linkedHM != null) {
			secondaryDisplayString = Mathf.Floor(linkedHM.health).ToString("000");
		} else {
			secondaryDisplayString = System.String.Empty;
		}
		if (displayRange && linkedHM != null) secondaryDisplayString += (comma + Vector3.Distance(playerCapsuleTransform.position,linkedHM.transform.position).ToString("00.0"), rangeMetersM); 
		if (displayAttitude && linkedHM != null) {
				secondaryDisplayString += comma;
			if (linkedHM.aic.asleep) {
				secondaryDisplayString = (secondaryDisplayString + Const.a.stringTable[519]); // Asleep
			} else {
				switch (linkedHM.aic.currentState) {
					case AIState.Walk: secondaryDisplayString = (secondaryDisplayString + Const.a.stringTable[517]); break; // Cautious
					case AIState.Inspect: secondaryDisplayString = (secondaryDisplayString + Const.a.stringTable[517]); break; // Cautious
					case AIState.Interacting: secondaryDisplayString = (secondaryDisplayString + Const.a.stringTable[517]); break; // Cautious
					case AIState.Run: secondaryDisplayString = (secondaryDisplayString + Const.a.stringTable[518]); break; // Hostile
					case AIState.Attack1: secondaryDisplayString = (secondaryDisplayString + Const.a.stringTable[518]); break; // Hostile
					case AIState.Attack2: secondaryDisplayString = (secondaryDisplayString + Const.a.stringTable[518]); break; // Hostile
					case AIState.Attack3: secondaryDisplayString = (secondaryDisplayString + Const.a.stringTable[518]); break; // Hostile
					case AIState.Pain: secondaryDisplayString = (secondaryDisplayString + Const.a.stringTable[518]); break; // Hostile
					default: secondaryDisplayString = (secondaryDisplayString + Const.a.stringTable[516]); break; // Idle
				}
			}
		}
		secondaryText.text = secondaryDisplayString;
		if (currentText != System.String.Empty) {
			if (linkedHM != null) {
				if (linkedHM.aic != null) {
					if (linkedHM.aic.tranquilizeFinished > PauseScript.a.relativeTime) {
						currentText = Const.a.stringTable[536];
					}
				}
			}
			text.text = currentText;
		}
    }
}
