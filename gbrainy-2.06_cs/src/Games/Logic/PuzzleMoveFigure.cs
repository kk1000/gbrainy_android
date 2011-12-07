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
	public class PuzzleMoveFigure: Game
	{
		private int lines;
		private int type;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Move figure");}
		}

		public override string Question {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("What is the minimum number of circles to be moved in order to convert the left figure into the right figure?");} 
		}
	
		public override string Rationale {
			get {
				switch (type) {
				case 0:
					return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Move the circle from the first line to the second and move two circles from the fourth line to the second and the fifth lines.");
				case 1:
					return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Move the first line to the seventh; move the two circles of the second line to third; and move first and last circles of the fifth line to the sixth.");
				default:	
					return string.Empty;
				}
			}
		}

		protected override void Initialize ()
		{	
			type = random.Next (2);
			lines = 4 + type;
		
			switch (type)
			{
				case 0:
					Answer.Correct = "3";
					break;
				case 1:
					Answer.Correct = "5";
					break;
			}
		
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double first_x, x, y;
			double figure_size = 0.07 + (0.01 * (5 - lines));
			double margin = 0;

			base.Draw (gr, area_width, area_height, rtl);

			// Figure 1
			margin = ((1.0 - (figure_size * lines * 2)) / 2);

			x = first_x = margin + (figure_size * lines / 2) + figure_size / 2;
			y = DrawAreaY + 0.2;
			for (int line = 0; line < lines + 1; line++)
			{
				for (int circles = 0; circles < line; circles++)
				{
					gr.Arc (x, y, figure_size / 2, 0, 2 * Math.PI);	
					gr.Stroke ();
					x += figure_size;
				}
				x = first_x = first_x - (figure_size / 2);
				y += figure_size;			
			}

			// Figure 2
			first_x = margin + (figure_size * lines);
			y = DrawAreaY + 0.2 + figure_size;
			for (int line = 0; line < lines; line++)
			{
				x = first_x = first_x + (figure_size / 2);
				for (int circles = 0; circles < lines - line; circles++)
				{
					gr.Arc (x, y, figure_size / 2, 0, 2 * Math.PI);	
					gr.Stroke ();
					x += figure_size;
				}
				y += figure_size;			
			}
		}
	}
}
