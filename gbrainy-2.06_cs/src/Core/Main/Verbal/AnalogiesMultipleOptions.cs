/*
 * Copyright (C) 2009-2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.IO;
using System.Collections.Generic;

using Cairo;

using gbrainy.Core.Libraries;
using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Core.Main.Verbal
{
	public class AnalogiesMultipleOptions : Analogies
	{
		static protected Dictionary <int, Analogy> analogies;

		public AnalogiesMultipleOptions ()
		{
			if (analogies == null)
				analogies = AnalogiesFactory. Get (Analogy.Type.MultipleOptions);
		}

		public override string Name {
			get { return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Multiple options #{0}"), Variant);}
		}

		public override string Question {
			get {
				if (Current == null)
					return string.Empty;

				if (Current.answers == null)
					return Current.question;

				// Translators: {0} is replaced by a question and {1} by the possible valid answers
				// E.g.: What is the correct option? Answer A, B, C or D.
				return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("{0} Answer {1}."),
					Current.question, Answer.GetMultiOptionsPossibleAnswers (Current.answers.Length));
			}
		}

		public override Dictionary <int, Analogy> List {
			get { return analogies; }
		}

		protected override void Initialize ()
		{
			Current = GetNext ();

			if (Current == null || Current.answers == null)
				return;
			
			Answer.CheckAttributes |= GameAnswerCheckAttributes.MultiOption;
			Answer.SetMultiOptionAnswer (Current.right, Current.answers[Current.right]);

			Container container = new Container (DrawAreaX + 0.1, 0.50, 0.5, Current.answers.Length * 0.15);
			AddWidget (container);
	
			for (int i = 0; i <  Current.answers.Length; i++)
			{
				DrawableArea drawable_area = new DrawableArea (0.8, 0.1);
				drawable_area.X = DrawAreaX;
				drawable_area.Y = DrawAreaY + 0.2 + i * 0.15;
				container.AddChild (drawable_area);
				drawable_area.Data = i;
				drawable_area.DataEx = Answer.GetMultiOption (i);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int n = (int) e.Data;

					e.Context.MoveTo (0.05, 0.02);
					e.Context.ShowPangoText (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("{0}) {1}"), Answer.GetMultiOption (n), Current.answers[n].ToString ()));
				};
			}
			SetAnswerCorrectShow ();
		}
	
		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.Draw (gr, area_width, area_height, rtl);

			if (Current == null || Current.answers == null)
				return;

			gr.SetPangoLargeFontSize ();
			gr.MoveTo (0.1, DrawAreaY + 0.05);
			gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Choose one of the following:"));
		}
	}
}
