using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetID : MonoBehaviour {
	public TextMesh text;
	public string currentText;
	public bool useLife;
	public float lifetime;
	[HideInInspector] public float lifetimeFinished;
	[HideInInspector] public float damageTime;
	[HideInInspector] public float damageTimeFinished;
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
	public bool stunned = false;

    void Start() {
		secondaryDisplayString = System.String.Empty;
		nameText.text = System.String.Empty;
		text.text = System.String.Empty;
    }

	void FixedUpdate() {
		if (parent != null) transform.position = parent.position;
	}

	public void SendDamageReceive(float damage, DamageData dd) {
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
		damageTime = 1f;
		lifetimeFinished = PauseScript.a.relativeTime + lifetime;
		damageTimeFinished = PauseScript.a.relativeTime + damageTime;
		if (dd.attackType == AttackType.Tranq) {
			damageTimeFinished = PauseScript.a.relativeTime - 1f; // Don't show
																  // damage for
																  // tranqs
		}
		text.text = currentText;
	}

	void Deactivate() {
		secondaryText.text = System.String.Empty; // blank out text
		text.text = System.String.Empty; // blank out text
		if (linkedHM != null) {
			linkedHM.linkedTargetID = null;
			linkedHM.aic.hasTargetIDAttached = false;
			linkedHM = null;
		}

		gameObject.SetActive(false); // put back into pool
		Destroy(this.gameObject);
	}

    void Update() {
		if (linkedHM != null) {
			if (linkedHM.health <= 0f) {
				Deactivate();
				return;
			}

			if (linkedHM.isNPC && linkedHM.aic != null) {
				if (linkedHM.aic.tranquilizeFinished > PauseScript.a.relativeTime) {
					stunned = true;
				} else {
					stunned = false;
				}
			}
		}
		if (parent == null) {
			Deactivate();
			return;
		}

		if (playerCapsuleTransform == null) {
			Deactivate();
			return;
		}

		if ((Vector3.Distance(transform.position,
							  playerCapsuleTransform.position)
			> playerLinkDistance)) {
			Deactivate();
			return;
		}

		if (lifetimeFinished < PauseScript.a.relativeTime) {
			Deactivate();
			return;
		}

		if (displayName) {
			if (nameText != null && linkedHM != null) {
				if (linkedHM.aic != null) nameText.text = linkedHM.aic.targetID; 
			}
		}

		if (displayHealth && linkedHM != null) {
			secondaryDisplayString = Mathf.Floor(linkedHM.health).ToString();
		} else {
			secondaryDisplayString = System.String.Empty;
		}

		if (displayRange && linkedHM != null) {
			float range = Vector3.Distance(playerCapsuleTransform.position,
										   linkedHM.transform.position);

			if (displayHealth) secondaryDisplayString += comma;
			secondaryDisplayString += (range.ToString("0.0") + rangeMetersM);
		}

		if (displayAttitude && linkedHM != null) {
			if (displayRange || displayHealth) secondaryDisplayString += comma;
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
					if (linkedHM.aic.tranquilizeFinished > PauseScript.a.relativeTime
						&& damageTimeFinished < PauseScript.a.relativeTime) {
						currentText = Const.a.stringTable[536]; // STUNNED
					} else {
						if (damageTimeFinished < PauseScript.a.relativeTime) {
							currentText = "";
							if (!Inventory.a.hasHardware[4]
								&& (currentText != Const.a.stringTable[511])) {

								Deactivate();
								return;
							}
						}
					}
				}
			}
			text.text = currentText;
		}
    }

	public static float GetTargetIDSensingRange(bool manual) {
		float sensingRange = 12f;
		if (manual) {
			// Get manual lockon distance for frob raytrace.  Less than tether.
			switch (Inventory.a.hardwareVersion[4]) {
				case 1: sensingRange = 13f; break;
				case 2: sensingRange = 13f; break;
				case 3: sensingRange = 13f; break;
				case 4: sensingRange = 18f; break;
			}
		} else {
			// Get auto-lock distance.  Less than tether.
			switch (Inventory.a.hardwareVersion[4]) {
				case 1: sensingRange = 0f; break; // No auto-lock on v1
				case 2: sensingRange = 0f; break; // No auto-lock on v2
				case 3: sensingRange = 13f; break;
				case 4: sensingRange = 20f; break;
			}
		}
		return sensingRange;
	}

	// Set to higher than the auto-lock distances above.
	public static float GetTargetIDTetherRange() {
		float dist = 15f;
		switch (Inventory.a.hardwareVersion[4]) {
			case 1: dist = 15f; break; // Set higher than manual lockons.
			case 2: dist = 15f; break; // Set higher than manual lockons.
			case 3: dist = 15f; break;
			case 4: dist = 22f; break;
		}

		return dist;
	}
}
