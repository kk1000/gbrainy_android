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

using gbrainy.Core.Libraries;

namespace gbrainy.Core.Services
{
	// This is a utility class to init default services
	public class DefaultServices
	{
		Dictionary <Type, IService> services;

		public DefaultServices ()
		{
			services = new Dictionary <Type, IService> ();

			// Default services
			services.Add (typeof (ITranslations), new TranslationsCatalog ());
			services.Add (typeof (IConfiguration), new MemoryConfiguration ());

#if CSHARP_STATIC
			services.Add (typeof (ICSharpCompiler), new CSharpCompilerStatic ());
#else
			services.Add (typeof (ICSharpCompiler), new CSharpCompiler ());
#endif
		}

		public void RemoveService <T> () where T : class, IService
		{
			services.Remove (typeof (T));
		}

		public void RegisterServices ()
		{
			foreach (Type t in services.Keys)
			{
				ServiceLocator.Instance.RegisterService (t, services[t]);
			}
		}
	}
}
