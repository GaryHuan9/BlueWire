using CodeHelpers.Vectors;
using UnityEngine;

namespace BlueWire.Tiles
{
	public abstract class Tile
	{
		protected Tile(Int2 position) => this.position = position;

		public readonly Int2 position;

		public abstract Sprite GetSprite(Int2 localPosition);
	}
}