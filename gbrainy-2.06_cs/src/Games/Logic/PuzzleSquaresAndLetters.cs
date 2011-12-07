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
	public class PuzzleSquaresAndLetters : Game
	{
		private char[] characters;
		private int step;
		private const double figure_size = 0.2;
		private const int figures = 3;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Squares and letters");}
		}

		public override string Question {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The letters around the squares follow a pattern. Which letter should replace the question mark in the last square?");} 
		}

		public override string Rationale {
			get {
				return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Every letter is calculated by taking the alphabetical position of the previous character and adding {0} to it in order to get the position of the next letter."), step);
			}
		}

		protected override void Initialize ()
		{
			int first_letter;
			ArrayListIndicesRandom first_letters;

			first_letters = new ArrayListIndicesRandom (figures); // Make sure that the first letter is never the same
			first_letters.Initialize ();
			step = random.Next (3) + 3;

			characters = new char [(1 + figures) * 4]; 
			for (int figure = 0; figure < figures; figure++) {
				first_letter = first_letters [figure];
				for (int letter = 0; letter < 4; letter++) {
					characters[(figure * 4) + letter] = (char) (65 + first_letter + (step * letter));
				}				
			}

			Answer.Correct = ToStr (characters[((figures - 1) * 4) + 3]);
			characters[((figures - 1) * 4) + 3] = '?';
		}

		static string ToStr (char ch)
		{
			string s = string.Empty;
			s+= ch;
			return s;
		}

		private void DrawRectangleWithText (CairoContextEx gr, double x, double y, int index)
		{
			gr.Rectangle (x, y, figure_size, figure_size);

			gr.MoveTo (x - 0.04, y);
			gr.ShowPangoText (ToStr (characters [index]));
			gr.Stroke ();

			gr.MoveTo (x + 0.01 + figure_size, y);
			gr.ShowPangoText (ToStr (characters [index + 1]));
			gr.Stroke ();

			gr.MoveTo (x - 0.04, y + figure_size);
			gr.ShowPangoText (ToStr (characters [index + 2]));
			gr.Stroke ();

			gr.MoveTo (x + 0.01 + figure_size, y + figure_size);
			gr.ShowPangoText (ToStr (characters [index + 3]));
			gr.Stroke ();
		}


		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = DrawAreaX + 0.05, y = DrawAreaY + 0.1;

			base.Draw (gr, area_width, area_height, rtl);
		
			DrawRectangleWithText (gr, x, y, 0);
			DrawRectangleWithText (gr, x + figure_size + 0.2, y, 4);
			DrawRectangleWithText (gr, x + figure_size + 0.05, y + 0.2 + figure_size, 8);
			
		}
	}
}
