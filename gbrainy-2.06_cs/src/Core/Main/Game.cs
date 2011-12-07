/*
 * Copyright (C) 2007-2011 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.ComponentModel;
using System.Collections.Generic;

using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Core.Main
{
	abstract public class Game : IDrawable, IDrawRequest, IMouseEvent
	{
		private Cairo.Color default_color;
		protected Random random;
		private TimeSpan game_time;
		private bool tip_used;
		private bool preview;
		private GameDifficulty difficulty;
		private ISynchronizeInvoke synchronize;
		private List <Toolkit.Container> containers;
		private int variant;
		private GameAnswer answer;

		public event EventHandler DrawRequest;
		public event EventHandler <UpdateUIStateEventArgs> UpdateUIElement;
		public event EventHandler <GameAnswerEventArgs> AnswerEvent;

		protected Game ()
		{
			containers = new List <Toolkit.Container> ();
			difficulty = GameDifficulty.Medium;
			answer = new GameAnswer ();
		}

		public GameAnswer Answer {
			get {return answer; }
			set {answer = value; }
		}

#region Methods to override in your own games

		public abstract string Name {
			get;
		}

		// The question text shown to the user
		public abstract string Question {
			get;
		}

		// Text that explains why the right answer is valid
		public virtual string Rationale {
			get { return string.Empty; }
		}

		public virtual string Tip {
			get { return string.Empty;}
		}

		protected abstract void Initialize ();

		public virtual int Variants {
			get { return 1;}
		}
#endregion

#region Methods that you can optionally override

		// Default GameType
		public virtual GameTypes Type {
			get { return GameTypes.LogicPuzzle;}
		}

		// Indicates in which difficulty levels the game should be shown
		public virtual GameDifficulty Difficulty {
			get { return GameDifficulty.Master | GameDifficulty.Medium | GameDifficulty.Easy; }
		}

		// Indicates if the game should be excluded for color blind users
		public virtual bool UsesColors {
			get { return false;}
		}
#endregion

		public void Begin ()
		{
			random = new Random ();
			default_color = new Cairo.Color (0, 0, 0);
			Initialize ();

			if (String.IsNullOrEmpty (Answer.Correct))
				throw new InvalidOperationException ("Answer cannot be empty");
		}

		public virtual int Variant {
			protected get { return variant; }
			set {
				if (value < 0 || value > Variants)
					throw new ArgumentOutOfRangeException (String.Format ("Variant out of range {0}", value));

				variant = value;
			}
		}

		// Builds a text answer for the puzzle
		public string AnswerText {
			get {
				string str;

				str = String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The correct answer is {0}."),
				                     Answer.CorrectShow);

				if (String.IsNullOrEmpty (Rationale))
					return str;

				// Translators: answer + rationale of the answer
				return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("{0} {1}"), str, Rationale);
			}
		}

		public ISynchronizeInvoke SynchronizingObject {
			set { synchronize = value; }
			get { return synchronize; }
		}

		public bool IsPreviewMode {
			get {return preview; }
			set {preview = value; }
		}

		// The level of difficulty selected for the current game
		public GameDifficulty CurrentDifficulty {
			set { difficulty = value; }
			get { return difficulty; }
		}

		public string TipString {
			get {
				string tip = Tip;

				if (tip != string.Empty)
					tip_used = true;

				return tip;
			}
		}

		public virtual bool ButtonsActive {
			get { return true;}
		}

		// An initialized game cannot be playable (for example, missing external files)
		public virtual bool IsPlayable {
			get { return true;}
		}

		public virtual double DrawAreaX {
			get {return 0.1;}
		}

		public virtual double DrawAreaY {
			get {return 0.1;}
		}

		public virtual double DrawAreaWidth {
			get {return 1 - DrawAreaX * 2;}
		}

		public virtual double DrawAreaHeight {
			get {return 1 - DrawAreaY * 2;}
		}

		public virtual double LineWidth {
			get {return 0.005; }
		}

		public virtual Cairo.Color DefaultDrawingColor {
			get {return default_color; }
		}

		public TimeSpan GameTime {
			get {return game_time; }
			set {game_time = value; }
		}

		// Expected time in seconds that a player is expected to complete this game
		public int ExpectedTime {
			get { return Main.Score.GameExpectedTime (Type, CurrentDifficulty); }
		}

		public Widget [] Widgets {
			get { return containers.ToArray (); }
		}

		// Used by games to request a redraw of the view
		protected void OnDrawRequest ()
		{
			if (DrawRequest == null)
				return;

			DrawRequest (this, EventArgs.Empty);
		}

		// Used by games to request a question repaint
		protected void UpdateQuestion (string question)
		{
			if (UpdateUIElement == null)
				return;

			UpdateUIElement (this, new UpdateUIStateEventArgs (UpdateUIStateEventArgs.EventUIType.QuestionText,
				question));
		}

		// Score algorithm returns a value between 0 and 10
		public virtual int Score (string answer)
		{
			return Main.Score.GameScore (CheckAnswer (answer), GameTime.TotalSeconds, ExpectedTime, tip_used);
		}

		public void AddWidget (Toolkit.Container container)
		{
			if (containers.Contains (container))
				throw new InvalidOperationException ("Child already exists in container");

			foreach (Toolkit.Container previous in containers)
			{
				if (previous.X == container.X && previous.Y == container.Y &&
					 previous.Width == container.Width && previous.Height == container.Height)
				{
					throw new InvalidOperationException ("Child on the same area exists in container. Overlapping drawings.");
				}
			}

			container.DrawRequest += delegate (object sender, EventArgs e)
			{
				OnDrawRequest ();
			};

			// If the user has selected an item we should propagate an answer
			container.SelectedEvent += delegate (object sender, SeletectedEventArgs e)
			{
				if (AnswerEvent != null)
					AnswerEvent (this, new GameAnswerEventArgs ((string) e.DataEx));
			};

			containers.Add (container);
		}

		public virtual void Finish () {}

		protected void InitDraw (CairoContextEx gr, int width, int height, bool rtl)
		{
			gr.Scale (width, height);
			gr.Color = default_color;
			gr.LineWidth = LineWidth;
			// Not all Cairo surfaces have a default font size (like PDF)
			gr.SetPangoNormalFontSize ();
		}

		public virtual void Draw (CairoContextEx gr, int width, int height, bool rtl)
		{
			InitDraw (gr, width, height, rtl);

			foreach (Toolkit.Container container in containers)
				container.Draw (gr, width, height, rtl);
		}

		public virtual void DrawPreview (CairoContextEx gr, int width, int height, bool rtl)
		{
			Draw (gr, width, height, rtl);
		}

		public virtual bool CheckAnswer (string answer)
		{
			return Answer.CheckAnswer (answer);
		}

		public void EnableMouseEvents (bool enable)
		{
			foreach (Toolkit.Container container in containers)
				foreach (Widget widget in container.Children)
					widget.Sensitive = enable;
		}

		public void MouseEvent (object obj, MouseEventArgs args)
		{
			foreach (Toolkit.Container container in containers)
				container.MouseEvent (obj, args);
		}
	}
}
