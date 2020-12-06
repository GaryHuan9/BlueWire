using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlueWire.UI
{
	public class ArchetypeButton : MonoBehaviour
	{
		void Awake()
		{
			button = GetComponent<Button>();
			button.onClick.AddListener(OnPressed);

			label = GetComponentInChildren<TextMeshProUGUI>();
			fitter = GetComponentInChildren<AspectRatioFitter>();
		}

		Button button;
		TextMeshProUGUI label;
		AspectRatioFitter fitter;

		Archetype _archetype;
		bool _selected;

		public Archetype Archetype
		{
			get => _archetype;
			set
			{
				if (_archetype == value) return;
				_archetype = value;

				RedrawUI();
			}
		}

		public bool Selected
		{
			get => _selected;
			set
			{
				if (_selected == value) return;
				_selected = value;

				RedrawUI();
			}
		}

		public event Action<ArchetypeButton> OnSelection;

		void RedrawUI()
		{
			button.interactable = !Selected;
			label.text = Archetype.Name;

			fitter.aspectRatio = (float)Archetype.Size.x / Archetype.Size.y;
		}

		void OnPressed() => OnSelection?.Invoke(this);
	}
}