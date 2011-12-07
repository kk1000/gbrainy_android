/*
 * Copyright (C) 2008-2011 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.Linq;

using gbrainy.Core.Main;
using gbrainy.Core.Services;

namespace gbrainy.Games.Calculation
{
	public class CalculationFractions : Game
	{
		public enum Operation
		{
			Addition,
			Subtraction,
			LastOperation
		}

		class FormulaFraction
		{
			public Operation Operation { get; set;}
			public int Numerator { get; private set;}
			public int Denominator { get; private set;}

			public FormulaFraction (int numerator, int denominator, Operation operation)
			{
				Numerator = numerator;
				Denominator = denominator;
				Operation = operation;
			}

			public double Result {
				get {
					return (double) Numerator / (double) Denominator;
				}
			}
		}

		int fractions_num, demominator_max, factor_max;
		FormulaFraction[] fractions;
		const string format_string = "{0:##0.###}";

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Fractions");}
		}

		public override GameTypes Type {
			get { return GameTypes.Calculation;}
		}

		public override string Question {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("What is the result of the given operation? Answer using either a fraction or a number.");}
		}

		private int Factor {
			get {
				switch (random.Next (factor_max)) {
				case 0:
				default:
					return 2;
				case 1:
					return 3;
				case 2:
					return 5;
				case 3:
					return 7;
				}
			}
		}

		protected override void Initialize ()
		{
			double rslt = 0;
			int factor = Factor;

			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
				fractions_num = 2;
				demominator_max = 3;
				factor_max = 2;
				break;
			default:
			case GameDifficulty.Medium:
				fractions_num = 4;
				demominator_max = 3;
				factor_max = 3;
				break;
			case GameDifficulty.Master:
				fractions_num = 4;
				demominator_max = 5;
				factor_max = 4;
				break;
			}

			fractions = new FormulaFraction [fractions_num];
			for (int i = 0; i < fractions_num; i++)
			{
				fractions[i] = new FormulaFraction (1 + random.Next (10), (1 + random.Next (demominator_max)) * factor,
					(Operation) random.Next ((int) Operation.LastOperation));

				if (i == 0)
					fractions[0].Operation = Operation.LastOperation; // No operation

				switch (fractions[i].Operation) {
				case Operation.Addition:
					rslt += fractions[i].Result;
					break;
				case Operation.Subtraction:
					rslt -= fractions[i].Result;
					break;
				default:
					rslt = fractions[i].Result;
					break;
				}
			}

			Answer.Correct = String.Format (format_string, rslt);
			Answer.CorrectShow = AnswerAsFraction ();
		}

		string AnswerAsFraction ()
		{
			int []den = new int [fractions.Length];
			int rslt, lcm;

			for (int i =0; i < fractions.Length; i++)
				den [i] = fractions[i].Denominator;

			lcm = LCM (den);
			rslt = 0;
			for (int i =0; i < fractions.Length; i++)
			{
				switch (fractions[i].Operation) {
				case Operation.Addition:
					rslt +=  lcm / fractions[i].Denominator * fractions[i].Numerator;
					break;
				case Operation.Subtraction:
					rslt -= lcm / fractions[i].Denominator * fractions[i].Numerator;
					break;
				default:
					rslt = lcm / fractions[i].Denominator * fractions[i].Numerator;
					break;
				}
			}

			return String.Format ("{0} / {1}", rslt, lcm);
		}

		static int LCM (int[] integerSet)
		{
			return integerSet.Aggregate (LCM);
		}

		static int LCM (int a, int b)
		{
			for (int n=1; ;n++)
			{
				if (n%a == 0 && n%b == 0)
					return n;
			}
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			const double fraction_size = 0.17;
			double x =  0.5  - (fractions_num * fraction_size / 2), y = DrawAreaY + 0.3;
			const double offset_x = 0.12;

			base.Draw (gr, area_width, area_height, rtl);

			gr.SetPangoLargeFontSize ();

			for (int i = 0; i < fractions_num; i++)
			{
				// Numerator
				gr.DrawTextAlignedRight (x + offset_x, y, fractions[i].Numerator.ToString ());

				// Sign
				gr.MoveTo (x, y + 0.04);
				switch (fractions[i].Operation) {
				case Operation.Addition:
					gr.ShowPangoText ("+");
					break;
				case Operation.Subtraction:
					gr.ShowPangoText ("-");
					break;
				}
				gr.Stroke ();

				// Line
				gr.MoveTo (x + 0.05, y + 0.08);
				gr.LineTo (x + offset_x + 0.02,  y + 0.08);
				gr.Stroke ();

				// Denominator
				gr.DrawTextAlignedRight (x + offset_x, y + 0.1, fractions[i].Denominator.ToString ());

				x += fraction_size;
			}
		}

		public override bool CheckAnswer (string answer)
		{
			string num_a = string.Empty;
			string num_b = string.Empty;
			double a, b;
			double rslt;
			bool first = true;

			for (int c = 0; c < answer.Length; c++)
			{
				if (answer[c] < '0' || answer[c] > '9') {
					if (answer[c] != '-' && answer[c] != '.' && answer[c] != ',') {
						first = false;
						continue;
					}
				}

				if (first == true)
					num_a += answer[c];
				else
					num_b += answer[c];
			}

			try {

				if (num_b != string.Empty) {
					a = Double.Parse (num_a);
					b = Double.Parse (num_b);
					rslt = (double) a / (double) b;
				} else {
					rslt = Double.Parse (num_a);
				}

			}

			catch (FormatException) {
				return false;
			}
			return Answer.Correct.Equals (String.Format (format_string, rslt));
		}
	}
}
