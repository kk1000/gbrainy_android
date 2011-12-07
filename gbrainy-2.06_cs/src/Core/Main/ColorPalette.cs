/*
 * Copyright (C) 2007 Javier M Mora <javiermm@gmail.com>
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

using Cairo;
using gbrainy.Core.Services;

namespace gbrainy.Core.Main
{
	// Utility class that contains the color palette used for all games
	public class ColorPalette
	{
		double alpha;

		public enum Id
		{
			First=0,
			Red=First,
			Green,
			Blue,
			PrimaryColors,
			Yellow=PrimaryColors, 
			Magenta,
			Orange,
			PrimarySecundaryColors,
			Black=PrimarySecundaryColors,
			Last,
			White=Last
		};

		private static readonly string[] ColorName= new string[] {
			ServiceLocator.Instance.GetService <ITranslations> ().GetString ("red"),
			ServiceLocator.Instance.GetService <ITranslations> ().GetString ("green"),
			ServiceLocator.Instance.GetService <ITranslations> ().GetString ("blue"),
			ServiceLocator.Instance.GetService <ITranslations> ().GetString ("yellow"),
			ServiceLocator.Instance.GetService <ITranslations> ().GetString ("magenta"),
			ServiceLocator.Instance.GetService <ITranslations> ().GetString ("orange"),
			ServiceLocator.Instance.GetService <ITranslations> ().GetString ("black"),
			ServiceLocator.Instance.GetService <ITranslations> ().GetString ("white")
		};

		private static Cairo.Color[] CairoColor = new Cairo.Color[] {
			new Cairo.Color (0.81, 0.1, 0.13),
			new Cairo.Color (0.54, 0.71, 0.24),
			new Cairo.Color (0.17, 0.23 ,0.56),
			new Cairo.Color (0.86, 0.85, 0.25),
			new Cairo.Color (0.82, 0.25, 0.59),
			new Cairo.Color (1, 0.54, 0),
			new Cairo.Color (0, 0, 0),
			new Cairo.Color (.9, .9, .9)
		};

		public ColorPalette ()
		{
			alpha = 1;
		}

		public double Alpha {
			set { alpha = value; }
			get { return alpha; }
		}

		public int Count {
			get { return ColorName.Length; }
		}

		public Cairo.Color Cairo (Id id) 
		{
			return Cairo (CairoColor[(int)id]);
		}

		public Cairo.Color Cairo (int id)
		{
			return Cairo (CairoColor[id]);
		}

		public string Name (int index)
		{
			return ColorName [index];
		}

		Cairo.Color Cairo (Cairo.Color color)
		{
			return new Cairo.Color(color.R, color.G, color.B, alpha);
		}
	}
}
