using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {
	public bool isPlayer = false;
	public float health = -1f; // current health
	public float maxhealth; // maximum health
	public float gibhealth; // point at which we splatter
	public bool gibOnDeath = false; // used for things like crates to "gib" and shatter
	public bool dropItemsOnGib = false;
	[DTValidator.Optional] public SearchableItem searchableItem; // not used universally
	public bool gibCorpse = false;
    public bool vaporizeCorpse = true;
	public bool isNPC = false;
	public bool isObject = false;
	public bool isScreen = false;
	public bool applyImpact = false;
	public int[] gibIndices;
	public GameObject[] gibObjects;
	public int index;
	public int securityAmount;
	[HideInInspector] public GameObject attacker;
	public Const.PoolType deathFX;
	public enum BloodType {None,Red,Yellow,Green,Robot};
	public BloodType bloodType;
	[DTValidator.Optional] public AudioClip backupDeathSound;
    public bool debugMessages = false;
	public HardwareInvCurrent hic;
	public HardwareInventory hinv;
	public PlayerHealth ph;
	public float justHurtByEnemy;
	public PainStaticFX pstatic;
	public bool teleportOnDeath = false;
	public GameObject teleportEffect;
	public bool actAsCorpseOnly = false;

	private bool initialized = false;
	private bool deathDone = false;
	[HideInInspector,DTValidator.Optional]
	public AIController aic;
	private Rigidbody rbody;
	private MeshCollider meshCol;
	private BoxCollider boxCol;
	private SphereCollider sphereCol;
	private CapsuleCollider capCol;
    private Vector3 tempVec;
    private float tempFloat;
	private float take;
	public string targetOnDeath;
	public string argvalue;
	[DTValidator.Optional] public SpawnManager spawnMother;  // not used universally
	public GameObject healingFXFlash;
	public bool god = false; // is this entity invincible? used for player cheat

	void Awake () {
		initialized = false;
		deathDone = false;
		rbody = GetComponent<Rigidbody>();
		meshCol = GetComponent<MeshCollider>();
		boxCol = GetComponent<BoxCollider>();
		sphereCol = GetComponent<SphereCollider>();
		capCol = GetComponent<CapsuleCollider>();
        attacker = null;
		//searchItems = GetComponent<SearchableItem>();

		if (isPlayer) {
			//Const.a.player1 = transform.parent.gameObject;
			if (hic == null) Debug.Log("BUG: No HardwareInvCurrent script referenced by a Player's HealthManager");
			if (hinv == null) Debug.Log("BUG: No HardwareInventory script referenced by a Player's HealthManager");
			if (ph == null) Debug.Log("BUG: No PlayerHealth script referenced by a Player's HealthManager");
		}
		take = 0;
		justHurtByEnemy = (Time.time - 31f); // set less than 30s below Time to guarantee we don't start playing action music right away, used by Music.cs
	}

	// Put into Start instead of Awake to give Const time to populate from enemy_tables.txt
	void Start () {
		if (isNPC) {
			aic = GetComponent<AIController>();
			if (aic == null) {
				Debug.Log("BUG: No AIController script on NPC!");
				return;
			}
            index = aic.index;
			if (health <= 0) health = Const.a.healthForNPC[index]; //leaves possibility of setting health lower than normal, for instance the cortex reaver on level 5
			if (maxhealth <= 0) maxhealth = Const.a.healthForNPC[index]; // set maxhealth to default healthForNPC, possible to set higher, e.g. for cyborg assassins on level 9 whose health is 3 times normal

            if (Const.a.difficultyCombat == 0) {
            	maxhealth = 1;
            	health = maxhealth;
            }
        }
		if (maxhealth < 1) maxhealth = health;
	}

	void Update () {
		if (!initialized) {
			initialized = true;
			if (health > 0) Const.a.RegisterObjectWithHealth(this);
		}
			
		if (health > maxhealth) health = maxhealth; // Don't go past max.  Ever.
		if (actAsCorpseOnly && isNPC) {
			DamageData dd = new DamageData();
			dd.damage = maxhealth * 2f;
			TakeDamage(dd); // harrycarry time, we's dead
			//aic. nevermind we already check health in AIController's Think() function and take care of anims
			actAsCorpseOnly = false;
		}
	}

	public void NotifyEnemyNearby(GameObject enemSent) {
		if (aic != null && isNPC && (health > 0f)) {
			aic.SendEnemy(enemSent);
		}
	}

	public void TakeDamage(DamageData dd) {
		if (applyImpact && rbody != null) {
			if (dd.impactVelocity <= 0) {
				rbody.AddForceAtPosition((dd.attacknormal*dd.damage*2f),dd.hit.point);
			} else {
				//rbody.AddForce(dd.impactVelocity*dd.attacknormal,ForceMode.Impulse); // Old, JJ changed 9/5/19 to AddForceAtPosition since it's more accurate when shooting objects
				rbody.AddForceAtPosition((dd.attacknormal*dd.impactVelocity*2f),dd.hit.point);
			}
		}

		if (god) {
			Debug.Log("God mode detected. Dmg = " + dd.damage.ToString() + ", Taken = 0");
			return; // untouchable!
		}

		if (health <= 0) {
			Debug.Log("GameObject was dead. Dmg = " + dd.damage.ToString() + ", Taken = 0");
			return;
		}

		if (dd.damage <= 0) {
			Debug.Log("Dmg = " + dd.damage.ToString() + ", Taken = 0");
			return;
		}

		take = dd.damage;
        tempFloat = health;
		if (isPlayer) {
			// Check if player shield is active

			if (hic.hardwareIsActive[5] && hinv.hasHardware[5]) {
				// Versions of shield protect against 20, 40, 75, 75%'s
				// Versions of shield thressholds are 0, 10, 15, 30...ooh what's this hang on now...Huh, turns out it absorbs all damage below the thresshold!  Cool!
				float absorb = 0;
				float thresh = 0;
				switch(hinv.hardwareVersion[5]) {
					case 0: absorb = 0.2f;
							thresh = 0;
							break;
					case 1: absorb = 0.4f;
							thresh = 10f;
							break;
					case 2: absorb = 0.75f;
							thresh = 15f;
							break;
					case 3: absorb = 0.75f;
							thresh = 30f;
							break;
				}
				if (take < thresh) {
					absorb = 1f; // ah yeah! absorb. it. all.
				}
				if (absorb > 0) {
					if (absorb < 1f) absorb = absorb + UnityEngine.Random.Range(-0.08f,0.08f); // +/- 8% variation - this was in the original I swear!  You could theoretically have 83% shielding max.
					take *= (1f-absorb); // shield doing it's thing
					ph.shieldEffect.SetActive(true); // Activate shield screen effect to indicate damage was absorbed, effect intensity determined by absorb amount
					ph.PlayerNoise.PlayOneShot(ph.ShieldClip); // Play shield absorb sound
					Const.sprint(Const.a.stringTable[208] + absorb.ToString() + Const.a.stringTable[209],dd.other);  // Shield absorbs x% damage
				}
			}
			if (take > 0) {
				ph.PlayerNoise.PlayOneShot(ph.PainSFXClip); // Play player pain noise
				// 0 = light, 1 = med, 2 = heavy
				int intensityOfPainFlash = 0;
				if (take > 15f) {
					intensityOfPainFlash = 2;
				}
				if (take > 10f) {
					intensityOfPainFlash = 1;
				}
				pstatic.Flash(intensityOfPainFlash);
			}

			if (dd.ownerIsNPC) {
				justHurtByEnemy = Time.time;
			}
		}

		// Do the damage, that's right do. your. worst!
		health -= take; //was directly dd.damage but changed since we are check for extra things in case GetDamageTakeAmount wasn't called on dd.damage beforehand (e.g. player fall damage, internal to player only, need to protect against shield, etc, JJ 9/5/19)
		attacker = dd.owner;
		
        if (aic != null && isNPC && (health > 0f)) {
			aic.goIntoPain = true;
			aic.SendEnemy(attacker);
			for (int ij=0;ij<Const.a.healthObjectsRegistration.Length;ij++) {
				if (Const.a.healthObjectsRegistration[ij] != null) {
					if (Const.a.healthObjectsRegistration[ij].isNPC) {
						if (Vector3.Distance(Const.a.healthObjectsRegistration[ij].gameObject.transform.position,gameObject.transform.position) < Const.a.healthObjectsRegistration[ij].aic.rangeToHear) {
							Const.a.healthObjectsRegistration[ij].NotifyEnemyNearby(attacker);
							//Debug.Log("Enemy took pain and then notified a nearby enemy to join the fray!");
						}
					}
				}
			}
		}

        if (health <= 0f) {
            if (!deathDone) {
				// use targets
				if (targetOnDeath != null && targetOnDeath != "" && targetOnDeath != " " && targetOnDeath != "  ") {
					UseData ud = new UseData();
					ud.owner = dd.owner;
					ud.argvalue = argvalue;
					TargetIO tio = GetComponent<TargetIO>();
					if (tio != null) {
						ud.SetBits(tio);
					} else {
						Debug.Log("BUG: no TargetIO.cs found on an object with a ButtonSwitch.cs script!  Trying to call UseTargets without parameters!");
					}
					Const.a.UseTargets(ud,targetOnDeath);
				}

                if (isObject) ObjectDeath(null);
				if (isScreen) ScreenDeath(backupDeathSound);
				if (teleportOnDeath) {
					TeleportAway();
				}

                if (isNPC) NPCDeath(null);
            } else {
                if (vaporizeCorpse && health < (0 - (maxhealth / 2))) {
                    GetComponent<MeshRenderer>().enabled = false;
                    GameObject explosionEffect = Const.a.GetObjectFromPool(Const.PoolType.Vaporize);
                    if (explosionEffect != null) {
                        explosionEffect.SetActive(true);
						if (aic != null) {
							tempVec = aic.sightPoint.transform.position;
						} else {
							tempVec = transform.position;
						}
                        explosionEffect.transform.position = tempVec; // put vaporization effect at raycast center
                    }
                }
            }
		}

		Debug.Log("Dmg = " + dd.damage.ToString() + ", Taken = " + take.ToString() + ", Health:" + health.ToString() + " at " + transform.position.x.ToString() + " " + transform.position.y.ToString() + " " + transform.position.z.ToString());
	}

	public void TeleportAway() {
		if (teleportEffect != null) {
			teleportEffect.SetActive(true);
		}
	}

	public void NPCDeath (AudioClip deathSound) {
		if (deathDone) return;

		deathDone = true;

		// Enable death effects (e.g. explosion particle effect)
		if (deathFX != Const.PoolType.None) {
			GameObject explosionEffect = Const.a.GetObjectFromPool(deathFX);
			if (explosionEffect != null) {
				explosionEffect.SetActive(true);
				explosionEffect.transform.position = transform.position;
				if (deathSound != null) {
					//GameObject tempAud = GameObject.Find ("TemporaryAudio");
					GameObject tempAud = Const.a.GetObjectFromPool(Const.PoolType.TempAudioSources);
					if (tempAud != null && deathSound != null) {
						tempAud.transform.position = transform.position;
						AudioSource aS = tempAud.GetComponent<AudioSource> ();
						if (aS != null && deathSound != null) aS.PlayOneShot (deathSound);
					}
				} else {
					//GameObject tempAud = GameObject.Find ("TemporaryAudio");
					GameObject tempAud = Const.a.GetObjectFromPool(Const.PoolType.TempAudioSources);
					if (tempAud != null && deathSound != null) {
						tempAud.transform.position = transform.position;
						AudioSource aS = tempAud.GetComponent<AudioSource> ();
						if (aS != null && deathSound != null) aS.PlayOneShot (backupDeathSound);
					}
				}
			}
		}

		if (spawnMother != null) spawnMother.SpawneeJustDied();
	}

    public void Gib() {
		if (gibObjects.Length > 0 ) {
			if (gibObjects[0] != null) {
				for (int i = 0; i < gibObjects.Length; i++) {
					gibObjects[i].SetActive(true); // turn on all the gibs to fall apart
				}
			}
		}

		if (dropItemsOnGib) {
			if (searchableItem != null) {
				GameObject levelDynamicContainer = LevelManager.a.GetCurrentLevelDynamicContainer();
				if (levelDynamicContainer == null) levelDynamicContainer = gameObject;
				for (int i=0;i<4;i++) {
					if (searchableItem.contents[i] >= 0) {
						GameObject tossObject = Instantiate(Const.a.useableItems[searchableItem.contents[i]],transform.position,Quaternion.identity) as GameObject;
						if (tossObject == null) {
							Const.sprint("BUG: Failed to instantiate object being dropped on gib.",Const.a.allPlayers);
							return;
						}

						if (tossObject.activeSelf != true) {
							tossObject.SetActive(true);
						}
						tossObject.transform.SetParent(levelDynamicContainer.transform,true);
						//tossObject.GetComponent<Rigidbody>().velocity = transform.forward * tossForce;
						tossObject.GetComponent<UseableObjectUse>().customIndex = searchableItem.customIndex[i];
					}
				}
			}
		}
    }

	public void ScreenDeath(AudioClip deathSound) {
		if (deathDone) return;

		deathDone = true;

		// Screens maintain collisions

		// Make some noise
		if (deathSound != null) {
			//GameObject tempAud = GameObject.Find ("TemporaryAudio");
			GameObject tempAud = Const.a.GetObjectFromPool(Const.PoolType.TempAudioSources);
			if (tempAud != null && deathSound != null) {
				tempAud.transform.position = transform.position;
				AudioSource aS = tempAud.GetComponent<AudioSource> ();
				if (aS != null && deathSound != null) aS.PlayOneShot (deathSound);
			}
		} else {
			//GameObject tempAud = GameObject.Find ("TemporaryAudio");
			GameObject tempAud = Const.a.GetObjectFromPool(Const.PoolType.TempAudioSources);
			if (tempAud != null && deathSound != null) {
				tempAud.transform.position = transform.position;
				AudioSource aS = tempAud.GetComponent<AudioSource> ();
				if (aS != null && deathSound != null) aS.PlayOneShot (backupDeathSound);
			}
		}

		ImageSequenceTextureArray ista = GetComponent<ImageSequenceTextureArray>();
		if (ista != null) ista.Destroy();

		// Throw some glass
		if (gibOnDeath) Gib();

		// Screens maintain visible mesh, don't turn it off
	}

	public void ObjectDeath(AudioClip deathSound) {
		if (deathDone) return;

		deathDone = true;

		// Disable collision
		if (boxCol != null) boxCol.enabled = false;
		if (meshCol != null) meshCol.enabled = false;
		if (sphereCol != null) sphereCol.enabled = false;
		if (capCol != null) capCol.enabled = false;

		if (securityAmount > 0)
			LevelManager.a.ReduceCurrentLevelSecurity (securityAmount);

		// Enabel death effects (e.g. explosion particle effect)
		if (deathFX != Const.PoolType.None) {
			GameObject explosionEffect = Const.a.GetObjectFromPool(deathFX);
			if (explosionEffect != null) {
				explosionEffect.SetActive(true);
				explosionEffect.transform.position = transform.position;
				if (deathSound != null) {
					//GameObject tempAud = GameObject.Find("TemporaryAudio");
					GameObject tempAud = Const.a.GetObjectFromPool(Const.PoolType.TempAudioSources);
					if (tempAud != null) {
						tempAud.transform.position = transform.position;
						AudioSource aS = tempAud.GetComponent<AudioSource>();
						if (aS != null) aS.PlayOneShot(deathSound);
					}
				}

			}
		}

		if (spawnMother != null) spawnMother.SpawneeJustDied();

		if (gibOnDeath) Gib();

		//gameObject.SetActive(false); // turn off the main object
		GetComponent<MeshRenderer>().enabled = false;
	}

	public void HealingBed(float amount,bool flashBed) {
		health += amount;
		if (flashBed) {
			if (healingFXFlash != null) {
				healingFXFlash.SetActive(true);
			}
		}
	}
}
