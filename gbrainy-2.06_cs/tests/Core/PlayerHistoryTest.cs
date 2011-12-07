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
using System.Collections.Generic;
using NUnit.Framework;

using gbrainy.Core.Main;

namespace gbrainy.Test.Core
{
	[TestFixture]
	public class PlayerHistoryTest : UnitTestSupport
	{
		PlayerHistory history;

		[TestFixtureSetUp]
		public void Construct ()
		{
			RegisterDefaultServices ();
			Preferences.Clear ();
		}

		[Test]
		public void Clean ()
		{
			GameSessionHistory game = new GameSessionHistory ();
			game.GamesPlayed = Preferences.Get <int> (Preferences.MinPlayedGamesKey);

			history = new PlayerHistory ();
			history.ConfigPath = ".";
			history.Clean ();
			history.SaveGameSession (game);

			Assert.AreEqual (1, history.Games.Count);
			history.Clean ();
			Assert.AreEqual (0, history.Games.Count);
		}

		[Test]
		public void SaveLoad ()
		{
			GameSessionHistory game = new GameSessionHistory ();
			game.GamesPlayed = Preferences.Get <int> (Preferences.MinPlayedGamesKey);
			game.MemoryScore = 20;

			history = new PlayerHistory ();
			history.ConfigPath = ".";
			history.SaveGameSession (game);

			history = new PlayerHistory ();
			history.ConfigPath = ".";
			history.Load ();

			Assert.AreEqual (1, history.Games.Count);
			Assert.AreEqual (20, history.Games[0].MemoryScore);
		}
	}
}
