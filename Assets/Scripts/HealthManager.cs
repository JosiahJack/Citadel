using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {
	public bool isPlayer = false;
	public bool inCyberSpace = false;
	public bool isGrenade = false;
	public float health = -1f; // current health //save
	public float cyberHealth = -1f; //save
	public int cyberEntityIndex = -1;
	public float maxhealth; // maximum health
	public float gibhealth; // point at which we splatter
	public bool gibOnDeath = false; // used for things like crates to "gib" and shatter
	public bool dropItemsOnGib = false;
	public SearchableItem searchableItem; // not used universally
    public bool vaporizeCorpse = true;
	public bool isNPC = false;
	public bool isObject = false;
	public bool isIce = false;
	public bool isScreen = false;
	public bool applyImpact = false;
	public bool isSecCamera = false;
	public Image linkedCameraOverlay;
	public SecurityCameraRotate scr;
	public int[] gibIndices;
	public GameObject[] gibObjects;
	public Vector3 gibVelocityBoost;
	public bool gibsGetVelocity = false;
	public int index;
	public int levelIndex;
	public LevelManager.SecurityType securityAffected;
	[HideInInspector] public GameObject attacker;
	public Const.PoolType deathFX;
	public enum BloodType {None,Red,Yellow,Green,Robot,Leaf,Mutation,GrayMutation};
	public BloodType bloodType;
	public AudioClip backupDeathSound;
    public bool debugMessages = false;
	public HardwareInvCurrent hic;
	public HardwareInventory hinv;
	public SoftwareInventory sinv;
	public PlayerHealth ph;
	public float justHurtByEnemy;
	public PainStaticFX pstatic;
	public PainStaticFX empstatic;
	public bool teleportOnDeath = false;
	public GameObject teleportEffect;
	public bool actAsCorpseOnly = false;
	[HideInInspector]
	public bool deathDone = false;
	[HideInInspector]
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
	public SpawnManager spawnMother;  // not used universally
	public GameObject healingFXFlash;
	public bool god = false; // is this entity invincible? used for player cheat
	private DamageData tempdd;
	private Rigidbody gibrbody;
	[HideInInspector]
	public PlayerEnergy pe;
	[HideInInspector]
	public bool teleportDone;
	public TargetID linkedTargetID;
	[HideInInspector]
	public bool awakeInitialized = false;
	[HideInInspector]
	public bool startInitialized = false;

	public void Awake () {
		deathDone = false;
		teleportDone = false;
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
			pe = GetComponent<PlayerEnergy>();
			if (hic == null) Debug.Log("BUG: No HardwareInvCurrent script referenced by a Player's HealthManager");
			if (hinv == null) Debug.Log("BUG: No HardwareInventory script referenced by a Player's HealthManager");
			if (ph == null) Debug.Log("BUG: No PlayerHealth script referenced by a Player's HealthManager");
			if (pe == null) Debug.Log("BUG: No PlayerEnergy script referenced by a Player's HealthManager");
		}
		take = 0;
		if (isNPC) justHurtByEnemy = (Time.time - 31f); // set less than 30s below Time to guarantee we don't start playing action music right away, used by Music.cs
		if (securityAffected != LevelManager.SecurityType.None) LevelManager.a.RegisterSecurityObject(levelIndex, securityAffected);
		Const.a.RegisterObjectWithHealth(this);
		if (gibOnDeath) {
			for (int i=0;i<gibObjects.Length;i++) {
				SaveObject so = gibObjects[i].GetComponent<SaveObject>();
				if (so != null) so.Start();
			}
		}
		awakeInitialized = true;
	}

	// Put into Start instead of Awake to give Const time to populate from enemy_tables.txt
	public void Start () {
		if (isNPC) {
			aic = GetComponent<AIController>();
			if (aic == null) {
				Debug.Log("BUG: No AIController script on NPC at + " + transform.position.ToString());
				startInitialized = true;
				return;
			} else {
				index = aic.index;
			}

			if (cyberEntityIndex >= 0 && cyberEntityIndex <= 4) {
				if (cyberHealth <= 0) cyberHealth = Const.a.healthForCyberNPC[cyberEntityIndex];
				if (maxhealth <= 0) maxhealth = Const.a.healthForCyberNPC[cyberEntityIndex];
			} else {
				if (health <= 0) health = Const.a.healthForNPC[index]; //leaves possibility of setting health lower than normal, for instance the cortex reaver on level 5
				if (maxhealth <= 0) maxhealth = Const.a.healthForNPC[index]; // set maxhealth to default healthForNPC, possible to set higher, e.g. for cyborg assassins on level 9 whose health is 3 times normal
			}

            if (Const.a.difficultyCombat == 0) {
            	maxhealth = 1;
            	health = maxhealth;
            }
        }
		if (maxhealth < 1) maxhealth = health;
		if (actAsCorpseOnly && isNPC && health > 0) InitializeCorpseOnly();
		startInitialized = true;
	}

	void InitializeCorpseOnly() {
		health = 0;
		cyberHealth = 0;
		if (aic != null) { if (aic.SFX != null) aic.SFX.enabled = false; }
		if (!deathDone) {
			// use targets targetOnDeath
			if (!string.IsNullOrWhiteSpace(targetOnDeath)) {
				UseData ud = new UseData();
				ud.argvalue = argvalue;
				TargetIO tio = GetComponent<TargetIO>();
				if (tio != null) {
					ud.SetBits(tio);
				} else {
					Debug.Log("BUG: no TargetIO.cs found on an object with a ButtonSwitch.cs script!  Trying to call UseTargets without parameters!");
				}
				Const.a.UseTargets(ud,targetOnDeath);
			}

			if (teleportOnDeath) {
				TeleportAway();
			}

			NPCDeath(null);
		}
	}

	float ApplyAttackTypeAdjustments(float take,DamageData dd) {
		if (aic != null && isNPC && health > 0f) {
			if (aic.npcType == Const.npcType.Mutant) {
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
			}

			if (aic.npcType == Const.npcType.Supermutant) {
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
			}

			if (aic.npcType == Const.npcType.Robot) {
				switch(dd.attackType) {
					case Const.AttackType.None: take *= 1f; break; // same
					case Const.AttackType.Melee: take *= 1f; break; // same
					case Const.AttackType.MeleeEnergy: take *= 1f; break; // same
					case Const.AttackType.EnergyBeam: take *= 1f; break; // same
					case Const.AttackType.Magnetic: take *= 4f; break; // same
					case Const.AttackType.Projectile: take *= 1f; break; // same
					case Const.AttackType.ProjectileEnergyBeam: take *= 1f; break; // same
					case Const.AttackType.ProjectileLaunched: take *= 1f; break; // same
					case Const.AttackType.Gas: take = 0; break; // no damage
					case Const.AttackType.ProjectileNeedle: take = 0; break; // no damage
					case Const.AttackType.Tranq: take *= 1f; break; // no damage
				}
			}

			if (aic.npcType == Const.npcType.Cyborg) {
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
			}

			if (aic.npcType == Const.npcType.Supercyborg) {
				switch(dd.attackType) {
					case Const.AttackType.None: take *= 1f; break; // same
					case Const.AttackType.Melee: take *= 1f; break; // same
					case Const.AttackType.MeleeEnergy: take *= 1f; break; // same
					case Const.AttackType.EnergyBeam: take *= 1f; break; // same
					case Const.AttackType.Magnetic: take *= 2f; break; // same
					case Const.AttackType.Projectile: take *= 1f; break; // same
					case Const.AttackType.ProjectileEnergyBeam: take *= 1f; break; // same
					case Const.AttackType.ProjectileLaunched: take *= 1f; break; // same
					case Const.AttackType.Gas: take = 0; break;
					case Const.AttackType.ProjectileNeedle: take = 0; break;
					case Const.AttackType.Tranq: take *= 1f; break;
				}
			}

			if (aic.npcType == Const.npcType.MutantCyborg) {
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
			}

			if (aic.npcType == Const.npcType.Cyber) {
				switch(dd.attackType) {
					case Const.AttackType.None: take *= 1f; break; // same
					case Const.AttackType.Melee: take *= 1f; break; // same
					case Const.AttackType.MeleeEnergy: take *= 1f; break; // same
					case Const.AttackType.EnergyBeam: take *= 1f; break; // same
					case Const.AttackType.Magnetic: take *= 1f; break; // same
					case Const.AttackType.Projectile: take *= 1f; break; // same
					case Const.AttackType.ProjectileEnergyBeam: take *= 1f; break; // same
					case Const.AttackType.ProjectileLaunched: take *= 1f; break; // same
					case Const.AttackType.Gas: take *= 1f; break; // same
					case Const.AttackType.ProjectileNeedle: take *= 1f; break; // same
					case Const.AttackType.Tranq: take *= 1f; break; // same
					case Const.AttackType.Drill: take = 0f; break; // same
				}
			}
		}
		return take;
	}

	public float TakeDamage(DamageData dd) {
		//Debug.Log("Entered takeDamage with damage: " + dd.damage.ToString());
		if (dd == null) return 0;
		// 5. Apply Velocity for Damage Amount
		if (applyImpact && rbody != null) {
			if (dd.impactVelocity <= 0) {
				//rbody.AddForceAtPosition((dd.attacknormal*dd.damage*3f),dd.hit.point);
			} else {
				rbody.AddForceAtPosition((dd.attacknormal*dd.impactVelocity*1.5f),dd.hit.point); // impacts were too weak, multiplying by 1.5f to increase impact effect
			}
		}

		if (dd.damage <= 0) {
			//Debug.Log("Dmg = " + dd.damage.ToString() + ", Taken = 0");
			return 0; // ah!! scaryy!! cannot divide by 0, let's get out of here!
		}

		if (god) {
			//Debug.Log("God mode detected. Dmg = " + dd.damage.ToString() + ", Taken = 0");
			return 0; // untouchable!
		}

        tempFloat = health;
		if (inCyberSpace || cyberEntityIndex >= 0) {
			tempFloat = cyberHealth;
			if (dd.attackType == Const.AttackType.Drill && isNPC) return 0; // Drill can't hurt NPC's
			if (dd.attackType != Const.AttackType.Drill && isIce) return 0; // Pulser can't hurt Ice
		}

		if (tempFloat <= 0 && !isObject) {
			//Debug.Log("GameObject was dead. Dmg = " + dd.damage.ToString() + ", Taken = 0");
			return 0;
		}

		take = dd.damage;
		if (isPlayer) {
			float absorb = 0;
			if (inCyberSpace) {
				if (sinv.hasSoft[2]) {
					switch(sinv.softVersions[2]) {
						case 0: absorb = 0f;
								break;
						case 1: absorb = 0.10f;
								break;
						case 2: absorb = 0.15f;
								break;
						case 3: absorb = 0.20f;
								break;
						case 4: absorb = 0.25f;
								break;
						case 5: absorb = 0.30f;
								break;
						case 6: absorb = 0.35f;
								break;
						case 7: absorb = 0.40f;
								break;
						case 8: absorb = 0.45f;
								break;
						case 9: absorb = 0.50f;
								break;
					}
					take = (take * (1f - absorb)); // absorb percentage from above table
					if (take <= 0f) return 0f; // nothing to see here
				}
			} else {
				// Check if player shield is active
				if (dd.attackType == Const.AttackType.Magnetic) {
					take = 0f; // don't get hurt by magnetic interactions
					empstatic.Flash(2);
					pe.TakeEnergy(11f);
				}
				if (hic.hardwareIsActive[5] && hinv.hasHardware[5]) {
					// Versions of shield protect against 20, 40, 75, 75%'s
					// Versions of shield thressholds are 0, 10, 15, 30...ooh what's this hang on now...Huh, turns out it absorbs all damage below the thresshold!  Cool!
					float thresh = 0;
					float enertake = 0;
					switch(hinv.hardwareVersion[5]) {
						case 0: absorb = 0.2f;
								thresh = 0;
								enertake = 24f;
								break;
						case 1: absorb = 0.4f;
								thresh = 10f;
								enertake = 60f;
								break;
						case 2: absorb = 0.75f;
								thresh = 15f;
								enertake = 105f;
								break;
						case 3: absorb = 0.75f;
								thresh = 30f;
								enertake = 30f;
								break;
					}
					if (take < thresh) {
						absorb = 1f; // ah yeah! absorb. it. all.
					}
					if (absorb > 0) {
						if (absorb < 1f) absorb = absorb + UnityEngine.Random.Range(-0.08f,0.08f); // +/- 8% variation - this was in the original I swear!  You could theoretically have 83% shielding max.
						if (absorb > 1f) absorb = 1f; // cap it at 100%....shouldn't really ever be here, nothing is 92% + 8%
						take *= (1f-absorb); // shield doing it's thing
						ph.shieldEffect.SetActive(true); // Activate shield screen effect to indicate damage was absorbed, effect intensity determined by absorb amount
						ph.PlayerNoise.PlayOneShot(ph.ShieldClip); // Play shield absorb sound
						int abs = (int)(absorb * 100f); //  for int display of absorbption percent
						Const.sprint(Const.a.stringTable[208] + abs.ToString() + Const.a.stringTable[209],dd.other);  // Shield absorbs x% damage
						//float shieldPercentAbsorbed = take/dd.damage;
						//if (shieldPercentAbsorbed > 1f) shieldPercentAbsorbed = 1f;
						//if (shieldPercentAbsorbed > 0) pe.TakeEnergy(enertake*shieldPercentAbsorbed);
						if (absorb > 0) {
							pe.TakeEnergy(enertake*absorb);
							pe.drainJPM += (int) enertake;
							pe.tickFinished = PauseScript.a.relativeTime + 0.1f; // So blip registers on biomonitor graph
						}
					}
				}
				if (take > 0 && ((absorb <0.4f) || Random.Range(0,1f) < 0.5f)) {
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
					justHurtByEnemy = PauseScript.a.relativeTime;
				}
			}
		}

		// Do the damage, that's right do. your. worst!
		if (inCyberSpace || cyberEntityIndex >= 0) {
			cyberHealth -= take;
			if (isPlayer) {
				ph.playerCyberHealthTicks.DrawTicks();
				if (cyberHealth <= 0) {
					MFDManager.a.playerMLook.ExitCyberspace();
					return 0f;
				}
			}
		} else {
			// Apply critical based on AttackType
			take = ApplyAttackTypeAdjustments(take,dd);

			health -= take; //was directly dd.damage but changed since we are check for extra things in case GetDamageTakeAmount wasn't called on dd.damage beforehand (e.g. player fall damage, internal to player only, need to protect against shield, etc, JJ 9/5/19)
			if (isPlayer) {
				ph.playerHealthTicks.DrawTicks();
				Music.a.inCombat = true;
			}
		}
		attacker = dd.owner;
		if (isNPC && (health > 0f || (cyberEntityIndex >= 0f && cyberHealth > 0f))) {
			AIController aic = GetComponent<AIController>();
			if (aic != null) {
				aic.goIntoPain = true;
				aic.attacker = attacker;
				if (linkedTargetID != null) linkedTargetID.SendDamageReceive(take);
				bool goPain = aic.CheckPain(); // setup enemy with NPC
			}
		}

        if (health <= 0f || (inCyberSpace && cyberHealth <= 0f)) {
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

                if (isObject) {
					if (vaporizeCorpse && dd.attackType == Const.AttackType.EnergyBeam && !isSecCamera)
						deathFX = Const.PoolType.Vaporize;
					ObjectDeath(null);

					if (searchableItem != null) MFDManager.a.NotifySearchThatSearchableWasDestroyed();
				}

				if (isScreen) ScreenDeath(backupDeathSound);
				if (teleportOnDeath) {
					TeleportAway();
				}

                if (isNPC) NPCDeath(null);
				if (isGrenade) GrenadeDeath();
            } else {
                if (vaporizeCorpse && (health < (0 - (maxhealth / 2))) && dd.attackType == Const.AttackType.EnergyBeam && !isSecCamera) {
					MeshRenderer mr = GetComponent<MeshRenderer>();
                    if (mr != null) mr.enabled = false;
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

					if (searchableItem != null) {
						MFDManager.a.NotifySearchThatSearchableWasDestroyed();
						GameObject levelDynamicContainer = LevelManager.a.GetCurrentLevelDynamicContainer();
						//if (levelDynamicContainer == null) levelDynamicContainer = gameObject;
						
						for (int i=0;i<4;i++) {
							if (searchableItem.contents[i] >= 0) {
								GameObject tossObject = Instantiate(Const.a.useableItems[searchableItem.contents[i]],transform.position,Quaternion.identity) as GameObject;
								if (tossObject != null) {
									if (tossObject.activeSelf != true) {
										tossObject.SetActive(true);
									}
									if (levelDynamicContainer != null) {
										tossObject.transform.SetParent(levelDynamicContainer.transform,true);
										SaveObject so = tossObject.GetComponent<SaveObject>();
										if (so != null) so.levelParentID = LevelManager.a.currentLevel;
									}
									//tossObject.GetComponent<Rigidbody>().velocity = transform.forward * tossForce;
									tossObject.GetComponent<UseableObjectUse>().customIndex = searchableItem.customIndex[i];
								} else {
									Const.sprint("BUG: Failed to instantiate object being dropped on gib.",Const.a.allPlayers);
								}
							}
						}
					}
                }
            }
		}

		//if (!actAsCorpseOnly) Debug.Log("Dmg = " + dd.damage.ToString() + ", Taken = " + take.ToString() + ", Health:" + health.ToString() + " at " + transform.position.x.ToString() + " " + transform.position.y.ToString() + " " + transform.position.z.ToString());
		return take;
	}

	public void TeleportAway() {
		if (teleportEffect != null && !teleportDone) {
			teleportDone = true;
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
						if (aS != null) aS.enabled = true;
						if (aS != null && deathSound != null) aS.PlayOneShot (deathSound);
					}
				} else {
					//GameObject tempAud = GameObject.Find ("TemporaryAudio");
					GameObject tempAud = Const.a.GetObjectFromPool(Const.PoolType.TempAudioSources);
					if (tempAud != null && !actAsCorpseOnly) {
						tempAud.transform.position = transform.position;
						tempAud.SetActive(true);
						AudioSource aS = tempAud.GetComponent<AudioSource> ();
						if (aS != null) aS.enabled = true;
						if (deathSound == null) deathSound = backupDeathSound;
						if (aS != null && deathSound != null && aS.enabled && gameObject.activeInHierarchy) aS.PlayOneShot (deathSound);
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
					if (gibsGetVelocity) {
						gibrbody = gibObjects[i].GetComponent<Rigidbody>();
						if (gibrbody != null) {
							gibrbody.AddForce(gibVelocityBoost,ForceMode.Impulse);
						}
					}
				}
			}
		}

		if (rbody != null) rbody.useGravity = false;
		
		if (dropItemsOnGib) {
			if (searchableItem != null) {
				MFDManager.a.NotifySearchThatSearchableWasDestroyed();
				GameObject levelDynamicContainer = LevelManager.a.GetCurrentLevelDynamicContainer();
				//if (levelDynamicContainer == null) levelDynamicContainer = gameObject;
				
				for (int i=0;i<4;i++) {
					if (searchableItem.contents[i] >= 0) {
						GameObject tossObject = Instantiate(Const.a.useableItems[searchableItem.contents[i]],transform.position,Quaternion.identity) as GameObject;
						if (tossObject != null) {
							if (tossObject.activeSelf != true) {
								tossObject.SetActive(true);
							}
							if (levelDynamicContainer != null) {
								tossObject.transform.SetParent(levelDynamicContainer.transform,true);
								SaveObject so = tossObject.GetComponent<SaveObject>();
								if (so != null) so.levelParentID = LevelManager.a.currentLevel;
							}
							//tossObject.GetComponent<Rigidbody>().velocity = transform.forward * tossForce;
							tossObject.GetComponent<UseableObjectUse>().customIndex = searchableItem.customIndex[i];
						} else {
							Const.sprint("BUG: Failed to instantiate object being dropped on gib.",Const.a.allPlayers);
						}
						searchableItem.contents[i] = -1;
						searchableItem.customIndex[i] = -1;
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

		if (isSecCamera) {
			//if (scr != null) scr.enabled = false;
			if (linkedCameraOverlay != null) linkedCameraOverlay.enabled = false; // disable on automap
		}
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
		MeshRenderer mr = GetComponent<MeshRenderer>();
		if (mr != null) {
			mr.enabled = false;
		} else {
			AIController aicP = GetComponentInParent<AIController>();
			if (aicP != null) {
				if (aicP.healthManager != null) {
					if (aicP.healthManager.gibOnDeath) {
						if (aicP.healthManager.gibObjects.Length > 0 ) {
							if (aicP.healthManager.gibObjects[0] != null) {
								for (int i = 0; i < aicP.healthManager.gibObjects.Length; i++) {
									aicP.healthManager.gibObjects[i].SetActive(false); // turn off all the gibs to fall apart
								}
							}
						}
					} else {
						aicP.visibleMeshEntity.SetActive(false);
					}
				}
			}
		}
	}

	public void HealingBed(float amount,bool flashBed) {
		health += amount;
		if (health > 255) health = 255;
		ph.playerHealthTicks.DrawTicks();
		if (flashBed) {
			if (healingFXFlash != null) {
				healingFXFlash.SetActive(true);
			}
		}
	}

	public void AwakeFromLoad() {
		if (!awakeInitialized) { Awake(); }
		if (!startInitialized) { Start(); }
		//if (!awakeInitialized || !startInitialized) return;

		// Already done this in Load:
		// hm.health = GetFloatFromString(entries[index],currentline); index++; // how much health we have
		// hm.deathDone = GetBoolFromString(entries[index]); index++; // bool - are we dead yet?
		// hm.god = GetBoolFromString(entries[index]); index++; // are we invincible? - we can save cheats?? OH WOW!
		// hm.teleportDone = GetBoolFromString(entries[index]); index++; // did we already teleport?

		// Handle objects
		if (isObject) {
			int gibcnt = gibObjects.Length;
			if (gibcnt > 0) {
				if (gibObjects[0] != null) {
					for (int i=0;i<gibcnt;i++) {
						if (health > 0) {
							if (gibObjects[i].activeSelf) gibObjects[i].SetActive(false);
						} else {
							if (!gibObjects[i].activeSelf) gibObjects[i].SetActive(true);
						}
					}
				}
			}
			if (health > 0) {
				if (boxCol != null) boxCol.enabled = true;
				if (meshCol != null) meshCol.enabled = true;
				if (sphereCol != null) sphereCol.enabled = true;
				if (capCol != null) capCol.enabled = true;
				if (isSecCamera) {
					if (linkedCameraOverlay != null) linkedCameraOverlay.enabled = true; // disable on automap
				}
				MeshRenderer mr = GetComponent<MeshRenderer>();
				if (mr != null) {
					mr.enabled = true;
				} else {
					AIController aicP = GetComponentInParent<AIController>();
					if (aicP != null) {
						if (aicP.healthManager != null) {
							if (!aicP.healthManager.gibOnDeath) {
								aicP.visibleMeshEntity.SetActive(true);
							}
						}
					}
				}
			} else {
				// Disable collision
				if (boxCol != null) boxCol.enabled = false;
				if (meshCol != null) meshCol.enabled = false;
				if (sphereCol != null) sphereCol.enabled = false;
				if (capCol != null) capCol.enabled = false;

				if (isSecCamera) {
					if (linkedCameraOverlay != null) linkedCameraOverlay.enabled = false; // disable on automap
				}

				MeshRenderer mr = GetComponent<MeshRenderer>();
				if (mr != null) {
					mr.enabled = false;
				} else {
					AIController aicP = GetComponentInParent<AIController>();
					if (aicP != null) {
						if (aicP.healthManager != null) {
							if (!aicP.healthManager.gibOnDeath) {
								aicP.visibleMeshEntity.SetActive(false);
							}
						}
					}
				}
			}
		}
		// end Handle objects

		// Handle screens
		if (isScreen) {
			if (health > 0) {
				ImageSequenceTextureArray ista = GetComponent<ImageSequenceTextureArray>();
				if (ista != null) {
					ista.AwakeFromLoad(health);
				}
			}
		}
		// end Handle screens

		// Handle NPCs
		if (isNPC) {
			if (spawnMother != null) spawnMother.AwakeFromLoad(health);
		}
		// end Handle NPCs

		// Handle grenades
		if (isGrenade) {
			GrenadeActivate ga = GetComponent<GrenadeActivate>();
			if (ga != null) ga.AwakeFromLoad(health);
		}
		// end Handle grenades
	}
}
