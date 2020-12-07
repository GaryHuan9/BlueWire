using System.Collections.Generic;
using BlueWire.Tiles;
using CodeHelpers.Collections;
using CodeHelpers.Vectors;

namespace BlueWire.Worlds
{
	public class TileWorld
	{
		public const int ChunkSize = 32;

		readonly Dictionary<Int2, TileChunk> chunks = new Dictionary<Int2, TileChunk>();
		public readonly Simulator simulator = new Simulator();

		public TileChunk GetChunk(Int2 position) => chunks.TryGetValue(position);
		public void AddChunk(Int2 position, TileChunk chunk) => chunks.Add(position, chunk);
	}
}