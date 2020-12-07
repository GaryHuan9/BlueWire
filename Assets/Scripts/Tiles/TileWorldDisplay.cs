using BlueWire.Wires;
using BlueWire.Worlds;
using CodeHelpers.Unity;
using CodeHelpers.Unity.DelayedExecute;
using CodeHelpers.Vectors;
using CodeHelpers.Vectors.Enumerables;
using UnityEngine;

namespace BlueWire.Tiles
{
	public class TileWorldDisplay : MonoBehaviour
	{
		void Awake()
		{
			if (SingletonHelper.ThrowExceptionDestroyGameObject(ref _instance, this) != this) return;
			viewportCamera.OnReoriented += RedrawWorld;
		}

		[SerializeField] float lineThickness = 0.03f;
		[SerializeField] Color lineColor = Color.white;

		[SerializeField] ViewportCamera viewportCamera;
		[SerializeField] Sprite whiteSprite;

		static TileWorldDisplay _instance;
		public static TileWorldDisplay Instance => _instance;

		//readonly List<Transform> lineTransforms = new List<Transform>();

		public void RedrawWorld()
		{
			transform.DestroyAllChildren();

			Float2 worldSize = viewportCamera.WorldSize;
			float multiplier = viewportCamera.Elevation;

			Int2 min = viewportCamera.ViewportToWorld(worldSize / -2f).Floored;
			Int2 max = viewportCamera.ViewportToWorld(worldSize / 2f).Ceiled;

#if !GRID
			for (int x = min.x; x <= max.x; x++)
			{
				GameObject line = new GameObject("Line");
				line.transform.SetParent(transform, false);

				var graphics = line.AddComponent<SpriteRenderer>();
				graphics.sprite = whiteSprite;
				graphics.color = lineColor;

				line.transform.localPosition = Float3.right * viewportCamera.WorldToViewport(Float2.right * x).x;
				line.transform.localScale = new Float3(lineThickness / multiplier, worldSize.y, 1f);
			}

			for (int y = min.y; y <= max.y; y++)
			{
				GameObject line = new GameObject("Line");
				line.transform.SetParent(transform, false);

				var graphics = line.AddComponent<SpriteRenderer>();
				graphics.sprite = whiteSprite;
				graphics.color = lineColor;

				line.transform.localPosition = Float3.up * viewportCamera.WorldToViewport(Float2.up * y).y;
				line.transform.localScale = new Float3(worldSize.x, lineThickness / multiplier, 1f);
			}
#endif

			foreach (Int2 position in new EnumerableSpace2D(min, max))
			{
				Tile tile = WorldUtility.GetTile(position);
				if (tile == null) continue;

				GameObject tileObject = new GameObject("Tile");
				tileObject.transform.SetParent(transform, false);

				tileObject.AddComponent<SpriteRenderer>().sprite = tile.GetSprite(tile.TransformWorldToLocal(position));

				if (tile is Wire wire && false)
				{
					int hashCode = wire.Wires.GetHashCode();
					Color color = new Color32((byte)((hashCode >> 16) & 0xFF), (byte)((hashCode >> 8) & 0xFF), (byte)(hashCode & 0xFF), byte.MaxValue);
					tileObject.GetComponent<SpriteRenderer>().color = color;
				}

				tileObject.transform.localPosition = viewportCamera.WorldToViewport(position + Float2.half).XY_;
				tileObject.transform.localEulerAngles = Float3.backward * tile.rotation;
				tileObject.transform.localScale = (Float2.one / multiplier).CreateXY(1f);
			}
		}
	}
}