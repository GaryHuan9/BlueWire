using System.Collections.Generic;
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

		void OnCameraReoriented()
		{
			transform.DestroyAllChildren();

			Float2 worldSize = viewportCamera.WorldSize;
			Float2 position = viewportCamera.Position;

			float multiplier = viewportCamera.Elevation;

			Int2 min = (position - worldSize * multiplier / 2f).Floored;
			Int2 max = (position + worldSize * multiplier / 2f).Ceiled;

			for (int x = min.x; x <= max.x; x++)
			{
				GameObject line = new GameObject("Line");
				line.transform.SetParent(transform, false);

				var graphics = line.AddComponent<SpriteRenderer>();
				graphics.sprite = whiteSprite;
				graphics.color = lineColor;

				line.transform.localPosition = Float3.right * (x - position.x) / multiplier;
				line.transform.localScale = new Float3(lineThickness / multiplier, worldSize.y, 1f);
			}

			for (int y = min.y; y <= max.y; y++)
			{
				GameObject line = new GameObject("Line");
				line.transform.SetParent(transform, false);

				var graphics = line.AddComponent<SpriteRenderer>();
				graphics.sprite = whiteSprite;
				graphics.color = lineColor;

				line.transform.localPosition = Float3.up * (y - position.y) / multiplier;
				line.transform.localScale = new Float3(worldSize.x, lineThickness / multiplier, 1f);
			}
		}
	}
}