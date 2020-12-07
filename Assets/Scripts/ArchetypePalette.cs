using System;
using System.Collections.Generic;
using System.Linq;
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
			AddArchetype(new RepeaterArchetype());
			AddArchetype(new ConstantArchetype());
		}

		readonly List<Archetype> archetypes = new List<Archetype>();

		public int Count => archetypes.Count;

		public event Action OnPaletteChanged;

		public Archetype GetArchetype(int index)
		{
			if (archetypes.IsIndexValid(index)) return archetypes[index];
			throw ExceptionHelper.Invalid(nameof(index), index, InvalidType.outOfBounds);
		}

		public T GetArchetype<T>() where T : Archetype => archetypes.FirstOrDefault(archetype => archetype is T) as T;

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
		public override Int2 Size => Int2.one;

		public override Tile GetNewTile(Int2 position, int rotation) => new Inverter(position, rotation, this);
	}

	public class RepeaterArchetype : Archetype
	{
		public override string Name => "Repeater";
		public override Int2 Size => Int2.one;

		public override Tile GetNewTile(Int2 position, int rotation) => new Repeater(position, rotation, this);
	}

	public class ConstantArchetype : Archetype
	{
		public override string Name => "Constant";
		public override Int2 Size => Int2.one;

		public override Tile GetNewTile(Int2 position, int rotation) => new Constant(position, rotation, this);
	}
}