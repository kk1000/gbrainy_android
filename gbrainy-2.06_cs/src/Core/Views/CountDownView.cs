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
using System.Timers;
using System.ComponentModel;

using gbrainy.Core.Main;
using gbrainy.Core.Services;

namespace gbrainy.Core.Views
{
	public class CountDownView : IDrawable, IDrawRequest
	{
		int countdown_time;
		System.Timers.Timer timer;
		EventHandler finish;
		ISynchronizeInvoke synchronize;

		public event EventHandler DrawRequest; // Not used in this view

		public CountDownView (EventHandler OnFinish)
		{
			timer = new System.Timers.Timer ();
			timer.Elapsed += TimerUpdater;
			timer.Interval = (1 * 1000); // 1 second
			finish = OnFinish;
		}

		public ISynchronizeInvoke SynchronizingObject {
			set { synchronize = value; }
			get { return synchronize; }
		}

		public void Start ()
		{
			timer.SynchronizingObject = SynchronizingObject;
			countdown_time = 3;
			timer.Enabled = true;
		}

		public void EndDrawCountDown ()
		{
			if (timer == null)
				return;

			timer.Enabled = false;
			timer.Dispose ();
			timer = null;
		}

		public void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			gr.Scale (area_width, area_height);

			gr.LineWidth = 0.01;
			gr.Color = new Cairo.Color (0, 0, 0, 1);

			gr.SetPangoLargeFontSize ();
			gr.DrawTextCentered (0.5, 0.1, ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Get ready to memorize the next objects..."));
			gr.Stroke ();

			gr.SetPangoFontSize (0.35);
			gr.DrawTextCentered (0.5, 0.5, countdown_time.ToString ());
			gr.Stroke ();

			gr.Arc (0.5, 0.5, 0.25, 0, 2 * Math.PI);
			gr.Stroke ();
			gr.Arc (0.5, 0.5, 0.28, 0, 2 * Math.PI);
			gr.Stroke ();
		}

		// It is executed in another thread
		void TimerUpdater (object source, ElapsedEventArgs e)
		{
			lock (this) {

				if (countdown_time <= 1) {
					finish (this, EventArgs.Empty);
				} else
					countdown_time--;

				if (DrawRequest != null)
					DrawRequest (this, EventArgs.Empty);
			}
		}
	}
}
