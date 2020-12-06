using System;
using System.Collections.Generic;
using System.Linq;
using CodeHelpers.Unity.DelayedExecute;
using UnityEngine;

namespace BlueWire.UI
{
	public class ArchetypePaletteDisplay : MonoBehaviour
	{
		[SerializeField] Transform buttonsParent;
		[SerializeField] GameObject archetypeButton;

		readonly List<ArchetypeButton> buttons = new List<ArchetypeButton>();

		ArchetypePalette _palette;
		Archetype _selectedArchetype;

		public ArchetypePalette Palette
		{
			get => _palette;
			set
			{
				if (_palette == value) return;

				var original = _palette;
				_palette = value;

				if (original != null) original.OnPaletteChanged -= ReorganizeButtons;
				if (value != null) value.OnPaletteChanged += ReorganizeButtons;

				ReorganizeButtons();
			}
		}

		public Archetype SelectedArchetype
		{
			get => _selectedArchetype;
			set
			{
				if (_selectedArchetype == value) return;

				var original = _selectedArchetype;
				_selectedArchetype = value;

				if (original != null) SetFirst(original, false);
				if (value != null) SetFirst(value, true);

				void SetFirst(Archetype archetype, bool selected)
				{
					var target = buttons.FirstOrDefault(button => button.Archetype == archetype);
					if (target != null) target.Selected = selected;
				}
			}
		}

		void Start()
		{
			Palette = new ArchetypePalette();
		}

		void ReorganizeButtons()
		{
			int count = Palette?.Count ?? 0;

			while (buttons.Count < count)
			{
				var button = Instantiate(archetypeButton, buttonsParent).GetComponent<ArchetypeButton>();

				buttons.Add(button);
				button.OnSelection += OnSelection;
			}

			while (buttons.Count > count)
			{
				DelayedDestroy.Destroy(buttons[buttons.Count - 1]);
				buttons.RemoveAt(buttons.Count - 1);
			}

			for (int i = 0; i < count; i++)
			{
				ArchetypeButton button = buttons[i];

				button.Archetype = Palette?.GetArchetype(i);
				button.Selected = false;
			}

			SelectedArchetype = null;
		}

		void OnSelection(ArchetypeButton button) => SelectedArchetype = button.Archetype;
	}
}