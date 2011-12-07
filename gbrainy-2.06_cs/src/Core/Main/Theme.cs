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
using System.IO;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Globalization;

using Cairo;

using gbrainy.Core.Services;

namespace gbrainy.Core.Main
{
	public class Theme
	{
		[XmlElementAttribute ("name")]
		public string Name { get; set; }

		[XmlElementAttribute ("_localized_name")]
		public string LocalizedName { get; set; }

		[XmlElementAttribute ("background_image")]
		public string BackgroundImage { get; set; }

		[XmlElementAttribute ("font_face")]
		public string FontFace { get; set; }

		[XmlElementAttribute ("ink_color")]
		public string InkColor { get; set; }

		Regex regex;
		static Cairo.Color default_color = new Cairo.Color (0, 0, 0);

		public Cairo.Color InkCairoColor {
			get {
				Match match;
				int r, g, b, a = 255;

				if (regex == null)
					regex = new Regex ("[A-Fa-f0-9]{2}", RegexOptions.IgnoreCase);

				match = regex.Match (InkColor);

				// r
				if (String.IsNullOrEmpty (match.Value))
					return default_color;

				int.TryParse (match.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out r);
				match = match.NextMatch ();

				// g
				if (String.IsNullOrEmpty (match.Value))
					return default_color;

				int.TryParse (match.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out g);
				match = match.NextMatch ();

				// b
				if (String.IsNullOrEmpty (match.Value))
					return default_color;

				int.TryParse (match.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out b);
				match = match.NextMatch ();

				// a
				int.TryParse (match.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out a);

				return new Cairo.Color ((double) r / 255d, (double) g / 255d, (double) b / 255d, (double) a / 255d);
			}
		}
		
		public string GetFullPath (string path)
		{
			IConfiguration config = ServiceLocator.Instance.GetService <IConfiguration> ();
			
			return System.IO.Path.Combine (config.Get <string> (ConfigurationKeys.ThemesDir), path);
		}
	}
}

