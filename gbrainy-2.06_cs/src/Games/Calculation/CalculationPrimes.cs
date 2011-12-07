/*
 * Copyright (C) 2009-2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Games.Calculation
{
	public class CalculationPrimes : Game
	{
		const int total_primes = 302;
		const int total_nums = 4;
		bool div3;
		int []numbers;
		int max_primeidx, answer, answer_idx;
		short []primes = new short []
		{
			2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31,
			37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79,
			83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137,
			139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193,
			197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257,
			263, 269, 271, 277, 281, 283, 293, 307, 311, 313, 317,
			331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389,
			397, 401, 409, 419, 421, 431, 433, 439, 443, 449, 457,
			461, 463, 467, 479, 487, 491, 499, 503, 509, 521, 523,
			541, 547, 557, 563, 569, 571, 577, 587, 593, 599, 601,
			607, 613, 617, 619, 631, 641, 643, 647, 653, 659, 661,
			673, 677, 683, 691, 701, 709, 719, 727, 733, 739, 743,
			751, 757, 761, 769, 773, 787, 797, 809, 811, 821, 823,
			827, 829, 839, 853, 857, 859, 863, 877, 881, 883, 887,
			907, 911, 919, 929, 937, 941, 947, 953, 967, 971, 977,
			983, 991, 997, 1009, 1013, 1019, 1021, 1031, 1033, 1039, 1049,
			1051, 1061, 1063, 1069, 1087, 1091, 1093, 1097, 1103, 1109, 1117,
			1123, 1129, 1151, 1153, 1163, 1171, 1181, 1187, 1193, 1201, 1213,
			1217, 1223, 1229, 1231, 1237, 1249, 1259, 1277, 1279, 1283, 1289,
			1291, 1297, 1301, 1303, 1307, 1319, 1321, 1327, 1361, 1367, 1373,
			1381, 1399, 1409, 1423, 1427, 1429, 1433, 1439, 1447, 1451, 1453,
			1459, 1471, 1481, 1483, 1487, 1489, 1493, 1499, 1511, 1523, 1531,
			1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583, 1597, 1601, 1607,
			1609, 1613, 1619, 1621, 1627, 1637, 1657, 1663, 1667, 1669, 1693,
			1697, 1699, 1709, 1721, 1723, 1733, 1741, 1747, 1753, 1759, 1777,
			1783, 1787, 1789, 1801, 1811, 1823, 1831, 1847, 1861, 1867, 1871,
			1873, 1877, 1879, 1889, 1901, 1907, 1913, 1931, 1933, 1949, 1951,
			1973, 1979, 1987, 1993, 1997, 1999
		};

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Primes");}
		}

		public override GameTypes Type {
			get { return GameTypes.Calculation;}
		}

		public override string Question {
			get { return String.Format (
				ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Which of the following numbers is a prime? A prime number is a positive integer that has exactly two different positive divisors, 1 and itself. Answer {0}, {1}, {2} or {3}."),
				Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3));}
		}

		public override string Tip {
			get { return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("If the sum of all digits in a given number is divisible by 3, then so is the number. For example 15 = 1 + 5 = 6, which is divisible by 3.");}
		}

		public override string Rationale {
			get { 
				return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The number {0} is a primer number."), answer);
			}
		}

		protected override void Initialize ()
		{
			Answer.CheckAttributes |= GameAnswerCheckAttributes.MultiOption;
			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
				div3 = true;
				max_primeidx = 55; // 263
				break;
			case GameDifficulty.Master:
				div3 = false;
				max_primeidx = total_primes;
				break;
			case GameDifficulty.Medium:
			default:
				div3 = true;
				max_primeidx = 95; // 503
				break;
			}

			numbers = new int [total_nums];

			for (int i = 0; i < numbers.Length; i++)
			 	numbers [i] = GenerateNonPrime ();

			answer_idx = random.Next (numbers.Length);
			answer = primes [random.Next (max_primeidx + 1)];
			numbers [answer_idx] = answer;
			Answer.SetMultiOptionAnswer (answer_idx, answer.ToString ());

			// Drawing objects
			double x = DrawAreaX + 0.25, y = DrawAreaY + 0.16;
			Container container = new Container (x, y,  1 - (x * 2), 0.6);
			AddWidget (container);

			for (int i = 0; i < numbers.Length; i++)
			{
				DrawableArea drawable_area = new DrawableArea (0.3, 0.1);
				drawable_area.X = x;
				drawable_area.Y = y + i * 0.15;
				container.AddChild (drawable_area);
				drawable_area.Data = i;
				drawable_area.DataEx = Answer.GetMultiOption (i);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int n = (int) e.Data;

					e.Context.SetPangoLargeFontSize ();
					e.Context.MoveTo (0.02, 0.02);
					e.Context.ShowPangoText (String.Format ("{0}) {1:##0.###}", Answer.GetMultiOption (n) , numbers [n]));
				};
			}
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.Draw (gr, area_width, area_height, rtl);

			gr.SetPangoLargeFontSize ();

			gr.MoveTo (0.1, 0.15);
			gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Choose one of the following:"));
		}

		short GenerateNonPrime ()
		{
			short num;
			while (true) {
				// Max value is a short
				num = (short) (100 + (random.Next (primes [max_primeidx] - 100)));

				if (num % 2 == 0)
					continue;

				if (div3 == false && num % 3 == 0)
					continue;

				if (Array.BinarySearch (primes, num) < 0)
					break;
			}
			return num;
		}
	}
}
