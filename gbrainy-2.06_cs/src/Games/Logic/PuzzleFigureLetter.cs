/*
 * Copyright (C) 2007 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
	public class PuzzleFigureLetter : Game
	{
		private QuestionType question;
	
		enum QuestionType
		{
			TwoSquares	= 0,
			TwoCercles,
			ThreeCercles,
			Length		
		}

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Figures and text");}
		}

		public override string Question {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The figures and the text are related. What text should go under the last figure?");} 
		}

		public override string Tip {
			get { return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Every character of the text represents a property of the figure.");}
		}

		public override string Rationale {
			get {
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("'A' indicates that the figures overlap, 'B' that are squares, 'C' that are circles, 'D' that the figures are separated, 'E' that there are three figures and 'F' that there are two figures.");
			}
		}

		protected override void Initialize ()
		{
			question = (QuestionType) random.Next ((int) QuestionType.Length);

			switch (question) {
			case QuestionType.TwoSquares:
				Answer.Correct = "A | B | F";
				Answer.CorrectShow = "ABF";
				break;
			case QuestionType.TwoCercles:
				Answer.Correct = "C | D | F";
				Answer.CorrectShow = "CDF";
				break;
			case QuestionType.ThreeCercles:
				Answer.Correct = "A | C | E";
				Answer.CorrectShow = "ACE";
				break;
			default:
				throw new InvalidOperationException ();
			}
			
			Answer.CheckExpression = Answer.GetMultiOptionsExpression ();
			Answer.CheckAttributes |= GameAnswerCheckAttributes.MatchAll;
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = DrawAreaX + 0.05;
			double y = DrawAreaY + 0.1;

			base.Draw (gr, area_width, area_height, rtl);
		
			// Three circles
			gr.Arc (x + 0.06, y, 0.05, 0, 2 * Math.PI);
			gr.Stroke ();
			gr.Arc (x, y + 0.1, 0.05, 0, 2 * Math.PI);
			gr.Stroke ();
			gr.Arc (x + 0.11, y + 0.1, 0.05, 0, 2 * Math.PI);
			gr.Stroke ();
			gr.MoveTo (x + 0.02, y + 0.18);
			gr.ShowPangoText ("CDE");
			gr.Stroke ();

			// Two linked circles
			gr.Arc (x + 0.3, y, 0.05, 0, 2 * Math.PI);
			gr.Stroke ();
			gr.Arc (x + 0.3 + 0.06, y + 0.05, 0.05, 0, 2 * Math.PI);
			gr.Stroke ();
			gr.MoveTo (x + 0.30, y + 0.18);
			gr.ShowPangoText ("ACF");
			gr.Stroke ();

			// Two squares
			gr.Rectangle (x + 0.5, y, 0.1, 0.1);
			gr.Rectangle (x + 0.62, y, 0.1, 0.1);
			gr.Stroke ();
			gr.MoveTo (x + 0.58, y + 0.18);
			gr.ShowPangoText ("BDF");
			gr.Stroke ();
	
			// Tree squares
			gr.Rectangle (x - 0.05, y + 0.35, 0.1, 0.1);
			gr.Rectangle (x + 0.06 - 0.05, y + 0.37, 0.1, 0.1);
			gr.Rectangle (x + 0.12 - 0.05, y + 0.39, 0.1, 0.1);
			gr.Stroke ();
			gr.MoveTo (x + 0.04, y + 0.53);
			gr.ShowPangoText ("ABE");
			gr.Stroke ();

			x += 0.25;
			y += 0.35;

			switch (question) {
			case QuestionType.TwoSquares:
				gr.Rectangle (x, y, 0.1, 0.1);
				gr.Rectangle (x + 0.05, y + 0.03, 0.1, 0.1);
				gr.Stroke ();
				gr.MoveTo (x + 0.05, y + 0.18);
				break;
			case QuestionType.TwoCercles:
				gr.Arc (x + 0.05, y + 0.05, 0.05, 0, 2 * Math.PI);
				gr.Stroke ();
				gr.Arc (x + 0.12 + 0.05, y + 0.05, 0.05, 0, 2 * Math.PI);
				gr.Stroke ();
				gr.MoveTo (x + 0.1, y + 0.18);
				break;
			case QuestionType.ThreeCercles:
				gr.Arc (x + 0.05 + 0.06, y + 0.04, 0.05, 0, 2 * Math.PI);
				gr.Stroke ();
				gr.Arc (x + 0.05, y + 0.06 + 0.04, 0.05, 0, 2 * Math.PI);
				gr.Stroke ();
				gr.Arc (x + 0.05  + 0.11, y + 0.06 + 0.04, 0.05, 0, 2 * Math.PI);
				gr.Stroke ();
				gr.MoveTo (x + 0.1, y + 0.18);
				break;
			}

			gr.ShowPangoText ("?");
			gr.Stroke ();		
		}
	}
}
