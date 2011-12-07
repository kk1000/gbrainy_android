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

using gbrainy.Core.Main.Verbal;

namespace gbrainy.Test.Core
{
	[TestFixture]
	public class AnalogiesFactoryTest : UnitTestSupport
	{
		[TestFixtureSetUp]
		public void Construct ()
		{
			RegisterDefaultServices ();
			AnalogiesFactory.Read ("test_analogies.xml");
		}

		[Test]
		public void MultipleOptionsWithIgnore ()
		{
			Dictionary <int, Analogy> analogies;

			// Checks also the <ignore> parameter
			analogies = AnalogiesFactory.Get (Analogy.Type.MultipleOptions);
			Assert.AreEqual (2, analogies.Count);
		}

		[Test]
		public void PairOfWordsOptions ()
		{
			Dictionary <int, Analogy> analogies;

			analogies = AnalogiesFactory.Get (Analogy.Type.PairOfWordsOptions);
			Assert.AreEqual (1, analogies.Count);
		}

		[Test]
		public void QuestionAnswer ()
		{
			Dictionary <int, Analogy> analogies;

			analogies = AnalogiesFactory.Get (Analogy.Type.QuestionAnswer);
			Assert.AreEqual (1, analogies.Count);
		}

		[Test]
		public void PairOfWordsCompare ()
		{
			Dictionary <int, Analogy> analogies;

			analogies = AnalogiesFactory.Get (Analogy.Type.PairOfWordsCompare);
			Assert.AreEqual (2, analogies.Count);
		}
	}
}
