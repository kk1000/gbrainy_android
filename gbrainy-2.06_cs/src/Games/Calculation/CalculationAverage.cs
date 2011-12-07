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
using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Games.Calculation
{
	public class CalculationAverage : Game
	{
		const int options_cnt = 4;
		const int correct_pos = 0;
		double []numbers;
		double []options;
		ArrayListIndicesRandom random_indices;
		double correct;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Average");}
		}

		public override GameTypes Type {
			get { return GameTypes.Calculation;}
		}

		public override string Question {
			get {
				string nums = string.Empty;

				for (int i = 0; i < numbers.Length - 1; i++)
					nums += numbers[i] + ", ";

				nums += numbers [numbers.Length - 1];

				return String.Format (
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Given the numbers: {0}. Which of the following numbers is closest to the average? Answer {1}, {2}, {3} or {4}."), nums,
					Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3));}
		}

		public override string Tip {
			get { return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The average of a list of numbers is their sum divided by the number of numbers in the list.");}
		}

		public override string Rationale {
			get {
				return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The result of the operation is {0}."),
					correct);
			}
		}

		protected override void Initialize ()
		{
			bool duplicated;
			int nums, options_next, dist, num_size, which = 0;

			Answer.CheckAttributes |= GameAnswerCheckAttributes.MultiOption;
			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
				nums = 3;
				dist = nums * 3;
				num_size = 50;
				break;
			default:
			case GameDifficulty.Medium:
				nums = 5;
				dist = nums * 3;
				num_size = 150;
				break;
			case GameDifficulty.Master:
				nums = 7;
				dist = nums * 3;
				num_size = 500;
				break;
			}

			numbers = new double [nums];
			options = new double [options_cnt];

			do
			{
				// Random set of numbers
				correct = 0;
				for (int i = 0; i < nums; i++)
				{
					numbers [i] = 10 + random.Next (num_size) + dist;
					correct += numbers [i];
				}

				correct = correct / nums;

			} while (correct != Math.Truncate (correct));

			options [correct_pos] = correct;
			options_next = correct_pos + 1;

			// Generate possible answers
			while (options_next < options_cnt) {
				double ans;

				ans = correct + random.Next (dist);
				duplicated = false;

				// No repeated answers
				for (int num = 0; num < options_next; num++)
				{
					// Due to decimal precision
					if (options [num] == ans || options [num] == ans + 1 || options [num] == ans - 1) {
						duplicated = true;
						break;
					}
				}

				if (duplicated)
					continue;

				options [options_next] = ans;
				options_next++;
			}

			random_indices = new ArrayListIndicesRandom (options_cnt);
			random_indices.Initialize ();

			for (int i = 0; i < options_cnt; i++)
			{
				if (random_indices [i] == correct_pos) {
					which = i;
					break;
				}
			}

			Answer.SetMultiOptionAnswer (which, options [correct_pos].ToString ());

			// Options
			double x = DrawAreaX + 0.25, y = DrawAreaY + 0.16;
			Container container = new Container (x, y,  1 - (x * 2), 0.6);
			AddWidget (container);

			for (int i = 0; i < options_cnt; i++)
			{
				DrawableArea drawable_area = new DrawableArea (0.3, 0.1);
				drawable_area.X = x;
				drawable_area.Y = y + i * 0.15;
				container.AddChild (drawable_area);
				drawable_area.Data = i;
				drawable_area.DataEx = Answer.GetMultiOption (i);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int n = (int) e.Data;
					int indx = random_indices[n];

					e.Context.SetPangoLargeFontSize ();
					e.Context.MoveTo (0.02, 0.02);
					e.Context.ShowPangoText (String.Format ("{0}) {1}", Answer.GetMultiOption (n) , options [indx]));
				};
			}
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.Draw (gr, area_width, area_height, rtl);

			gr.SetPangoLargeFontSize ();

			gr.MoveTo (0.1, 0.15);
			gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Choose one of the following:"));
		}
	}
}
