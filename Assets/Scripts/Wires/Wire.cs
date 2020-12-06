using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using BlueWire.Tiles;
using BlueWire.Worlds;
using CodeHelpers.Collections;
using CodeHelpers.ObjectPooling;
using CodeHelpers.Vectors;
using UnityEngine;
using UnityEngine.Assertions;

namespace BlueWire.Wires
{
	public class Wire : Tile
	{
		public Wire(Int2 mainPosition, Archetype archetype) : base(mainPosition, 0, archetype) => RecalculateNeighbors();

		static Wire()
		{
			const int Size = 5;

			var edges = Int2.edges4;
			var sprites = new List<Sprite>();

			Color wireColor = Color.white;
			Color coreColor = Color.clear;

			begin:

			for (int i = 0; i < 1 << edges.Count; i++)
			{
				Color[,] pixels = new Color[Size, Size];

				//Add inner ring + clear pixels
				for (int x = 0; x < Size; x++)
				{
					for (int y = 0; y < Size; y++)
					{
						Int2 offset = new Int2(x, y) - (Int2)(Size / 2);
						Color color = offset == Int2.zero ? coreColor : wireColor;

						pixels[x, y] = offset.Absoluted.MaxComponent > 1 ? Color.clear : color;
					}
				}

				//Add connections
				for (int j = 0; j < edges.Count; j++)
				{
					if ((i & (1 << j)) == 0) continue;
					Int2 edge = edges[j];

					for (int k = 1; k <= Size / 2; k++)
					{
						Int2 position = edge * k + (Int2)(Size / 2);

						pixels[position.x + edge.y, position.y + edge.x] = wireColor;
						pixels[position.x - edge.y, position.y - edge.x] = wireColor;
						pixels[position.x, position.y] = coreColor;
					}
				}

				sprites.Add(CreateSprite(pixels));
			}

			if (coreColor == Color.clear)
			{
				coreColor = Color.red;
				goto begin;
			}

			wireSprites = new ReadOnlyCollection<Sprite>(sprites);
		}

		static readonly ReadOnlyCollection<Sprite> wireSprites;

		BitVector32 neighbors;

		public WireBundle Wires { get; private set; }

		public override void OnCreated()
		{
			base.OnCreated();

			var bundles = CollectionPooler<WireBundle, BitVector32>.dictionary.GetObject();

			for (int i = 0; i < Int2.edges4.Count; i++)
			{
				if (!neighbors[1 << i]) continue;

				Int2 position = mainPosition + Int2.edges4[i];
				Wire wire = WorldUtility.GetTile<Wire>(position);

				WireBundle bundle = wire.Wires;
				var directions = bundles.TryGetValue(bundle);

				wire.RecalculateNeighbors();

				directions[1 << i] = true;
				bundles[bundle] = directions;
			}

			foreach (KeyValuePair<WireBundle, BitVector32> pair in bundles)
			{
				if (Wires == null)
				{
					Wires = pair.Key;
					continue;
				}

				Wires.Merge(pair.Key);

				for (int i = 0; i < Int2.edges4.Count; i++)
				{
					if (!pair.Value[1 << i]) continue;

					Int2 position = mainPosition + Int2.edges4[i];
					Wire wire = WorldUtility.GetTile<Wire>(position);

					FloodFill(wire, current => current.Wires = Wires);
				}
			}

			if (bundles.Count == 0) Wires = new WireBundle();
			CollectionPooler<WireBundle, BitVector32>.dictionary.ReleaseObject(bundles);
		}

		public override void OnRemoved()
		{
			base.OnRemoved();

			//If the wire has no neighbor, then no bundle to reorganize
			if (neighbors.Data == 0) return;

			//Recalculate neighbors
			for (int i = 0; i < Int2.edges4.Count; i++)
			{
				if (!neighbors[1 << i]) continue;

				Int2 position = mainPosition + Int2.edges4[i];
				Wire wire = WorldUtility.GetTile<Wire>(position);

				wire.RecalculateNeighbors();
			}

			//If the wire has one neighbor, that bundle will not change
			if (neighbors.Data.IsPowerOfTwo()) return;

			//Search for all branches
			var branches = CollectionPooler<HashSet<Wire>>.list.GetObject();

			for (int i = 0; i < Int2.edges4.Count; i++)
			{
				if (!neighbors[1 << i]) continue;

				Int2 position = mainPosition + Int2.edges4[i];
				Wire wire = WorldUtility.GetTile<Wire>(position);

				if (branches.Any(hashset => hashset.Contains(wire))) continue;

				var visited = CollectionPooler<Wire>.hashSet.GetObject();
				FloodFill(wire, current => visited.Add(current));

				branches.Add(visited);
			}

			if (branches.Count == 1) return;
			Assert.AreNotEqual(branches.Count, 0);

			//Reorganize wire bundles
			for (int i = 1; i < branches.Count; i++) //NOTE: Start at one
			{
				HashSet<Wire> visited = branches[i];
				WireBundle bundle = Wires.Split
				(
					port =>
					{
						Wire wire = WorldUtility.GetTile<Wire>(port.Position + port.Direction);
						return wire != null && visited.Contains(wire);
					}
				);

				foreach (Wire wire in visited) wire.Wires = bundle;
				CollectionPooler<Wire>.hashSet.ReleaseObject(visited);
			}

			CollectionPooler<HashSet<Wire>>.list.ReleaseObject(branches);
		}

		public override Sprite GetSprite(Int2 localPosition) => wireSprites[neighbors.Data];

		void RecalculateNeighbors()
		{
			var edges = Int2.edges4;
			neighbors = new BitVector32();

			for (int i = 0; i < edges.Count; i++)
			{
				if (WorldUtility.GetTile<Wire>(mainPosition + edges[i]) == null) continue;
				neighbors[1 << i] = true;
			}
		}

		static void FloodFill(Wire wire, Action<Wire> action, Func<Wire, bool> predicate = null)
		{
			Stack<Wire> frontier = CollectionPooler<Wire>.stack.GetObject();
			HashSet<Wire> visited = CollectionPooler<Wire>.hashSet.GetObject();

			frontier.Push(wire);

			while (frontier.Count > 0)
			{
				Wire current = frontier.Pop();

				action(current);
				visited.Add(current);

				for (int i = 0; i < Int2.edges4.Count; i++)
				{
					if (!current.neighbors[1 << i]) continue;

					Int2 position = current.mainPosition + Int2.edges4[i];
					Wire neighbor = WorldUtility.GetTile<Wire>(position);

					if (predicate != null && !predicate(neighbor)) continue;
					if (!visited.Contains(neighbor)) frontier.Push(neighbor);
				}
			}

			CollectionPooler<Wire>.stack.ReleaseObject(frontier);
			CollectionPooler<Wire>.hashSet.ReleaseObject(visited);
		}
	}
}