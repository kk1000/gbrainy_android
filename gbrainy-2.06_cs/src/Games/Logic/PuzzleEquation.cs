/*
 * Copyright (C) 2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
	public class PuzzleEquation : Game
	{
		private int num_a, num_b, num_c, num_d, num_e;
		private string formula;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Equation");}
		}

		public override string Question {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("What is the result of the equation below?");} 
		}

		public override string Rationale {
			get {
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The order of arithmetical operations is always as follows: exponents and roots, multiplication and division, addition and subtraction.");
			}
		}

		protected override void Initialize ()
		{
			bool found  = false;
			int order = 0, sequential;

			while (found == false) {
				num_a = 2 + random.Next (5);
				num_b = 2 + random.Next (5);
				num_c = 2 + random.Next (5);
				num_d = 2 + random.Next (5);
				num_e = 2 + random.Next (5);
				order = num_a * num_b + num_c *  num_d - num_e;
				sequential = ((num_a * num_b + num_c) * num_d) - num_e;

				if (order != sequential)
					found = true;
			}

			formula = String.Format ("{0} * {1} + {2} * {3} - {4} = ?", num_a, num_b, num_c, num_d, num_e);
			Answer.Correct = (order).ToString ();
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.Draw (gr, area_width, area_height, rtl);

			gr.SetPangoLargeFontSize ();
			gr.DrawTextCentered (0.5, DrawAreaY + 0.3, formula);
		}
	}
}
