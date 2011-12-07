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
using Cairo;

using gbrainy.Core.Main;
using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Games.Memory
{
	public class MemoryColouredFigures : Core.Main.Memory
	{
		enum SquareColor
		{
			Color1 = 0,
			Color2,
			Color3,
			Length
		}

		private int columns, rows;
		private int squares;
		private double rect_w;
		private double rect_h;
		private SquareColor []squares_colours;
		private ArrayListIndicesRandom answers_order;
		private const int answers = 4;
		private ColorPalette palette;
		private int color_sheme;
		private const double block_space = 0.35;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Colored figures");}
		}

		public override bool UsesColors {
			get { return true;}
		}

		public override string MemoryQuestion {
			get { return String.Format (
				ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Which of these figures was previously shown? Answer {0}, {1}, {2} or {3}."),
				Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3));}
		}

		protected override void Initialize ()
		{
			Answer.CheckAttributes |= GameAnswerCheckAttributes.MultiOption | GameAnswerCheckAttributes.IgnoreSpaces;

			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
				columns = rows = 5;
				break;
			case GameDifficulty.Medium:
				columns = rows = 6;
				break;
			case GameDifficulty.Master:
				columns = rows = 7;
				break;
			}

			squares = columns * rows;
			rect_w = 0.3 / rows;
			rect_h = 0.3 / columns;
			squares_colours = new SquareColor [squares * answers];
			color_sheme = random.Next (2);
			palette = new ColorPalette ();

			for (int i = 0; i < squares; i++)
				squares_colours[i] = (SquareColor) random.Next ((int) SquareColor.Length);
		
			Randomize (squares_colours, 0, squares);
			Randomize (squares_colours, 0, squares * 2);
			Randomize (squares_colours, 0, squares * 3);

			answers_order = new ArrayListIndicesRandom (answers);
			answers_order.Initialize ();

			for (int i = 0; i < answers_order.Count; i++) {
				if (answers_order[i] == 0) {
					Answer.SetMultiOptionAnswer (i, Answer.GetFigureName (i));
					break;
				}
			}

			base.Initialize ();

			HorizontalContainer container = new HorizontalContainer (DrawAreaX, DrawAreaY, 0.8, 0.4);
			AddWidget (container);

			for (int i = 0; i < answers_order.Count; i++) 
			{
				if (i == 2) {
					container = new HorizontalContainer (DrawAreaX, DrawAreaY + 0.45, 0.8, 0.4);
					AddWidget (container);
				}

				DrawableArea drawable_area = new DrawableArea (0.4, 0.4);
				container.AddChild (drawable_area);
				drawable_area.SelectedArea = new Rectangle (0.05, 0, 0.3, 0.3);
				drawable_area.Data = i;
				drawable_area.DataEx = Answer.GetMultiOption (i);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int n = (int) e.Data;

					palette.Alpha = alpha;
					DrawSquare (e.Context, 0.05, 0, squares_colours, squares * answers_order[n]);
					e.Context.MoveTo (0.05, 0 + block_space - 0.02);
					e.Context.ShowPangoText (Answer.GetFigureName (n));
					e.Context.Stroke ();
				};
			}
		}

		private void Randomize (SquareColor []colours, int source, int target)
		{	
			int elements = 4 + random.Next (2);
			bool done = false;

			while (done == false) {
				for (int i = 0; i < squares; i++) {
					colours[i + target] = colours[i + source];
				}

				for (int i = 0; i < elements; i++) {
					colours[target + random.Next (squares)] = (SquareColor) random.Next ((int) SquareColor.Length);
				}

				// Is not valid if it is already present
				bool equals = true;
				for (int answer = 0; answer < answers; answer++) {
					if (answer * squares == target)
						continue;

					equals = true;
					for (int i = 0; i < squares; i++) {
						if (colours[i + target] != colours[i + (answer * squares)]) {
							equals = false;
							break;
						}
					}

					if (equals == true)
						break;
				}

				if (equals == false)
					done = true;
			}
		}

		public override void DrawObjectToMemorize (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.DrawObjectToMemorize (gr, area_width, area_height, rtl);
			palette.Alpha = alpha; 
			DrawSquare (gr, DrawAreaX + 0.3, DrawAreaY + 0.1, squares_colours, 0);
		}

		private void DrawSquare (CairoContextEx gr, double x, double y, SquareColor []colours, int index)
		{
			gr.Save ();
			for (int column = 0; column < columns; column++) {
				for (int row = 0; row < rows; row++) {

					// if you want 2 schemes (primary or secundary colors)
					Color c = palette.Cairo(ColorPalette.Id.First+ color_sheme*3 + (int)colours[index+(columns * row) + column]);
					gr.Rectangle (x + row * rect_w, y + column * rect_h, rect_w, rect_h);
					gr.FillGradient (x + row * rect_w, y + column * rect_h, rect_w, rect_h, c);
				}
			}
			gr.Restore ();
			for (int column = 0; column < columns; column++) {
				for (int row = 0; row < rows; row++) {
					gr.Rectangle (x + row * rect_w, y + column * rect_h, rect_w, rect_h);
					gr.Stroke ();			
				}
			}
		}
	}
}
