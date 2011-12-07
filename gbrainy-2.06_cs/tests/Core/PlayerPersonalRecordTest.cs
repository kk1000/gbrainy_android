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
	public class PlayerPersonalRecordTest : UnitTestSupport
	{
		[TestFixtureSetUp]
		public void Construct ()
		{
			RegisterDefaultServices ();
			Preferences.Clear ();
		}

		[Test]
		public void MinGamesRecord ()
		{
			PlayerHistory history;

			GameSessionHistory game = new GameSessionHistory ();
			game.GamesPlayed = Preferences.Get <int> (Preferences.MinPlayedGamesKey);

			history = new PlayerHistory ();
			history.ConfigPath = ".";
			history.Clean ();

			for (int i = 0; i < PlayerPersonalRecord.MIN_GAMES_RECORD - 2; i++)
			{
				history.SaveGameSession (game);
			}

			game.LogicScore = 10;
			history.SaveGameSession (game);

			Assert.AreEqual (0, history.GetLastGameRecords ().Count,
				"Did not reach MinPlayedGamesKey, the game should not be a person record yet");

			game.LogicScore = 30;
			history.SaveGameSession (game);

			Assert.AreEqual (1, history.GetLastGameRecords ().Count,
				"We have just recorded a personal record");

			game.LogicScore = 20;
			history.SaveGameSession (game);

			Assert.AreEqual (0, history.GetLastGameRecords ().Count,
				"Score saved was lower than previous, no record");
		}
	}
}
