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
	public class PuzzleTrains : Game
	{
		enum GameType
		{
#if _DISABLED___FOR_663259
			//Overtake - Temporary removed as part of fixing #663259 in stable
#endif
			Meet,
			Apart,
			Total
		}

		string question, answer;
		GameType gametype;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Trains");}
		}

		public override string Question {
			get {return question; }
		}

		public override string Rationale {
			get { return answer;}
		}

		public override string Tip {
			get { return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The distance formula is 'distance = rate x time'.");}
		}

		protected override void Initialize ()
		{
			int ans;
			double speed_a, speed_b, rslt;

			gametype = (GameType) random.Next ((int) GameType.Total);

			switch (gametype)
			{
#if _DISABLED___FOR_663259
			case GameType.Overtake:
			{
				double hours;

				do
				{
					speed_a = 20 + (5 * random.Next (5));
					speed_b = 50 + (5 * random.Next (5));
					hours = 2 * (1 + random.Next (3));
					rslt = speed_a * hours / (speed_b - speed_a);

				}  while (rslt != Math.Truncate (rslt));

				question = String.Format (
					// Translators:
					//  - mph (miles per hour). You must localize this using the right unit of speed for your locale
					/// - The translated string should not use more characters than the original sentence
					ServiceLocator.Instance.GetService <ITranslations> ().GetPluralString ("A train leaves the station traveling at {0} mph. {1} hour later a second train leaves the station traveling in the same direction at {2} mph. How many hours since the first train left does it take the second train to overtake the first train?",
						"A train leaves the station traveling at {0} mph. {1} hours later a second train leaves the station traveling in the same direction at {2} mph. How many hours since the first train left does it take the second train to overtake the first train?", (int) hours),
						speed_a, hours, speed_b);
			
				answer = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("You can calculate the answer by multiplying the speed of the first train by the time and dividing it by the difference of speeds.");
				break;
			}
#endif
			case GameType.Meet:
			{
				double distance, hours;

				speed_a = 20 + (5 * random.Next (5));
				speed_b = 50 + (5 * random.Next (5));
				hours = 2 + random.Next (5);
				distance = hours * (speed_b + speed_a);
				rslt = hours;

				question = String.Format (
					// Translators:
					//  - mph (miles per hour) and miles must be localized this using the right unit of speed for your locale
					ServiceLocator.Instance.GetService <ITranslations> ().GetPluralString ("Two trains separated by {0} mile are heading towards each other on straight parallel tracks. One travels at {1} mph and the other at {2} mph. In how many hours will they meet?",
						"Two trains separated by {0} miles are heading towards each other on straight parallel tracks. One travels at {1} mph and the other at {2} mph. In how many hours will they meet?",
						(int) distance),
						distance, speed_a, speed_b);

				answer = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("You can calculate the answer by dividing the distance by the sum of both speeds.");
				break;
			}
			case GameType.Apart:
			{
				double distance;

				speed_a = 10 + (2 * random.Next (5));
				speed_b = 20 + (2 * random.Next (5));
				distance = (speed_a + speed_b) * (2 + random.Next (5));

				// Time in hours
				rslt = distance / (speed_a + speed_b);

				question = String.Format (
					// Translators:
					//  - mph (miles per hour) and miles must be localized this using the right unit of speed for your locale
					ServiceLocator.Instance.GetService <ITranslations> ().GetPluralString ("Two trains on straight parallel tracks leave from the same point and time traveling in opposite directions at {0} and {1} mph respectively. In how many hours will they be {2} mile apart?",
						"Two trains on straight parallel tracks leave from the same point and time traveling in opposite directions at {0} and {1} mph respectively. In how many hours will they be {2} miles apart?",
						(int) distance),
						speed_a, speed_b, distance);

				answer = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("You can calculate the answer by dividing the distance by the sum of both speeds.");
				break;
			}
			default:
				throw new Exception ("Unexpected value");
			}

			ans = (int) rslt;
			Answer.Correct = (ans).ToString ();
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			string train_right = "train_right.svg";
			string train_left = "train_left.svg";

			base.Draw (gr, area_width, area_height, rtl);

			switch (gametype)
			{
#if _DISABLED___FOR_663259
			case GameType.Overtake:
				gr.DrawImageFromAssembly (train_right, 0, 0.1, 0.5, 0.55);
				gr.MoveTo (0.1, 0.45);
				gr.LineTo (0.9, 0.45);
				gr.Stroke ();

				gr.DrawImageFromAssembly (train_right, 0.5, 0.1, 0.5, 0.55);
				break;
#endif
			case GameType.Meet:
				gr.DrawImageFromAssembly (train_right, 0, 0.1, 0.5, 0.55);
				gr.MoveTo (0.1, 0.45);
				gr.LineTo (0.9, 0.45);
				gr.Stroke ();

				gr.DrawImageFromAssembly (train_left, 0.55, 0.3, 0.5, 0.55);
				gr.MoveTo (0.1, 0.65);
				gr.LineTo (0.9, 0.65);
				gr.Stroke ();
				break;

			case GameType.Apart:
				gr.DrawImageFromAssembly (train_right, 0.35, 0.1, 0.5, 0.55);
				gr.MoveTo (0.1, 0.45);
				gr.LineTo (0.9, 0.45);
				gr.Stroke ();

				gr.DrawImageFromAssembly (train_left, 0.15, 0.3, 0.5, 0.55);
				gr.MoveTo (0.1, 0.65);
				gr.LineTo (0.9, 0.65);
				gr.Stroke ();
				break;
			default:
				throw new Exception ("Unexpected value");
			}
		}

		public override bool CheckAnswer (string answer)
		{
			string num = string.Empty;

			// Clean the answer from every non-numeric values after the numbers (like 5 hours)
			for (int c = 0; c < answer.Length; c++)
			{
				if (answer[c] < '0' || answer[c] > '9')
					break;

				num += answer[c];
			}

			return base.CheckAnswer (num);
		}
	}
}
