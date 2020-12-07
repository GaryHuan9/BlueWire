using System.Collections.Generic;
using BlueWire.Tiles;
using BlueWire.Wires;
using CodeHelpers.Collections;
using CodeHelpers.Unity;
using UnityEngine;
using UnityEngine.Assertions;

namespace BlueWire
{
	public class Simulator
	{
		public Simulator() => CodeHelperMonoBehavior.UnityUpdateMethods += ConstantUpdate;

		readonly List<WireBundle> wireBundles = new List<WireBundle>();
		readonly List<Microchip> microchips = new List<Microchip>();

		public void AddWireBundle(WireBundle wireBundle)
		{
			Assert.IsFalse(wireBundles.Contains(wireBundle));
			wireBundles.Add(wireBundle);
		}

		public void RemoveWireBundle(WireBundle wireBundle)
		{
			bool removed = wireBundles.RemoveIgnoreOrder(wireBundle);
			Assert.IsTrue(removed);
		}

		public void AddMicrochip(Microchip microchip)
		{
			Assert.IsFalse(microchips.Contains(microchip));
			microchips.Add(microchip);
		}

		public void RemoveMicrochip(Microchip microchip)
		{
			bool removed = microchips.RemoveIgnoreOrder(microchip);
			Assert.IsTrue(removed);
		}

		void ConstantUpdate()
		{
			if (!Input.GetKeyDown(KeyCode.Space)) return;

			for (int i = 0; i < wireBundles.Count; i++) wireBundles[i].Transmit();
			for (int i = 0; i < microchips.Count; i++) microchips[i].Transmit();

			TileWorldDisplay.Instance.RedrawWorld();
		}
	}
}