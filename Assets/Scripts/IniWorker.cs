using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text;

// Initially written by Tito-to
// Fixes by WizzardMaker.
// Updated myself for Citadel.  - Josiah
// Originally sourced here: https://forum.unity.com/threads/read-write-ini-file-o-config-file.244770/ 
public class INIWorker {
    private static bool Initialized = false;
	private static string path;

    private static Dictionary<string, Dictionary<string, string>> IniDictionary = 
        new Dictionary<string, Dictionary<string, string>>();

    private static bool FirstRead() {
		if (Application.platform == RuntimePlatform.Android) {
		    return false;
			//Utils.ConfirmExistsInPersistentDataMakeIfNot("Config.ini");
			//path = Utils.SafePathCombine(Application.persistentDataPath,"Config.ini");
		} else {
		    Utils.ConfirmExistsInStreamingAssetsMakeIfNot("Config.ini");
            path = Utils.SafePathCombine(Application.streamingAssetsPath,"Config.ini");
        }

        if (File.Exists(path)) {
            using (StreamReader sr = new StreamReader(path)) {
                string line;
                string theSection = System.String.Empty;
                string theKey = System.String.Empty;
                string theValue = System.String.Empty;
				char[] chs;
				while (!((line = sr.ReadLine()) == null)) {
                    line.Trim();
					chs = line.ToCharArray();

					if (chs.Length < 2) continue;

                    if (chs[0] == '[' && chs[(chs.Length - 1)] == ']') {
                        theSection = line.Substring(1, line.Length - 2); // Get the section name
                    } else {
						// if (line.StartsWith("//") || line.Length <= 1) continue;
						if (chs[0] == '/' && chs[1] == '/') continue;

                        string[] ln = line.Split(new char[] { '=' });
                        theKey = ln[0].Trim();
                        theValue = ln[1].Trim();
                    }

                    if (theSection == System.String.Empty
                        || theKey == System.String.Empty
                        || theValue == System.String.Empty) {
                        continue;
                    }

                    PopulateIni(theSection, theKey, theValue); // Load the data from the ini into the dictionary
                }
				Initialized = true;
            }
		}
        return true;
    }
 
    private static void PopulateIni(string _Section, string _Key, string _Value) {
        if (IniDictionary.ContainsKey(_Section)) {
            if (IniDictionary[_Section].ContainsKey(_Key)) {
                IniDictionary[_Section][_Key] = _Value; // Update existing key with value
            } else {
                IniDictionary[_Section].Add(_Key, _Value); // Create new key with value
            }
        } else {
            Dictionary<string, string> neuVal = new Dictionary<string, string>();
            neuVal.Add(_Key, _Value);  // Create key|value pair
            IniDictionary.Add(_Section, neuVal); // Add key|value pair to the section inside the dictionary
        }
    }

    // Write data to INI file. Section and Key no in enum.
    public static void IniWriteValue(string _Section, string _Key, string _Value) {
        if (!Initialized) FirstRead();
        PopulateIni(_Section, _Key, _Value);
        WriteIni();
    }
 
    private static void WriteIni() {
		if (Application.platform == RuntimePlatform.Android) {
            return; // Ah to heck with Google
		}
		
		Utils.ConfirmExistsInStreamingAssetsMakeIfNot("Config.ini");
        path = Utils.SafePathCombine(Application.streamingAssetsPath,"Config.ini");
        using (StreamWriter sw = new StreamWriter(path,false,Encoding.ASCII)) {
			bool init = true;
			sw.WriteLine("// Citadel Configuration File");
            foreach (KeyValuePair<string, Dictionary<string,
                     string>> sezioni in IniDictionary) {
				if (!init) sw.WriteLine(System.String.Empty);
                sw.WriteLine("[" + sezioni.Key + "]");
				init = false;
                foreach (KeyValuePair<string, string> chiave in sezioni.Value) {
                    // value must be in one line
                    string vale = chiave.Value;
                    vale = vale.Replace(Environment.NewLine, " ");
                    vale = vale.Replace("\r\n", " ");
                    sw.WriteLine(chiave.Key + " = " + vale);
                }
            }
        }
    }

    // Read data from INI file. Section and Key no in enum.
    public static string IniReadValue(string _Section, string _Key) {
        if (!Initialized) FirstRead();
        if (IniDictionary.ContainsKey(_Section))
            if (IniDictionary[_Section].ContainsKey(_Key))
                return IniDictionary[_Section][_Key];
        return null;
    }
}
