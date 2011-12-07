/*
 * Copyright (C) 2007-2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using Cairo;

using gbrainy.Core.Libraries;

namespace gbrainy.Core.Main
{
	// Implements functionality specific to gbrainy
	public class CairoContextEx : CairoContext
	{
		static SVGImage image;

		public CairoContextEx (IntPtr state, Gtk.Widget widget) : base (state, widget)
		{
			CommonConstructor ();		
		}

		// Used by GeneratePDF
		public CairoContextEx (Cairo.Surface s, string font, int dpis) : base (s, font, dpis)
		{
			CommonConstructor ();
		}

		void CommonConstructor ()
		{
			Theme theme;
	
			theme = ThemeManager.FromName (Preferences.Get <string> (Preferences.ThemeKey));
			FontFace = theme.FontFace;

			SetPangoNormalFontSize ();
		}
		
		public static void ResetDrawBackgroundCache ()
		{
			if (image == null)
				return;

			image.Dispose ();
			image = null;
		}

		virtual public void DrawBackground ()
		{
			try {
				if (image == null)
				{
					Theme theme;
	
					theme = ThemeManager.FromName (Preferences.Get <string> (Preferences.ThemeKey));
					image = new SVGImage (System.IO.Path.Combine (Defines.DATA_DIR, theme.BackgroundImage));
				}

				Save ();
				Rectangle (0, 0, 1, 1);
				Scale (0.999 / image.Width, 0.999 / image.Height);
				image.RenderToCairo (Handle);
				Restore ();
			}
			catch (Exception e)
			{
				Console.WriteLine ("CairoContextEx.DrawBackground {0}", e);
			}
		}

		public void SetPangoNormalFontSize ()
		{
			SetPangoFontSize (0.022);
		}

		public void SetPangoLargeFontSize ()
		{
			SetPangoFontSize (0.0325);
		}

		public void DrawEquilateralTriangle (double x, double y, double size)
		{
			MoveTo (x + (size / 2), y);
			LineTo (x, y + size);
			LineTo (x + size, y + size);
			LineTo (x + (size / 2), y);
			Stroke ();
		}

		public void DrawDiamond (double x, double y, double size)
		{
			MoveTo (x + size / 2, y);
			LineTo (x, y + size / 2);
			LineTo (x + size / 2, y + size);
			LineTo (x + size, y + size / 2);
			LineTo (x + size / 2, y);
			Stroke ();
		}

		// Draws a regular pentagon
		public void DrawPentagon (double x, double y, double size)
		{
			MoveTo (x + (0.4998 * size), y + ( 0.0051 * size));
			LineTo (x + (0.9949 * size), y + (0.3648 * size));
			LineTo (x + (0.8058 * size), y + (0.9468 * size));
			LineTo (x + (0.1938 * size), y + (0.9468 * size));
			LineTo (x + (0.0046 * size), y + (0.3648 * size));
			LineTo (x + (0.4998 * size), y + (0.0051 * size));
			Stroke ();
		}

		public void FillGradient (double x, double y, double w, double h)
		{
			Save ();
			LinearGradient shadow = new LinearGradient (x, y, x + w, y + h);
			shadow.AddColorStop (0, new Cairo.Color (0, 0, 0, 0.3));
			shadow.AddColorStop (0.5, new Cairo.Color (0, 0, 0, 0.1));
			Source = shadow;
			Fill ();
			Restore ();
			((IDisposable)shadow).Dispose ();
		}

		public void DrawClock (double x, double y, double size, int hand_short, int hand_large)
		{
			const double radian = Math.PI / 180;
			double radius = size / 2;
			double x0, y0;
			int num, degrees;

			Arc (x, y, radius, 0, 2 * Math.PI);
			Stroke ();
			for (degrees = 0; degrees < 360; degrees+= 30) {
				x0 = radius * Math.Cos (degrees * radian);
				y0 = radius * Math.Sin (degrees * radian);
				 // Small lines
				MoveTo (x + 0.9 * x0, y + 0.9 * y0);
				LineTo (x + x0, y + y0);
				Stroke ();
				// Numbers
				num = (degrees / 30) + 3;
				if (num > 12) num = num - 12;

				DrawTextCentered (x + x0 * 0.75,  y + y0 * 0.75, num.ToString ());
				Stroke ();
			}

			if (hand_large >=1 && hand_large <= 12 ) {
				// Hand Large
				degrees = (hand_large - 3) * 30;
				x0 = radius * Math.Cos (degrees * radian);
				y0 = radius * Math.Sin (degrees * radian);
				MoveTo (x, y);
				LineTo (x + x0 * 0.55, y + y0 * 0.55);
				Stroke ();
			}

			if (hand_short >=1 && hand_short <= 12) {
				// Hand Short
				degrees = (hand_short - 3) * 30;
				x0 = radius * Math.Cos (degrees * radian);
				y0 = radius * Math.Sin (degrees * radian);
				MoveTo (x, y);
				LineTo (x + x0 * 0.4, y + y0 * 0.4);
				Stroke ();
			}
		}
	}
}
