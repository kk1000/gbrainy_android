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
	public enum GameDifficulty
	{
		None			= 0,
		Easy			= 2,
		Medium			= 4,
		Master			= 8,
		All			= Easy | Medium | Master,
	}

	public static class GameDifficultyDescription
	{
		// string (not localized) to enum representation
		static public GameDifficulty FromString (string type)
		{
			GameDifficulty difficulty;

			difficulty = GameDifficulty.None;

			if (type.IndexOf ("Easy", StringComparison.InvariantCultureIgnoreCase) != -1)
				difficulty |= GameDifficulty.Easy;

			if (type.IndexOf ("Medium", StringComparison.InvariantCultureIgnoreCase) != -1)
				difficulty |= GameDifficulty.Medium;

			if (type.IndexOf ("Master", StringComparison.InvariantCultureIgnoreCase) != -1)
				difficulty |= GameDifficulty.Master;

			if (type.IndexOf ("All", StringComparison.InvariantCultureIgnoreCase) != -1)
				difficulty |= GameDifficulty.All;

			return difficulty;
		}
	}
}
