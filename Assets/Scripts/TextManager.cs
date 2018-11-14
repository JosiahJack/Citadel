using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

public class TextManager : MonoBehaviour {
    public string[] stringTable;

    //Instance container variable
    public static TextManager a;

    // Instantiate it so that it can be accessed globally. MOST IMPORTANT PART!!
    // =========================================================================
    void Awake() {
        a = this;
    }

    private void Start() {
        LoadTextForLanguage(1); //initialize with US English (index 0)
    }

    public void LoadTextForLanguage(int lang) {
        string readline; // variable to hold each string read in from the file
        int currentline = 0;
        string sourceFile = "/StreamingAssets/text_english.txt";

        // TODO: support other languages
        switch (lang) {
            case 0:
                sourceFile = "/StreamingAssets/text_english.txt";
                break;
            case 1:
                sourceFile = "/StreamingAssets/text_espanol.txt";
                break;
            case 2:
                sourceFile = "/StreamingAssets/text_francois.txt";
                break;
            default:
                sourceFile = "/StreamingAssets/text_english.txt";
                break;
        }
        StreamReader dataReader = new StreamReader(Application.dataPath + sourceFile, Encoding.Default);
        using (dataReader) {
            do {
                // Read the next line
                readline = dataReader.ReadLine();
                if (currentline < stringTable.Length)
                    stringTable[currentline] = readline;
                currentline++;
            } while (!dataReader.EndOfStream);
            dataReader.Close();
            return;
        }
    }
}
