/*
 * Copyright (C) 2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
	public class PuzzleRelatedNumbers : Game
	{
		// A sequence of numbers (right) (middle) (left)
		// Where left and middle are related
		struct LineOfNumbers
		{
			internal int [] right;
			internal int middle;
			internal int [] left;

			internal int GetNumber (int pos)
			{
				if (pos < right.Length)
					return right [pos];

				if (pos == right.Length)
					return middle;

				return left [pos - right.Length - 1];
			}

			internal bool IsMiddle (int pos)
			{
				return pos == right.Length;
			}

			internal int TotalNumbers {
				get { return 1 + right.Length + left.Length; }
			}

			internal int Middle {
				get { return middle; }
			}

		};

		enum Operation
		{
			Add,		// (left + right) is equal X
			AddHalf,	// (left + right) /2 is equal X
			AddDouble,	// (left + right) *2 is equal X
			Total,
		}

		LineOfNumbers [] lines;
		Operation operation;

		// Generates a line of the puzzle based on the number of digits of the right / left side
		LineOfNumbers CreateNumbers (Operation operation, int digits)
		{
			LineOfNumbers line = new LineOfNumbers ();

			line.right = new int [digits];
			line.left = new int [digits];
			int max_number, added;

			switch (operation) {
			case Operation.AddHalf:
				max_number = 32;
				break;
			case Operation.Add:
				max_number = 10;
				break;
			case Operation.AddDouble:
				max_number = 16;
				break;
			default:
				throw new InvalidOperationException ("Invalid value");
			}

			while (true)
			{
				int right, left;

				right = left = 0;

				// Generate random numbers
				for (int i = 0; i < digits; i++)
				{
					line.right[i] = 1 + random.Next (max_number);
					line.left[i] = 1 + random.Next (max_number);

					right += line.right [i];
					left += line.left [i];
				}

				added = right + left;

				if (operation == Operation.Add)
				{
					if (added > 31 || added < 3)
						continue;
				}

				if (operation == Operation.AddHalf)
				{
					if (added % 2 != 0)
						continue;

					added = added / 2;

					if (added > 64)
						continue;
				}

				if (operation == Operation.AddDouble)
				{
					added = added * 2;
					if (added > 99 || added < 3)
						continue;
				}

				line.middle = added;

				bool found = false;
				// Check if the middle has been already repeated
				foreach (LineOfNumbers prev in lines)
				{
					if (prev.Middle == line.Middle)
					{
						found = true;
						break;
					}
				}

				if (found == false)
					break;
			}
			return line;
		}

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Related numbers");}
		}

		public override string Question {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("In the grid below, which number should replace the question mark?");}
		}

		public override string Tip {
			get { return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The pattern is arithmetical and works horizontally."); }
		}

		public override string Rationale {
			get {
				switch (operation) {
				case Operation.AddHalf:
					return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The number in the middle of every row is half of the sum of the other numbers in the row.");
				case Operation.Add:
					return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The number in the middle of every row is the sum of the other numbers in the row.");
				case Operation.AddDouble:
					return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The number in the middle of every row is the double of the sum of the other numbers in the row.");
				default:
					throw new InvalidOperationException ("Invalid value");
				}
			}
		}

		protected override void Initialize ()
		{
			operation = (Operation) random.Next ((int) Operation.Total);
			lines = new LineOfNumbers [7];
			lines [0] = CreateNumbers (operation, 1);
			lines [1] = CreateNumbers (operation, 2);
			lines [2] = CreateNumbers (operation, 3);
			lines [3] = CreateNumbers (operation, 4);
			lines [4] = CreateNumbers (operation, 3);
			lines [5] = CreateNumbers (operation, 2);
			lines [6] = CreateNumbers (operation, 1);

			Answer.Correct = (lines [lines.Length - 1].Middle).ToString ();
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			int rows = 7, columns = 9;
			double rect_w = DrawAreaWidth / columns;
			double rect_h = DrawAreaHeight / rows;
			int first_column;
			string text;

			base.Draw (gr, area_width, area_height, rtl);
			gr.SetPangoLargeFontSize ();

			for (int row = 0; row < rows; row++)
			{
				for (int column = 0; column < columns; column++)
				{
					gr.Rectangle (DrawAreaX + column * rect_w, DrawAreaY + row * rect_h, rect_w, rect_h);
					gr.Stroke ();

					if (row >= lines.Length)
						continue;

					first_column = (columns - lines[row].TotalNumbers) / 2;

					if (column < first_column || column - first_column >= lines [row].TotalNumbers)
						continue;

					if (row + 1 == lines.Length && lines [row].IsMiddle (column - first_column))
						text = "?";
					else
						text = lines [row].GetNumber (column - first_column).ToString ();

					gr.DrawTextCentered (DrawAreaX + (column * rect_w) + rect_w / 2,
							DrawAreaY + (row * rect_h) + rect_h / 2,
							text.ToString());
				}
			}
		}
	}
}
