/*
 * Copyright (C) 2008-2011 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Games.Logic
{
	public class PuzzleExtraCircle : Game
	{
		const int total_slices = 6;
		const int circles = 4;
		const double radius = 0.1;
		const double radian = Math.PI / 180;
		int ans_pos;
		Color[] cercle_colors;
		Color[] badcercle_colors;
		int[] start_indices;
		ColorPalette cp;
		CircleParameters[] circle_parameters;

		struct CircleParameters
		{
			public Color [] Colors {get; set; }

			public CircleParameters (Color [] colors) : this ()
			{
				Colors = colors;
			}
		};

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Extra circle");}
		}

		public override bool UsesColors {
			get { return true;}
		}

		public override string Question {
			get {return String.Format (
				ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Which circle does not belong to the group? It is not a sequence of elements. Answer {0}, {1}, {2} or {3}."),
					Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3));}
		}

		public override string Tip {
			get { return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("All circles share a common property except for one.");}
		}

		public override string Rationale {
			get {
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("In all circles the color slices follow the same order except for this one.");
			}
		}

		protected override void Initialize ()
		{
			ArrayListIndicesRandom random_indices = new ArrayListIndicesRandom (total_slices);
			Color clr;

			cp = new ColorPalette ();

			Answer.CheckAttributes |= GameAnswerCheckAttributes.MultiOption | GameAnswerCheckAttributes.IgnoreSpaces;

			cercle_colors = new Color [total_slices];
			badcercle_colors =  new Color [total_slices];
			for (int i = 0; i < total_slices; i++) {
				cercle_colors [i] = cp.Cairo (i);
				badcercle_colors [i] = cp.Cairo (i);
			}

			// Correct answer
			random_indices.Initialize ();
			clr = badcercle_colors [random_indices[0]];
			badcercle_colors [random_indices[0]] =  badcercle_colors [random_indices[1]];
			badcercle_colors [random_indices[1]] = clr;

			// Create random color order for the session
			start_indices = new int [circles];
			for (int i = 0; i < circles; i++)
				start_indices[i] = (random_indices[i]);

			ans_pos = random.Next (circles);
			Answer.SetMultiOptionAnswer (ans_pos, Answer.GetFigureName (ans_pos));

			const double text_offset = 0.04;
			const double witdh_used = 0.9; // Total width used for drawing all the figures
			const double margin = 0.1 / circles / 2;
			const double box_size = witdh_used / circles;
			double y;
			HorizontalContainer container;
			DrawableArea drawable_area;
			Color [] colors;

			y = DrawAreaY + 0.1 + (radius / 2);

			container = new HorizontalContainer (0.05, y, witdh_used, box_size);
			AddWidget (container);

			circle_parameters = new CircleParameters [circles];
			for (int i = 0; i < circles; i++)
			{
				if (ans_pos == i)
					colors = badcercle_colors;
				else
					colors = cercle_colors;

				circle_parameters [i] = new CircleParameters (colors);
				drawable_area = new DrawableArea (box_size, box_size);
				drawable_area.SelectedArea = new Rectangle ((box_size - box_size) / 2, 0, box_size, box_size);
				drawable_area.Data = i;
				drawable_area.DataEx = Answer.GetMultiOption (i);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int idx = (int) e.Data;
					CircleParameters circle = circle_parameters [idx];
					double x1, y1;

					x1 = y1 = radius + margin;

					DrawCircle (e.Context, x1, y1, circle.Colors, start_indices [idx]);
					e.Context.DrawTextCentered (e.Width / 2, box_size + text_offset,
						Answer.GetFigureName (idx));
					e.Context.Stroke ();
				};
				container.AddChild (drawable_area);
			}
		}

		static void DrawSlice (CairoContextEx gr, double x, double y, double dg, Color color)
		{
			double x1, y1, smallest_x, smallest_y, degrees;

			smallest_x = x;
			smallest_y = y;
			degrees = radian * (60 + dg);
			gr.MoveTo (x, y);
			x1 = x + radius * Math.Cos (degrees);
			y1 = y + radius * Math.Sin (degrees);
			gr.LineTo (x1, y1);
			if (x1 < smallest_x) smallest_x = x1;
			if (y1 < smallest_y) smallest_y = y1;

			degrees = dg * radian;
			gr.MoveTo (x, y);
			x1 = x + radius * Math.Cos (degrees);
			y1 = y + radius * Math.Sin (degrees);
			gr.LineTo (x1, y1);
			if (x1 < smallest_x) smallest_x = x1;
			if (y1 < smallest_y) smallest_y = y1;

			gr.Arc (x, y, radius, dg * radian, radian * (60 + dg));
			gr.FillGradient (smallest_x, smallest_y, radius, radius, color);
			gr.Stroke ();
		}

		static void DrawCircle (CairoContextEx gr, double x, double y, Color[] colors, int color_indice)
		{
			double dg;
			gr.Arc (x, y, radius, 0, 2 * Math.PI);
			gr.Stroke ();

			gr.Save ();
			for (int slice = 0; slice < total_slices; slice++)
			{
				dg = slice * (360 / total_slices);
				DrawSlice (gr, x, y, dg, colors [color_indice]);

				color_indice++;
				if (color_indice >= colors.Length)
					color_indice = 0;

			}
			gr.Restore ();
		}
	}
}
