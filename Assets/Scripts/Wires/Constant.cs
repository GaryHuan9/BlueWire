using System;
using System.Collections.Generic;
using System.Linq;
using CodeHelpers.Vectors;
using UnityEngine;

namespace BlueWire.Wires
{
	public class Constant : Microchip
	{
		public Constant(Int2 mainPosition, int rotation, Archetype archetype) : base(mainPosition, rotation, archetype)
		{
			InPorts = Array.Empty<Port>();
			OutPorts = Int2.edges4.Select(direction => new Port(this, PortType.output, Int2.zero, direction)).ToArray();
		}

		static Constant()
		{
			Color[,] pixels = new Color[5, 5];

			foreach (Int2 position in new Int2(5, 5).Loop())
			{
				pixels[position.x, position.y] = Color.yellow;
			}

			constantSprite = CreateSprite(pixels);
		}

		static readonly Sprite constantSprite;

		public override IReadOnlyList<Port> InPorts { get; }
		public override IReadOnlyList<Port> OutPorts { get; }

		public override Sprite GetSprite(Int2 localPosition) => constantSprite;

		public override void Transmit()
		{
			foreach (Port port in OutPorts) port.Value = true;
		}
	}
}