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
using System.Text;
using NUnit.Framework;

using gbrainy.Clients.Classical;
using gbrainy.Core.Main;

namespace gbrainy.Test.Clients.Classical
{
	[TestFixture]
	public class CommandLineTest : UnitTestSupport
	{
		[TestFixtureSetUp]
		public void Construct ()
		{
			RegisterDefaultServices ();
		}

		[Test]
		public void RandomOrder ()
		{
			CommandLine line;
			string [] args;

			args = new string [1];
			args [0] = "--norandom";

			line = new CommandLine (args);
			Assert.AreEqual (true, line.RandomOrder);
			line.Parse ();

			Assert.AreEqual (false, line.RandomOrder);
		}

		[Test]
		public void CustomGame ()
		{
			const int game_cnt = 5; // number of games to pass as parameters
			CommandLine line;
			string [] args;
			GameManager.GameLocator [] games;
			GameManager gm = new GameManager ();
			StringBuilder game_list = new StringBuilder ();
			int [] candidates; // Stores the indexes of the games passed as parameters
			int cand_idx = 0;

			games = gm.AvailableGames;
			candidates = new int [game_cnt];

			for (int i = 0; i < games.Length; i++)
			{
				if (games[i].IsGame == false)
					continue;

				Game game = (Game) Activator.CreateInstance (games[i].TypeOf, true);
				game.Variant = games[i].Variant;

				if (cand_idx > 0)
					game_list.Append (CommandLine.GAME_SEPARATOR + " ");

				game_list.Append (game.Name);
				candidates [cand_idx++] = i;

				if (cand_idx >= game_cnt)
					break;
			}

			args = new string [2];
			args [0] = "--customgame";
			args [1] = game_list.ToString ();

			line = new CommandLine (args);
			line.Parse ();

			Assert.AreEqual (cand_idx, line.PlayList.Length);

			cand_idx = 0;
			foreach (int idx in line.PlayList)
				Assert.AreEqual (idx, candidates [cand_idx++]);
		}
	}
}
