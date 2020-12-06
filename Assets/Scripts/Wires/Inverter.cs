using System.Collections.Generic;
using CodeHelpers.Vectors;
using UnityEngine;

namespace BlueWire.Wires
{
	public class Inverter : Microchip
	{
		public Inverter(Int2 mainPosition, int rotation, Archetype archetype) : base(mainPosition, rotation, archetype)
		{
			InPorts = new[] {new Port(this, PortType.input, Int2.zero, Int2.left)};
			OutPorts = new[] {new Port(this, PortType.output, Int2.right, Int2.right)};
		}

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

		public override IReadOnlyList<Port> InPorts { get; }
		public override IReadOnlyList<Port> OutPorts { get; }

		public override Sprite GetSprite(Int2 localPosition) => localPosition.x == 0 ? spriteLeft : spriteRight;
	}
}