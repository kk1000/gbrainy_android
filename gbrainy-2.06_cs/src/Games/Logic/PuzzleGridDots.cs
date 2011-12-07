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
using Cairo;

using gbrainy.Core.Main;
using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Games.Logic
{
	public class PuzzleGridDots : Game
	{
		const double figure_size = 0.25;
		const int lines = 6;
		const int columns = 6;
		const int figures = 6;
		static bool X = true;
		static bool O = false;
		const double space_figures = 0.05;
		ArrayListIndicesRandom possible_answers;
		int puzzle_index;

		bool [] [] puzzles = new bool [] []
		{
			puzzle_a,
			puzzle_b,
			puzzle_c,
		};

		bool [] puzzle;

		static bool [] puzzle_a  = new bool []
		{
			// Figure A
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, X, X, O, O,	// Down, Diagonal down left
			O, O, X, X, O, O,	// Up, Diagonal up left
			O, O, O, O, O, O,
			O, O, O, O, O, O,

			// Figure B
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, X, O, O, O,
			O, O, X, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, O,

			// Figure C
			O, O, O, O, O, O,
			O, X, X, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, X, X, O, O, O,
			O, O, O, O, O, O,

			// Wrong answer 1
			O, X, X, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, X, X, O, O, O,

			// Correct Answer
			X, O, X, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			X, O, X, O, O, O,

			// Wrong answer 2
			O, O, O, O, O, O,
			O, X, X, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, X, X, O, O, O,
		};

		static bool [] puzzle_b = new bool []
		{
			// Figure A
			X, O, O, O, O, O,
			X, O, O, O, O, O,
			X, O, O, O, O, O,
			X, O, O, O, O, O,
			X, O, O, O, O, O,
			X, O, O, O, O, O,

			// Figure B
			O, O, X, X, X, X,
			O, O, O, O, O, X,
			O, O, O, O, O, X,
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, O,

			// Figure C
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			X, X, X, X, X, X,

			// Wrong answer 1
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			X, O, O, O, O, O,
			X, O, O, O, O, O,
			X, X, X, X, O, O,

			// Correct Answer
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			X, O, O, O, O, O,
			X, O, O, O, O, O,
			X, O, O, O, O, O,
			X, X, X, O, O, O,

			// Wrong answer 2
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			X, O, O, O, O, O,
			X, X, X, X, X, O,
		};

		static bool [] puzzle_c = new bool []
		{
			// Figure A
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, X,
			O, O, X, X, X, X,

			// Figure B
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, X, X,
			O, O, O, O, O, X,
			O, O, O, O, O, X,
			O, O, O, O, O, X,

			// Figure C
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, X, X, X, X,
			O, O, X, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, O,

			// Wrong answer 1
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, X, O, O, O,
			O, O, X, O, O, O,
			O, O, X, X, X, O,

			// Correct Answer
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, X, O, O, O,
			O, O, X, O, O, O,
			O, O, X, O, O, O,
			O, O, X, X, O, O,


			// Wrong answer 2
			O, O, O, O, O, O,
			O, O, O, O, O, O,
			O, O, X, X, O, O,
			O, O, X, O, O, O,
			O, O, X, O, O, O,
			O, O, X, O, O, O,
		};

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Square with dots");}
		}

		public override string Question {
			get {return (String.Format (
				ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Which is the next logical figure in the sequence? Answer {0}, {1} or {2}."),
				Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2)));}
		}


		public override string Rationale {
			get {
				switch (puzzle_index) {
				case 0:
					return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("From the top-left figure, the top-left circle moves down, the bottom-left circle moves up, the bottom-right moves diagonally up-left and the top-right moves diagonally down-left.");
				case 2:
					return ServiceLocator.Instance.GetService <ITranslations> ().GetString
					("From the top-left figure, the figure is rotated counterclockwise 90 degrees.");
				case 1: // TODO
				default:
					return string.Empty;
				}
			}
		}

		protected override void Initialize ()
		{
			Answer.CheckAttributes |= GameAnswerCheckAttributes.MultiOption | GameAnswerCheckAttributes.IgnoreSpaces;
			possible_answers = new ArrayListIndicesRandom (3);
			possible_answers.Initialize ();

			puzzle_index = random.Next (puzzles.Length);
			puzzle = puzzles [puzzle_index];

			DrawableArea drawable_area;
			HorizontalContainer container = new HorizontalContainer (0.05, 0.5, 0.9, figure_size + 0.1);
			AddWidget (container);

			for (int i = 0; i < possible_answers.Count; i++) {

				drawable_area = new DrawableArea (figure_size + space_figures, figure_size + 0.1);
				drawable_area.Data = i;
				drawable_area.DataEx = Answer.GetMultiOption (i);
				drawable_area.SelectedArea = new Rectangle (space_figures / 2, space_figures / 2, figure_size, figure_size);

				container.AddChild (drawable_area);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					DrawPossibleAnswer (e.Context, space_figures / 2, space_figures / 2, possible_answers [(int)e.Data]);
					e.Context.DrawTextCentered (space_figures / 2 + figure_size / 2, space_figures + figure_size + 0.02,
						Answer.GetFigureName ((int)e.Data));
				};
			}

			for (int i = 0; i < possible_answers.Count; i++) {
				if (possible_answers[i] == 0) {
					Answer.SetMultiOptionAnswer (i, Answer.GetFigureName (i));
					break;
				}
			}
		}

		public void DrawFigure (CairoContextEx gr, double x, double y, bool [] puzzle, int index)
		{
			double pos_x = x, pos_y = y;
			double square_size = figure_size / lines;
			double center_square = square_size / 2;
			double radius_square = (square_size - (LineWidth *2)) / 2.5;

			gr.Rectangle (pos_x, pos_y, figure_size, figure_size);
			gr.Stroke ();

			for (int line = 0; line < lines - 1; line++) // Horizontal
			{
				pos_y += square_size;
				gr.MoveTo (pos_x, pos_y);
				gr.LineTo (pos_x + figure_size, pos_y);
				gr.Stroke ();
			}

			pos_y = y;
			for (int column = 0; column < columns - 1; column++) // Vertical
			{
				pos_x += square_size;
				gr.MoveTo (pos_x, pos_y);
				gr.LineTo (pos_x, pos_y + figure_size);
				gr.Stroke ();
			}

			pos_y = y + center_square;
			pos_x = x + center_square;

			for (int line = 0; line < lines; line++) // Circles
			{
				for (int column = 0; column < columns; column++)
				{
					if (puzzle[index + (columns * line) + column] == false)
						continue;

					gr.Arc (pos_x + (square_size * column), pos_y, radius_square, 0, 2 * Math.PI);
					gr.Fill ();
					gr.Stroke ();
				}
				pos_y += square_size;
			}
		}

		public void DrawPossibleAnswer (CairoContextEx gr, double x, double y, int figure)
		{
			switch (figure) {
			case 0: // Good answer
				DrawFigure (gr, x, y, puzzle, columns * lines * 4);
				break;
			case 1:
				DrawFigure (gr, x, y, puzzle,  columns * lines * 3);
				break;
			case 2:
				DrawFigure (gr, x, y, puzzle, columns * lines * 5);
				break;
			}
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = 0.05 + space_figures / 2, y = DrawAreaY;

			base.Draw (gr, area_width, area_height, rtl);

			DrawFigure (gr, x, y, puzzle, 0);
			DrawFigure (gr, x + figure_size + space_figures, y, puzzle, columns * lines);
			DrawFigure (gr, x + (figure_size + space_figures) * 2, y, puzzle, columns * lines * 2);

			y += figure_size + 0.10;
			gr.MoveTo (x, y - 0.02);
			gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Choose one of the following:"));
		}
	}
}
