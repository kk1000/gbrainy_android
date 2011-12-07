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
	public class PuzzlePeopleTable : Game
	{
		private const double figure_size = 0.15;
		private string ques1, ques2;
	
		private class Circle
		{	
			public double x;
			public double y;

			public Circle (double x, double y) 
			{
				this.x = x;
				this.y = y;
			}
		}

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("People at a table");}
		}

		public override string Question {
			get {return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("A group of people are sitting at a round table spaced out evenly. How many people are there if the {0} person is across from the {1}?"), ques1, ques2);} 
		}

		public override string Rationale {
			get {
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Subtracting the two positions you find out how many people are seated half way around the table. Doubling this number leaves you with the total amount of people.");
			}
		}

		protected override void Initialize ()
		{
			switch (random.Next (3)) {
			case 0:
				ques1 = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("5th");
				ques2 = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("19th");
				Answer.Correct = "28";
				break;
			case 1:
				ques1 = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("4th");
				ques2 = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("12th");
				Answer.Correct = "16";
				break;
			case 2:
				ques1 = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("9th");
				ques2 = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("22nd");
				Answer.Correct = "26";
				break;
			}			
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = DrawAreaX + 0.22, y = DrawAreaY + 0.2;
			double pos_x = x;
			double pos_y = y;
			Circle[] circles = null;

			base.Draw (gr, area_width, area_height, rtl);

			circles =  new Circle [] {
				new Circle (0.01, 0.06),
				new Circle (0.27, 0.06),
				new Circle (0.01, 0.21),
				new Circle (0.27, 0.21),
				new Circle (0.14, 0),
				new Circle (0.14, 0.29)
			};

			// Circle
			gr.Arc (pos_x + figure_size, pos_y + figure_size, figure_size, 0, 2 * Math.PI);
			gr.Stroke ();		

			const double point_size = 0.01;
			for (int i = 0; i < circles.Length; i++) {
				gr.Arc (x + point_size + circles[i].x, y + point_size + circles[i].y, point_size, 0, 2 * Math.PI);
				gr.Fill ();
				gr.Stroke ();
			}

			gr.MoveTo (x + circles[2].x + 0.01, y + circles[2].y + 0.01);
			gr.LineTo (x + circles[1].x + 0.01, y + circles[1].y + 0.01);
			gr.Stroke ();

			gr.DrawTextCentered (pos_x + figure_size, pos_y + 0.08 + figure_size * 2, 
				ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Two people in the table sitting across each other"));
		}
	}
}
