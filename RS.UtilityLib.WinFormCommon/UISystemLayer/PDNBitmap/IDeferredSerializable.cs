﻿/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) Rick Brewster, Tom Jackson, and past contributors.            //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;

namespace RS.UtilityLib.WinFormCommon.UISystemLayer
{
    /// <summary>
    /// 延迟格式化程序接口
    /// </summary>
    public interface IDeferredSerializable
        : ISerializable
    {
        void FinishSerialization(Stream output, DeferredFormatter context);
        void FinishDeserialization(Stream input, DeferredFormatter context);
    }
    /// <summary>
    /// 延迟格式化程序
    /// </summary>
    public sealed class DeferredFormatter
    {
        private ArrayList objects = ArrayList.Synchronized(new ArrayList());
        private bool used = false;
        private object context;
        private long totalSize;
        private long totalReportedBytes;
        private bool useCompression;
        private object lockObject = new object();

        public object Context {
            get {
                return this.context;
            }
        }

        public bool UseCompression {
            get {
                return this.useCompression;
            }
        }

        public DeferredFormatter()
            : this(false, null) {
        }

        public DeferredFormatter(bool useCompression, object context) {
            this.useCompression = useCompression;
            this.context = context;
        }

        public void AddDeferredObject(IDeferredSerializable theObject, long objectByteSize) {
            if (used) {
                throw new InvalidOperationException("object already finished serialization");
            }

            this.totalSize += objectByteSize;
            objects.Add(theObject);
        }

        public event EventHandler ReportedBytesChanged;
        private void OnReportedBytesChanged() {
            if (ReportedBytesChanged != null) {
                ReportedBytesChanged(this, EventArgs.Empty);
            }
        }

        public long ReportedBytes {
            get {
                lock (lockObject) {
                    return totalReportedBytes;
                }
            }
        }

        /// <summary>
        /// Reports that bytes have been successfully been written.
        /// </summary>
        /// <param name="bytes"></param>
        public void ReportBytes(long bytes) {
            lock (lockObject) {
                totalReportedBytes += bytes;
            }

            OnReportedBytesChanged();
        }

        public void FinishSerialization(Stream output) {
            if (used) {
                throw new InvalidOperationException("object already finished deserialization or serialization");
            }

            used = true;

            foreach (IDeferredSerializable obj in this.objects) {
                obj.FinishSerialization(output, this);
            }

            this.objects = null;
        }

        public void FinishDeserialization(Stream input) {
            if (used) {
                throw new InvalidOperationException("object already finished deserialization or serialization");
            }

            used = true;

            foreach (IDeferredSerializable obj in this.objects) {
                obj.FinishDeserialization(input, this);
            }

            this.objects = null;
        }
    }
}
