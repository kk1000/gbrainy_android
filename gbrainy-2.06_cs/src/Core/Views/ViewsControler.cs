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

namespace gbrainy.Core.Views
{
	public class ViewsControler
	{
		GameSession.SessionStatus status;
		WelcomeView welcome;
		FinishView finish;
		Game game;

		public ViewsControler (GameSession session)
		{
			welcome = new WelcomeView ();
			finish = new FinishView (session);
		}

		public GameSession.SessionStatus Status {
			set { status = value;}
		}

		public Game Game {
			set { game = value;}
		}

		public IDrawable CurrentView {
			get {
				switch (status) {
				case GameSession.SessionStatus.NotPlaying:
					return welcome;
				case GameSession.SessionStatus.Playing:
				case GameSession.SessionStatus.Answered:
					return game;
				case GameSession.SessionStatus.Finished:
					return finish;
				default:
					throw new InvalidOperationException ("Invalid status");
				}
			}
		}
	}
}
