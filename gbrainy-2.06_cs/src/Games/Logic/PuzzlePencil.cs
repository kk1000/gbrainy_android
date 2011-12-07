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
	public class PuzzlePencil : Game
	{
		private ArrayListIndicesRandom random_indices;
		private const double figure_width = 0.1, figure_height = 0.1, space_width = 0.1, space_height = 0.15;
		private const double figure_size = 0.2;
		private const int figures = 5;
		private const int answer_index = 4;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Pencil");}
		}

		public override string Question {
			get {return String.Format ( ServiceLocator.Instance.GetService <ITranslations> ().GetString 
				("Which of the following figures cannot be drawn without crossing any previous lines nor lifting the pencil? Answer {0}, {1}, {2}, {3} or {4}."),
				Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3), Answer.GetMultiOption (4));} 
		}

		protected override void Initialize ()
		{
			random_indices = new ArrayListIndicesRandom (figures);
			random_indices.Initialize ();

			Answer.CheckAttributes |= GameAnswerCheckAttributes.MultiOption | GameAnswerCheckAttributes.IgnoreSpaces;

			for (int i = 0; i < random_indices.Count; i++) {
				if (random_indices[i] != answer_index)
					continue;
			
				Answer.SetMultiOptionAnswer (i, Answer.GetFigureName (i));
				break;
			}

			const double text_offset = 0.04;
			double x = DrawAreaX, y = DrawAreaY + 0.1, box_size = (1 - (DrawAreaX * 2)) / 3;
			HorizontalContainer container1, container2, container = null;
			DrawableArea drawable_area;

			for (int figure = 0; figure < figures; figure++)
			{
				switch (figure) {
				case 0:
					x = DrawAreaX;
					container1 = new HorizontalContainer (x, y, 0.8, figure_size);
					container = container1;
					AddWidget (container);
					break;
				case 3:
					x = DrawAreaX;
					y += 0.4;
					container2 = new HorizontalContainer (x, y, 0.8, figure_size);
					container = container2;
					AddWidget (container);
					break;
				default:
					break;
				}

				drawable_area = new DrawableArea (box_size, figure_size);
				drawable_area.SelectedArea = new Rectangle ((box_size - figure_size) / 2, 0, figure_size, figure_size);
				drawable_area.Data = figure;
				drawable_area.DataEx = Answer.GetMultiOption (figure);

				switch (random_indices[figure]) {
				case 0:
					drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
					{
						DrawTriangle (e.Context, (e.Width - figure_size) / 2, 0);
						e.Context.DrawTextCentered (e.Width / 2, figure_size + text_offset, 
							Answer.GetFigureName ((int) e.Data));
					};
					break;
				case 1:
					drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
					{
						DrawDiamon (e.Context, (e.Width - figure_size) / 2, 0);
						e.Context.DrawTextCentered (e.Width / 2, figure_size + text_offset,
							Answer.GetFigureName ((int) e.Data));
					};
					break;
				case 2:
					drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
					{
						DrawRectangleWithTriangles (e.Context, (e.Width - figure_size) / 2, 0);
						e.Context.DrawTextCentered (e.Width / 2, figure_size + text_offset,
							Answer.GetFigureName ((int) e.Data));
					};
					break;
				case 3:
					drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
					{
						DrawThreeTriangles (e.Context, (e.Width - figure_size) / 2, 0);
						e.Context.DrawTextCentered (e.Width / 2, figure_size + text_offset,
							Answer.GetFigureName ((int) e.Data));
					};
					break;
				case answer_index:
					drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
					{
						DrawRectangleWithCross (e.Context, (e.Width - figure_size) / 2, 0);
						e.Context.DrawTextCentered (e.Width / 2, figure_size + text_offset,
							Answer.GetFigureName ((int) e.Data));
					};
					break;
				}			

				container.AddChild (drawable_area);
				x += box_size;
			}			
		}

		static private void DrawTriangle (CairoContextEx gr, double x, double y)
		{
			gr.MoveTo (x + (figure_size / 2), y);
			gr.LineTo (x, y + figure_size);
			gr.LineTo (x + figure_size, y + figure_size);
			gr.LineTo (x + (figure_size / 2), y);
			gr.LineTo (x + (figure_size / 2), y + figure_size);
			gr.Stroke ();	
		}

		static private void DrawDiamon (CairoContextEx gr, double x, double y)
		{
			x += 0.1;
			gr.MoveTo (x, y);
			gr.LineTo (x - (figure_size / 2), y + (figure_size / 2));
			gr.LineTo (x, y + figure_size);
			gr.LineTo (x + figure_size / 2, y + (figure_size / 2));
			gr.LineTo (x, y);
			gr.LineTo (x, y + figure_size);
			gr.Stroke ();
		}

		static private void DrawRectangleWithTriangles (CairoContextEx gr, double x, double y)
		{
			gr.Rectangle (x, y, figure_size, figure_size);
			gr.Stroke ();	
	
			gr.MoveTo (x, y + figure_size);
			gr.LineTo (x + figure_size / 4, y);
			gr.LineTo (x + figure_size / 2, y + figure_size);

			gr.Stroke ();		
	
			gr.MoveTo (x + figure_size / 2, y + figure_size);
			gr.LineTo (x + figure_size / 4 * 3, y);
			gr.LineTo (x + figure_size, y + figure_size);

			gr.Stroke ();
		}

		static private void DrawThreeTriangles (CairoContextEx gr, double x, double y)
		{
			gr.MoveTo (x, y);
			gr.LineTo (x, y + figure_size);
			gr.LineTo (x + figure_size, y);
			gr.LineTo (x, y);
			gr.LineTo (x + figure_size, y + figure_size);
			gr.LineTo (x + figure_size, y);

			gr.Stroke ();
		
		}

		static private void DrawRectangleWithCross (CairoContextEx gr, double x, double y)
		{
			gr.Rectangle (x, y, figure_size, figure_size);

			gr.MoveTo (x, y);
			gr.LineTo (x + figure_size, y + figure_size);
			gr.Stroke ();

			gr.MoveTo (x + figure_size, y);
			gr.LineTo (x, y + figure_size);
			gr.Stroke ();
		}
	}
}
