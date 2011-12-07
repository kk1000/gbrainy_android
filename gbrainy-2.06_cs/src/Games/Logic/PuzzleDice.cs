/*
 * Copyright (C) 2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
	public class PuzzleDice : Game
	{
		int problem;
		const string format_string = "{0:##0.###}";

		struct Problem
		{
			internal string question;
			internal string answer;
			internal string rationale;
			internal bool single;

			internal Problem (string question, string answer, string rationale, bool single)
			{
				this.question = question;
				this.rationale = rationale;
				this.answer = answer;
				this.single = single;
			}
		};

		Problem [] problems;

		void LoadProblems ()
		{
			problems = new Problem []
			{
				new Problem (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("What is the probability of getting a '2' or a '6' in a single throw of an unmodified 6 sided die? Answer using a fraction (e.g.: 1/2)."),
					"1/3",
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("There are 2 of 6 possibilities."), true),

				new Problem (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("What is the probability of not getting a '5' in a single throw of an unmodified 6 sided die? Answer using a fraction (e.g.: 1/2)."),
					"5/6",
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("There are 5 of 6 possibilities."), true),

				new Problem (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Two unmodified 6 sided dice are thrown simultaneously. What is the probability of getting two even numbers? Answer using a fraction (e.g.: 1/2)."),
					"9/36",
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("There are 9 of 36 possibilities of getting two even numbers."), false),

				new Problem (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Two unmodified 6 sided dice are thrown simultaneously. What is the probability of getting two '6'? Answer using a fraction (e.g.: 1/2)."),
					"1/36",
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("There is 1 of 6 possibilities of getting a '6' on the first die and the same for the second die."), false),
			};
		}

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Dice");}
		}

		public override string Question {
			get { return problems[problem].question;}
		}

		public override string Rationale {
			get { return problems[problem].rationale; }
		}

		protected override void Initialize ()
		{
			if (problems == null)
				LoadProblems ();

			problem = random.Next (problems.Length);
			Answer.Correct = problems[problem].answer;
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.Draw (gr, area_width, area_height, rtl);
			
			if (problems[problem].single)
				gr.DrawImageFromAssembly ("dice.svg", 0.3, 0.3, 0.4, 0.4);
			else {
				gr.DrawImageFromAssembly ("dice.svg", 0.1, 0.3, 0.4, 0.4);
				gr.DrawImageFromAssembly ("dice.svg", 0.5, 0.3, 0.4, 0.4);
			}
		}

		public override bool CheckAnswer (string answer)
		{
			string ans;
			
			ans = String.Format (format_string, FractionOrNumberToDouble (problems[problem].answer));
			return ans.Equals (String.Format (format_string, FractionOrNumberToDouble (answer)));
		}

		// Takes a string with Fraction or Number and converts it to double
		static double FractionOrNumberToDouble (string answer)
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
				return 0;
			}
			return rslt;
		}
	}
}
