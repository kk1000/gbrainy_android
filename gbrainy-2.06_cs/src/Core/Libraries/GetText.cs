/*
 * Copyright (C) 2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using Mono.Unix;
using System.Runtime.InteropServices;

namespace gbrainy.Core.Libraries
{
	static public class GetText
	{
		[DllImport("intl")]
		static extern IntPtr gettext (IntPtr instring);
		
		// Verifies if a string is present (true) in the Catalog file 
		static public bool StringExists (string s)
		{
			IntPtr ints = UnixMarshal.StringToHeap (s);
			try {
				// gettext returns the input pointer if no translation is found
				IntPtr r = gettext (ints);
				return r != ints;
			}
			catch (Exception) {
				return true;
			}
	
			finally {
				UnixMarshal.FreeHeap (ints);
			}
		}
	}
}
