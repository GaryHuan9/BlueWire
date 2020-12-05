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
		public Wire(Int2 position) : base(position) { }

		static Wire()
		{
			const int Size = 5;
			var edges = Int2.edges4;

			var defaultColors = new Color[Size * Size];
			var sprites = new List<Sprite>();

			Color wireColor = Color.white;
			Color coreColor = Color.clear;

			begin:

			//Add inner circle
			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					Int2 position = new Int2(x, y) + (Int2)(Size / 2);
					Color color = x == 0 && y == 0 ? coreColor : wireColor;

					defaultColors[position.x * Size + position.y] = color;
				}
			}

			//Add connection lines
			for (int i = 0; i < 1 << edges.Count; i++)
			{
				Texture2D texture = new Texture2D(Size, Size, TextureFormat.RGBA32, true)
									{
										filterMode = FilterMode.Point,
										wrapMode = TextureWrapMode.Clamp
									};

				texture.SetPixels(defaultColors);

				for (int j = 0; j < edges.Count; j++)
				{
					if ((i & (1 << j)) == 0) continue;
					Int2 edge = edges[j];

					for (int k = 1; k <= Size / 2; k++)
					{
						Int2 position = edge * k + (Int2)(Size / 2);

						texture.SetPixel(position.x + edge.y, position.y + edge.x, wireColor);
						texture.SetPixel(position.x - edge.y, position.y - edge.x, wireColor);
						texture.SetPixel(position.x, position.y, coreColor);
					}
				}

				texture.Apply();
				sprites.Add(Sprite.Create(texture, new Rect(Float2.zero, (Int2)Size), Float2.half, Size));
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
				if (WorldUtility.GetTile<Wire>(position + edges[i]) == null) continue;
				neighbors |= 1 << i;
			}
		}

		public override Sprite GetSprite(Int2 localPosition) => wireSprites[(powered > 0 ? 0b10000 : 0b0) | neighbors];
	}
}