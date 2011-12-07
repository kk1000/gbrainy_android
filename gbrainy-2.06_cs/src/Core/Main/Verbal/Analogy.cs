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

namespace gbrainy.Core.Main.Verbal
{
	public class Analogy
	{
		public enum Type
		{
			QuestionAnswer = 0,
			MultipleOptions,
			PairOfWordsOptions,
			PairOfWordsCompare,
			Last
		}

		public string question;
		public string [] answers;
		public Type type;
		public string tip;
		public string rationale;
		public int right;

		public bool MultipleAnswers {
			get {
				string [] items = answers[right].Split (AnalogiesFactory.Separator);

				return items.Length > 1;
			}
		}

		public override string ToString ()
		{
			string str = string.Empty;
	
			str += String.Format ("Question: {0}\n", question);
			str += String.Format ("Type: {0}\n", type);
			str += String.Format ("Tip: {0}\n", tip);
			str += String.Format ("Rational: {0}\n", rationale);
			return str;
		}

		public Analogy ()
		{

		}

		// Uses deep copy
		public Analogy Copy ()
		{
			Analogy analogy;

			analogy = new Analogy ();
			analogy.question = question;
			analogy.type = type;
			analogy.tip = tip;
			analogy.rationale = rationale;
			analogy.right = right;

			if (answers != null) {
				analogy.answers = new string [answers.Length];
				for (int i = 0; i < answers.Length; i++)
					analogy.answers [i] = answers[i];
			}

			return analogy;
		}
	}
}
