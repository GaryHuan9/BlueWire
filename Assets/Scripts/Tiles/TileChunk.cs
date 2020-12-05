using BlueWire.Worlds;
using CodeHelpers.Vectors;

namespace BlueWire.Tiles
{
	public class TileChunk
	{
		public TileChunk() => tiles = new Tile[TileWorld.ChunkSize * TileWorld.ChunkSize];

		readonly Tile[] tiles;

		public Tile GetTile(Int2 position) => tiles[PositionToIndex(position)];

		public Tile SetTile(Int2 position, Tile tile)
		{
			ref Tile location = ref tiles[PositionToIndex(position)];
			Tile original = location;

			location = tile;
			return original;
		}

		static int PositionToIndex(Int2 position) => position.x * TileWorld.ChunkSize + position.y;
	}
}