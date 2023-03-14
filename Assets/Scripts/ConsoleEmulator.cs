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

			if (Input.GetKeyDown(KeyCode.UpArrow)) SetToCommandMoreDistant();
			if (Input.GetKeyDown(KeyCode.DownArrow)) SetToCommandMoreRecent();
			if ((Input.GetKeyUp(KeyCode.Return)
                  || Input.GetKeyUp(KeyCode.KeypadEnter)
				  || Input.GetKeyDown(KeyCode.JoystickButton0))
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
				MouseLookScript.a.playerCamera.useOcclusionCulling = true;
				PlayerMovement.a.CheatNoclip = false;
				PlayerMovement.a.grounded = false;
				Utils.EnableCapsuleCollider(PlayerMovement.a.capsuleCollider);
				Utils.EnableCapsuleCollider(PlayerMovement.a.leanCapsuleCollider);
				Const.sprint("noclip disabled");
			} else {
				MouseLookScript.a.playerCamera.useOcclusionCulling = false;
				PlayerMovement.a.CheatNoclip = true;
				PlayerMovement.a.grounded = false;
				PlayerMovement.a.rbody.useGravity = false;
				Utils.DisableCapsuleCollider(PlayerMovement.a.capsuleCollider);
				Utils.DisableCapsuleCollider(PlayerMovement.a.leanCapsuleCollider);
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
        } else if (ts.Contains("load") && (tn.Contains("0") || ts.Contains("loadr") || ts.Contains("load r")) && !ts.Contains("arsenal"))CheatLoadLevel(0);
        else if (ts.Contains("load") && tn.Contains("1") && !ts.Contains("arsenal"))  CheatLoadLevel(1);
		else if (ts.Contains("load") && tn.Contains("2") && !ts.Contains("arsenal"))  CheatLoadLevel(2);
		else if (ts.Contains("load") && tn.Contains("3") && !ts.Contains("arsenal"))  CheatLoadLevel(3);
		else if (ts.Contains("load") && tn.Contains("4") && !ts.Contains("arsenal"))  CheatLoadLevel(4);
		else if (ts.Contains("load") && tn.Contains("5") && !ts.Contains("arsenal"))  CheatLoadLevel(5);
		else if (ts.Contains("load") && tn.Contains("6") && !ts.Contains("arsenal"))  CheatLoadLevel(6);
		else if (ts.Contains("load") && tn.Contains("7") && !ts.Contains("arsenal"))  CheatLoadLevel(7);
		else if (ts.Contains("load") && tn.Contains("8") && !ts.Contains("arsenal"))  CheatLoadLevel(8);
		else if (ts.Contains("load") && tn.Contains("9") && !ts.Contains("arsenal"))  CheatLoadLevel(9);
		else if (ts.Contains("load") && ts.Contains("g1") && !ts.Contains("arsenal")) CheatLoadLevel(10);
        else if (ts.Contains("load") && ts.Contains("g2") && !ts.Contains("arsenal")) CheatLoadLevel(11);
        else if (ts.Contains("load") && ts.Contains("g4") && !ts.Contains("arsenal")) CheatLoadLevel(12);
		else if (ts.Contains("load") && ts.Contains("g3") && !ts.Contains("arsenal")) {
			Const.sprint("Gamma grove already jettisoned!  Those poor "
                         + "arrogant people.");
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
			if (val < 438 && val >= 0) {
				SpawnDynamicObject(val,LevelManager.a.currentLevel,true,-1);
			}
        } else if (ts.Contains("undo")) {
			if (lastSpawnedGO != null) Utils.SafeDestroy(lastSpawnedGO);
        } else if (ts.Contains("settargetfps")) {
			int val = Utils.GetIntFromString(ts.Split(' ').Last()); // That's a slow line to compute!
			if (val <= 200 && val > 10) {
				Const.a.TARGET_FPS = val;
				Config.SetVSync();
			}
        } else if (ts.Contains("shake")) {
			Const.a.Shake(true,-1,-1);
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
41  chunk_eng2_1                 ... ok this is getting ridiculous, maybe fill out later
42  chunk_eng2_1_slice45
43  chunk_eng2_1_slice384high
44  chunk_eng2_1_slice384highrh
45  chunk_eng2_1d
46  chunk_eng2_2
47  chunk_eng2_2d
48  chunk_eng2_3
49  chunk_eng2_3d
50  chunk_eng2_4
51  chunk_eng2_5
52  chunk_eng2_5_slice45
53  chunk_eng2_6
54  chunk_exec1_1
55  chunk_exec1_1d
56  chunk_exec1_2
57  chunk_exec1_2d
58  chunk_exec2_1
59  chunk_exec2_2
60  chunk_exec2_2d
61  chunk_exec2_3
62  chunk_exec2_4
63  chunk_exec2_4_slice45
64  chunk_exec2_5
65  chunk_exec2_6
66  chunk_exec2_7
67  chunk_exec3_1
68  chunk_exec3_1d
69  chunk_exec3_2
70  chunk_exec3_4
71  chunk_exec4_1
72  chunk_exec4_2
73  chunk_exec4_3
74  chunk_exec4_4
75  chunk_exec4_5
76  chunk_exec4_6
77  chunk_exec6_1
78  chunk_exteriorpanel1
79  chunk_fan1
80  chunk_flight1_1
81  chunk_flight1_1b
82  chunk_flight1_2
83  chunk_flight1_2_slice45rh
84  chunk_flight1_3
85  chunk_flight1_4
86  chunk_flight1_5
87  chunk_flight1_5_slice45lh
88  chunk_flight1_6
89  chunk_flight2_1
90  chunk_flight2_2
91  chunk_flight2_2_slice45
92  chunk_flight2_3
93  chunk_grove1_1
94  chunk_grove1_2
95  chunk_grove1_2_slice45
96  chunk_grove1_3
97  chunk_grove1_4
98  chunk_grove1_5
99  chunk_grove1_6
100 chunk_grove1_7
101 chunk_grove2_1
102 chunk_grove2_2
103 chunk_grove2_3
104 chunk_grove2_4
105 chunk_grove2_5
106 chunk_grove2_6
107 chunk_grove2_7
108 chunk_grove2_8
109 chunk_grove2_9
110 chunk_grove2_9b
111 chunk_grove2_9c
112 chunk_lift1
113 chunk_maint1_1
114 chunk_maint1_2
115 chunk_maint1_2d
116 chunk_maint1_3
117 chunk_maint1_3b
118 chunk_maint1_4
119 chunk_maint1_4b
120 chunk_maint1_5
121 chunk_maint1_6
122 chunk_maint1_7
123 chunk_maint1_8
124 chunk_maint1_9
125 chunk_maint1_9d
126 chunk_maint2_1
127 chunk_maint2_1b
128 chunk_maint2_1d
129 chunk_maint2_2
130 chunk_maint2_3
131 chunk_maint2_3d
132 chunk_maint2_4
133 chunk_maint2_4d
134 chunk_maint2_5
135 chunk_maint2_5d
136 chunk_maint2_6
137 chunk_maint2_6d
138 chunk_maint2_7
139 chunk_maint2_7d
140 chunk_maint2_8
141 chunk_maint2_9
142 chunk_maint2_9_slice45RH
143 chunk_maint2_9_slice128_top
144 chunk_maint3_1
145 chunk_maint3_1_slice32_lh
146 chunk_maint3_1_slice32_rh
147 chunk_maint3_1_slice45
148 chunk_maint3_1d
149 chunk_med1_1
150 chunk_med1_1_half_top
151 chunk_med1_1_slice128high
152 chunk_med1_1_slice192RH
153 chunk_med1_1_slice256
154 chunk_med1_1d
155 chunk_med1_2
156 chunk_med1_2d
157 chunk_med1_3
158 chunk_med1_3d
159 chunk_med1_4
160 chunk_med1_5
161 chunk_med1_6
162 chunk_med1_7
163 chunk_med1_7_slice14_64
164 chunk_med1_7_slice45_320lh
165 chunk_med1_7_slice45_320rh
166 chunk_med1_7_slice96high
167 chunk_med1_7d
168 chunk_med1_7d_slice128
169 chunk_med1_8
170 chunk_med1_8d
171 chunk_med1_9
172 chunk_med1_9_ofs32_90
173 chunk_med1_9_ofs64_90
174 chunk_med1_9d
175 chunk_med1_9d_ofs48_90
176 chunk_med1_9d_ofs112_90
177 chunk_med1_9d_ofs144_90
178 chunk_med2_1
179 chunk_med2_1_slice32RH
180 chunk_med2_1d
181 chunk_med2_2
182 chunk_med2_2_half_bottom
183 chunk_med2_2d
184 chunk_med2_3
185 chunk_med2_3d
186 chunk_med2_4
187 chunk_med2_5
188 chunk_med2_6
189 chunk_med2_7
190 chunk_med2_8
191 chunk_med2_8_half_top
192 chunk_med2_8_slice32RH
193 chunk_med2_8_slice45
194 chunk_med2_9
195 chunk_med2_9d
196 chunk_med3_1
197 chunk_rad1_1
198 chunk_rad1_2
199 chunk_reac1_1
200 chunk_reac1_1_slice45
201 chunk_reac1_2
202 chunk_reac1_3
203 chunk_reac1_4
204 chunk_reac1_5
205 chunk_reac1_6
206 chunk_reac1_7
207 chunk_reac1_8
208 chunk_reac1_9
209 chunk_reac2_1
210 chunk_reac2_1_slice45LH
211 chunk_reac2_1_slice45LH_up
212 chunk_reac2_1_slice45RH
213 chunk_reac2_1_slice45RH_up
214 chunk_reac2_1b
215 chunk_reac2_1bmirror
216 chunk_reac2_1mirror
217 chunk_reac2_2
218 chunk_reac2_4
219 chunk_reac2_4_slice128lower
220 chunk_reac2_5
221 chunk_reac2_6
222 chunk_reac2_7
223 chunk_reac2_8
224 chunk_reac2_9
225 chunk_reac3_1
226 chunk_reac3_2
227 chunk_reac3_3
228 chunk_reac3_4
229 chunk_reac3_5
230 chunk_reac3_6
231 chunk_reac3_7
232 chunk_reac4_1
233 chunk_reac4_1_slice45lh
234 chunk_reac4_2
235 chunk_reac5_1
236 chunk_reac5_2
237 chunk_reac5_3
238 chunk_reac6_1
239 chunk_reac6_2
240 chunk_reac6_3
241 chunk_sci1_1
242 chunk_sci1_1_slice45_toplh
243 chunk_sci1_1_slice45_toprh
244 chunk_sci1_1d
245 chunk_sci1_2
246 chunk_sci1_2_slice45lh
247 chunk_sci1_2_slice45lh_up
248 chunk_sci1_2_slice45rh
249 chunk_sci1_2_slice45rh_up
250 chunk_sci1_2d
251 chunk_sci1_3
252 chunk_sci1_4
253 chunk_sci1_5
254 chunk_sci1_6
255 chunk_sci1_6_slice45
256 chunk_sci1_7
257 chunk_sci1_7d
258 chunk_sci1_8
259 chunk_sci1_8d
260 chunk_sci1_9
261 chunk_sci1_9d
262 chunk_sci2_1
263 chunk_sci2_1_slice45lh
264 chunk_sci2_1_slice45rh
265 chunk_sci2_1d
266 chunk_sci2_2
267 chunk_sci2_2d
268 chunk_sci2_3
269 chunk_sci2_4
270 chunk_sci2_5
271 chunk_sci2_5d
272 chunk_sci3_1
273 chunk_sci3_1d
274 chunk_sci3_2
275 chunk_sci3_3
276 chunk_sci3_4
277 chunk_sci3_5
278 chunk_sci3_6
279 chunk_screen
280 chunk_sec1_1
281 chunk_sec1_1b
282 chunk_sec1_1c
283 chunk_sec1_1c_slice45
284 chunk_sec1_1c_slice64highlh
285 chunk_sec1_1c_slice64highrh
286 chunk_sec1_1c_slice320high
287 chunk_sec1_1c_slice320highrh
288 chunk_sec1_2
289 chunk_sec1_2b
290 chunk_sec1_3
291 chunk_sec1_3_slice45
292 chunk_stor1_1
293 chunk_stor1_2
294 chunk_stor1_3
295 chunk_stor1_4
296 chunk_stor1_5
297 chunk_stor1_6
298 chunk_stor1_6_slice128_up_lh
299 chunk_stor1_6_slice128_uprh
300 chunk_stor1_6_slice192lh
301 chunk_stor1_6_slice192rh
302 chunk_stor1_7
303 chunk_stor1_7_slice45
304 chunk_stor1_7d
305 chunk_teleporter
306 chunk_white                 Const.a.useableItems
307 item_paper_wad              0
308 prop_phys_warecasing        1
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
409 null                        102
410 null                        103
411 null                        104
412 null                        105
413 null                        106
414 null                        107
415 null                        108
416 null                        109    Access Card Type
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
457 item_cyber_turbo              Misc    9 
458 prop_phys_barrel_chemical     0
459 prop_phys_barrel_radiation    1
460 prop_phys_barrel_toxic        2
461 prop_phys_cart                3
462 prop_phys_pot                 4
463 prop_phys_toolcart            5 
464 se_briefcase                  6
465 se_corpse_blueshirt           7
466 se_corpse_brownshirt          8
467 se_corpse_eaten               9
468 se_corpse_labcoat             10
469 se_corpse_security            11
470 se_corpse_tan                 12
471 se_corpse_torso               13
472 se_crate1                     14
473 se_crate2                     15
474 se_crate3                     16
475 se_crate4                     17
476 se_crate5                     18
477 sec_camera                    19
478 sec_cpunode                   20
479 sec_cpunode_small             21
480 weapon_cyber_mine      Proj   22
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
514 door_secret3              18  Misc
515 func_forcebridge              23
516 prop_lift2                    24
517 func_wall                     25
518 BulletHoleLarge
519 BulletHoleScorchLarge
520 BulletHoleScorchSmall
521 BulletHoleSmall
522 BulletHoleTiny
523 BulletHoleTinySpread

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

*/

	public static GameObject SpawnDynamicObject(int val, int lev, bool cheat,
												GameObject forcedContainer,
												int saveID) {
		if (LevelManager.a == null) {
			Debug.Log("Missing LevelManager");
			return null;
		}
		if (cheat && Inventory.a == null) {
			Debug.Log("Missing Inventory");
			return null;
		}
		if (cheat && Const.a == null) {
			Debug.Log("Missing Const");
			return null;
		}

		Vector3 spawnPos = Vector3.zero;
		if (cheat) spawnPos = PlayerMovement.a.transform.position;

		GameObject go = null;
		if (val >= 0 && val < 307) {			// [0, 306]
			if (val > (Const.a.chunkPrefabs.Length - 1)) {
				Debug.Log("SpawnDynamicObject failure: val > (Const.a.chunkPrefabs.Length - 1), val: " + val.ToString());
				return null;
			}
			if (Const.a.chunkPrefabs[val] == null) {
				Debug.Log("SpawnDynamicObject failure: Const.a.chunkPrefabs[val] == null, val: " + val.ToString());
				return null;
			}

			go = MonoBehaviour.Instantiate(Const.a.chunkPrefabs[val],spawnPos,
									  Const.a.quaternionIdentity) as GameObject;
		} else if (val >= 307 && val < 419) {	// [307, 418]
			val -= 307;
			if (val > (Const.a.useableItems.Length - 1)) {
				Debug.Log("SpawnDynamicObject failure: val > (Const.a.useableItems.Length - 1), val: " + val.ToString());
				return null;
			}
			if (Const.a.useableItems[val] == null) {
				Debug.Log("SpawnDynamicObject failure: val > Const.a.useableItems[val] == null, val: " + val.ToString());
				return null;
			}

			go = MonoBehaviour.Instantiate(Const.a.useableItems[val],spawnPos,
									  Const.a.quaternionIdentity) as GameObject;
		} else if (val >= 419 && val < 448) {	// [419, 447]
			val -= 419;
			if (val > (Const.a.npcPrefabs.Length - 1)) {
				Debug.Log("SpawnDynamicObject failure: val > (Const.a.npcPrefabs.Length - 1), val: " + val.ToString());
				return null;
			}
			if (Const.a.npcPrefabs[val] == null) {
				Debug.Log("SpawnDynamicObject failure: Const.a.npcPrefabs[val] == null, val: " + val.ToString());
				return null;
			}

			go = MonoBehaviour.Instantiate(Const.a.npcPrefabs[val],spawnPos,
									  Const.a.quaternionIdentity) as GameObject;
		} else if (val >= 448 && val < 458) {	// [448, 457]
			val -= 448;
			if (val > (Const.a.cyberItemPrefabs.Length - 1)) {
				Debug.Log("SpawnDynamicObject failure: val > (Const.a.cyberItemPrefabs.Length - 1), val: " + val.ToString());
				return null;
			}
			if (Const.a.cyberItemPrefabs[val] == null) {
				Debug.Log("SpawnDynamicObject failure: Const.a.cyberItemPrefabs[val] == null, val: " + val.ToString());
				return null;
			}

			go = MonoBehaviour.Instantiate(Const.a.cyberItemPrefabs[val],
							 spawnPos,Const.a.quaternionIdentity) as GameObject;
		} else if ((val >= 458 && val < 481) 	// [458, 480], [515, 517]
				   || val == 515
				   || val == 516
				   || val == 517) {
			if (val == 515) val = 23; // func_forcebridge out of order exception.
			else if (val == 516) val = 24; // prop_lift2 out of order exception.
			else if (val == 517) val = 25; // func_wall out of order exception.
			else val -= 458;

			if (val > (Const.a.miscellaneousPrefabs.Length - 1)) {
				Debug.Log("SpawnDynamicObject failure: val > (Const.a.miscellaneousPrefabs.Length - 1), val: " + val.ToString());
				return null;
			}
			if (Const.a.miscellaneousPrefabs[val] == null) {
				Debug.Log("SpawnDynamicObject failure: Const.a.miscellaneousPrefabs[val] == null, val: " + val.ToString());
				return null;
			}

			go = MonoBehaviour.Instantiate(Const.a.miscellaneousPrefabs[val],
							 spawnPos,Const.a.quaternionIdentity) as GameObject;
		} else if (val >= 481 && val < 496) {	// [481, 495]
			val -= 481;
			if (val > (Const.a.projectilesLaunched.Length - 1)) {
				Debug.Log("SpawnDynamicObject failure: val > (Const.a.projectilesLaunched.Length - 1), val: " + val.ToString());
				return null;
			}
			if (Const.a.projectilesLaunched[val] == null) {
				Debug.Log("SpawnDynamicObject failure: Const.a.projectilesLaunched[val] == null, val: " + val.ToString());
				return null;
			}

			go = MonoBehaviour.Instantiate(Const.a.projectilesLaunched[val],
							 spawnPos,Const.a.quaternionIdentity) as GameObject;
		} else if (val >= 496 && val < 515) {	// [496, 514]
			val -= 496;
			if (val > (Const.a.doorPrefabs.Length - 1)) {
				Debug.Log("SpawnDynamicObject failure: val > (Const.a.doorPrefabs.Length - 1), val: " + val.ToString());
				return null;
			}
			if (Const.a.doorPrefabs[val] == null) {
				Debug.Log("SpawnDynamicObject failure: Const.a.doorPrefabs[val] == null, val: " + val.ToString());
				return null;
			}

			go = MonoBehaviour.Instantiate(Const.a.doorPrefabs[val],spawnPos,
									  Const.a.quaternionIdentity) as GameObject;
		}

		if (go != null) {
			if (forcedContainer != null) {
				go.transform.SetParent(forcedContainer.transform);
			} else {
				go.transform.SetParent(LevelManager.a.levelScripts[lev].dynamicObjectsContainer.transform);
			}
			if (cheat && (val < 33) && (val > 20)) {
				UseableObjectUse uo = go.GetComponent<UseableObjectUse>();
				int dex14 = Inventory.a.hardware14fromConstdex(uo.useableItemIndex);
				if (Inventory.a.hasHardware[dex14]) {
					uo.customIndex = (Inventory.a.hardwareVersion[dex14] + 1);
				}
			}
		} else Debug.Log("SpawnDynamicObject failure: go == null at the end");

		if (go != null) {
			lastSpawnedGO = go;
			if (Application.isPlaying) { // Not called from Test.cs in editor.
				SaveObject sob = go.GetComponent<SaveObject>();
				if (sob != null) {
					if (saveID <= -1) {
						sob.SaveID = Const.a.nextFreeSaveID;
						Const.a.nextFreeSaveID++;
					} else sob.SaveID = saveID;
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
}