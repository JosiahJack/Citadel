using System.Collections;
using UnityEngine;

namespace DTValidator.Internal {
	public static class ColorUtil {
		public static Color LerpWhiteBlack(float lerp) {
			return Color.Lerp(Color.white, Color.black, lerp);
		}

		public static Color HexStringToColor(string hex) {
			hex = hex.Replace("0x", "");        // in case the string is formatted 0xFFFFFF
			hex = hex.Replace("#", "");         // in case the string is formatted #FFFFFF
			byte a = 255;                        // assume fully visible unless specified in hex
			byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
			// Only use alpha if the string has enough characters
			if (hex.Length == 8) {
				a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
			}
			return new Color32(r, g, b, a);
		}
	}
}
