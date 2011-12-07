/*
 * Copyright (C) 2007 Javier M Mora <javiermm@gmail.com>
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

namespace gbrainy.Games.Memory
{
	public class MemoryCountDots : Core.Main.Memory
	{
		private const int NUMCOLUMNS = 7;
		private const int MINDOTS = 1;
		private const int MAXDOTS = 25;
		private int maxdotscolor;

		private ArrayListIndicesRandom location_order;
		private ColorPalette palette;

		private int [] dotsPerColor;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Counting dots");}
		}

		public override bool UsesColors {
			get { return true;}
		}

		public override string MemoryQuestion {
			get { 
				return String.Format (
					// Translators: {0} is the name of the color. The color name is always singular
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("How many dots of {0} color were in the previous image? Answer using numbers."),
					palette.Name (0));
			}
		}

		protected override void Initialize ()
		{
			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
				maxdotscolor = 2;
				break;
			case GameDifficulty.Medium:
				maxdotscolor = 5;
				break;
			case GameDifficulty.Master:
				maxdotscolor = 8;
				break;
			}

			location_order = new ArrayListIndicesRandom (NUMCOLUMNS*NUMCOLUMNS);
			location_order.Initialize();

			palette = new ColorPalette ();

			// dotsPerColor is compared with iterator of dots. (this iterator is 0 based, so I
			// have to substract 1 to make dotsPerColor contents 0 based.
			dotsPerColor = new int [palette.Count];
			for (int i=0,before=-1; i< palette.Count; i++) {
				dotsPerColor[i] = before + MINDOTS + random.Next(maxdotscolor-MINDOTS+1);
				before = dotsPerColor[i];
			}

			Answer.Correct = (dotsPerColor[0]+1).ToString ();
		
			base.Initialize ();
		}

		public override void DrawObjectToMemorize (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.DrawObjectToMemorize (gr, area_width, area_height, rtl);
			DrawObject (gr, area_width, area_height);
		}

		private void DrawObject (CairoContextEx gr, int area_width, int area_height)
		{
			palette.Alpha = alpha;
			double x = DrawAreaX + 0.15, y = DrawAreaY + 0.1;

			gr.Color = palette.Cairo (ColorPalette.Id.Black);
			double pos_x = x, pos_y = y;
			const double figure_size = 0.6;
			const double square_size = figure_size / NUMCOLUMNS ;
			const double center_square = square_size / 2;
			double radius_square = .8 * (square_size - (LineWidth *2)) / 2;

			gr.Rectangle (pos_x, pos_y, figure_size, figure_size);
			gr.Stroke ();

			for (int line = 0; line < NUMCOLUMNS - 1; line++) // Horizontal
			{
				pos_y += square_size;
				gr.MoveTo (pos_x, pos_y);
				gr.LineTo (pos_x + figure_size, pos_y);
				gr.Stroke ();
			}

			pos_y = y;
			for (int column = 0; column < NUMCOLUMNS - 1; column++) // Vertical
			{
				pos_x += square_size;
				gr.MoveTo (pos_x, pos_y);
				gr.LineTo (pos_x, pos_y + figure_size);
				gr.Stroke ();
			}

			pos_y = y + center_square;
			pos_x = x + center_square;

			for (int i = 0,itcolor=0; i < MAXDOTS && itcolor<palette.Count; i++)
			{
				int dx,dy;
				Color color = palette.Cairo(itcolor);
				dx = (location_order[i]) % NUMCOLUMNS;
				dy = (location_order[i]) / NUMCOLUMNS;

				gr.Arc (pos_x+square_size*dx, pos_y+square_size*dy,radius_square,0,2*Math.PI);
				gr.FillGradient (pos_x+square_size*dx, pos_y+square_size*dy, radius_square, radius_square, color);

				if (i==dotsPerColor[itcolor]) itcolor++;
			}
		}
	}
}
