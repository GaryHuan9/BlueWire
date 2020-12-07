using System.Linq;
using BlueWire.Tiles;
using BlueWire.UI;
using BlueWire.Worlds;
using CodeHelpers.Vectors;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlueWire
{
	public class PlacementController : MonoBehaviour
	{
		[SerializeField] ArchetypePaletteDisplay paletteDisplay;
		[SerializeField] TileWorldDisplay tileWorldDisplay;
		[SerializeField] ViewportCamera viewportCamera;

		int rotation;

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.R)) rotation = (rotation + 90).ToUnsignedAngle();
			if (Input.GetKeyDown(KeyCode.Mouse2)) paletteDisplay.SelectedArchetype = paletteDisplay.Palette.GetArchetype<DemolitionArchetype>();

			if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse1)) paletteDisplay.SelectedArchetype = null;
			if (!Input.GetKey(KeyCode.Mouse0) || EventSystem.current.IsPointerOverGameObject()) return;

			var archetype = paletteDisplay.SelectedArchetype;
			if (archetype == null) return;

			Float2 worldPosition = viewportCamera.ViewportToWorld(viewportCamera.MousePosition);

			if (archetype is DemolitionArchetype)
			{
				Tile tile = WorldUtility.GetTile(worldPosition.Floored);
				if (tile == null) return;

				foreach (Int2 position in tile.Cover) WorldUtility.SetTile(position, null);

				tile.OnRemoved();

				foreach (Int2 position in tile.Cover) NotifyNeighbors(tile, position);
			}
			else
			{
				Int2 min = (worldPosition - (Tile.GetSize(archetype, rotation) - Int2.one) / 2f).Floored;
				if (Tile.GetCover(min, archetype, rotation).Any(position => WorldUtility.GetTile(position) != null)) return;

				Tile tile = archetype.GetNewTile(min, rotation);

				foreach (Int2 position in tile.Cover) WorldUtility.SetTile(position, tile);

				tile.OnCreated();

				foreach (Int2 position in tile.Cover) NotifyNeighbors(tile, position);
			}

			tileWorldDisplay.RedrawWorld();
		}

		static void NotifyNeighbors(Tile tile, Int2 position)
		{
			Int2 offset = position - tile.mainPosition;
			Int2 oneLess = tile.Size - Int2.one;

			if (offset.x == 0) WorldUtility.GetTile(position + Int2.left)?.OnNeighborChanged(position);
			if (offset.y == 0) WorldUtility.GetTile(position + Int2.down)?.OnNeighborChanged(position);

			if (offset.x == oneLess.x) WorldUtility.GetTile(position + Int2.right)?.OnNeighborChanged(position);
			if (offset.y == oneLess.y) WorldUtility.GetTile(position + Int2.up)?.OnNeighborChanged(position);
		}
	}
}