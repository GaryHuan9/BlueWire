using BlueWire.Tiles;
using CodeHelpers.Vectors;
using UnityEngine;

namespace BlueWire.Wires
{
	public class Inverter : Tile
	{
		public Inverter(Int2 mainPosition, int rotation, Archetype archetype) : base(mainPosition, rotation, archetype) { }

		static Inverter()
		{
			Color[,] pixels = new Color[5, 5];

			for (int x = 0; x < 5; x++)
			{
				pixels[x, 1] = Color.red;
				pixels[x, 3] = Color.red;
			}

			spriteLeft = CreateSprite(pixels);

			pixels[4, 0] = Color.red;
			pixels[4, 4] = Color.red;

			spriteRight = CreateSprite(pixels);
		}

		static readonly Sprite spriteLeft;
		static readonly Sprite spriteRight;

		public override Sprite GetSprite(Int2 localPosition) => localPosition.x == 0 ? spriteLeft : spriteRight;
	}
}