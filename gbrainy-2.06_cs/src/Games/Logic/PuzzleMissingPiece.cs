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
using Cairo;

using gbrainy.Core.Main;
using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Games.Logic
{
	public class PuzzleMissingPiece : Game
	{
		private ArrayListIndicesRandom random_indices;
		private const double sub_figure = 0.15;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Missing piece");}
		}

		public override string Question {
			get {return String.Format (
				ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Which square completes the figure below? Answer {0}, {1} or {2}."),
				Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2));}
		}

		public override string Tip {
			get { return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The logic works at row level.");}
		}

		public override string Rationale {
			get {
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("In every row the third square is made by flipping the first square and superimposing it on the second square, followed by removing the matching lines.");
			}
		}

		protected override void Initialize ()
		{
			Answer.CheckAttributes |= GameAnswerCheckAttributes.MultiOption| GameAnswerCheckAttributes.IgnoreSpaces;
			random_indices = new ArrayListIndicesRandom (3);
			random_indices.Initialize ();

			for (int i = 0; i < random_indices.Count; i++) {
				if (random_indices [i] == 0) {
					Answer.SetMultiOptionAnswer (i, Answer.GetFigureName (i));
					break;
				}
			}

			HorizontalContainer container = new HorizontalContainer (DrawAreaX, 0.7, 0.8, 0.3);
			DrawableArea drawable_area;
			AddWidget (container);

			for (int i = 0; i < random_indices.Count; i++)
			{
				drawable_area = new DrawableArea (0.8 / 3, 0.3);
						
				drawable_area.SelectedArea = new Rectangle (0, 0, sub_figure, sub_figure);
				drawable_area.Data = i;
				drawable_area.DataEx = Answer.GetMultiOption (i);
				container.AddChild (drawable_area);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int n = (int) e.Data;

					DrawAnswerFigures (e.Context, 0, 0, random_indices [n]);
					e.Context.MoveTo (0, 0.2);
					e.Context.ShowPangoText (Answer.GetFigureName (n));
				};
			}
		}

		static private void DrawFigureSequence (CairoContextEx gr, double x, double y, int sequence, bool last_block)
		{
			gr.Rectangle (x, y, sub_figure, sub_figure);
			gr.Rectangle (x + sub_figure, y, sub_figure, sub_figure);

			if (last_block)
				gr.Rectangle (x + sub_figure * 2, y, sub_figure, sub_figure);

			switch (sequence) {
			case 0:
				gr.MoveTo (x, y + sub_figure);
				gr.LineTo (x + sub_figure, y);
				gr.MoveTo (x, y);
				gr.LineTo (x + sub_figure, y + sub_figure);
				x+= sub_figure;
				gr.MoveTo (x, y);
				gr.LineTo (x + sub_figure, y  + sub_figure);
				x+= sub_figure;
				gr.MoveTo (x, y + sub_figure);
				gr.LineTo (x + sub_figure, y);
				break;
			case 1:
				gr.MoveTo (x + sub_figure, y);
				gr.LineTo (x, y + sub_figure);
				gr.MoveTo (x + sub_figure / 2, y + sub_figure);
				gr.LineTo (x + sub_figure, y + sub_figure / 2);
				x+= sub_figure;
				gr.MoveTo (x, y + sub_figure / 2);
				gr.LineTo (x + sub_figure / 2, y + sub_figure);
				x+= sub_figure;
				gr.MoveTo (x, y);
				gr.LineTo (x + sub_figure, y + sub_figure);
				break;
			case 2:
				gr.MoveTo (x + sub_figure / 2, y);
				gr.LineTo (x + sub_figure, y + sub_figure / 2);
				gr.MoveTo (x, y + sub_figure);
				gr.LineTo (x + sub_figure / 2, y + sub_figure / 2);
				gr.LineTo (x + sub_figure, y + sub_figure);
				x+= sub_figure;
				gr.MoveTo (x, y + sub_figure / 2);
				gr.LineTo (x + sub_figure / 2, y);
				break;
			}

			gr.Stroke ();
		}

		static private void DrawAnswerFigures (CairoContextEx gr, double x, double y, int figure)
		{
			gr.Rectangle (x, y, sub_figure, sub_figure);

			switch (figure) {
			case 0:
				gr.MoveTo (x, y + sub_figure);
				gr.LineTo (x + sub_figure / 2, y + sub_figure / 2);
				gr.LineTo (x + sub_figure, y + sub_figure);
				break;
			case 1:
				gr.MoveTo (x, y + sub_figure);
				gr.LineTo (x + sub_figure, y);
				break;
			case 2:
				gr.MoveTo (x, y);
				gr.LineTo (x + sub_figure, y + sub_figure);
				break;
			}
			gr.Stroke ();
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = DrawAreaX + 0.15;

			base.Draw (gr, area_width, area_height, rtl);
		
			for (int i = 0; i < 2; i++)
				DrawFigureSequence (gr, x, DrawAreaY + sub_figure * i , i, true);

			DrawFigureSequence (gr, x, DrawAreaY + sub_figure * 2 , 2, false);

			gr.MoveTo (0.1, 0.62);
			gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Choose one of the following:"));
		}
	}
}
