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
	public class PuzzleHandshakes : Game
	{
		int people, handshakes;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Handshakes");}
		}

		public override string Question {
			get {return String.Format (
				ServiceLocator.Instance.GetService <ITranslations> ().GetString ("All attendees to a party are introduced to one another. {0} handshakes are made in total. How many people are attending the party?"), 				handshakes);
			}
		}

		public override string Tip {
			get { return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Try to imagine a situation in which you are meeting a small number of people.");}
		}

		protected override void Initialize ()
		{
			handshakes = 0;

			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
				people = 4 + random.Next (4);
				break;
			case GameDifficulty.Master:
				people = 5 + random.Next (8);
				break;		
			case GameDifficulty.Medium:
			default:
				people = 5 + random.Next (4);
				break;		
			}
		
			for (int i = 1; i < people; i++)
				handshakes += i;
		
			Answer.Correct = people.ToString ();
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.Draw (gr, area_width, area_height, rtl);
			gr.DrawImageFromAssembly ("handshake.svg", 0.2, 0.6, 0.6, 0.3);
		}
	}
}
