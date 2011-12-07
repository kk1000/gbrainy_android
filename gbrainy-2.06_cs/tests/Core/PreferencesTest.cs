/*
 * Copyright (C) 2011 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
	public class PreferencesTest : UnitTestSupport
	{
		[TestFixtureSetUp]
		public void Construct ()
		{
			RegisterDefaultServices ();
			Preferences.ConfigPath = ".";
		}

		void SetBool (bool val)
		{
			Preferences.Clear ();
			Preferences.Set <bool> (Preferences.EnglishKey, val);
			Assert.AreEqual (val, Preferences.Get <bool> (Preferences.EnglishKey));
			Preferences.Save ();
			Preferences.Load ();
			Assert.AreEqual (val, Preferences.Get <bool> (Preferences.EnglishKey), "Error when reloaded saved preferences");
		}

		[Test]
		public void SetGetBool ()
		{
			SetBool (true);
			SetBool (false);
		}

		[Test]
		public void SetGetString ()
		{
			string val = "classic";

			Preferences.Clear ();
			Preferences.Set <string> (Preferences.ThemeKey, val);
			Assert.AreEqual (val, Preferences.Get <string> (Preferences.ThemeKey));
			Preferences.Save ();
			Preferences.Load ();
			Assert.AreEqual (val, Preferences.Get <string> (Preferences.ThemeKey), "Error when reloaded saved preferences");
		}


		[Test]
		public void SetGetInt ()
		{
			int val = 5;

			Preferences.Clear ();
			Preferences.Set <int> (Preferences.MinPlayedGamesKey, val);
			Assert.AreEqual (val, Preferences.Get <int> (Preferences.MinPlayedGamesKey));
			Preferences.Save ();
			Preferences.Load ();
			Assert.AreEqual (val, Preferences.Get <int> (Preferences.MinPlayedGamesKey), "Error when reloaded saved preferences");
		}
	}
}
