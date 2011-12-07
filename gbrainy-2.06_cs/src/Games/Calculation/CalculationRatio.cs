/*
 * Copyright (C) 2009 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

namespace gbrainy.Games.Calculation
{
	public class CalculationRatio : Game
	{
		int number_a, number_b, ratio_a, ratio_b;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Ratio");}
		}

		public override GameTypes Type {
			get { return GameTypes.Calculation;}
		}

		public override string Question {
			get {
				return String.Format (
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Which two numbers have a sum of {0} and have a ratio of {1} to {2}. Answer using two numbers (e.g.: 1 and 2)."), 
					number_a + number_b, ratio_a, ratio_b);
			}
		}

		public override string Rationale {
			get {
				return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The second number can be calculated by multiplying the first number by {0} and dividing it by {1}."),
					ratio_b, ratio_a);
			}
		}

		public override string Tip {
			get { return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("A ratio specifies a proportion between two numbers. A ratio of a:b means that for every 'a' parts you have 'b' parts.");}
		}

		protected override void Initialize ()
		{
			int random_max;
		
			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
				random_max = 5;
				break;
			default:
			case GameDifficulty.Medium:
				random_max = 8;
				break;
			case GameDifficulty.Master:
				random_max = 15;
				break;
			}

			number_a = 10 + random.Next (random_max);

			if (number_a % 2 !=0)
				number_a++;
		
			ratio_a = 2;
			ratio_b = 3 + random.Next (random_max);
			number_b = number_a / ratio_a * ratio_b;

			Answer.Correct = String.Format ("{0} | {1}", number_a, number_b);
			Answer.CheckExpression = "[0-9]+";
			Answer.CheckAttributes |= GameAnswerCheckAttributes.MatchAll;
			Answer.CorrectShow = String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("{0} and {1}"), number_a, number_b);
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{	
			double x = DrawAreaX + 0.1;

			base.Draw (gr, area_width, area_height, rtl);

			gr.SetPangoLargeFontSize ();

			gr.MoveTo (x, DrawAreaY + 0.22);
			gr.ShowPangoText (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("x + y = {0}"), number_a + number_b));
		
			gr.MoveTo (x, DrawAreaY + 0.44);
			gr.ShowPangoText (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("have a ratio of {0}:{1}"), ratio_a, ratio_b));
		}
	}
}
