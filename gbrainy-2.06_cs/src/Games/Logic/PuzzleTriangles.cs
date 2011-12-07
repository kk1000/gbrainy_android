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
	public class PuzzleTriangles : Game
	{
		int type;
		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Triangles");}
		}

		public override string Question {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("How many triangles of any size do you count in the figure below?");} 
		}

		public override string Tip {
			get { return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("A triangle can be embedded inside another triangle.");}
		}

		public override string Rationale {
			get {
				return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The triangles are made by connecting the following points: {0}"),
					(type == 0) ? "bdc, dcf, dfg, abd, ade, edg, acg, abg, bcg, afg, ecg, acd, acf, ace, adg, cdg." : 
					"dcf, ade, acg, afg, ecg, acd, acf, ace.");
			}
		}

		protected override void Initialize ()
		{
			if (CurrentDifficulty==GameDifficulty.Easy)
				type = 1;
			else
				type = random.Next (2);

			if (type == 0)	
				Answer.Correct = "16";
			else
				Answer.Correct = "8";
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = DrawAreaX + 0.1, y = DrawAreaY + 0.2;
			const double witdh = 0.6, height = 0.5;

			base.Draw (gr, area_width, area_height, rtl);
		
			gr.MoveTo (x, y);
			gr.LineTo (x + witdh, y);		
			gr.LineTo (x + witdh / 2, y + height / 2);
			gr.LineTo (x, y);
			gr.LineTo (x + 0.45, y + height /4);
			gr.Stroke ();
	
			if (type == 0) {
				gr.MoveTo (x + witdh / 2, y);
				gr.LineTo (x + witdh / 2, y + height / 2);
				gr.Stroke ();
			}

			gr.MoveTo (x + 0.152, y + 0.125);
			gr.LineTo (x + witdh, y);
			gr.Stroke ();

			if (Answer.Draw == false)
				return;

			// References
			gr.MoveTo (x - 0.02, y);
			gr.ShowPangoText ("a");

			gr.MoveTo (x + witdh /2  - 0.02, y);
			gr.ShowPangoText ("b");

			gr.MoveTo (x + witdh, y);
			gr.ShowPangoText ("c");

			gr.MoveTo (x + witdh /2  - 0.03, y + 0.07 - 0.02);
			gr.ShowPangoText ("d");

			gr.MoveTo (x + 0.11, y + 0.16);
			gr.ShowPangoText ("e");

			gr.MoveTo (x + 0.47, y + 0.16);
			gr.ShowPangoText ("f");

			gr.MoveTo (x + (witdh /2) - 0.01, y + 0.26);
			gr.ShowPangoText ("g");

		}
	}
}


