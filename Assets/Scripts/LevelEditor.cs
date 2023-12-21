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

        if (MouseLookScript.a.inventoryMode) ToolMode();
    }

    void ToolMode() {
        if (Input.GetKey(KeyCode.A)) { // Add Cell Tool

        } else if (Input.GetKey(KeyCode.D)) { // Delete Cell Tool
            Debug.Log("Delete Cell Tool");
        } else if (Input.GetKey(KeyCode.E)) { // Enemy Insert Tool
            Debug.Log("Enemy Insert Tool");
        } else if (Input.GetKey(KeyCode.T)) { // Item Insert Tool
            Debug.Log("Item Insert Tool");
        } else if (Input.GetKey(KeyCode.W)) { // Light Insert Tool
            Debug.Log("Light Insert Tool");
        } else if (Input.GetKey(KeyCode.Q)) { // Quest and Misc Insert Tool
            Debug.Log("Quest and Misc Insert Tool");
        } else if (Input.GetKey(KeyCode.Z)) { // Cell Floor Height Tool
            Debug.Log("Cell Floor Height Tool");
        } else if (Input.GetKey(KeyCode.C)) { // Cell Ceiling Height Tool
            Debug.Log("Cell Ceiling Height Tool");
        }
    }
}
