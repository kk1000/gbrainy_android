/*
 * Copyright (C) 2007-2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

using gbrainy.Core.Main;
using gbrainy.Core.Services;

namespace gbrainy.Core.Views
{
	public class PlayerHistoryView : IDrawable
	{
		const double area_h = 0.80, area_w = 0.9, point_size = 0.005 * 1.25;
		const double grid_offsetx = 0.1, grid_offsety = 0.1;
		const double grid_x = grid_offsetx;
		const double grid_y = grid_offsety;
		const double grid_width = area_w -grid_offsetx;
		const double grid_height = area_h - grid_offsety;
		readonly Cairo.Color math_color = new Cairo.Color (0.56, 0.71, 0.20);    // 8fb735
		readonly Cairo.Color logic_color = new Cairo.Color (0.81, 0.54, 0.23);   // d18c3b
		readonly Cairo.Color memory_color = new Cairo.Color (0.73, 0.22, 0.51);  // bb3a84
		readonly Cairo.Color verbal_color = new Cairo.Color (0.68, 0.16, 0.17);  // af2b2c
		readonly Cairo.Color total_color = new Cairo.Color (0, 0, 0.6);
		readonly Cairo.Color text_color = new Cairo.Color (0, 0, 0);
		readonly Cairo.Color axis_color = new Cairo.Color (0.8, 0.8, 0.8);
		readonly Cairo.Color desc_color = new Color (0.3, 0.3, 0.3);
		PlayerHistory history;

		public PlayerHistoryView (PlayerHistory history)
		{
			this.history = history;
			ShowLogic = ShowMemory = ShowCalculation = ShowVerbal = true;
		}

		public bool ShowTotal { get; set; }
		public bool ShowLogic { get; set; }
		public bool ShowMemory { get; set; }
		public bool ShowCalculation { get; set; }
		public bool ShowVerbal { get; set; }

		void DrawAxisDescription (CairoContextEx cr, double x, double y, string description)
		{
			cr.Save ();
			cr.Color = desc_color;
			cr.MoveTo (x, y);
			cr.ShowPangoText (description);
			cr.Stroke ();
			cr.Restore ();
		}

		void DrawLegend (CairoContextEx cr, double x, double y)
		{
			const double line_size = 0.05, offset_x = 0.01, second_row = 0.05, space_hor = 0.35;
			double old_width;

			old_width = cr.LineWidth;
			cr.LineWidth = 0.01;

			cr.Color = total_color;
			cr.MoveTo (x, y);
			cr.LineTo (x + line_size, y);
			cr.Stroke ();
			cr.Color = text_color;
			cr.MoveTo (x + line_size + offset_x, y - 0.01);
			cr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Total"));
			cr.Stroke ();

			cr.Color = logic_color;
			cr.MoveTo (x, y + second_row);
			cr.LineTo (x + line_size, y + second_row);
			cr.Stroke ();
			cr.Color = text_color;
			cr.MoveTo (x + line_size + offset_x, y - 0.01 + second_row);
			cr.ShowPangoText (GameTypesDescription.GetLocalized (GameTypes.LogicPuzzle));
			cr.Stroke ();

			x += space_hor;
			cr.Color = memory_color;
			cr.MoveTo (x, y);
			cr.LineTo (x + line_size, y);
			cr.Stroke ();
			cr.Color = text_color;
			cr.MoveTo (x + line_size + offset_x, y - 0.01);
			cr.ShowPangoText (GameTypesDescription.GetLocalized (GameTypes.Memory));
			cr.Stroke ();

			cr.Color = math_color;
			cr.MoveTo (x, y + second_row);
			cr.LineTo (x + line_size, y + second_row);
			cr.Stroke ();
			cr.Color = text_color;
			cr.MoveTo (x + line_size + offset_x, y - 0.01 + second_row);
			cr.ShowPangoText (GameTypesDescription.GetLocalized (GameTypes.Calculation));
			cr.Stroke ();

			x += space_hor;
			cr.Color = verbal_color;
			cr.MoveTo (x, y);
			cr.LineTo (x + line_size, y);
			cr.Stroke ();
			cr.Color = text_color;
			cr.MoveTo (x + line_size + offset_x, y - 0.01);
			cr.ShowPangoText (GameTypesDescription.GetLocalized (GameTypes.VerbalAnalogy));
			cr.Stroke ();

			cr.LineWidth = old_width;
		}

		void DrawLines (CairoContextEx cr, double x, double y)
		{
			double px, py;
			double ratio;
			int pos;

			if (history.Games.Count == 0)
				return;

			ratio = grid_width / (history.Games.Count - 1);

			if (ShowLogic) {
				cr.Color = logic_color;
				cr.MoveTo (x, grid_height - (grid_height * history.Games[0].LogicScore / 100));

				pos = 1;
				for (int i = 1; i < history.Games.Count; i++)
				{
					if (history.Games[i].LogicScore < 0)
						continue;

					px = x + (ratio * pos);
					py = y + grid_height - (grid_height * history.Games[i].LogicScore / 100);
					cr.LineTo (px, py);
					pos++;
				}
				cr.Stroke ();
			}

			if (ShowCalculation) {
				cr.Color = math_color;
				cr.MoveTo (x, grid_height - (grid_height * history.Games[0].MathScore / 100));

				pos = 1;
				for (int i = 1; i < history.Games.Count; i++)
				{
					if (history.Games[i].MathScore < 0)
						continue;

					px = x + (ratio * pos);
					py = y + grid_height - (grid_height * history.Games[i].MathScore / 100);
					cr.LineTo (px, py);
					pos++;
				}
				cr.Stroke ();
			}

			if (ShowMemory) {
				cr.Color = memory_color;
				cr.MoveTo (x, grid_height - (grid_height * history.Games[0].MemoryScore / 100));

				pos = 1;
				for (int i = 1; i < history.Games.Count; i++)
				{
					if (history.Games[i].MemoryScore < 0)
						continue;

					px = x + (ratio * pos);
					py = y + grid_height - (grid_height * history.Games[i].MemoryScore / 100);
					cr.LineTo (px, py);
					pos++;
				}
				cr.Stroke ();
			}

			if (ShowVerbal) {
				cr.Color = verbal_color;
				cr.MoveTo (x, grid_height - (grid_height * history.Games[0].VerbalScore / 100));

				pos = 1;
				for (int i = 1; i < history.Games.Count; i++)
				{
					if (history.Games[i].VerbalScore < 0)
						continue;

					px = x + (ratio * i);
					py = y + grid_height - (grid_height * history.Games[i].VerbalScore / 100);
					cr.LineTo (px, py);
					pos++;
				}
				cr.Stroke ();
			}

			if (ShowTotal) {
				cr.Color = total_color;
				cr.MoveTo (x, grid_height - (grid_height * history.Games[0].TotalScore / 100));

				pos = 1;
				for (int i = 1; i < history.Games.Count; i++)
				{
					if (history.Games[pos].TotalScore < 0)
						continue;

					px = x + (ratio * pos);
					py = y + grid_height - (grid_height * history.Games[i].TotalScore / 100);
					cr.LineTo (px, py);
					pos++;
				}
				cr.Stroke ();
			}
		}

		void DrawGrid (CairoContextEx cr, double x, double y)
		{
			// Draw Axis
			cr.MoveTo (x, y);
			cr.LineTo (x, y + grid_height);
			cr.LineTo (x + grid_width, y + grid_height);
			cr.Stroke ();

			cr.Save ();
			cr.Color = axis_color;
			cr.LineWidth = 0.001;

			for (double line_y = y; line_y < grid_height + y; line_y += grid_height / 10) {
				cr.MoveTo (x, line_y);
				cr.LineTo (x + grid_width, line_y);
				cr.Stroke ();
			}

			cr.Restore ();

			// Draw score scale
			int pos = 100;
			for (double line_y = y; line_y < grid_height + y; line_y += grid_height / 10) {
				cr.DrawTextAlignedRight (x- 0.01, line_y, String.Format ("{0}", pos));
				pos -= 10;
			}
		}

		public void Draw (CairoContextEx cr, int width, int height, bool rtl)
		{
			double x = 0, y = 0;

			// Background
			cr.Color = new Cairo.Color (1, 1, 1);
			cr.Paint ();
			cr.Stroke ();

			cr.LineWidth = point_size;
			cr.Rectangle (x, y, area_w, area_h);
			cr.Clip ();
			DrawLines (cr, grid_x, grid_y);
			cr.ResetClip ();
			DrawLegend (cr, x + grid_offsetx, y + area_h + 0.06);
			DrawGrid (cr, grid_x, grid_y);

			DrawAxisDescription (cr, x + area_w + 0.01, 0.78, ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Time"));
			DrawAxisDescription (cr, 0, 0.03, ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Score"));
		}
	}
}
