using System;
using System.Collections.Generic;
using CodeHelpers;
using UnityEngine.Assertions;

namespace BlueWire.Wires
{
	public class WireBundle
	{
		readonly List<Port> inPorts = new List<Port>();
		readonly List<Port> outPorts = new List<Port>();

		bool discarded;

		public void JoinPort(Port port)
		{
			Assert.AreEqual(port.ConnectedBundle, this);
			List<Port> list = GetPortList(port);

			if (!list.Contains(port)) list.Add(port);
			else throw ExceptionHelper.Invalid(nameof(port), port, InvalidType.foundDuplicate);
		}

		public void DisjoinPort(Port port)
		{
			Assert.AreEqual(port.ConnectedBundle, this);

			List<Port> list = GetPortList(port);
			if (list.Remove(port)) return;

			throw ExceptionHelper.Invalid(nameof(port), port, InvalidType.notFound);
		}

		public void Merge(WireBundle bundle)
		{
			for (int i = bundle.inPorts.Count - 1; i >= 0; i--) TransferPort(bundle.inPorts[i]);
			for (int i = bundle.outPorts.Count - 1; i >= 0; i--) TransferPort(bundle.outPorts[i]);

			bundle.discarded = true;

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

		List<Port> GetPortList(Port port)
		{
			if (!discarded) return port.portType == PortType.input ? inPorts : outPorts;
			throw ExceptionHelper.Invalid(nameof(WireBundle), this, "is already discarded!");
		}
	}
}