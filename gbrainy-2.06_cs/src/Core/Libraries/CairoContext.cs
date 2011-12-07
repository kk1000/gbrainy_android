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
using Pango;


namespace gbrainy.Core.Libraries
{
	// Basic encapsulation of:
	//	- Cairo Context
	//	- Pango Drawing
	//	- RSvg image drawing
	//
	public class CairoContext : Cairo.Context
	{
		Pango.Layout layout;
		FontDescription font_description;
		double font_size;
		const double def_linespace = 0.018;
		const double width_margin = 0.04;

		public CairoContext (IntPtr state, Gtk.Widget widget) : base (state)
		{
			layout = Pango.CairoHelper.CreateLayout (this);

			// We do not honor DPI settings or font sizes (just font name)
			// User should resize the window
			font_description = layout.FontDescription = widget.PangoContext.FontDescription.Copy ();
			FontLineSpace = def_linespace;
		}

		// Used by GeneratePDF
		public CairoContext (Cairo.Surface s, string font, int dpis) : base (s)
		{
			layout = Pango.CairoHelper.CreateLayout (this);
			font_description = layout.FontDescription = FontDescription.FromString (font);

			if (dpis > 0)  {
				Pango.Context c = layout.Context;
				Pango.CairoHelper.ContextSetResolution (c, dpis);
				c.Dispose ();
			}
			FontLineSpace = def_linespace;
		}

		new public string FontFace {
			set {
				if (String.IsNullOrEmpty (value) == true)
					return;

				if (font_description != null)
					font_description.Dispose ();

				font_description = layout.FontDescription = Pango.FontDescription.FromString (value);
			}
		}

		public double FontLineSpace { get; set; }

		// True if we want Pango to process XML entities and formatting attributes
		public bool UseMarkup  { get; set; }

		// No dispose of resources on this class
		protected override void Dispose (bool disposing)
		{
			layout.Dispose ();

			if (font_description != null)
				font_description.Dispose ();
		}

		private void UpdateFontSize ()
		{
			// For questions and answers area the size is not necessary proportional
			double m = Matrix.Yy < Matrix.Xx ? Matrix.Yy : Matrix.Xx;
			layout.FontDescription.Size = (int) (font_size * Pango.Scale.PangoScale * m);
		}

		/*
			Font drawing using Pango

			* Pango does not work well with float numbers. We should work on
			the device unit space and then translate to our user space.

			* Cairo Show.Text paints on the bottom-left of the coordinates
			and Pango paints on the top-left of the coordinates
		*/

		void SetText (string text)
		{			
			if (UseMarkup)
				layout.SetMarkup (text);
			else
				layout.SetText (text);
		}
	
		// Shows a text from the current position. No Width defined then no RTL positioning
		public void ShowPangoText (string str)
		{
			Cairo.Matrix old = Matrix;

			UpdateFontSize ();
			Matrix = new Cairo.Matrix ();
			SetText (str);
			layout.SingleParagraphMode = true;
			layout.Width = -1;
			Pango.CairoHelper.ShowLayout (this, layout);
			Matrix = old;
		}

		// Shows a text from the current position. Defines all the line as text drawing box
		public void ShowPangoText (string str, bool bold, double width, double rotation)
		{
			Pango.Alignment align = layout.Alignment;

			if (bold) {
				layout.FontDescription.Weight = Pango.Weight.Bold;
			}

			if (width == -1) { // Calculates maximum width in the user space
				layout.Width = (int) (((1 - width_margin) * Matrix.Xx - (CurrentPoint.X * Matrix.Xx)) * Pango.Scale.PangoScale);
			} else
				layout.Width = (int) (width * Matrix.Xx * Pango.Scale.PangoScale);

			if (rotation != 0) {

				Cairo.Matrix old = Matrix;

				UpdateFontSize ();
				Matrix = new Cairo.Matrix ();
				Rotate (rotation);
				SetText (str);
				layout.SingleParagraphMode = true;

				Pango.CairoHelper.ShowLayout (this, layout);
				Matrix = old;
			}
			else
				ShowPangoText (str);

			layout.FontDescription.Weight = Pango.Weight.Normal;
			layout.Alignment = align;
		}

		public void SetPangoFontSize (double size)
		{
			font_size = size;
		}

		/*
			Draw text functions
		*/

		// Used for fractions that right align is needed
		public void DrawTextAlignedRight (double x, double y, string str)
		{
			int w, h;
			Cairo.Matrix old = Matrix;

			UpdateFontSize ();
			Matrix = new Cairo.Matrix ();

			SetText (str);
			layout.SingleParagraphMode = true;
			layout.Width = -1;
			layout.GetPixelSize (out w, out h);
			MoveTo ((old.X0 + x * old.Xx) - w, old.Y0 + y * old.Yy);
			Pango.CairoHelper.ShowLayout (this, layout);
			Matrix = old;
		}

		// From a giving point centers the text into it
		public void DrawTextCentered (double x, double y, string str)
		{
			int w, h;
			Cairo.Matrix old = Matrix;

			UpdateFontSize ();
			Matrix = new Cairo.Matrix ();

			SetText (str);
			layout.SingleParagraphMode = true;
			layout.Width = -1;
			layout.GetPixelSize (out w, out h);
			MoveTo ((old.X0 + x * old.Xx) - w / 2d, old.Y0 + (y * old.Yy) - h / 2d);
			Pango.CairoHelper.ShowLayout (this, layout);
			Matrix = old;
		}

		public void DrawStringWithWrapping (double x, double y, string str, double max_width)
		{
			int w, h, spacing;
			Cairo.Matrix old = Matrix;

			if (max_width < 0)
				throw new ArgumentOutOfRangeException ("Invalid maximum width value");

			MoveTo (x, y);
			UpdateFontSize ();
			Matrix = new Cairo.Matrix ();

			spacing = layout.Spacing;
			layout.Width = (int) (max_width * old.Xx * Pango.Scale.PangoScale);
			layout.Spacing = (int) (FontLineSpace * (old.Yy * Pango.Scale.PangoScale));

			layout.SingleParagraphMode = false;
			SetText (str);
			Pango.CairoHelper.ShowLayout (this, layout);
			layout.GetPixelSize (out w, out h);

			layout.Spacing = spacing;
			Matrix = old;
		}

		public void MeasureString (string str, double max_width, bool wrapping, out double width, out double height)
		{
			int w, h, spacing;
			Cairo.Matrix old = Matrix;

			if (max_width < 0 || max_width > 1)
				throw new ArgumentOutOfRangeException ("Invalid maximum width value");

			UpdateFontSize ();
			Matrix = new Cairo.Matrix ();

			spacing = layout.Spacing;
			layout.Width = (int) (max_width * old.Xx * Pango.Scale.PangoScale);
			layout.Spacing = (int) (FontLineSpace * (old.Xx * Pango.Scale.PangoScale));

			layout.SingleParagraphMode = !wrapping;
			SetText (str);
			layout.GetPixelSize (out w, out h);
			Matrix = old;
			layout.Spacing = spacing;

			height = h / old.Yy;
			width = w / old.Xx;
		}

		public void FillGradient (double x, double y, double w, double h, Cairo.Color color)
		{
			Save ();
			LinearGradient shadow = new LinearGradient (x, y, x + w, y + h);
			shadow.AddColorStop (0, new Cairo.Color (color.R, color.G, color.B, color.A));
			shadow.AddColorStop (0.5, new Cairo.Color (color.R, color.G, color.B, color.A * 0.7));
			Source = shadow;
			Fill ();
			Restore ();
			((IDisposable)shadow).Dispose ();
		}

		public void DrawImageFromAssembly (string  resource, double x, double y, double width, double height)
		{
			try {
				using (SVGImage image = new SVGImage (System.Reflection.Assembly.GetCallingAssembly (), resource))
				{
					DrawImage (image, x, y, width, height);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine ("CairoContext.DrawImageFromAssembly. Could not load resource {0}. Error {1}", resource, e);
			}
		}

		public void DrawImageFromFile (string filename, double x, double y, double width, double height)
		{
			try {
				using (SVGImage image = new SVGImage (filename))
				{
					DrawImage (image, x, y, width, height);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine ("CairoContext.DrawImageFromFile. Could not load file {0}. Error {1}", filename, e);
			}
		}

		public void DrawImage (SVGImage image, double x, double y, double width, double height)
		{
			Save ();
			Translate (x, y);
			Scale (width / image.Width, height / image.Height);
			image.RenderToCairo (Handle);
			Restore ();
		}
	}
}
