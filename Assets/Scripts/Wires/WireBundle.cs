using System;
using System.Collections.Generic;
using BlueWire.Worlds;
using CodeHelpers;
using UnityEngine.Assertions;

namespace BlueWire.Wires
{
	public class WireBundle : IDisposable
	{
		public WireBundle() => WorldUtility.Active.simulator.AddWireBundle(this);

		readonly List<Port> inPorts = new List<Port>();  //The ports that takes input from this bundle
		readonly List<Port> outPorts = new List<Port>(); //The ports that outputs into this bundle

		bool disposed;
		int lastPowered = int.MaxValue;

		public bool Powered => lastPowered != int.MaxValue;

		public void JoinPort(Port port)
		{
			Assert.IsFalse(disposed);
			Assert.AreEqual(port.ConnectedBundle, this);

			List<Port> list = GetPortList(port);

			if (!list.Contains(port)) list.Add(port);
			else throw ExceptionHelper.Invalid(nameof(port), port, InvalidType.foundDuplicate);
		}

		public void DisjoinPort(Port port)
		{
			Assert.IsFalse(disposed);
			Assert.AreEqual(port.ConnectedBundle, this);

			List<Port> list = GetPortList(port);
			if (list.Remove(port)) return;

			throw ExceptionHelper.Invalid(nameof(port), port, InvalidType.notFound);
		}

		List<Port> GetPortList(Port port)
		{
			if (!disposed) return port.portType == PortType.input ? inPorts : outPorts;
			throw ExceptionHelper.Invalid(nameof(WireBundle), this, "is already disposed!");
		}

		public void Merge(WireBundle bundle)
		{
			Assert.IsFalse(disposed);

			for (int i = bundle.inPorts.Count - 1; i >= 0; i--) TransferPort(bundle.inPorts[i]);
			for (int i = bundle.outPorts.Count - 1; i >= 0; i--) TransferPort(bundle.outPorts[i]);

			bundle.Dispose();

			void TransferPort(Port port)
			{
				port.Disconnect();
				port.Connect(this);
			}
		}

		/// <summary>
		/// Divides this <see cref="WireBundle"/> into two bundles. Returns the newly split bundle.
		/// <paramref name="predicate"/> identifies the ports that should be split to the new bundle.
		/// </summary>
		public WireBundle Split(Func<Port, bool> predicate)
		{
			Assert.IsFalse(disposed);
			WireBundle bundle = new WireBundle();

			for (int i = inPorts.Count - 1; i >= 0; i--) TransferPort(inPorts[i]);
			for (int i = outPorts.Count - 1; i >= 0; i--) TransferPort(outPorts[i]);

			return bundle;

			void TransferPort(Port port)
			{
				if (!predicate(port)) return;

				port.Disconnect();
				port.Connect(bundle);
			}
		}

		public void Transmit()
		{
			Assert.IsFalse(disposed);

			//Power transmits from the ports that output to this bundle to the ports that take inputs from this bundle

			bool powered = outPorts.Count > lastPowered && outPorts[lastPowered].Value;
			int index = 0;

			for (; !powered && index < outPorts.Count; index++)
			{
				if (index == lastPowered) continue;
				powered |= outPorts[index].Value;
			}

			for (int i = 0; i < inPorts.Count; i++) inPorts[i].Value = powered;
			lastPowered = powered ? index : int.MaxValue;
		}

		public void Dispose()
		{
			if (disposed) return;

			WorldUtility.Active.simulator.RemoveWireBundle(this);
			disposed = true;
		}
	}
}