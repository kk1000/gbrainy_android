/*
 * Copyright (C) 2007 Jordi Mas i Hernàndez <jmas@softcatala.org>
 * Copyright (C) 2007 Javier Mª Mora Merchán <jamarier@gmail.com>
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
	public class PuzzleGridNumbers : Game
	{
		public enum Operation
		{
			MultiplyAndAdd = 0,	// Multiplies two elements and adds a third
			MutilplyAndSubs,	// Multiplies two elements and substracts a third
			AddAndSubs,		// Adds two elements and  substracts a third 
			LastOperation
		}

		private int [] numbers;
		private Operation operation;
		private bool orientation;
		private const int rows = 4, columns = 4;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Numbers in a grid");}
		}

		public override string Question {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The numbers in the grid below follow a pattern. Which number should replace the question mark?");}
		}

		public override string Tip {
			get { 
				if (orientation) 
					return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The pattern is arithmetical and works vertically.");
				else 
					return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The pattern is arithmetical and works horizontally.");
			}
		}

		public override string Rationale {
			get { 
				switch (operation) {
				case Operation.MultiplyAndAdd:
					if (orientation) {
						return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The fourth row is calculated by multiplying the first two rows and adding the third.");
					} else {
						return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The fourth column is calculated by multiplying the first two columns and adding the third.");
					}
				case Operation.MutilplyAndSubs:
					if (orientation) {
						return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The fourth row is calculated by multiplying the first two rows and subtracting the third.");
					} else {
						return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The fourth column is calculated by multiplying the first two columns and subtracting the third.");
					}
				case Operation.AddAndSubs:
					if (orientation) {
						return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The fourth row is calculated by adding the first two rows and subtracting the third.");
					} else {
						return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The fourth column is calculated by adding the first two columns and subtracting the third.");
					}
				default:
					return string.Empty;
				}
			}
		}

		protected override void Initialize ()
		{
			operation = (Operation) random.Next ((int) Operation.LastOperation);
			orientation = (random.Next ((int) 2) == 0) ? true : false;
			numbers = new int [4 * 4];

			int coordinateA, coordinateB;

			if (orientation) {
				coordinateA=4; 
				coordinateB=1;
			}
			else {
				coordinateA=1;
				coordinateB=4;
			}

		
			for (int n = 0; n < 3; n++)
				for (int i = 0; i < 4; i++) 
					numbers[n*coordinateA + i*coordinateB] = random.Next (10) + random.Next (5);

			for (int i = 0; i < 4; i++) {
				switch (operation) {
				case Operation.MultiplyAndAdd:
					numbers[3*coordinateA + i*coordinateB] = (numbers [0*coordinateA + i*coordinateB ] * numbers[1*coordinateA + i*coordinateB]) + numbers[2*coordinateA + i*coordinateB];
					break;
				case Operation.MutilplyAndSubs:
					numbers[3*coordinateA + i*coordinateB] = (numbers [0*coordinateA + i*coordinateB ] * numbers[1*coordinateA + i*coordinateB]) - numbers[2*coordinateA + i*coordinateB];
					break;
				case Operation.AddAndSubs:
					numbers[3*coordinateA + i*coordinateB] = (numbers [0*coordinateA + i*coordinateB ] + numbers[1*coordinateA + i*coordinateB]) - numbers[2*coordinateA + i*coordinateB];
					break;
				default:
					break;
				}			
			}

			Answer.Correct = numbers[3*coordinateA + 3*coordinateB].ToString ();
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double rect_w = DrawAreaWidth / rows;
			double rect_h = DrawAreaHeight / columns;

			base.Draw (gr, area_width, area_height, rtl);

			for (int column = 0; column < columns; column++) {
				for (int row = 0; row < rows; row++) {
					gr.Rectangle (DrawAreaX + row * rect_w, DrawAreaY + column * rect_h, rect_w, rect_h);

					if (row != 3  || column != 3) {
						gr.DrawTextCentered (DrawAreaX + column * rect_w + rect_w / 2, 
							DrawAreaY + row * rect_h + rect_h / 2,
							(numbers[column + (row * 4)]).ToString());
					}
				}
			}

			gr.DrawTextCentered (DrawAreaX + 3 * rect_w + rect_w / 2,
				DrawAreaY + 3 * rect_h + rect_h / 2,
				"?");
			gr.Stroke ();
		}
	}
}
