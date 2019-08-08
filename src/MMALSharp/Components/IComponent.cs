using System.Collections.Generic;
using MMALSharp.Ports;
using MMALSharp.Ports.Controls;
using MMALSharp.Ports.Inputs;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Components
{
    public interface IComponent : IMMALObject
    {
        IControlPort Control { get; }
        List<IInputPort> Inputs { get; }
        List<IOutputPort> Outputs { get; }
        List<IPort> Clocks { get; }
        List<IPort> Ports { get; }

        string Name { get; }
        bool Enabled { get; }
        bool ForceStopProcessing { get; set; }

        void EnableConnections();

        void DisableConnections();

        void PrintComponent();

        void AcquireComponent();

        void ReleaseComponent();

        void DestroyComponent();

        void EnableComponent();

        void DisableComponent();

        void CleanPortPools();
    }
}
