/*
 * Copyright (C) 2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
	public class PuzzleCountSeries : Game
	{
		enum GameType
		{
			HowManyNines,
			HowManySmallerDigits,
			HowManyBiggerDigits,
			Length		
		}

		private string question, rationale;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Count series");}
		}

		public override string Question {
			get {return question;} 
		}

		public override string Rationale {
			get {return rationale;} 
		}

		short []numbers;

		protected override void Initialize ()
		{
			GameType game_type;
		
			game_type = (GameType) random.Next ((int) GameType.Length);

			switch (game_type)
			{
				case GameType.HowManyNines:
					question = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("How many numbers '9' are required to represent the numbers between 10 to 100?");
					Answer.Correct = "19";
					numbers = new short [] {19, 29, 39, 49, 59, 69, 79, 89, 90, 91, 92 , 93, 94, 95, 96, 97, 98, 99};
					break;

				case GameType.HowManyBiggerDigits:
					question = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("How many two digit numbers occur where the first digit is larger than the second (e.g.: 20 and 21)?");
					Answer.Correct = "45";
					numbers = new short [] {10, 20, 21, 30, 31, 32, 40, 41, 42, 43, 50, 51, 52, 53, 54, 60, 61, 62, 63, 64, 65, 70, 71, 72, 73, 74, 75, 76, 80, 81, 82, 83, 84, 85, 86, 87, 90, 91, 92, 93, 94, 95, 96, 97, 98};
					break;

				case GameType.HowManySmallerDigits:
					question = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("How many two digit numbers occur where the first digit is smaller than the second (e.g.: 12 and 13)?");
					Answer.Correct = "36";
					numbers = new short [] {12, 13, 14, 15, 16, 17, 18, 19, 23, 24, 25, 26, 27, 28, 29, 34, 35, 36, 37, 38, 39, 45, 46, 47, 48, 49, 56, 57, 58, 59, 67, 68, 69, 78, 79, 89};
					break;
				default:
					throw new InvalidOperationException ("Invalid value");
			}

			rationale = numbers[0].ToString ();
			for (int i = 1; i < numbers.Length; i++)
			{
				// Translators: A sequence of numbers 1, 2, 3, etc.
				rationale = string.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("{0}, {1}"), rationale, numbers[i].ToString ());
			}

			rationale = string.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The numbers are: {0}."), rationale);

			if (game_type == GameType.HowManyNines) {
				// Translators: Concatenating two strings (rationale of answer + extra information).
				rationale = string.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("{0} {1}"), rationale,
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Notice that 99 contains two numbers '9'."));
			}				
		}
	}
}
