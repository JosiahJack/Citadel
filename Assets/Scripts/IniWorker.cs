using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
 
public class INIWorker {
	private static string path = Path.Combine(Application.streamingAssetsPath, "Config.ini");
    private static Dictionary<string, Dictionary<string, string>> IniDictionary = new Dictionary<string, Dictionary<string, string>>();
    private static bool Initialized = false;

    /// Sections list
    public enum Sections {
        Section01,
    }
    /// Keys list
    public enum Keys {
        Key01,
        Key02,
        Key03,
    }

	// USE IT LIKE THIS
	//IniFile.IniWriteValue("Stats","Time",_inTime.ToString());
 
    private static bool FirstRead() {
        if (File.Exists(path)) {
            using (StreamReader sr = new StreamReader(path)) {
                string line;
                string theSection = System.String.Empty;
                string theKey = System.String.Empty;
                string theValue = System.String.Empty;
				//while (!string.IsNullOrEmpty(line = sr.ReadLine())) {
				while (!((line = sr.ReadLine()) == null)) {
                    line.Trim();
                    if (line.StartsWith("[") && line.EndsWith("]")) {
                        theSection = line.Substring(1, line.Length - 2); // Get the section name
                    } else {
						if (line.StartsWith("//") || line.Length <= 1) continue;
                        string[] ln = line.Split(new char[] { '=' });
                        theKey = ln[0].Trim();
                        theValue = ln[1].Trim();
                    }
                    if (theSection == System.String.Empty || theKey == System.String.Empty || theValue == System.String.Empty)
                        continue;
                    PopulateIni(theSection, theKey, theValue); // Load the data from the ini into the dictionary
                }
            }
		}
        return true;
    }
 
    private static void PopulateIni(string _Section, string _Key, string _Value) {
        if (IniDictionary.ContainsKey(_Section)) {
            if (IniDictionary[_Section].ContainsKey(_Key))
                IniDictionary[_Section][_Key] = _Value; // Update existing key with value
            else
                IniDictionary[_Section].Add(_Key, _Value); // Create new key with value
        } else {
            Dictionary<string, string> neuVal = new Dictionary<string, string>();
            neuVal.Add(_Key.ToString(), _Value);  // Create key|value pair
            IniDictionary.Add(_Section.ToString(), neuVal); // Add key|value pair to the section inside the dictionary
        }
    }
    /// <summary>
    /// Write data to INI file. Section and Key no in enum.
    /// </summary>
    /// <param name="_Section"></param>
    /// <param name="_Key"></param>
    /// <param name="_Value"></param>
    public static void IniWriteValue(string _Section, string _Key, string _Value) {
        if (!Initialized)
            FirstRead();
        PopulateIni(_Section, _Key, _Value);
        //write ini
        WriteIni();
    }
    /// <summary>
    /// Write data to INI file. Section and Key bound by enum
    /// </summary>
    /// <param name="_Section"></param>
    /// <param name="_Key"></param>
    /// <param name="_Value"></param>
    public static void IniWriteValue(Sections _Section, Keys _Key, string _Value) {
        IniWriteValue(_Section.ToString(), _Key.ToString(), _Value);
    }
 
    private static void WriteIni() {
        using (StreamWriter sw = new StreamWriter(path)) {
			bool init = true;
			sw.WriteLine("// Citadel Configuration File");
            foreach (KeyValuePair<string, Dictionary<string, string>> sezioni in IniDictionary) {
				if (!init) sw.WriteLine(System.String.Empty);
                sw.WriteLine("[" + sezioni.Key.ToString() + "]");
				init = false;
                foreach (KeyValuePair<string, string> chiave in sezioni.Value) {
                    // value must be in one line
                    string vale = chiave.Value.ToString();
                    vale = vale.Replace(Environment.NewLine, " ");
                    vale = vale.Replace("\r\n", " ");
                    sw.WriteLine(chiave.Key.ToString() + " = " + vale);
                }
            }
        }
    }
    /// <summary>
    /// Read data from INI file. Section and Key bound by enum
    /// </summary>
    /// <param name="_Section"></param>
    /// <param name="_Key"></param>
    /// <returns></returns>
    public static string IniReadValue(Sections _Section, Keys _Key) {
        if (!Initialized)
            FirstRead();
        return IniReadValue(_Section.ToString(), _Key.ToString());
    }
    /// <summary>
    /// Read data from INI file. Section and Key no in enum.
    /// </summary>
    /// <param name="_Section"></param>
    /// <param name="_Key"></param>
    /// <returns></returns>
    public static string IniReadValue(string _Section, string _Key) {
        if (!Initialized)
            FirstRead();
        if (IniDictionary.ContainsKey(_Section))
            if (IniDictionary[_Section].ContainsKey(_Key))
                return IniDictionary[_Section][_Key];
        return null;
    }
}