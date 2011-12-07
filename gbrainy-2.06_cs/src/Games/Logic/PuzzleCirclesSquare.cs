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
	public class PuzzleCirclesSquare : Game
	{
		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Circles in a square");}
		}

		public override string Question {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("What is the maximum number of circles (as shown) that fit in the square below?");} 
		}

		public override string Tip {
			get { return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("You can fit more than 64 circles.");}
		}

		public override string Rationale {
			get {
				// Translators: {0} is replaced always by 0.1340
				return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Using the above layout {0} units of height are gained per row leaving enough space for an additional row."), 0.1340);
			}
		}

		protected override void Initialize ()
		{
			Answer.Correct = "68";
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double first_x = DrawAreaX + 0.05;
			double first_y = DrawAreaY + 0.1;
			const double space_fromrect = 0.02, space_fromcircle = 0.01;
			int circles = 8;
			const double unit = 0.0625;

			base.Draw (gr, area_width, area_height, rtl);

			gr.Rectangle (first_x, first_y, unit * 8, unit * 8);
			gr.Stroke ();

			// |-------|
			gr.MoveTo (first_x, first_y - 0.04 - space_fromrect);
			gr.LineTo (first_x, first_y - space_fromrect);
			gr.Stroke ();
			gr.MoveTo (first_x, first_y - 0.02 - space_fromrect);
			gr.LineTo (first_x + 0.5, first_y - 0.02 - space_fromrect);
			gr.Stroke ();
			gr.MoveTo (first_x + 0.5, first_y - 0.04 - space_fromrect);
			gr.LineTo (first_x + 0.5, first_y - space_fromrect);
			gr.Stroke ();

			gr.MoveTo (first_x + 0.2, first_y - 0.06 - space_fromrect);
			gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("8 units"));
			gr.Stroke ();



			//  ---
			//	 |
			//	 |
			//	 |
			//  ---
			gr.MoveTo (first_x  - space_fromrect, first_y);
			gr.LineTo (first_x  - space_fromrect - 0.04, first_y);
			gr.Stroke ();
			gr.MoveTo (first_x - space_fromrect - 0.02, first_y);
			gr.LineTo (first_x - space_fromrect - 0.02, first_y + 0.5);
			gr.Stroke ();
			gr.MoveTo (first_x - space_fromrect, first_y + 0.5);
			gr.LineTo (first_x - space_fromrect - 0.04, first_y + 0.5);
			gr.Stroke ();

			gr.MoveTo (first_x - space_fromrect - 0.07, first_y + 0.3);
			gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("8 units"), false, -1, 270 * Math.PI/180);
			gr.Stroke ();

			// Sample circle
			gr.Arc (first_x + 0.7, first_y + 0.1, unit / 2, 0, 2 * Math.PI);
			gr.Stroke ();

			// |-------|
			gr.MoveTo (first_x + 0.65, first_y + 0.05 - 0.04 - space_fromcircle);
			gr.LineTo (first_x + 0.65, first_y + 0.05 - space_fromcircle);
			gr.Stroke ();
			gr.MoveTo (first_x + 0.65, first_y + 0.05 - 0.02 - space_fromcircle);
			gr.LineTo (first_x + 0.65 + 0.1, first_y + 0.05 - 0.02 - space_fromcircle);
			gr.Stroke ();
			gr.MoveTo (first_x + 0.65 + 0.1, first_y + 0.05 - 0.04 - space_fromcircle);
			gr.LineTo (first_x + 0.65 + 0.1, first_y + 0.05 - space_fromcircle);
			gr.Stroke ();

			gr.MoveTo (first_x + 0.65, first_y - 0.04 - space_fromcircle);
			gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("1 unit"));
			gr.Stroke ();

			//  ---
			//	 |
			//	 |
			//	 |
			//  ---
			gr.MoveTo (first_x + 0.65  - space_fromcircle, first_y + 0.05);
			gr.LineTo (first_x + 0.65  - space_fromcircle - 0.04, first_y + 0.05);
			gr.Stroke ();
			gr.MoveTo (first_x + 0.65 - space_fromcircle - 0.02, first_y + 0.05);
			gr.LineTo (first_x + 0.65 - space_fromcircle - 0.02, first_y  + 0.05 + 0.1);
			gr.Stroke ();
			gr.MoveTo (first_x + 0.65 - space_fromcircle, first_y + 0.1 + 0.05);
			gr.LineTo (first_x + 0.65 - space_fromcircle - 0.04, first_y + 0.1 + 0.05);
			gr.Stroke ();

			gr.MoveTo (first_x + 0.65 - space_fromcircle - 0.08, first_y + 0.15);
			gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("1 unit"), false, -1, 270 * Math.PI/180);
			gr.Stroke ();

			if (Answer.Draw == false)
				return;

			double x;
			for (int line = 0; line < 9; line++)
			{
				for (int circle = 0; circle < circles; circle++) 
				{
					x = first_x + (unit / 2) + (circle * unit);
				
					if (circles == 7)
						x+= unit / 2;

					gr.Arc (x, (unit / 2) + first_y + (unit * line) - (unit / 8) * line, 
							(unit / 2), 0, 2 * Math.PI);
					gr.Stroke ();
				}

				if (circles ==8)
					circles = 7;
				else
					circles = 8;
			}

		}
	}
}
