using BlueWire.Wires;
using BlueWire.Worlds;
using CodeHelpers.Unity.DelayedExecute;
using CodeHelpers.Vectors;
using CodeHelpers.Vectors.Enumerables;
using UnityEngine;

namespace BlueWire.Tiles
{
	public class TileGridDisplay : MonoBehaviour
	{
		void Awake()
		{
			viewportCamera.OnReoriented += OnCameraReoriented;
		}

		[SerializeField] float lineThickness = 0.03f;
		[SerializeField] Color lineColor = Color.white;

		[SerializeField] ViewportCamera viewportCamera;
		[SerializeField] Sprite whiteSprite;

		//readonly List<Transform> lineTransforms = new List<Transform>();

		void Update()
		{
			bool add = Input.GetKey(KeyCode.Mouse0);
			bool remove = Input.GetKey(KeyCode.Mouse1);

			if (add == remove) return;

			Float2 worldPosition = viewportCamera.MousePosition;
			Int2 position = viewportCamera.ViewportToWorld(worldPosition).Floored;
			Tile tile = WorldUtility.GetTile(position);

			if (tile == null ? remove : add) return;
			tile = add ? new Wire(position) : null;

			WorldUtility.SetTile(position, tile);
			if (tile is Wire wire) wire.RecalculateNeighbors();

			foreach (Int2 offset in Int2.edges4)
			{
				WorldUtility.GetTile<Wire>(offset + position)?.RecalculateNeighbors();
			}

			OnCameraReoriented();
		}

		void OnCameraReoriented()
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

				tileObject.AddComponent<SpriteRenderer>().sprite = tile.GetSprite(Int2.zero);

				tileObject.transform.localPosition = viewportCamera.WorldToViewport(position + Float2.half).XY_;
				tileObject.transform.localScale = (Float2.one / multiplier).CreateXY(1f);
			}
		}
	}
}