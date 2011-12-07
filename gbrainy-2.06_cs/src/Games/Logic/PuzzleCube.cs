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
using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Games.Logic
{
	public class PuzzleCube : Game
	{
		private char question;
		const int pairs = 4;
		const double figure_size = 0.1;
		const double txtoff_x = 0.04;
		const double txtoff_y = 0.03;

		private int[] question_answer = 
		{
			1, 4,
			6, 2,
			4, 1,
			2, 6,
		};

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Cube");}
		}

		public override string Question {
			get {return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("When you fold the figure below as a cube, which face on the figure is opposite the face with a {0} drawn on it? Answer the number written on the face."), question);} 
		}

		protected override void Initialize ()
		{
			int pair = random.Next (pairs);
			question = (char) (48 + question_answer[pair * 2]);
			
			char num = (char) (48 + question_answer[(pair * 2) + 1]);
			Answer.Correct = num.ToString ();
			
			Container container;
			DrawableArea drawable_area;
			double x = DrawAreaX + 0.1;
			double y = DrawAreaY + 0.2;

			container = new Container ();
			AddWidget (container);

			drawable_area = new DrawableArea (x + 0.1, y, figure_size, figure_size);
			drawable_area.DataEx = "1";
			container.AddChild (drawable_area);
			drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
			{
				e.Context.Rectangle (0, 0, figure_size, figure_size);
				e.Context.Stroke ();
				e.Context.MoveTo (txtoff_x, txtoff_y);
				e.Context.ShowPangoText ("1");
			};

			drawable_area = new DrawableArea (x + 0.2, y, figure_size, figure_size);
			drawable_area.DataEx = "2";
			container.AddChild (drawable_area);
			drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
			{
				e.Context.Rectangle (0, 0, figure_size, figure_size);
				e.Context.Stroke ();
				e.Context.MoveTo (txtoff_x, txtoff_y);
				e.Context.ShowPangoText ("2");
			};

			drawable_area = new DrawableArea (x + 0.2, y + 0.1, figure_size, figure_size);
			drawable_area.DataEx = "3";
			container.AddChild (drawable_area);
			drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
			{
				e.Context.Rectangle (0, 0, figure_size, figure_size);
				e.Context.Stroke ();
				e.Context.MoveTo (txtoff_x, txtoff_y);
				e.Context.ShowPangoText ("3");
			};

			drawable_area = new DrawableArea (x + 0.3, y + 0.1, figure_size, figure_size);
			drawable_area.DataEx = "4";
			container.AddChild (drawable_area);
			drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
			{
				e.Context.Rectangle (0, 0, figure_size, figure_size);
				e.Context.Stroke ();
				e.Context.MoveTo (txtoff_x, txtoff_y);
				e.Context.ShowPangoText ("4");
			};

			drawable_area = new DrawableArea (x + 0.4, y + 0.1, figure_size, figure_size);
			drawable_area.DataEx = "5";
			container.AddChild (drawable_area);
			drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
			{
				e.Context.Rectangle (0, 0, figure_size, figure_size);
				e.Context.Stroke ();
				e.Context.MoveTo (txtoff_x, txtoff_y);
				e.Context.ShowPangoText ("5");
			};

			drawable_area = new DrawableArea (x + 0.4, y + 0.2, figure_size, figure_size);
			drawable_area.DataEx = "6";
			container.AddChild (drawable_area);
			drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
			{
				e.Context.Rectangle (0, 0, figure_size, figure_size);
				e.Context.Stroke ();
				e.Context.MoveTo (txtoff_x, txtoff_y);
				e.Context.ShowPangoText ("6");
			};
		}
	}
}
