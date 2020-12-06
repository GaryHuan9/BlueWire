using CodeHelpers.Vectors;
using CodeHelpers.Vectors.Enumerables;
using UnityEngine;

namespace BlueWire.Tiles
{
	public abstract class Tile
	{
		protected Tile(Int2 mainPosition, int rotation, Archetype archetype)
		{
			this.mainPosition = mainPosition;
			this.rotation = rotation.ToUnsignedAngle();
			this.archetype = archetype;
		}

		public readonly Int2 mainPosition;
		public readonly int rotation;
		public readonly Archetype archetype;

		/// <summary>
		/// Returns whether this tile is rotated;
		/// </summary>
		public bool Rotated => GetRotated(rotation);

		/// <summary>
		/// The rotated size of this entire tile.
		/// </summary>
		public Int2 Size => GetSize(archetype, Rotated);

		/// <summary>
		/// Returns an enumerable that can be looped through every position this tile covers.
		/// </summary>
		public EnumerableSpace2D Cover => GetCover(mainPosition, archetype, Rotated);

		/// <summary>
		/// Invoked once when <see cref="Tile"/> was just created. Invoked after the constructor
		/// when all of the tile pieces have been assigned to the world.
		/// </summary>
		public virtual void OnCreated() { }

		/// <summary>
		/// Invoked once when <see cref="Tile"/> was just removed. Invoked after removal
		/// when all of the tile pieces have been removed from the world.
		/// </summary>
		public virtual void OnRemoved() { }

		public abstract Sprite GetSprite(Int2 localPosition);

		/// <summary>
		/// Transforms a point in local tile space to world space.
		/// </summary>
		public Int2 TransformLocalToWorld(Int2 localPosition)
		{
			Float2 localCenter = (archetype.Size - Float2.one) / 2f; //Not rotated center

			Float2 center = (Size - Float2.one) / 2f;    //Rotated center of the cube
			Float2 offset = localPosition - localCenter; //The position compared to center

			return (center + offset.Rotate(-rotation)).Rounded + mainPosition;
		}

		/// <summary>
		/// Transforms a point in world space to local tile space.
		/// </summary>
		public Int2 TransformWorldToLocal(Int2 worldPosition)
		{
			worldPosition -= mainPosition;
			Float2 localCenter = (archetype.Size - Float2.one) / 2f; //Not rotated center

			Float2 center = (Size - Float2.one) / 2f;                  //Rotated center of the cube
			Float2 offset = (worldPosition - center).Rotate(rotation); //The position compared to center

			return (offset + localCenter).Rounded;
		}

		/// <summary>
		/// Invoked when the immediate neighbors of this tile changed.
		/// </summary>
		public virtual void OnNeighborChanged(Int2 neighbor) { }

		protected static Sprite CreateSprite(Color[,] colors)
		{
			Int2 size = new Int2(colors.GetLength(0), colors.GetLength(1));
			Texture2D texture = new Texture2D(size.x, size.y, TextureFormat.RGBA32, true)
								{
									filterMode = FilterMode.Point,
									wrapMode = TextureWrapMode.Clamp
								};

			Color[] pixels = new Color[colors.Length];

			foreach (Int2 position in size.Loop())
			{
				pixels[position.y * size.x + position.x] = colors[position.x, position.y];
			}

			texture.SetPixels(pixels);
			texture.Apply();

			return Sprite.Create(texture, new Rect(Float2.zero, size), Float2.half, size.MinComponent);
		}

		public static bool GetRotated(int rotation) => rotation == 90 || rotation == 270;

		public static Int2 GetSize(Archetype archetype, int rotation) => GetSize(archetype, GetRotated(rotation));
		public static Int2 GetSize(Archetype archetype, bool rotated) => rotated ? archetype.Size.YX : archetype.Size;

		public static EnumerableSpace2D GetCover(Int2 mainPosition, Archetype archetype, int rotation) => GetCover(mainPosition, archetype, GetRotated(rotation));
		public static EnumerableSpace2D GetCover(Int2 mainPosition, Archetype archetype, bool rotated) => new EnumerableSpace2D(mainPosition, mainPosition + GetSize(archetype, rotated) - Int2.one);

		public override string ToString() => $"{GetType()} at {mainPosition} with rotation {rotation}";
	}
}