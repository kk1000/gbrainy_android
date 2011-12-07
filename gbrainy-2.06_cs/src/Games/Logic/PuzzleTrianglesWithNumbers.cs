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
	public class PuzzleTrianglesWithNumbers : Game
	{
		private const double figure_size = 0.2;
		private const int elements_group = 12;
		private int group;
		private string answer_number;
		private int [] numbers = new int []
		{
			15, 14,	// 210
			35, 6,
			70, 3,
			42, 5,
			7, 30,
			21, 10,		
		
			8, 20,	// 160
			5, 32,
			40, 4,
			2, 80,
			10, 16,
			1, 160,

			6, 20,  // 120
			40, 3,
			4, 30,
			15, 8,
			24, 5,
			2, 60,
		};

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Triangles with numbers");}
		}

		public override string Question {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Which number should replace the question mark below?");} 
		}


		public override string Tip {
			get { return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("All the triangles share a property and are independent of the rest.");}
		}

		public override string Rationale {
			get {
				return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The result of multiplying the two numbers inside every triangle is {0}."), answer_number);
			}
		}

		protected override void Initialize ()
		{
			group = random.Next (3);
			switch (group) {
			case 0:
				Answer.Correct = "10";
				answer_number = "210";
				break;
			case 1:
				Answer.Correct = "160";
				answer_number = "160";
				break;
			case 2:
				Answer.Correct = "60";
				answer_number = "120";
				break;
			}
		}


		private void DrawTriangle (CairoContextEx gr, double x, double y, int index, bool question)
		{
			gr.MoveTo (x + figure_size / 2, y);
			gr.LineTo (x, y + figure_size);
			gr.LineTo (x + figure_size, y + figure_size);
			gr.LineTo (x + figure_size / 2, y);
			gr.LineTo (x + figure_size / 2, y + figure_size);
			gr.Stroke ();

			gr.MoveTo (x + 0.04, y + 0.15);
			gr.ShowPangoText (numbers [(elements_group * group) + index * 2].ToString ());	
			gr.MoveTo (x + 0.12, y + 0.15);

			if (question == true)
				gr.ShowPangoText ("?");	
			else
				gr.ShowPangoText (numbers [(elements_group * group) + (index * 2) + 1].ToString ());	
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = DrawAreaX, y = DrawAreaY + 0.1;

			base.Draw (gr, area_width, area_height, rtl);
		
			DrawTriangle (gr, x, y, 0, false);
			x += 0.3;
			DrawTriangle (gr, x, y, 1, false);
			x += 0.3;
			DrawTriangle (gr, x, y, 2, false);

			y += 0.3;
			x = DrawAreaX;	
			DrawTriangle (gr, x, y, 3, false);
			x += 0.3;
			DrawTriangle (gr, x, y, 4, false);
			x += 0.3;
			DrawTriangle (gr, x, y, 5, true);
		}
	}
}
