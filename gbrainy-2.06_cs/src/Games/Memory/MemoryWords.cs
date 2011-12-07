/*
 * Copyright (C) 2007-2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.Collections.Generic;

using gbrainy.Core.Main;
using gbrainy.Core.Services;

namespace gbrainy.Games.Memory
{
	public class MemoryWords : Core.Main.Memory
	{
		private ArrayListIndicesRandom words_order;
		private List <string> words;
		private const int total_words = 35;
		private int showed;
		private int answer;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Memorize words");}
		}

		public override string MemoryQuestion {
			get { 
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("There is a missing word from the previous list. Which one is the missing word?");}
		}

		protected override void Initialize ()
		{
			int tmp;
			words = new List <string> (total_words);

			// Body parts
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("wrist"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("elbow"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("armpit"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("hand"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("chest"));
			
			//Fishes
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("sardine"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("trout"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("monkfish"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("cod"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("salmon"));

			// Vegetables
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("potato"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("ginger"));			
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("pepper"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("garlic"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("pumpkin"));

			// Bicycle
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("brake"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("pedal"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("chain"));			
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("wheel"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("handlebar"));

			// Music
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("drummer"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("speaker"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("lyrics"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("beat"));			
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("song"));

			// Weather
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("cloud"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("rain"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("storm"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("fog"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("rainbow"));

			// Animals
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("rabbit"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("mouse"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("monkey"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("bear"));
			words.Add (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("wolf"));

			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
				showed = 6;
				break;
			case GameDifficulty.Medium:
				showed = 9;
				break;
			case GameDifficulty.Master:
				showed = 12;
				break;
			}

			words_order = new ArrayListIndicesRandom (total_words);
			words_order.Initialize ();
			answer = random.Next (showed);
			tmp = words_order [answer];
			Answer.Correct = words [tmp];
			base.Initialize ();
		}
	
		public override void DrawPossibleAnswers (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x= DrawAreaX + 0.125, y = DrawAreaY + 0.1;
			int cnt = 0;

			for (int i = 0; i < showed; i++)
			{
				if (i == answer)
					continue;

				gr.MoveTo (x, y);
				gr.ShowPangoText (words[words_order[i]]);
				gr.Stroke ();

				if ((cnt + 1) % 3 == 0) {
					y += 0.2;
					x = DrawAreaX + 0.125;
				} else {
					x+= 0.25;
				}
				cnt++;
			}
		}
	
		public override void DrawObjectToMemorize (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.DrawObjectToMemorize (gr, area_width, area_height, rtl);
			DrawObject (gr, area_width, area_height);
		}
	
		private void DrawObject (CairoContextEx gr, int area_width, int area_height)
		{
			double x= DrawAreaX + 0.125, y = DrawAreaY + 0.1;
			for (int i = 0; i < showed; i++)
			{
				gr.MoveTo (x, y);
				gr.ShowPangoText (words[words_order[i]]);
				gr.Stroke ();
			
				if ((i + 1) % 3 == 0) {
					y += 0.2;
					x = DrawAreaX + 0.125;
				} else {
					x+= 0.25;
				}
			}
		}
	}
}
