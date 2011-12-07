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

using Cairo;

using gbrainy.Core.Main;
using gbrainy.Core.Services;

namespace gbrainy.Games.Logic
{
	public class PuzzleCoverPercentage : Game
	{
		private bool cover_zone1, cover_zone2, cover_zone3, cover_zone4;
		private int partial_zone, partial_zones;
		private const double width = 0.5, height = 0.5;
		private const double line_width = 0.001;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Cover percentage");}
		}

		public override string Question {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("What percentage of the figure is colored?");} 
		}

		protected override void Initialize ()
		{
			int total = 0;

			cover_zone1 = random.Next (3) == 0;
			cover_zone2 = random.Next (3) == 0;
			cover_zone3 = random.Next (3) == 0;
			cover_zone4 = random.Next (3) == 0;

			if (cover_zone1 && cover_zone2 && cover_zone3)
				cover_zone4 = false;

			if (cover_zone1) total+= 25;
			if (cover_zone2) total+= 25;
			if (cover_zone3) total+= 25;
			if (cover_zone4) total+= 25;

			if (cover_zone1 == false)
				partial_zone = 1;
			else
				if (cover_zone2 == false)
					partial_zone = 2;
				else
					if (cover_zone3 == false)
						partial_zone = 3;
					else
						if (cover_zone4 == false)
							partial_zone = 4;

			partial_zones = random.Next (2) + 1;
			total += partial_zones * 5;
			Answer.Correct = total.ToString ();
			Answer.CheckExpression = "[0-9]+";
		}

		private static void Fill (CairoContextEx gr, double x, double y, double w, double h)
		{
			gr.Rectangle (x, y, w, h);
			gr.FillGradient (x, y, w, h);
		}

		private void DrawSection (CairoContextEx gr, double x, double y)
		{
			double w =  width / 2, h = height / 2;
			double pos_x = x, pos_y = y;
			for (int i = 0; i < 5; i++) {
				gr.MoveTo (pos_x, pos_y + h / 5);
				gr.LineTo (pos_x + w, pos_y + h / 5);
				gr.Stroke ();
				pos_y += h / 5;
			}
	
			gr.MoveTo (x + w / 2, y);
			gr.LineTo (x + w / 2, y + h);
			gr.Stroke ();

			gr.Save ();
			gr.Color = new Cairo.Color (0.90, 0.90, 0.90);
			for (int i = 0; i < partial_zones; i++) {
				Fill (gr, x + line_width, line_width + y, 
					(w / 2) - (line_width * 2) , (h / 5) - (line_width * 2));
				Fill (gr, x + (w / 2) + line_width * 2, line_width + y, 
					(w / 2) - (line_width * 4) , (h / 5) - (line_width * 2));
				y += h / 5;
			}
			gr.Restore ();
		}

		static private void CoverZone (CairoContextEx gr, double x, double y)
		{
			Fill (gr, x + line_width, y + line_width , (width / 2) - (line_width * 2), (height / 2) - (line_width * 2));
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = 0.25, y = 0.25;

			base.Draw (gr, area_width, area_height, rtl);
		
			gr.Rectangle (x, y, width, height);
			gr.Stroke ();

			gr.MoveTo (x, y + height / 2);
			gr.LineTo (x + width, y + height / 2);
			gr.Stroke ();

			gr.MoveTo (x + width / 2, y);
			gr.LineTo (x + width / 2, y + height);
			gr.Stroke ();

			if (cover_zone1) 
				CoverZone (gr, x, y);

			if (cover_zone2) 
				CoverZone (gr, x + width / 2, y);

			if (cover_zone3) 
				CoverZone (gr, x, y + height / 2);
		
			if (cover_zone4) 
				CoverZone (gr, x + width / 2, y + height / 2);

			switch (partial_zone) {
				case 1:
					break;
				case 2:
					x += width / 2;
					break;
				case 3:
					y += height / 2;
					break;
				case 4:
					y += height / 2;
					x += width / 2;
					break;
			}

			DrawSection (gr, x, y);
		}
	}
}
