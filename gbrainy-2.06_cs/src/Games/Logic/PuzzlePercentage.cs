/*
 * Copyright (C) 2009 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
	public class PuzzlePercentage : Game
	{
		enum GameType
		{
			Discount,
			Sales,
			Water,
			Total
		}

		string question, answer, svg_image;
		GameType gametype;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Percentage");}
		}

		public override string Question {
			get {return question; }
		}

		public override string Rationale {
			get {
				return answer;
			}
		}

		protected override void Initialize ()
		{
			int ans;

			gametype = (GameType) random.Next ((int) GameType.Total);

			switch (gametype)
			{
			case GameType.Discount:
				double price, discount, paid;

				do
				{
					discount = 10 + random.Next (30);
					price = 100 + random.Next (100);
					paid = price - (price * discount / 100);

				}  while (paid != Math.Truncate (paid));

				question = String.Format (
					ServiceLocator.Instance.GetService <ITranslations> ().GetPluralString (
						"After getting {0}% discount you have paid {1} monetary unit for a TV set. What was the original price of the TV set?",
						"After getting {0}% discount you have paid {1} monetary units for a TV set. What was the original price of the TV set?",
						(int) paid),
					discount, paid);
				ans = (int)price;
				svg_image = "tv_set.svg";
				break;
			case GameType.Sales:
				double sales, increase, previous;

				do
				{
					previous = 10 + random.Next (90);
					increase = 10 + random.Next (20);
					sales = previous + (previous * increase / 100);

				}  while (sales != Math.Truncate (sales));

			
				question = String.Format (
					ServiceLocator.Instance.GetService <ITranslations> ().GetPluralString (
						"John's shop had sales of {0} monetary unit. This was an increase of {1}% over last month. What were last month sales?",
						"John's shop had sales of {0} monetary units. This was an increase of {1}% over last month. What were last month sales?",	
						(int) sales),
					sales, increase);
				ans = (int) previous;
				svg_image = "shop.svg";
				break;
			case GameType.Water:
				double decrease, percentage;

				do
				{
					decrease = (1 + random.Next (70));
					percentage = decrease / (100 - decrease) * 100;

				} while (percentage != Math.Truncate (percentage));
			
				question = String.Format (
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The amount of water in a bucket decreases by {0}%. By what percentage must the amount of water increase to reach its original value?"),
					decrease);

				answer = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The objective is to obtain the same total amount.");
				ans = (int) percentage;
				svg_image = "bucket.svg";
				break;
			default:
				throw new Exception ("Unexpected value");
			}

			Answer.Correct = (ans).ToString ();
			Answer.CheckExpression = "[0-9]+";
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.Draw (gr, area_width, area_height, rtl);

			if (String.IsNullOrEmpty (svg_image) == false)
				gr.DrawImageFromAssembly (svg_image, 0.25, 0.25, 0.5, 0.5);
		}
	}
}
