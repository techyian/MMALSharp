using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Native;

namespace MMALSharp
{
    /// <summary>
    /// Used for Standalone use of MMALSharp without camera.
    /// </summary>
    public class MMALStandalone
    {
        /// <summary>
        /// Bootstraps MMAL for standalone use.
        /// </summary>
        public static MMALStandalone Instance => Lazy.Value;
        
        private static readonly Lazy<MMALStandalone> Lazy = new Lazy<MMALStandalone>(() => new MMALStandalone());
        
        private MMALStandalone()
        {
            BcmHost.bcm_host_init();

            MMALLog.ConfigureLogger();
        }

        /// <summary>
        /// Helper method to begin processing user provided image/video data. Starts the initial component control, input and output ports and awaits until processing is complete.
        /// Cleans up resources upon finish.
        /// </summary>
        /// <param name="initialComponent">The first component in your pipeline.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for a task to complete.</param>
        /// <returns>The awaitable Task.</returns>
        public async Task ProcessAsync(IDownstreamComponent initialComponent,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var handlerComponents = this.PopulateProcessingList(initialComponent);
            
            initialComponent.Control.Start();
            initialComponent.Inputs[0].Start();
            
            var tasks = new List<Task>();

            tasks.Add(initialComponent.Inputs[0].Trigger.Task);

            // Enable all connections associated with these components
            foreach (var component in handlerComponents)
            {
                component.EnableConnections();
                component.ForceStopProcessing = false;

                foreach (var port in component.ProcessingPorts.Values)
                {
                    if (port.ConnectedReference == null)
                    {
                        port.Start();
                        tasks.Add(port.Trigger.Task);
                    }
                }
            }

            if (handlerComponents.Count == 0)
            {
                initialComponent.Outputs[0].Start();
                tasks.Add(initialComponent.Outputs[0].Trigger.Task);
            }

            // Get buffer from input port pool                
            var inputBuffer = initialComponent.Inputs[0].BufferPool.Queue.GetBuffer();

            if (inputBuffer.CheckState())
            {
                initialComponent.Inputs[0].SendBuffer(inputBuffer);
            }
            
            await Task.WhenAll(tasks).ConfigureAwait(false);

            // Cleanup each connected downstream component.
            foreach (var component in handlerComponents)
            {
                foreach (var port in component.ProcessingPorts.Values)
                {
                    if (port.ConnectedReference == null)
                    {
                        port.DisablePort();
                    }
                }

                component.CleanPortPools();
                component.DisableConnections();
            }

            if (handlerComponents.Count == 0)
            {
                initialComponent.Outputs[0].DisablePort();
                initialComponent.CleanPortPools();
            }
        }

        /// <summary>
        /// Prints the currently configured component pipeline to the console window.
        /// </summary>
        /// <param name="initialComponent">The first component in your pipeline.</param>
        public void PrintPipeline(IDownstreamComponent initialComponent)
        {
            MMALLog.Logger.Info("Current pipeline:");
            MMALLog.Logger.Info(string.Empty);

            foreach (var component in this.PopulateProcessingList(initialComponent))
            {
                component.PrintComponent();
            }
        }

        /// <summary>
        /// Cleans up any unmanaged resources. It is intended for this method to be run when no more activity is to be done with MMAL.
        /// </summary>
        public void Cleanup()
        {
            MMALLog.Logger.Debug("Destroying final components");

            var tempList = new List<MMALDownstreamComponent>(MMALBootstrapper.DownstreamComponents);

            tempList.ForEach(c => c.Dispose());

            BcmHost.bcm_host_deinit();
        }
        
        private List<IDownstreamComponent> PopulateProcessingList(IDownstreamComponent initialComponent)
        {
            var list = new List<IDownstreamComponent>();
            
            if (initialComponent != null)
            {
                this.FindComponents(initialComponent, list);
            }
            
            return list;
        }

        private void FindComponents(IDownstreamComponent downstream, List<IDownstreamComponent> list)
        {
            if (downstream.Outputs.Count == 0)
            {
                return;
            }

            if (downstream.Outputs.Count == 1 && downstream.Outputs[0].ConnectedReference == null)
            {
                list.Add(downstream);
                return;
            }

            if (downstream.GetType().BaseType == typeof(MMALDownstreamHandlerComponent))
            {
                list.Add((MMALDownstreamHandlerComponent)downstream);
            }

            foreach (var output in downstream.Outputs)
            {
                if (output.ConnectedReference != null)
                {
                    this.FindComponents(output.ConnectedReference.DownstreamComponent, list);
                }
            }
        }
    }
}