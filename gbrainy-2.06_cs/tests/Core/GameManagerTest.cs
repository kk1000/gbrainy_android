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
	public class GameManagerTest : UnitTestSupport
	{
		GameManager manager;

		[TestFixtureSetUp]
		public void Construct ()
		{
			RegisterDefaultServices ();
			manager = new GameManager ();
		}

		//Lists the games without tip
		public void GamesWithNoTip ()
		{
			int notip = 0;
			GameManager.GameLocator [] games = manager.AvailableGames;

			foreach (GameManager.GameLocator locator in games)
			{
				Game game = (Game) Activator.CreateInstance (locator.TypeOf, true);
				game.Variant = locator.Variant;
				if (game.TipString == String.Empty)
				{
					notip++;
					//Console.WriteLine ("Game with no tip {0} - {1}", game.Name, game);
				}
			}
			Console.WriteLine ("Games without tip: {0} ({1}%)", notip, notip * 100 / games.Length);
		}

		[Test]
		public void GamesNoDuplicatedName ()
		{
			Dictionary <string, bool> dictionary;
			GameManager.GameLocator [] games = manager.AvailableGames;
			dictionary = new Dictionary <string, bool> (games.Length);

			foreach (GameManager.GameLocator locator in games)
			{
				if (locator.IsGame == false)
					continue;

				Game game = (Game) Activator.CreateInstance (locator.TypeOf, true);
				game.Variant = locator.Variant;

				Assert.AreEqual (false, dictionary.ContainsKey (game.Name),
					String.Format ("Game name {0} is duplicated", game.Name));

				dictionary.Add (game.Name, true);
			}
		}

		[Test]
		public void CustomGamesRandomOrder ()
		{
			Dictionary <int, string> dictionary;
			GameManager.GameLocator [] games;

			List <int> list = new List <int> ();
			games = manager.AvailableGames;

			// Create a hash to map from game name to locator
			dictionary = new Dictionary <int, string> (games.Length);
			for (int i = 0; i < games.Length; i++)
			{
				if (games[i].IsGame == false)
					continue;

				Game game = (Game) Activator.CreateInstance (games[i].TypeOf, true);
				game.Variant = games[i].Variant;
				dictionary.Add (i, game.Name);
				list.Add (dictionary.Count - 1);
			}

			Game current;
			string name;

			manager.RandomOrder = false;
			manager.PlayList = list.ToArray ();

			for (int i = 0; i < list.Count; i++)
			{
				current = manager.GetPuzzle ();

				try
				{
					name = dictionary [i];
					Assert.AreEqual (true, name == current.Name);
				}
				catch (KeyNotFoundException)
				{

				}
			}
		}

		[Test]
		public void ColorBlind ()
		{
			Game game;
			GameManager.GameLocator [] games;

			GameManager manager = new GameManager ();
			manager.GameType = GameSession.Types.AllGames;
			manager.ColorBlind = true;
			games = manager.AvailableGames;

			for (int i = 0; i < games.Length; i++)
			{
				game = (Game) manager.GetPuzzle ();
				Assert.AreEqual (false, game.UsesColors);
			}
		}

		[Test]
		public void ResetAvailableGames ()
		{
			GameManager manager = new GameManager ();
			manager.GameType = GameSession.Types.AllGames;
			manager.LoadAssemblyGames ("gbrainy.Games.dll");
			Assert.AreNotEqual (0, manager.AvailableGames.Length);

			manager.ResetAvailableGames ();
			Assert.AreEqual (0, manager.AvailableGames.Length);
		}

		[Test]
		public void XmlGames ()
		{
			GameManager manager = new GameManager ();
			manager.GameType = GameSession.Types.AllGames;
			manager.LoadGamesFromXml ("test_games.xml");

			Assert.AreEqual (3, manager.AvailableGames.Length);
			int logic_variants = 0;
			int logic_games = 0;

			foreach (GameManager.GameLocator locator in manager.AvailableGames)
			{
				if (locator.GameType != GameTypes.LogicPuzzle)
					continue;

				if (locator.IsGame == true)
					logic_games++;
				else
					logic_variants++;

			}
			Assert.AreEqual (2, logic_games);
			Assert.AreEqual (1, logic_variants);
		}
	}
}
