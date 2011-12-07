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
using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Games.Logic
{
	public class PuzzleClocks : Game
	{
		private const double figure_size = 0.3;
		private const double radian = Math.PI / 180;
		private int addition;
		private int []handles;
		private const int clocks = 4;
		private const int handle_num = 2;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Clocks");}
		}

		public override string Question {
			get {return (String.Format (
				// Translators: {0} is replaced by 'Figure X'
				ServiceLocator.Instance.GetService <ITranslations> ().GetString ("To what number should the large handle of the '{0}' clock point? Answer using numbers."),
				Answer.GetFigureName (3)));}
		}

		public override string Rationale {
			get {
				// Translators: {0} is replaced by 'Figure X'
				return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString (
					"Starting from the first clock, add {1} to the number obtained by appending the values to which the hands point. For example, the values of the hands for '{0}' are {3} ({2} + {1})."),
					Answer.GetFigureName (3), addition, handles [4].ToString () + handles [5].ToString (), handles [6].ToString () + handles [7].ToString ());
			}
		}

		public override string Tip {
			get { return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The clocks do not follow the time logic.");}
		}

		protected override void Initialize ()
		{
			int position;

			// In all these cases the clock logic clearly do not work since the small hand is in the next hour
			switch (random.Next (3)) {
			case 0:
				position = 48;
				addition = 5;
				break;
			case 1:
				position = 38;
				addition = 15;
				break;
			case 2:
			default:
				position = 24;
				addition = 5;
				break;
			}

			handles = new int [clocks * handle_num];

			for (int i = 0; i < handles.Length; i++, i++)
			{
				handles [i] = position / 10;
				handles [i + 1] = position - ((position / 10) * 10);
				position += addition;
			}

			Answer.Correct = handles[7].ToString ();

			// First row
			HorizontalContainer container = new HorizontalContainer (DrawAreaX, 0.05, 0.8, 0.45);
			DrawableArea drawable_area;
			AddWidget (container);

			drawable_area = new DrawableArea (0.8 / 2, 0.4);
			drawable_area.Sensitive = false;
			container.AddChild (drawable_area);

			drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
			{
				DrawClock (e.Context, 0.2, 0.16, handles[0], handles[1], true);
				e.Context.DrawTextCentered (drawable_area.Width / 2, 0.36, Answer.GetFigureName (0));
				e.Context.Stroke ();
			};

			drawable_area = new DrawableArea (0.8 / 2, 0.4);
			drawable_area.Sensitive = false;
			container.AddChild (drawable_area);

			drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
			{
				DrawClock (e.Context, 0.2, 0.16, handles[2], handles[3], true);
				e.Context.MoveTo (0.03, 0.29);
				e.Context.DrawTextCentered (drawable_area.Width / 2, 0.36, Answer.GetFigureName (1));
				e.Context.Stroke ();
			};

			// Second row
			container = new HorizontalContainer (DrawAreaX, 0.05 + 0.45, 0.8, 0.45);
			AddWidget (container);

			drawable_area = new DrawableArea (0.8 / 2, 0.4);
			drawable_area.Sensitive = false;
			container.AddChild (drawable_area);

			drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
			{
				DrawClock (e.Context, 0.2, 0.16, handles[4], handles[5], true);
				e.Context.DrawTextCentered (drawable_area.Width / 2, 0.36, Answer.GetFigureName (2));
				e.Context.Stroke ();
			};

			drawable_area = new DrawableArea (0.8 / 2, 0.4);
			drawable_area.Sensitive = false;
			container.AddChild (drawable_area);

			drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
			{
				DrawClock (e.Context, 0.2, 0.16, handles[6], handles[7], Answer.Draw == true);
				e.Context.MoveTo (0.03, 0.29);
				e.Context.DrawTextCentered (drawable_area.Width / 2, 0.36, Answer.GetFigureName (3));
				e.Context.Stroke ();
			};

			/*DateTime dt1 = new DateTime (2008, 2, 20, handles[0], handles[1] * 5, 0);
			DateTime dt2 = new DateTime (2008, 2, 20, handles[2], handles[3] * 5, 0);
			Console.WriteLine ("t1 {0}", dt1);
			Console.WriteLine ("t2 {0}", dt2);
			Console.WriteLine ("Time diff {0} from 1st to 2nd", dt2-dt1);

			dt1 = new DateTime (2008, 2, 20, handles[2], handles[3] * 5, 0);
			dt2 = new DateTime (2008, 2, 20, handles[4], handles[5] * 5, 0);
			Console.WriteLine ("t1 {0}", dt1);
			Console.WriteLine ("t2 {0}", dt2);
			Console.WriteLine ("Time diff {0} from 1st to 2nd", dt2-dt1);

			dt1 = new DateTime (2008, 2, 20, handles[4], handles[5] * 5, 0);
			dt2 = new DateTime (2008, 2, 20, handles[6], handles[7] * 5, 0);
			Console.WriteLine ("t1 {0}", dt1);
			Console.WriteLine ("t2 {0}", dt2);
			Console.WriteLine ("Time diff {0} from 1st to 2nd", dt2-dt1);*/

		}

		static void DrawClock (CairoContextEx gr, double x, double y, int hand_short, int hand_large, bool draw_large)
		{
			const double radius = figure_size / 2;
			double x0, y0;
			int num, degrees;

			gr.Arc (x, y, radius, 0, 2 * Math.PI);
			gr.Stroke ();
			for (degrees = 0; degrees < 360; degrees+= 30) {
				x0 = radius * Math.Cos (degrees * radian);
				y0 = radius * Math.Sin (degrees * radian);
				 // Small lines
				gr.MoveTo (x + 0.9 * x0, y + 0.9 * y0);
				gr.LineTo (x + x0, y + y0);
				gr.Stroke ();
				// Numbers
				num = (degrees / 30) + 3;
				if (num > 12) num = num - 12;

				gr.DrawTextCentered (x + x0 * 0.75,  y + y0 * 0.75, num.ToString ());
				gr.Stroke ();
			}

			if (draw_large) {
				// Hand Large
				degrees = (hand_large - 3) * 30;
				x0 = radius * Math.Cos (degrees * radian);
				y0 = radius * Math.Sin (degrees * radian);
				gr.MoveTo (x, y);
				gr.LineTo (x + x0 * 0.55, y + y0 * 0.55);
				gr.Stroke ();
			}
			// Hand Short
			degrees = (hand_short - 3) * 30;
			x0 = radius * Math.Cos (degrees * radian);
			y0 = radius * Math.Sin (degrees * radian);
			gr.MoveTo (x, y);
			gr.LineTo (x + x0 * 0.4, y + y0 * 0.4);
			gr.Stroke ();
		}
	}
}
