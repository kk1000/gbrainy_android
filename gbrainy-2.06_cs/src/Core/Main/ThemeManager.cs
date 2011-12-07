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
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace gbrainy.Core.Main
{
	public static class ThemeManager
	{
		static string file, config_path;
		static List <Theme> themes;

		static ThemeManager ()
		{
			ConfigPath = Defines.DATA_DIR;
		}

		static public string ConfigPath {
			set {
				config_path = value;
				file = Path.Combine (config_path, "themes.xml");
			}
		}

		static public Theme [] Themes {
			get {
				if (themes == null)
				{
					Load ();
					if (themes == null)
						themes = new List <Theme> ();
				}
				return themes.ToArray ();
			}
		}

		static public Theme FromName (string name)
		{
			foreach (Theme theme in Themes)
			{
				if (String.Compare (name, theme.Name, true) == 0)
				{
					return theme;
				}
			}
			throw new InvalidOperationException (String.Format ("ThemeManager. Theme not found '{0}'", name));
		}

		static public void Load ()
		{
			try {
				using (FileStream str = File.OpenRead (file))
				{
					XmlSerializer bf = new XmlSerializer (typeof (List <Theme>));
				    	themes = (List <Theme>) bf.Deserialize(str);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine ("ThemeManager.Load. Could not load file {0}. Error {1}", file, e);
			}
		}
	}
}
