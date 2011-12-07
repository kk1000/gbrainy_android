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
using gbrainy.Core.Services;

namespace gbrainy.Games.Memory
{
	public class MemoryFacts : Core.Main.Memory
	{
		const int total_questions = 4;
		string question;
		Fact[] facts;

		struct Fact
		{
			public string fact;
			public string[] questions;
			public int[] answers;
			public int Length;
	
			public void Initialize (int items)
			{
				questions = new string [items];
				answers = new int [items];
				Length = items;
			}
		}

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Memorize facts");}
		}

		public override string MemoryQuestion {
			get { return question;}
		}

		protected override void Initialize ()
		{
			int fact_idx, quest_idx, questions;
			ArrayListIndicesRandom indices;

			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
				questions = 1;
				break;
			default:
			case GameDifficulty.Medium:
				questions = 2;
				break;
			case GameDifficulty.Master:
				questions = 3;
				break;
			}

			indices = new ArrayListIndicesRandom (total_questions);
			indices.Initialize ();

			facts = new Fact [questions];
			base.Initialize ();

			for (int i = 0; i < facts.Length; i++)
				facts[i] = GetFact (indices[i]);

			fact_idx = random.Next (questions);
			quest_idx = random.Next (facts [fact_idx].Length);
			question = facts [fact_idx].questions [quest_idx];
			Answer.Correct = (facts [fact_idx].answers [quest_idx]).ToString ();
			Answer.CheckExpression = "[0-9]+";

			// Since this particular test requires to read and understand text
			// lets give the user twice time to be able to understand the text properly
			TotalTime = TotalTime * 2;
			
		}

		Fact GetFact (int index)
		{
			Fact fact = new Fact ();

			switch (index) {
			case 0:
				fact.Initialize (2);
				fact.answers [0] = 2 + random.Next (14);
				fact.answers [1] = 1914 + random.Next (50);
				fact.fact = String.Format (
					// Translators: {0} is replaced by a number, {1} by a year (like 1940)
					// Day in English does not need to be plural
					ServiceLocator.Instance.GetService <ITranslations> ().GetPluralString ("Shiny Cars had already announced a {0} day production halt next month, but before then it had not halted production since {1}.",
					"Shiny Cars had already announced a {0} day production halt next month, but before then it had not halted production since {1}.",
					fact.answers [0]),
					fact.answers [0], fact.answers [1]);
				fact.questions [0] = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("How many days did Shiny Cars halt its production for?");
				fact.questions [1] = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("In what year did Shiny Cars last halt its production?");
				break;
			case 1:
				fact.Initialize (2);
				fact.answers [0] = 10 + random.Next (30);
				fact.answers [1] = 1914 + random.Next (50);
				fact.fact = String.Format (
					// Translators: {0} is replaced by a number, {1} by a year (like 1940)
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Shiny Cars sales fell {0}% this past December, the worst decline since {1}."),
					fact.answers [0], fact.answers [1]);
				fact.questions [0] = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("By how much did company sales fall last December?");
				fact.questions [1] = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("In what year was Shiny Cars sales total lower than that of last December?");
				break;
			case 2:
				fact.Initialize (1);
				fact.answers [0] = 10 + random.Next (30);
				fact.fact = String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("About {0}% of Shiny Cars produced worldwide are sold in Europe."),
					fact.answers [0]);
				fact.questions [0] = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("What percentage of all Shiny Cars produced worldwide are sold in Europe?");
				break;
			case 3:
				fact.Initialize (2);
				fact.answers [0] = 10 + random.Next (30);
				fact.answers [1] = 100 - (1 + random.Next (10)) - fact.answers [0];
				fact.fact = String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("About {0}% of Shiny Cars use diesel, {1}% use gasoline and the remainder use electricity."),
					fact.answers [0], fact.answers [1]);
				fact.questions [0] = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("What percentage of Shiny Cars use diesel?");
				fact.questions [1] = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("What percentage of Shiny Cars use gasoline?");
				break;
			default:
				throw new Exception ("Invalid index value");
			}

			return fact;
		}
	
		public override void DrawObjectToMemorize (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			string text = string.Empty;

			base.DrawObjectToMemorize (gr, area_width, area_height, rtl);
	
			for (int i = 0; i < facts.Length; i++)
			{
				text += facts[i].fact;
				text += "\n\n";
			}
			gr.DrawStringWithWrapping (0.3, DrawAreaY + 0.2, text, 0.95 - 0.3);
		}
	}
}
