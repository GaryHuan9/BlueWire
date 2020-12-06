using System.Collections.Generic;
using System.Collections.ObjectModel;
using BlueWire.Tiles;
using BlueWire.Worlds;
using CodeHelpers.Vectors;
using UnityEngine;

namespace BlueWire.Wires
{
	public class Wire : Tile
	{
		public Wire(Int2 mainPosition, int rotation, Archetype archetype) : base(mainPosition, rotation, archetype) => RecalculateNeighbors();

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

		int powered;
		int neighbors;

		public void RecalculateNeighbors()
		{
			var edges = Int2.edges4;
			neighbors = 0;

			for (int i = 0; i < edges.Count; i++)
			{
				if (WorldUtility.GetTile<Wire>(mainPosition + edges[i]) == null) continue;
				neighbors |= 1 << i;
			}
		}

		public override Sprite GetSprite(Int2 localPosition) => wireSprites[(powered > 0 ? 0b10000 : 0b0) | neighbors];

		public override void OnNeighborChanged()
		{
			base.OnNeighborChanged();
			RecalculateNeighbors();
		}
	}
}