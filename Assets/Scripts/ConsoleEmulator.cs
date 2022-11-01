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

			if (Input.GetKeyDown(KeyCode.UpArrow)) SetToCommandMoreDistant();
			if (Input.GetKeyDown(KeyCode.DownArrow)) SetToCommandMoreRecent();
			if ((Input.GetKeyUp(KeyCode.Return)
                  || Input.GetKeyUp(KeyCode.KeypadEnter))
                && !PauseScript.a.mainMenu.activeSelf == true) {

                string enteredText = PlayerMovement.a.consoleinpFd.text;
                ShiftLastCommand(enteredText);
                ConsoleEntry(enteredText);
            }
		} else {
			if (PlayerMovement.a.consoleplaceholderText.activeSelf) {
                PlayerMovement.a.consoleplaceholderText.SetActive(false);
            }
		}
	}

    private static void SetToCommandMoreDistant() {
        if (string.IsNullOrWhiteSpace(lastCommand[0])) return; // No entries.

        string currentText = PlayerMovement.a.consoleentryText.text;
        if (string.IsNullOrWhiteSpace(PlayerMovement.a.consoleentryText.text)
            && currentText != lastCommand[0]) {
            PlayerMovement.a.consoleentryText.text = lastCommand[0];
            return;
        }

        consoleMemdex++;
        if (consoleMemdex > 6) consoleMemdex = 6;
        if (string.IsNullOrWhiteSpace(lastCommand[consoleMemdex])) return;

        PlayerMovement.a.consoleentryText.text = lastCommand[consoleMemdex];
    }

    private static void SetToCommandMoreRecent() {
        if (string.IsNullOrWhiteSpace(lastCommand[0])) return; // No entries.
        if (string.IsNullOrWhiteSpace(PlayerMovement.a.consoleentryText.text)) {
            return;
        }

        consoleMemdex--;
        if (consoleMemdex < 0) consoleMemdex = 0;
        PlayerMovement.a.consoleentryText.text = lastCommand[consoleMemdex];
    }

    private static void ShiftLastCommand(string entry) {
        if (string.IsNullOrWhiteSpace(entry)) return; // Only remember real cmd.
        if (lastCommand[0] == entry) return; // Only remember unique cmds.

        lastCommand[6] = lastCommand[5];
        lastCommand[5] = lastCommand[4];
        lastCommand[4] = lastCommand[3];
        lastCommand[3] = lastCommand[2];
        lastCommand[2] = lastCommand[1];
        lastCommand[1] = lastCommand[0];
        lastCommand[0] = entry;
    }

    private static void ConsoleEntry(string entry) {
		string ts = entry.ToLower(); // test string = lower case text
		string tn = entry; // test number = number searching
        if (ts.Contains("noclip") || ts.Contains("idclip")
            || ts.Contains("no clip")) {
			if (PlayerMovement.a.CheatNoclip) {
				PlayerMovement.a.CheatNoclip = false;
				PlayerMovement.a.grounded = false;
				PlayerMovement.a.capsuleCollider.enabled = true;
				PlayerMovement.a.leanCapsuleCollider.enabled = true;
				Const.sprint("noclip disabled");
			} else {
				PlayerMovement.a.CheatNoclip = true;
				PlayerMovement.a.grounded = false;
				PlayerMovement.a.rbody.useGravity = false;
				PlayerMovement.a.capsuleCollider.enabled = false;
				PlayerMovement.a.leanCapsuleCollider.enabled = false;
				Const.sprint("noclip activated!");
			}
        } else if (ts.Contains("notarget") || ts.Contains("no target")) {
			if (PlayerMovement.a.Notarget) {
				PlayerMovement.a.Notarget = false;
				Const.sprint("notarget disabled");
			} else {
				PlayerMovement.a.Notarget = true;
				Const.sprint("notarget activated!");
			}
        } else if (ts.Contains("god")
                   || (ts.Contains("power") && ts.Contains("overwhelming"))
                   || ts.Contains("whosyourdaddy")
                   || ts.Contains("iddqd")) {
			if (PlayerMovement.a.hm.god) {
				Const.sprint("god mode disabled");
				PlayerMovement.a.hm.god = false;
			} else {
				Const.sprint("god mode activated!");
				PlayerMovement.a.hm.god = true;
			}
        } else if (ts.Contains("load") && tn.Contains("0"))CheatLoadLevel(0);
        else if (ts.Contains("load") && tn.Contains("1"))  CheatLoadLevel(1);
		else if (ts.Contains("load") && tn.Contains("2"))  CheatLoadLevel(2);
		else if (ts.Contains("load") && tn.Contains("3"))  CheatLoadLevel(3);
		else if (ts.Contains("load") && tn.Contains("4"))  CheatLoadLevel(4);
		else if (ts.Contains("load") && tn.Contains("5"))  CheatLoadLevel(5);
		else if (ts.Contains("load") && tn.Contains("6"))  CheatLoadLevel(6);
		else if (ts.Contains("load") && tn.Contains("7"))  CheatLoadLevel(7);
		else if (ts.Contains("load") && tn.Contains("8"))  CheatLoadLevel(8);
		else if (ts.Contains("load") && tn.Contains("9"))  CheatLoadLevel(9);
		else if (ts.Contains("load") && ts.Contains("g1")) CheatLoadLevel(10);
        else if (ts.Contains("load") && ts.Contains("g2")) CheatLoadLevel(11);
        else if (ts.Contains("load") && ts.Contains("g4")) CheatLoadLevel(12);
		else if (ts.Contains("load") && ts.Contains("g3")) {
			Const.sprint("Gamma grove already jettisoned!  Those poor "
                         + "arrogant people.");
		} else if (ts.Contains("load") && ts.Contains("arsenal")) {
            if (ts.Contains("arsenalr") || ts.Contains("arsenal r"))
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
                Const.sprint("Gamma grove already jettisoned!  Those poor "
                            + "arrogant people.");
            }
        } else if (ts.Contains("bottomless") && ts.Contains("clip")) { // bottomlessclip
			if (WeaponCurrent.a.bottomless) {
				Const.sprint("Hose disconnected from interdimensional " + 
                             "wormhole. Normal ammo operation restored.");
				WeaponCurrent.a.bottomless = false;
			} else {
				Const.sprint("bottomlessclip!  Bring it!");
				WeaponCurrent.a.bottomless = true;
			}
        } else if (ts.Contains("ifeelthepower")
                   || (ts.Contains("i") && ts.Contains("feel")
                       && ts.Contains("the") && ts.Contains("power"))) {
			if (WeaponCurrent.a.redbull) {
				Const.sprint("Energy usage normal");
				WeaponCurrent.a.redbull = false;
			} else {
				Const.sprint("I feel the power! 0 energy consumption!");
				WeaponCurrent.a.redbull = true; // Might not be wings, but hey.
			}
        } else if (ts.Contains("show") && ts.Contains("fps")) { // showfps
			Const.sprint("Toggling FPS counter for framerate (bottom right corner)...");
			PlayerMovement.a.fpsCounter.SetActive(!PlayerMovement.a.fpsCounter.activeInHierarchy);
        } else if (ts.Contains("show") && ts.Contains("location")) { // showlocation
			Const.sprint("Toggling locationIndicator (bottom left corner)...");
			PlayerMovement.a.locationIndicator.SetActive(!PlayerMovement.a.locationIndicator.activeInHierarchy);
		} else if (ts.Contains("i") && ts.Contains("am") && ts.Contains("shodan")) { // iamshodan
			if (LevelManager.a.superoverride) {
				Const.sprint("SHODAN has regained control of security from you");
				LevelManager.a.superoverride = false;
			} else {
				Const.sprint("Full security override enabled!");
				LevelManager.a.superoverride = true;
			}
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
				else if (ts.Contains("branch") || ts.Contains("-b")) Const.sprint("Created new branch " + Utils.GetIntFromString(ts.Split(' ').Last()));
				else if (ts.Contains("checkout")) Const.sprint("Branch name not recognized.  Contact your TriopBucket representative.");
				else Const.sprint("Branch name not recognized.  Contact your TriopBucket representative.");
		} else if (ts.Contains("restart")) {
				Const.sprint("Yeah...better not");
		} else if (ts.Contains("quit") || ts.Contains("exit")) {
				Const.sprint("Use the Pause Menu by hitting Escape and clicking QUIT");
		} else if (ts.Contains("cd") || ts.Contains("./")) {
				Const.sprint("Attempting to access directory... already at root");
		} else if (ts.Contains("kill") || ts.Contains("kick") || ts.Contains("ban") || ts.Contains("destroy") || ts.Contains("attack") || ts.Contains("suicide") || ts.Contains("die")) {
				Const.sprint("Player decides to become a cyborg.");
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
			int val = Utils.GetIntFromString(ts.Split(' ').Last()); // That's a slow line to compute!
			if (val < 102 && val >= 0) {
				GameObject cheatObject = MonoBehaviour.Instantiate(Const.a.useableItems[val],PlayerMovement.a.transform.position,Const.a.quaternionIdentity) as GameObject;
				if (cheatObject != null) {
					cheatObject.transform.SetParent(LevelManager.a.GetCurrentDynamicContainer().transform);
					if (val < 33 && val > 20) {
						UseableObjectUse uo = cheatObject.GetComponent<UseableObjectUse>();
						int dex14 = Inventory.a.hardware14fromConstdex(uo.useableItemIndex);
						if (Inventory.a.hasHardware[dex14]) {
							uo.customIndex = (Inventory.a.hardwareVersion[dex14] + 1);
						}
					}
				}
			}
        } else if (ts.Contains("const.")) {
			string numGet = Regex.Match(ts, @"\d+").Value;
			int numGot = Int32.Parse(numGet);
			if (numGot >= 0) {
				// Debug value parsing within build
				if (ts.Contains("useableItemsNameText")) {
					if (numGot < Const.a.useableItemsNameText.Length) Const.sprint(Const.a.useableItemsNameText[numGot]);
					else Const.sprint("Value of " + numGot.ToString() + " was outside of bounds, needs to be 0 - " + Const.a.useableItemsNameText.Length.ToString());
				} else if (ts.Contains("isFullAutoForWeapon")) {
					if (numGot < Const.a.isFullAutoForWeapon.Length) Const.sprint(Const.a.isFullAutoForWeapon[numGot].ToString());
					else Const.sprint("Value of " + numGot.ToString() + " was outside of bounds, needs to be 0 - " + Const.a.isFullAutoForWeapon.Length.ToString());
				} else if (ts.Contains("moveTypeForNPC")) {
					if (numGot < Const.a.moveTypeForNPC.Length) Const.sprint(Const.a.moveTypeForNPC[numGot].ToString());
					else Const.sprint("Value of " + numGot.ToString() + " was outside of bounds, needs to be 0 - " + Const.a.moveTypeForNPC.Length.ToString());
				}
			}
		} else {
            Const.sprint("Uknown command or function: " + entry);
        }

        PlayerMovement.a.consoleinpFd.text = ""; // Reset console and hide it, command was entered.
        PlayerMovement.a.ToggleConsole();
    }

    private static void CheatLoadLevel(int lev) {
        LevelManager.a.CheatLoadLevel(lev);
    }
}