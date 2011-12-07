/*
 * Copyright (C) 2007-2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
	public class CalculationTwoNumbers : Game
	{
		int number_a, number_b;
		int op1, op2, max_operand;
		SubGameTypes type;

		enum SubGameTypes
		{
			Addition,
			Subtraction,
			Length
		};

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Two numbers");}
		}

		public override GameTypes Type {
			get { return GameTypes.Calculation;}
		}

		public override string Question {
			get {
				switch (type) {
				case SubGameTypes.Addition:
					return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Which two numbers when added are {0} and when multiplied are {1}? Answer using two numbers (e.g.: 1 and 2)."), op1, op2);
				case SubGameTypes.Subtraction:
					return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Which two numbers when subtracted are {0} and when multiplied are {1}? Answer using two numbers (e.g.: 1 and 2)."), op1, op2);
				default:
					throw new InvalidOperationException ();
				}
			}
		}

		protected override void Initialize ()
		{
			Answer.CheckAttributes |= GameAnswerCheckAttributes.MatchAll;
			type = (SubGameTypes) random.Next ((int) SubGameTypes.Length);

			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
				max_operand = 8;
				break;
			case GameDifficulty.Medium:
				max_operand = 10;
				break;
			case GameDifficulty.Master:
				max_operand = 15;
				break;
			}

			number_a = 5 + random.Next (max_operand);
			number_b = 3 + random.Next (max_operand);

			switch (type) {
			case SubGameTypes.Addition:
				op1 = number_a + number_b;
				break;
			case SubGameTypes.Subtraction:
				if (number_a < number_b) {
					int tmp = number_a;

					number_a = number_b;
					number_b = tmp;
				}
				op1 = number_a - number_b;
				break;
			default:
				throw new InvalidOperationException ();
			}

			op2 = number_a * number_b;
			Answer.Correct = String.Format ("{0} | {1}", number_a, number_b);
			Answer.CheckExpression = "[-0-9]+";
			Answer.CorrectShow = String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("{0} and {1}"), number_a, number_b);
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = DrawAreaX + 0.25;

			base.Draw (gr, area_width, area_height, rtl);

			gr.SetPangoLargeFontSize ();
			gr.MoveTo (x, DrawAreaY + 0.22);

			switch (type) {
			case SubGameTypes.Addition:
				gr.ShowPangoText (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("x + y = {0}"), op1));
				break;
			case SubGameTypes.Subtraction:
				gr.ShowPangoText (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("x - y = {0}"), op1));
				break;
			default:
				throw new InvalidOperationException ();
			}

			gr.MoveTo (x, DrawAreaY + 0.44);
			gr.ShowPangoText (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("x * y = {0}"), op2));
		}

		public override bool CheckAnswer (string answer)
		{
			if (base.CheckAnswer (answer) == true)
				return true;

			// Support case: -b - (-a) / (-b) * (-a)
			if (type == SubGameTypes.Subtraction) {
				int num_a, num_b;
	
				num_a = number_a;
				num_b = number_b;
				number_a = -num_b;
				number_b = -num_a;

				Answer.Correct = String.Format ("{0} | {1}", number_a, number_b);
				Answer.CheckAttributes &= ~GameAnswerCheckAttributes.MatchAll;
				Answer.CheckAttributes |= GameAnswerCheckAttributes.MatchAllInOrder;
				if (base.CheckAnswer (answer) == true) 
					return true;

				number_a = num_a;
				number_b = num_b;
			}
			return false;
		}
	}
}
