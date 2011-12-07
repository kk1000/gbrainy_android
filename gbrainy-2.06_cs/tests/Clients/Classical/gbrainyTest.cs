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
using System.Text;
using NUnit.Framework;

using gbrainy.Clients.Classical;
using gbrainy.Core.Main;
using gbrainy.Core.Services;

namespace gbrainy.Test.Clients.Classical
{
	[TestFixture]
	public class gbrainyTest : UnitTestSupport
	{
		TranslationsTest translations;

		public class TranslationsTest : ITranslations
		{
			public int Percentage { set; get; }
			public int TranslationPercentage {
				get { return Percentage; }
			}

			public void Init (string package, string localedir) {}
			public string GetString (string s) { return string.Empty; }
			public string GetPluralString (string s, string p, int n) { return string.Empty; }
		}

		[TestFixtureSetUp]
		public void Construct ()
		{
			RegisterDefaultServices ();
			translations =  new TranslationsTest ();
			ServiceLocator.Instance.RegisterService (typeof (ITranslations), translations);
		}

		[Test]
		public void ShowTranslationMessageHonorPercentage ()
		{
			GtkClient client = new GtkClient ();
			Preferences.ConfigPath = ".";
			Preferences.Clear ();

			translations.Percentage = client.MIN_TRANSLATION;
			Assert.AreEqual (false, client.ShowTranslationWarning ());
		}

		[Test]
		public void ShowTranslationMessageOnlyOnce ()
		{
			GtkClient client = new GtkClient ();
			Preferences.ConfigPath = ".";
			Preferences.Clear ();

			translations.Percentage = client.MIN_TRANSLATION - 1;
			Assert.AreEqual (true, client.ShowTranslationWarning ());
			Assert.AreEqual (false, client.ShowTranslationWarning ());
		}

		[Test]
		public void ShowTranslationMessageWhenChangingVersion ()
		{
			GtkClient client = new GtkClient ();
			Preferences.ConfigPath = ".";
			Preferences.Clear ();

			translations.Percentage = client.MIN_TRANSLATION - 1;
			Assert.AreEqual (true, client.ShowTranslationWarning ());
			Preferences.Set <string> (Preferences.EnglishVersionKey, "n.n.n");
			Assert.AreEqual (true, client.ShowTranslationWarning ());
		}
	}
}
