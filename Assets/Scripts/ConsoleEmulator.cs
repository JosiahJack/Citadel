using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

// CHEAT CODES you cheaty cheatface you!
// This is my attempt at a terminal emulator, because you know, why not throw
// in another new system while we are making all these systems and getting
// shocked.
//
// This should support memory for 7 most recent commands, accessible using up
// arrow or down arrow to change the entered text to the commands in memory.
//
// Does not support tab completion!  What do I look like a wizard?
public static class ConsoleEmulator {
	public static string[] lastCommand;
	public static int consoleMemdex;
	public static GameObject lastSpawnedGO;

	public static void ConsoleUpdate() {
        if (GetInput.a.Console()) PlayerMovement.a.ToggleConsole();
		if (PlayerMovement.a.consoleActivated) {

			if (!String.IsNullOrEmpty(PlayerMovement.a.consoleentryText.text)) {
				if (PlayerMovement.a.consoleplaceholderText.activeSelf) {
                    PlayerMovement.a.consoleplaceholderText.SetActive(false);
                }
			} else {
				if (!PlayerMovement.a.consoleplaceholderText.activeSelf) {
                    PlayerMovement.a.consoleplaceholderText.SetActive(true);
                }
			}

			if (Input.GetKeyUp(KeyCode.UpArrow)) SetToCommandMoreDistant();
			if (Input.GetKeyUp(KeyCode.DownArrow)) SetToCommandMoreRecent();
			if ((Input.GetKeyUp(KeyCode.Return)
                  || Input.GetKeyUp(KeyCode.KeypadEnter)
				  || Input.GetKeyDown(KeyCode.JoystickButton0))
                && !PauseScript.a.mainMenu.activeSelf == true) {

          	    string enteredText = PlayerMovement.a.consoleinpFd.text;
                ConsoleEntry(enteredText);
            }

			if (Input.GetKeyDown(KeyCode.LeftControl) ||
				Input.GetKeyDown(KeyCode.RightControl)) {

				if (Input.GetKeyUp(KeyCode.U)) {
					PlayerMovement.a.consoleinpFd.text = "";
				}
			}

		} else {
			if (PlayerMovement.a.consoleplaceholderText.activeSelf) {
                PlayerMovement.a.consoleplaceholderText.SetActive(false);
            }
		}
	}

    private static void SetToCommandMoreDistant() {
		string val = PlayerMovement.a.lastCommand0;
		switch(consoleMemdex) {
			case 0: val = PlayerMovement.a.lastCommand0; break;
			case 1: val = PlayerMovement.a.lastCommand1; break;
			case 2: val = PlayerMovement.a.lastCommand2; break;
			case 3: val = PlayerMovement.a.lastCommand3; break;
			case 4: val = PlayerMovement.a.lastCommand4; break;
			case 5: val = PlayerMovement.a.lastCommand5; break;
			case 6: val = PlayerMovement.a.lastCommand6; break;
		}

        if (string.IsNullOrWhiteSpace(val)) return;

        PlayerMovement.a.consoleinpFd.text = val;
		PlayerMovement.a.consoleinpFd.MoveTextEnd(false);
		PlayerMovement.a.consoleinpFd.selectionAnchorPosition = PlayerMovement.a.consoleinpFd.caretPosition;
		PlayerMovement.a.consoleinpFd.selectionFocusPosition = PlayerMovement.a.consoleinpFd.caretPosition;
        consoleMemdex++;
        if (consoleMemdex > 6) consoleMemdex = 6;
    }

    private static void SetToCommandMoreRecent() {
		if (consoleMemdex <= 0) return;

		string val = PlayerMovement.a.lastCommand0;
		switch(consoleMemdex) {
			case 0: val = PlayerMovement.a.lastCommand0; break;
			case 1: val = PlayerMovement.a.lastCommand1; break;
			case 2: val = PlayerMovement.a.lastCommand2; break;
			case 3: val = PlayerMovement.a.lastCommand3; break;
			case 4: val = PlayerMovement.a.lastCommand4; break;
			case 5: val = PlayerMovement.a.lastCommand5; break;
			case 6: val = PlayerMovement.a.lastCommand6; break;
		}
        if (string.IsNullOrWhiteSpace(val)) { consoleMemdex = 0; return; }

        PlayerMovement.a.consoleinpFd.text = val;
		PlayerMovement.a.consoleinpFd.MoveTextEnd(false);
		PlayerMovement.a.consoleinpFd.selectionAnchorPosition = PlayerMovement.a.consoleinpFd.caretPosition;
		PlayerMovement.a.consoleinpFd.selectionFocusPosition = PlayerMovement.a.consoleinpFd.caretPosition;
        consoleMemdex--;
        if (consoleMemdex < 0) consoleMemdex = 0;
    }

    private static void ShiftLastCommand(string entry) {
        if (string.IsNullOrWhiteSpace(entry)) return; // Only remember real cmd.

        lastCommand[6] = lastCommand[5];
        lastCommand[5] = lastCommand[4];
        lastCommand[4] = lastCommand[3];
        lastCommand[3] = lastCommand[2];
        lastCommand[2] = lastCommand[1];
        lastCommand[1] = lastCommand[0];
        lastCommand[0] = entry;
    }

	public static void ConsoleEntryEnter() {
		string enteredText = PlayerMovement.a.consoleinpFd.text;
		if (String.IsNullOrEmpty(enteredText)) return;

		ConsoleEntry(enteredText);
	}

	static void EnterNoclip() {
		PlayerMovement.a.CheatNoclip = true;
		PlayerMovement.a.grounded = false;
		PlayerMovement.a.rbody.useGravity = false;
		Utils.DisableCapsuleCollider(PlayerMovement.a.capsuleCollider);
		Utils.DisableCapsuleCollider(PlayerMovement.a.leanCapsuleCollider);
		Utils.DisableSphereCollider(PlayerMovement.a.cyberCollider);
		Const.sprint("noclip " + Const.a.stringTable[1000]); // "ACTIVATED"
	}

	static void ExitNoclip() {
		PlayerMovement.a.CheatNoclip = false;
		PlayerMovement.a.grounded = false;
		if (PlayerMovement.a.inCyberSpace) {
			Utils.EnableSphereCollider(PlayerMovement.a.cyberCollider);
		} else {
			Utils.EnableCapsuleCollider(PlayerMovement.a.capsuleCollider);
			Utils.EnableCapsuleCollider(PlayerMovement.a.leanCapsuleCollider);
		}
		Const.sprint("noclip " + Const.a.stringTable[717]); // "DISABLED"
	}

    private static void ConsoleEntry(string entry) {
        ShiftLastCommand(entry);
		consoleMemdex = 0;
		string ts = entry.ToLower(); // test string = lower case text
		string tn = entry; // test number = number searching
        if (ts.Contains("noclip") || ts.Contains("idclip")
            || ts.Contains("no clip")) {
			if (PlayerMovement.a.CheatNoclip) {
				ExitNoclip();
			} else {
				EnterNoclip();
			}
        } else if (ts.Contains("editmode") || ts.Contains("edit mode")
			 || ts.Contains("editor")) {
			Const.a.editMode = !Const.a.editMode;
			if (Const.a.editMode) {
				Const.sprint(Const.a.stringTable[998]); // "Edit Mode activated! The current level can be shaped to your heart's content!"
				EnterNoclip();
				PlayerMovement.a.Notarget = true;
			}

			if (!Const.a.editMode) {
				Const.sprint(Const.a.stringTable[999]); // "Edit Mode deactivated, normal play"
				LevelEditor.a.EditorExit();
				ExitNoclip();
				PlayerMovement.a.Notarget = false;
			}
        } else if (ts.Contains("notarget") || ts.Contains("no target")) {
			if (PlayerMovement.a.Notarget) {
				PlayerMovement.a.Notarget = false;
				Const.sprint("notarget " + Const.a.stringTable[717]); // "DISABLED"
			} else {
				PlayerMovement.a.Notarget = true;
				Const.sprint("notarget " + Const.a.stringTable[1000]); // "ACTIVATED"
			}
        } else if (ts.Contains("god")
                   || (ts.Contains("power") && ts.Contains("overwhelming"))
                   || ts.Contains("whosyourdaddy")
                   || ts.Contains("iddqd")) {
			if (PlayerMovement.a.hm.god) {
				Const.sprint("god mode " + Const.a.stringTable[717]); // "DISABLED"
				PlayerMovement.a.hm.god = false;
			} else {
				Const.sprint("god mode " + Const.a.stringTable[1000]); // "ACTIVATED"
				PlayerMovement.a.hm.god = true;
			}
        } else if (ts.Contains("load") && (tn.Contains("0") || ts.Contains("loadr") || ts.Contains("load r")) && !ts.Contains("10") && !ts.Contains("arsenal")) CheatLoadLevel(0);
        else if (ts.Contains("load") && tn.Contains("1") && !ts.Contains("10") && !ts.Contains("11") && !ts.Contains("12")  && !ts.Contains("13") && !ts.Contains("g1") && !ts.Contains("arsenal"))  CheatLoadLevel(1);
		else if (ts.Contains("load") && tn.Contains("2") && !ts.Contains("12") && !ts.Contains("g2") && !ts.Contains("arsenal"))  CheatLoadLevel(2);
		else if (ts.Contains("load") && tn.Contains("3") && !ts.Contains("13") && !ts.Contains("g3") && !ts.Contains("arsenal"))  CheatLoadLevel(3);
		else if (ts.Contains("load") && tn.Contains("4") && !ts.Contains("g4") && !ts.Contains("arsenal"))  CheatLoadLevel(4);
		else if (ts.Contains("load") && tn.Contains("5") && !ts.Contains("arsenal"))  CheatLoadLevel(5);
		else if (ts.Contains("load") && tn.Contains("6") && !ts.Contains("arsenal"))  CheatLoadLevel(6);
		else if (ts.Contains("load") && tn.Contains("7") && !ts.Contains("arsenal"))  CheatLoadLevel(7);
		else if (ts.Contains("load") && tn.Contains("8") && !ts.Contains("arsenal"))  CheatLoadLevel(8);
		else if (ts.Contains("load") && tn.Contains("9") && !ts.Contains("arsenal"))  CheatLoadLevel(9);
		else if (ts.Contains("load") && ts.Contains("g1") && !ts.Contains("arsenal")) CheatLoadLevel(10);
        else if (ts.Contains("load") && ts.Contains("g2") && !ts.Contains("arsenal")) CheatLoadLevel(11);
        else if (ts.Contains("load") && ts.Contains("g4") && !ts.Contains("arsenal")) CheatLoadLevel(12);
		else if (ts.Contains("load") && ts.Contains("10") && !ts.Contains("arsenal")) CheatLoadLevel(10);
        else if (ts.Contains("load") && ts.Contains("11") && !ts.Contains("arsenal")) CheatLoadLevel(11);
        else if (ts.Contains("load") && ts.Contains("12") && !ts.Contains("arsenal")) CheatLoadLevel(12);
		else if (ts.Contains("load") && ts.Contains("g3") && !ts.Contains("arsenal")) {
			Const.sprint(Const.a.stringTable[1001]); // "Gamma grove already jettisoned!  Those poor arrogant people."
		} else if (ts.Contains("load") && ts.Contains("arsenal")) {
            if (ts.Contains("arsenalr") || ts.Contains("arsenal r") || ts.Contains("0"))
                                        PlayerMovement.a.EnableCheatArsenal(0);
            else if (ts.Contains("1"))  PlayerMovement.a.EnableCheatArsenal(1);
            else if (ts.Contains("2"))  PlayerMovement.a.EnableCheatArsenal(2);
            else if (ts.Contains("3"))  PlayerMovement.a.EnableCheatArsenal(3);
            else if (ts.Contains("4"))  PlayerMovement.a.EnableCheatArsenal(4);
            else if (ts.Contains("5"))  PlayerMovement.a.EnableCheatArsenal(5);
            else if (ts.Contains("6"))  PlayerMovement.a.EnableCheatArsenal(6);
            else if (ts.Contains("7"))  PlayerMovement.a.EnableCheatArsenal(7);
            else if (ts.Contains("8"))  PlayerMovement.a.EnableCheatArsenal(8);
            else if (ts.Contains("9"))  PlayerMovement.a.EnableCheatArsenal(9);
            else if (ts.Contains("g1")) PlayerMovement.a.EnableCheatArsenal(10);
            else if (ts.Contains("g2")) PlayerMovement.a.EnableCheatArsenal(11);
            else if (ts.Contains("g4")) PlayerMovement.a.EnableCheatArsenal(12);
            else if (ts.Contains("g3")) {
                Const.sprint(Const.a.stringTable[1001]); // "Gamma grove already jettisoned!  Those poor arrogant people."
            }
        } else if (ts.Contains("bottomless") && ts.Contains("clip")) { // bottomlessclip
			if (WeaponCurrent.a.bottomless) {
				Const.sprint(Const.a.stringTable[1003]); // "Hose disconnected from interdimensional wormhole. Normal ammo operation restored."
				WeaponCurrent.a.bottomless = false;
			} else {
				Const.sprint("bottomlessclip!  " + Const.a.stringTable[1002]); // "Bring it!"
				WeaponCurrent.a.bottomless = true;
			}
        }  else if (ts.Contains("nohud")) { // No HUD
			if (Const.a.noHUD) {
				// Normal
				Const.a.noHUD = false;
				Const.sprint("HUD " + Const.a.stringTable[1000]); // "ACTIVATED"
				if (MouseLookScript.a.inventoryMode) {
					MouseLookScript.a.shootModeButton.SetActive(true);
				}
				MFDManager.a.overallLeftMFD.SetActive(true);
				MFDManager.a.overallRightMFD.SetActive(true);
				MFDManager.a.overallCenterMFD.SetActive(true);
				MFDManager.a.overallHardwareButtons.SetActive(true);
				MFDManager.a.overallHealthTickPanel.SetActive(true);
				if (!PlayerMovement.a.inCyberSpace) {
					MFDManager.a.healthIndicator.SetActive(true);
				} else {
					MFDManager.a.cyberHealthIndicator.SetActive(true);
				}

				MFDManager.a.overallEnergyTickPanel.SetActive(true);
				MFDManager.a.overallEnergyIndicator.SetActive(true);
				MFDManager.a.overallEnergyDrainText.SetActive(true);
				MFDManager.a.overallEnergyJPMText.SetActive(true);
				MFDManager.a.overallTextWarnings.SetActive(true);
				MFDManager.a.overallMissionTimerT.SetActive(true);
				MFDManager.a.overallMissionTimer.SetActive(true);
				if (PlayerMovement.a.inCyberSpace) {
					MFDManager.a.cyberTimerT.SetActive(true);
					MFDManager.a.cyberTimer.SetActive(true);
				}
				MFDManager.a.TabReset(true);
				MFDManager.a.TabReset(false);
				MFDManager.a.ReturnToLastTab(true);
				MFDManager.a.ReturnToLastTab(false);
				if (Inventory.a.hasHardware[1]) {
					MouseLookScript.a.compassContainer.SetActive(true);
				}
			} else {
				// HUDless Screenshot mode!
				Const.a.noHUD = true;
				Const.sprint(Const.a.stringTable[1004]); // "No HUD! Enjoy the cinematic screenshot experience!"
				MouseLookScript.a.shootModeButton.SetActive(false);
				MFDManager.a.overallLeftMFD.SetActive(false);
				MFDManager.a.overallRightMFD.SetActive(false);
				MFDManager.a.overallCenterMFD.SetActive(false);
				MFDManager.a.overallHardwareButtons.SetActive(false);
				MFDManager.a.overallHealthTickPanel.SetActive(false);
				MFDManager.a.healthIndicator.SetActive(false);
				MFDManager.a.cyberHealthIndicator.SetActive(false);
				MFDManager.a.overallEnergyTickPanel.SetActive(false);
				MFDManager.a.overallEnergyIndicator.SetActive(false);
				MFDManager.a.overallEnergyDrainText.SetActive(false);
				MFDManager.a.overallEnergyJPMText.SetActive(false);
				MFDManager.a.overallTextWarnings.SetActive(false);
				MFDManager.a.overallMissionTimerT.SetActive(false);
				MFDManager.a.overallMissionTimer.SetActive(false);
				MFDManager.a.cyberTimerT.SetActive(false);
				MFDManager.a.cyberTimer.SetActive(false);
				MouseLookScript.a.compassContainer.SetActive(false);
			}	
        } else if (ts.Contains("ifeelthepower")
                   || (ts.Contains("i") && ts.Contains("feel")
                       && ts.Contains("the") && ts.Contains("power"))) {
			if (WeaponCurrent.a.redbull) {
				Const.sprint(Const.a.stringTable[1005]); // Energy usage normal
				WeaponCurrent.a.redbull = false;
			} else {
				Const.sprint(Const.a.stringTable[1006]); // "I feel the power! 0 energy consumption!"
				WeaponCurrent.a.redbull = true; // Might not be wings, but hey.
			}
        } else if (ts.Contains("show") && ts.Contains("fps")) { // showfps
			Const.sprint(Const.a.stringTable[1007]); // "Toggling FPS counter for framerate (bottom right corner)..."
			PlayerMovement.a.fpsCounter.SetActive(!PlayerMovement.a.fpsCounter.activeInHierarchy);
			Inventory.a.hardwareButtonManager.bioMonitorContainer.SetActive(true);
        } else if (ts.Contains("show") && ts.Contains("location")) { // showlocation
			Const.sprint(Const.a.stringTable[1008]); // "Toggling locationIndicator (bottom left corner)..."
			PlayerMovement.a.locationIndicator.SetActive(!PlayerMovement.a.locationIndicator.activeInHierarchy);
		} else if (ts.Contains("i") && ts.Contains("am") && ts.Contains("shodan")) { // iamshodan
			if (LevelManager.a.superoverride) {
				Const.sprint(Const.a.stringTable[1009]); // "SHODAN has regained control of security from you"
				LevelManager.a.superoverride = false;
			} else {
				Const.sprint(Const.a.stringTable[1010]); // "Full security override enabled!"
				LevelManager.a.superoverride = true;
			}
		} else if (entry == "dizzy") {
			if (LevelManager.a.skyRotate.rotateSpeed < 0.9f) LevelManager.a.skyRotate.rotateSpeed = 1f;
			else if (LevelManager.a.skyRotate.rotateSpeed < 1.9f) LevelManager.a.skyRotate.rotateSpeed = 2f;
			else if (LevelManager.a.skyRotate.rotateSpeed < 4.9f) LevelManager.a.skyRotate.rotateSpeed = 5f;
			else if (LevelManager.a.skyRotate.rotateSpeed < 9.9f) LevelManager.a.skyRotate.rotateSpeed = 10f;
			else LevelManager.a.skyRotate.rotateSpeed = LevelManager.a.skyRotate.defaultSpeed;
		} else if (entry == "Mr. Bean") {
			Const.sprint("Nice try, there are no go carts to slow down here");
		} else if (entry == "Simon Foster") {
			Const.sprint("Nice try, nothing to paint here");
		} else if (entry == "Motherlode" || entry == "Rosebud" || entry == "Kaching" || entry == "money") {
			Const.sprint("Nice try, there's no money here.");
		} else if (entry == "Richard Branson") {
			Const.sprint("Nice try, there's no money here.  You do realize this isn't Rollercoaster Tycoon right?");
		} else if (entry == "John Wardley") {
			Const.sprint("WOW!");
		} else if (entry == "John Mace") {
			Const.sprint("Nice try, there's nothing to pay double for here");
		} else if (entry == "Melanie Warn") {
				Const.sprint("I feel happy!!!");
		} else if (entry == "Damon Hill") {
				Const.sprint("Nice try, there are no go carts to speed up here");
		} else if (entry == "Michael Schumacher") {
				Const.sprint("Nice try, there are no go carts to give ludicrous speed here");
		} else if (entry == "Tony Day") {
				Const.sprint("Ok, now I want a hamburger");
		} else if (entry == "Katie Brayshaw") {
				Const.sprint("Hi there! Hello! Hey! Howdy!");
		} else if (ts.Contains("sudo") || ts.Contains("admin")) {
				Const.sprint("Super user access granted...ERROR: access restricted by SHODAN");
		} else if (ts.Contains("git")) {
				if (ts.Contains("pull") || ts.Contains("fetch")) Const.sprint("remote: Enumerating objects: 24601, done. Failed, could not connect with origin/triop.");
				else if (ts.Contains("status")) Const.sprint("Your branch is up to date with origin/triop. Working directory clean.");
				else if (ts.Contains("log")) Const.sprint("<Merge pull request #451 from SHODAN/NeuralLinkBugfix> 6 months ago...");
				else if (ts.Contains("reflog")) Const.sprint("dc51440 HEAD0 -> master: commit: Establish neural connection ... ERROR: invalid ID `2-4601`");
				else if (ts.Contains("merge")) Const.sprint("Failed, could not connect with origin/triop");
				else if (ts.Contains("push")) Const.sprint("Could not find Username for 'triopttp://192.168.1.451'");
				else if (ts.Contains("clone")) Const.sprint("Failed, connection blocked by SHODAN. Employee ID invalid.");
				else if (ts.Contains("branch") || ts.Contains("-b")) Const.sprint("Created new branch " + ts.Split(' ').Last());
				else if (ts.Contains("checkout")) Const.sprint("Branch name not recognized.  Contact your TriopBucket representative.");
				else Const.sprint("Branch name not recognized.  Contact your TriopBucket representative.");
		} else if (ts.Contains("restart")) {
				Const.sprint("Yeah...better not");
		} else if (ts.Contains("quit") || ts.Contains("exit")) {
				Const.sprint("Use the Pause Menu by hitting Escape and using the QUIT option via mouse or arrow keys + ENTER");
		} else if (ts.Contains("cd") || ts.Contains("./")) {
				Const.sprint("Attempting to access directory... already at root");
		} else if (ts.Contains("kill") || ts.Contains("kick") || ts.Contains("ban") || ts.Contains("destroy") || ts.Contains("attack") || ts.Contains("suicide") || ts.Contains("die")) {
				Const.sprint(Const.a.stringTable[1011]); // "Player decides to become a cyborg."
				DamageData dd = new DamageData();
				dd.damage = PlayerMovement.a.hm.health + 1.0f;
				dd.other = PlayerMovement.a.gameObject; // Player capsule
				PlayerMovement.a.hm.TakeDamage(dd);
		} else if (ts.Contains("justinbailey")) {
				Const.sprint("Well, you don't have a suit already so...");
		} else if (ts.Contains("woodstock")) {
				Const.sprint("How much wood could a woodchuck chuck...there's no wood in SPACE!");
		} else if (ts.Contains("quarry")) {
				Const.sprint("There's obsidian on levels 6 and 8 if you want to feel decadant, otherwise we are lacking in the stone department.");
		} else if (ts.Contains("help")) {
				Const.sprint("There's no one to save you now Hacker!");
		} else if (ts.Contains("zelda")) {
				Const.sprint("Too late, already been to level 1");
		} else if (ts.Contains("allyourbasearebelongtous") || (ts.Contains("all") && ts.Contains("your") && ts.Contains("base"))) {
				Const.sprint("ERROR: SHODAN has overriden your command, remove SHODAN first."); // This is not an easter egg if you run this after removing SHODAN!!
		} else if (ts.Contains("i") && ts.Contains("am") && ((ts.Contains("iron") && ts.Contains("man")) || ts.Contains("amazing") || ts.Contains("cool") || ts.Contains("best"))) {
				Const.sprint("That's nice dear.");
		} else if ((ts.Contains("impulse") && tn.Contains("9")) || ts.Contains("idkfa")) {
				Const.sprint("I can only hold 7 weapons!! Nice try dearies!");
		} else if (ts.Contains("summon_obj")) {
			int val = Utils.GetIntFromStringAudLogText(ts.Split(' ').Last()); // That's a slow line to compute!
			if (val < 438 && val >= 0) {
				SpawnDynamicObject(val,LevelManager.a.currentLevel,true,-1);
			}
        } else if (ts.Contains("undo")) {
			if (lastSpawnedGO != null && Const.a.editMode) Utils.SafeDestroy(lastSpawnedGO);
			if (!Const.a.editMode) Const.sprint("Cannot undo when not in Edit Mode");
        } else if (ts.Contains("settargetfps") || ts.Contains("setfps")) {
			int val = Utils.GetIntFromStringAudLogText(ts.Split(' ').Last()); // That's a slow line to compute!
			if (val <= 200 && val > 10) {
				Const.a.TARGET_FPS = val;
				Config.SetVSync();
			}
			
			Const.sprint("FPS] -> " + val.ToString());
        } else if (ts.Contains("shake")) {
			Const.a.Shake(true,-1,-1);
        } else if (ts.Contains("tired") || ts.Contains("staminup")) {
            if (PlayerMovement.a.FatigueCheat) {
                Const.sprint(Const.a.stringTable[1012]); // "Fatigue returned to normal"
                PlayerMovement.a.FatigueCheat = false;
            } else {
                Const.sprint("Stamin-Up! " + Const.a.stringTable[1013]); // "Fatigue no longer affects you!"
                PlayerMovement.a.FatigueCheat = true;
            }
        } else {
            Const.sprint(Const.a.stringTable[1014] + entry); // "Uknown command or function: "
        }

        PlayerMovement.a.consoleinpFd.text = ""; // Reset console and hide it, command was entered.
        PlayerMovement.a.ToggleConsole();
    }

    public static void CheatLoadLevel(int lev) {
		if (PauseScript.a.MenuActive()) {
			Const.sprint(Const.a.stringTable[1015]); // "Cannot load levels via cheat while on the menu!"
			return;
		}

        LevelManager.a.CheatLoadLevel(lev);
    }

/*
Prefab Indices Master List
Pardon the staggered tables here.  This is an attempt to show as much as I can.
===============================================================================
Master Index
    Prefab Name                  Secondary Array Indices
0   chunk_black
1   chunk_blocker                Texture Array Index(es)
2   chunk_bridg1_1               0
3   chunk_bridg1_1flipx          0
4   chunk_bridg1_2               1
5   chunk_bridg1_3               2
6   chunk_bridg1_3_slice45       2
7   chunk_bridg1_3flipx          2
8   chunk_bridg1_4               3
9   chunk_bridg1_4_slice32       3
10  chunk_bridg1_4_slice32flipx  3
11  chunk_bridg1_5               4
12  chunk_bridg2_2               6
13  chunk_bridg2_3               7
14  chunk_bridg2_4               8
15  chunk_bridg2_5               9
16  chunk_bridg2_6               10
17  chunk_bridg2_7               11
18  chunk_bridg2_8               12
19  chunk_bridg2_9               13
20  chunk_crate_impenetrable
21  chunk_cyberpanel
22  chunk_cyberpanel_slice45
23  chunk_eng1_1                 14,16,21
24  chunk_eng1_1d                14,15,16,21
25  chunk_eng1_2                 16
26  chunk_eng1_2d                16,17
27  chunk_eng1_3                 18
28  chunk_eng1_3d                19
29  chunk_eng1_4                 20,21
30  chunk_eng1_5                 21
31  chunk_eng1_5_slice45lh       21
32  chunk_eng1_5_slice45rh       21
33  chunk_eng1_5d                22
34  chunk_eng1_6                 23
35  chunk_eng1_6d                24
36  chunk_eng1_7                 21,25
37  chunk_eng1_7d                26
38  chunk_eng1_8                 21,27
39  chunk_eng1_9                 28
40  chunk_eng1_9d                29
41  chunk_eng2_1                 30
42  chunk_eng2_1_slice45         30
43  chunk_eng2_1_slice384high    30
44  chunk_eng2_1_slice384highrh  30
45  chunk_eng2_1d                31
46  chunk_eng2_2                 32
47  chunk_eng2_2d                33
48  chunk_eng2_3                 34
49  chunk_eng2_3d                35
50  chunk_eng2_4                 36
51  chunk_eng2_5                 37
52  chunk_eng2_5_slice45         37
53  chunk_eng2_6
54  chunk_exec1_1                38
55  chunk_exec1_1d               39
56  chunk_exec1_2                40
57  chunk_exec1_2d               41
58  chunk_exec2_1                42
59  chunk_exec2_2                43
60  chunk_exec2_2d               44
61  chunk_exec2_3                45
62  chunk_exec2_4                46
63  chunk_exec2_4_slice45        46
64  chunk_exec2_5                47
65  chunk_exec2_6                48
66  chunk_exec2_7                49
67  chunk_exec3_1                50
68  chunk_exec3_1d               51
69  chunk_exec3_2                52
70  chunk_exec3_4                53
71  chunk_exec4_1                54
72  chunk_exec4_2                55
73  chunk_exec4_3                56
74  chunk_exec4_4                57
75  chunk_exec4_5                58
76  chunk_exec4_6                59
77  chunk_exec6_1                60
78  chunk_exteriorpanel1
79  chunk_fan1
80  chunk_flight1_1              61
81  chunk_flight1_1b             62
82  chunk_flight1_2              63
83  chunk_flight1_2_slice45rh    63
84  chunk_flight1_3              64
85  chunk_flight1_4              65
86  chunk_flight1_5              66
87  chunk_flight1_5_slice45lh    66
88  chunk_flight1_6              67
89  chunk_flight2_1              68
90  chunk_flight2_2              69
91  chunk_flight2_2_slice45      69
92  chunk_flight2_3              70
93  chunk_grove1_1
94  chunk_grove1_2               71
95  chunk_grove1_2_slice45       71
96  chunk_grove1_3               72
97  chunk_grove1_4               73
98  chunk_grove1_5               74
99  chunk_grove1_6               75
100 chunk_grove1_7               76
101 chunk_grove2_1               77
102 chunk_grove2_2               78
103 chunk_grove2_3               79
104 chunk_grove2_4               80
105 chunk_grove2_5               81
106 chunk_grove2_6               82
107 chunk_grove2_7               83
108 chunk_grove2_8               84
109 chunk_grove2_9               85
110 chunk_grove2_9b              86
111 chunk_grove2_9c              87
112 chunk_lift1
113 chunk_maint1_1               88
114 chunk_maint1_2               89
115 chunk_maint1_2d              90
116 chunk_maint1_3               91
117 chunk_maint1_3b              92
118 chunk_maint1_4               93
119 chunk_maint1_4b              94
120 chunk_maint1_5               95
121 chunk_maint1_6               96
122 chunk_maint1_7               97
123 chunk_blockerflightbay
124 chunk_maint1_9               98
125 chunk_maint1_9d              99
126 chunk_maint2_1               100
127 chunk_maint2_1b              101
128 chunk_maint2_1d              102
129 chunk_maint2_2               103
130 chunk_maint2_3               104
131 chunk_maint2_3d              105
132 chunk_maint2_4               106
133 chunk_maint2_4d              107
134 chunk_maint2_5               108
135 chunk_maint2_5d              109
136 chunk_maint2_6               110
137 chunk_maint2_6d              111
138 chunk_maint2_7               112
139 chunk_maint2_7d              113
140 chunk_maint2_8               114
141 chunk_maint2_9               115
142 chunk_maint2_9_slice45RH     115
143 chunk_maint2_9_slice128_top  115
144 chunk_maint3_1               116
145 chunk_maint3_1_slice32_lh    116
146 chunk_maint3_1_slice32_rh    116
147 chunk_maint3_1_slice45       116
148 chunk_maint3_1d              117
149 chunk_med1_1                 118
150 chunk_med1_1_half_top        118
151 chunk_med1_1_slice128high    118
152 chunk_med1_1_slice192RH      118
153 chunk_med1_1_slice256        118
154 chunk_med1_1d                119
155 chunk_med1_2                 120
156 chunk_med1_2d                121
157 chunk_med1_3                 122
158 chunk_med1_3d                123
159 chunk_med1_4                 124
160 chunk_med1_5                 125
161 chunk_med1_6                 126
162 chunk_med1_7                 127
163 chunk_med1_7_slice14_64      127
164 chunk_med1_7_slice45_320lh   127
165 chunk_med1_7_slice45_320rh   127
166 chunk_med1_7_slice96high     127
167 chunk_med1_7d                128
168 chunk_med1_7d_slice128       128
169 chunk_med1_8                 129
170 chunk_med1_8d                130
171 chunk_med1_9                 131
172 Replaced with chunk_black, unused variant chunk_med1_9_ofs32_90 removed
173 Replaced with chunk_black, unused variant chunk_med1_9_ofs64_90 removed
174 chunk_med1_9d                132
175 Replaced with chunk_black, unused variant chunk_med1_9d_ofs48_90 removed
176 chunk_med1_9d_ofs112_90      132
177 chunk_med1_9d_ofs144_90      132
178 chunk_med2_1                 133
179 chunk_med2_1_slice32RH       133
180 chunk_med2_1d                134
181 chunk_med2_2                 135
182 chunk_med2_2_half_bottom     135
183 chunk_med2_2d                136
184 chunk_med2_3                 137
185 chunk_med2_3d                138
186 chunk_med2_4                 139
187 chunk_med2_5                 140
188 chunk_med2_6                 141
189 chunk_med2_7                 142
190 chunk_med2_8                 143
191 chunk_med2_8_half_top        143
192 chunk_med2_8_slice32RH       143
193 chunk_med2_8_slice45         143
194 chunk_med2_9                 144
195 chunk_med2_9d                145
196 chunk_med3_1                 146
197 chunk_rad1_1                 147
198 chunk_rad1_2                 148
199 chunk_reac1_1                149
200 chunk_reac1_1_slice45        149
201 chunk_reac1_2                150
202 chunk_reac1_3                151
203 chunk_reac1_4                152
204 chunk_reac1_5                153
205 chunk_reac1_6                154
206 chunk_reac1_7                155
207 chunk_reac1_8                156
208 chunk_reac1_9                157
209 chunk_reac2_1                158
210 chunk_reac2_1_slice45LH      158
211 chunk_reac2_1_slice45LH_up   158
212 chunk_reac2_1_slice45RH      158
213 chunk_reac2_1_slice45RH_up   158
214 chunk_reac2_1b               159
215 chunk_reac2_1bmirror         159
216 chunk_reac2_1mirror          158
217 chunk_reac2_2                160
218 chunk_reac2_4                161
219 chunk_reac2_4_slice128lower  161
220 chunk_reac2_5                162
221 chunk_reac2_6                163
222 chunk_reac2_7                164
223 chunk_reac2_8                165
224 chunk_reac2_9                166
225 chunk_reac3_1                167
226 chunk_reac3_2                168
227 chunk_reac3_3                169
228 chunk_reac3_4                170
229 chunk_reac3_5                171
230 chunk_reac3_6                172
231 chunk_reac3_7                173
232 chunk_reac4_1                174
233 chunk_reac4_1_slice45lh      174
234 chunk_reac4_2                175
235 chunk_reac5_1                176
236 chunk_reac5_2                177
237 chunk_reac5_3                178
238 chunk_reac6_1                179
239 chunk_reac6_2                180
240 chunk_reac6_3                181
241 chunk_sci1_1                 182
242 chunk_sci1_1_slice45_toplh   182
243 chunk_sci1_1_slice45_toprh   182
244 chunk_sci1_1d                183
245 chunk_sci1_2                 184
246 chunk_sci1_2_slice45lh       184
247 chunk_sci1_2_slice45lh_up    184
248 chunk_sci1_2_slice45rh       184
249 chunk_sci1_2_slice45rh_up    184
250 chunk_sci1_2d                185
251 chunk_sci1_3                 186
252 chunk_sci1_4                 187
253 chunk_sci1_5                 188
254 chunk_sci1_6                 189
255 chunk_sci1_6_slice45         189
256 chunk_sci1_7                 190
257 chunk_sci1_7d                191
258 chunk_sci1_8                 192
259 chunk_sci1_8d                193
260 chunk_sci1_9                 194
261 chunk_sci1_9d                195
262 chunk_sci2_1                 196
263 chunk_sci2_1_slice45lh       196
264 chunk_sci2_1_slice45rh       196
265 chunk_sci2_1d                197
266 chunk_sci2_2                 198
267 chunk_sci2_2d                199
268 chunk_sci2_3                 200
269 chunk_sci2_4                 201
270 chunk_sci2_5                 202
271 chunk_sci2_5d                203
272 chunk_sci3_1                 204
273 chunk_sci3_1d                205
274 chunk_sci3_2                 206
275 chunk_sci3_3                 207
276 chunk_sci3_4                 208
277 chunk_sci3_5                 209
278 chunk_sci3_6                 210
279 chunk_screen
280 chunk_sec1_1                 211
281 chunk_sec1_1b                212
282 chunk_sec1_1c                213
283 chunk_sec1_1c_slice45        213
284 chunk_sec1_1c_slice64highlh  213
285 chunk_sec1_1c_slice64highrh  213
286 Replaced with chunk_black, unused chunk_sec1_1c_slice320high removed
287 Replaced with chunk_black, unused chunk_sec1_1c_slice320highrh removed
288 chunk_sec1_2                 214
289 chunk_sec1_2b                215
290 chunk_sec1_3                 216
291 chunk_sec1_3_slice45         216
292 chunk_stor1_1                217
293 chunk_stor1_2                218
294 chunk_stor1_3                219
295 chunk_stor1_4                220
296 chunk_stor1_5                221
297 chunk_stor1_6                222
298 chunk_stor1_6_slice128_up_lh 222
299 chunk_stor1_6_slice128_up_rh 222
300 chunk_stor1_6_slice192lh     222
301 chunk_stor1_6_slice192rh     222
302 chunk_stor1_7                223
303 chunk_stor1_7_slice45        223
304 chunk_stor1_7d               224
305 chunk_teleporter
306 chunk_white                 Const.a.useableItemsFrobIcons
307 item_paper_wad              0
308 item_warecasing             1
309 item_beaker                 2
310 item_beverage               3
311 item_skull                  4
312 item_arm                    5
313 item_audiolog               6  Grenade Type (sorry about the order)
314 weapon_grenadefrag          7  0
315 weapon_grenadeconc          8  3
316 weapon_grenadeemp           9  1
317 weapon_grenadeearth         10 6
318 weapon_grenademine          11 4
319 weapon_grenadenitro         12 5
320 weapon_grenadegas           13 2   Patch Type (whoops on the order again)
321 item_patch_berserk          14     2
322 item_patch_detox            15     6
323 item_patch_genius           16     5
324 item_patch_medi             17     3
325 item_patch_reflex           18     4
326 item_patch_sight            19     1
327 item_patch_staminup         20     0   Hardware Type
328 item_hw_system              21         0
329 item_hw_navunit             22         1
330 item_hw_ereader             23         2
331 item_hw_sensaround          24         3
332 item_hw_targetid            25         4
333 item_hw_shield              26         5
334 item_hw_bio                 27         6
335 item_hw_lantern             28         7
336 item_hw_envirosuit          29         8
337 item_hw_booster             30         9
338 item_hw_jumpjets            31         10
339 item_hw_infrared            32         11
340 item_fireextinguisher       33     Access Card Type
341 item_access_card_admin      34     Admin (Green Rim, Turquoise Inner with Yellow Cross) (card_group5)
342 item_workerhelmet           35  Weapon Type (aka Wep16Index)
343 weapon_mk3                  36  0
344 weapon_blaster              37  1
345 weapon_dartgun              38  2
346 weapon_flechette            39  3
347 weapon_ionrifle             40  4
348 weapon_rapier               41  5
349 weapon_pipe                 42  6
350 weapon_magnum               43  7
351 weapon_magpulse             44  8
352 weapon_pistol               45  9
353 weapon_plasma               46  10
354 weapon_railgun              47  11
355 weapon_riotgun              48  12
356 weapon_skorpion             49  13
357 weapon_sparqbeam            50  14
358 weapon_stungun              51  15
359 item_battery                52
360 item_battery_icad           53
361 item_logic_probe            54
362 item_healthkit              55
363 item_plastique              56
364 item_chipset_interfacedemod 57
365 item_flask                  58
366 item_chipset_bitflag        59
367 item_ammo_rubber            60
368 item_isotopex22             61
369 item_testtube               62  Grenade Type (same as non-live)
370 weapon_grenadefrag_live     63  0
371 item_chipset_isolinear      64  Grenade Type (same as non-live)
372 weapon_grenadeconc_live     65  3
373 item_ammo_needle            66
374 item_ammo_tranq             67
375 item_ammo_standard          68
376 item_ammo_teflon            69
377 item_ammo_hollow            70
378 item_ammo_slug              71
379 item_ammo_magnesium         72
380 item_ammo_penetrator        73
381 item_ammo_hornet            74
382 item_ammo_splinter          75
383 item_ammo_rail              76
384 item_ammo_slag              77
385 item_ammo_slaglarge         78
386 item_ammo_magcart           79  Grenade Type (same as non-live)
387 weapon_grenadeemp_live      80  1  Access Card Type
388 item_access_card_std        81     Standard (Orange Rim, Turquoise Inner) (card_std)
389 weapon_grenadeearth_live    82  6
390 item_access_card_group1     83     Group 1 (Blue Rim, Orange Inner) (card_group1_actual)
391 item_access_card_science    84     Science (All Yellow) (card_group1)
392 item_access_card_eng        85     Engineering (Blue Rim, Turquoise Inner) (card_eng)
393 item_access_card_groupB     86     Group B (Blue Rim, Orange Inner) (card_group1_actual)
394 item_access_card_security   87     Security (Red Rim, Yellow Inner) (card_per1)
395 item_access_card_per5diego  88     Personnel 5 (Edward Diego) (Purple Rim, Red Inner) (card_per5)
396 item_access_card_medi       89     Medical (Gray with Red Cross) (card_medi)
397 item_access_card_group3     90     Group3 (Blue Rim, Orange Inner) (card_blue)
398 item_access_card_purple     91     Group4
399 item_head_male              92
400 item_head_female            93
401 item_severedhead            94  Grenade Type (same as non-live)
402 weapon_grenademine_live     95  4
403 weapon_grenadenitro_live    96  5
404 weapon_grenadegas_live      97  2
405 line_sparqbeam              98
406 line_blaster                99
407 line_ion                    100
408 line_hopperbeam             101
409 red crosshair               102
410 orange crosshair            103
411 yellow crosshair            104
412 green crosshair small       105
413 teal crosshair              106
414 blue crosshair              107
415 cursor vmail                108
416 cursor cyberspace           109    Access Card Type
417 item_access_card_perdarcy   110    Personnel 1 (Darcy) (Purple Rim, Red Inner) (card_per5)
418 null  (whoops!, oh well)          NPC
419 npc_autobomb                      0
420 npc_cyborg_assassin               1
421 npc_avian_mutant                  2
422 npc_exec_bot                      3
423 npc_cyborg_drone                  4
424 npc_cortex_reaver                 5
425 npc_cyborg_warrior                6
426 npc_cyborg_enforcer               7
427 npc_cyborg_elite                  8
428 npc_cyborg_diego                  9
429 npc_sec1_bot                      10
430 npc_sec2_bot                      11
431 npc_maint_bot                     12
432 npc_mutant_cyborg                 13
433 npc_hopper                        14
434 npc_humanoid_mutant               15
435 npc_invisomut                     16
436 npc_virus_mutant                  17
437 npc_servbot                       18
438 npc_flier_bot                     19
439 npc_zerog_mutant                  20
440 npc_gorilla_tiger_mutant          21
441 npc_repairbot                     22
442 npc_plant_mutant                  23
443 npc_cyberdog                      24
444 npc_cyberguard                    25
445 npc_cyberram                      26
446 npc_cyber_reaver                  27
447 npc_cybershodan                   28  Cyber Items
448 item_cyber_data                       0
449 item_cyber_decoy                      1
450 item_cyber_drill                      2
451 item_cyber_game                       3
452 item_cyber_integrity                  4
453 item_cyber_keycard                    5
454 item_cyber_pulser                     6
455 item_cyber_recall                     7
456 item_cyber_shield                     8
457 item_cyber_turbo                      9 
458 prop_phys_barrel_chemical     
459 prop_phys_barrel_radiation    
460 prop_phys_barrel_toxic        
461 prop_phys_cart                
462 prop_phys_pot                 
463 prop_phys_toolcart             
464 se_briefcase                  
465 se_corpse_blueshirt           
466 se_corpse_brownshirt          
467 se_corpse_eaten               
468 se_corpse_labcoat             
469 se_corpse_security            
470 se_corpse_tan                 
471 se_corpse_torso               
472 se_crate1                     
473 se_crate2                     
474 se_crate3                     
475 se_crate4                     
476 se_crate5                     
477 sec_camera                    
478 sec_cpunode                   
479 sec_cpunode_small             
480 weapon_cyber_mine      Proj
481 proj_enemshot2         0
482 proj_magpulse_shot     1
483 proj_stungun_shot      2
484 proj_rail_shot         3
485 proj_plasmarifle_shot  4
486 proj_enemshot6         5
487 proj_enemshot5         6
488 proj_enemshot4         7
489 proj_throwingstar      8
490 proj_magpulsenpc_shot  9
491 proj_railnpc_shot     10
492 proj_cyberplayer_shot 11
493 proj_cyberdog_shot    12
494 proj_cyberreaver_shot 13
495 proj_cyberice_shot    14  Doors
496 doorA                     0
497 doorB                     1
498 doorC                     2
499 doorD                     3
500 doorE                     4
501 doorF                     5
502 doorG                     6
503 doorH                     7
504 doorI                     8
505 doorJ                     9
506 doorK                     10
507 doorL                     11
508 door_elevator1            12
509 door_elevator2            13
510 door_elevator3            14
511 door_elevator4            15
512 door_secret1              16
513 door_secret2              17
514 door_secret3              18
515 func_forcebridge
516 prop_lift2
517 func_wall                     
518 BulletHoleLarge
519 BulletHoleScorchLarge         
520 BulletHoleScorchSmall         
521 BulletHoleSmall               
522 BulletHoleTiny                
523 BulletHoleTinySpread          
524 func_door_cyber           19
525 prop_console01                
526 prop_console02                
527 prop_grate1_1                 
528 prop_grate1_2                 
529 prop_grate1_3                 
530 se_cabinet                    
531 se_thermos                    
532 prop_beaker_holder            
533 prop_bed                      
534 prop_bed_hospital             
535 prop_bed_neurosurgery         
536 prop_bonepile1                
537 prop_bridgewall1              
538 prop_broken_clock             
539 prop_brokengun                
540 prop_chair01                  
541 prop_chair02                  
542 prop_chair03                  
543 prop_chair04                  
544 prop_chair05                  
545 prop_chandelier               
546 prop_charge_station           
547 prop_clothes                  
548 prop_computer                 
549 prop_couch                    
550 prop_couch2                   
551 prop_cpuscreen                
552 prop_cyber_datafrag           
553 prop_cyber_decoy              
554 prop_cyber_exit               
555 prop_cyber_switch             
556 prop_cyberport                
557 prop_desk01                   
558 prop_desk02                   
559 prop_dexmissile               
560 prop_foliage_fernpoison              
561 prop_foliage_bush             
562 prop_foliage_fern             
563 prop_foliage_fernblueflower   
564 prop_foliage_pinetreem        
565 prop_foliage_poisonbush1      
566 prop_gear_large               
567 prop_gear_small               
568 prop_grass1                   
569 prop_grass2                   
570 prop_grass3                   
571 prop_grass4                   
572 prop_grass5                   
573 prop_grate4                   
574 prop_healingbed               
575 prop_lamp                     
576 prop_light_emergsignal        
577 prop_microscope               
578 prop_pipe                     
579 prop_puddle                   
580 prop_puddle_grease            
581 prop_puddle_oil               
582 prop_shelves                  
583 prop_skeleton                 
584 prop_sleeping_cables          
585 prop_sparkingwire             
586 prop_table                    
587 prop_tv_on_a_post             
588 prop_vendingmachines1         
589 prop_vendingmachines2         
590 prop_weapon_rack              
591 prop_xray                     
592 text_decal                    
593 text_decalStopDSS1           
594 trigger_counter              
595 trigger_cyberpush            
596 trigger_gravitylift          
597 trigger_ladder               
598 trigger_multiple             
599 trigger_music                
600 trigger_once                 
601 trigger_radiation            
602 us_isotopepanel              
603 us_paperlog                  
604 us_puz_elevatorkeypad        
605 us_puz_elevatorkeypad2       
606 us_puz_elevatorkeypad3       
607 us_puz_elevatorkeypad4       
608 us_puz_keypad                
609 us_puz_panel_blue_grid
610 us_puz_panel_brown_grid
611 us_puz_panel_gray_grid
612 us_puz_panel_red_grid
613 us_puz_panel_teal_grid
614 us_relaypanel 
615 us_retinalscanner
616 prop_vending1_1
617 prop_vending1_2
618 prop_vending1_3
619 prop_vending2_1
620 prop_vending2_2
621 ambient_airhiss              
622 ambient_clicker              
623 ambient_compressor           
624 ambient_dishwasher           
625 ambient_drip_amb             
626 ambient_fan                  
627 ambient_generator_gas        
628 ambient_gurgle               
629 ambient_icemaker             
630 ambient_intake               
631 ambient_lathe                
632 ambient_lev3loop1            
633 ambient_lev3loop2            
634 ambient_lev3loop3            
635 ambient_lev3loop4            
636 ambient_liquid_bubble        
637 ambient_liquid_lava2         
638 ambient_looping              
639 ambient_machgear_loop        
640 ambient_machine_ambience     
641 ambient_machine_go           
642 ambient_machine_humamb7      
643 ambient_machine_humlonoise   
644 ambient_machine_loop1        
645 ambient_machine_loop2        
646 ambient_machinea1            
647 ambient_machinevat_loop      
648 ambient_mist                 
649 ambient_pipewater_loop       
650 ambient_powerloom            
651 ambient_pump                 
652 ambient_pump2                
653 ambient_rain                 
654 ambient_steam_loop           
655 ambient_washing_machine      
656 decal_blood_die              
657 decal_blood_resist           
658 decal_blood_stayaway         
659 decal_blood_words2           
660 decal_bloodfonta             
661 decal_bloodfonte             
662 decal_bloodfontg             
663 decal_bloodfonth             
664 decal_bloodfontr             
665 decal_bloodfonty             
666 decal_bloodsplat2            
667 decal_logo_antenna           
668 decal_logo_armory            
669 decal_logo_biohazard         
670 decal_logo_bridge            
671 decal_logo_cyborg            
672 decal_logo_gears             
673 decal_logo_medical           
674 decal_logo_radhazard         
675 decal_logo_research          
676 decal_logo_security          
677 decal_painting1              
678 decal_painting2              
679 decal_painting3              
680 decal_posterbetterfuture     
681 decal_postergenetics         
682 decal_scorch1                
683 decal_scorch2                
684 decal_scorch3                
685 decal_scorch4                
686 decal_scorchtiny             
687 decal_blood_splat // missed typing it so alphabetically down here, oh well
688 func_switch1                 
689 func_switch2                 
690 func_switch3                 
691 func_switch4                 
692 func_switch5                 
693 func_switch5broken           
694 func_switch7                 
695 func_switch8                 
696 func_switchbroken1           
697 clip_npc
698 clip_objects
699 logic_relay
700 logic_branch
701 logic_timer
702 logic_spawner
703 info_teleport_destination
704 prop_debris_panel
705 info_cyborgconversion
706 info_elev_destination
707 info_email
708 info_gameend
709 info_message
710 info_mission
711 info_note
712 info_playsound
713 info_ressurection_point
714 info_screenshake
715 info_spawnpoint
716 fx_reverbzone
717 ef_cyber_ice                      // Child component, not a spawnable prefab.
718 ef_fragexplosion
719 ef_line_sparqbeam
720 ef_mist
721 ef_particle_bloodspurtsmall       // Child component, not a spawnable prefab.  Unsaved temporary entity.
722 ef_particle_bloodspurtsmallgreen  // Child component, not a spawnable prefab.  Unsaved temporary entity.
723 ef_particle_bloodspurtsmallyellow // Child component, not a spawnable prefab.  Unsaved temporary entity.
724 ef_particle_bloodspurttiny        // Child component, not a spawnable prefab.  Unsaved temporary entity.
725 ef_particle_camerahit             // Child component, not a spawnable prefab.  Unsaved temporary entity.
726 ef_particle_darthit               // Child component, not a spawnable prefab.  Unsaved temporary entity.
727 ef_particle_sec2muzburst          // Child component, not a spawnable prefab.  Unsaved temporary entity.
728 ef_particle_sec2rotmuzburst       // Child component, not a spawnable prefab.  Unsaved temporary entity.
729 ef_particle_sparksmall            // Child component, not a spawnable prefab.  Unsaved temporary entity.
730 ef_particle_sparksmallblue        // Child component, not a spawnable prefab.  Unsaved temporary entity.
731 ef_particle_sparqhit              // Child component, not a spawnable prefab.  Unsaved temporary entity.
732 ef_sparkspits                     // Unsaved temporary entity.
733 ef_spraydrips
734 ef_steam
735 env_sparksmall
736 TargetIDInstance                  // Unsaved temporary entity.
737 prop_papers01
738 prop_papers02
739 ef_particle_blasterhit
740 ef_particle_ionhit
741 us_puz_panel_blue_wire
742 us_puz_panel_brown_wire
743 us_puz_panel_gray_wire
744 us_puz_panel_red_wire
745 us_puz_panel_teal_wire
746 weapon_grenadeenergmine_live
747 decal_logo_storage

999 PlayerCapsule
*/

public static bool ConstIndexInBounds(int constdex) {
	return (constdex >= 0 && constdex <= 748);
}

public static bool ConstIndexIsGeometry(int constdex) {
	return (constdex >= 0 && constdex <= 306);
}

public static bool ConstIndexIsDynamicObject(int constdex) {
	return ((constdex >= 307 && constdex < 405)
			|| constdex == 417
			|| (constdex >= 458 && constdex < 480) // 480 is static saveable
			|| (constdex >= 481 && constdex < 496)
			|| constdex == 515 || constdex == 517
			|| constdex == 553);
}

public static bool ConstIndexIsDoor(int constdex) {
	return (constdex >= 496 && constdex < 515);
}

public static bool ConstIndexIsStaticObjectSaveable(int constdex) {
	return ((constdex >= 448 && constdex < 458)
			|| constdex == 480 || constdex == 516
			|| (constdex >= 518 && constdex <= 526)
			|| constdex == 530 || constdex == 531 || constdex == 546
			|| constdex == 555 || constdex == 594 || constdex == 596
			|| constdex == 598
			|| (constdex >= 600 && constdex < 603)
			|| (constdex >= 604 && constdex < 616)
			|| (constdex >= 688 && constdex < 693)
			|| constdex == 694 || constdex == 695
			|| (constdex >= 699 && constdex < 704)
			|| (constdex >= 741 && constdex < 746));
}

public static bool ConstIndexIsStaticObjectImmutable(int constdex) {
	return ((constdex >= 527 && constdex < 530)
			|| (constdex >= 532 && constdex < 546)
			|| (constdex >= 547 && constdex < 553)
			|| constdex == 554
			|| (constdex >= 556 && constdex < 594)
			|| constdex == 595 || constdex == 597 || constdex == 599
			|| constdex == 601 || constdex == 603
			|| (constdex >= 616 && constdex < 688)
			|| constdex == 693 || constdex == 696 || constdex == 697
			|| constdex == 698
			|| (constdex >= 704 && constdex < 717)
			|| constdex == 720
			|| (constdex >= 733 && constdex < 736)
			|| (constdex >= 737 && constdex < 739)
			|| constdex == 746
			|| constdex == 747);
}

public static bool ConstIndexIsNPC(int constdex) {
	return (constdex >= 419 && constdex < 448);
}

public static bool ConstIndexIsHardware(int constdex) {
	return (constdex >= 328) && (constdex <= 339);
}

/*
Generic Materials (Const.a.genericMaterials[])
0  col1                 Dark Gray            In hindsight, maybe I should have named these descriptively.
1  col1_unlit           Dark Gray unlit
2  col2                 Yellow
3  col3                 Red
4  col3_bright          Red
5  col3_forcefield      Red
6  col3_forcefieldfaint Red
7  col4                 Blue
8  col4_forcefield      Blue
9  col5_forcefield      Green
10 col6                 Light Gray
11 col7                 Purple
12 col7_forcefield      Purple
13 colwhite             Pure White Emissive
14 console1_1
15 console1_2
16 console1_3
17 console1_4
18 console1_5
19 console1_6
20 console1_7
21 console1_8
22 console1_9
23 console2_1
24 console2_2
25 console2_3
26 console2_4
27 console2_5
28 console2_6
29 console2_7
30 console2_8
31 console2_9
32 console3_1
33 console3_2
34 console3_4
35 console3_7
36 console3_8
37 console3_9
38 citmat1_1
39 citmat1_2
40 citmat1_3
41 citmat1_4
42 citmat1_5
43 citmat1_8
44 citmat1_8_mutated
45 citmat1_9
46 citmat2_1
47 citmat2_4
48 chunk             THE BIG ONE
49 cyberpanel_black
50 cyberpanel_blue
51 cyberpanel_bluegray
52 cyberpanel_cyan
53 cyberpanel_cyandark
54 cyberpanel_gray
55 cyberpanel_green
56 cyberpanel_greendark
57 cyberpanel_orange
58 cyberpanel_orangedark
59 cyberpanel_paleorange
60 cyberpanel_palepurple
61 cyberpanel_palered
62 cyberpanel_paleyellow
63 cyberpanel_purple
64 cyberpanel_red
65 cyberpanel_slice45
66 cyberpanel_slice45_blue
67 cyberpanel_slice45_bluegray
68 cyberpanel_slice45_cyan
69 cyberpanel_slice45_cyandark
70 cyberpanel_slice45_gray
71 cyberpanel_slice45_green
72 cyberpanel_slice45_greendark
73 cyberpanel_slice45_orange
74 cyberpanel_slice45_orangedark
75 cyberpanel_slice45_paleorange
76 cyberpanel_slice45_palepurple
77 cyberpanel_slice45_palered
78 cyberpanel_slice45_paleyellow
79 cyberpanel_slice45_purple
80 cyberpanel_slice45_red
81 cyberpanel_slice45_yellow
82 cyberpanel_touching
83 cyberpanel_white
84 cyberpanel_yellow
85 cyberpanel_yellowdark
86 pipe_maint2_3_coolant
87 text_3dwhite
88 text_3dred
89 text_3dgold
90 text_3dgreen
91 text_3dblack
92 text_3dredStopD
93 text_3dwhiteStopD
94 text_3dblackStopD
95 text_3dgoldStopD
96 text_3dblueStopD
97 text_3dblackStopD
98 text_3dgoldunlit
99 text_3dgoldunlitoverlay
*/
	public static GameObject SpawnDynamicObject(int val, int lev, bool cheat,
												GameObject forcedContainer,
												int saveID) {
		if (!ConstIndexInBounds(val)) {
			Debug.Log("Const index out of bounds: " + val.ToString());
			return null;
		}
		
		if (cheat) {
			Debug.Log("Cheat spawn: " + val.ToString() + ", level: "
					  + lev.ToString() + ", from cheat: " + cheat.ToString()
					  + ", name: "
					  + (forcedContainer == null ? "null" : forcedContainer.name)
					  + ", saveID: " + saveID.ToString());
		}

		if (LevelManager.a == null) { Debug.Log("No LevelManager"); return null; }
		if (cheat && Inventory.a == null) { Debug.Log("No Inventory"); return null; }
		if (cheat && Const.a == null) { Debug.Log("No Const"); return null; }

		if (lev < 0 || lev > 13) lev = 1; // Fallback to Medical.
		Vector3 spawnPos = Vector3.zero;
		if (cheat) spawnPos = PlayerMovement.a.transform.position;
		GameObject go = null;
		if (ConstIndexIsGeometry(val)) {
			if (Const.a.editMode || !cheat) {
				go = MonoBehaviour.Instantiate(Const.a.GetPrefab(val),spawnPos,
									Const.a.quaternionIdentity) as GameObject;
			} else {
				Const.sprint("Indices 0 through 306 (level geometry chunks) "
							 + "not possible when not on edit mode!");
			}
		} else {
			go = MonoBehaviour.Instantiate(Const.a.GetPrefab(val),spawnPos,
									Const.a.quaternionIdentity) as GameObject;
		}

		if (go != null) {
			if (forcedContainer != null) {
				go.transform.SetParent(forcedContainer.transform);
			} else {
				Level levS = LevelManager.a.levelScripts[lev];
				
				GameObject parGO = levS.dynamicObjectsContainer;
				if (ConstIndexIsNPC(val)) {
					parGO = levS.NPCsSaveableInstantiated;
				} else if (ConstIndexIsGeometry(val)) {
					parGO = levS.geometryContainer;
				} else if (ConstIndexIsDynamicObject(val)) {
					parGO = levS.dynamicObjectsContainer;
				}

				go.transform.SetParent(parGO.transform);
			}
			
			if (cheat && ConstIndexIsHardware(val)) { // Hardware
				UseableObjectUse uo = go.GetComponent<UseableObjectUse>();
				int dex14 = Inventory.a.hardware14fromConstdex(uo.useableItemIndex);
				if (Inventory.a.hasHardware[dex14]) {
					uo.customIndex = (Inventory.a.hardwareVersion[dex14] + 1);
				}
			}
		} else {
			Debug.Log("SpawnDynamicObject failure: go == null at the end when "
					  + "trying to spawn constdex " + val.ToString()
					  + ", for level " + lev.ToString() + ", given container "
					  + (forcedContainer == null ? "-" : forcedContainer.name)
					  + ", and saveID of " + saveID.ToString());
		}

		if (go != null) {
			lastSpawnedGO = go;
			if (Application.isPlaying) { // Not called from Test.cs in editor.
				SaveObject so = SaveLoad.GetPrefabSaveObject(go);
				if (so != null) {
					if (saveID <= -1) {
						so.SaveID = Const.a.nextFreeSaveID;
						Const.a.nextFreeSaveID++;
					} else {
						so.SaveID = saveID;
					}
				}
			}
		}
		return go;
	}

	public static GameObject SpawnDynamicObject(int val, int lev, bool cheat,
												int saveID) {
		return SpawnDynamicObject(val, lev, cheat, null, saveID);
	}

	public static GameObject SpawnDynamicObject(int val, int saveID) {
		return SpawnDynamicObject(val,LevelManager.a.currentLevel,false,null,
								  saveID);
	}
	
	public static GameObject SpawnDynamicObject(int val) {
		return SpawnDynamicObject(val,LevelManager.a.currentLevel,false,null,-1);
	}
}
