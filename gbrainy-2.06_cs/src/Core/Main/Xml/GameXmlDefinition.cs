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
using System.Collections.Generic;

namespace gbrainy.Core.Main.Xml
{
	// Container for a game defined in an Xml file
	public class GameXmlDefinition : GameXmlDefinitionVariant
	{
		public string Name { get; set; }
		public GameDifficulty Difficulty { get; set; }
		public GameTypes Type { get; set; }

		public List <GameXmlDefinitionVariant> Variants { get; set; }

		public GameXmlDefinition ()
		{
			Difficulty = GameDifficulty.Medium;
			Type = GameTypes.LogicPuzzle; // TODO: temporary, should be mandatory in games.xml
			Variants = new List <GameXmlDefinitionVariant> ();
		}

		public void NewVariant ()
		{
			Variants.Add (new GameXmlDefinitionVariant ());
		}

		public override string ToString ()
		{
			StringBuilder str = new StringBuilder ();

			str.AppendLine ("Name: " + Name);
			str.AppendLine ("Difficulty: " + Difficulty);
			str.AppendLine (base.ToString ());

			foreach (GameXmlDefinitionVariant variant in Variants)
			{
				str.AppendLine ("---");
				str.AppendLine (variant.ToString ());
				str.AppendLine ("---");
			}

			return str.ToString ();
		}
	}
}
