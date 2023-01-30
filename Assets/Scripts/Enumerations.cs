// Global enumerations
// ============================================================================
public enum SaveableType : byte {Transform,Player,Useable,Grenade,NPC,
                                 Destructable,SearchableStatic,
                                 SearchableDestructable,Door,ForceBridge,Switch,
                                 FuncWall,TeleDest,LBranch,LRelay,LSpawner,
                                 InteractablePanel,ElevatorPanel,Keypad,
                                 PuzzleGrid,PuzzleWire,TCounter,TGravity,
                                 MChanger,GravPad,TransformParentless,
                                 ChargeStation,Light,LTimer,Camera,
                                 DelayedSpawn,SecurityCamera,Trigger,
                                 Projectile,NormalScreen,CyberSwitch};
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
                                    Per3,Per4,Per5}; // Command = STO+MTN+SEC
public enum MusicType : byte {None,Walking,Combat,Overlay,Override};
public enum TrackType : byte {None,Walking,Combat,MutantNear,CyborgNear,
                              CyborgDroneNear,RobotNear,Transition,Revive,Death,
                              Cybertube,Elevator,Distortion};
public enum BloodType : byte {None,Red,Yellow,Green,Robot,Leaf,Mutation,
                              GrayMutation};
public enum SecurityType : byte {None,Camera,NodeSmall,NodeLarge};
public enum AudioLogType : byte {TextOnly,Normal,Email,Papers,Vmail};
public enum EnergyType : byte {Battery,ChargeStation};

// Pool references
public enum PoolType {None,SparqImpacts,CameraExplosions,SparksSmall,
                      BloodSpurtSmall,BloodSpurtSmallYellow,
                      BloodSpurtSmallGreen,SparksSmallBlue,HopperImpact,
					  GrenadeFragExplosions,Vaporize,BlasterImpacts,IonImpacts,
					  MagpulseImpacts,StungunImpacts,RailgunImpacts,
                      PlasmaImpacts,ProjEnemShot6Impacts,ProjEnemShot2Impacts,
					  ProjSeedPodsImpacts,TempAudioSources,
                      GrenadeEMPExplosions,ProjEnemShot4Impacts,CrateExplosions,
                      GrenadeFragLive,ConcussionLive,EMPLive,GasLive,
                      GasExplosions,CorpseHit,LeafBurst,MutationBurst,
                      GraytationBurst,BarrelExplosions,BulletHoleLarge,
                      BulletHoleScorchLarge,BulletHoleScorchSmall,
                      BulletHoleSmall,BulletHoleTiny,BulletHoleTinySpread,
                      CyberDissolve,TargetIDInstances,AutomapBotOverlays,
                      AutomapCyborgOverlays,AutomapMutantOverlays,
                      AutomapCameraOverlays};

// UI
public enum ConfigToggleType : byte {Fullscreen,SSAO,Bloom,Reverb,Subtitles,
                                     InvertLook,InvertCyber,
                                     InvertInventoryCycling,QuickPickup,
                                     QuickReload,Reflections,Vsync,
                                     NoShootMode};
public enum HUDColor : byte {White,Red,Orange,Yellow,Green,Blue,Purple,Gray};
public enum ForceFieldColor : byte {Red,Green,Blue,Purple,RedFaint};
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
