/*
 * Copyright (C) 2007 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.Timers;
using gbrainy.Core.Services;

using gbrainy.Core.Views;

namespace gbrainy.Core.Main
{
	public abstract class Memory : Game
	{
		protected int time_left;
		protected int total_time;
		protected System.Timers.Timer timer;
		private bool request_answer = false;
		private bool buttons_active;
		protected bool shade = false;
		const int shading_time = 15;
		private LinearGradient gradient = null;
		protected double alpha;
		private bool draw_timer;
		CountDownView downview;

		public override bool ButtonsActive {
			get { return buttons_active;}
		}

		public abstract string MemoryQuestion {
			get;
		}

		public override string Question {
			get {
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Memorize the objects below in the given time");
			}
		}

		public override GameTypes Type {
			get { return GameTypes.Memory;}
		}

		public int TotalTime {
			get { return total_time;}
			set { 
				total_time = value;
				time_left = value;
			}
		}

		protected override void Initialize ()
		{
			if (Preferences.Get <bool> (Preferences.MemQuestionWarnKey) == false || 
				IsPreviewMode == true) {
				InitializeGame ();
				return;
			}

			downview = new CountDownView (OnCountDownFinish);
			downview.SynchronizingObject = SynchronizingObject;
			downview.DrawRequest += OnCountDownRedraw;
			downview.Start ();
		}

		public void OnCountDownRedraw (object o, EventArgs args)
		{
			OnDrawRequest ();
		}

		void FinishCountDown ()
		{
			downview.EndDrawCountDown ();
			downview.DrawRequest -= OnCountDownRedraw;
			downview = null;
		}

		void OnCountDownFinish (object source, EventArgs e)
		{
			FinishCountDown ();
			InitializeGame ();
		}

		void InitializeGame ()
		{
			timer = new System.Timers.Timer ();
			timer.SynchronizingObject = SynchronizingObject;
			timer.Elapsed += TimerUpdater;
			timer.Interval = (1 * 100); // 0.1 seconds
			buttons_active = false;
			timer.Enabled = false;
			alpha = 1;
			draw_timer = false;

			time_left = total_time = Preferences.Get <int> (Preferences.MemQuestionTimeKey) * 10; // Seconds

			StartTimer ();
		}

		public void StartTimer ()
		{
			timer.Enabled = true;
			draw_timer = true;
		}

		private void TimerUpdater (object source, ElapsedEventArgs e)
		{
			// In slow systems you can get pending events after the timer is disabled
			if (timer.Enabled == false)
				return;

			if (shade == false && time_left == 0) {
				lock (this) {
					time_left = shading_time;
					shade = true;
					draw_timer = false;
				}
				return;	
			}	

			if (time_left == 0) {
				lock (this) {
					shade = false;
					timer.Enabled = false;
					request_answer = true;
					buttons_active = true;
				}
				UpdateQuestion (MemoryQuestion);
			} else {
				lock (this) {
					time_left--;
				}
			}
	
			OnDrawRequest ();
		}

		public override void Finish ()
		{
			if (downview != null)
				FinishCountDown ();
			else 
				timer.Enabled = false;
		}		

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{			
			if (downview != null) {
				downview.Draw (gr, area_width, area_height, rtl);
				return;
			}
			
			InitDraw (gr, area_width, area_height, rtl);

			if (shade) {
				if (alpha > 0)
					alpha -= (1 / (double) shading_time);

				gr.Color = new Color (DefaultDrawingColor.R, DefaultDrawingColor.G, DefaultDrawingColor.B, alpha);
				DrawObjectToMemorize (gr, area_width, area_height, rtl);
				return;
			}
		
			alpha = 1;
			gr.Color = new Color (DefaultDrawingColor.R, DefaultDrawingColor.G, DefaultDrawingColor.B, alpha);
			if (request_answer && Answer.Draw == false) {
				DrawPossibleAnswers (gr, area_width, area_height, rtl);
			} else {
				DrawObjectToMemorize (gr, area_width, area_height, rtl);
			}		
		}

		public override void DrawPreview (CairoContextEx gr, int width, int height, bool rtl)
		{
			gr.Scale (width, height);
			gr.Color = new Cairo.Color (0, 0, 0);
			gr.LineWidth = LineWidth;
			DrawObjectToMemorize (gr, width, height, rtl);
		}

		public virtual void DrawPossibleAnswers (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			foreach (Toolkit.Container container in Widgets)
				container.Draw (gr, area_width, area_height, rtl);
		}

		public virtual void DrawObjectToMemorize (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double percentage;

			if (draw_timer == false || IsPreviewMode == true)
				return;

			percentage = 100 - ((time_left * 100) / total_time);
			DrawTimeBar (gr, 0.1, 0.2, percentage);
		}

		public void DrawTimeBar (CairoContextEx gr, double x, double y, double percentage)
		{
			double width = 0.04, height = 0.6;
			const double w = 0.003, h = 0.003;

			gr.DrawTextCentered (x + (width / 2), y + height + 0.05, ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Time left"));
			gr.Stroke ();

			gr.Save ();
			gr.Color = new Color (0, 0, 0);	
			gr.MoveTo (x, y);
			gr.LineTo (x, y + height);
			gr.LineTo (x + width, y + height);
			gr.LineTo (x + width, y);
			gr.LineTo (x, y);
			gr.Stroke ();

			x+= w;
			y+= h;
			width -= w * 2;
			height -= h * 2;
			y += height * (100 - percentage) / 100;
			height *= percentage / 100;

			if (gradient == null) {
				gradient = new LinearGradient (x, y, x + width, y + height);
				gradient.AddColorStop (0, new Color (1, 0, 0, 1));
				gradient.AddColorStop (1, new Color (0.2, 0, 0, 1));
			}

			gr.Source = gradient;			
			gr.MoveTo (x, y);
			gr.LineTo (x, y + height);
			gr.LineTo (x + width, y + height);
			gr.LineTo (x + width, y);
			gr.LineTo (x, y);
			gr.FillPreserve ();
			gr.Stroke ();
			gr.Restore ();
		}
	}
}
