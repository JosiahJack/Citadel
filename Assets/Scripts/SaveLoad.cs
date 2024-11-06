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

        if (ConsoleEmulator.ConstIndexIsGeometry(pid.constIndex)) {
            return SaveGeometry(go,pid);
        } else if (ConsoleEmulator.ConstIndexIsDynamicObject(pid.constIndex)) {
            return SaveObject.Save(go);
        } else if (ConsoleEmulator.ConstIndexIsDoor(pid.constIndex)) {
            return SaveObject.Save(go);
        } else if (ConsoleEmulator.ConstIndexIsStaticObjectSaveable(pid.constIndex)) {
            return SaveObject.Save(go);
        } else if (ConsoleEmulator.ConstIndexIsNPC(pid.constIndex)) {
            return SaveObject.Save(go);
        } else if (ConsoleEmulator.ConstIndexIsStaticObjectImmutable(pid.constIndex)) {
           return SaveStaticImmutable(go,pid);
        } else {
            Light lit = go.GetComponent<Light>();
            if (lit != null) {
                return SaveLight(go);
            } else return "";
        }
    }
    
    public static void LoadPrefab(ref string[] entries, int lineNum, int curlevel) {
        int constIndex = Utils.GetIntFromString(entries[0],"constIndex");
        int saveID = Utils.GetIntFromString(entries[2],"SaveID");
        if (ConsoleEmulator.ConstIndexIsGeometry(constIndex)) {
            LoadGeometry(entries,lineNum,curlevel);
        } else if (ConsoleEmulator.ConstIndexIsDynamicObject(constIndex)
                   || ConsoleEmulator.ConstIndexIsDoor(constIndex)
                   || ConsoleEmulator.ConstIndexIsStaticObjectSaveable(constIndex)
                   || ConsoleEmulator.ConstIndexIsNPC(constIndex)) {

            GameObject container = LevelManager.a.GetRequestedLevelDynamicContainer(LevelManager.a.currentLevel);
			GameObject newGO = ConsoleEmulator.SpawnDynamicObject(constIndex,curlevel,false,container,saveID);
			PrefabIdentifier prefID = SaveLoad.GetPrefabIdentifier(newGO,true);
			if (newGO != null) SaveObject.Load(newGO,ref entries,lineNum,prefID);
        } else if (ConsoleEmulator.ConstIndexIsStaticObjectImmutable(constIndex)) {
           LoadStaticImmutable(entries,lineNum,curlevel);
        }
    }
    
    // GameObect already null checked by originator.
    // PrefabIdentifier already null checked by originator.  
    private static string SaveStaticImmutable(GameObject go, PrefabIdentifier pid) {
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
            s1.Append(TargetIO.Save(go,pid));
        } else if (pid.constIndex == 574) { // prop_healingbed
            HealingBed heb = go.GetComponent<HealingBed>();
            s1.Append(splitChar);
            s1.Append(Utils.BoolToString(heb.broken,"broken"));
        } else if (pid.constIndex == 592 || pid.constIndex == 593) { // text_decal, text_decalStopDSS1
            TextMesh tm = go.GetComponent<TextMesh>();
            s1.Append(splitChar);
            s1.Append(Utils.SaveString(tm.text,"text"));
            s1.Append(splitChar);
            TextLocalization tz = go.GetComponent<TextLocalization>();
            s1.Append(Utils.UintToString(tz.lingdex,"lingdex"));
            s1.Append(splitChar);
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            Material mat = mr.material;
            if (mat.name == "text_3dwhite") s1.Append(Utils.UintToString(49,"matIndex"));
            else if (mat.name == "text_3dgold") s1.Append(Utils.UintToString(51,"matIndex"));
            else if (mat.name == "text_3dgreen") s1.Append(Utils.UintToString(52,"matIndex"));
            else if (mat.name == "text_3dblack") s1.Append(Utils.UintToString(53,"matIndex"));
            else if (mat.name == "text_3dredStopD") s1.Append(Utils.UintToString(54,"matIndex"));
            else if (mat.name == "text_3dwhiteStopD") s1.Append(Utils.UintToString(55,"matIndex"));
            else if (mat.name == "text_3dblackStopD") s1.Append(Utils.UintToString(56,"matIndex"));
            else if (mat.name == "text_3dgoldStopD") s1.Append(Utils.UintToString(57,"matIndex"));
            else if (mat.name == "text_3dblueStopD") s1.Append(Utils.UintToString(58,"matIndex"));
            else if (mat.name == "text_3dblackStopD") s1.Append(Utils.UintToString(59,"matIndex"));
            else if (mat.name == "text_3dgoldunlit") s1.Append(Utils.UintToString(60,"matIndex"));
            else if (mat.name == "text_3dgoldunlitoverlay") s1.Append(Utils.UintToString(61,"matIndex"));
            else s1.Append(Utils.UintToString(50,"matIndex")); // text_3dred
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
        } else if (pid.constIndex == 599) { // trigger_radiation
            Radiation rad = go.GetComponent<Radiation>();
            s1.Append(splitChar);
            s1.Append(Utils.FloatToString(rad.radiationAmount,"radiationAmount"));
            s1.Append(splitChar);
            s1.Append(Utils.SaveBoxCollider(go));
        } else if (pid.constIndex == 603) { // us_paperlog
            PaperLog plog = go.GetComponent<PaperLog>();
            s1.Append(splitChar);
            s1.Append(Utils.UintToString(plog.logIndex,"logIndex"));
        }

        return s1.ToString();
    }

    private static void LoadStaticImmutable(string[] entries, int lineNum, int curlevel) {
        if (entries.Length <= 1) { 
            Debug.Log("Can't load static immutable from line "
                      + lineNum.ToString() + ", line had only one or no "
                      + "entries[]");
            return;
        }

        int index = 0;
        int constIndex = Utils.GetIntFromString(entries[index],"constIndex"); index++;
        if (!ConsoleEmulator.ConstIndexIsGeometry(constIndex)) {
            Debug.Log("Load stat imm invalid constIndex: " + constIndex.ToString());
            return;
        }

        GameObject go = ConsoleEmulator.SpawnDynamicObject(constIndex,curlevel,
                                                           false,null,0);

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
            
            PrefabIdentifier prefID = go.GetComponent<PrefabIdentifier>();
            index = TargetIO.Load(go,ref entries,index,true,prefID);
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
        } else if (constIndex == 599) { // trigger_radiation
            Radiation rad = go.GetComponent<Radiation>();
            rad.radiationAmount = Utils.GetFloatFromString(entries[index],"radiationAmount"); index++;
            index = Utils.LoadBoxCollider(go, ref entries,index);
        } else if (constIndex == 603) { // us_paperlog
            PaperLog plog = go.GetComponent<PaperLog>();
            plog.logIndex = Utils.GetIntFromString(entries[index],"logIndex"); index++;
        }
    }

    // GameObect already null checked by originator.
    // PrefabIdentifier already null checked by originator.
    private static string SaveGeometry(GameObject go, PrefabIdentifier pid) {
        bool hasBoxColliderOverride = false;
        bool hasTextOverride = false;
        string materialOverride = "";
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

    public static void LoadGeometry(string[] entries, int lineNum, int curlevel) {
        if (entries.Length <= 1) { 
            Debug.Log("Can't load geometry from line " + lineNum.ToString()
                      + ", line had only one or no entries[]");
            return;
        }

        int index = 0;
        int constdex = Utils.GetIntFromString(entries[index],"constIndex"); index++;
        if (!ConsoleEmulator.ConstIndexIsGeometry(constdex)) {
            Debug.Log("Invalid constdex loading geometry: " + constdex.ToString());
            return;
        }

        GameObject chunk = ConsoleEmulator.SpawnDynamicObject(constdex,curlevel,
                                                              false,null,0);
        if (chunk == null) return;

        chunk.name = entries[index]; index++;
        index = Utils.LoadTransform(chunk.transform,ref entries,index);
        if (!((entries.Length - 1) >= index)) return; // Nothing else to load.

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
            }

            if (index < entries.Length) {
                splits = entries[index].Split(':');
                variableName = splits[0];
            }
        } else if (variableName == "material") {
            Transform childChunk;
            MeshRenderer mr = chunk.GetComponent<MeshRenderer>();
            int matVal = GetMaterialByName(variableValue);
            if (matVal == 86 && constdex == 130) {
                childChunk = chunk.transform.GetChild(0);
                if (childChunk != null) {
                    mr = childChunk.gameObject.GetComponent<MeshRenderer>();
                    if (mr != null) {
                        mr.sharedMaterial = Const.a.genericMaterials[matVal];
                    }
                }
            }
            
            if (mr != null) {
                mr.sharedMaterial = Const.a.genericMaterials[matVal];
            } else if (chunk.transform.childCount > 0) {
                childChunk = chunk.transform.GetChild(0);
                if (childChunk != null) {
                    mr = childChunk.gameObject.GetComponent<MeshRenderer>();
                    if (mr != null) {
                        mr.sharedMaterial = Const.a.genericMaterials[matVal];
                    }
                }
            }
        }

        if (constdex == 218) { // chunk_reac2_4 has text on it.
            if (entries.Length - 1 >= index) {
                Transform textr1 = chunk.transform.GetChild(1); // text_decalStopDSS1
                Transform textr2 = chunk.transform.GetChild(2); // text_decalStopDSS1 (1)
                TextLocalization tex1 = textr1.gameObject.GetComponent<TextLocalization>();
                TextLocalization tex2 = textr2.gameObject.GetComponent<TextLocalization>();
                tex1.lingdex = Utils.GetIntFromString(entries[index],"lingdex"); index++;
                tex2.lingdex = Utils.GetIntFromString(entries[index],"lingdex"); index++;
            }
        }
    }

    public static string SaveLight(GameObject go) {
        Light lit = go.GetComponent<Light>();
        if (lit == null) return "";

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
        return s1.ToString();
    }
    
    public static void LoadLight(string[] entries, int lineNum, int curlevel) {
        if (entries.Length <= 1) { Debug.Log("Couldn't load light on line number: " + lineNum.ToString()); return; }

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
        if (lit.intensity < 0.3f || (lit.range > 7f && lit.intensity < 2f)) lit.shadows = LightShadows.None;
        lit.shadowStrength = Utils.GetFloatFromString(entries[index],"shadowStrength"); index++;
        lit.shadowResolution = GetShadowResFromString(entries[index],"shadowResolution"); index++;
        lit.shadowBias = Utils.GetFloatFromString(entries[index],"shadowBias"); index++;
        lit.shadowNormalBias = Utils.GetFloatFromString(entries[index],"shadowNormalBias"); index++;
        lit.shadowNearPlane = Utils.GetFloatFromString(entries[index],"shadowNearPlane"); index++;
        lit.layerShadowCullDistances = shadCullArray;
        lit.cullingMask = litCullingMask;
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
            SaveObject so = SaveLoad.GetPrefabSaveObject(go);
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
}
