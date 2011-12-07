/*
 * Copyright (C) 2007-2009 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using Cairo;
using System.Collections.Generic;

using gbrainy.Core.Main;
using gbrainy.Core.Services;

namespace gbrainy.Core.Views
{
	public class FinishView : IDrawable
	{
		GameSession session;
		const int tips_shown = 4;
		const double smaller_font = 0.018;

		// Caching mechanism to use always the same tips during different redraws of the same view
		int cached_sessionid;
		List <string> tips;

		public FinishView (GameSession session)
		{
			this.session = session;
			tips = new List <string> ();
			cached_sessionid = -1;
		}

		static void DrawBand (CairoContextEx gr, double x, double y)
		{
			gr.Save ();
			gr.Rectangle (x, y, 1 - 0.06, 0.06);
			gr.Color = new Cairo.Color (0, 0, 0.2, 0.2);
			gr.Fill ();
			gr.Restore ();		
		}

		static void DrawBar (CairoContextEx gr, double x, double y, double w, double h, double percentage)
		{
			double per = percentage / 100;
	
			gr.Rectangle (x, y - h * per, w, h * per);
			gr.FillGradient (x, y - h * per, w, h * per, new Cairo.Color (0, 0, 1));
			gr.DrawTextCentered (x + w / 2, (y - 0.03) - h * per, String.Format ("{0}", percentage));

			gr.Save ();
			gr.Color = new Cairo.Color (0, 0, 0);	
			gr.MoveTo (x, y);
			gr.LineTo (x, y - h * per);
			gr.LineTo (x + w, y - h * per);
			gr.LineTo (x + w, y);
			gr.LineTo (x, y);
			gr.Stroke ();
			gr.Restore ();
		}

		void DrawColumnBarGraphic (CairoContextEx gr, double x, double y)
		{
			const double area_w = 0.85, area_h = 0.28;
			const double bar_w = 0.05, bar_h = area_h - 0.02;
			const double space_x = 0.08;
		
			gr.LineWidth = 0.005;

			// Draw X reference values
			gr.SetPangoFontSize (smaller_font);
			gr.DrawTextAlignedRight (x + 0.05, y, "100");
			gr.DrawTextAlignedRight (x + 0.05, y + area_h - 0.02, "0");

			x += 0.06;

			// Axis
			gr.MoveTo (x, y);
			gr.LineTo (x, y + area_h);
			gr.LineTo (x + area_w, y + area_h);
			gr.Stroke ();

			x = x + space_x;
			DrawBar (gr, x, y + area_h, bar_w, bar_h, session.History.TotalScore);
			gr.DrawTextCentered (x + bar_w / 2, y + area_h + 0.03, ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Total"));

			x = x + space_x * 2;

			if (session.History.LogicPlayed > 0)
				DrawBar (gr, x, y + area_h, bar_w, bar_h, session.History.LogicScore);

			gr.DrawTextCentered (x + bar_w / 2, y + area_h + 0.03, 	ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Logic")); 

			x = x + space_x * 2;

			if (session.History.MathPlayed > 0)
				DrawBar (gr, x, y + area_h, bar_w, bar_h, session.History.MathScore);

			gr.DrawTextCentered (x + bar_w / 2, y + area_h + 0.03, ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Calculation"));

			x = x + space_x * 2;

			if (session.History.MemoryPlayed > 0)
				DrawBar (gr, x, y + area_h, bar_w, bar_h, session.History.MemoryScore);

			gr.DrawTextCentered (x + bar_w / 2, y + area_h + 0.03, ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Memory"));

			x = x + space_x * 2;

			if (session.History.VerbalPlayed > 0)
				DrawBar (gr, x, y + area_h, bar_w, bar_h, session.History.VerbalScore);

			gr.DrawTextCentered (x + bar_w / 2, y + area_h + 0.03, ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Verbal"));
		}

		public void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double y = 0.04, x = 0.05;
			const double space_small = 0.02;
			List <PlayerPersonalRecord> records;
			string s, tip;
			double width, height;

			gr.Scale (area_width, area_height);
			gr.Color = new Cairo.Color (0, 0, 0, 1);

			gr.MoveTo (x, y);
			gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Score"), false, -1, 0);
			DrawBand (gr, 0.03, y - 0.01);

			y += 0.08;
			gr.MoveTo (x, y);
	
			s = session.Result;
			if (s == string.Empty)
				gr.ShowPangoText (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Games won: {0} ({1} played)"), session.History.GamesWon, session.History.GamesPlayed));
			else
				gr.ShowPangoText (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("{0}. Games won: {1} ({2} played)"), s, session.History.GamesWon, session.History.GamesPlayed));

			y += 0.06;
			gr.MoveTo (x, y);
			gr.ShowPangoText (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Time played {0} (average per game {1})"), session.GameTime, session.TimePerGame));
		
			y += 0.09;
			DrawColumnBarGraphic (gr, x, y);

			y += 0.36;
			gr.MoveTo (x, y);
			gr.SetPangoFontSize (smaller_font);
			// Translators: translated string should not be longer that the English original (space restriction on the UI)
			gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("For details on how gbrainy's scoring works refer to the help."));

			y += 0.07;
			gr.SetPangoNormalFontSize ();
			records	= session.PlayerHistory.GetLastGameRecords ();
			gr.MoveTo (x, y);

			if (records.Count == 0) {
				bool caching = cached_sessionid != session.ID;
	
				gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Tips for your next games"), false, -1, 0);
				DrawBand (gr, 0.03, y - 0.01);

				y += 0.08;

				if (caching)
					tips.Clear ();

				for (int i = 0; i < tips_shown; i++)
				{
					if (caching)
						tips.Add (GameTips.Tip);
	
					tip = "- " + tips [i];

					gr.MeasureString (tip, 1.0 - x, true, out width, out height);

					if (y + height > 0.98)
						break;

					gr.DrawStringWithWrapping (x, y, tip , 1.0 - x);
					y += height + space_small;
				}

				if (caching)
					cached_sessionid = session.ID;
			} 
			else  {
				gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Congratulations! New personal record"), false, -1, 0);
				DrawBand (gr, 0.03, y - 0.01);

				y += 0.08;

				for (int i = 0; i < records.Count; i++)
				{
					switch (records[i].GameType) {
					case GameTypes.LogicPuzzle:
						s = String.Format (ServiceLocator.Instance.GetService <ITranslations> ().
							GetString ("By scoring {0} in logic puzzle games you have established a new personal record. Your previous record was {1}."),
							records[i].NewScore,
							records[i].PreviousScore);
						break;
					case GameTypes.Calculation:
						s = String.Format (ServiceLocator.Instance.GetService <ITranslations> ().
							GetString ("By scoring {0} in calculation games you have established a new personal record. Your previous record was {1}."),
							records[i].NewScore,
							records[i].PreviousScore);
						break;
					case GameTypes.Memory:
						s = String.Format (ServiceLocator.Instance.GetService <ITranslations> ().
							GetString ("By scoring {0} in memory games you have established a new personal record. Your previous record was {1}."),
							records[i].NewScore,
							records[i].PreviousScore);
						break;
					case GameTypes.VerbalAnalogy:
						s = String.Format (ServiceLocator.Instance.GetService <ITranslations> ().
							GetString ("By scoring {0} in verbal analogies you have established a new personal record. Your previous record was {1}."),
							records[i].NewScore,
							records[i].PreviousScore);
						break;
					default:
						break;
					}

					tip = "- " + s;

					gr.MeasureString (tip, 1.0 - x, true, out width, out height);

					if (y + height > 0.98)
						break;

					gr.DrawStringWithWrapping (x, y, tip , 1.0 - x);
					y += height + space_small;
				}
			}

			gr.Stroke ();
		}
	}
}
