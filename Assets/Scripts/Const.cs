using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Const : MonoBehaviour {
	[SerializeField] public GameObject[] useableItems;
	[SerializeField] public Texture2D[] useableItemsFrobIcons;
    [SerializeField] public Sprite[] useableItemsIcons;
    [SerializeField] public string[] useableItemsNameText;
	[SerializeField] public Sprite[] searchItemIconSprites;
	[SerializeField] public string[] audiologNames;
	[SerializeField] public AudioClip[] audioLogs;
	[SerializeField] public bool[] audioLogHasAudio;
	[SerializeField] public string[] audioLogSpeech2Text;
	public float doubleClickTime = 0.500f;
	public float frobDistance = 5f;
	public enum PoolType{DartImpacts};
	public enum AttackType{None,Melee,EnergyBeam,Magnetic,Projectile,ProjectileEnergyBeam};
	public GameObject Pool_DartImpacts;
	public GameObject statusBar;
    public static Const a;

	// Instantiate it so that it can be accessed globally. MOST IMPORTANT PART!!
	// =========================================================================
	void Awake() {a = this; }
	// =========================================================================

	// Check if particular bit is 1 (ON/TRUE) in binary format of given integer
	public bool CheckFlags (int checkInt, int flag) {
		if ((checkInt & flag) != 0)
			return true;

		return false;
	}

	// StatusBar Print
	public static void sprint (string input) {
		print(input);  // print to console
		if (a != null) {
			if (a.statusBar != null)
				a.statusBar.GetComponent<StatusBarTextDecay>().SendText(input);
		}
	}

	public GameObject GetObjectFromPool(PoolType pool) {
		if (Pool_DartImpacts == null) {
			sprint("Cannot find pool of type PoolType.DartImpacts");
			return null;
		}

		switch (pool) {
		case PoolType.DartImpacts: 
			for (int i=0;i<Pool_DartImpacts.transform.childCount;i++) {
				Transform child = Pool_DartImpacts.transform.GetChild(i);
				if (child.gameObject.activeInHierarchy == false) {
					//sprint("Found a DartImpact!");
					return child.gameObject;
				}
			}
			sprint("Warning: No free objects in DartImpacts pool");
			return null;
		}

		return null;
	}

	// ========================DAMAGE SYSTEM===========================
	// 0. First checks against whether the entity is damageable (i.e. not the world)
	// 1. Armor absorption (see ICE Breaker Guide for all of 4 these)
	// 2. Weapon vulnerabilities based on attack type and the a_att_type bits stored in the npc
	// 3. Critical hits, chance for critical hit damage based on defense and offense of attack and target
	// 4. Random Factor, +/- 10% damage for randomness
	// 5. Apply velocity for damage, this is after all the above because otherwise the damage multipliers wouldn't affect velocity
	// 6. Return the damage to original TakeDamage() function

	public float GetDamageTakeAmount (float a_damage, float a_offense, float a_penetration, AttackType a_att_type, bool a_notclient, float o_armorvalue, float o_defense) {
		float take = 0f;
		float chance = 0f;
		float f = 0f;
		// 1. Armor Absorption
		if (o_armorvalue > a_penetration) {
			take = (a_damage - a_penetration);
		} else {
			take = a_damage;
		}

		// 2. Weapon Vulnerabilities
		if (a_att_type != AttackType.None) {

		}

		// 3. Critical Hits (NPCs only)
		if (a_notclient) {
			f = (a_offense - o_defense);
			float crit = f;
			if (f > 0) {
				// 71% success with 5/6  5 = f, 6 = max offense or defense value
				// 62% success with 4/6
				// 50% success with 3/6
				// 24% success with 2/6
				// 10% success with 1/6
				chance = (f/6); // 5/6|4/6|3/6|2/6|1/6 = .833|.666|.5|.333|.166
				if (f == 1)
					chance = 0.280f; //anything less than 0.25, 0.1666 in this case taken from 1/6, will fail

				chance = (chance * Random.Range(0f,1f) * 2);
				if (chance > 0.5f) {
					// SUCCESS! Applying critical hit.
					crit = (take * f);  //How many extra damages we add to damage that we will take
					take = (take + crit); // Maximum extra is 5X + 1X Damage
				}
			}
		}

		// 4. Random Factor +/- 10% (aka 0.10 damage)
		chance = (0.1f * Random.Range(0f,1f));

		// 50% chance of being positive or negative
		f = Random.Range(0f,1f);
		if (f > 0.5f) {
			chance = (chance * (-1)); // Make negative
		}
		chance = (chance * take);
		take = (take + chance); // Add the random factor, anywhere up to +/- 10%
		if (take <= 0f)
			return take;

		// 5. Apply Velocity for Damage Amount

		// 6. Return the Damage 
		return take;
	}
}
