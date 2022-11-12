// Global enumerations
// ============================================================================
public enum SaveableType : byte {Transform,Player,Useable,Grenade,NPC,
                                 Destructable,SearchableStatic,
                                 SearchableDestructable,Door,ForceBridge,Switch,
                                 FuncWall,TeleDest,LBranch,LRelay,LSpawner,
                                 InteractablePanel,ElevatorPanel,Keypad,
                                 PuzzleGrid,PuzzleWire,TCounter,TGravity,
                                 MChanger,RadTrig,GravPad,TransformParentless,
                                 ChargeStation,Light,LTimer,Camera,
                                 DelayedSpawn};
public enum BodyState : byte {Standing,Crouch,CrouchingDown,StandingUp,Prone,
                              ProningDown,ProningUp};
public enum Handedness : byte {Center,LH,RH};
public enum AttackType : byte {None,Melee,MeleeEnergy,EnergyBeam,Magnetic,
                               Projectile,ProjectileNeedle,ProjectileEnergyBeam,
                               ProjectileLaunched,Gas,Tranq,Drill};
public enum NPCType : byte {Mutant,Supermutant,Robot,Cyborg,Supercyborg,Cyber,
                            MutantCyborg};
public enum PerceptionLevel : byte {Low,Medium,High,Omniscient};
public enum AIState : byte {Idle,Walk,Run,Attack1,Attack2,Attack3,Pain,Dying,
                            Dead,Inspect,Interacting};
public enum AIMoveType : byte {Walk,Fly,Swim,Cyber,None};
public enum DoorState : byte {Closed, Open, Closing, Opening};
public enum FuncStates : byte {Start, Target, MovingStart, MovingTarget,
                               AjarMovingStart,AjarMovingTarget};
public enum SoftwareType : byte {None,Drill,Pulser,CShield,Decoy,Recall,Turbo,
                                 Game,Data,Integrity,Keycard};
public enum AccessCardType : byte {	None,Standard,Medical,Science,Admin,Group1,
                                    Group2,Group3,Group4,GroupA,GroupB,Storage,
                                    Engineering,Maintenance,Security,Per1,Per2,
                                    Per3,Per4,Per5,Command};
public enum MusicType : byte {None,Walking,Combat,Overlay,Override};
public enum TrackType : byte {None,Walking,Combat,MutantNear,CyborgNear,
                              CyborgDroneNear,RobotNear,Transition,Revive,Death,
                              Cybertube,Elevator,Distortion};
public enum BloodType : byte {None,Red,Yellow,Green,Robot,Leaf,Mutation,
                              GrayMutation};
public enum SecurityType : byte {None,Camera,NodeSmall,NodeLarge};
public enum AudioLogType : byte {TextOnly,Normal,Email,Papers,Vmail};

// Pool references
public enum PoolType {None,SparqImpacts,CameraExplosions,ProjEnemShot2,
                      SparksSmall,BloodSpurtSmall,BloodSpurtSmallYellow,
                      BloodSpurtSmallGreen,SparksSmallBlue,HopperImpact,
					  GrenadeFragExplosions,Vaporize,BlasterImpacts,IonImpacts,
					  MagpulseShots,MagpulseImpacts,StungunShots,StungunImpacts,
                      RailgunShots,RailgunImpacts,PlasmaShots,PlasmaImpacts,
                      ProjEnemShot6,ProjEnemShot6Impacts,ProjEnemShot2Impacts,
					  ProjSeedPods,ProjSeedPodsImpacts,TempAudioSources,
                      GrenadeEMPExplosions,ProjEnemShot4,ProjEnemShot4Impacts,
                      CrateExplosions,GrenadeFragLive,
                      CyborgAssassinThrowingStars,ConcussionLive,EMPLive,
                      GasLive,GasExplosions,CorpseHit,NPCMagpulseShots,
                      NPCRailgunShots,LeafBurst,MutationBurst,GraytationBurst,
                      BarrelExplosions,CyberPlayerShots,CyberDogShots,
					  CyberReaverShots,BulletHoleLarge,BulletHoleScorchLarge,
                      BulletHoleScorchSmall,BulletHoleSmall,BulletHoleTiny,
                      BulletHoleTinySpread,CyberPlayerIceShots,CyberDissolve,
                      TargetIDInstances,AutomapBotOverlays,
                      AutomapCyborgOverlays,AutomapMutantOverlays,
                      AutomapCameraOverlays};

// UI
public enum ConfigToggleType : byte {Fullscreen,SSAO,Bloom,Reverb,Subtitles,
                                     InvertLook,InvertCyber,
                                     InvertInventoryCycling,QuickPickup,
                                     QuickReload,Reflections};
public enum HUDColor : byte {White,Red,Orange,Yellow,Green,Blue,Purple,Gray};
public enum ButtonType : byte {Generic,GeneralInv,Patch,Grenade,Weapon,Search,
                               None,PGrid,PWire};
public enum TabMSG : byte {None,Search,AudioLog,Keypad,Elevator,GridPuzzle,
                           WirePuzzle,EReader,Weapon,SystemAnalyzer};
public enum PuzzleCellType : byte {
    Off,      // Blank
    Standard, // X or +
    And,      // Takes two power sources for on
    Bypass    // Always +
};
public enum PuzzleGridType : byte {King,Queen,Knight,Rook,Bishop,Pawn};
