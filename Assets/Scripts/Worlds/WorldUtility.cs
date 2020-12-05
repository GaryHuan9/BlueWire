using BlueWire.Tiles;
using CodeHelpers.Vectors;

namespace BlueWire.Worlds
{
	public static class WorldUtility
	{
		public static TileWorld Active { get; private set; } = new TileWorld();

		public static T GetTile<T>(Int2 position) where T : Tile => GetTile(position) as T;

		public static Tile GetTile(Int2 position) => Active.GetChunk(WorldToChunkSpace(position))?.GetTile(WorldToChunkLocal(position));

		public static Tile SetTile(Int2 position, Tile tile)
		{
			Int2 chunkPosition = WorldToChunkSpace(position);
			TileChunk chunk = Active.GetChunk(chunkPosition);

			if (chunk == null) Active.AddChunk(chunkPosition, chunk = new TileChunk());
			return chunk.SetTile(WorldToChunkLocal(position), tile);
		}

		static Int2 WorldToChunkLocal(Int2 position) => position.Repeat(TileWorld.ChunkSize);
		static Int2 WorldToChunkSpace(Int2 position) => position.FlooredDivide(TileWorld.ChunkSize);
	}
}