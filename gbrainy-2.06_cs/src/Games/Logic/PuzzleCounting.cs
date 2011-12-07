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
	public class PuzzleCounting : Game
	{
		enum GameType
		{
			Machine,
			Fence,
			Present,
			Total
		}

		string question, answer;
		GameType gametype;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Counting");}
		}

		public override string Question {
			get {return question; }
		}

		public override string Rationale {
			get { return answer;}
		}

		protected override void Initialize ()
		{
			int ans, var, total;

			gametype = (GameType) random.Next ((int) GameType.Total);

			switch (gametype)
			{
			case GameType.Machine:
				var = 2 + random.Next (5);
				total = 50 + random.Next (100);
				question = String.Format (
					ServiceLocator.Instance.GetService <ITranslations> ().GetPluralString ("We have a {0} meter piece of fabric.", "We have a {0} meters piece of fabric.", total),
					total);
				question += " ";
				question += String.Format (
					ServiceLocator.Instance.GetService <ITranslations> ().GetPluralString ("Machine A takes {0} second to cut 1 meter of this fabric. How many seconds does Machine A take to cut the entire piece of fabric into 1 meter pieces?",
						"Machine A takes {0} seconds to cut 1 meter of this fabric. How many seconds does Machine A take to cut the entire piece of fabric into 1 meter pieces?"
						, var), var);
				answer = String.Format (
					// Translators: {0} is always a number greater than 1
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("With the {0} cut, Machine A creates two 1 meter pieces."), (total - 1));

				ans = (total - 1) * var;
				break;

			case GameType.Fence:
				total = 20 + random.Next (20);
				ans = 4 * total - 4;
				question = String.Format (
					// Translators: {0} is a number
					ServiceLocator.Instance.GetService <ITranslations> ().GetPluralString (
						"A fence is built to enclose a square shaped region. {0} fence pole is used in each side of the square. How many fence poles are used in total?",
						"A fence is built to enclose a square shaped region. {0} fence poles are used in each side of the square. How many fence poles are used in total?",
						total),
					total);
					// Translators: {0} is a number
				answer = String.Format (
					ServiceLocator.Instance.GetService <ITranslations> ().GetPluralString (
						"There is {0} fence pole since the poles on the corners of the square are shared.",
						"There are {0} fence poles since the poles on the corners of the square are shared.",
						ans)
					, ans);
				break;

			case GameType.Present:
				int present = 5 + random.Next (20);
				total = present + 2;
				ans = total;
				question = String.Format (
					// Translators: {0} is a number
					ServiceLocator.Instance.GetService <ITranslations> ().GetPluralString (
						"Wrapping an anniversary present costs one monetary unit. The anniversary present costs {0} monetary unit more than the cost to wrap it. How much does it cost to both purchase and wrap the present?",
						"Wrapping an anniversary present costs one monetary unit. The anniversary present costs {0} monetary units more than the cost to wrap it. How much does it cost to both purchase and wrap the present?",
						present),
					present);

				answer = String.Format (
					ServiceLocator.Instance.GetService <ITranslations> ().GetPluralString (
					"It is the cost of the present, {0} monetary unit, plus one monetary unit of the wrapping.",
					"It is the cost of the present, {0} monetary units, plus one monetary unit of the wrapping.",
					present + 1), present + 1);
				break;
			default:
				throw new Exception ("Unexpected value");
			}

			Answer.Correct = (ans).ToString ();
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.Draw (gr, area_width, area_height, rtl);

			if (gametype == GameType.Present) {
				gr.DrawImageFromAssembly ("present.svg", 0.2, 0.4, 0.6, 0.2);
			} else {
				if (gametype == GameType.Fence)
				{
					double x105, y105, y;
					const double x = 0.35;
					const double figure_size = 0.4;

					x105 = figure_size * Math.Cos (105 * Math.PI / 180);
					y105 = figure_size * Math.Sin (105 * Math.PI / 180);

					y = (1 - y105) / 2;
					gr.MoveTo (x, y);
					gr.LineTo (x + x105, y + y105);
					gr.LineTo (x + x105 + figure_size, y + y105);
					gr.Stroke ();
					gr.MoveTo (x + figure_size, y);
					gr.LineTo (x + figure_size + x105, y + y105);
					gr.Stroke ();
					gr.MoveTo (x, y);
					gr.LineTo (x + figure_size, y);
					gr.Stroke ();
				}
			}
		}
	}
}
