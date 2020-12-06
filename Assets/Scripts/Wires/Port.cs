using System;
using CodeHelpers.Unity.Debugs;
using CodeHelpers.Vectors;
using UnityEngine.Assertions;

namespace BlueWire.Wires
{
	public class Port
	{
		public Port(Microchip microchip, PortType portType, Int2 localPosition, Int2 localDirection)
		{
			this.microchip = microchip;
			this.portType = portType;

			this.localPosition = localPosition;
			this.localDirection = localDirection;

			Assert.IsTrue(localDirection.Absoluted.MinComponent == 0);
			Assert.IsTrue(localDirection.Absoluted.MaxComponent == 1);
		}

		public readonly Microchip microchip;
		public readonly PortType portType;

		public readonly Int2 localPosition;
		public readonly Int2 localDirection;

		public WireBundle ConnectedBundle { get; private set; }
		public bool Connected => ConnectedBundle != null;

		public Int2 Position => microchip.TransformLocalToWorld(localPosition);
		public Int2 Direction => localDirection.Rotate(-microchip.rotation).Rounded;

		public void Connect(WireBundle bundle)
		{
			if (Connected) throw new Exception($"Disconnect the existing bundle '{ConnectedBundle}' first!");

			ConnectedBundle = bundle;
			bundle.JoinPort(this);

			DebugHelperUnity.Log($"Connected to {bundle}");
		}

		public void Disconnect()
		{
			if (!Connected) throw new Exception("No connection to sever!");
			DebugHelperUnity.Log($"Disconnected from {ConnectedBundle}");

			ConnectedBundle.DisjoinPort(this);
			ConnectedBundle = null;
		}

		public override string ToString() => $"{nameof(Port)} ({portType}) of '{microchip}' ({nameof(Position)}: {Position}; {nameof(Direction)}: {Direction})";
	}

	public enum PortType : byte
	{
		input,
		output
	}
}