using System.Collections.Generic;
using BlueWire.Tiles;
using BlueWire.Worlds;
using CodeHelpers.Vectors;

namespace BlueWire.Wires
{
	public abstract class Microchip : Tile
	{
		protected Microchip(Int2 mainPosition, int rotation, Archetype archetype) : base(mainPosition, rotation, archetype) { }

		public abstract IReadOnlyList<Port> InPorts { get; }
		public abstract IReadOnlyList<Port> OutPorts { get; }

		public override void OnCreated()
		{
			base.OnCreated();
			RefreshPorts();
		}

		public override void OnRemoved()
		{
			base.OnRemoved();

			foreach (Port port in InPorts) DisconnectPort(port);
			foreach (Port port in OutPorts) DisconnectPort(port);

			void DisconnectPort(Port port)
			{
				if (port.Connected) port.Disconnect();
			}
		}

		public override void OnNeighborChanged(Int2 neighbor)
		{
			base.OnNeighborChanged(neighbor);
			RefreshPorts(neighbor);
		}

		void RefreshPorts(Int2? position = null)
		{
			foreach (Port port in InPorts) RefreshPort(port);
			foreach (Port port in OutPorts) RefreshPort(port);

			void RefreshPort(Port port)
			{
				Int2 target = port.Position + port.Direction;
				if (position != null && target != position) return;

				Wire wire = WorldUtility.GetTile<Wire>(target);
				if (port.ConnectedBundle == wire?.Wires) return;

				if (wire == null) port.Disconnect();
				else port.Connect(wire.Wires);
			}
		}
	}
}