/*
 * Copyright (C) 2010-2011 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using NUnit.Framework;

using gbrainy.Core.Main;

namespace gbrainy.Test.Core
{
	[TestFixture]
	public class GameAnswerTest : UnitTestSupport
	{
		[TestFixtureSetUp]
		public void Construct ()
		{
			RegisterDefaultServices ();
		}

		// Test individual attributes (MatchAll follows a different logic path)
		[Test]
		public void Trim ()
		{
			GameAnswer answer = new GameAnswer ();

			answer.CheckAttributes = GameAnswerCheckAttributes.None;
			answer.Correct = "icon";
			Assert.AreEqual (true, answer.CheckAnswer ("icon"));
			Assert.AreEqual (false, answer.CheckAnswer (" icon "));

			answer.CheckAttributes = GameAnswerCheckAttributes.Trim;
			Assert.AreEqual (true, answer.CheckAnswer ("icon"));
			Assert.AreEqual (true, answer.CheckAnswer (" icon "));

			answer.CheckAttributes = GameAnswerCheckAttributes.MatchAll;
			Assert.AreEqual (true, answer.CheckAnswer ("icon"));
			Assert.AreEqual (false, answer.CheckAnswer (" icon "));

			answer.CheckAttributes = GameAnswerCheckAttributes.Trim | GameAnswerCheckAttributes.MatchAll;
			Assert.AreEqual (true, answer.CheckAnswer ("icon"));
			Assert.AreEqual (true, answer.CheckAnswer (" icon "));
		}

		[Test]
		public void IgnoreCase ()
		{
			GameAnswer answer = new GameAnswer ();

			answer.CheckAttributes = GameAnswerCheckAttributes.None;
			answer.Correct = "icon";
			Assert.AreEqual (true, answer.CheckAnswer ("icon"));
			Assert.AreEqual (false, answer.CheckAnswer ("ICON"));

			answer.CheckAttributes = GameAnswerCheckAttributes.IgnoreCase;
			Assert.AreEqual (true, answer.CheckAnswer ("icon"));
			Assert.AreEqual (true, answer.CheckAnswer ("ICON"));

			answer.CheckAttributes = GameAnswerCheckAttributes.MatchAll;
			Assert.AreEqual (true, answer.CheckAnswer ("icon"));
			Assert.AreEqual (false, answer.CheckAnswer ("ICON"));

			answer.CheckAttributes = GameAnswerCheckAttributes.IgnoreCase | GameAnswerCheckAttributes.MatchAll;
			Assert.AreEqual (true, answer.CheckAnswer ("icon"));
			Assert.AreEqual (true, answer.CheckAnswer ("ICON"));
		}

		[Test]
		public void IgnoreSpaces ()
		{
			GameAnswer answer = new GameAnswer ();

			answer.CheckAttributes = GameAnswerCheckAttributes.None;
			answer.Correct = "10 pm";
			Assert.AreEqual (true, answer.CheckAnswer ("10 pm"));
			Assert.AreEqual (false, answer.CheckAnswer ("10pm"));

			answer.CheckAttributes = GameAnswerCheckAttributes.IgnoreSpaces;
			Assert.AreEqual (true, answer.CheckAnswer ("10 pm"));
			Assert.AreEqual (true, answer.CheckAnswer ("10pm"));

			answer.CheckAttributes = GameAnswerCheckAttributes.MatchAll;
			Assert.AreEqual (true, answer.CheckAnswer ("10 pm"));
			Assert.AreEqual (false, answer.CheckAnswer ("10pm"));

			answer.CheckAttributes = GameAnswerCheckAttributes.IgnoreSpaces | GameAnswerCheckAttributes.MatchAll;
			Assert.AreEqual (true, answer.CheckAnswer ("10 pm"));
			Assert.AreEqual (true, answer.CheckAnswer ("10pm"));
		}

		[Test]
		public void MatchAllInOder ()
		{
			GameAnswer answer = new GameAnswer ();

			answer.CheckAttributes = GameAnswerCheckAttributes.MatchAllInOrder;
			answer.CheckExpression = "[0-9]+";
			answer.Correct = "10 | 20 | 30";

			Assert.AreEqual (true, answer.CheckAnswer ("10 20 30"));
			Assert.AreEqual (false, answer.CheckAnswer ("30 20 10"));
		}

		[Test]
		public void MatchAll ()
		{
			GameAnswer answer = new GameAnswer ();

			answer.CheckAttributes = GameAnswerCheckAttributes.MatchAll;
			answer.CheckExpression = "[0-9]+";
			answer.Correct = "10 | 20 | 30";
			Assert.AreEqual (true, answer.CheckAnswer ("10 20 30"));
			Assert.AreEqual (true, answer.CheckAnswer ("30 20 10"));
		}

		// Test attributes as used in real games

		[Test]
		public void DefaultAnswer ()
		{
			GameAnswer answer = new GameAnswer ();

			answer.Correct = "icon";
			Assert.AreEqual (true, answer.CheckAnswer ("icon"));

			answer.Correct = "icona";
			Assert.AreEqual (true, answer.CheckAnswer ("icona"));
		}

		[Test]
		public void DefaultAnswerOptions ()
		{
			GameAnswer answer = new GameAnswer ();

			answer.Correct = "option1 | option2";
			Assert.AreEqual (true, answer.CheckAnswer ("option1"));
			Assert.AreEqual (true, answer.CheckAnswer ("option2"));
			Assert.AreEqual (true, answer.CheckAnswer (" option2 "));

			Assert.AreEqual (false, answer.CheckAnswer ("option3"));
		}

		[Test]
		public void CheckPuzzleTimeNowAnswer ()
		{
			GameAnswer answer = new GameAnswer ();
			answer.Correct = "10 PM";
			answer.CheckAttributes = GameAnswerCheckAttributes.Trim | GameAnswerCheckAttributes.IgnoreCase | GameAnswerCheckAttributes.IgnoreSpaces;

			Assert.AreEqual (true, answer.CheckAnswer ("10 PM"));
			Assert.AreEqual (true, answer.CheckAnswer ("10 pm"));
			Assert.AreEqual (true, answer.CheckAnswer ("10pm"));
			Assert.AreEqual (true, answer.CheckAnswer ("10Pm"));
			Assert.AreEqual (true, answer.CheckAnswer (" 10Pm "));

			Assert.AreEqual (false, answer.CheckAnswer ("10 P"));
			Assert.AreEqual (false, answer.CheckAnswer ("10"));
		}

		[Test]
		public void CheckCalculationOperator ()
		{
			GameAnswer answer = new GameAnswer ();
			answer.Correct = "+ | -";
			answer.CheckExpression = "[+*-/]";
			answer.CheckAttributes = GameAnswerCheckAttributes.Trim | GameAnswerCheckAttributes.MatchAllInOrder;

			Assert.AreEqual (true, answer.CheckAnswer ("+ i -"));
			Assert.AreEqual (true, answer.CheckAnswer ("+ and -"));
			Assert.AreEqual (true, answer.CheckAnswer ("+ -"));
			Assert.AreEqual (true, answer.CheckAnswer ("+-"));

			Assert.AreEqual (false, answer.CheckAnswer ("- +"));
		}

		[Test]
		public void CheckPuzzleBuildTriangle ()
		{
			GameAnswer answer = new GameAnswer ();

			answer.Correct = "A | B | C";
			answer.CheckExpression = "[ABCDF]";
			answer.CheckAttributes = GameAnswerCheckAttributes.Trim | GameAnswerCheckAttributes.IgnoreCase | GameAnswerCheckAttributes.MatchAll;

			Assert.AreEqual (true, answer.CheckAnswer ("A B C"));
			Assert.AreEqual (true, answer.CheckAnswer ("C B A"));
			Assert.AreEqual (true, answer.CheckAnswer ("B C A"));
			Assert.AreEqual (true, answer.CheckAnswer ("A B C"));
			Assert.AreEqual (true, answer.CheckAnswer ("C A B"));
			Assert.AreEqual (true, answer.CheckAnswer ("a b c"));

			Assert.AreEqual (false, answer.CheckAnswer ("B C C"));
			Assert.AreEqual (false, answer.CheckAnswer ("B C"));
			Assert.AreEqual (false, answer.CheckAnswer ("BC"));
		}


		[Test]
		public void CheckPuzzlePercentage ()
		{
			GameAnswer answer = new GameAnswer ();

			answer.Correct = "10";
			answer.CheckExpression = "[0-9]+";

			Assert.AreEqual (true, answer.CheckAnswer ("10%"));
			Assert.AreEqual (true, answer.CheckAnswer ("10 %"));
			Assert.AreEqual (true, answer.CheckAnswer ("10"));

			answer.Correct = "9";
			Assert.AreEqual (true, answer.CheckAnswer ("9%"));
			Assert.AreEqual (true, answer.CheckAnswer ("9 %"));
			Assert.AreEqual (true, answer.CheckAnswer ("9"));
		}

		[Test]
		public void TwoNumbersAnswer ()
		{
			GameAnswer answer = new GameAnswer ();
			answer.Correct = "10 | 20";
			answer.CheckExpression = "[0-9]+";
			answer.CheckAttributes = GameAnswerCheckAttributes.Trim | GameAnswerCheckAttributes.MatchAll;

			// Right answers
			Assert.AreEqual (true, answer.CheckAnswer ("10 and 20"));
			Assert.AreEqual (true, answer.CheckAnswer ("10 i 20"));
			Assert.AreEqual (true, answer.CheckAnswer ("10 y 20"));
			Assert.AreEqual (true, answer.CheckAnswer ("10 20"));
			Assert.AreEqual (true, answer.CheckAnswer (" 10 20 "));

			Assert.AreEqual (true, answer.CheckAnswer ("20 and 10"));
			Assert.AreEqual (true, answer.CheckAnswer ("20 i 10"));
			Assert.AreEqual (true, answer.CheckAnswer ("20 y 10"));
			Assert.AreEqual (true, answer.CheckAnswer ("20 10"));
			Assert.AreEqual (true, answer.CheckAnswer (" 20 10 "));

			// Invalid answers
			Assert.AreEqual (false, answer.CheckAnswer (" 10 30 "));
			Assert.AreEqual (false, answer.CheckAnswer ("10"));
			Assert.AreEqual (false, answer.CheckAnswer ("20"));
			Assert.AreEqual (false, answer.CheckAnswer ("10 2"));
		}
	}
}
