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
using Mono.Unix;

using gbrainy.Core.Services;

namespace gbrainy.Core.Libraries
{
	public class TranslationsCatalog : ITranslations
	{
		double strings, translated;
		const int max_sample = 250;

		public int TranslationPercentage { 
			get {
				if (strings > 0)
					return (int) (translated / strings * 100d);

				return 100; // Cannot tell
			}
		}

		public void Init (string package, string localedir)
		{
			Catalog.Init (package, localedir);
		}
		
		public string GetString (string s)
		{
			if (strings < max_sample)
			{
				if (GetText.StringExists (s))
					translated++;

				strings++;
			}

			return Catalog.GetString (s);
		}

		public string GetPluralString (string s, string p, int n)
		{
			return Catalog.GetPluralString (s, p, n);
		}
	}
}

