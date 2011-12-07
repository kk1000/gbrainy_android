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
using Gtk;
using Cairo;
using Mono.Unix;
using gbrainy.Core.Libraries;

using gbrainy.Core.Main;

namespace gbrainy.Clients.Classical.Widgets
{
	// This client code because every client can decide how to composite its game view
	// For example, asking the question in another area, or using the same space for
	// question or answer, etc.
	public class GameDrawingArea : DrawingArea
	{
		public enum SolutionType
		{
			None,
			CorrectAnswer,
			InvalidAnswer,
			Tip,
		};

		public IDrawable Drawable { get; set; }
		public string Question { get; set; }
		public string Solution { get; set; }
		public int DrawingSquare { get; private set; }
		public double OffsetX { get; private set; }
		public double OffsetY { get; private set; }
		public bool UseSolutionArea { get; set; }
		public SolutionType SolutionIcon { get; set; }

		bool paused;
		public bool Paused {
			get { return paused; }
			set {
				paused = value;
				QueueDraw ();
			}
		}

		// Constants
		const int question_high = 55;
		const int solution_high = 55;
		const double text_margin = 0.010;
		const double icon_size = 0.08;
		const double icon_margin = 0.01;

		SVGImage [] images;

		public GameDrawingArea ()
		{
			UseSolutionArea = true;
			SolutionIcon = SolutionType.None;
			images = new SVGImage [Enum.GetValues (typeof (SolutionType)).Length];
		}

		public void ReloadBackground ()
		{
			CairoContextEx.ResetDrawBackgroundCache ();
			QueueDraw ();
		}

		protected override bool OnExposeEvent (Gdk.EventExpose args)
		{
			if (!IsRealized)
				return false;

			int w, h, total_w, total_h;

			Cairo.Context cc = Gdk.CairoHelper.Create (args.Window);
			CairoContextEx cr = new CairoContextEx (cc.Handle, this);

			args.Window.GetSize (out total_w, out total_h);

			h = total_h - question_high;
			if (UseSolutionArea)
				h -= solution_high;

			w = total_w;

			// We want a square drawing area for the puzzles then the figures are shown as designed.
			// For example, squares are squares. This also makes sure that proportions are kept when resizing
			DrawingSquare = Math.Min (w, h);

			if (DrawingSquare < w)
				OffsetX = (w - DrawingSquare) / 2d;
			else
				OffsetX = 0;

			if (DrawingSquare < h)
				OffsetY = (h - DrawingSquare) / 2d;
			else
				OffsetY = 0;

			OffsetY += question_high;

			// Draw a background taking all the window area
			cr.Save ();
			cr.Scale (total_w, total_h);
			cr.DrawBackground ();

			if (Paused == false) {
				DrawQuestionAndAnswer (cr, total_h);
			} else {
				cr.SetPangoFontSize (0.08);
				cr.DrawTextCentered (0.5, 0.5, Catalog.GetString ("Paused"));
				cr.Stroke ();
			}
			cr.Restore ();

			if (Paused == false) {
				// Draw the game area
				cr.Translate (OffsetX, OffsetY);
				cr.SetPangoNormalFontSize ();
				cr.Color = new Color (1, 1, 1, 0.5);
				Drawable.Draw (cr, DrawingSquare, DrawingSquare, Direction == Gtk.TextDirection.Rtl);
				cr.Stroke ();
			}

			((IDisposable)cc).Dispose();
			((IDisposable)cr).Dispose();
			return true;
		}

		void DrawQuestionAndAnswer (CairoContextEx cr, int height)
		{
			double max_width;
			double line_space;

			line_space = cr.FontLineSpace;
			cr.FontLineSpace = 0;
			cr.SetPangoFontSize (0.020);

			max_width = 1 - (text_margin * 2);
			cr.UseMarkup = true;

			DrawQuestion (cr, height, max_width);
			DrawSolution (cr, height, max_width);

			cr.UseMarkup = false;
			cr.FontLineSpace = line_space;
		}

		void DrawQuestion (CairoContextEx cr, int height, double max_width)
		{
			if (String.IsNullOrEmpty (Question) == true)
				return;

			cr.DrawStringWithWrapping (text_margin, text_margin, Question, max_width);
			cr.Stroke ();

			double w, h, question_high_scaled;
			cr.MeasureString (Question, max_width, true, out w, out h);

			// We use a minimum hight, but if the text is longer (L10 versions) move the line as needed
			question_high_scaled = Math.Max (question_high / (double) height, h);
			cr.LineWidth = 0.002;
			cr.MoveTo (0.01, question_high_scaled + 0.01);
			cr.LineTo (0.99, question_high_scaled + 0.01);
			cr.Stroke ();
		}

		void DrawSolution (CairoContextEx cr, int height, double max_width)
		{
			if (UseSolutionArea == false || String.IsNullOrEmpty (Solution) == true)
				return;

			double width_str, height_str, x_text, icon_x, icon_w, icon_h, box_height_scaled;

			cr.Save ();
			cr.LineWidth = 0.001;

			icon_w = icon_size * (cr.Matrix.Xx > cr.Matrix.Yy ? cr.Matrix.Yy / cr.Matrix.Xx : 1);
			icon_h = icon_size * (cr.Matrix.Yy > cr.Matrix.Xx ? cr.Matrix.Xx / cr.Matrix.Yy : 1);

			cr.MeasureString (Solution, max_width - icon_w, true, out width_str, out height_str);

			// In case that the string to show is longer than the space reserved (long translations for example)
			// allow the box to grow taking part of the lower part of the graphic
			box_height_scaled = Math.Max (height_str, (double) solution_high / (double) height);

			// Draw black box
			cr.Color = new Color (0.1, 0.1, 0.1);

			cr.Rectangle (text_margin,
				1 - box_height_scaled - text_margin,
				max_width,
				box_height_scaled);
			cr.Fill ();
			cr.Stroke ();

			// Draw text and icon
			cr.Color = new Color (1, 1, 1);

			if (Direction == Gtk.TextDirection.Rtl)
			{
				x_text = 0;
				icon_x = max_width - icon_w;
			}
			else
			{
				x_text =  icon_w + text_margin;
				icon_x = 0;
			}

			cr.DrawStringWithWrapping (x_text,
				(1 - box_height_scaled - text_margin) + ((box_height_scaled - height_str) / 2),
				Solution, max_width - icon_w);
			cr.Stroke ();

			DrawSolutionIcon (cr, icon_x,
				(1 - box_height_scaled - text_margin) + ((box_height_scaled - icon_h) / 2),
				icon_w, icon_h);
			cr.Restore ();
		}

		void DrawSolutionIcon (CairoContextEx cr, double x, double y, double width, double height)
		{
			string image;
			int img_index = (int) SolutionIcon;

			switch (SolutionIcon) {
			case SolutionType.CorrectAnswer:
				image = "gtk-ok.svg";
				break;
			case SolutionType.InvalidAnswer:
				image = "gtk-stop.svg";
				break;
			case SolutionType.Tip:
				image = "gtk-info.svg";
				break;
			default:
				return;
			}

			// In memory games, the image gets painted several dozen times
			if (images [img_index] == null) {
				images [img_index] = new SVGImage (System.Reflection.Assembly.GetExecutingAssembly (), image);
			}

			cr.DrawImage (images [img_index], x + icon_margin, y, width, height);
		}
	}
}
