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

using gbrainy.Core.Main;
using gbrainy.Core.Services;

namespace gbrainy.Games.Logic
{
	public class PuzzleSquareSheets : Game
	{
		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Square sheets");}
		}

		public override string Question {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("What is the minimum number of square sheets of paper of any size required to create the figure? Lines indicate frontiers between different sheets.");}
		}

		public override string Tip {
			get { return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The sheets should overlap.");}
		}

		public override string Rationale {
			get {
				// Translators: the translated version should not take more characters that the English original
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("A full sized square of paper, a 3/4 of the whole size square of paper in the bottom right corner, another 3/4 square of paper in the top left corner and a 1/4 square of paper in the top left corner.");
			}
		}

		protected override void Initialize ()
		{
			Answer.Correct = "4";
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = DrawAreaX + 0.2, y = DrawAreaY + 0.2, width = 0.4, height = 0.4;

			base.Draw (gr, area_width, area_height, rtl);

			gr.Rectangle (x, y, width, height);
			gr.Stroke ();

			gr.MoveTo (x, y + 0.1);
			gr.LineTo (x + width, y + 0.1);  // First horizontal
			gr.Stroke ();

			gr.MoveTo (x, y + 0.3);
			gr.LineTo (x + width - 0.1, y + 0.3); // Second horizontal
			gr.Stroke ();

			gr.MoveTo (x + 0.1, y);
			gr.LineTo (x + 0.1, y + height);  // First vertical
			gr.Stroke ();

			gr.MoveTo (x + 0.3, y);
			gr.LineTo (x + 0.3, y + height - 0.1);  // Second vertical
			gr.Stroke ();
		}
	}
}


