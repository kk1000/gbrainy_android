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
using System.Text;

using gbrainy.Core.Main;
using gbrainy.Core.Services;

namespace gbrainy.Games.Logic
{
	public class PuzzleNumericRelation : Game
	{
		private const int group_size = 3;
		private int sum_value;
		private int question;
		private int[] numbers;
		private Formula formula;
		private const int max_num = 9;

		public enum Formula
		{
			AllAdding,
			ThirdMultiply,
			ThirdSubstracting,
			Length
		};

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Numeric relation");}
		}

		public override string Question {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("What number should replace the question mark?");} 
		}

		public override string Tip {
			get { return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The numbers are related arithmetically.");}
		}

		public override string Rationale {
			get {
				switch (formula) {
				case Formula.AllAdding:
					// Translators: {0} is always replaced by the number 3
					return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Every group of {0} numbers sums exactly {1}."), group_size, sum_value);
				case Formula.ThirdMultiply:
					return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Divide the sequence in groups of three numbers. Every third number is calculated by multiplying by the two previous ones.");
				case Formula.ThirdSubstracting:
					return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Divide the sequence in groups of three numbers. Every third number is calculated by subtracting the second number from the first.");
				default:
					throw new InvalidOperationException ("Invalid Value");
				}
			}
		}

		protected override void Initialize ()
		{
			bool validate = false;
			int group = 0, inc;

			if (CurrentDifficulty == GameDifficulty.Easy) {
				sum_value = 10 + random.Next (10);
				inc = 5;
			}
			else {
				sum_value = 30 + random.Next (10);
				inc = 12;
			}

			while (validate == false)
			{
				question = 1 + random.Next (max_num - 2);
				formula = (Formula) random.Next ((int) Formula.Length);
				numbers =  new int [max_num];
		
				for (int i = 0; i < max_num; i++) {
					if (group == group_size - 1) {	
						switch (formula) {
						case Formula.AllAdding:
							numbers[i] = sum_value - numbers[i - 1] - numbers[i - 2];
							break;
						case Formula.ThirdMultiply:
							numbers[i] = numbers[i - 1] * numbers[i - 2];
							break;
						case Formula.ThirdSubstracting:
							numbers[i] = numbers[i - 2] - numbers[i - 1];
							break;
						}
						group = 0;
						continue;
					}
					numbers[i] = 1 + random.Next (inc);
					group++;
				}

				validate = Validate (numbers, formula, question);
			}
			Answer.Correct = numbers[question].ToString ();
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			StringBuilder sequence = new StringBuilder (64);

			base.Draw (gr, area_width, area_height, rtl);

			gr.SetPangoLargeFontSize ();

			for (int num = 0; num < max_num - 1; num++)
			{
				if (num == question) {
					sequence.Append ("?, ");
				} else {
					sequence.Append (numbers[num]);
					sequence.Append (", ");
				}
			}
			sequence.Append (numbers[max_num - 1]);
			gr.DrawTextCentered (0.5, DrawAreaY + 0.3, sequence.ToString ());
		}

		/*
			The objective of this validation is to make sure that the sequence
			presented is not also a valid sequence using other logic criteria
		*/	
		public static bool Validate (int[] numbers, Formula formula, int question)
		{
			switch (formula) {
			case Formula.AllAdding:
				return ValidateAddinginGroupsOfTree (numbers, question); // In 0.15% of the cases returns invalid
			case Formula.ThirdMultiply:
				break;
			case Formula.ThirdSubstracting:
				return ValidateAllAdding (numbers, question); // In 8.3% of the cases returns invalid
			}

			return true;
		}

		static bool ThirdSubstractingGroup (int[] numbers, int start, int len)
		{		
			return (numbers [start + 1] + numbers [start + 0]) == numbers [start + 2];
		}

		static bool ValidateAddinginGroupsOfTree (int[] numbers, int question)
		{
			int group_f1, group_f2, group_q;			

			group_q = question / group_size;
			switch (group_q) {
			case 0:
				group_f1 = 1;
				group_f2 = 2;
				break;
			case 1:
				group_f1 = 0;
				group_f2 = 2;
				break;
			case 2:
				group_f1 = 0;
				group_f2 = 1;
				break;
			default:
				throw new InvalidOperationException ("group_q");
			}

			return (! (ThirdSubstractingGroup (numbers, group_f1 * group_size, group_size) == true &&
				ThirdSubstractingGroup (numbers, group_f2 * group_size, group_size) == true));
		}		

		static int SumGroup (int[] numbers, int start, int len)
		{
			int sum = 0;
		
			for (int n = start; n < len + start; n++)
				sum += numbers[n];

			return sum;
		}

		// Taken the sequence (a b c) (d ? f) (g h i)
		// If a + b + c = g + h = i you can calculate a replament for ? that makes then add too
		static bool ValidateAllAdding (int[] numbers, int question)
		{
			int group_f1, group_f2, group_q;			

			group_q = question / group_size;
			switch (group_q) {
			case 0:
				group_f1 = 1;
				group_f2 = 2;
				break;
			case 1:
				group_f1 = 0;
				group_f2 = 2;
				break;
			case 2:
				group_f1 = 0;
				group_f2 = 1;
				break;
			default:
				throw new InvalidOperationException ("group_q");
			}

			return SumGroup (numbers, group_f1 * group_size, group_size) != SumGroup (numbers, group_f2 * group_size, group_size);
		}
	}
}
