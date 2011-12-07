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
using gbrainy.Core.Main.Xml;

namespace gbrainy.Test.Core
{
	[TestFixture]
	public class GameXmlFactoryTest : UnitTestSupport
	{
		GamesXmlFactory factory;
		List <GameXmlDefinition> definitions;
		GameXmlDefinition definition;

		[TestFixtureSetUp]
		public void Construct ()
		{
			RegisterDefaultServices ();
		}

		[Test]
		public void BasicGameDefinition ()
		{
			factory = new GamesXmlFactory ();
			factory.Read ("test_games.xml");
			definitions = factory.Definitions;

			definition = definitions [0];
			Assert.AreEqual ("Clock Rotation", definition.Name);
			Assert.AreEqual (0, definition.Variants.Count);
			Assert.AreEqual (GameTypes.LogicPuzzle, definition.Type);
			Assert.AreEqual (GameDifficulty.Medium | GameDifficulty.Master, definition.Difficulty);
			Assert.AreEqual ("Rationale text", definition.Rationale.String);
			Assert.AreEqual ("How many degrees rotates the minute hand of a clock?", definition.Question.String);
			Assert.AreEqual ("How many degrees rotates the minute hand of a clocks?", definition.Question.PluralString);
			Assert.AreEqual ("[rslt]", definition.AnswerText);


		}

		[Test]
		public void DrawingElements ()
		{	
			factory = new GamesXmlFactory ();
			factory.Read ("test_games.xml");
			definitions = factory.Definitions;

			definition = definitions [0];
			Assert.AreEqual (2, definition.DrawingObjects.Length);

			// Check image drawing object
			Assert.AreEqual (typeof (gbrainy.Core.Main.Xml.ImageDrawingObject), definition.DrawingObjects [0].GetType ());

			ImageDrawingObject image = definition.DrawingObjects [0] as ImageDrawingObject;

			Assert.AreEqual ("clock.svg", image.Filename);
			Assert.AreEqual (0.30, image.X);
			Assert.AreEqual (0.40, image.Y);
			Assert.AreEqual (0.50, image.Width);
			Assert.AreEqual (0.60, image.Height);

			// Check text drawing object
			Assert.AreEqual (typeof (gbrainy.Core.Main.Xml.TextDrawingObject), definition.DrawingObjects [1].GetType ());

			TextDrawingObject text = definition.DrawingObjects [1] as TextDrawingObject;
			Assert.AreEqual ("Sample text for unit tests", text.Text);
			Assert.AreEqual (0.5, text.X);
			Assert.AreEqual (0.4, text.Y);
			Assert.AreEqual (true, text.Centered);
			Assert.AreEqual (TextDrawingObject.Sizes.Large, text.Size);
		}

		[Test]
		public void GameDefinitionWithVariants ()
		{
			GameXmlDefinitionVariant variant;
			factory = new GamesXmlFactory ();
			factory.Read ("test_games.xml");
			definitions = factory.Definitions;
			definition = definitions [1]; // Age Game

			Assert.AreEqual ("Age", definition.Name);
			Assert.AreEqual (2, definition.Variants.Count);
			Assert.AreEqual ("father_son.svg", ((definition.DrawingObjects[0]) as ImageDrawingObject).Filename);

			// Variant: John is 46 years old.
			variant = definition.Variants [0];
			Assert.AreEqual (true, variant.Question.String.Contains ("John is 46 years old"));
			Assert.AreEqual ("[son]", variant.AnswerText);
			Assert.AreEqual (true, variant.Variables.Contains ("int father = 46;"));

			// Variant: John's age is nowadays 2 times his son's age.
			variant = definition.Variants [1];
			Assert.AreEqual (true, variant.Question.String.Contains ("John's age is nowadays 2 times his son's age."));
			Assert.AreEqual ("24", variant.AnswerText);
			Assert.AreEqual (true, variant.Variables.Contains ("int ago = years [idx];"));
		}
	}
}

