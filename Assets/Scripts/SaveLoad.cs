using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public static class SaveLoad {
    public static string commentIndicator = "//"; // Ignore anything after on a line.
    public static string sectionIndicator = "#"; // Separates types of items in files.
    public static string splitChar = "|"; // Common delimiter, not a comma as in csv.
    public static char keyValueSplitChar = ':';
    public static int litCullingMask = LayerMask.GetMask("Default",
                                            "TransparentFX",
                                            "Water","GunViewModel",
                                            "Geometry","NPC","Bullets",
                                            "Player","Corpse",
                                            "PhysObjects","Door",
                                            "InterDebris","Player2",
                                            "Player3","Player4",
                                            "NPCBulllet",
                                            "CorpseSearchable");

    public static float[] shadCullArray = new float[] {
        15f, // Default
        0.0f, // TransparentFX
        0.0f, // Ignore Raycast
        0.0f, // 
        0.0f, // Water
        0.0f, // UI
        0.0f, // 
        0.0f, // 
        0.0f, // GunViewModel
        20.0f, // Geometry
        15f, // NPC
        5f, // Bullets
        7f, // Player
        15f, // Corpse
        20f, // PhysObjects
        0.0f, // Sky
        0.0f, // PlayerTriggerOnly
        0.0f, // Trigger
        20f, // Door
        15f, // InterDebris
        0.0f, // Player2
        0.0f, // Player3
        0.0f, // Player4
        0.0f, // NPCTrigger
        5f, // NPCBullet
        0.0f, // NPCClip
        0.0f, // Clip
        0.0f, // Automap
        0.0f, // Culling
        15f, // CorpseSearchable
        0.0f, //
        0.0f  //
    };

    public static string SavePrefab(GameObject go) {
        if (go == null) { Debug.LogError("Tried to save null GameObject in SavePrefab()"); return ""; }

        PrefabIdentifier pid = GetPrefabIdentifier(go,false);
        if (pid == null) { Debug.LogError("Tried to save " + go.name + ", but had no PrefabIdentifier!"); return ""; }

        bool isGeom = ConsoleEmulator.ConstIndexIsGeometry(pid.constIndex);
        bool isDyn = ConsoleEmulator.ConstIndexIsDynamicObject(pid.constIndex);
        bool isDor = ConsoleEmulator.ConstIndexIsDoor(pid.constIndex);
        bool isStatSav = ConsoleEmulator.ConstIndexIsStaticObjectSaveable(pid.constIndex);
        bool isStatImm = ConsoleEmulator.ConstIndexIsStaticObjectImmutable(pid.constIndex);
        bool isNPC = ConsoleEmulator.ConstIndexIsNPC(pid.constIndex);
        bool isLitSav = ConsoleEmulator.ConstIndexIsLightStaticSaveable(pid.constIndex);
        Light lit = go.GetComponent<Light>();
        bool isLit = (lit != null);
        if (isLit && isLitSav) isLitSav = false;
        Debug.Log("Saving " + go.name + " with constIndex " + pid.constIndex.ToString() + ", isGeom: " + isGeom.ToString() + ", isDyn: " + isDyn.ToString() + ", isDor: " + isDor.ToString() + ", isStatSav: " + isStatSav.ToString() + ", isStatImm: " + isStatImm.ToString() + ", isNPC: " + isNPC.ToString() + ", isLit: " + isLit.ToString() + ", isLitSav: " + isLitSav.ToString());
        if (pid.constIndex == 717) return ""; // Not a saveable prefab, child only.
        if (pid.constIndex == 718) return ""; // Not a saveable prefab, temp ent.
        if (pid.constIndex == 719) return ""; // Not a saveable prefab, temp ent.
        if (pid.constIndex == 721) return ""; // Not a saveable prefab, temp ent.
        if (pid.constIndex == 722) return ""; // Not a saveable prefab, temp ent.
        if (pid.constIndex == 723) return ""; // Not a saveable prefab, temp ent.
        if (pid.constIndex == 724) return ""; // Not a saveable prefab, temp ent.
        if (pid.constIndex == 725) return ""; // Not a saveable prefab, temp ent.
        if (pid.constIndex == 726) return ""; // Not a saveable prefab, temp ent.
        if (pid.constIndex == 727) return ""; // Not a saveable prefab, temp ent.
        if (pid.constIndex == 728) return ""; // Not a saveable prefab, temp ent.
        if (pid.constIndex == 729) return ""; // Not a saveable prefab, temp ent.
        if (pid.constIndex == 730) return ""; // Not a saveable prefab, temp ent.
        if (pid.constIndex == 731) return ""; // Not a saveable prefab, temp ent.
        if (pid.constIndex == 732) return ""; // Unused? ef_sparkspits
        if (pid.constIndex == 736) return ""; // Not a saveable prefab, temp ent.
        if (pid.constIndex == 739) return ""; // Not a saveable prefab, temp ent.
        if (pid.constIndex == 740) return ""; // Not a saveable prefab, temp ent.

        if (isGeom) {           return SaveGeometry(go,pid);
        } else if (isDyn) {     return SaveObject.Save(go);
        } else if (isDor) {     return SaveObject.Save(go);
        } else if (isStatSav) { return SaveObject.Save(go);
        } else if (isNPC) {     return SaveObject.Save(go);
        } else if (isStatImm) { 
            if (go.transform.childCount > 1) {
                PrefabIdentifier pidmain = go.GetComponent<PrefabIdentifier>();
                if (pidmain == null) { // Ok we are a saveable container object most likely
                    SaveObject sobmain = go.GetComponent<SaveObject>();
                    if (sobmain != null) {
                        if (sobmain.saveType == SaveableType.Transform) { // Ok final check, most definitely a toggleable container that has nothing of its own.
                            StringBuilder nest = new StringBuilder();
                            nest.Clear();
                            nest.Append("container " + go.name);
                            nest.Append(splitChar);
                            nest.Append(Utils.SaveTransform(go.transform));
                            for (int i=0;i<go.transform.childCount;i++) {
                                nest.Append(Environment.NewLine);
                                nest.Append(SavePrefab(go.transform.GetChild(i).gameObject));
                            }
                            return nest.ToString(); // Or not?
                        }
                    }
                }
             }
            return SaveStaticImmutable(go,pid);
        } else if (isLit) { return SaveLight(go);
        } else if (isLitSav) {
            if (go.transform.childCount >= 1) {
                SaveObject sobmain = go.GetComponent<SaveObject>();
                if (sobmain != null) {
                    if (sobmain.saveType == SaveableType.Transform) { // Ok final check, most definitely a toggleable container that has nothing of its own.
                        StringBuilder nest = new StringBuilder();
                        nest.Clear();
                        nest.Append("container " + go.name);
                        nest.Append(splitChar);
                        nest.Append(Utils.SaveTransform(go.transform));
                        for (int i=0;i<go.transform.childCount;i++) {
                            nest.Append(Environment.NewLine);
                            nest.Append(SaveLight(go.transform.GetChild(i).gameObject));
                        }
                        return nest.ToString(); // Or not?
                    } else Debug.LogError("Tried to save lights saveable container " + go.name + " but it didn't have a SaveObject set to Transform");
                } else Debug.LogError("Tried to save lights saveable container " + go.name + " but it didn't have a SaveObject attached");
                return "";
             } else {
                Debug.LogError("Light static saveable " + go.name + " is not a container, has no children!");
                return "constIndex:" + pid.constIndex.ToString();
             }
        } else {
            Debug.LogError("Uncategorized object " + go.name + "!");
            return "constIndex:" + pid.constIndex.ToString();
        }
    }
    
    public static GameObject LoadPrefab(ref string[] entries, int lineNum, int curlevel) {
        if (!(entries[0].Contains("constIndex"))) { // [sic], need to fix light file to start with constIndex:7777
            return LoadLight(entries,lineNum,curlevel);
        }

        int constIndex = Utils.GetIntFromString(entries[0],"constIndex");
        if (ConsoleEmulator.ConstIndexIsGeometry(constIndex)) {
            return LoadGeometry(entries,lineNum,curlevel);
        } else if (ConsoleEmulator.ConstIndexIsDynamicObject(constIndex)
                   || ConsoleEmulator.ConstIndexIsDoor(constIndex)
                   || ConsoleEmulator.ConstIndexIsStaticObjectSaveable(constIndex)
                   || ConsoleEmulator.ConstIndexIsNPC(constIndex)) {

            int saveID = Utils.GetIntFromString(entries[2],"SaveID");
            GameObject container = LevelManager.a.GetRequestedLevelDynamicContainer(LevelManager.a.currentLevel);
			GameObject newGO = ConsoleEmulator.SpawnDynamicObject(constIndex,curlevel,false,container,saveID);
			PrefabIdentifier prefID = SaveLoad.GetPrefabIdentifier(newGO,true);
			if (newGO != null) SaveObject.Load(newGO,ref entries,lineNum,prefID);
			return newGO;
        } else if (ConsoleEmulator.ConstIndexIsStaticObjectImmutable(constIndex)) {
            return LoadStaticImmutable(entries,lineNum,curlevel);
        } else {
            // Something went wrong, rebuild the line and print it.
            StringBuilder s1 = new StringBuilder();
            s1.Clear();
            for (int i=0;i<entries.Length;i++) {
                s1.Append(entries[i]);
                s1.Append(splitChar);
            }

            Debug.Log("Unknown line within save: " + s1.ToString());
        }
        
        return null;
    }
    
    // GameObect already null checked by originator.
    // PrefabIdentifier already null checked by originator.  
    private static string SaveStaticImmutable(GameObject go, PrefabIdentifier pid) {
        if (!ConsoleEmulator.ConstIndexIsStaticObjectImmutable(pid.constIndex)) {
            Debug.LogError(go.name + " is not a static object immutable, has constIndex of " + pid.constIndex.ToString());
        }
        
        StringBuilder s1 = new StringBuilder();
        s1.Clear();
        s1.Append(Utils.IntToString(pid.constIndex,"constIndex"));
        s1.Append(splitChar);
        s1.Append(Utils.SaveTransform(go.transform));
        if (pid.constIndex == 552) { // prop_cyber_datafrag
            s1.Append(splitChar);
            CyberDataFragment cybfrag = go.GetComponent<CyberDataFragment>();
            s1.Append(Utils.IntToString(cybfrag.textIndex,"textIndex"));
        } else if (pid.constIndex == 556) { // prop_cyberport
            CyberAccess cybacc = go.GetComponent<CyberAccess>();
            s1.Append(splitChar);
            s1.Append(Utils.SaveString(cybacc.target,"target"));
            s1.Append(splitChar);
            s1.Append(Utils.SaveString(cybacc.argvalue,"argvalue"));
            s1.Append(splitChar);
            s1.Append(TargetIO.Save(go));
        } else if (pid.constIndex == 574) { // prop_healingbed
            HealingBed heb = go.GetComponent<HealingBed>();
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(heb.broken,"broken"));
        } else if (pid.constIndex == 592 || pid.constIndex == 593) { // text_decal, text_decalStopDSS1
            TextMesh tm = go.GetComponent<TextMesh>();
            s1.Append(splitChar);
            string stripped = tm.text.Replace(System.Environment.NewLine, "#");
            s1.Append(Utils.SaveString(stripped,"text"));
            s1.Append(splitChar);
            TextLocalization tz = go.GetComponent<TextLocalization>();
            s1.Append(Utils.UintToString(tz.lingdex,"lingdex"));
            s1.Append(splitChar);
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            Material mat = mr.sharedMaterial;
            if (mat.name == "text_3dwhite") s1.Append(Utils.UintToString(87,"matIndex"));
            else if (mat.name == "text_3dgold") s1.Append(Utils.UintToString(89,"matIndex"));
            else if (mat.name == "text_3dgreen") s1.Append(Utils.UintToString(90,"matIndex"));
            else if (mat.name == "text_3dblack") s1.Append(Utils.UintToString(91,"matIndex"));
            else if (mat.name == "text_3dredStopD") s1.Append(Utils.UintToString(92,"matIndex"));
            else if (mat.name == "text_3dwhiteStopD") s1.Append(Utils.UintToString(93,"matIndex"));
            else if (mat.name == "text_3dblackStopD") s1.Append(Utils.UintToString(94,"matIndex"));
            else if (mat.name == "text_3dgoldStopD") s1.Append(Utils.UintToString(95,"matIndex"));
            else if (mat.name == "text_3dblueStopD") s1.Append(Utils.UintToString(96,"matIndex"));
            else if (mat.name == "text_3dblackStopD") s1.Append(Utils.UintToString(97,"matIndex"));
            else if (mat.name == "text_3dgoldunlit") s1.Append(Utils.UintToString(98,"matIndex"));
            else if (mat.name == "text_3dgoldunlitoverlay") s1.Append(Utils.UintToString(99,"matIndex"));
            else s1.Append(Utils.UintToString(88,"matIndex")); // text_3dred
        } else if (pid.constIndex == 595) { // trigger_cyberpush
            CyberPush cybp = go.GetComponent<CyberPush>();
            s1.Append(splitChar);
            s1.Append(Utils.FloatToString(cybp.force,"force"));
            s1.Append(splitChar);
            s1.Append(Utils.FloatToString(cybp.direction.x,"direction.x"));
            s1.Append(splitChar);
            s1.Append(Utils.FloatToString(cybp.direction.y,"direction.y"));
            s1.Append(splitChar);
            s1.Append(Utils.FloatToString(cybp.direction.z,"direction.z"));
            s1.Append(splitChar);
            s1.Append(Utils.SaveBoxCollider(go));
        } else if (pid.constIndex == 597) { // trigger_ladder
            s1.Append(splitChar);
            s1.Append(Utils.SaveBoxCollider(go));
        } else if (pid.constIndex == 599) { // trigger_music
            MusicTrigger must = go.GetComponent<MusicTrigger>();
            s1.Append(splitChar);
            s1.Append(Utils.FloatToString(must.tick,"tick"));
            s1.Append(splitChar);
            s1.Append(Utils.TrackTypeToString(must.trackType,"trackType"));
            s1.Append(splitChar);
            s1.Append(Utils.MusicTypeToString(must.musicType,"musicType"));
            s1.Append(splitChar);
            s1.Append(Utils.SaveBoxCollider(go));
        } else if (pid.constIndex == 601) { // trigger_radiation
            Radiation rad = go.GetComponent<Radiation>();
            s1.Append(splitChar);
            s1.Append(Utils.FloatToString(rad.radiationAmount,"radiationAmount"));
            s1.Append(splitChar);
            s1.Append(Utils.SaveBoxCollider(go));
        } else if (pid.constIndex == 603) { // us_paperlog
            PaperLog plog = go.GetComponent<PaperLog>();
            s1.Append(splitChar);
            s1.Append(Utils.UintToString(plog.logIndex,"logIndex"));
        } else if (pid.constIndex == 697 || pid.constIndex == 698) { // clip_npc, clip_objects
            s1.Append(splitChar);
            s1.Append(Utils.SaveBoxCollider(go));
        } else if (pid.constIndex == 707) { // info_email
            Email em = go.GetComponent<Email>();
            s1.Append(splitChar);
            s1.Append(Utils.UintToString(em.emailIndex,"emailIndex"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(em.autoPlayEmail,"autoPlayEmail"));
            s1.Append(splitChar);
            s1.Append(TargetIO.Save(go));
        } else if (pid.constIndex == 709) { // info_message
            TriggeredSprintMessage msg = go.GetComponent<TriggeredSprintMessage>();
            s1.Append(splitChar);
            s1.Append(Utils.UintToString(msg.messageLingdex,"messageLingdex"));
            s1.Append(splitChar);
            s1.Append(Utils.SaveString(msg.messageToDisplay,"messageToDisplay"));
            s1.Append(splitChar);
            s1.Append(TargetIO.Save(go));
        } else if (pid.constIndex == 710) { // info_mission
            QuestBitRelay qbr = go.GetComponent<QuestBitRelay>();
            s1.Append(splitChar);
            s1.Append(Utils.UintToString(qbr.lev1SecCode,"lev1SecCode"));
            s1.Append(splitChar);
            s1.Append(Utils.UintToString(qbr.lev2SecCode,"lev2SecCode"));
            s1.Append(splitChar);
            s1.Append(Utils.UintToString(qbr.lev3SecCode,"lev3SecCode"));
            s1.Append(splitChar);
            s1.Append(Utils.UintToString(qbr.lev4SecCode,"lev4SecCode"));
            s1.Append(splitChar);
            s1.Append(Utils.UintToString(qbr.lev5SecCode,"lev5SecCode"));
            s1.Append(splitChar);
            s1.Append(Utils.UintToString(qbr.lev6SecCode,"lev6SecCode"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(qbr.RobotSpawnDeactivated,"RobotSpawnDeactivated"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(qbr.IsotopeInstalled,"IsotopeInstalled"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(qbr.ShieldActivated,"ShieldActivated"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(qbr.LaserSafetyOverriden,"LaserSafetyOverriden"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(qbr.LaserDestroyed,"LaserDestroyed"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(qbr.BetaGroveCyberUnlocked,"BetaGroveCyberUnlocked"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(qbr.GroveAlphaJettisonEnabled,"GroveAlphaJettisonEnabled"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(qbr.GroveBetaJettisonEnabled,"GroveBetaJettisonEnabled"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(qbr.GroveDeltaJettisonEnabled,"GroveDeltaJettisonEnabled"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(qbr.MasterJettisonBroken,"MasterJettisonBroken"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(qbr.Relay428Fixed,"Relay428Fixed"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(qbr.MasterJettisonEnabled,"MasterJettisonEnabled"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(qbr.BetaGroveJettisoned,"BetaGroveJettisoned"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(qbr.AntennaNorthDestroyed,"AntennaNorthDestroyed"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(qbr.AntennaSouthDestroyed,"AntennaSouthDestroyed"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(qbr.AntennaEastDestroyed,"AntennaEastDestroyed"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(qbr.AntennaWestDestroyed,"AntennaWestDestroyed"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(qbr.SelfDestructActivated,"SelfDestructActivated"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(qbr.BridgeSeparated,"BridgeSeparated"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(qbr.IsolinearChipsetInstalled,"IsolinearChipsetInstalled"));
            s1.Append(splitChar);
            s1.Append(Utils.SaveString(qbr.target,"target"));
            s1.Append(splitChar);
            s1.Append(Utils.SaveString(qbr.targetIfFalse,"targetIfFalse"));
            s1.Append(splitChar);
            s1.Append(Utils.SaveString(qbr.argvalue,"argvalue"));
            s1.Append(splitChar);
            s1.Append(Utils.SaveString(qbr.argvalueIfFalse,"argvalueIfFalse"));
            s1.Append(splitChar);
            s1.Append(TargetIO.Save(go));
        } else if (pid.constIndex == 711) { // info_note
            Note nt = go.GetComponent<Note>();
            s1.Append(splitChar);
            s1.Append(Utils.SaveString(nt.note,"note"));
        } else if (pid.constIndex == 712) { // info_playsound
            PlaySoundTriggered snd = go.GetComponent<PlaySoundTriggered>();
            s1.Append(splitChar);
            s1.Append(Utils.IntToString(snd.SFXClip,"SFXClip"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(snd.loopingAmbient,"loopingAmbient"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(snd.playEverywhere,"playEverywhere"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(snd.currentlyPlaying,"currentlyPlaying"));
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(snd.playSoundOnParticleEmit,"playSoundOnParticleEmit"));
            s1.Append(splitChar);
            s1.Append(Utils.IntToString(snd.numparticles,"numparticles"));
            s1.Append(splitChar);
            s1.Append(Utils.IntToString(snd.burstemittcnt1,"burstemittcnt1"));
            s1.Append(splitChar);
            s1.Append(Utils.IntToString(snd.burstemittcnt2,"burstemittcnt2"));
            s1.Append(splitChar);
            s1.Append(TargetIO.Save(go));
            s1.Append(splitChar);
            s1.Append(Utils.SaveAudioSource(go));
        } else if (pid.constIndex == 714) { // info_screenshake
            EffectScreenShake eft = go.GetComponent<EffectScreenShake>();
            s1.Append(splitChar);
            s1.Append(Utils.FloatToString(eft.distance,"distance"));
            s1.Append(splitChar);
            s1.Append(Utils.FloatToString(eft.force,"force"));
        } else if (pid.constIndex == 715) { // info_spawnpoint
            s1.Append(TargetIO.Save(go));
        } else if (pid.constIndex == 716) { // fx_reverbzone
            AudioReverbZone arz = go.GetComponent<AudioReverbZone>();
            s1.Append(splitChar);
            s1.Append(Utils.FloatToString(arz.minDistance,"minDistance"));
            s1.Append(splitChar);
            s1.Append(Utils.FloatToString(arz.maxDistance,"maxDistance"));
            s1.Append(splitChar);
            s1.Append(Utils.UintToString(GetIntFromAudioReverbPreset(arz.reverbPreset),"reverbPreset"));
        } else if (pid.constIndex == 750 || pid.constIndex == 752 || pid.constIndex == 753) { // Pumpkins
            UseName un = go.GetComponent<UseName>();
            s1.Append(splitChar);
            s1.Append(Utils.SaveString(un.targetname,"targetname"));
        }

        return s1.ToString();
    }

    private static GameObject LoadStaticImmutable(string[] entries, int lineNum, int curlevel) {
        if (entries.Length <= 1) { 
            Debug.Log("Can't load static immutable from line "
                      + lineNum.ToString() + ", line had only one or no "
                      + "entries[]");
            return null;
        }

        int index = 0;
        int constIndex = Utils.GetIntFromString(entries[index],"constIndex"); index++;
        if (!ConsoleEmulator.ConstIndexIsStaticObjectImmutable(constIndex)) {
            Debug.Log("Load stat imm invalid constIndex: " + constIndex.ToString());
            return null;
        }

        GameObject go = ConsoleEmulator.SpawnDynamicObject(constIndex,curlevel,false,null,0);
        index = Utils.LoadTransform(go.transform,ref entries,index);
        if (constIndex == 552) { // prop_cyber_datafrag
            CyberDataFragment cybfrag = go.GetComponent<CyberDataFragment>();
            if (cybfrag != null) cybfrag.textIndex = Utils.GetIntFromString(entries[index],"textIndex");
            else cybfrag.textIndex = 442;

            index++;
        } else if (constIndex == 556) { // prop_cyberport
            CyberAccess cybacc = go.GetComponent<CyberAccess>();
            if (cybacc != null) {
                cybacc.target = Utils.LoadString(entries[index],"target"); index++;
                cybacc.argvalue = Utils.LoadString(entries[index],"argvalue"); index++;
            }

            index = TargetIO.Load(go,ref entries,index);
        } else if (constIndex == 574) { // prop_healingbed
            HealingBed heb = go.GetComponent<HealingBed>();
            heb.broken = Utils.GetBoolFromString(entries[index],"broken"); index++;
        } else if (constIndex == 592 || constIndex == 593) { // text_decal, text_decalStopDSS1
            string textRead = Utils.LoadString(entries[index],"text"); index++;
            TextLocalization tz = go.GetComponent<TextLocalization>();
            tz.lingdex = Utils.GetIntFromString(entries[index],"lingdex"); index++;
            if (tz.lingdex < 0) {
                // Only set text if index is -1 to not override the
                // TextLocalization which takes precedence.
                TextMesh tm = go.GetComponent<TextMesh>();
                tm.text = textRead; // Override.
            }
            
            int matIndex = Utils.GetIntFromString(entries[index],"matIndex"); index++;
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            mr.sharedMaterial = Const.a.genericMaterials[matIndex]; // sharedMaterial doesn't create new instance.
        } else if (constIndex == 595) { // trigger_cyberpush
            CyberPush cybp = go.GetComponent<CyberPush>();
            cybp.force = Utils.GetFloatFromString(entries[index],"force"); index++;
            float readX = Utils.GetFloatFromString(entries[index],"direction.x"); index++;
            float readY = Utils.GetFloatFromString(entries[index],"direction.y"); index++;
            float readZ = Utils.GetFloatFromString(entries[index],"direction.z"); index++;
            cybp.direction = new Vector3(readX,readY,readZ);
            index = Utils.LoadBoxCollider(go, ref entries,index);
        } else if (constIndex == 597) { // trigger_ladder
            index = Utils.LoadBoxCollider(go, ref entries,index);
        } else if (constIndex == 599) { // trigger_music
            MusicTrigger must = go.GetComponent<MusicTrigger>();
            must.tick = Utils.GetFloatFromString(entries[index],"tick"); index ++;
            must.trackType = Utils.IntToTrackType(entries[index],"trackType"); index++;
            must.musicType = Utils.IntToMusicType(entries[index],"musicType"); index++;
            index = Utils.LoadBoxCollider(go, ref entries,index);
        } else if (constIndex == 601) { // trigger_radiation
            Radiation rad = go.GetComponent<Radiation>();
            rad.radiationAmount = Utils.GetFloatFromString(entries[index],"radiationAmount"); index++;
            index = Utils.LoadBoxCollider(go, ref entries,index);
        } else if (constIndex == 603) { // us_paperlog
            PaperLog plog = go.GetComponent<PaperLog>();
            plog.logIndex = Utils.GetIntFromString(entries[index],"logIndex"); index++;
        } else if (constIndex == 697 || constIndex == 698) { // clip_npc, clip_objects
            index = Utils.LoadBoxCollider(go, ref entries,index);
        } else if (constIndex == 707) { // info_email
            Email em = go.GetComponent<Email>();
            em.emailIndex = Utils.GetIntFromString(entries[index],"emailIndex"); index++;
            em.autoPlayEmail = Utils.GetBoolFromString(entries[index],"autoPlayEmail"); index++;
            index = TargetIO.Load(go,ref entries,index);
        } else if (constIndex == 709) { // info_message
            TriggeredSprintMessage msg = go.GetComponent<TriggeredSprintMessage>();
            msg.messageLingdex = Utils.GetIntFromString(entries[index],"messageLingdex"); index++;
            msg.messageToDisplay = Utils.LoadString(entries[index],"messageToDisplay"); index++;
            index = TargetIO.Load(go,ref entries,index);
        } else if (constIndex == 710) { // info_mission
            QuestBitRelay qbr = go.GetComponent<QuestBitRelay>();
            qbr.lev1SecCode = Utils.GetIntFromString(entries[index],"lev1SecCode"); index++;
            qbr.lev2SecCode = Utils.GetIntFromString(entries[index],"lev2SecCode"); index++;
            qbr.lev3SecCode = Utils.GetIntFromString(entries[index],"lev3SecCode"); index++;
            qbr.lev4SecCode = Utils.GetIntFromString(entries[index],"lev4SecCode"); index++;
            qbr.lev5SecCode = Utils.GetIntFromString(entries[index],"lev5SecCode"); index++;
            qbr.lev6SecCode = Utils.GetIntFromString(entries[index],"lev6SecCode"); index++;
            qbr.RobotSpawnDeactivated = Utils.GetBoolFromString(entries[index],"RobotSpawnDeactivated"); index++;
            qbr.IsotopeInstalled = Utils.GetBoolFromString(entries[index],"IsotopeInstalled"); index++;
            qbr.ShieldActivated = Utils.GetBoolFromString(entries[index],"ShieldActivated"); index++;
            qbr.LaserSafetyOverriden = Utils.GetBoolFromString(entries[index],"LaserSafetyOverriden"); index++;
            qbr.LaserDestroyed = Utils.GetBoolFromString(entries[index],"LaserDestroyed"); index++;
            qbr.BetaGroveCyberUnlocked = Utils.GetBoolFromString(entries[index],"BetaGroveCyberUnlocked"); index++;
            qbr.GroveAlphaJettisonEnabled = Utils.GetBoolFromString(entries[index],"GroveAlphaJettisonEnabled"); index++;
            qbr.GroveBetaJettisonEnabled = Utils.GetBoolFromString(entries[index],"GroveBetaJettisonEnabled"); index++;
            qbr.GroveDeltaJettisonEnabled = Utils.GetBoolFromString(entries[index],"GroveDeltaJettisonEnabled"); index++;
            qbr.MasterJettisonBroken = Utils.GetBoolFromString(entries[index],"MasterJettisonBroken"); index++;
            qbr.Relay428Fixed = Utils.GetBoolFromString(entries[index],"Relay428Fixed"); index++;
            qbr.MasterJettisonEnabled = Utils.GetBoolFromString(entries[index],"MasterJettisonEnabled"); index++;
            qbr.BetaGroveJettisoned = Utils.GetBoolFromString(entries[index],"BetaGroveJettisoned"); index++;
            qbr.AntennaNorthDestroyed = Utils.GetBoolFromString(entries[index],"AntennaNorthDestroyed"); index++;
            qbr.AntennaSouthDestroyed = Utils.GetBoolFromString(entries[index],"AntennaSouthDestroyed"); index++;
            qbr.AntennaEastDestroyed = Utils.GetBoolFromString(entries[index],"AntennaEastDestroyed"); index++;
            qbr.AntennaWestDestroyed = Utils.GetBoolFromString(entries[index],"AntennaWestDestroyed"); index++;
            qbr.SelfDestructActivated = Utils.GetBoolFromString(entries[index],"SelfDestructActivated"); index++;
            qbr.BridgeSeparated = Utils.GetBoolFromString(entries[index],"BridgeSeparated"); index++;
            qbr.IsolinearChipsetInstalled = Utils.GetBoolFromString(entries[index],"IsolinearChipsetInstalled"); index++;
            qbr.target = Utils.LoadString(entries[index],"target"); index++;
            qbr.targetIfFalse = Utils.LoadString(entries[index],"targetIfFalse"); index++;
            qbr.argvalue = Utils.LoadString(entries[index],"argvalue"); index++;
            qbr.argvalueIfFalse = Utils.LoadString(entries[index],"argvalueIfFalse"); index++;
            index = TargetIO.Load(go,ref entries,index);
        } else if (constIndex == 711) { // info_note Actually unused but leaving for people making levels
            Note nt = go.GetComponent<Note>();
            nt.note = Utils.LoadString(entries[index],"note"); index++;
        } else if (constIndex == 712) { // info_playsound
            PlaySoundTriggered snd = go.GetComponent<PlaySoundTriggered>();
            snd.SFXClip = Utils.GetIntFromString(entries[index],"SFXClip"); index++;
            snd.loopingAmbient = Utils.GetBoolFromString(entries[index],"loopingAmbient"); index++;
            snd.playEverywhere = Utils.GetBoolFromString(entries[index],"playEverywhere"); index++;
            snd.currentlyPlaying = Utils.GetBoolFromString(entries[index],"currentlyPlaying"); index++;
            snd.playSoundOnParticleEmit = Utils.GetBoolFromString(entries[index],"playSoundOnParticleEmit"); index++;
            snd.numparticles = Utils.GetIntFromString(entries[index],"numparticles"); index++;
            snd.burstemittcnt1 = Utils.GetIntFromString(entries[index],"burstemittcnt1"); index++;
            snd.burstemittcnt2 = Utils.GetIntFromString(entries[index],"burstemittcnt2"); index++;
            index = TargetIO.Load(go,ref entries,index);
            index = Utils.LoadAudioSource(go,ref entries,index);
        } else if (constIndex == 714) { // info_screenshake
            EffectScreenShake eft = go.GetComponent<EffectScreenShake>();
            eft.distance = Utils.GetFloatFromString(entries[index],"distance"); index++;
            eft.force = Utils.GetFloatFromString(entries[index],"force"); index++;
        } else if (constIndex == 715) { // info_spawnpoint
            index = TargetIO.Load(go,ref entries,index);
        } else if (constIndex == 716) { // fx_reverbzone
            AudioReverbZone arz = go.GetComponent<AudioReverbZone>();
            arz.minDistance = Utils.GetFloatFromString(entries[index],"minDistance"); index++;
            arz.maxDistance = Utils.GetFloatFromString(entries[index],"maxDistance"); index++; 
            arz.reverbPreset = GetAudioReverbPresetFromInt(Utils.GetIntFromString(entries[index],"reverbPreset")); index++;
        } else if (constIndex == 750 || constIndex == 752 || constIndex == 753) { // Pumpkins
            UseName un = go.GetComponent<UseName>();
            un.targetname = Utils.LoadString(entries[index],"targetname"); index++;
        }
        
        return go;
    }

    // GameObect already null checked by originator.
    // PrefabIdentifier already null checked by originator.
    private static string SaveGeometry(GameObject go, PrefabIdentifier pid) {
        bool hasBoxColliderOverride = false;
        bool hasTextOverride = false;
        string materialOverride = "";
        #if UNITY_EDITOR
            if (PrefabUtility.IsPartOfPrefabInstance(go)) {
                List<ObjectOverride> ovides = PrefabUtility.GetObjectOverrides(go,false);
                for (int j=0; j < ovides.Count; j++) {
        //             UnityEngine.Object ob = ovides[j].instanceObject;
        //             SerializedObject sob = new UnityEditor.SerializedObject(ob);
        // 
        //             hasBoxColliderOverride = false;
        //             // List overridden properties
        //             SerializedProperty prop = sob.GetIterator();
        //             while (prop.NextVisible(true)) {
        //                 if (prop.propertyPath == "m_Name" ||
        //                     prop.propertyPath == "m_LocalPosition" ||
        //                     prop.propertyPath == "m_LocalPosition.x" ||
        //                     prop.propertyPath == "m_LocalPosition.y" ||
        //                     prop.propertyPath == "m_LocalPosition.z" ||
        //                     prop.propertyPath == "m_LocalRotation" ||
        //                     prop.propertyPath == "m_LocalRotation.x" ||
        //                     prop.propertyPath == "m_LocalRotation.y" ||
        //                     prop.propertyPath == "m_LocalRotation.z" ||
        //                     prop.propertyPath == "m_LocalRotation.w" ||
        //                     prop.propertyPath == "m_LocalScale" ||
        //                     prop.propertyPath == "m_LocalScale.x" ||
        //                     prop.propertyPath == "m_LocalScale.y" ||
        //                     prop.propertyPath == "m_LocalScale.z")  {
        //                     
        //                     continue; // Skip transform overrides
        //                 }
        // 
        //                 if (prop.prefabOverride) {
        //                     if (prop.propertyPath == "m_Size" ||
        //                         prop.propertyPath == "m_Size.x" ||
        //                         prop.propertyPath == "m_Size.y" ||
        //                         prop.propertyPath == "m_Size.z" ||
        //                         prop.propertyPath == "m_Center" ||
        //                         prop.propertyPath == "m_Center.x" ||
        //                         prop.propertyPath == "m_Center.y" ||
        //                         prop.propertyPath == "m_Center.z") {
        // 
        //                         hasBoxColliderOverride = true;
        //                     }
        // 
        //                     if (prop.propertyPath == "lingdex") hasTextOverride = true;
        //                     string value = GetPropertyValue(prop);
        //                     if (prop.propertyPath == "m_Materials.Array.data[0]") {
        //                         materialOverride = value;
        //                     }
        // 
        //                     UnityEngine.Debug.Log(go.name + ":: Found Override: "
        //                                           + prop.propertyPath + ", Value: "
        //                                           + value);
        //                 }
        //             }
                }
            } else {
                Debug.LogWarning("Saving geometry " + go.name + " but it wasn't a prefab!");
            } 
        #endif
        
        StringBuilder s1 = new StringBuilder();
        s1.Clear();
        s1.Append(Utils.UintToString(pid.constIndex,"constIndex"));
        s1.Append(Utils.splitChar);
        s1.Append(go.name);
        s1.Append(Utils.splitChar);
        s1.Append(Utils.SaveTransform(go.transform));
        if (hasBoxColliderOverride) {
            BoxCollider bcol = go.GetComponent<BoxCollider>();
            s1.Append(Utils.splitChar);
            s1.Append(Utils.BoolToString(bcol.enabled,"BoxCollider.enabled"));
            s1.Append(Utils.splitChar);
            s1.Append(Utils.FloatToString(bcol.size.x,"size.x"));
            s1.Append(Utils.splitChar);
            s1.Append(Utils.FloatToString(bcol.size.y,"size.y"));
            s1.Append(Utils.splitChar);
            s1.Append(Utils.FloatToString(bcol.size.z,"size.z"));
            s1.Append(Utils.splitChar);
            s1.Append(Utils.FloatToString(bcol.center.x,"center.x"));
            s1.Append(Utils.splitChar);
            s1.Append(Utils.FloatToString(bcol.center.y,"center.y"));
            s1.Append(Utils.splitChar);
            s1.Append(Utils.FloatToString(bcol.center.z,"center.z"));
            if (go.transform.childCount >= 1) {
                // Get collision aid.
                Transform subtr = go.transform.GetChild(0);
                if (subtr != null) {
                    s1.Append(Utils.splitChar);
                    s1.Append(Utils.BoolToString(
                        subtr.gameObject.activeSelf,
                        "collisionAid.activeSelf"));
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(materialOverride)) {
            s1.Append(Utils.splitChar);
            s1.Append(Utils.SaveString(materialOverride,"material"));
        }

        if (pid.constIndex == 218) { // chunk_reac2_4 has text on it.
            if (hasTextOverride) {
                s1.Append(Utils.splitChar);
                Transform textr1 = go.transform.GetChild(1); // text_decalStopDSS1
                Transform textr2 = go.transform.GetChild(2); // text_decalStopDSS1 (1)
                TextLocalization tex1 = textr1.gameObject.GetComponent<TextLocalization>();
                TextLocalization tex2 = textr2.gameObject.GetComponent<TextLocalization>();
                s1.Append(Utils.UintToString(tex1.lingdex,"lingdex"));
                s1.Append(Utils.splitChar);
                s1.Append(Utils.UintToString(tex2.lingdex,"lingdex"));
            }
        }

        return s1.ToString();
    }

    private static GameObject LoadGeometry(string[] entries, int lineNum, int curlevel) {
        if (entries.Length <= 1) { 
            Debug.Log("Can't load geometry from line " + lineNum.ToString()
                      + ", line had only one or no entries[]");
            return null;
        }

        int index = 0;
        int constdex = Utils.GetIntFromString(entries[index],"constIndex"); index++;
        if (!ConsoleEmulator.ConstIndexIsGeometry(constdex)) {
            Debug.Log("Invalid constdex loading geometry: " + constdex.ToString());
            return null;
        }

        GameObject chunk = ConsoleEmulator.SpawnDynamicObject(constdex,curlevel,false,null,0);
        if (chunk == null) return null;

        chunk.name = entries[index]; index++;
//         if (chunk.name == "chunk_cyberpanel (3918)") UnityEngine.Debug.Log("Loading chunk_cyberpanel (3918)");
        index = Utils.LoadTransform(chunk.transform,ref entries,index);
        Quaternion quat = chunk.transform.localRotation;
        bool pointsUp = Utils.QuaternionApproximatelyEquals(quat,Quaternion.Euler(180f,0f,0f),30f);
        bool pointsDn = Utils.QuaternionApproximatelyEquals(quat,Quaternion.Euler(0,0f,0f),30f);
        MeshRenderer mr = chunk.GetComponent<MeshRenderer>();
        MeshRenderer childMR = null;
        // Shadowcaster mode for ceilings set in DynamicCulling so that if sky is visible we preserve twosided shadows and don't get sun leaks.
        if (mr != null) {
            if (pointsUp && mr.sharedMaterial != Const.a.shadowCaster) mr.shadowCastingMode = ShadowCastingMode.Off;
//             if (mr.sharedMaterial == Const.a.shadowCaster) {
//                 mr.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
//             }
        }
        
        if (curlevel <= 9) {
            bool scaleGood =    Utils.InTol(chunk.transform.localScale.x,1.0f,0.01f)
                            && Utils.InTol(chunk.transform.localScale.y,1.0f,0.01f)
                            && Utils.InTol(chunk.transform.localScale.z,1.0f,0.01f);
            
            float floorHeight = Utils.GetFloorHeight(quat,chunk.transform.position.y);
            if (pointsUp) {
                MarkAsFloor maf = chunk.GetComponent<MarkAsFloor>();
                if (maf == null) maf = chunk.AddComponent<MarkAsFloor>();
                
                maf.floorHeight = floorHeight;
            }
            
            // Align to grid if not rotated
            float yShifted = chunk.transform.localPosition.y;
            if (!(chunk.name == "chunk_stor1_5 (99)" || chunk.name == "chunk_stor1_5 (101)" || chunk.name == "chunk_stor1_5 (105)" || chunk.name == "chunk_stor1_5 (110)" || chunk.name == "chunk_med2_1 (27)")) {
                yShifted = Mathf.Round(chunk.transform.localPosition.y / 0.16f) * 0.16f;
            }
            
            if (Utils.IsAxisAligned(quat) && scaleGood && curlevel < 12 && chunk.name != "chunk_stor1_5 (98)") {
                chunk.transform.localPosition = new Vector3(Mathf.Round(chunk.transform.localPosition.x / 2.56f) * 2.56f,
                                                            yShifted, // Actual Z, stupid Unity
                                                            Mathf.Round(chunk.transform.localPosition.z / 2.56f) * 2.56f);
            } else {
                // Used https://www.h-schmidt.net/FloatConverter/IEEE754.html to
                // determine quantized version that fits exactly in 32bit float with
                // only error of -0.00000002979518463982683078349420677028 after
                // doing sqrt2 over 2 in https://www.mathsisfun.com/calculator-precision.html
                float xzOffsetFor45s = 0.90509665012359619140625f; // = 1.28f * sin(45deg) = 1.28f * (Mathf.Sqrt(2) / 2f)
                
                float sqrtOf2 = 1.41421353816986083984375f;
                
                bool is45 = Utils.ChunkIs45NW_NE_SW_SE_Laterally(quat);
                bool nw45 = false;
                bool ne45 = false;
                bool sw45 = false;
                bool se45 = false;
                bool ceilSlopeX = false;
                bool ceilSlopeNegX = false;
                bool ceilSlopeZ = false;
                bool ceilSlopeNegZ = false;
                
                // This is already shifted close to 0.905.... away from x and z
                // axis in direction needed, so can get nearest value that is
                // 2.56f aligned in x,z (oh gosh I hope all the levels are aligned
                // to 2.56f increments, think I did that at some point heh.
                Vector3 cellCenter = Utils.GetCellCenter(chunk.transform.localPosition);
                
                Vector3 eulerAngs = quat.eulerAngles;
                float angTol = 0.5f; // Must be positive.  This is in degrees.
                if (Utils.QuaternionApproximatelyEquals(quat,Quaternion.Euler(-90f,0f, -45f),angTol)) nw45 = true; // Chunk normal points NW
                if (Utils.QuaternionApproximatelyEquals(quat,Quaternion.Euler(-90f,0f,  45f),angTol)) ne45 = true; // Chunk normal points NE
                if (Utils.QuaternionApproximatelyEquals(quat,Quaternion.Euler(-90f,0f, 135f),angTol)) se45 = true; // Chunk normal points SE
                if (Utils.QuaternionApproximatelyEquals(quat,Quaternion.Euler(-90f,0f,-135f),angTol)) sw45 = true; // Chunk normal points SW
                if (Utils.QuaternionApproximatelyEquals(quat,Quaternion.Euler(0f,0f,-45f),angTol)) ceilSlopeX = true; // Chunk normal points SW
                if (Utils.QuaternionApproximatelyEquals(quat,Quaternion.Euler(0f,0f,45f),angTol)) ceilSlopeNegX = true; // Chunk normal points SW
                if (Utils.QuaternionApproximatelyEquals(quat,Quaternion.Euler(-45f,0f,0f),angTol)) ceilSlopeZ = true; // Chunk normal points SW
                if (Utils.QuaternionApproximatelyEquals(quat,Quaternion.Euler(45f,0f,0f),angTol)) ceilSlopeNegZ = true; // Chunk normal points SW
                if (is45 && curlevel != 12) {
                    Vector3 ofs = chunk.transform.localPosition;
                    if      (nw45) { ofs.x = cellCenter.x - xzOffsetFor45s; ofs.z = cellCenter.z + xzOffsetFor45s; }
                    else if (ne45) { ofs.x = cellCenter.x + xzOffsetFor45s; ofs.z = cellCenter.z + xzOffsetFor45s; }
                    else if (sw45) { ofs.x = cellCenter.x - xzOffsetFor45s; ofs.z = cellCenter.z - xzOffsetFor45s; }
                    else if (se45) { ofs.x = cellCenter.x + xzOffsetFor45s; ofs.z = cellCenter.z - xzOffsetFor45s; }
                    else if (ceilSlopeX) ofs.x = cellCenter.x - xzOffsetFor45s;
                    else if (ceilSlopeNegX) ofs.x = cellCenter.x + xzOffsetFor45s;
                    else if (ceilSlopeZ) ofs.z = cellCenter.z + xzOffsetFor45s;
                    else if (ceilSlopeNegZ) ofs.z = cellCenter.z - xzOffsetFor45s;

                    chunk.transform.localPosition = ofs; // Fix minor alignment issues by putting at exact coords such that card center is at cell center and origin which is 1.28 away along card normal is at correct position.
                    
                    Vector3 scala = chunk.transform.localScale;
                    float scaleTolFine = 0.0005f; // Used to see that it's set wrong for a 45deg card, not precise enough.
                    float scaleTolLoose = 0.05f; // Used to see that it is a 45deg card.
                    if (!Utils.InTol(scala.x,sqrtOf2,scaleTolFine) && Utils.InTol(scala.x,sqrtOf2,scaleTolLoose)) scala.x = sqrtOf2 + 0.001f;
                    if (!Utils.InTol(scala.y,sqrtOf2,scaleTolFine) && Utils.InTol(scala.y,sqrtOf2,scaleTolLoose)) scala.y = sqrtOf2 + 0.001f;
                    if (!Utils.InTol(scala.z,sqrtOf2,scaleTolFine) && Utils.InTol(scala.z,sqrtOf2,scaleTolLoose)) scala.z = sqrtOf2 + 0.001f;
                }
            }
        } else {
            if (chunk.name == "chunk_cyberpanel (3918)") UnityEngine.Debug.Log("Checking chunk_cyberpanel (3918) pid");

            // Cyberspace chunk
            PrefabIdentifier pid = GetPrefabIdentifier(chunk,false);
            if (pid.constIndex == 1 || chunk.name.Contains("chunk_cyberpanelcollisiononly")) { // blockers need to be invisible, not glass.
                if (mr != null) mr.enabled = false;
                else {
                    if (chunk.transform.childCount > 0) {
                        mr = chunk.transform.GetChild(0).GetComponent<MeshRenderer>();
                        if (mr != null) mr.enabled = false;
                    }
                }
            }
        }
        
        if (!((entries.Length - 1) >= index)) return chunk; // Nothing else to load.
        string[] splits = entries[index].Split(':');
        string variableName = splits[0];
        string variableValue = splits[1];
        if (variableName == "BoxCollider.enabled") {
            BoxCollider bcol = chunk.GetComponent<BoxCollider>();
            bcol.enabled = Utils.GetBoolFromString(entries[index],"BoxCollider.enabled"); index++;
            Vector3 vec = new Vector3(1f,1f,1f);
            vec.x = Utils.GetFloatFromString(entries[index],"size.x"); index++;
            vec.y = Utils.GetFloatFromString(entries[index],"size.y"); index++;
            vec.z = Utils.GetFloatFromString(entries[index],"size.z"); index++;
            bcol.size = vec;
            vec.x = Utils.GetFloatFromString(entries[index],"center.x"); index++;
            vec.y = Utils.GetFloatFromString(entries[index],"center.y"); index++;
            vec.z = Utils.GetFloatFromString(entries[index],"center.z"); index++;
            bcol.center = vec;
            if (chunk.transform.childCount >= 1) {
                // Get collisionAid
                Transform subtr = chunk.transform.GetChild(0);
                if (subtr != null) {
                    subtr.gameObject.SetActive(Utils.GetBoolFromString(entries[index],"collisionAid.activeSelf")); index++;
                }
                
                if (chunk.transform.childCount >= 2) { // Remove shadowCaster on thin wafer chunks acting as bridges.
                    subtr = chunk.transform.GetChild(1);
                    if (subtr != null) {
                        MeshRenderer shadRenderer = subtr.GetComponent<MeshRenderer>();
                        if (shadRenderer != null) {
                            if (shadRenderer.sharedMaterial == Const.a.shadowCaster) {
                                MonoBehaviour.DestroyImmediate(shadRenderer);
                                shadRenderer = null;
                                MonoBehaviour.DestroyImmediate(subtr.gameObject);
                            }
                        }
                    }
                }
            }

            if (index < entries.Length) {
                splits = entries[index].Split(':');
                variableName = splits[0];
            }
        } else if (variableName == "material") {
            index++;
            Transform childChunk;
            int matVal = GetMaterialByName(variableValue);
            if (matVal == 86 && constdex == 130) {
                childChunk = chunk.transform.GetChild(0);
                if (childChunk != null) {
                    childMR = childChunk.gameObject.GetComponent<MeshRenderer>();
                    if (childMR != null) {
                        childMR.sharedMaterial = Const.a.genericMaterials[matVal];
                    }
                }
            }
            
            if (mr != null) {
                mr.sharedMaterial = Const.a.genericMaterials[matVal];
            } else if (chunk.transform.childCount > 0) {
                childChunk = chunk.transform.GetChild(0);
                if (childChunk != null) {
                    childMR = childChunk.gameObject.GetComponent<MeshRenderer>();
                    if (childMR != null) {
                        childMR.sharedMaterial = Const.a.genericMaterials[matVal];
                    }
                }
            }
        } else if (constdex == 218) { // chunk_reac2_4 has text on it.
            Transform textr1 = chunk.transform.GetChild(1); // text_decalStopDSS1
            Transform textr2 = chunk.transform.GetChild(2); // text_decalStopDSS1 (1)
            TextLocalization tex1 = textr1.gameObject.GetComponent<TextLocalization>();
            TextLocalization tex2 = textr2.gameObject.GetComponent<TextLocalization>();
            tex1.lingdex = Utils.GetIntFromString(entries[index],"lingdex"); index++;
            tex2.lingdex = Utils.GetIntFromString(entries[index],"lingdex"); index++;
        }

        if (!((entries.Length - 1) >= index)) return chunk; // Nothing else to load.
        splits = entries[index].Split(':');
        variableName = splits[0];
        variableValue = splits[1];
        if (variableName == "BoxCollider.enabled") {
            BoxCollider bcol = chunk.GetComponent<BoxCollider>();
            bcol.enabled = Utils.GetBoolFromString(entries[index],"BoxCollider.enabled"); index++;
            Vector3 vec = new Vector3(1f,1f,1f);
            vec.x = Utils.GetFloatFromString(entries[index],"size.x"); index++;
            vec.y = Utils.GetFloatFromString(entries[index],"size.y"); index++;
            vec.z = Utils.GetFloatFromString(entries[index],"size.z"); index++;
            bcol.size = vec;
            vec.x = Utils.GetFloatFromString(entries[index],"center.x"); index++;
            vec.y = Utils.GetFloatFromString(entries[index],"center.y"); index++;
            vec.z = Utils.GetFloatFromString(entries[index],"center.z"); index++;
            bcol.center = vec;
            if (chunk.transform.childCount >= 1) {
                // Get collisionAid
                Transform subtr = chunk.transform.GetChild(0);
                if (subtr != null) {
                    subtr.gameObject.SetActive(Utils.GetBoolFromString(entries[index],"collisionAid.activeSelf")); index++;
                }
            }

            if (index < entries.Length) {
                splits = entries[index].Split(':');
                variableName = splits[0];
            }
        } else if (variableName == "material") {
            index++;
            Transform childChunk;
            int matVal = GetMaterialByName(variableValue);
            if (matVal == 86 && constdex == 130) {
                childChunk = chunk.transform.GetChild(0);
                if (childChunk != null) {
                    childMR = childChunk.gameObject.GetComponent<MeshRenderer>();
                    if (childMR != null) {
                        childMR.sharedMaterial = Const.a.genericMaterials[matVal];
                    }
                }
            }
            
            if (mr != null) {
                mr.sharedMaterial = Const.a.genericMaterials[matVal];
            } else if (chunk.transform.childCount > 0) {
                childChunk = chunk.transform.GetChild(0);
                if (childChunk != null) {
                    childMR = childChunk.gameObject.GetComponent<MeshRenderer>();
                    if (childMR != null) {
                        childMR.sharedMaterial = Const.a.genericMaterials[matVal];
                    }
                }
            }
        } else if (constdex == 218) { // chunk_reac2_4 has text on it.
            Transform textr1 = chunk.transform.GetChild(1); // text_decalStopDSS1
            Transform textr2 = chunk.transform.GetChild(2); // text_decalStopDSS1 (1)
            TextLocalization tex1 = textr1.gameObject.GetComponent<TextLocalization>();
            TextLocalization tex2 = textr2.gameObject.GetComponent<TextLocalization>();
            tex1.Awake();
            tex1.lingdex = Utils.GetIntFromString(entries[index],"lingdex"); index++;
            tex1.UpdateText();
            tex2.Awake();
            tex2.lingdex = Utils.GetIntFromString(entries[index],"lingdex"); index++;
            tex2.UpdateText();
        }
        
        return chunk;
    }

    private static string SaveLight(GameObject go) {
        Light lit = go.GetComponent<Light>();
        if (lit == null) { Debug.LogError("Missing Light component when trying to SaveLight on " + go.name); return ""; }

        StringBuilder s1 = new StringBuilder();
        s1.Clear();
        Transform tr = go.transform;
        s1.Append(Utils.SaveTransform(go.transform));
        s1.Append(Utils.splitChar);
        s1.Append(Utils.FloatToString(lit.intensity,"intensity"));
        s1.Append(Utils.splitChar);
        s1.Append(Utils.FloatToString(lit.range,"range"));
        s1.Append(Utils.splitChar);
        s1.Append("type:" + lit.type.ToString());
        s1.Append(Utils.splitChar);
        s1.Append(Utils.FloatToString(lit.color.r,"color.r"));
        s1.Append(Utils.splitChar);
        s1.Append(Utils.FloatToString(lit.color.g,"color.g"));
        s1.Append(Utils.splitChar);
        s1.Append(Utils.FloatToString(lit.color.b,"color.b"));
        s1.Append(Utils.splitChar);
        s1.Append(Utils.FloatToString(lit.color.a,"color.a"));
        s1.Append(Utils.splitChar);
        s1.Append(Utils.FloatToString(lit.spotAngle,"spotAngle"));
        s1.Append(Utils.splitChar);
        s1.Append("shadows:" + lit.shadows.ToString());
        s1.Append(Utils.splitChar);
        s1.Append(Utils.FloatToString(lit.shadowStrength,"shadowStrength"));
        s1.Append(Utils.splitChar);
        s1.Append("shadowResolution:" + lit.shadowResolution);
        s1.Append(Utils.splitChar);
        s1.Append(Utils.FloatToString(lit.shadowBias,"shadowBias"));
        s1.Append(Utils.splitChar);
        s1.Append(Utils.FloatToString(lit.shadowNormalBias,"shadowNormalBias"));
        s1.Append(Utils.splitChar);
        s1.Append(Utils.FloatToString(lit.shadowNearPlane,"shadowNearPlane"));
        SaveObject sob = go.GetComponent<SaveObject>();
        if (sob != null) {
            if (sob.saveType == SaveableType.Light) {
                s1.Append(Utils.splitChar);
                s1.Append(LightAnimation.Save(go));
                s1.Append(Utils.splitChar);
                s1.Append(TargetIO.Save(go));
            }
        }
        s1.Append(Utils.splitChar);
        return s1.ToString();
    }

    public static int numLightsWithShadows = 0;
    private static GameObject LoadLight(string[] entries, int lineNum, int curlevel) {
        if (entries.Length <= 1) { Debug.Log("Couldn't load light on line number: " + lineNum.ToString()); return null; }

        int index = 0;
		float readFloatx, readFloaty, readFloatz, readFloatw;
        GameObject go = new GameObject("PointLight" + curlevel.ToString() + "." + lineNum.ToString());
        go.transform.parent = LevelManager.a.GetRequestedLightsStaticImmutableContainer(curlevel).transform;
        Light lit = go.AddComponent<Light>();
        index = Utils.LoadTransform(go.transform,ref entries,index);
        lit.intensity = Utils.GetFloatFromString(entries[index],"intensity"); index++;
        lit.range = Utils.GetFloatFromString(entries[index],"range"); index++;
        lit.type = GetLightTypeFromString(entries[index],"type"); index++;
        readFloatx = Utils.GetFloatFromString(entries[index],"color.r"); index++;
        readFloaty = Utils.GetFloatFromString(entries[index],"color.g"); index++;
        readFloatz = Utils.GetFloatFromString(entries[index],"color.b"); index++;
        readFloatw = Utils.GetFloatFromString(entries[index],"color.a"); index++;
        lit.color = new Color(readFloatx, readFloaty, readFloatz, readFloatw);
        lit.spotAngle = Utils.GetFloatFromString(entries[index],"spotAngle"); index++;
        lit.shadows = GetLightShadowsFromString(entries[index],"shadows"); index++;
        float luminosity = (lit.intensity / (lit.range * lit.range));
        float thresh = Const.a.shadowThreshold;
        if (curlevel >= 10) thresh += 0.015f;
        if (curlevel == 7 || curlevel == 0 || curlevel == 8) thresh += 0.0051f; // makes it 0.0451, heehehe
        if (curlevel == 8) thresh += 0.005f;
        if (luminosity < thresh) lit.shadows = LightShadows.None;
        else {
            if (lit.range > 7f || lit.intensity < 1f) lit.shadows = LightShadows.None;
            else numLightsWithShadows++;
        }
        lit.shadowStrength = Utils.GetFloatFromString(entries[index],"shadowStrength"); index++;
        lit.shadowResolution = GetShadowResFromString(entries[index],"shadowResolution"); index++;
        if (lit.shadows != LightShadows.None && lit.intensity > 2f) lit.shadowResolution = LightShadowResolution.High;
        else lit.shadowResolution = LightShadowResolution.Low;
        lit.shadowBias = Utils.GetFloatFromString(entries[index],"shadowBias"); index++;
        lit.shadowBias = 0.004f;
        lit.shadowNormalBias = Utils.GetFloatFromString(entries[index],"shadowNormalBias"); index++;
        lit.shadowNearPlane = Utils.GetFloatFromString(entries[index],"shadowNearPlane"); index++;
        lit.shadowNearPlane = 0.02f; // Force all to match the player camera value of 1 chunk texel.
        lit.layerShadowCullDistances = shadCullArray;
        lit.cullingMask = litCullingMask;
        Utils.CreateSEGIEmitter(go,curlevel,lineNum,lit);
        return go;
    }

    public static LightType GetLightTypeFromString(string val, string name) {
        string[] splits = val.Split(':');
        if (splits.Length < 2) {
            Debug.Log("Invalid light type value, missing variable name: "
                      + name + " on string:" + val);

            return LightType.Point;
        }

        if (splits[0] != name) {
			UnityEngine.Debug.LogError("BUG: Attempting to parse " + val
								  + " when wanting light type named " + name
								  + ", returning Point as fallback");

			return LightType.Point;
		}
		
		if (     splits[1] == "Spot")        return LightType.Spot;
		else if (splits[1] == "Directional") return LightType.Directional;
		else if (splits[1] == "Rectangle")   return LightType.Rectangle;
		else if (splits[1] == "Disc")        return LightType.Disc;
		return LightType.Point;	
	}

	public static LightShadows GetLightShadowsFromString(string val,
                                                         string name) {
        string[] splits = val.Split(':');
        if (splits.Length < 2) {
            Debug.Log("Invalid light shadow value, missing variable name: "
                      + name + " on string:" + val);

            return LightShadows.Soft;
        }

        if (splits[0] != name) {
			UnityEngine.Debug.LogError("BUG: Attempting to parse " + val
								  + " when wanting shadow named " + name
								  + ", returning soft shadows as fallback");

			return LightShadows.Soft;
		}

		if (     splits[1] == "None") return LightShadows.None;
		else if (splits[1] == "Hard") return LightShadows.Hard;
		return LightShadows.Soft;	
	}

	public static LightShadowResolution GetShadowResFromString(string val,
                                                               string name) {
        string[] splits = val.Split(':');
        if (splits.Length < 2) {
            Debug.Log("Invalid light shadow res value, missing variable name: "
                      + name + " on string:" + val);

            return LightShadowResolution.FromQualitySettings;
        }

        if (splits[0] != name) {
			UnityEngine.Debug.LogError("BUG: Attempting to parse " + val
								  + " when wanting shadow res named " + name
								  + ", returning quality setting as fallback");

			return LightShadowResolution.FromQualitySettings;
		}

		if (     splits[1] == "Low")      return LightShadowResolution.Low; 
		else if (splits[1] == "Medium")   return LightShadowResolution.Medium;
		else if (splits[1] == "High")     return LightShadowResolution.High;
		else if (splits[1] == "VeryHigh") return LightShadowResolution.VeryHigh;
		return LightShadowResolution.FromQualitySettings;	
	}

    private static int GetMaterialByName(string name) {
		switch (name) {
			case "cyberpanel_black":              return 49;
 			case "cyberpanel_blue":               return 50;
			case "cyberpanel_bluegray":           return 51;
			case "cyberpanel_cyan":               return 52;
			case "cyberpanel_cyandark":           return 53;
			case "cyberpanel_gray":               return 54;
			case "cyberpanel_green":              return 55;
			case "cyberpanel_greendark":          return 56;
			case "cyberpanel_orange":             return 57;
			case "cyberpanel_orangedark":         return 58;
			case "cyberpanel_paleorange":         return 59;
			case "cyberpanel_palepurple":         return 60;
			case "cyberpanel_palered":            return 61;
			case "cyberpanel_paleyellow":         return 62;
			case "cyberpanel_purple":             return 63;
			case "cyberpanel_red":                return 64;
			case "cyberpanel_slice45":            return 65;
			case "cyberpanel_slice45_blue":       return 66;
			case "cyberpanel_slice45_bluegray":   return 67;
			case "cyberpanel_slice45_cyan":       return 68;
			case "cyberpanel_slice45_cyandark":   return 69;
			case "cyberpanel_slice45_gray":       return 70;
			case "cyberpanel_slice45_green":      return 71;
			case "cyberpanel_slice45_greendark":  return 72;
			case "cyberpanel_slice45_orange":     return 73;
			case "cyberpanel_slice45_orangedark": return 74;
			case "cyberpanel_slice45_paleorange": return 75;
			case "cyberpanel_slice45_palepurple": return 76;
			case "cyberpanel_slice45_palered":    return 77;
			case "cyberpanel_slice45_paleyellow": return 78;
			case "cyberpanel_slice45_purple":     return 79;
			case "cyberpanel_slice45_red":        return 80;
			case "cyberpanel_slice45_yellow":     return 81;
			case "cyberpanel_touching":           return 82;
			case "cyberpanel_white":              return 83;
			case "cyberpanel_yellow":             return 84;
			case "cyberpanel_yellowdark":         return 85;
			case "pipe_maint2_3_coolant":         return 86;
			case "text_3dwhite":                  return 87;
			case "text_3dred":                    return 88;
			case "text_3dgold":                   return 89;
			case "text_3dgreen":                  return 90;
			case "text_3dblack":                  return 91;
			case "text_3dredStopD":               return 92;
			case "text_3dwhiteStopD":             return 93;
			case "text_3dblackStopD":             return 94;
			case "text_3dgoldStopD":              return 95;
			case "text_3dblueStopD":              return 96;
          //case "text_3dblackStopD":             return 97;  Whoops
			case "text_3dgoldunlit":              return 98;
			case "text_3dgoldunlitoverlay":       return 99;
			case "black_libraryscreen":          return 100;
            case "grass_mutatedorange":          return 101;
		}
		
		return 0;
	}

    public static SaveObject GetPrefabSaveObject(GameObject go) {
        SaveObject so = go.GetComponent<SaveObject>();
		if (so == null) {
			if (go.transform.childCount > 0) { // Exception for some prefabs.
				so = go.transform.GetChild(0).GetComponent<SaveObject>();
			}
		}

		return so;
    }
    
    public static PrefabIdentifier GetPrefabIdentifier(GameObject go, bool sob) {
		PrefabIdentifier prefID;
		if (go.name == "Player") { // Need to get it on child PlayerCapsule.
            prefID = go.transform.GetChild(0).GetComponent<PrefabIdentifier>();
        } else {
            prefID = go.GetComponent<PrefabIdentifier>();
        }
		
        bool isNotALight = true;
        bool isNotATransform = true;
		if (sob) {
            SaveObject so = GetPrefabSaveObject(go);
            if (so != null) {
                isNotALight = (so.saveType != SaveableType.Light);
                isNotATransform = (so.saveType != SaveableType.Transform);
            }
        }

        if (prefID == null) {
            if (go.transform.childCount > 1) {
                prefID = go.transform.GetChild(0).GetComponent<PrefabIdentifier>();
                if (prefID == null && isNotATransform && isNotALight) {
                    Debug.Log("No PrefabIdentifier on " + go.name);
                }
            }
        }

		return prefID;
    }
    
        
    private static AudioReverbPreset GetAudioReverbPresetFromInt(int val) {
        if (Enum.IsDefined(typeof(AudioReverbPreset), val)) {
            return (AudioReverbPreset)val; // Cast the int to AudioReverbPreset
        } else {
            Debug.LogWarning($"Invalid AudioReverbPreset value: {val}. Defaulting to 'Off'.");
            return AudioReverbPreset.Off; // Default or fallback preset
        }
    }

    // Get the integer value from an AudioReverbPreset
    private static int GetIntFromAudioReverbPreset(AudioReverbPreset preset) {
        return (int)preset;
    }
}
