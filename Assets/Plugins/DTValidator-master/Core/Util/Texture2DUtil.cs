using System.Collections.Generic;
using UnityEngine;

namespace DTValidator {
	public static class Texture2DUtil {
		// PRAGMA MARK - Public
		public static Texture2D ClearTexture {
			get { return clearTexture_ ?? (clearTexture_ = new Texture2D(0, 0)); }
		}

		public static Texture2D CreateTextureWithColor(Color color, int width = 1, int height = 1) {
			Color[] pixels = new Color[width * height];

			for (int i = 0; i < pixels.Length; i++) {
				pixels[i] = color;
			}

			Texture2D tex = new Texture2D(width, height);
			tex.SetPixels(pixels);
			tex.hideFlags = HideFlags.DontSave;
			tex.Apply();

			return tex;
		}

		public static Texture2D GetCached1x1TextureWithColor(Color color) {
			return cached1x1Textures_.GetOrCreateCached(color, c => CreateTextureWithColor(c));
		}


		// PRAGMA MARK - Internal
		private static Texture2D clearTexture_;

		private static Dictionary<Color, Texture2D> cached1x1Textures_ = new Dictionary<Color, Texture2D>();
	}
}