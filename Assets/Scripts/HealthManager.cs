using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {
	// External references, required

	// External references, optional
	/*[DTValidator.Optional] */public SearchableItem searchableItem; // Not used universally.  Some objects can be destroyed but not searched, such as barrels.
	/*[DTValidator.Optional] */public Image linkedOverlay;
	public SecurityType securityAffected; // Not a reference, needs no optional flag, if using DTValidator that is.
	/*[DTValidator.Optional] */public AudioClip backupDeathSound;
	/*[DTValidator.Optional] */public GameObject teleportEffect;
	/*[DTValidator.Optional] */public GameObject[] gibObjects;
	/*[DTValidator.Optional] */public GameObject[] disableOnGib;

	// External references, optional...Player references only
	/*[DTValidator.Optional] */public PainStaticFX pstatic;
	/*[DTValidator.Optional] */public PainStaticFX empstatic;
	/*[DTValidator.Optional] */public GameObject healingFXFlash;

	// Externally set values in inspector per instance
	public float health = -1f; // save, Current health, set in inspector for a different starting health than default on enemies.
	public float cyberHealth = -1f; //save
	public float maxhealth; // maximum health
	private int index; // NPC Index
	public int levelIndex; // Only for if a security camera.
	public bool isPlayer = false;
	public bool isGrenade = false;
	public bool gibOnDeath = false; // used for things like crates to "gib" and shatter
	public bool dropItemsOnGib = false;
    public bool vaporizeCorpse = true;
	public bool isNPC = false;
	public bool isObject = false;
	public bool isIce = false;
	public bool isScreen = false;
	public bool isSecCamera = false;
	public bool teleportOnDeath = false;
	public bool actAsCorpseOnly = false;
	public bool gibsGetVelocity = false;
	public Vector3 gibVelocityBoost;
	public PoolType deathFX;
	public BloodType bloodType;
	public string targetOnDeath;
	public string argvalue;
	public bool inCyberSpace = false; // Externally modifiable

	// Internal references
	[HideInInspector] public GameObject attacker;
	[HideInInspector] public float justHurtByEnemy;
	[HideInInspector] public bool deathDone = false;
	[HideInInspector] public AIController aic;
	private Rigidbody rbody;
    private float tempFloat;
	private float take;
	[HideInInspector] public SpawnManager spawnMother;  // not used universally
	[HideInInspector] public bool god = false; // is this entity invincible? used for player cheat
	private DamageData tempdd;
	private Rigidbody gibrbody;
	[HideInInspector] public bool teleportDone;
	[HideInInspector] public TargetID linkedTargetID;
	[HideInInspector] public bool awakeInitialized = false;
	[HideInInspector] public bool startInitialized = false;

	public void Awake () {
		if (awakeInitialized) return;

		deathDone = false;
		teleportDone = false;
		rbody = GetComponent<Rigidbody>();
        attacker = null;
		tempdd = new DamageData();
		take = 0;
		if (isPlayer) {
			health = 211;
			cyberHealth = 255;
			maxhealth = 255;
		}
		if (isPlayer) justHurtByEnemy = (Time.time - 31f); // set less than 30s below Time to guarantee we don't start playing action music right away, used by Music.cs
		if (securityAffected != SecurityType.None && LevelManager.a != null) LevelManager.a.RegisterSecurityObject(levelIndex, securityAffected);
		if (Const.a != null) Const.a.RegisterObjectWithHealth(this);
		if (gibOnDeath) {
			for (int i=0;i<gibObjects.Length;i++) {
				SaveObject so = gibObjects[i].GetComponent<SaveObject>();
				if (so != null) so.Start();
			}
		}
		awakeInitialized = true;
	}

	// Put into Start instead of Awake to give Const time to populate from enemy_tables.csv
	public void Start () {
		if (startInitialized) return;

		if (isNPC) {
			aic = GetComponent<AIController>();
			index = aic.index;

			if (Const.a != null) {
				if (IsCyberEntity()) {
					if (cyberHealth == -1) cyberHealth = Const.a.healthForCyberNPC[index];
					if (maxhealth == -1) maxhealth = Const.a.healthForCyberNPC[index];
				} else {
					if (health == -1) health = Const.a.healthForNPC[index]; //leaves possibility of setting health lower than normal, for instance the cortex reaver on level 5
					if (maxhealth == -1) maxhealth = Const.a.healthForNPC[index]; // set maxhealth to default healthForNPC, possible to set higher, e.g. for cyborg assassins on level 9 whose health is 3 times normal
				}

				if (Const.a.difficultyCombat == 0) {
					maxhealth = 1;
					health = maxhealth;
				}
			}
			if (actAsCorpseOnly && isNPC) InitializeCorpseOnly();
        }
		if (maxhealth <= 0) maxhealth = health;
		LinkToAutomapOverlay();
		startInitialized = true;
	}

	void LinkToAutomapOverlay() {
		if (!isSecCamera && !isNPC) return;
		if (linkedOverlay != null) return; // Already have an overlay.

		PoolType pt = PoolType.AutomapCameraOverlays;
		if (isNPC && aic.index > 0 && aic.index < Const.a.typeForNPC.Length) {
			switch (Const.a.typeForNPC[aic.index]) {
				case NPCType.Mutant:       pt = PoolType.AutomapMutantOverlays;
										   break;
				case NPCType.Supermutant:  pt = PoolType.AutomapMutantOverlays;
										   break;
				case NPCType.Robot:        pt = PoolType.AutomapBotOverlays;
										   break;
				case NPCType.Cyborg:       pt = PoolType.AutomapCyborgOverlays;
										   break;
				case NPCType.Supercyborg:  pt = PoolType.AutomapCyborgOverlays;
										   break;
				case NPCType.MutantCyborg: pt = PoolType.AutomapCyborgOverlays;
										   break;
			}
		}

		GameObject overlay = Const.a.GetObjectFromPool(pt);
		if (overlay != null) {
			linkedOverlay = overlay.GetComponent<Image>();
		} else {
			Debug.Log("BUG: No automap icon type " + pt.ToString());
		}

		UpdateLinkedOverlay();
	}

	public void UpdateLinkedOverlay() {
		if (!isSecCamera && !isNPC) return;
		if (IsCyberEntity()) return;
		if (isNPC && Inventory.a.NavUnitVersion() <= 1) return;

		Automap.a.TurnOnLinkedOverlay(linkedOverlay,health,gameObject,isNPC);
		Automap.a.SetLinkedOverlayPos(linkedOverlay,health,gameObject);
	}

	void OnDisable() {
		if (linkedOverlay != null) {
			Utils.Deactivate(linkedOverlay.gameObject);
			Utils.DisableImage(linkedOverlay);
		}
	}

	void OnEnable() {
		Start();
		UpdateLinkedOverlay();
	}

	public void ClearOverlays() {
		if (pstatic != null) pstatic.Deactivate();
		if (empstatic != null) empstatic.Deactivate();
		if (healingFXFlash != null) healingFXFlash.SetActive(false);
	}

	void UseDeathTargets() {
		if (!string.IsNullOrWhiteSpace(targetOnDeath)) {
			UseData ud = new UseData();
			ud.argvalue = argvalue;
			TargetIO tio = GetComponent<TargetIO>();
			if (tio != null) {
				ud.SetBits(tio);
			} else {
				Debug.Log("BUG: no TargetIO.cs found on a gameobject with a HealthManager.cs script!  Trying to call UseTargets without parameters!");
			}
			Const.a.UseTargets(ud,targetOnDeath);
		}
	}

	void InitializeCorpseOnly() {
		health = 0;
		cyberHealth = 0;
		if (aic != null) { if (aic.SFX != null) aic.SFX.enabled = false; }
		if (!deathDone) {
			UseDeathTargets();
			if (teleportOnDeath) TeleportAway();
			NPCDeath(null);
		}
	}

	float ApplyAttackTypeAdjustments(float take,DamageData dd) {
		if (isNPC && health > 0f) {
			if (Const.a.typeForNPC[index] == NPCType.Mutant) {
				switch(dd.attackType) {
					case AttackType.None: take *= 1f; break; // same
					case AttackType.Melee: take *= 1f; break; // same
					case AttackType.MeleeEnergy: take *= 1f; break; // same
					case AttackType.EnergyBeam: take *= 1f; break; // same
					case AttackType.Magnetic: take *= 0; break;
					case AttackType.Projectile: take *= 1f; break; // same
					case AttackType.ProjectileEnergyBeam: take *= 1f; break; // same
					case AttackType.ProjectileLaunched: take *= 1f; break; // same
					case AttackType.Gas: take *= 2f; break;
					case AttackType.ProjectileNeedle: take *= 2f; break; // same
					case AttackType.Tranq: take *= 1f; break; // same
				}
			}

			if (Const.a.typeForNPC[index] == NPCType.Supermutant) {
				switch(dd.attackType) {
					case AttackType.None: take *= 1f; break; // same
					case AttackType.Melee: take *= 1f; break; // same
					case AttackType.MeleeEnergy: take *= 1f; break; // same
					case AttackType.EnergyBeam: take *= 1f; break; // same
					case AttackType.Magnetic: take *= 0; break; // no damage
					case AttackType.Projectile: take *= 1f; break; // same
					case AttackType.ProjectileEnergyBeam: take *= 1f; break; // same
					case AttackType.ProjectileLaunched: take *= 1f; break; // same
					case AttackType.Gas: take *= 1.5f; break;
					case AttackType.ProjectileNeedle: take *= 1f; break; // same
					case AttackType.Tranq: take *= 1f; break; // same
				}
			}

			if (Const.a.typeForNPC[index] == NPCType.Robot) {
				switch(dd.attackType) {
					case AttackType.None: take *= 1f; break; // same
					case AttackType.Melee: take *= 1f; break; // same
					case AttackType.MeleeEnergy: take *= 1f; break; // same
					case AttackType.EnergyBeam: take *= 1f; break; // same
					case AttackType.Magnetic: take *= 4f; break; // same
					case AttackType.Projectile: take *= 1f; break; // same
					case AttackType.ProjectileEnergyBeam: take *= 1f; break; // same
					case AttackType.ProjectileLaunched: take *= 1f; break; // same
					case AttackType.Gas: take = 0; break; // no damage
					case AttackType.ProjectileNeedle: take = 0; break; // no damage
					case AttackType.Tranq: take *= 1f; break; // no damage
				}
			}

			if (Const.a.typeForNPC[index] == NPCType.Cyborg) {
				switch(dd.attackType) {
					case AttackType.None: take *= 1f; break; // same
					case AttackType.Melee: take *= 1f; break; // same
					case AttackType.MeleeEnergy: take *= 1f; break; // same
					case AttackType.EnergyBeam: take *= 1f; break; // same
					case AttackType.Magnetic: take *= 2f; break; // same
					case AttackType.Projectile: take *= 1f; break; // same
					case AttackType.ProjectileEnergyBeam: take *= 1f; break; // same
					case AttackType.ProjectileLaunched: take *= 1f; break; // same
					case AttackType.Gas: take *= 1f; break; // same
					case AttackType.ProjectileNeedle: take *= 1f; break; // same
					case AttackType.Tranq: take *= 1f; break; // same
				}
			}

			if (Const.a.typeForNPC[index] == NPCType.Supercyborg) {
				switch(dd.attackType) {
					case AttackType.None: take *= 1f; break; // same
					case AttackType.Melee: take *= 1f; break; // same
					case AttackType.MeleeEnergy: take *= 1f; break; // same
					case AttackType.EnergyBeam: take *= 1f; break; // same
					case AttackType.Magnetic: take *= 2f; break; // same
					case AttackType.Projectile: take *= 1f; break; // same
					case AttackType.ProjectileEnergyBeam: take *= 1f; break; // same
					case AttackType.ProjectileLaunched: take *= 1f; break; // same
					case AttackType.Gas: take = 0; break;
					case AttackType.ProjectileNeedle: take = 0; break;
					case AttackType.Tranq: take *= 1f; break;
				}
			}

			if (Const.a.typeForNPC[index] == NPCType.MutantCyborg) {
				switch(dd.attackType) {
					case AttackType.None: take *= 1f; break; // same
					case AttackType.Melee: take *= 1f; break; // same
					case AttackType.MeleeEnergy: take *= 1f; break; // same
					case AttackType.EnergyBeam: take *= 1f; break; // same
					case AttackType.Magnetic: take *= 0.5f; break; // same
					case AttackType.Projectile: take *= 1f; break; // same
					case AttackType.ProjectileEnergyBeam: take *= 1f; break; // same
					case AttackType.ProjectileLaunched: take *= 1f; break; // same
					case AttackType.Gas: take *= 2f; break; // same
					case AttackType.ProjectileNeedle: take *= 2f; break; // same
					case AttackType.Tranq: take *= 1.5f; break; // same
				}
			}

			if (Const.a.typeForNPC[index] == NPCType.Cyber) {
				switch(dd.attackType) {
					case AttackType.None: take *= 1f; break; // same
					case AttackType.Melee: take *= 1f; break; // same
					case AttackType.MeleeEnergy: take *= 1f; break; // same
					case AttackType.EnergyBeam: take *= 1f; break; // same
					case AttackType.Magnetic: take *= 1f; break; // same
					case AttackType.Projectile: take *= 1f; break; // same
					case AttackType.ProjectileEnergyBeam: take *= 1f; break; // same
					case AttackType.ProjectileLaunched: take *= 1f; break; // same
					case AttackType.Gas: take *= 1f; break; // same
					case AttackType.ProjectileNeedle: take *= 1f; break; // same
					case AttackType.Tranq: take *= 1f; break; // same
					case AttackType.Drill: take = 0f; break; // same
				}
			}
		}
		return take;
	}

	public float TakeDamage(DamageData dd) {
		if (dd == null) return 0;
		if (dd.damage <= 0) return 0; // Ah!! scaryy!! cannot divide by 0, let's get out of here!
		if (god) return 0; // untouchable!

        tempFloat = health;
		if (IsCyberEntity()) {
			tempFloat = cyberHealth;
			if (dd.attackType == AttackType.Drill && isNPC) return 0; // Drill can't hurt NPC's
			if (dd.attackType != AttackType.Drill && isIce) return 0; // Pulser can't hurt Ice
		}

		// Object is dead exceptions.
		if (tempFloat <= 0) {
			if (gibOnDeath || isIce || isPlayer || isGrenade || isScreen || isSecCamera || teleportOnDeath) return 0;
		}

		take = dd.damage;
		if (isPlayer) {
			float absorb = 0;
			if (inCyberSpace) {
				if (Inventory.a.hasSoft[2]) {
					switch(Inventory.a.softVersions[2]) {
						case 0: absorb = 0.00f; break;
						case 1: absorb = 0.10f; break;
						case 2: absorb = 0.15f; break;
						case 3: absorb = 0.20f; break;
						case 4: absorb = 0.25f; break;
						case 5: absorb = 0.30f; break;
						case 6: absorb = 0.35f; break;
						case 7: absorb = 0.40f; break;
						case 8: absorb = 0.45f; break;
						case 9: absorb = 0.50f; break;
					}

					take = (take * (1f - absorb)); // absorb percentage from above table
					if (take <= 0f) return 0f; // nothing to see here
				}
			} else {
				// Check if player shield is active
				if (dd.attackType == AttackType.Magnetic) {
					take = 0f; // don't get hurt by magnetic interactions
					empstatic.Flash(2);
					PlayerEnergy.a.TakeEnergy(11f);
					if (BiomonitorGraphSystem.a != null) {
						BiomonitorGraphSystem.a.EnergyPulse(11f);
					}
				}

				if (Inventory.a.hardwareIsActive[5] && Inventory.a.hasHardware[5]) {
					// Versions of shield protect against 20, 40, 75, 75%'s
					// Versions of shield thresholds are 0, 10, 15, 30...ooh what's this hang on now...Huh, turns out it absorbs all damage below the thresshold!  Cool!
					float thresh = 0;
					switch(Inventory.a.hardwareVersion[5]) {
						case 0: absorb = 0.2f;   thresh = 0f; break;
						case 1: absorb = 0.4f;  thresh = 10f; break;
						case 2: absorb = 0.75f; thresh = 15f; break;
						case 3: absorb = 0.75f; thresh = 30f; break;
					}

					if (take < thresh) absorb = 1f; // ah yeah! absorb. it. all.
					if (absorb > 0) {
						if (absorb < 1f) absorb = absorb + UnityEngine.Random.Range(-0.08f,0.08f); // +/- 8% variation - this was in the original I swear!  You could theoretically have 83% shielding max.
						if (absorb > 1f) absorb = 1f; // cap it at 100%....shouldn't really ever be here, nothing is 92% + 8%
						take *= (1f-absorb); // shield doing it's thing
						PlayerHealth.a.shieldEffect.SetActive(true); // Activate shield screen effect to indicate damage was absorbed, effect intensity determined by absorb amount
						Utils.PlayOneShotSavable(PlayerHealth.a.SFX,PlayerHealth.a.ShieldClip); // Play shield absorb sound
						int abs = (int)(absorb * 100f); //  for int display of absorbption percent
						Const.sprint(Const.a.stringTable[208] + abs.ToString() + Const.a.stringTable[209],dd.other);  // Shield absorbs x% damage
					}
				}
				if (take > 0 && ((absorb <0.4f) || Random.Range(0,1f) < 0.5f)) {
					Utils.PlayOneShotSavable(PlayerHealth.a.SFX,PlayerHealth.a.PainSFXClip); // Play player pain noise
					int intensityOfPainFlash = 0; // 0 = light
					if (take > 15f) {
						intensityOfPainFlash = 2; // 2 = heavy
					}

					if (take > 10f) {
						intensityOfPainFlash = 1; // 1 = med
					}

					pstatic.Flash(intensityOfPainFlash);
				}

				if (dd.ownerIsNPC) justHurtByEnemy = PauseScript.a.relativeTime;
			}
		}

		// Do the damage, that's right do. your. worst!
		if (IsCyberEntity()) {
			float cybbefore = cyberHealth;
			cyberHealth -= take;
			if (isPlayer) {
			    Const.a.damageReceived += take;
				MFDManager.a.DrawTicks(true);
				if (cyberHealth <= 0) {
					MouseLookScript.a.ExitCyberspace();
					return 0f;
				}
			}
			
			if (dd != null) {
				if (dd.owner != null) {
					if (dd.owner.CompareTag("Player")) {
						Const.a.damageDealt += take;
					}
				}
			}
		} else {
			float before = health;
			take = ApplyAttackTypeAdjustments(take,dd); // Apply critical based on AttackType

			// Was directly dd.damage but changed since we are check for extra
			// things in case GetDamageTakeAmount wasn't called on dd.damage
			// beforehand (e.g. player fall damage, internal to player only,
			// need to protect against shield, etc, JJ 9/5/19).
			health -= take;
			if (isPlayer) {
			    Const.a.damageReceived += take;
				MFDManager.a.DrawTicks(true);
				Music.a.inCombat = true;
			}
			
			if (dd.owner != null) {
				if (dd.owner.CompareTag("Player")) Const.a.damageDealt += take;
			}
		}

		attacker = dd.owner;
		if (isNPC && (health > 0f || (IsCyberEntity() && cyberHealth > 0f))) {
			AIController aic = GetComponent<AIController>();
			if (aic != null) {
				if (Const.a.timeBetweenPainForNPC[aic.index] > 0) {
					aic.goIntoPain = true;
				}

				aic.attacker = attacker;
				if (linkedTargetID != null) {
					linkedTargetID.SendDamageReceive(take,dd);
				}

				aic.CheckPain(); // setup enemy with NPC
			}
		}

		if (IsCyberEntity()) {
			if (cyberHealth <= 0f) {
			    Const.a.cyberkills++;
				Death(dd.attackType == AttackType.EnergyBeam);
			}
		} else {
			if (health <= 0f) {
			    Const.a.kills++;
				Death(dd.attackType == AttackType.EnergyBeam);
			}
		}

		return take;
	}

	void Death(bool energyVaporized) {
		if (!deathDone) {
			UseDeathTargets();
			if (vaporizeCorpse && !isSecCamera) VaporizeCorpse(energyVaporized);
			else if (isObject) ObjectDeath(null);
			else if (isScreen) ScreenDeath(backupDeathSound);
			else if (teleportOnDeath) TeleportAway();
			else if (isGrenade) GrenadeDeath();

			if (isNPC) NPCDeath(null);
			if (isPlayer) PlayerHealth.a.deaths++;
			deathDone = true;
		}
	}

	void VaporizeCorpse(bool energyVaporized) {
		deathDone = true;
		DropSearchables();
		if (deathFX == PoolType.None) deathFX = PoolType.CorpseHit;
		if (energyVaporized) deathFX = PoolType.Vaporize;
		MeshRenderer mr = GetComponent<MeshRenderer>();
		Utils.DisableMeshRenderer(mr);
		GameObject par = transform.parent.gameObject;
		AIController aic = par.GetComponent<AIController>();
		if (aic != null) {
			if (!aic.healthManager.gibOnDeath) { // We are a corpse here.
				// Turn off visible mesh entity from destroyed corpse.
				Utils.Deactivate(aic.visibleMeshEntity);
				Utils.Deactivate(aic.deathBurst); // For any extra effects.
												  // We are totally gone now!
			}
			Utils.Deactivate(aic.searchColliderGO);
			Utils.Deactivate(aic.gameObject);
		}
		CreateDeathEffects(deathFX);
		Utils.DisableCollision(gameObject);
	}

	public void TeleportAway() {
		if (!teleportDone) {
			teleportDone = true;
			Utils.Activate(teleportEffect);
		}
	}

	void PlayDeathSound(AudioClip deathSound) {
		if (actAsCorpseOnly) return;

		if (deathSound != null) {
			Utils.PlayTempAudio(transform.position,deathSound);
		} else if (backupDeathSound != null) {
			Utils.PlayTempAudio(transform.position,backupDeathSound);
		}
	}

	void NPCDeath (AudioClip deathSound) {
		if (deathDone) return; // We died the death, no 2nd deaths here.

		deathDone = true; // Mark it so we only die once.
		CreateDeathEffects(deathFX);
		PlayDeathSound(deathSound); // Play death sound, if present
		if (spawnMother != null) spawnMother.SpawneeJustDied();
		GameObject par = transform.parent.gameObject;
		if (aic == null) {
			AIController aic = par.GetComponent<AIController>();
		}

		if (aic == null) return;

		if (Const.a.typeForNPC[aic.index] == NPCType.Cyber) {
			Utils.SafeDestroy(aic.gameObject);
		} else {
			gameObject.layer = 13; // Corpse layer

			// Ok.  We've been through this.  Must keep the parent collider on
			// in order to prevent NPC's randomly falling through the floor
			// when killed because Unity's physics are junk.
			//Utils.DisableCollision(gameObject);
		}
	}

    public void Gib() {
		CreateDeathEffects(deathFX);
		if (gibObjects.Length > 0 ) {
			for (int i = 0; i < gibObjects.Length; i++) {
				if (gibObjects[i] != null) {
					if (!gibObjects[i].activeSelf) {
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
			for (int k=0;k<disableOnGib.Length;k++) {
				if (disableOnGib[k] != null) disableOnGib[k].SetActive(false);
			}
		}
		DropSearchables();
		Utils.DisableCollision(gameObject);
		AIController aic = GetComponent<AIController>();
		if (aic != null) {
			if (aic.healthManager.gibOnDeath) { // We are a corpse here.
				aic.visibleMeshEntity.SetActive(false); // Turn off visible mesh entity from destroyed corpse.
			}
		}

		HideSelf(); // Can't deactivate parent as gibs are children!
    }

	void DropSearchables() {
		if (searchableItem == null) return;

		MFDManager.a.NotifySearchThatSearchableWasDestroyed();
		GameObject levelDynamicContainer = LevelManager.a.GetCurrentDynamicContainer();
		for (int i=0;i<4;i++) {
			if (searchableItem.contents[i] >= 0) {
				GameObject tossObject = Instantiate(Const.a.useableItems[searchableItem.contents[i]],transform.position,Const.a.quaternionIdentity) as GameObject;
				if (tossObject != null) {
					if (tossObject.activeSelf != true) tossObject.SetActive(true);
					if (levelDynamicContainer != null) {
						tossObject.transform.SetParent(levelDynamicContainer.transform,true);
					}
					tossObject.GetComponent<UseableObjectUse>().customIndex = searchableItem.customIndex[i];
				} else {
					Const.sprint("BUG: Failed to instantiate object being dropped on gib.");
				}
				searchableItem.contents[i] = -1;
				searchableItem.customIndex[i] = -1;
			}
		}
	}

	public void GrenadeDeath() {
		GrenadeActivate ga = GetComponent<GrenadeActivate>();
		ga.Explode();
	}

	public void ScreenDeath(AudioClip deathSound) {
		if (deathDone) return;

		deathDone = true; // Screens maintain collisions, so not disabling here; also maintain visible mesh, don't turn it off
		PlayDeathSound(deathSound); // Make some noise
		ImageSequenceTextureArray ista = GetComponent<ImageSequenceTextureArray>();
		ista.Destroy(); // ista deada nowa
		if (gibOnDeath) Gib();
	}

	void CreateDeathEffects(PoolType fx) {
		if (fx == PoolType.None) return;

		GameObject explosionEffect = Const.a.GetObjectFromPool(fx);
		if (explosionEffect == null) return;

		Vector3 pos = transform.position;
		BoxCollider boxCol = GetComponent<BoxCollider>();
		if (boxCol != null) pos = transform.TransformPoint(boxCol.center);

		// MeshCollider doesn't have a center, so don't check meshCol here.

		SphereCollider sphereCol = GetComponent<SphereCollider>();
		if (sphereCol != null) pos = transform.TransformPoint(sphereCol.center);

		CapsuleCollider capCol = GetComponent<CapsuleCollider>();
		if (capCol != null) pos = transform.TransformPoint(capCol.center);

 		// Enable death effects (e.g. explosion particle effect)
		explosionEffect.SetActive(true);
		explosionEffect.transform.position = pos;
	}

	void HideSelf() {
		if (isScreen) return;

		MeshRenderer mr = GetComponent<MeshRenderer>();
		Utils.DisableMeshRenderer(mr);
		if (rbody != null) rbody.useGravity = false;		
	}

	public void ObjectDeath(AudioClip deathSound) {
		if (deathDone) return;

		if (gibOnDeath) {
			Gib();
		} else {
			Utils.DisableCollision(gameObject);
			DropSearchables();
			CreateDeathEffects(deathFX);
		}

		deathDone = true;
		if (linkedOverlay != null) {
			Utils.DisableImage(linkedOverlay); // Disable on automap
			Utils.Deactivate(linkedOverlay.gameObject);
		}

		if (securityAffected != SecurityType.None) {
			LevelManager.a.ReduceCurrentLevelSecurity(securityAffected);
		}

		PlayDeathSound(deathSound); // Make some noise
		if (spawnMother != null) spawnMother.SpawneeJustDied();
		if (deathFX != PoolType.None) HideSelf();
	}

	public void HealingBed(float amount,bool flashBed) {
		health += amount;
		if (health > 255) health = 255;
		if (isPlayer) MFDManager.a.DrawTicks(true);
		if (flashBed && healingFXFlash != null) healingFXFlash.SetActive(true);
	}

	public void AwakeFromLoad() {
		if (!awakeInitialized) { Awake(); }
		if (!startInitialized) { Start(); }

		// Handle objects (includes corpses)
		if (isObject) {
			if (gibOnDeath) {
				int gibcnt = gibObjects.Length;
				if (gibcnt > 0) {
					if (gibObjects[0] != null) {
						for (int i=0;i<gibcnt;i++) {
							if (health > 0) {
								if (gibObjects[i].activeSelf) {
									gibObjects[i].SetActive(false);
								}
							} else {
								if (!gibObjects[i].activeSelf) {
									gibObjects[i].SetActive(true);
								}
							}
						}
					}
				}
			}

			UpdateLinkedOverlay();
			if ((health > 0 && !IsCyberEntity())
				|| (cyberHealth > 0 && IsCyberEntity())) {

				Utils.EnableCollision(gameObject);
				MeshRenderer mr = GetComponent<MeshRenderer>();
				if (mr != null) {
					mr.enabled = true;
				} else {
					AIController aicP = GetComponentInParent<AIController>();
					if (aicP != null) {
						if (!aicP.startInitialized) aicP.Start();
						if (aicP.healthManager != null) {
							if (!aicP.healthManager.gibOnDeath) {
								if (aicP.visibleMeshEntity != null) {
									// We are a corpse, re-enable parent
									aicP.visibleMeshEntity.SetActive(true);
								}
							}
						}
					}
				}
			} else {
				Utils.DisableCollision(gameObject);
				if (linkedOverlay != null) {
					Utils.DisableImage(linkedOverlay); // Disable on automap.
					Utils.Deactivate(linkedOverlay.gameObject);
				}

				MeshRenderer mr = GetComponent<MeshRenderer>();
				if (mr != null) {
					mr.enabled = false;
				} else {
					AIController aicP = GetComponentInParent<AIController>();
					if (aicP != null) {
						if (!aicP.startInitialized) aicP.Start();
						if (aicP.healthManager != null) {
							if (!aicP.healthManager.gibOnDeath) {
								if (aicP.visibleMeshEntity != null) aicP.visibleMeshEntity.SetActive(false);
							}
						}
					}
				}
			}
		}

		// Handle screens
		if (isScreen) {
			if (health > 0) {
				ImageSequenceTextureArray ista = GetComponent<ImageSequenceTextureArray>();
				if (ista != null) {
					ista.AwakeFromLoad(health);
				}
			}
		}

		// Handle NPCs
		if (spawnMother != null && isNPC) spawnMother.AwakeFromLoad(health);

		// Handle grenades
		if (isGrenade) {
			GrenadeActivate ga = GetComponent<GrenadeActivate>();
			if (ga != null) ga.AwakeFromLoad(health);
		}
	}

	public bool IsCyberEntity() { 
		if (inCyberSpace) return true;
		if (!isPlayer && cyberHealth > 0f) return true;
		return (index > 23 && isNPC); // 24, 25, 26, 27, 28 are Cyber enemies
	}

	// Generic health info string
	public static string Save(GameObject go, PrefabIdentifier prefID) {
		HealthManager hm = go.GetComponent<HealthManager>();
		if (hm == null) {
			hm = go.transform.GetChild(0).GetComponent<HealthManager>();
			if (hm == null) {
				Debug.Log("HealthManager missing on savetype of HealthManager!"
						  + "  GameObject.name: " + go.name);
				return Utils.DTypeWordToSaveString("ffbbbu");
			}
		}

		if (!hm.awakeInitialized) hm.Awake();
		if (!hm.startInitialized) hm.Start();
		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		s1.Append(Utils.UintToString(hm.levelIndex,"levelIndex"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(hm.health,"health"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(hm.cyberHealth,"cyberHealth"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(hm.deathDone,"deathDone")); // Dead yet?
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(hm.god,"godmode")); // Are we invincible? 
														 // We can save cheats?
														 // OH WOW!
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(hm.teleportDone,"teleportDone"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveString(hm.targetOnDeath,"targetOnDeath"));
		if (!string.IsNullOrWhiteSpace(hm.targetOnDeath)) {
		    s1.Append(Utils.splitChar);
		    s1.Append(TargetIO.Save(go));
		}
		
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(hm.gibObjects.Length,"gibObjects.Length"));

		for (int i=0;i<hm.gibObjects.Length; i++) {
			s1.Append(Utils.splitChar);
			s1.Append(Utils.SaveSubActivatedGOState(hm.gibObjects[i]));
		}

		if (prefID != null) {
			if (prefID.constIndex == 526) { // prop_console02
				if (go.transform.childCount > 0) { // Screen is first child.
					GameObject child = go.transform.GetChild(0).gameObject; 
					s1.Append(Utils.splitChar);
					s1.Append(Utils.BoolToString(child.activeSelf,
												 "child.activeSelf"));

					ImageSequenceTextureArray ista
						= child.GetComponent<ImageSequenceTextureArray>();

					if (ista != null) {
						s1.Append(Utils.splitChar);
						s1.Append(Utils.SaveString(ista.resourceFolder,
												   "resourceFolder"));
					}
				} else {
					s1.Append(Utils.splitChar);
					s1.Append(Utils.BoolToString(true,"child.activeSelf"));
					s1.Append(Utils.splitChar);
					s1.Append(Utils.SaveString("MedScreen27","resourceFolder"));
				}
			}
		}

		s1.Append(Utils.splitChar);

		// Did we already teleport?
		s1.Append(Utils.BoolToString(hm.teleportOnDeath,"teleportOnDeath"));
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index,
						   PrefabIdentifier prefID) {
		HealthManager hm= go.GetComponent<HealthManager>();
		if (hm == null) {
			hm = go.transform.GetChild(0).GetComponent<HealthManager>();
		}

		if (hm == null) {
			Debug.Log("HealthManager.Load failure, hm == null");
			return index + 7;
		}

		if (index < 0) {
			Debug.Log("HealthManager.Load failure, index < 0");
			return index + 7;
		}

		if (entries == null) {
			Debug.Log("HealthManager.Load failure, entries == null");
			return index + 7;
		}

        hm.levelIndex = Utils.GetIntFromString(entries[index],"levelIndex");
        index++; SaveObject.currentSaveEntriesIndex = index.ToString();
        
		if (!hm.awakeInitialized) hm.Awake();
		if (!hm.startInitialized) hm.Start();
		hm.health = Utils.GetFloatFromString(entries[index],"health");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();
		if (!Utils.IndexEntriesOk(index,ref entries,go)) return index;

		hm.cyberHealth = Utils.GetFloatFromString(entries[index],"cyberHealth");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();
		if (!Utils.IndexEntriesOk(index,ref entries,go)) return index;

		hm.deathDone = Utils.GetBoolFromString(entries[index],"deathDone");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();
		if (!Utils.IndexEntriesOk(index,ref entries,go)) return index;

		hm.god = Utils.GetBoolFromString(entries[index],"godmode");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();
		if (!Utils.IndexEntriesOk(index,ref entries,go)) return index;

		hm.teleportDone = Utils.GetBoolFromString(entries[index],
												  "teleportDone");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();
		if (!Utils.IndexEntriesOk(index,ref entries,go)) return index;
		
        hm.targetOnDeath = Utils.LoadString(entries[index],"targetOnDeath");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();
		if (!Utils.IndexEntriesOk(index,ref entries,go)) return index;
		if (!string.IsNullOrWhiteSpace(hm.targetOnDeath)) {
		    index = TargetIO.Load(go,ref entries,index,true);
		    SaveObject.currentSaveEntriesIndex = index.ToString();
	    	if (!Utils.IndexEntriesOk(index,ref entries,go)) return index;
		}
		
		hm.AwakeFromLoad();

		int numChildren = hm.gibObjects.Length;
		int numChildrenFromSave = Utils.GetIntFromString(entries[index],
														  "gibObjects.Length");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();
		if (!Utils.IndexEntriesOk(index,ref entries,go)) return index;

		if (numChildren != numChildrenFromSave) {
			Debug.Log("BUG: HealthManager gibObjects.Length("
					  + numChildren.ToString() + ") != children("
					  + numChildrenFromSave.ToString() + ") from"
					  + " savefile on " + go.name + "!");
			return index;
		}

		if (numChildren > 0) {
			for (int i=0; i<numChildren; i++) {
				index = Utils.LoadSubActivatedGOState(hm.gibObjects[i],
													  ref entries,index);

				if (!Utils.IndexEntriesOk(index,ref entries,go)) return index;
			}
		}

		if (prefID != null) {
			if (prefID.constIndex == 526) { // prop_console02
				if (go.transform.childCount > 0) { // Screen is first child.
					GameObject child = go.transform.GetChild(0).gameObject;
					child.SetActive(Utils.GetBoolFromString(entries[index],
														"child.activeSelf"));
					index++;
					if (!Utils.IndexEntriesOk(index,ref entries,go)) {
						return index;
					}

					ImageSequenceTextureArray ista
						= child.GetComponent<ImageSequenceTextureArray>();

					if (ista != null) {
						ista.resourceFolder =
							Utils.LoadString(entries[index],
												"resourceFolder");
						index++;
						if (!Utils.IndexEntriesOk(index,ref entries,go)) {
							return index;
						}
					}
				}
			}	
		}

		hm.teleportOnDeath = Utils.GetBoolFromString(entries[index],
													 "teleportOnDeath");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();
		return index;
	}
}
