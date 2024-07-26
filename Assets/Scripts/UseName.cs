using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class UseName : MonoBehaviour {
	public string targetname;

	public static void UseNameSprint(GameObject go) {
		UseName un = go.GetComponent<UseName>();
		PrefabIdentifier pid = go.GetComponent<PrefabIdentifier>();
		if (pid == null) { // Ok, maybe the parent has it.
			pid = go.transform.parent.gameObject.GetComponent<PrefabIdentifier>();
			if (pid == null) { // Ok, maybe the grandparent?
				pid = go.transform.parent.parent.gameObject.GetComponent<PrefabIdentifier>();
			}
		}

		if (un == null) { // Ok, maybe the parent has it.
			un = go.transform.parent.gameObject.GetComponent<UseName>();
		}

		if (un == null) {// Ok...so maybe a child has UseName on it
			un = go.GetComponentInChildren<UseName>(true);
		}

		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		s1.Append(Const.a.stringTable[29]); // "Can't use "
		int id = -1;
		if (pid != null) id = pid.constIndex;
		switch(id) {
			case 0:   s1.Append("inky blackness"); break;
			case 1:   s1.Append("window"); break;
			case 2:   s1.Append("biological infestation"); break;
			case 3:   s1.Append("biological infestation"); break;
			case 4:   s1.Append("biological infestation"); break;
			case 5:   s1.Append("biological infestation"); break;
			case 6:   s1.Append("biological infestation"); break;
			case 7:   s1.Append("biological infestation"); break;
			case 8:   s1.Append("biological infestation"); break;
			case 9:   s1.Append("biological infestation"); break;
			case 10:  s1.Append("biological infestation"); break;
			case 11:  s1.Append("data transfer schematic"); break;
			case 12:  s1.Append("stone mosiac tiling"); break;
			case 13:  s1.Append("monitoring post"); break;
			case 14:  s1.Append("video observation screen"); break;
			case 15:  s1.Append("cyber monitoring station"); break;
			case 16:  s1.Append("burnished platinum panelling"); break;
			case 17:  s1.Append("monitoring ports"); break;
			case 18:  s1.Append("SHODAN neural bud"); break;
			case 19:  s1.Append("computer"); break;
			case 20:  s1.Append("secure crate"); break;
			// Cyber panel
			// Cyber panel 45
			case 23:  s1.Append("environmental regulator"); break;
			case 24:  s1.Append("damaged environmental regulator"); break;
			case 25:  s1.Append("fluid transport pipes"); break;
			case 26:  s1.Append("damaged fluid transport pipes"); break;
			case 27:  s1.Append("engineering panelling"); break;
			case 28:  s1.Append("damaged engineering panelling"); break;
			case 29:  s1.Append("ladder, but it can be climbed"); break;
			case 30:  s1.Append("engineering panelling"); break;
			case 31:  s1.Append("engineering panelling"); break;
			case 32:  s1.Append("engineering panelling"); break;
			case 33:  s1.Append("damaged engineering panelling"); break;
			case 34:  s1.Append("system function gauges"); break;
			case 35:  s1.Append("damaged system function gauges"); break;
			case 36:  s1.Append("engineering instruments"); break;
			case 37:  s1.Append("damaged engineering instruments"); break;
			case 38:  s1.Append("electric cable access"); break;
			case 39:  s1.Append("data circuit access port"); break;
			case 40:  s1.Append("damaged data circuit access port"); break;
			case 41:  s1.Append("hi-grip surface"); break;
			case 42:  s1.Append("hi-grip surface"); break;
			case 43:  s1.Append("hi-grip surface"); break;
			case 44:  s1.Append("hi-grip surface"); break;
			case 45:  s1.Append("damaged hi-grip surface"); break;
			case 46:  s1.Append("halogen light fixture"); break;
			case 47:  s1.Append("damaged halogen light fixture"); break;
			case 48:  s1.Append("observation station"); break;
			case 49:  s1.Append("damaged observation station"); break;
			case 50:  s1.Append("thick rug"); break;
			case 51:  s1.Append("modular panelling"); break;
			case 52:  s1.Append("modular panelling"); break;
			case 53:  s1.Append("He-3 extraction pumping system"); break;
			case 54:  s1.Append("soft panelling"); break;
			case 55:  s1.Append("damaged soft panelling"); break;
			case 56:  s1.Append("tech-rack"); break;
			case 57:  s1.Append("damaged tech-rack"); break;
			case 58:  s1.Append("corridor wall"); break;
			case 59:  s1.Append("corridor wall"); break;
			case 60:  s1.Append("damaged corridor wall"); break;
			case 61:  s1.Append("oak panelling"); break;
			case 62:  s1.Append("titanium panelling"); break;
			case 63:  s1.Append("titanium panelling"); break;
			case 64:  s1.Append("copper coated insulation"); break;
			case 65:  s1.Append("copper coated insulation with light"); break;
			case 66:  s1.Append("copper coated insulation"); break;
			case 67:  s1.Append("corridor wall"); break;
			case 68:  s1.Append("damaged corridor wall"); break;
			case 69:  s1.Append("corridor wall"); break;
			case 70:  s1.Append("carpet"); break;
			case 71:  s1.Append(Const.a.stringTable[593]); break; // ATM, cyber security lockout
			case 72:  s1.Append("elevator panelling"); break;
			case 73:  s1.Append("elevator panelling"); break;
			case 74:  s1.Append("transaction machine, market closed due to active incident"); break;
			case 75:  s1.Append("carpet"); break;
			case 76:  s1.Append("marble slab"); break;
			case 77:  s1.Append("display screen"); break;
			case 78:  s1.Append("Citadel Space Station"); break;
			case 79:  s1.Append("ventilation system"); break;
			case 80:  s1.Append("energ-light"); break;
			case 81:  s1.Append("energ-light"); break;
			case 82:  s1.Append("non-dent steel panelling"); break;
			case 83:  s1.Append("non-dent steel panelling"); break;
			case 84:  s1.Append("non-dent steel panelling with logo"); break;
			case 85:  s1.Append("non-dent steel panelling"); break;
			case 86:  s1.Append("non-dent steel panelling"); break;
			case 87:  s1.Append("non-dent steel panelling"); break;
			case 88:  s1.Append("environmental regulator"); break;
			case 89:  s1.Append("structural panelling"); break;
			case 90:  s1.Append("energ-light"); break;
			case 91:  s1.Append("energ-light"); break;
			case 92:  s1.Append("energ-light"); break;
			case 93:  s1.Append("observation ceiling"); break;
			case 94:  s1.Append("grass"); break;
			case 95:  s1.Append("grass"); break;
			case 96:  s1.Append("grass"); break;
			case 97:  s1.Append("wet grass"); break;
			case 98:  s1.Append("virus infestation"); break;
			case 99:  s1.Append("virus infestation"); break;
			case 100: s1.Append("virus infestation"); break;
			case 101: s1.Append("pod wall"); break;
			case 102: s1.Append("overgrown pod wall"); break;
			case 103: s1.Append("pod wall"); break;
			case 104: s1.Append("pod wall with environmental regulator"); break;
			case 105: s1.Append("pod wall with sprinkler system"); break;
			case 106: s1.Append("pod wall with overgrowth"); break;
			case 107: s1.Append("pod wall with sprinkler system"); break;
			case 108: s1.Append("pod wall with overgrowth"); break;
			case 109: s1.Append("virus infestation"); break;
			case 110: s1.Append("virus infestation"); break;
			case 111: s1.Append("virus infestation"); break;
			case 112: s1.Append("gravity lift"); break;
			case 113: s1.Append("locked extinguisher storage cabinets"); break;
			case 114: s1.Append("locked storage cabinets"); break;
			case 115: s1.Append("damaged storage cabinets"); break;
			case 116: s1.Append("chemical storage tanks"); break;
			case 117: s1.Append("damaged chemical storage tanks"); break;
			case 118: s1.Append("repair station"); break;
			case 119: s1.Append("damaged repair station"); break;
			case 120: s1.Append("repair station"); break;
			case 121: s1.Append("robot diagnostic system"); break;
			case 122: s1.Append("repair station"); break;
			// chunk_maint1_8 deleted, duplicate of chunk_maint1_1    case 123:
			case 124: s1.Append("industrial tiles"); break;
			case 125: s1.Append("damaged industrial tiles"); break;
			case 126: s1.Append("quartz light fixture"); break;
			case 127: s1.Append("ladder"); break;
			case 128: s1.Append("damaged quartz light fixture"); break;
			case 129: s1.Append("structural panelling with light"); break;
			case 130: s1.Append("grating"); break;

			case 202: // halogen lamp
				s1.Clear();
				s1.Append(Const.a.stringTable[594]); // "bulb needs replacing"
				break;
			case 207: // diagnostic module, comm panel
				s1.Clear();
				s1.Append(Const.a.stringTable[595]); // "inactive"
				break;
			case 208: // diagnostic module, comm panel
				s1.Clear();
				s1.Append(Const.a.stringTable[595]); // "inactive"
				break;
			case 234: // halogen lamp
				s1.Clear();
				s1.Append(Const.a.stringTable[594]); // "bulb needs replacing"
				break;
			default:
				if (un != null) s1.Append(un.targetname);
				break;
		}

		Const.sprint(s1.ToString(),MouseLookScript.a.player);
	}
}
