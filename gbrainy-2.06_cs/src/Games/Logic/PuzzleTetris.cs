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
using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Games.Logic
{
	public class PuzzleTetris : Game
	{
		private ArrayListIndicesRandom random_indices_questions;
		private ArrayListIndicesRandom random_indices_answers;
		private const double rect_witdh = 0.04, rect_height = 0.04, space_figures = 0.22;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Tetris");}
		}

		public override string Question {
			get {return String.Format (
				ServiceLocator.Instance.GetService <ITranslations> ().GetString ("What figure completes the set below? Answer {0}, {1} or {2}."),
				Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2));}
		}

		public override string Rationale {
			get {
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("It is the figure that completes all possible combinations with four blocks without taking into account rotations.");
			}
		}

		protected override void Initialize ()
		{
			Answer.CheckAttributes |= GameAnswerCheckAttributes.MultiOption | GameAnswerCheckAttributes.IgnoreSpaces;
			random_indices_questions = new ArrayListIndicesRandom (4);
			random_indices_questions.Initialize ();

			random_indices_answers = new ArrayListIndicesRandom (3);
			random_indices_answers.Initialize ();

			for (int i = 0; i < random_indices_answers.Count; i++) {
				if ((int) random_indices_answers [i] == 0) {
					Answer.SetMultiOptionAnswer (i, Answer.GetFigureName (i));
					break;
				}
			}

			HorizontalContainer container = new HorizontalContainer (0.1, 0.5, 0.8, 0.4);
			DrawableArea drawable_area;
			AddWidget (container);

			for (int i = 0; i < 3; i++)
			{
				drawable_area = new DrawableArea (0.8 / 3, 0.4);
				drawable_area.Data = i;
				drawable_area.DataEx = Answer.GetMultiOption (i);
				container.AddChild (drawable_area);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int n = (int) e.Data;

					DrawAnswerFigures (e.Context, 0.05, 0.2, random_indices_answers [n]);
					e.Context.MoveTo (0.05, 0.33);
					e.Context.ShowPangoText (Answer.GetFigureName (n));
				};
			}
		}

		private static void DrawQuestionFigures (CairoContextEx gr, double x, double y, int figure)
		{
			switch (figure) {
			case 0:
				// XX
				// XX
				for (int i = 0; i < 2; i++) {
					gr.Rectangle (x + i * rect_witdh, y, rect_witdh, rect_height);
					gr.Rectangle (x + i * rect_witdh, y - rect_height, rect_witdh, rect_height);
				}
				gr.Stroke ();
				break;
			case 1:
				//  X
				// XXX
				for (int i = 0; i < 3; i++) {
					gr.Rectangle (x + i * rect_witdh, y, rect_witdh, rect_height);
				}
				gr.Rectangle (x + 1 * rect_witdh, y - rect_height, rect_witdh, rect_height);
				gr.Stroke ();
				break;
			case 2:
				//   X
				// XXX
				for (int i = 0; i < 3; i++) {
					gr.Rectangle (x + i * rect_witdh, y, rect_witdh, rect_height);
				}
				gr.Rectangle (x + 2 * rect_witdh, y - rect_height, rect_witdh, rect_height);
				gr.Stroke ();
				break;
			case 3:
				// XXXX
				for (int i = 0; i < 4; i++) {
					gr.Rectangle (x + i * rect_witdh, y, rect_witdh, rect_height);
				}
				gr.Stroke ();
				break;
			}
		}

		private static void DrawAnswerFigures (CairoContextEx gr, double x, double y, int figure)
		{
			switch (figure) {
			case 0:
				//  XX
				// XX
				for (int i = 0; i < 2; i++) {
					gr.Rectangle (x + i * rect_witdh, y, rect_witdh, rect_height);
					gr.Rectangle (x + rect_witdh + i * rect_witdh, y -  rect_height, rect_witdh, rect_height);
				}
				break;
			case 1:
				// X
				// X
				// X
				// X
				for (int i = 0; i < 4; i++) {
					gr.Rectangle (x + 0.05, y -  rect_height * i, rect_witdh, rect_height);
				}
				break;
			case 2:
				// XXX
				//  X
				for (int i = 0; i < 3; i++) {
					gr.Rectangle (x + i * rect_witdh, y - rect_height, rect_witdh, rect_height);
				}
				gr.Rectangle (x + rect_witdh, y, rect_witdh, rect_height);
				break;
			}
			gr.Stroke ();
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = DrawAreaX, y = DrawAreaY + 0.1;

			base.Draw (gr, area_width, area_height, rtl);
		
			for (int i = 0; i < 4; i++) {
				DrawQuestionFigures (gr, x, y, random_indices_questions [i]);
				x += space_figures;
			}

			gr.MoveTo (0.1, 0.4 - 0.02);
			gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Choose one of the following:"));
		}
	}
}
