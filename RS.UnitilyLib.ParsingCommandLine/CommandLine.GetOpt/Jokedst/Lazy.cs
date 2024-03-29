﻿using System;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using System.Diagnostics;

namespace Jokedst.GetOpt
{
	public enum LazyThreadSafetyMode
	{
		None,
		PublicationOnly,
		ExecutionAndPublication
	}
	[SerializableAttribute]
	[ComVisibleAttribute(false)]
	[HostProtectionAttribute(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
	[DebuggerDisplay("ThreadSafetyMode={Mode}, IsValueCreated={IsValueCreated}, IsValueFaulted={IsValueFaulted}, Value={ValueForDebugDisplay}")]
	public class Lazy<T>
	{
		T value;
		Func<T> factory;
		object monitor;
		Exception exception;
		LazyThreadSafetyMode mode;
		bool inited;

		public Lazy()
			: this(LazyThreadSafetyMode.ExecutionAndPublication) {
		}

		public Lazy(Func<T> valueFactory)
			: this(valueFactory, LazyThreadSafetyMode.ExecutionAndPublication) {
		}

		public Lazy(bool isThreadSafe)
			: this(Activator.CreateInstance<T>, isThreadSafe ? LazyThreadSafetyMode.ExecutionAndPublication : LazyThreadSafetyMode.None) {
		}

		public Lazy(Func<T> valueFactory, bool isThreadSafe)
			: this(valueFactory, isThreadSafe ? LazyThreadSafetyMode.ExecutionAndPublication : LazyThreadSafetyMode.None) {
		}

		public Lazy(LazyThreadSafetyMode mode)
			: this(Activator.CreateInstance<T>, mode) {
		}



		public Lazy(Func<T> valueFactory, LazyThreadSafetyMode mode) {
			if (valueFactory == null)
				throw new ArgumentNullException("valueFactory");
			this.factory = valueFactory;
			if (mode != LazyThreadSafetyMode.None)
				monitor = new object();
			this.mode = mode;
		}

		// Don't trigger expensive initialization
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public T Value {
			get {
				if (inited)
					return value;
				if (exception != null)
					throw exception;

				return InitValue();
			}
		}

		T InitValue() {
			Func<T> init_factory;
			T v;

			switch (mode) {
				case LazyThreadSafetyMode.None:
					init_factory = factory;
					if (init_factory == null) {
						if (exception == null)
							throw new InvalidOperationException("The initialization function tries to access Value on this instance");
						throw exception;
					}

					try {
						factory = null;
						v = init_factory();
						value = v;
						Thread.MemoryBarrier();
						inited = true;
					}
					catch (Exception ex) {
						exception = ex;
						throw;
					}
					break;

				case LazyThreadSafetyMode.PublicationOnly:
					init_factory = factory;

					//exceptions are ignored
					if (init_factory != null)
						v = init_factory();
					else
						v = default(T);

					lock (monitor) {
						if (inited)
							return value;
						value = v;
						Thread.MemoryBarrier();
						inited = true;
						factory = null;
					}
					break;

				case LazyThreadSafetyMode.ExecutionAndPublication:
					lock (monitor) {
						if (inited)
							return value;

						if (factory == null) {
							if (exception == null)
								throw new InvalidOperationException("The initialization function tries to access Value on this instance");

							throw exception;
						}

						init_factory = factory;
						try {
							factory = null;
							v = init_factory();
							value = v;
							Thread.MemoryBarrier();
							inited = true;
						}
						catch (Exception ex) {
							exception = ex;
							throw;
						}
					}
					break;

				default:
					throw new InvalidOperationException("Invalid LazyThreadSafetyMode " + mode);
			}

			return value;
		}

		public bool IsValueCreated {
			get {
				return inited;
			}
		}

		public override string ToString() {
			if (inited)
				return value.ToString();
			else
				return "Value is not created";
		}
	}
}
