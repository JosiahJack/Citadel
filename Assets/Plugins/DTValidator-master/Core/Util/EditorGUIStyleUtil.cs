#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DTValidator {
	public static class EditorGUIStyleUtil {
		// PRAGMA MARK - Public Interface
		public const float kTitleHeight = 20.0f;
		public static readonly GUILayoutOption TitleHeight = GUILayout.Height(kTitleHeight);

		public static GUIStyle StyleWithBackgroundColor(Color color) {
			return EditorGUIStyleUtil.StyleWithTexture(Texture2DUtil.GetCached1x1TextureWithColor(color));
		}

		public static GUIStyle StyleWithTexture(Texture2D texture, Action<GUIStyle> customizationCallback = null) {
			return EditorGUIStyleUtil.StyleWithTexture(GUIStyle.none, texture, customizationCallback);
		}

		public static GUIStyle StyleWithTexture(GUIStyle baseStyle, Texture2D texture, Action<GUIStyle> customizationCallback = null) {
			GUIStyle style = EditorGUIStyleUtil._cachedTextureStyles.GetValueOrDefault(texture);
			if (style == null) {
				style = new GUIStyle(baseStyle);
				style.normal.background = texture;
				if (customizationCallback != null) {
					customizationCallback.Invoke(style);
				}
				EditorGUIStyleUtil._cachedTextureStyles[texture] = style;
			}

			return style;
		}

		public static GUIStyle CachedLabelTitleStyle() {
			return EditorGUIStyleUtil.CachedStyle("EditorGUIStyleUtil::LabelTitle", GUI.skin.label, (style) => {
				style.fontSize = 14;
				style.fontStyle = FontStyle.Bold;
			});
		}

		public static GUIStyle CachedAlignedButtonStyle() {
			return EditorGUIStyleUtil.CachedStyle("EditorGUIStyleUtil::AlignedButton", GUI.skin.button, (style) => {
				style.margin.top = 0;
				style.padding.top = 3;
			});
		}

		public static GUIStyle CachedTextAreaStyleWithWordWrap() {
			return EditorGUIStyleUtil.CachedStyleWithWordWrap("EditorGUIStyleUtil::TextAreaWordWrap", GUI.skin.textArea);
		}

		public static GUIStyle CachedLabelStyleWithWordWrap() {
			return EditorGUIStyleUtil.CachedStyleWithWordWrap("EditorGUIStyleUtil::LabelWordWrap", GUI.skin.label);
		}

		public static GUIStyle CachedColorlessFoldoutStyle() {
			return EditorGUIStyleUtil.CachedStyle("EditorGUIStyleUtil::ColorlessFoldout", EditorStyles.foldout, (style) => {
				style.hover.textColor = style.normal.textColor;
				style.focused.textColor = style.normal.textColor;
				style.active.textColor = style.normal.textColor;
				style.onFocused.textColor = style.normal.textColor;
				style.onNormal.textColor = style.normal.textColor;
				style.onHover.textColor = style.normal.textColor;
				style.onActive.textColor = style.normal.textColor;

				style.hover.background = style.normal.background;
				style.focused.background = style.normal.background;
				style.active.background = style.normal.background;
				style.onHover.background = style.onNormal.background;
				style.onFocused.background = style.onNormal.background;
				style.onActive.background = style.onNormal.background;
			});
		}


		public static GUIStyle CachedStyle(string key, GUIStyle baseStyle, Action<GUIStyle> customizeAction) {
			return _cachedStyles.GetOrCreateCached(key, (k) => {
				var newStyle = new GUIStyle(baseStyle);
				customizeAction.Invoke(newStyle);
				return newStyle;
			});
		}


		// PRAGMA MARK - Internal
		private static Dictionary<Texture2D, GUIStyle> _cachedTextureStyles = new Dictionary<Texture2D, GUIStyle>();
		private static Dictionary<string, GUIStyle> _cachedStyles = new Dictionary<string, GUIStyle>();

		private static GUIStyle CachedStyleWithWordWrap(string key, GUIStyle baseStyle) {
			return EditorGUIStyleUtil.CachedStyle(key, baseStyle, (style) => style.wordWrap = true);
		}
	}
}
#endif