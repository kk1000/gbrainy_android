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
using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Games.Calculation
{
	public class CalculationCloserFraction : Game
	{
		private double question_num;
		private const int options_cnt = 4;
		private double []options;
		private ArrayListIndicesRandom random_indices;
		private int which;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Closer fraction");}
		}

		public override GameTypes Type {
			get { return GameTypes.Calculation;}
		}

		public override string Question {
			get {return String.Format (
				ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Which of the following numbers is closer to {0}? Answer {1}, {2}, {3} or {4}."),
				String.Format ("{0:##0.###}", question_num),
				Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3));}
		}

		public override string Rationale {
			get {
				int ans_idx = random_indices[which];

				return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The result of the operation {0} / {1} is {2}"),
					options[ans_idx * 2], options[(ans_idx * 2) + 1], String.Format ("{0:##0.###}", question_num));
			}
		}

		protected override void Initialize ()
		{
			Answer.CheckAttributes |= GameAnswerCheckAttributes.MultiOption | GameAnswerCheckAttributes.IgnoreSpaces;
			options = new double [options_cnt * 2];
			bool duplicated;
			bool done = false;
			int i, ans_idx, basenum, randnum;

			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
				basenum = 5;
				randnum = 10;
				break;
			default:
			case GameDifficulty.Medium:
				basenum = 5;
				randnum = 30;
				break;
			case GameDifficulty.Master:
				basenum = 9;
				randnum = 60;
				break;
			}

			while (done == false) {
				duplicated = false;
				options[0 + 0] = basenum + random.Next (randnum);
				options[0 + 1] = basenum + random.Next (randnum);

				options[2 + 0] = options[0 + 0] + random.Next (2);
				options[2 + 1] = options[0 + 1] + random.Next (2);

				options[(2 * 2) + 0] = options[0 + 0] - random.Next (5);
				options[(2 * 2) + 1] = options[0 + 1] - random.Next (5);

				options[(3 * 2) + 0] = options[0 + 0] +  random.Next (5);
				options[(3 * 2) + 1] = options[0 + 1] +  random.Next (5);

				// No repeated answers
				for (int num = 0; num < options_cnt; num++)
				{
					for (int n = 0; n < options_cnt; n++)
					{
						if (n == num) continue;

						if (options [(num * 2) + 0] / options [(num * 2) + 1] ==
							options [(n * 2) + 0] / options [(n * 2) + 1]) {
							duplicated = true;
							break;
						}
					}
				}

				if (duplicated)
					continue;

				// No numerator = denominator (1 value)
				if (options [0 + 0] == options [0 + 1]) continue;
				if (options [(1 * 2) + 0] == options [(1 * 2) + 1]) continue;
				if (options [(2 * 2) + 0] == options [(2 * 2) + 1]) continue;
				if (options [(3 * 2) + 0] == options [(3 * 2) + 1]) continue;

				// No < 2
				for (i = 0; i < options_cnt * 2; i++) {
					if (options [i] < 2)
						break;
				}

				if (i < options_cnt * 2)
					continue;

				done = true;
			}

			random_indices = new ArrayListIndicesRandom (4);
			random_indices.Initialize ();

			which = random.Next (options_cnt);
			ans_idx = random_indices[which];
			question_num = options[ans_idx * 2] / options[(ans_idx * 2) + 1];

			Answer.SetMultiOptionAnswer (which, options [ans_idx * 2] +  " / " + options [(ans_idx  * 2) +1]);

			// Options
			double x = DrawAreaX + 0.25, y = DrawAreaY + 0.16;
			Container container = new Container (x, y,  1 - (x * 2), 0.6);
			AddWidget (container);

			for (i = 0; i < options_cnt; i++)
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
					e.Context.ShowPangoText (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("{0}) {1}"), Answer.GetMultiOption (n) ,
						options [indx * 2] +  " / " + options [(indx  * 2) +1]));
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
