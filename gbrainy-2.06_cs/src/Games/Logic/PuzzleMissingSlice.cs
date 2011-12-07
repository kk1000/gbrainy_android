/*
 * Copyright (C) 2007 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
	public class PuzzleMissingSlice : Game
	{
		private const int total_slices = 6;
		private const int half_slices = total_slices / 2;
		private const int items_per_slice = 3;
		private const double radius = 0.22;
		private const double radian = Math.PI / 180;
		private const double arc_centerx = 0.2, arc_centery = 0.2;
		private const int possible_answers = 3;
		private ArrayListIndicesRandom random_indices;
		private int ans_pos;
		private int[] bad_answers;
		private int sum_offset;

		private int [] slices = new int []
		{
			2, 4, 3,
			2, 3, 6,
			1, 3, 4,
			3, 7, 1,
			6, 3, 2,
		};

		private int [] slices_opposite = new int []
		{
			6, 4, 5,
			6, 5, 2,
			7, 5, 4,
			5, 1, 7,
			2, 5, 6,
		};

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Missing slice");}
		}

		public override string Question {
			get {return String.Format (
				ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The slices below have some kind of relation. Which is the missing slice in the circle below? Answer {0}, {1} or {2}."),
				Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2));}
		}

		public override string Tip {
			get { return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Each slice is related to the opposite one.");}
		}

		public override string Rationale {
			get {
				return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("All numbers of each slice, when added to the ones of the opposite slice, add always {0}."), sum_offset + 8);
			}
		}

		protected override void Initialize ()
		{
			sum_offset = random.Next (3);
			random_indices = new ArrayListIndicesRandom (slices.Length / items_per_slice);
			random_indices.Initialize ();
			ans_pos = random.Next (possible_answers);

			Answer.CheckAttributes |= GameAnswerCheckAttributes.MultiOption | GameAnswerCheckAttributes.IgnoreSpaces;
			Answer.SetMultiOptionAnswer (ans_pos, Answer.GetFigureName (ans_pos));

			bad_answers = new int [possible_answers * items_per_slice];
			for (int i = 0; i < bad_answers.Length; i++) {
				bad_answers[i] = 1 + random.Next (9);
			}

			HorizontalContainer container = new HorizontalContainer (DrawAreaX, 0.62, 0.8, 0.3);
			DrawableArea drawable_area;
			AddWidget (container);

			for (int i = 0; i < possible_answers; i++)
			{
				drawable_area = new DrawableArea (0.8 / 3, 0.3);
				drawable_area.SelectedArea = new Rectangle (0, 0, radius, 0.2);
				drawable_area.Data = i;
				drawable_area.DataEx = Answer.GetMultiOption (i);
				container.AddChild (drawable_area);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int n = (int) e.Data;

					DrawSlice (e.Context, 0, 0);
					if (n == ans_pos) {
						int pos = random_indices [0];
						DrawSliceText (e.Context, 0, 0, 0, (sum_offset +
						slices [pos * items_per_slice]).ToString (),
						(sum_offset + slices [1 + (pos * items_per_slice)]).ToString (),
						(sum_offset + slices [2 + (pos * items_per_slice)]).ToString ());
					} else {
						DrawSliceText (e.Context, 0, 0, 0,
							bad_answers [n * items_per_slice].ToString (),
							bad_answers [1 + (n * items_per_slice)].ToString (),
							bad_answers [2 + (n * items_per_slice)].ToString ());
					}

					e.Context.MoveTo (0.0, 0.25);
					e.Context.DrawTextCentered (radius / 2, 0.25, Answer.GetFigureName (n));
					e.Context.Stroke ();
				};
			}
		}

		private static void DrawSlice (CairoContextEx gr, double x, double y)
		{
			double degrees, x1, y1;

			degrees = 0;
			gr.MoveTo (x, y);
			x1 = x + radius * Math.Cos (degrees);
			y1 = y + radius * Math.Sin (degrees);
			gr.LineTo (x1, y1);
			gr.Stroke ();

			degrees = radian * 60;
			gr.MoveTo (x, y);
			x1 = x + radius * Math.Cos (degrees);
			y1 = y + radius * Math.Sin (degrees);
			gr.LineTo (x1, y1);
			gr.Stroke ();

			gr.Arc (x, y, radius, 0, radian * 60);
			gr.Stroke ();
		}

		static private void DrawSliceText (CairoContextEx gr, double x, double y, int slice, string str1, string str2, string str3)
		{
			double x0, y0, degrees;

			// Number more near to the center;
			degrees = radian * (slice * ((360 / total_slices)) + (360 / 12));
			x0 = 0.35 * radius * Math.Cos (degrees);
			y0 = 0.35 * radius * Math.Sin (degrees);
			gr.DrawTextCentered (x + x0, y + y0, str1);

			// Number opposite to the center and at the top
			degrees = radian * (slice * ((360 / total_slices)) + (360 / 24));
			x0 = 0.8 * radius * Math.Cos (degrees);
			y0 = 0.8 * radius * Math.Sin (degrees);
			gr.DrawTextCentered (x + x0, y + y0, str2);

			// Number opposite to the center and at the bottom
			degrees = radian * (slice * ((360 / total_slices)) + (360 / 8));
			x0 = 0.8 * radius * Math.Cos (degrees);
			y0 = 0.8 * radius * Math.Sin (degrees);
			gr.DrawTextCentered (x + x0, y + y0, str3);
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = DrawAreaX + 0.2, y = DrawAreaY;
			double x0, y0, degrees;
			int pos;

			base.Draw (gr, area_width, area_height, rtl);

			gr.Arc (x + arc_centerx, y + arc_centery, radius, 0, 2 * Math.PI);
			gr.Stroke ();

			for (int slice = 0; slice < total_slices; slice++)
			{
				degrees = radian * slice * (360 / total_slices);
				gr.MoveTo (x + arc_centerx, y + arc_centery);
				gr.LineTo (x + arc_centerx + (radius * Math.Cos (degrees)), y + arc_centery + (radius * Math.Sin (degrees)));

				if (slice > total_slices - 1) continue;

				if (slice == 0) {
					degrees = radian * (slice * ((360 / total_slices)) + (360 / 12));
					x0 = 0.5 * radius * Math.Cos (degrees);
					y0 = 0.5 * radius * Math.Sin (degrees);
					gr.DrawTextCentered (x + arc_centerx + x0, y + arc_centery + y0, "?");
					continue;
				}

				if (slice < half_slices) {
					pos = random_indices [slice];
					DrawSliceText (gr, x + arc_centerx, y + arc_centery, slice, (sum_offset + slices [pos * items_per_slice]).ToString (),
						 (sum_offset + slices [1 + (pos * items_per_slice)]).ToString (), (sum_offset + slices [2 + (pos * items_per_slice)]).ToString ());
				}
				else {
					pos = random_indices [slice - half_slices];
					DrawSliceText (gr, x + arc_centerx, y + arc_centery, slice, slices_opposite [pos * items_per_slice].ToString (),
						 slices_opposite [2 + (pos * items_per_slice)].ToString (), slices_opposite [1 + (pos * items_per_slice)].ToString ());
				}
			}

			gr.MoveTo (0.1, 0.55);
			gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Choose one of the following:"));
			gr.Stroke ();
		}
	}
}
