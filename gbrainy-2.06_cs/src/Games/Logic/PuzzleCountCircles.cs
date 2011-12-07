/*
 * Copyright (C) 2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
	public class PuzzleCountCircles : Game
	{
		private const double figure_size = 0.3;
		private const double radian = Math.PI / 180;
		private int n_circles;

		class ItemCircle
		{
			public double x, y, rad;

			public ItemCircle (double x, double y, double rad)
			{
				this.x = x;
				this.y = y;
				this.rad = rad;
			}
		}

		ItemCircle[] circles;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Count circles");}
		}

		public override string Question {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("How many circles do you count?");} 
		}

		public override string Tip {
			get { return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("It is an easy exercise if you systematically count the circles.");}
		}

		protected override void Initialize ()
		{
			double x, y, rad;

			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
				n_circles = 7;
				break;
			case GameDifficulty.Master:
				n_circles = 14;
				break;		
			case GameDifficulty.Medium:
			default:
				n_circles = 10;
				break;		
			}

			n_circles += random.Next (5);
			circles = new ItemCircle [n_circles];
			for (int i = 0; i < circles.Length; i++)
			{
				x = random.Next (500) / 1000d;
				y = random.Next (500) / 1000d;
				rad = 0.03 +  random.Next (500) / 3200d;

				circles[i] = new ItemCircle (x, y, rad);
			}

			Answer.Correct = n_circles.ToString ();
		}	


		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = DrawAreaX + 0.1, y = DrawAreaY + 0.05;

			base.Draw (gr, area_width, area_height, rtl);

			for (int i = 0; i < circles.Length; i++)
			{
				gr.Arc (x + circles[i].x + 0.1, y + circles[i].y + 0.1, circles[i].rad, 0, 2 * Math.PI);
				gr.Stroke ();
			}
		}
	}
}
