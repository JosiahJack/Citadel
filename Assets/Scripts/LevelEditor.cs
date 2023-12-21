using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Level Editor for creating new levels or modifying the base game's levels.
public class LevelEditor : MonoBehaviour {
    public bool inEditMode;

    public static LevelEditor a;

    void Awake() {
        a = this;
    }

    private void EditorEntry() {
        inEditMode = true;
        PauseScript.a.PauseSystems();
        PlayerMovement.a.ConsoleDisable();
    }

    public void EditorExit() {
        inEditMode = false;
        PauseScript.a.UnpauseSystems();
    }

    void Update() {
        if (!Const.a.editMode) return;

        if (!inEditMode) EditorEntry();
    }
}
