/*
 * Copyright (C) 2009 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

using gbrainy.Core.Main;
using gbrainy.Core.Services;

namespace gbrainy.Games.Logic
{
	public class PuzzleTimeNow : Game
	{
		const double figure_size = 0.3;
		int after;
		DateTime position_a, position_b, ans, sample;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Time now");}
		}

		public override string Question {
			get {return (String.Format (
				// TimeNow Puzzle. Translators: {1}, {2} {3} are replaced by hours. Use the right time format specification for your culture
 				// Explanation of the date and time format specifications can be found here:
				// http://msdn.microsoft.com/en-us/library/system.globalization.datetimeformatinfo.aspx
				// For 12-hour clock format use {0:%h} and for 24-hour clock format use {0:%H}. The date formats {0:h} and {0:H} are invalid.
				//
				ServiceLocator.Instance.GetService <ITranslations> ().GetPluralString (
					"{0} hour ago it was as long after {1:h tt} as it was before {2:h tt} on the same day. What is the time now? Answer using the hour (e.g.: {3:h tt})",
					"{0} hours ago it was as long after {1:h tt} as it was before {2:h tt} on the same day. What is the time now? Answer using the hour (e.g.: {3:h tt})",
					after),
				after, position_a, position_b, sample));}
		}

		public override string Rationale {
			get {
				return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetPluralString (
					"Determine the hour half way between the given times, and then add {0} hour to convert it to the present time.",
					"Determine the hour half way between the given times, and then add {0} hours to convert it to the present time.",
					after),
					after);
			}
		}

		protected override void Initialize ()
		{
			int hour;
			DateTime now;

			after = 4 + random.Next (3);
			hour = 2 + random.Next (3);
			now = DateTime.Now;

			position_a = new DateTime (now.Year, now.Month, now.Day, hour, 0, 0);
			position_b = new DateTime (now.Year, now.Month, now.Day, hour + 12, 0, 0);
			ans = new DateTime (now.Year, now.Month, now.Day, ((hour + hour + 12) / 2) + after, 0, 0);

			if (position_b != ans)
				sample = position_b;
			else {
				sample = position_a;
			}

			// TimeNow Puzzle. Translators: {0} is used to check the hour answered by the user.
			// Use the right time format specification for your culture
 			// Explanation of the date and time format specifications can be found here:
			// http://msdn.microsoft.com/en-us/library/system.globalization.datetimeformatinfo.aspx
			// For 12-hour clock format use {0:%h} and for 24-hour clock format use {0:%H}. The date formats {0:h} and {0:H} are invalid.
			Answer.Correct = String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("{0:h tt}"), ans);
			Answer.CheckAttributes = GameAnswerCheckAttributes.IgnoreCase | GameAnswerCheckAttributes.IgnoreSpaces;
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.Draw (gr, area_width, area_height, rtl);
			gr.DrawClock (DrawAreaX + 0.4, DrawAreaY + 0.4, figure_size,
				0, 0 /* No hands */);

			gr.DrawTextCentered (0.5, DrawAreaY + 0.3 + figure_size, ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Sample clock"));
		}
	}
}
