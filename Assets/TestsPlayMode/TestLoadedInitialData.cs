using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Tests {
    public class TestLoadedInitialData {
        private bool sceneLoaded = false;

        //private IEnumerator LoadSceneAsync(string sceneName) {
        //    var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        //    while (!asyncLoad.isDone) {
        //        yield return null;
        //    }
        //}

        public void RunBeforeAnyTests() {
            if (sceneLoaded) return;

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("CitadelScene");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.name == "CitadelScene") {
                // Unsubscribe from the event to avoid handling it multiple
                // times
                SceneManager.sceneLoaded -= OnSceneLoaded;
                sceneLoaded = true; // Indicate that the scene is loaded.
            }
        }


				//readline = dataReader.ReadLine();
				//if (readline == null) continue; // just in case
				//string[] entries = readline.Split(logSplitChar);
				//readIndexOfLog = Utils.GetIntFromString(entries[i]); i++;
				//readLogImageLHIndex = Utils.GetIntFromString(entries[i]); i++;
				//readLogImageRHIndex = Utils.GetIntFromString(entries[i]); i++;
				//audioLogImagesRefIndicesLH[readIndexOfLog] = readLogImageLHIndex;
				//audioLogImagesRefIndicesRH[readIndexOfLog] = readLogImageRHIndex;
				//audiologNames[readIndexOfLog] = entries[i]; i++;
				//audiologSenders[readIndexOfLog] = entries[i]; i++;
				//audiologSubjects[readIndexOfLog] = entries[i]; i++;
				//audioLogType[readIndexOfLog] = Utils.GetAudioLogTypeFromInt(Utils.GetIntFromString(entries[i])); i++;
				//audioLogLevelFound[readIndexOfLog] = Utils.GetIntFromString(entries[i]); i++;
				//readLogText = entries[i]; i++;
				//// handle extra commas within the body text and append remaining portions of the line
				//if (entries.Length > 8) {
				//	for (int j=9;j<entries.Length;j++) {
				//		readLogText = (readLogText +"," + entries[j]);  // combine remaining portions of text after other commas and add comma back
				//	}
				//}
				//audioLogSpeech2Text[readIndexOfLog] = readLogText;

        [UnityTest]
        public IEnumerator CheckLogsLoadedHaveAssignedAudio() {
            RunBeforeAnyTests();
            yield return new WaitUntil(() => sceneLoaded);

            yield return new WaitForSeconds(2f);
            
            bool check = Const.a.audioLogType.Length == Const.a.audioLogs.Length;
            string msg = "Const.a.audioLogType length of "
                         + Const.a.audioLogType.Length.ToString()
                         + " not equal to Const.a.audioLogs length of "
                         + Const.a.audioLogs.Length.ToString();
            Assert.That(check,msg);
            
            //for (int i=0; i< Const.a.audioLogType.Length;i++) {
            for (int i=0; i< Const.a.audioLogs.Length;i++) {
                if (Const.a.audioLogType[i] == AudioLogType.TextOnly) continue;
                if (Const.a.audioLogType[i] == AudioLogType.Papers) continue;
                if (Const.a.audioLogType[i] == AudioLogType.Vmail) continue;
                
                // 94 needs shodan_enjoyyourvictory.wav
                if (i == 94) continue;
                
                // Only normal and emails have .wav files
                // 6 is a known TextOnly and has null.wav assigned to it.
                check = Const.a.audioLogs[i] != Const.a.audioLogs[6];
                msg = "audioLogs " + i.ToString() + " using null.wav";
                Assert.That(check,msg);
            }
        }
    }
}
