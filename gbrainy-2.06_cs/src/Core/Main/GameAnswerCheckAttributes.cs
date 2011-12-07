/*
 * Copyright (C) 2007-2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

namespace gbrainy.Core.Main
{
	[Flags]
	public enum GameAnswerCheckAttributes
	{
		None			= 0,
		Trim			= 2,
		IgnoreCase		= 4,
		IgnoreSpaces		= 8,
		MatchAll		= 16,
		MatchAllInOrder		= 32,
		MultiOption		= 64, // Allows calling GameAnswer.GetMultiOption
	}
	
	// Since we cannot override ToString in an enum type we use a helper class
	public static class GameAnswerCheckAttributesDescription
	{
		// string (not localized) to enum representation 
		static public GameAnswerCheckAttributes FromString (string type)
		{
			GameAnswerCheckAttributes attributes;

			attributes = GameAnswerCheckAttributes.None;

			if (type.IndexOf ("Trim", StringComparison.InvariantCultureIgnoreCase) != -1)
				attributes |= GameAnswerCheckAttributes.Trim;

			if (type.IndexOf ("IgnoreCase", StringComparison.InvariantCultureIgnoreCase) != -1)
				attributes |= GameAnswerCheckAttributes.IgnoreCase;

			if (type.IndexOf ("IgnoreSpaces", StringComparison.InvariantCultureIgnoreCase) != -1)
				attributes |= GameAnswerCheckAttributes.IgnoreSpaces;

			if (type.IndexOf ("MatchAll", StringComparison.InvariantCultureIgnoreCase) != -1)
				attributes |= GameAnswerCheckAttributes.MatchAll;

			if (type.IndexOf ("MatchAllInOrder", StringComparison.InvariantCultureIgnoreCase) != -1)
				attributes |= GameAnswerCheckAttributes.MatchAllInOrder;

			if (type.IndexOf ("MultiOption", StringComparison.InvariantCultureIgnoreCase) != -1)
				attributes |= GameAnswerCheckAttributes.MultiOption;

			return attributes;
		}
	}
}
