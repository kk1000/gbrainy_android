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
using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Games.Calculation
{
	public class CalculationGreatestDivisor : Game
	{
		private int []numbers;
		private int []answers;
		private int max_num;
		private int num_answ_ques;
		private int answer_idx;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Greatest divisor");}
		}

		public override GameTypes Type {
			get { return GameTypes.Calculation;}
		}

		public override string Question {
			get { return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString (
				"Which of the possible divisors is the greatest that divides all numbers? Answer {0}, {1}, {2} or {3}."),
				Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3));
			}
		}

		protected override void Initialize ()
		{
			bool found;
			int n, m;
			int []mult = new int [3];

			Answer.CheckAttributes |= GameAnswerCheckAttributes.MultiOption;
			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
				max_num = 999;
				num_answ_ques = 3;
				break;
			case GameDifficulty.Medium:
				max_num = 999;
				num_answ_ques = 4;
				break;
			case GameDifficulty.Master:
				max_num = 9999;
				num_answ_ques = 4;
				break;
			}

			numbers = new int [num_answ_ques];
			answers = new int [num_answ_ques];

			// Common multipliers for all numbers
			for (m = 0; m < mult.Length; m++) {
				mult[m] = GetMultiplier (mult);
			}

			n = 0;
			while (n < numbers.Length) {
				numbers [n] = 4 + random.Next (5);
				for (int i = 1; i < 5; i++) {
					numbers [n] =  numbers [n]  * (1 + random.Next (10));
				}

				for (m = 0; m < mult.Length; m++) {
					numbers[n] = numbers [n] * mult[m];
				}

				if (numbers[n] > max_num || numbers[n] < 50)
					continue;

				found = false;
				for (int i = 0; i < n; i++) {
					if (numbers[i]  == numbers [n]) {
						found = true;
						break;
					}
				}
				if (found == false) {
					n++;
				}
			}

			int answer = 0;
			// Avoid generating a set of answers in which none matches the condition
			while (answer == 0)
			{
				// Build a list of answers
				for (n = 0; n < answers.Length; n++) {
					answers[n] = GetUniqueAnswer (mult, answers);
				}

				n = 0;
				for (int a = 0; a < answers.Length; a++)
				{
					for (n = 0; n < answers.Length; n++)
					{
						if ((double)numbers[n] / (double)answers[a] !=  Math.Abs (numbers[n] / answers[a]))
							break;
					}

					if (n == answers.Length && answers[a] > answer)
					{
						answer = answers[a];
						answer_idx = a;
					}
				}
			}

			Answer.SetMultiOptionAnswer (answer_idx, answer.ToString ());

			// Drawing objects
			Container container = new Container (DrawAreaX + 0.2, DrawAreaY + 0.25, 0.4, answers.Length * 0.15);
			AddWidget (container);

			for (int i = 0; i < answers.Length; i++)
			{
				DrawableArea drawable_area = new DrawableArea (0.3, 0.1);
				drawable_area.X = DrawAreaX + 0.23;
				drawable_area.Y = DrawAreaY + 0.27 + i * 0.15;
				container.AddChild (drawable_area);
				drawable_area.Data = i;
				drawable_area.DataEx = Answer.GetMultiOption (i);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int d = (int) e.Data;
					e.Context.SetPangoLargeFontSize ();
					e.Context.MoveTo (0.07, 0.02);
					e.Context.ShowPangoText (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("{0}) {1}"), Answer.GetMultiOption (d),
						answers[d].ToString ()));
				};
			}
		}

		private int GetUniqueAnswer (int []mult, int []answers)
		{
			int answer = 0;
			bool found = false;
			int n;

			while (found == false) {
				switch (random.Next (7)) {
				case 0:
					answer = mult[0];
					break;
				case 1:
					answer = mult[0] * mult[1];
					break;
				case 2:
					answer = mult[0] * mult[2];
					break;
				case 3:
					answer = mult[0] * 7;
					break;
				case 4:
					answer = mult[0] * 13;
					break;
				case 5:
					answer = mult[0] * mult[1] * mult[2];
					break;
				case 6:
					answer = mult[0] * 19;
					break;
				}

				for (n = 0; n < answers.Length; n++) {
					if (answers [n] == answer)
						break;
				}

				if (n == answers.Length)
					found = true;
			}

			return answer;
		}

		private int GetMultiplier (int []nums)
		{
			int rslt = 1;
			bool found = false;
			int n;

			while (found == false) {
				switch (random.Next (4)) {
				case 0:
					rslt = 2;
					break;
				case 1:
					rslt = 3;
					break;
				case 2:
					rslt = 5;
					break;
				case 3:
					rslt = 7;
					break;

				}
				for (n = 0; n < nums.Length; n++) {
					if (nums[n] == rslt)
						break;
				}

				if (n == nums.Length)
					found = true;
			}

			return rslt;
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = DrawAreaX, y = 0.05;

			base.Draw (gr, area_width, area_height, rtl);

			gr.MoveTo (0.05, y);
			gr.SetPangoLargeFontSize ();
			gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Numbers"));
			y += 0.08;

			for (int n = 0; n < numbers.Length; n++)
			{
				gr.MoveTo (x, y);
				gr.ShowPangoText (numbers[n].ToString ());
				gr.Stroke ();
				x += 0.17;
			}

			y += 0.16;

			gr.MoveTo (0.05, y);
			gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Possible divisors"));
		}
	}
}
