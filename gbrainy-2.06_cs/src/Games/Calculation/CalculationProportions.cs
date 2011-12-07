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
	public class CalculationProportions : Game
	{
		const int options_cnt = 4;
		const int correct_pos = 0;
		double []options;
		ArrayListIndicesRandom random_indices;
		double num, den, percentage, correct;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Proportions");}
		}

		public override GameTypes Type {
			get { return GameTypes.Calculation;}
		}

		public override string Question {
			get {
				return String.Format (
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("What is {0}% of {1}/{2}? Answer {3}, {4}, {5} or {6}."), 
					percentage, num, den, Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3));}
		}

		protected override void Initialize ()
		{
			int options_next, random_max, which = 0;
		
			Answer.CheckAttributes |= GameAnswerCheckAttributes.MultiOption;
			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
				random_max = 30;
				break;
			default:
			case GameDifficulty.Medium:
				random_max = 50;
				break;
			case GameDifficulty.Master:
				random_max = 80;
				break;
			}

			do {
				// Fraction
				num = 10 + random.Next (random_max);
				den = 2 + random.Next (random_max / 5);
				percentage = 10 + random.Next (random_max) ;
				correct = percentage / 100d * num / den;
			} while (correct < 1);

			options = new double [options_cnt];

			options_next = 0;
			options [options_next++] = correct;
			options [options_next++] = percentage / 70d * num / den;
			options [options_next++] = percentage / 120d * num / den;
			options [options_next++] = percentage / 150d * num / den;

			random_indices = new ArrayListIndicesRandom (options_cnt);
			random_indices.Initialize ();
		
			for (int i = 0; i < options_cnt; i++)
			{
				if (random_indices [i] == correct_pos) {
					which = i;
					break;
				}
			}

			Answer.SetMultiOptionAnswer (which, String.Format ("{0:##0.##}", options[which]));

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
					e.Context.ShowPangoText (String.Format ("{0}) {1:##0.##}", Answer.GetMultiOption (n), options [indx]));
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
