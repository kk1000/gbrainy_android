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
using System.Collections.Generic;

using gbrainy.Core.Services;

namespace gbrainy.Core.Main.Verbal
{
	public class AnalogiesPairOfWordsCompare : Analogies
	{
		static protected Dictionary <int, Analogy> analogies;
		static protected ArrayListIndicesRandom analogies_indices;

		string samples, sample;

		public AnalogiesPairOfWordsCompare ()
		{
			if (analogies == null)
				analogies = AnalogiesFactory. Get (Analogy.Type.PairOfWordsCompare);
		}

		public override string Name {
			get { return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Pair of words compare #{0}"), Variant);}
		}

		public override Dictionary <int, Analogy> List {
			get { return analogies; }
		}

		public override string Question {
			get {
				if (Current == null)
					return string.Empty;

				if (Current.answers == null)
					return Current.question;

				return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString (
					"Given the relationship between the two words below, which word has the same relationship to '{0}'?"),
					sample);
			}
		}

		protected override void Initialize ()
		{
			Current = GetNext ();

			if (Current == null || Current.answers == null)
				return;

			string [] items;

			items = Current.question.Split (AnalogiesFactory.Separator);

			if (items.Length == 2)
				sample = items [1].Trim ();
			else
				sample = string.Empty;

			samples = items [0].Trim ();
			Answer.Correct = Current.answers [Current.right];
			SetAnswerCorrectShow ();
		}
	
		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double y = DrawAreaY + 0.1;

			base.Draw (gr, area_width, area_height, rtl);

			if (Current == null || Current.answers == null)
				return;

			gr.SetPangoLargeFontSize ();
			gr.DrawTextCentered (0.5, y + 0.25,
				String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Words: {0}"), samples));
		}
	}
}
