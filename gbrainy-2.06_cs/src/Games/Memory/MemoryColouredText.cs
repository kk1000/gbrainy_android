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

namespace gbrainy.Games.Memory
{
	public class MemoryColouredText : Core.Main.Memory
	{
		private ColorPalette palette;
		private int question;
		private string question_colorname;
		private int colors_shown;
		private ArrayListIndicesRandom color_order;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Colored text");}
		}

		public override bool UsesColors {
			get { return true;}
		}

		public override string MemoryQuestion {
			get { 
				return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("What was the color of the text that said '{0}'?"), question_colorname);}
		}

		protected override void Initialize ()
		{
			bool done = false;

			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
				colors_shown = 3;
				break;
			case GameDifficulty.Medium:
				colors_shown = 4;
				break;
			case GameDifficulty.Master:
				colors_shown = 6;
				break;
			}

			palette = new ColorPalette ();

			// It is not acceptable that all the random colors names match the right colors
			while (done == false) {
				color_order = new ArrayListIndicesRandom (colors_shown);
				color_order.Initialize ();

				for (int i = 0; i < colors_shown; i++)
				{
					if (palette.Name (color_order [i]) != palette.Name (i)) {
						done = true;
						break;
					}
				}
			}			
		
			question = random.Next (colors_shown);
			Answer.Correct = palette.Name (color_order [question]);
			question_colorname = palette.Name (question);
		
			base.Initialize ();
		}
	
		public override void DrawObjectToMemorize (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.DrawObjectToMemorize (gr, area_width, area_height, rtl);
			DrawObject (gr);
		}

		private void DrawObject (CairoContextEx gr)
		{
			double x = DrawAreaX + 0.125, y = DrawAreaY + 0.2;
			int idx;

			palette.Alpha = alpha;

			for (int i = 0; i < colors_shown; i++)
			{
				idx = color_order [i];
				gr.Color = palette.Cairo (idx);
				gr.MoveTo (x, y);
				gr.ShowPangoText (palette.Name (i));
				gr.Stroke ();
			
				if (i == 2) {
					y += 0.2;
					x = DrawAreaX + 0.125;
				} else {
					x+= 0.25;
				}
			}
		}
	}
}
