/*
 * Copyright (C) 2007-2011 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.Text.RegularExpressions;
using System.Text;

using gbrainy.Core.Services;

namespace gbrainy.Core.Main
{
	public class GameAnswer
	{
		static char separator = '|';
		const int MAX_POSSIBLE_ANSWER = 8;
		string correct;

		public GameAnswer ()
		{
			CheckAttributes = GameAnswerCheckAttributes.Trim | GameAnswerCheckAttributes.IgnoreCase;
			CheckExpression = ".+";
		}

		public char Separator {
			get { return separator; }
		}

		// Correct answer as shown to the user. Usually equals to Correct, but can be different
		// when the answer contains multiple options (e.g. 1 | 2 shown as 1 and 2).
		public string CorrectShow { get; set; }

		// This is the correct answer used for validating the answer (a | b)
		public string Correct {
			get { return correct;}
			set {
				correct = value;
				if (CorrectShow == null) // Set default answer to show
					CorrectShow = correct;
			}
		}

		public string CheckExpression { get; set; }
		public bool Draw { get; set; }

		public GameAnswerCheckAttributes CheckAttributes { get; set; }

		public string GetMultiOptionsExpression ()
		{
			StringBuilder str = new StringBuilder ();
			str.Append ("[");
			for (int i = 0; i < MAX_POSSIBLE_ANSWER; i++)
				str.Append (GetMultiOptionInternal (i));

			str.Append ("]");
			return str.ToString ();
		}
		
		// Index of the option (A, B) and answer (dog, cat)
		public void SetMultiOptionAnswer (int multioption, string answer)
		{
			if (String.IsNullOrEmpty (answer) == true)
				throw new InvalidOperationException ("Both options should be defined");

			string option = GetMultiOption (multioption);
			
			Correct = option + Separator + answer;
			CorrectShow = option;
		}

		public string GetMultiOption (int answer)
		{
			bool multioption;
			
			multioption = (CheckAttributes & GameAnswerCheckAttributes.MultiOption) == GameAnswerCheckAttributes.MultiOption;
			
			if (multioption == false)
				throw new InvalidOperationException ("Cannot call Multioption API if the game does not have the multioption attribute");	
		
			return GetMultiOptionInternal (answer);
		}

		string GetMultiOptionInternal (int answer)
		{			
			switch (answer) {
				// Translators Note
				// The following series of answers may need to be adapted
				// in cultures with alphabets different to the Latin one.
				// The idea is to enumerate a sequence of possible answers
				// For languages represented with the Latin alphabet use
				// the same than English
			case 0: // First possible answer for a series (e.g.: Figure A)
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("A");
			case 1: // Second possible answer for a series
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("B");
			case 2: // Third possible answer for a series
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("C");
			case 3: // Fourth possible answer for a series
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("D");
			case 4: // Fifth possible answer for a series
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("E");
			case 5: // Sixth possible answer for a series
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("F");
			case 6: // Seventh possible answer for a series
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("G");
			case 7: // Eighth possible answer for a series
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("H");
			default:
				throw new ArgumentOutOfRangeException ("Do not have an option for this answer");
			}
		}

		// A string of for format "A, B or C
		public string GetMultiOptionsPossibleAnswers (int num_answers)
		{
			switch (num_answers) {
			case 0:
			case 1:
				throw new InvalidOperationException ("You need more than 1 answer to select from");
			case 2:
				// Translators. This is the list of valid answers, like A or B.
				return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("{0} or {1}"),
					GetMultiOption (0), GetMultiOption (1));
			case 3:
				// Translators. This is the list of valid answers, like A, B or C.
				return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("{0}, {1} or {2}"),
					GetMultiOption (0), GetMultiOption (1), GetMultiOption (2));
			case 4:
				// Translators. This is the list of valid answers, like A, B, C or D.
				return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("{0}, {1}, {2} or {3}"),
					GetMultiOption (0), GetMultiOption (1), GetMultiOption (2), GetMultiOption (3));
			default:
				throw new InvalidOperationException ("Number of multiple options not supported");
			}
		}

		public string GetFigureName (int answer)
		{
			return String.Format (ServiceLocator.Instance.GetService <ITranslations> ()
				.GetString ("Figure {0}"), GetMultiOptionInternal (answer));
		}

		public bool CheckAnswer (string answer)
		{
			Regex regex;
			Match match;
			bool ignore_case, ignore_spaces;

			if (String.IsNullOrEmpty (answer))
				return false;

			ignore_case = (CheckAttributes & GameAnswerCheckAttributes.IgnoreCase) == GameAnswerCheckAttributes.IgnoreCase;
			ignore_spaces = (CheckAttributes & GameAnswerCheckAttributes.IgnoreSpaces) == GameAnswerCheckAttributes.IgnoreSpaces;

			if (ignore_case == true) // This necessary to make pattern selection (e.g. [a-z]) case insensitive
				regex = new Regex (CheckExpression, RegexOptions.IgnoreCase);
			else
				regex = new Regex (CheckExpression);

			string [] right_answers = Correct.Split (Separator);

			for (int i = 0; i < right_answers.Length; i++)
			{
				right_answers [i] = right_answers[i].Trim ();

				if (ignore_spaces)
					right_answers [i] = RemoveWhiteSpace (right_answers [i]);
			}

			if ((CheckAttributes & GameAnswerCheckAttributes.Trim) == GameAnswerCheckAttributes.Trim)
				answer = answer.Trim ();

			if (ignore_spaces)
				answer = RemoveWhiteSpace (answer);

			// All strings from the list of expected answers (two numbers: 22 | 44) must present in the answer
			if ((CheckAttributes & GameAnswerCheckAttributes.MatchAll) == GameAnswerCheckAttributes.MatchAll ||
				(CheckAttributes & GameAnswerCheckAttributes.MatchAllInOrder) == GameAnswerCheckAttributes.MatchAllInOrder)
			{
				int pos = 0;
				match = regex.Match (answer);
				while (String.IsNullOrEmpty (match.Value) == false)
				{
					if ((CheckAttributes & GameAnswerCheckAttributes.MatchAll) == GameAnswerCheckAttributes.MatchAll)
					{
						for (int i = 0; i < right_answers.Length; i++)
						{
							if (String.Compare (match.Value, right_answers[i], ignore_case) == 0)
							{
								right_answers[i] = null;
								break;
							}
						}
					}
					else //MatchAllInOrder
					{
						if (String.Compare (match.Value, right_answers[pos++], ignore_case) != 0)
							return false;

					}
					match = match.NextMatch ();
				}

				if ((CheckAttributes & GameAnswerCheckAttributes.MatchAllInOrder) == GameAnswerCheckAttributes.MatchAllInOrder)
					return true;

				// Have all the expected answers been matched?
				for (int i = 0; i < right_answers.Length; i++)
				{
					if (right_answers[i] != null)
						return false;
				}

				return true;
			}
			else // Any string from the list of possible answers (answer1 | answer2) present in the answer will do it
			{
				foreach (string s in right_answers)
				{
					match = regex.Match (answer);
					if (String.Compare (match.Value, s, ignore_case) == 0)
						return true;
				}
			}
			return false;
		}

		static string RemoveWhiteSpace (string source)
		{
			StringBuilder str = new StringBuilder ();
			for (int n = 0; n < source.Length; n++)
			{
				if (char.IsWhiteSpace (source [n]) == false)
					str.Append (source[n]);
			}
			return str.ToString ();
		}
	}
}
