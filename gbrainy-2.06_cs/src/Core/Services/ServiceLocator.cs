/*
 * Copyright (C) 2011 Jordi Mas i Hernàndez <jmas@softcatala.org>
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as
 * published by the Free Software Foundation; either version 2 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public
 * License along with this program; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */

using System;
using System.Collections.Generic;

namespace gbrainy.Core.Services
{
	public class ServiceLocator
	{
		Dictionary <Type, IService> services;
		static ServiceLocator instance = new ServiceLocator ();
		static readonly object sync = new object ();

		public ServiceLocator ()
		{
			services = new Dictionary <Type, IService> ();
		}

		public static ServiceLocator Instance {
			get {
				return instance;
			}
		}

		public void RegisterService (Type t, IService service)
		{
			lock (sync)
			{
				if (services.ContainsKey (t) == false)
				{
					services.Add (t, service);
				}
				else
				{
					services[t] = service;
				}
			}	
		}		

		public void RegisterService <T> (T service) where T : class, IService
		{
			Type t = typeof (T);

			lock (sync)
			{
				if (services.ContainsKey (t) == false)
				{
					services.Add (t, service);
				}
				else
				{
					services[t] = service;
				}
			}
		}

		public T GetService <T> () where T: IService
		{
			lock (sync)
			{
				try
				{
					return (T) services [typeof (T)];
				}

				catch (KeyNotFoundException)
				{
					string service = typeof (T).ToString ();

					throw new InvalidOperationException (
						String.Format ("ServiceLocator. The requested service {0} is not registered", service));
				}
			}
		}
	}
}
