// <copyright file="PortCallbackHandlerBase.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Collections.Generic;
using MMALSharp.Common.Utility;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// The base class for Output port callback handlers.
    /// </summary>
    public abstract class PortCallbackHandlerBase : ICallbackHandler, IObservable<ObservableCallback>
    {
        public bool Triggered { get; set; }

        /// <inheritdoc />
        public MMALEncoding EncodingType { get; }

        /// <inheritdoc />
        public PortBase WorkingPort { get; }

        public List<IObserver<ObservableCallback>> Observers { get; }

        /// <summary>
        /// Creates a new instance of <see cref="PortCallbackHandlerBase"/>.
        /// </summary>
        /// <param name="port">The working <see cref="PortBase"/>.</param>
        protected PortCallbackHandlerBase(PortBase port)
        {
            this.WorkingPort = port;
            this.Observers = new List<IObserver<ObservableCallback>>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="PortCallbackHandlerBase"/>.
        /// </summary>
        /// <param name="port">The working <see cref="PortBase"/>.</param>
        /// <param name="encodingType">The <see cref="MMALEncoding"/> type to restrict on.</param>
        protected PortCallbackHandlerBase(PortBase port, MMALEncoding encodingType)
        {
            this.WorkingPort = port;
            this.EncodingType = encodingType;
            this.Observers = new List<IObserver<ObservableCallback>>();
        }
        
        /// <inheritdoc />
        public virtual void Callback(MMALBufferImpl buffer)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.Debug($"In managed {this.WorkingPort.PortType.GetPortType()} callback");
            }
            
            if (this.EncodingType != null && this.WorkingPort.EncodingType != this.EncodingType)
            {
                throw new ArgumentException("Port Encoding Type not supported for this handler.");
            }
        }

        /// <inheritdoc />
        public virtual ProcessResult InputCallback(MMALBufferImpl buffer)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.Debug("In managed input callback");
            }

            if (this.EncodingType != null && this.WorkingPort.EncodingType != this.EncodingType)
            {
                throw new ArgumentException("Port Encoding Type not supported for this handler.");
            }

            MMALLog.Logger.Info($"Processing {this.WorkingPort.Handler?.TotalProcessed()}");

            return this.WorkingPort.Handler?.Process(buffer.AllocSize);
        }

        public IDisposable Subscribe(IObserver<ObservableCallback> observer)
        {
            if (!this.Observers.Contains(observer))
            {
                this.Observers.Add(observer);
            }

            return new Unsubscriber(this.Observers, observer);
        }

        public void SendCompleted()
        {
            this.Triggered = true;

            foreach (var observer in this.Observers)
            {
                observer.OnNext(new ObservableCallback { Status = ObservableCallbackStatus.EOS });
            }
        }
        
        public void SendError(Exception e)
        {
            foreach (var observer in this.Observers)
            {
                observer.OnError(e);
            }
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<ObservableCallback>> _observers;
            private IObserver<ObservableCallback> _observer;

            public Unsubscriber(List<IObserver<ObservableCallback>> observers, IObserver<ObservableCallback> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null)
                {
                    _observers.Remove(_observer);
                }
            }
        }
    }
}
