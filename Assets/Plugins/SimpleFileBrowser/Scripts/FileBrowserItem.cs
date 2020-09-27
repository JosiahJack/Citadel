﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SimpleFileBrowser
{
	public class FileBrowserItem : ListItem, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
#if UNITY_EDITOR || ( !UNITY_ANDROID && !UNITY_IOS )
		, IPointerEnterHandler, IPointerExitHandler
#endif
	{
		#region Constants
		private const float DOUBLE_CLICK_TIME = 0.5f;
		private const float TOGGLE_MULTI_SELECTION_HOLD_TIME = 0.5f;
		#endregion

		#region Variables
		protected FileBrowser fileBrowser;

#pragma warning disable 0649
		[SerializeField]
		private Image background;

		[SerializeField]
		private Image icon;

		[SerializeField]
		private Image multiSelectionToggle;

		[SerializeField]
		private Text nameText;
#pragma warning restore 0649

		private bool isSelected;

		private float pressTime = Mathf.Infinity;
		private float prevClickTime;
		#endregion

		#region Properties
		private RectTransform m_transform;
		public RectTransform TransformComponent
		{
			get
			{
				if( m_transform == null )
					m_transform = (RectTransform) transform;

				return m_transform;
			}
		}

		public string Name { get { return nameText.text; } }
		public bool IsDirectory { get; private set; }
		#endregion

		#region Initialization Functions
		public void SetFileBrowser( FileBrowser fileBrowser )
		{
			this.fileBrowser = fileBrowser;
		}

		public void SetFile( Sprite icon, string name, bool isDirectory )
		{
			this.icon.sprite = icon;
			nameText.text = name;

			IsDirectory = isDirectory;
		}
		#endregion

		#region Messages
		private void Update()
		{
			if( fileBrowser.AllowMultiSelection && Time.realtimeSinceStartup - pressTime >= TOGGLE_MULTI_SELECTION_HOLD_TIME )
			{
				// Item is held for a while
				pressTime = Mathf.Infinity;
				fileBrowser.MultiSelectionToggleSelectionMode = true;

				if( !isSelected )
					fileBrowser.OnItemSelected( this, false );
			}
		}
		#endregion

		#region Pointer Events
		public void OnPointerClick( PointerEventData eventData )
		{
			if( Time.realtimeSinceStartup - prevClickTime < DOUBLE_CLICK_TIME )
			{
				prevClickTime = 0f;
				fileBrowser.OnItemSelected( this, true );
			}
			else
			{
				prevClickTime = Time.realtimeSinceStartup;
				fileBrowser.OnItemSelected( this, false );
			}
		}

		public void OnPointerDown( PointerEventData eventData )
		{
			pressTime = Time.realtimeSinceStartup;
		}

		public void OnPointerUp( PointerEventData eventData )
		{
			if( pressTime == Mathf.Infinity )
			{
				// We have activated MultiSelectionToggleSelectionMode with this press, processing the click would result in
				// deselecting this item since its selected state would be toggled
				eventData.eligibleForClick = false;
			}
			else
				pressTime = Mathf.Infinity;
		}

#if UNITY_EDITOR || ( !UNITY_ANDROID && !UNITY_IOS )
		public void OnPointerEnter( PointerEventData eventData )
		{
			if( !isSelected )
				background.color = fileBrowser.hoveredFileColor;
		}
#endif

#if UNITY_EDITOR || ( !UNITY_ANDROID && !UNITY_IOS )
		public void OnPointerExit( PointerEventData eventData )
		{
			if( !isSelected )
				background.color = fileBrowser.normalFileColor;
		}
#endif
		#endregion

		#region Other Events
		public void SetSelected( bool isSelected )
		{
			this.isSelected = isSelected;
			background.color = isSelected ? fileBrowser.selectedFileColor : fileBrowser.normalFileColor;

			if( fileBrowser.MultiSelectionToggleSelectionMode )
			{
				if( !multiSelectionToggle.gameObject.activeSelf )
				{
					multiSelectionToggle.gameObject.SetActive( true );

					Vector2 shiftAmount = new Vector2( multiSelectionToggle.rectTransform.sizeDelta.x, 0f );
					icon.rectTransform.anchoredPosition += shiftAmount;
					nameText.rectTransform.anchoredPosition += shiftAmount;
				}

				multiSelectionToggle.sprite = isSelected ? fileBrowser.multiSelectionToggleOnIcon : fileBrowser.multiSelectionToggleOffIcon;
			}
			else if( multiSelectionToggle.gameObject.activeSelf )
			{
				multiSelectionToggle.gameObject.SetActive( false );

				Vector2 shiftAmount = new Vector2( -multiSelectionToggle.rectTransform.sizeDelta.x, 0f );
				icon.rectTransform.anchoredPosition += shiftAmount;
				nameText.rectTransform.anchoredPosition += shiftAmount;

				// Clicking a file shortly after disabling MultiSelectionToggleSelectionMode does nothing, this workaround fixes that issue
				prevClickTime = 0f;
			}
		}

		public void SetHidden( bool isHidden )
		{
			Color c = icon.color;
			c.a = isHidden ? 0.5f : 1f;
			icon.color = c;

			c = nameText.color;
			c.a = isHidden ? 0.55f : 1f;
			nameText.color = c;
		}
		#endregion
	}
}