using System.Collections;
using UnityEngine;

namespace DTValidator.Internal {
	public static class ColorExtensions {
		public static string ToHexString(this Color c) {
			return string.Format("#{0:X2}{1:X2}{2:X2}", MathUtil.ConvertToByte(c.r), MathUtil.ConvertToByte(c.g), MathUtil.ConvertToByte(c.b));
		}

		public static Color WithAlpha(this Color c, float alpha) {
			c.a = alpha;
			return c;
		}
	}
}
