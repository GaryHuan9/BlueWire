using System;
using System.Collections.Generic;
using BlueWire.Tiles;
using BlueWire.Wires;
using CodeHelpers;
using CodeHelpers.Collections;
using CodeHelpers.Vectors;

namespace BlueWire
{
	public class ArchetypePalette
	{
		public ArchetypePalette()
		{
			AddArchetype(new DemolitionArchetype());
			AddArchetype(new WireArchetype());
			AddArchetype(new InverterArchetype());
		}

		readonly List<Archetype> archetypes = new List<Archetype>();

		public int Count => archetypes.Count;

		public event Action OnPaletteChanged;

		public Archetype GetArchetype(int index)
		{
			if (archetypes.IsIndexValid(index)) return archetypes[index];
			throw ExceptionHelper.Invalid(nameof(index), index, InvalidType.outOfBounds);
		}

		public void AddArchetype(Archetype archetype)
		{
			if (archetype == null || archetypes.Contains(archetype)) return;

			archetypes.Add(archetype);
			OnPaletteChanged?.Invoke();
		}
	}

	public abstract class Archetype
	{
		public abstract string Name { get; }
		public abstract Int2 Size { get; }

		public abstract Tile GetNewTile(Int2 position, int rotation);
	}

	public class DemolitionArchetype : Archetype
	{
		public override string Name => "Demolition";
		public override Int2 Size => Int2.one;

		public override Tile GetNewTile(Int2 position, int rotation) => throw new NotSupportedException();
	}

	public class WireArchetype : Archetype
	{
		public override string Name => "Wire";
		public override Int2 Size => Int2.one;

		public override Tile GetNewTile(Int2 position, int rotation) => new Wire(position, this);
	}

	public class InverterArchetype : Archetype
	{
		public override string Name => "NOT";
		public override Int2 Size => new Int2(2, 1);

		public override Tile GetNewTile(Int2 position, int rotation) => new Inverter(position, rotation, this);
	}
}