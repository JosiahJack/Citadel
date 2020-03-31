using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour, IBatchUpdate {
	public bool isPlayer = false;
	public bool isGrenade = false;
	public float health = -1f; // current health
	public float maxhealth; // maximum health
	public float gibhealth; // point at which we splatter
	public bool gibOnDeath = false; // used for things like crates to "gib" and shatter
	public bool dropItemsOnGib = false;
	public SearchableItem searchableItem; // not used universally
	public bool gibCorpse = false;
    public bool vaporizeCorpse = true;
	public bool isNPC = false;
	public bool isObject = false;
	public bool isScreen = false;
	public bool applyImpact = false;
	public int[] gibIndices;
	public GameObject[] gibObjects;
	public int index;
	public int levelIndex;
	public LevelManager.SecurityType securityAffected;
	[HideInInspector] public GameObject attacker;
	public Const.PoolType deathFX;
	public enum BloodType {None,Red,Yellow,Green,Robot};
	public BloodType bloodType;
	public AudioClip backupDeathSound;
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
	[HideInInspector]
	public AIController aic;
	public NPC_Hopper hopc;
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
	public SpawnManager spawnMother;  // not used universally
	public GameObject healingFXFlash;
	public bool god = false; // is this entity invincible? used for player cheat
	private DamageData tempdd;

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
		tempdd = new DamageData();

		if (isPlayer) {
			//Const.a.player1 = transform.parent.gameObject;
			if (hic == null) Debug.Log("BUG: No HardwareInvCurrent script referenced by a Player's HealthManager");
			if (hinv == null) Debug.Log("BUG: No HardwareInventory script referenced by a Player's HealthManager");
			if (ph == null) Debug.Log("BUG: No PlayerHealth script referenced by a Player's HealthManager");
		}
		take = 0;
		justHurtByEnemy = (Time.time - 31f); // set less than 30s below Time to guarantee we don't start playing action music right away, used by Music.cs
		if (securityAffected != LevelManager.SecurityType.None) LevelManager.a.RegisterSecurityObject(levelIndex, securityAffected);
		UpdateManager.Instance.RegisterSlicedUpdate(this, UpdateManager.UpdateMode.BucketA);
	}

	// Put into Start instead of Awake to give Const time to populate from enemy_tables.txt
	void Start () {
		if (isNPC) {
			aic = GetComponent<AIController>();
			hopc = GetComponent<NPC_Hopper>();
			if (aic == null) {
				if (hopc == null) {
					Debug.Log("BUG: No AIController or NPC_Hopper script on NPC!");
					return;
				}
			}
            if (aic != null) {
				index = aic.index;
			} else {
				index = hopc.index;
			}
			if (health <= 0) health = Const.a.healthForNPC[index]; //leaves possibility of setting health lower than normal, for instance the cortex reaver on level 5
			if (maxhealth <= 0) maxhealth = Const.a.healthForNPC[index]; // set maxhealth to default healthForNPC, possible to set higher, e.g. for cyborg assassins on level 9 whose health is 3 times normal

            if (Const.a.difficultyCombat == 0) {
            	maxhealth = 1;
            	health = maxhealth;
            }
        }
		if (maxhealth < 1) maxhealth = health;
	}

	public void BatchUpdate () {
		if (!initialized) {
			initialized = true;
			if (health > 0) Const.a.RegisterObjectWithHealth(this);
		}
			
		if (health > maxhealth) health = maxhealth; // Don't go past max.  Ever.
		if (actAsCorpseOnly && isNPC) {
			tempdd.ResetDamageData(tempdd);
			tempdd.damage = maxhealth * 2f;
			TakeDamage(tempdd); // harrycarry time, we's dead
			actAsCorpseOnly = false;
		}
	}

	public void NotifyEnemyNearby(GameObject enemSent) {
		if (enemSent == null) return;
		if (aic != null && isNPC && (health > 0f)) {
			aic.SendEnemy(enemSent);
		}
	}

	public float TakeDamage(DamageData dd) {
		// 5. Apply Velocity for Damage Amount
		if (applyImpact && rbody != null) {
			if (dd.impactVelocity <= 0) {
				rbody.AddForceAtPosition((dd.attacknormal*dd.damage*2f),dd.hit.point);
			} else {
				rbody.AddForceAtPosition((dd.attacknormal*dd.impactVelocity*2f),dd.hit.point);
			}
		}

		if (god) {
			Debug.Log("God mode detected. Dmg = " + dd.damage.ToString() + ", Taken = 0");
			return 0; // untouchable!
		}

		if (health <= 0) {
			Debug.Log("GameObject was dead. Dmg = " + dd.damage.ToString() + ", Taken = 0");
			return 0;
		}

		if (dd.damage <= 0) {
			Debug.Log("Dmg = " + dd.damage.ToString() + ", Taken = 0");
			return 0;
		}

		take = dd.damage;
        tempFloat = health;
		if (isPlayer) {
			// Check if player shield is active
			if (dd.attackType == Const.AttackType.Magnetic) {
				take = 0f; // don't get hurt by magnetic interactions
				ph.gameObject.GetComponent<PlayerEnergy>().TakeEnergy(11f);
			}
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

		// Apply critical based on AttackType
		if (aic != null && isNPC && (health > 0f)) {
			switch(aic.npcType) {
				case Const.npcType.Mutant:
					switch(dd.attackType) {
						case Const.AttackType.None: take *= 1f; break; // same
						case Const.AttackType.Melee: take *= 1f; break; // same
						case Const.AttackType.MeleeEnergy: take *= 1f; break; // same
						case Const.AttackType.EnergyBeam: take *= 1f; break; // same
						case Const.AttackType.Magnetic: take *= 0; break;
						case Const.AttackType.Projectile: take *= 1f; break; // same
						case Const.AttackType.ProjectileEnergyBeam: take *= 1f; break; // same
						case Const.AttackType.ProjectileLaunched: take *= 1f; break; // same
						case Const.AttackType.Gas: take *= 2f; break;
						case Const.AttackType.ProjectileNeedle: take *= 2f; break; // same
						case Const.AttackType.Tranq: take *= 1f; break; // same
					}
					break;
				case Const.npcType.Supermutant:
					switch(dd.attackType) {
						case Const.AttackType.None: take *= 1f; break; // same
						case Const.AttackType.Melee: take *= 1f; break; // same
						case Const.AttackType.MeleeEnergy: take *= 1f; break; // same
						case Const.AttackType.EnergyBeam: take *= 1f; break; // same
						case Const.AttackType.Magnetic: take *= 0; break; // no damage
						case Const.AttackType.Projectile: take *= 1f; break; // same
						case Const.AttackType.ProjectileEnergyBeam: take *= 1f; break; // same
						case Const.AttackType.ProjectileLaunched: take *= 1f; break; // same
						case Const.AttackType.Gas: take *= 1.5f; break;
						case Const.AttackType.ProjectileNeedle: take *= 1f; break; // same
						case Const.AttackType.Tranq: take *= 1f; break; // same
					}
					break;
				case Const.npcType.Robot:
					switch(dd.attackType) {
						case Const.AttackType.None: take *= 1f; break; // same
						case Const.AttackType.Melee: take *= 1f; break; // same
						case Const.AttackType.MeleeEnergy: take *= 1f; break; // same
						case Const.AttackType.EnergyBeam: take *= 1f; break; // same
						case Const.AttackType.Magnetic: take *= 4f; break; // same
						case Const.AttackType.Projectile: take *= 1f; break; // same
						case Const.AttackType.ProjectileEnergyBeam: take *= 1f; break; // same
						case Const.AttackType.ProjectileLaunched: take *= 1f; break; // same
						case Const.AttackType.Gas: take *= 0; break; // no damage
						case Const.AttackType.ProjectileNeedle: take *= 0; break; // no damage
						case Const.AttackType.Tranq: take *= 0; break; // no damage
					}
					break;
				case Const.npcType.Cyborg:
					switch(dd.attackType) {
						case Const.AttackType.None: take *= 1f; break; // same
						case Const.AttackType.Melee: take *= 1f; break; // same
						case Const.AttackType.MeleeEnergy: take *= 1f; break; // same
						case Const.AttackType.EnergyBeam: take *= 1f; break; // same
						case Const.AttackType.Magnetic: take *= 2f; break; // same
						case Const.AttackType.Projectile: take *= 1f; break; // same
						case Const.AttackType.ProjectileEnergyBeam: take *= 1f; break; // same
						case Const.AttackType.ProjectileLaunched: take *= 1f; break; // same
						case Const.AttackType.Gas: take *= 1f; break; // same
						case Const.AttackType.ProjectileNeedle: take *= 1f; break; // same
						case Const.AttackType.Tranq: take *= 1f; break; // same
					}
					break;
				case Const.npcType.Supercyborg:
					switch(dd.attackType) {
						case Const.AttackType.None: take *= 1f; break; // same
						case Const.AttackType.Melee: take *= 1f; break; // same
						case Const.AttackType.MeleeEnergy: take *= 1f; break; // same
						case Const.AttackType.EnergyBeam: take *= 1f; break; // same
						case Const.AttackType.Magnetic: take *= 2f; break; // same
						case Const.AttackType.Projectile: take *= 1f; break; // same
						case Const.AttackType.ProjectileEnergyBeam: take *= 1f; break; // same
						case Const.AttackType.ProjectileLaunched: take *= 1f; break; // same
						case Const.AttackType.Gas: take *= 0; break;
						case Const.AttackType.ProjectileNeedle: take *= 0; break;
						case Const.AttackType.Tranq: take *= 0; break;
					}
					break;
				case Const.npcType.MutantCyborg:
					switch(dd.attackType) {
						case Const.AttackType.None: take *= 1f; break; // same
						case Const.AttackType.Melee: take *= 1f; break; // same
						case Const.AttackType.MeleeEnergy: take *= 1f; break; // same
						case Const.AttackType.EnergyBeam: take *= 1f; break; // same
						case Const.AttackType.Magnetic: take *= 0.5f; break; // same
						case Const.AttackType.Projectile: take *= 1f; break; // same
						case Const.AttackType.ProjectileEnergyBeam: take *= 1f; break; // same
						case Const.AttackType.ProjectileLaunched: take *= 1f; break; // same
						case Const.AttackType.Gas: take *= 2f; break; // same
						case Const.AttackType.ProjectileNeedle: take *= 2f; break; // same
						case Const.AttackType.Tranq: take *= 1.5f; break; // same
					}
					break;
				//case Const.npcType.Cyber:    assumed to be AttackType.None
			}
		} else {
			if (hopc != null && isNPC && health > 0f) {
				// check against hopper (same as robot)
				switch(dd.attackType) {
					case Const.AttackType.None: take *= 1f; break; // same
					case Const.AttackType.Melee: take *= 1f; break; // same
					case Const.AttackType.MeleeEnergy: take *= 1f; break; // same
					case Const.AttackType.EnergyBeam: take *= 1f; break; // same
					case Const.AttackType.Magnetic: take *= 4f; break; // same
					case Const.AttackType.Projectile: take *= 1f; break; // same
					case Const.AttackType.ProjectileEnergyBeam: take *= 1f; break; // same
					case Const.AttackType.ProjectileLaunched: take *= 1f; break; // same
					case Const.AttackType.Gas: take *= 0; break; // no damage
					case Const.AttackType.ProjectileNeedle: take *= 0; break; // no damage
					case Const.AttackType.Tranq: take *= 0; break; // no damage
				}
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
		} else {
			if (hopc != null && isNPC && health > 0f) {
				//hopc.goIntoPain = true;
				//hopc.SendEnemy(attacker);  // UPDATE does this need to be here?
				for (int ij=0;ij<Const.a.healthObjectsRegistration.Length;ij++) {
					if (Const.a.healthObjectsRegistration[ij] != null) {
						if (Const.a.healthObjectsRegistration[ij].isNPC) {
							if (Vector3.Distance(Const.a.healthObjectsRegistration[ij].gameObject.transform.position,gameObject.transform.position) < 10f) {
								Const.a.healthObjectsRegistration[ij].NotifyEnemyNearby(attacker);
								//Debug.Log("Hopper took pain and then notified a nearby enemy to join the fray!");
							}
						}
					}
				}
			}
		}

        if (health <= 0f) {
            if (!deathDone) {
				// use targets targetOnDeath
				if (!string.IsNullOrWhiteSpace(targetOnDeath)) {
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
				if (isGrenade) GrenadeDeath();
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
		return take;
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
						tempAud.SetActive(true);
						AudioSource aS = tempAud.GetComponent<AudioSource> ();
						if (aS != null && deathSound != null) aS.PlayOneShot (deathSound);
					}
				} else {
					//GameObject tempAud = GameObject.Find ("TemporaryAudio");
					GameObject tempAud = Const.a.GetObjectFromPool(Const.PoolType.TempAudioSources);
					if (tempAud != null && deathSound != null) {
						tempAud.transform.position = transform.position;
						tempAud.SetActive(true);
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

		if (rbody != null) rbody.useGravity = false;
		
		if (dropItemsOnGib) {
			if (searchableItem != null) {
				GameObject levelDynamicContainer = LevelManager.a.GetCurrentLevelDynamicContainer();
				//if (levelDynamicContainer == null) levelDynamicContainer = gameObject;
				
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
						if (levelDynamicContainer != null) tossObject.transform.SetParent(levelDynamicContainer.transform,true);
						//tossObject.GetComponent<Rigidbody>().velocity = transform.forward * tossForce;
						tossObject.GetComponent<UseableObjectUse>().customIndex = searchableItem.customIndex[i];
					}
				}
			}
		}
    }

	public void GrenadeDeath() {
		GrenadeActivate ga = GetComponent<GrenadeActivate>();
		if (ga == null) {
			Debug.Log("BUG: no GrenadeActivate script on a grenade while dying from HealthManager.GrenadeDeath()");
		}
		ga.Explode();
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
				tempAud.SetActive(true);
				AudioSource aS = tempAud.GetComponent<AudioSource> ();
				if (aS != null && deathSound != null) aS.PlayOneShot (deathSound);
			}
		} else {
			//GameObject tempAud = GameObject.Find ("TemporaryAudio");
			GameObject tempAud = Const.a.GetObjectFromPool(Const.PoolType.TempAudioSources);
			if (tempAud != null && deathSound != null) {
				tempAud.transform.position = transform.position;
				tempAud.SetActive(true);
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

		if (securityAffected != LevelManager.SecurityType.None)
			LevelManager.a.ReduceCurrentLevelSecurity(securityAffected);

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
						tempAud.SetActive(true);
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
