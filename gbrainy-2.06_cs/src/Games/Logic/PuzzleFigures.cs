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
using System.Text;

using gbrainy.Core.Main;
using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Games.Logic
{
	public class PuzzleFigures : Game
	{
		enum Figure
		{
			Pentagon,
			Circle,
			Triangle
		}

		readonly Figure [] figures  = new Figure []
		{
			Figure.Pentagon, 	Figure.Pentagon, 	Figure.Circle, 		Figure.Circle, 		Figure.Triangle, 	Figure.Triangle,
			Figure.Circle, 		Figure.Triangle, 	Figure.Triangle, 	Figure.Pentagon, 	Figure.Circle, 		Figure.Pentagon,
			Figure.Triangle, 	Figure.Circle, 		Figure.Pentagon, 	Figure.Triangle, 	Figure.Pentagon, 	Figure.Circle
		};

		const double figure_size = 0.1, space_width = 0.05, space_height = 0.03;
		const int possible_answers = 3;
		const int question_columns = 6;
		const int figures_column = 3;
		const int good_answer = 0;
		const int bad_answer1 = 1;
		const int bad_answer2 = 2;

		ArrayListIndicesRandom question_indices;
		ArrayListIndicesRandom answer_indices;
		Figure [] figure_answers;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Figures");}
		}

		public override string Question {
			get {return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("What is the next logical sequence of objects in the last column? Answer {0}, {1} or {2}."),
				Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2));}
		}

		public override string Rationale {
			get {
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("It is the only combination that you can build with the given elements without repeating them.");
			}
		}

		void BuildPossibleAnswers ()
		{
			const int good_answer_column = question_columns - 1;
			int pos;
			figure_answers = new Figure [figures_column * possible_answers];

			// Good answer
			pos = answer_indices [good_answer] * figures_column;
			figure_answers [pos++] = figures[question_indices [good_answer_column]];
			figure_answers [pos++] = figures[question_columns + question_indices [good_answer_column]];
			figure_answers [pos] = figures[(2 * question_columns) + question_indices [good_answer_column]];

			// Bad Answer 1
			pos = answer_indices [bad_answer1] * figures_column;
			figure_answers [pos++] = figures[(2 * question_columns) + question_indices [good_answer_column]];
			figure_answers [pos++] = figures[question_columns + question_indices [good_answer_column]];
			figure_answers [pos] = figures[question_indices [good_answer_column]];

			// Bad Answer 2
			pos = answer_indices [bad_answer2] * figures_column;
			figure_answers [pos++] = figures[question_indices [good_answer_column]];
			figure_answers [pos++] = figures[(2 * question_columns) + question_indices [good_answer_column]];
			figure_answers [pos] = figures[question_columns + question_indices [good_answer_column]];
		}

		protected override void Initialize ()
		{
			question_indices = new ArrayListIndicesRandom (question_columns);
			question_indices.Initialize ();

			answer_indices = new ArrayListIndicesRandom (possible_answers);
			answer_indices.Initialize ();

			Answer.CheckAttributes |= GameAnswerCheckAttributes.MultiOption | GameAnswerCheckAttributes.IgnoreSpaces;
			BuildPossibleAnswers ();

			const double total_width = 0.8, total_height = 0.42;
			HorizontalContainer container = new HorizontalContainer (DrawAreaX, 0.52, total_width, total_height);
			AddWidget (container);

			// Possible answers columns
			for (int ans = 0; ans < possible_answers; ans++)
			{
				DrawableArea drawable_area = new DrawableArea (total_width / possible_answers, total_height);
				drawable_area.Data = ans;
				drawable_area.DataEx = Answer.GetMultiOption (ans);
				container.AddChild (drawable_area);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					double width = total_width / possible_answers;
					double x = (width - figure_size) / 2, y = 0;
					int column = (int) e.Data;

					for (int figure = 0; figure < figures_column; figure++)
					{
						DrawFigure (e.Context, x, y, figure_answers [(column * figures_column) + figure]);
						y+= figure_size + space_height;
					}

					e.Context.DrawTextCentered (width / 2, total_height - 0.02, Answer.GetFigureName (column));
					e.Context.Stroke ();
				};
			}
			Answer.SetMultiOptionAnswer (answer_indices[good_answer], Answer.GetFigureName (answer_indices[good_answer]));
		}

		static void DrawFigure (CairoContextEx gr, double x, double y, Figure figure)
		{
			switch (figure)
			{
				case Figure.Pentagon:
					gr.DrawPentagon (x, y, figure_size);
					break;
				case Figure.Circle:
					gr.Arc (x + figure_size / 2, y + (figure_size / 2), figure_size / 2, 0, 2 * Math.PI);
					break;
				case Figure.Triangle:
					gr.DrawEquilateralTriangle (x, y, figure_size);
					break;
				default:
					throw new InvalidOperationException ("Invalid figure type");
			}
			gr.Stroke ();
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			int element;
			double x = DrawAreaX;
			double y = 0.05, pos_y;

			base.Draw (gr, area_width, area_height, rtl);

			for (int i = 0; i < (Answer.Draw ? question_columns : question_columns - 1); i++)
			{
				element = question_indices [i];
				pos_y = y;
				for (int n = 0; n < figures_column; n++)
				{
					DrawFigure (gr, x, pos_y, figures [(n * question_columns) + element]);
					pos_y+= figure_size + space_height;
				}
				x+= figure_size + space_width;
			}

			if (Answer.Draw == false) {
				gr.Save ();
				gr.SetPangoFontSize (figure_size);
				for (int n = 0; n < figures_column; n++) {
					gr.MoveTo (x, y - 0.02);
					gr.ShowPangoText ("?");
					gr.Stroke ();
					y+= figure_size + space_height;
				}
				gr.SetPangoNormalFontSize ();
				gr.Restore ();
			}

			gr.MoveTo (0.08, 0.45);
			gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Choose one of the following:"));
			gr.Stroke ();
		}
	}
}
