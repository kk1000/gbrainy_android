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

using Cairo;
using System;

using gbrainy.Core.Main;
using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Games.Logic
{
	public class PuzzleNextFigure : Game
	{
		const double figure_size = 0.2;
		const double space_figures = figure_size + 0.066;
		ArrayListIndicesRandom random_indices;

		public enum CerclePosition 
		{
			None		= 0,
			Top		= 2,
			Right		= 4,
			Bottom		= 8,
			Left		= 16,
		}

		enum Figures
		{
			First,
			Second,
			Third,
			Last
		};

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Next figure");}
		}

		public override string Question {
			get {return String.Format (
				ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Which is the next logical figure in the sequence? Answer {0}, {1} or {2}."),
				Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2));} 
		}


		public override string Rationale {
			get {
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("From first figure, the top circle advances by two positions clockwise, while the left circle goes backwards one position.");
			}
		}

		protected override void Initialize ()
		{
			random_indices = new ArrayListIndicesRandom ((int) Figures.Last);
			random_indices.Initialize ();
			Answer.CheckAttributes |= GameAnswerCheckAttributes.MultiOption | GameAnswerCheckAttributes.IgnoreSpaces;

			for (int i = 0; i < (int) Figures.Last; i++)
			{
				if (random_indices[i] == (int) Figures.Third) {
					Answer.SetMultiOptionAnswer (i, Answer.GetFigureName (i));
					break;
				}
			}

			HorizontalContainer container = new HorizontalContainer (DrawAreaX, DrawAreaY + figure_size + 0.16, 0.8, 0.3);

			DrawableArea drawable_area;
			AddWidget (container);

			for (int i = 0; i < (int) Figures.Last; i++)
			{
				drawable_area = new DrawableArea (space_figures, 0.2);						
				drawable_area.SelectedArea = new Rectangle (0, 0, figure_size, figure_size);
				drawable_area.Data = i;
				drawable_area.DataEx = Answer.GetMultiOption (i);
				container.AddChild (drawable_area);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int n = (int) e.Data;

				 	switch ((Figures) random_indices[n]) {
					case Figures.First:
						DrawDiamon (e.Context, 0, 0, CerclePosition.Right | CerclePosition.Left);
						break;
					case Figures.Second:
						DrawDiamon (e.Context, 0, 0, CerclePosition.Top | CerclePosition.Right);
						break;
					case Figures.Third:
						DrawDiamon (e.Context, 0, 0, CerclePosition.Bottom | CerclePosition.Top);
						break;
					default:
						throw new InvalidOperationException ();
					}

					e.Context.MoveTo (0.02, 0.25);
					e.Context.ShowPangoText (Answer.GetFigureName (n));
					e.Context.Stroke ();
				};
			}
		}

		static private void DrawDiamon (CairoContextEx gr, double x, double y, CerclePosition cercles)
		{	
			double distance = 0.04;

			gr.MoveTo (x + figure_size / 2, y);
			gr.LineTo (x, y + figure_size / 2);
			gr.LineTo (x + figure_size / 2, y + figure_size);
			gr.LineTo (x + figure_size, y + figure_size / 2);
			gr.LineTo (x + figure_size / 2, y);
			gr.Stroke ();

			if ((cercles & CerclePosition.Top) == CerclePosition.Top) {
				gr.Arc (x + figure_size / 2, y + distance, 0.01, 0, 2 * Math.PI);	
				gr.Stroke ();
			}

			if ((cercles & CerclePosition.Right) == CerclePosition.Right) {
				gr.Arc (x + figure_size - distance, y + figure_size / 2, 0.01, 0, 2 * Math.PI);	
				gr.Stroke ();
			}

			if ((cercles & CerclePosition.Bottom) == CerclePosition.Bottom) {
				gr.Arc (x + figure_size / 2, y + figure_size - distance, 0.01, 0, 2 * Math.PI);	
				gr.Stroke ();
			}

			if ((cercles & CerclePosition.Left) == CerclePosition.Left) {
				gr.Arc (x + distance, y + figure_size / 2, 0.01, 0, 2 * Math.PI);
				gr.Stroke ();
			}
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = DrawAreaX;
			double y = DrawAreaY;
		
			base.Draw (gr, area_width, area_height, rtl);

			DrawDiamon (gr, x, y, CerclePosition.Top | CerclePosition.Left);
			DrawDiamon (gr, x + space_figures , y, CerclePosition.Bottom);
			DrawDiamon (gr, x + space_figures * 2, y, CerclePosition.Top | CerclePosition.Right);
		
			y += figure_size + 0.06;
			gr.MoveTo (x, y);
			gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Choose one of the following:"));
			gr.Stroke ();
		}
	}
}
