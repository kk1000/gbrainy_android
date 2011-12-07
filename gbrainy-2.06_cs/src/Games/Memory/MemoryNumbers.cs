/*
 * Copyright (C) 2008-2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

using System.Text;

using gbrainy.Core.Main;
using gbrainy.Core.Services;

namespace gbrainy.Games.Memory
{
	public class MemoryNumbers : Core.Main.Memory
	{
		private Challenge current_game;
		private const int num_games = 3;

		class Challenge
		{
			protected static int [] numbers;

			public Challenge () {}

			public static int[] Numbers {
				set { numbers = value;}
				get { return numbers;}
			}

			virtual public string Question {
				get {return string.Empty; }
			}

			virtual public string Answer {
				get {return string.Empty; }
			}
		}

		class ChallengeOdds : Challenge
		{
			public override string Question {
				get {
					return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("How many odd numbers were in the previous image? Answer using numbers.");
				}
			}

			public override string Answer {
				get {
					int odds = 0;
					for (int i = 0; i < numbers.Length; i++) {
						if (numbers[i] % 2 != 0)
							odds++;
					}
					return odds.ToString ();
				}
			}
		}

		class ChallengeEvens : Challenge
		{
			public override string Question {
				get {
					return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("How many even numbers were in the previous image? Answer using numbers.");
				}
			}

			public override string Answer {
				get {
					int evens = 0;
					for (int i = 0; i < numbers.Length; i++) {
						if (numbers[i] % 2 == 0)
							evens++;
					}
					return evens.ToString ();
				}
			}
		}

		class ChallengeTwoDigits : Challenge
		{
			public override string Question {
				get {
					return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("How many numbers with more than one digit were in the previous image? Answer using numbers.");
				}
			}

			public override string Answer {
				get {
					int digits = 0;
					for (int i = 0; i < numbers.Length; i++) {
						if (numbers[i] > 9)
							digits++;
					}
					return digits.ToString ();
				}
			}
		}

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString  ("Memorize numbers");}
		}

		public override string MemoryQuestion {
			get { return current_game.Question; }
		}

		protected override void Initialize ()
		{
			base.Initialize ();
			int total;

			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
				total = 5;
				break;
			case GameDifficulty.Medium:
			default:
				total = 7;
				break;
			case GameDifficulty.Master:
				total = 9;
				break;
			}

			int[] nums = new int [total];

			for (int i = 0; i < nums.Length; i++)
			{
				nums[i] = GetUniqueRandomNumber (nums);
			}

			switch (random.Next (num_games)) {
			case 0:
				current_game = new ChallengeOdds ();
				break;
			case 1:
				current_game = new ChallengeEvens ();
				break;
			case 2:
				current_game = new ChallengeTwoDigits ();
				break;
			}

			Challenge.Numbers = nums;
			Answer.Correct = current_game.Answer;
		}

		// Generate a random number that is unique at the numbers array
		int GetUniqueRandomNumber (int [] numbers)
		{
			int candidate;
			bool unique = true;

			do
			{
				candidate = 1 + random.Next (15);
				unique = true;
				for (int i = 0; i < numbers.Length; i++)
				{
					if (numbers[i] == candidate)
					{
						unique = false;
						break;
					}
				}
			} while (unique == false);

			return candidate;
		}

		public override void DrawObjectToMemorize (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			StringBuilder sequence = new StringBuilder (64);

			base.DrawObjectToMemorize (gr, area_width, area_height, rtl);
			gr.SetPangoLargeFontSize ();

			for (int num = 0; num < Challenge.Numbers.Length - 1; num++)
			{
				sequence.Append (Challenge.Numbers [num]);
				sequence.Append (", ");
			}
			sequence.Append (Challenge.Numbers [Challenge.Numbers.Length - 1]);

			gr.DrawTextCentered (0.5, DrawAreaY + 0.3, sequence.ToString ());

		}
	}
}
