/*
 * Copyright (C) 2007-2008 Brandon Perry <bperry.volatile@gmail.com>
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

using System;

using gbrainy.Core.Main;
using gbrainy.Core.Services;

namespace gbrainy.Games.Calculation
{
	public class CalculationOperator : Game
	{
		private double number_a, number_b, number_c, total;
		private readonly char[] opers = {'*', '+', '-', '/'};
		private char oper1, oper2;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Operators");}
		}

		public override string Tip {
			get {return String.Format( ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The first operator is {0}."), oper1);}
		}

		public override GameTypes Type {
			get { return GameTypes.Calculation;}
		}

		public override string Question {
			get {return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Which operators make {0}, {1}, and {2} equal {3}? Answer using '+-/*'."), number_a, number_b, number_c, total);}
		}

		static private double ProcessOperation (double total, double number, char op)
		{
			switch (op) {
			case '-':
				return total - number;
			case '*':
				return total * number;
			case '/':
				return total / number;
			case '+':
			default:
				return total + number;
			}
		}

		protected override void Initialize ()
		{
			while (true)
			{
				number_a = 5 + random.Next (1000);
				number_b = 3 + random.Next (1000);
				number_c = 7 + random.Next (1000);

				oper1 = opers[random.Next (opers.Length)];
				oper2 = opers[random.Next (opers.Length)];

				total = ProcessOperation (number_a, number_b, oper1);
				total = ProcessOperation (total, number_c, oper2);

				if ((total < 20000 && total > -20000 && total != 0 && total == (int) total) == false)
					continue;

				// Make sure that the operation cannot be accomplished with other operators too
				// For example, if number b = number c (+ and -) and (* and /) are valid
				double rslt;
				bool other_ops = false;
				for (int op1 = 0; op1 < opers.Length; op1++)
				{
					for (int op2 = 0; op2 < opers.Length; op2++)
					{
						if (opers[op1] == oper1 && opers[op2] == oper2)
							continue;

						rslt = ProcessOperation (number_a, number_b, opers[op1]);
						rslt = ProcessOperation (rslt, number_c, opers[op2]);

						if (rslt == total) {
							other_ops = true;
							break;
						}
					}
				}

				if (other_ops == false)
					break;
			}

			Answer.Correct = String.Format ("{0} | {1}", oper1, oper2);
			Answer.CheckExpression = "[+*-/]";
			Answer.CheckAttributes |= GameAnswerCheckAttributes.MatchAll;
			Answer.CorrectShow = String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("{0} and {1}"), oper1, oper2);
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			const double aligned_pos = 0.58;

			base.Draw (gr, area_width, area_height, rtl);

			gr.SetPangoLargeFontSize ();
			gr.DrawTextAlignedRight (aligned_pos, DrawAreaY + 0.2, number_a.ToString ());
			gr.DrawTextAlignedRight (aligned_pos, DrawAreaY + 0.3, number_b.ToString ());
			gr.DrawTextAlignedRight (aligned_pos, DrawAreaY + 0.4, number_c.ToString ());

			gr.MoveTo (DrawAreaX + 0.2, DrawAreaY + 0.5);
			gr.LineTo (DrawAreaX + 0.5, DrawAreaY + 0.5);
			gr.Stroke ();

			gr.DrawTextAlignedRight (aligned_pos, DrawAreaY + 0.55, total.ToString ());

			gr.MoveTo (DrawAreaX + 0.2, DrawAreaY + 0.25);
			gr.ShowPangoText ((Answer.Draw == true) ? oper1.ToString () : "?");

			gr.MoveTo (DrawAreaX + 0.2, DrawAreaY + 0.35);
			gr.ShowPangoText ((Answer.Draw == true) ?  oper2.ToString () : "?");

		}
	}
}
