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

using gbrainy.Core.Main;
using gbrainy.Core.Services;

namespace gbrainy.Games.Logic
{
	public class PuzzleDivideCircle : Game
	{
		private const double figure_size = 0.15;
		private int dots;
	
		private class Circle
		{	
			public double x;
			public double y;

			public Circle (double x, double y) 
			{
				this.x = x;
				this.y = y;
			}
		}

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Divide circles");}
		}

		public override string Question {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("In the last figure, in how many regions is the circle divided into when all dots are connected?");} 
		}

		protected override void Initialize ()
		{
			if (CurrentDifficulty==GameDifficulty.Easy)
				dots = 5;
			else
				dots = 5 + random.Next (2);

			switch (dots) {
			case 5:
				Answer.Correct = "16";
				break;
			case 6:
				Answer.Correct = "30";
				break;
			}			
		}

		static private void DrawAndConnectPoints (CairoContextEx gr, double x, double y, Circle[] circles, bool connect)
		{
			const double point_size = 0.01;
			for (int i = 0; i < circles.Length; i++) {
				gr.Arc (x + point_size + circles[i].x, y + point_size + circles[i].y, point_size, 0, 2 * Math.PI);
				gr.Fill ();
				gr.Stroke ();
			}

			if (connect == false)
				return;
		
			gr.Save ();
			gr.LineWidth = 0.003;
			double offset = point_size;
			for (int from = 0; from < circles.Length; from++) {
				for (int to = 0; to < circles.Length; to++) {
					gr.MoveTo (x + circles[from].x+ offset, y + circles[from].y + offset);
					gr.LineTo (x + circles[to].x + offset, y + circles[to].y + offset);
					gr.Stroke ();
				}
			}
			gr.Restore ();
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = DrawAreaX + 0.05, y = DrawAreaY;
			double pos_x = x;
			double pos_y = y;
			Circle[] circles = null;

			base.Draw (gr, area_width, area_height, rtl);

			// First circle
			gr.Arc (pos_x + figure_size, y + figure_size, figure_size, 0, 2 * Math.PI);
			gr.Stroke ();
			DrawAndConnectPoints (gr, pos_x, pos_y, 
				new Circle [] {
					new Circle (0.14, 0),
					new Circle (0.14, 0.29),
				}, true);

			gr.MoveTo (pos_x, pos_y + figure_size * 2 + 0.05);
			gr.ShowPangoText (HasNRegionString (2));
			gr.Stroke ();

			// Second circle
			pos_x += 0.4;
			gr.Arc (pos_x + figure_size, pos_y + figure_size, figure_size, 0, 2 * Math.PI);
			gr.Stroke ();		
			DrawAndConnectPoints (gr, pos_x, pos_y,
				new Circle [] {
					new Circle (0.01, 0.06),
					new Circle (0.27, 0.06),
					new Circle (0.14, 0.29),
				}, true);

			gr.MoveTo (pos_x, pos_y + figure_size * 2 + 0.05);
			gr.ShowPangoText (HasNRegionString (4));
			gr.Stroke ();

			// Third circle
			pos_x = x;
			pos_y += 0.45;
			gr.Arc (pos_x + figure_size, pos_y + figure_size, figure_size, 0, 2 * Math.PI);
			gr.Stroke ();		
			DrawAndConnectPoints (gr, pos_x, pos_y,
				new Circle [] {
					new Circle (0.01, 0.06),
					new Circle (0.27, 0.06),
					new Circle (0.01, 0.21),
					new Circle (0.27, 0.21),
				}, true);

			gr.MoveTo (pos_x, pos_y + figure_size * 2 + 0.05);
			gr.ShowPangoText (HasNRegionString (8));
			gr.Stroke ();

			switch (dots) {
			case 5:
				circles =  new Circle [] {
					new Circle (0.01, 0.06),
					new Circle (0.27, 0.06),
					new Circle (0.01, 0.21),
					new Circle (0.27, 0.21),
					new Circle (0.14, 0),
				};
				break;
			case 6:
				circles =  new Circle [] {
					new Circle (0.01, 0.06),
					new Circle (0.27, 0.06),
					new Circle (0.01, 0.21),
					new Circle (0.27, 0.21),
					new Circle (0.14, 0),
					new Circle (0.14, 0.29)
				};
				break;
			}

			// Forth circle
			pos_x += 0.4;
			gr.Arc (pos_x + figure_size, pos_y + figure_size, figure_size, 0, 2 * Math.PI);
			gr.Stroke ();		
			DrawAndConnectPoints (gr, pos_x, pos_y, circles, Answer.Draw);
		}
		
		static string HasNRegionString (int regions)
		{
			return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetPluralString ("Has {0} region", 
				"Has {0} regions", regions), regions);
			
		}
	}
}
