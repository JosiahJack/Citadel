using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionHandler : MonoBehaviour {
    private bool transitionActive;
    public bool setActiveAtNext;
    private bool loadActive;
    public int saveGameIndex;
    private AsyncOperation unloaderStatus;
    private int sceneIndexToLoad;
    private float loadDelayFinished;
    public int diffCombatCarryover;
    public int diffCyberCarryover;
    public int diffPuzzleCarryover;
    public int diffMissionCarryover;

    void Update() {
        if (setActiveAtNext) {
            setActiveAtNext = false;
            Scene mainScene = SceneManager.GetSceneByName("CitadelScene");
            if (mainScene != null) SceneManager.SetActiveScene(mainScene);
            Debug.Log("Reinit FULL const tables");
            Const.a.Awake();
            Const.a.difficultyCombat = diffCombatCarryover;
            Const.a.difficultyMission = diffMissionCarryover;
            Const.a.difficultyPuzzle = diffPuzzleCarryover;
            Const.a.difficultyCyber = diffCyberCarryover;
            Debug.Log("Const.a.difficultyCombat " + Const.a.difficultyCombat.ToString());
            // Must delay by 1 frame.
            Scene loadScene = SceneManager.GetSceneByName("LoadScene");
            Debug.Log("Unloading LoadScene");
            //if (loadScene != null) SceneManager.UnloadSceneAsync(loadScene);
        }

        if (transitionActive) {
            if (unloaderStatus == null) { transitionActive = false; return; }
            if (!unloaderStatus.isDone) return;

            Scene loadScene = SceneManager.GetSceneByName("LoadScene");
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            SceneManager.LoadScene(sceneIndexToLoad); // Reload. it. all.
            setActiveAtNext = true;
            transitionActive = false;
        } else if (loadActive) {
            if (loadDelayFinished < Time.time) {
			    Const.a.Load(saveGameIndex,true); // Actual hook to load a game!
                loadActive = false;
            }
        }
    }

    public void Load() {
        loadDelayFinished = Time.time + 2.0f; // Ensure all Awake/Start's ran.
        loadActive = true;
        transitionActive = false;
    }

    public void Reload(int scenedex, ref AsyncOperation aso) {
        unloaderStatus = aso;
        sceneIndexToLoad = scenedex;
        loadActive = false;
        transitionActive = true;
        setActiveAtNext = false;
    }
}
