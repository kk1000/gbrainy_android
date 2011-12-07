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
using Gtk;
using Mono.Unix;
using System.Diagnostics;
using Gdk;
using System.Reflection;

using gbrainy.Core.Main;
using gbrainy.Core.Platform;
using gbrainy.Core.Services;
using gbrainy.Clients.Classical.Dialogs;
using gbrainy.Clients.Classical.Widgets;

#if MONO_ADDINS
using Mono.Addins;
using Mono.Addins.Setup;
#endif

namespace gbrainy.Clients.Classical
{
	public class GtkClient
	{
		[GtkBeans.Builder.Object("gbrainy")] Gtk.Window app_window;
		[GtkBeans.Builder.Object] Gtk.CheckMenuItem showtoolbar_menuitem;
		[GtkBeans.Builder.Object] Box drawing_vbox;
		[GtkBeans.Builder.Object] Gtk.HBox main_hbox;
		[GtkBeans.Builder.Object] Gtk.VBox framework_vbox;
		[GtkBeans.Builder.Object] Gtk.Entry answer_entry;
		[GtkBeans.Builder.Object] Gtk.Button answer_button;
		[GtkBeans.Builder.Object] Gtk.Label answer_label;
		[GtkBeans.Builder.Object] Gtk.Button tip_button;
		[GtkBeans.Builder.Object] Gtk.Button next_button;
		[GtkBeans.Builder.Object] Gtk.Statusbar statusbar;
		[GtkBeans.Builder.Object] Gtk.MenuBar menubar;
		[GtkBeans.Builder.Object] Gtk.MenuItem pause_menuitem;
		[GtkBeans.Builder.Object] Gtk.MenuItem finish_menuitem;
		[GtkBeans.Builder.Object] Gtk.MenuItem newgame_menuitem;
		[GtkBeans.Builder.Object] Gtk.MenuItem allgames_menuitem;
		[GtkBeans.Builder.Object] Gtk.MenuItem logic_menuitem;
		[GtkBeans.Builder.Object] Gtk.MenuItem calculation_menuitem;
		[GtkBeans.Builder.Object] Gtk.MenuItem memory_menuitem;
		[GtkBeans.Builder.Object] Gtk.MenuItem verbal_menuitem;
		[GtkBeans.Builder.Object] Gtk.MenuItem extensions_menuitem;
		[GtkBeans.Builder.Object] Gtk.RadioMenuItem vertical_radiomenuitem;
		[GtkBeans.Builder.Object] Gtk.RadioMenuItem horizontal_radiomenuitem;
		[GtkBeans.Builder.Object] Gtk.MenuItem toolbar_orientation_menuitem;

		Widgets.Toolbar toolbar;

		GameDrawingArea drawing_area;
		GameSession session;
		bool low_res;
		bool full_screen;
		GameSession.Types initial_session;

		public readonly int MIN_TRANSLATION = 80;

		public GtkClient ()
		{
			if (Preferences.Get <bool> (Preferences.EnglishKey) == false)
			{
				Catalog.Init ("gbrainy", Defines.GNOME_LOCALE_DIR);
			}

			Unix.FixLocaleInfo ();
		}

		public GameSession Session {
			get { return session; }
		}

		public GameSession.Types InitialSessionType {
			get { return initial_session; }
			set { initial_session = value; }
		}

		public void Initialize ()
		{
			session = new GameSession ();
			GameManagerPreload (session.GameManager);
			Console.WriteLine (session.GameManager.GetGamesSummary ());

			session.GameManager.ColorBlind = Preferences.Get <bool> (Preferences.ColorBlindKey);
			session.DrawRequest += SessionDrawRequest;
			session.UpdateUIElement += SessionUpdateUIElement;
			session.SynchronizingObject = new GtkSynchronize ();
			session.Difficulty = (GameDifficulty) Preferences.Get <int> (Preferences.DifficultyKey);

			BuildUI ();
		}

		public static void GameManagerPreload (GameManager gm)
		{
			gm.LoadAssemblyGames (Defines.GAME_ASSEMBLY);
			gm.LoadVerbalAnalogies (System.IO.Path.Combine (Defines.DATA_DIR, Defines.VERBAL_ANALOGIES));
			gm.LoadGamesFromXml (System.IO.Path.Combine (Defines.DATA_DIR, Defines.GAMES_FILE));
			gm.LoadPlugins ();
		}

		void BuildUI ()
		{
			bool show_toolbar;

			GtkBeans.Builder builder = new GtkBeans.Builder ("gbrainy.ui");
			builder.Autoconnect (this);

			show_toolbar = Preferences.Get <bool> (Preferences.ToolbarShowKey) == true && low_res == false;

			// Toolbar creation
			toolbar = new Widgets.Toolbar (main_hbox, framework_vbox);
			toolbar.Attach ((Gtk.Orientation) Preferences.Get <int> (Preferences.ToolbarOrientationKey));
			toolbar.AllButton.Clicked += OnAllGames;
			toolbar.LogicButton.Clicked += OnLogicOnly;
			toolbar.CalculationButton.Clicked += OnMathOnly;
			toolbar.MemoryButton.Clicked += OnMemoryOnly;
			toolbar.VerbalButton.Clicked += OnVerbalOnly;
			toolbar.PauseButton.Clicked += OnPauseGame;
			toolbar.FinishButton.Clicked += OnEndGame;

			drawing_area = new GameDrawingArea ();
			drawing_area.Drawable = session;
			GameSensitiveUI ();

			// For low resolutions, hide the toolbar and made the drawing area smaller
			if (drawing_area.Screen.Width> 0 && drawing_area.Screen.Height > 0) {
				if (drawing_area.Screen.Height < 700) {
					drawing_vbox.HeightRequest = 350;
					low_res = true;
				}
			}

			EventBox eb = new EventBox (); // Provides a window for drawing area windowless widget

			eb.Events = Gdk.EventMask.PointerMotionMask;
			drawing_vbox.Add (eb);

			eb.Add (drawing_area);

			eb.MotionNotifyEvent += OnMouseMotionEvent;
			eb.ButtonPressEvent += OnHandleButtonPress;

			show_toolbar = Preferences.Get <bool> (Preferences.ToolbarShowKey) == true && low_res == false;

			// We only disable the Arrow if we are going to show the toolbar.
			// It has an impact on the total window width size even if we do not show it
			if (show_toolbar)
				toolbar.ShowArrow = false;

			app_window.IconName = "gbrainy";

			app_window.ShowAll ();

			toolbar_orientation_menuitem.Sensitive = toolbar.Visible;

			// Check default radio button
			switch (toolbar.Orientation) {
			case Gtk.Orientation.Vertical:
				vertical_radiomenuitem.Active = true;
				break;
			case Gtk.Orientation.Horizontal:
				horizontal_radiomenuitem.Active = true;
				break;
			default:
				throw new InvalidOperationException ();
			}

			// The toolbar by default is enabled. By setting this menu entry to false
			// triggers the OnActivateToolbar event that hides the toolbar
			if (show_toolbar == false)
				showtoolbar_menuitem.Active = false;

		#if MONO_ADDINS
			extensions_menuitem.Activated += delegate (object sender, EventArgs ar) 
			{ 
				Mono.Addins.Gui.AddinManagerWindow.Run (app_window);
				GameManagerPreload (session.GameManager);
				CustomGameDialog.Clear ();
			};
		#else
			extensions_menuitem.Visible = false;
		#endif
			ActiveInputControls (false);
		}

		public void ProcessDefaults ()
		{
			if (InitialSessionType != GameSession.Types.None)
				OnNewGame (InitialSessionType);
		}

		// Gamesession has requested a question refresh
		public void SessionUpdateUIElement (object o, UpdateUIStateEventArgs args)
		{
			switch (args.EventType) {
			case UpdateUIStateEventArgs.EventUIType.QuestionText:
				UpdateQuestion ((string) args.Data);
				ActiveInputControls (true);
				break;
			case UpdateUIStateEventArgs.EventUIType.Time:
				UpdateStatusBar ();
				break;
			default:
				throw new InvalidOperationException ("Unknown value");
			}
		}

		// Gamesession has requested a redraw of the drawingarea
		public void SessionDrawRequest (object o, EventArgs args)
		{
			drawing_area.QueueDraw ();
		}

		void OnMouseMotionEvent (object o, MotionNotifyEventArgs ev_args)
		{
			SendMouseEvent (ev_args.Event.X, ev_args.Event.Y, MouseEventType.Move);
		}

		void OnHandleButtonPress (object o, ButtonPressEventArgs ev_args)
		{
			if (ev_args.Event.Type != EventType.ButtonPress)
				return;

			SendMouseEvent (ev_args.Event.X, ev_args.Event.Y, MouseEventType.ButtonPress);
		}

		void SendMouseEvent (double ev_x, double ev_y, MouseEventType type)
		{
			double x, y;
			int drawing_square = drawing_area.DrawingSquare;

			x = ev_x - drawing_area.OffsetX;
			y = ev_y - drawing_area.OffsetY;

			if (x < 0 || y < 0 || x > drawing_square || y > drawing_square)
				return;

			x =  x / drawing_square;
			y =  y / drawing_square;

			session.MouseEvent (this, new MouseEventArgs (x, y, type));
		}

		public void UpdateStatusBar ()
		{
			statusbar.Push (0, session.StatusText);
		}

		// These are UI elements dependent of the game status
		public void ActiveInputControls (bool active)
		{
			bool answer, next, tip, can_pause;

			can_pause = answer = next = tip = active;

			if (active == true && session.CurrentGame != null && session.CurrentGame.ButtonsActive == true && String.IsNullOrEmpty (session.CurrentGame.Tip ) == false)
				tip = true;
			else
				tip = false;

			switch (session.Status) {
			case GameSession.SessionStatus.NotPlaying:
			case GameSession.SessionStatus.Finished:
				answer = false;
				next = false;
				tip = false;
				can_pause = false;
				break;
			case GameSession.SessionStatus.Playing:
				if (session.CurrentGame != null) {
					can_pause = session.CurrentGame.ButtonsActive;
				}
				else {
					can_pause = true;
				}
				break;
			case GameSession.SessionStatus.Answered:
				answer = false;
				tip = false;
				can_pause = false;
				break;
			}

			answer_button.Sensitive = answer;
			answer_entry.Sensitive = answer;
			answer_label.Sensitive = answer;
			next_button.Sensitive = next;
			tip_button.Sensitive = tip;
			pause_menuitem.Sensitive = toolbar.PauseButton.Sensitive = can_pause;

			if (answer == true)
				answer_entry.GrabFocus ();
		}

		public void UpdateQuestion (string question)
		{
			drawing_area.Question = question;
		}

		public void QueueDraw ()
		{
			drawing_area.QueueDraw ();
		}

		void UpdateSolution (string solution, GameDrawingArea.SolutionType solution_type)
		{
			drawing_area.Solution = solution;
			drawing_area.SolutionIcon = solution_type;
			QueueDraw ();
		}

		// These are UI elements independent of the game status, set only when the game starts / ends
		void GameSensitiveUI ()
		{
			//Toolbar buttons and menu items that are sensitive when the user is playing
			bool playing;
			GameTypes available;

			playing = (session.Status == GameSession.SessionStatus.Playing);
			toolbar.FinishButton.Sensitive = playing;

			available = session.AvailableGames;

			if (playing == false && ((available & GameTypes.LogicPuzzle) == GameTypes.LogicPuzzle))
				logic_menuitem.Sensitive = toolbar.LogicButton.Sensitive = true;
			else
				logic_menuitem.Sensitive = toolbar.LogicButton.Sensitive = false;

			if (playing == false && ((available & GameTypes.Calculation) == GameTypes.Calculation))
				memory_menuitem.Sensitive = toolbar.MemoryButton.Sensitive = true;
			else
				memory_menuitem.Sensitive = toolbar.MemoryButton.Sensitive = false;

			if (playing == false && ((available & GameTypes.Calculation) == GameTypes.Calculation))
				calculation_menuitem.Sensitive = toolbar.CalculationButton.Sensitive = true;
			else
				calculation_menuitem.Sensitive = toolbar.CalculationButton.Sensitive = false;

			if (playing == false && ((available & GameTypes.VerbalAnalogy) == GameTypes.VerbalAnalogy))
				verbal_menuitem.Sensitive = toolbar.VerbalButton.Sensitive = true;
			else
				verbal_menuitem.Sensitive = toolbar.VerbalButton.Sensitive = false;

			if (playing == false && (available != GameTypes.None))
				allgames_menuitem.Sensitive = toolbar.AllButton.Sensitive = true;
			else
				allgames_menuitem.Sensitive = toolbar.AllButton.Sensitive = false;

			finish_menuitem.Sensitive = playing;
			newgame_menuitem.Sensitive = !playing;
		}

		private void GetNextGame ()
		{
			UpdateSolution (String.Empty, GameDrawingArea.SolutionType.None);
			UpdateQuestion (String.Empty);

			if (session.CurrentGame != null) {
				session.CurrentGame.AnswerEvent -= OnAnswerFromGame;
			}

			session.NextGame ();
			session.CurrentGame.AnswerEvent += OnAnswerFromGame;

			ActiveInputControls (session.CurrentGame.ButtonsActive);
			next_button.Sensitive = true;
			UpdateQuestion (session.CurrentGame.Question);
			answer_entry.Text = string.Empty;
			UpdateStatusBar ();
			session.CurrentGame.Answer.Draw = false;
			drawing_area.QueueDraw ();
		}

		// The user has clicked with the mouse in an answer and generated this event
		void OnAnswerFromGame (object obj, GameAnswerEventArgs args)
		{
			answer_entry.Text = args.AnswerText;
			OnAnswerButtonClicked (this, EventArgs.Empty);
			session.CurrentGame.AnswerEvent -= OnAnswerFromGame; // User can only answer once
		}

		void OnMenuAbout (object sender, EventArgs args)
		{
			Dialogs.AboutDialog about = new Dialogs.AboutDialog ();
			about.Run ();
		}

		void OnMenuHelp (object sender, EventArgs args)
		{
			Unix.ShowUri (null, "ghelp:gbrainy",
				Gdk.EventHelper.GetTime (new Gdk.Event(IntPtr.Zero)));
		}

		void OnAnswerButtonClicked (object sender, EventArgs args)
		{
			string answer;
			bool correct;

			if (session.CurrentGame == null)
				return;

			correct = session.ScoreGame (answer_entry.Text);
			if (correct)
				answer = Catalog.GetString ("Congratulations.");
			else
				answer = Catalog.GetString ("Incorrect answer.");

			session.EnableTimer = false;
			answer_entry.Text = String.Empty;
			UpdateStatusBar ();
			UpdateSolution (answer + " " + session.CurrentGame.AnswerText,
				correct == true ? GameDrawingArea.SolutionType.CorrectAnswer :
			        GameDrawingArea.SolutionType.InvalidAnswer);

			session.CurrentGame.Answer.Draw = true;
			ActiveInputControls (true);
			next_button.GrabFocus ();
			drawing_area.QueueDraw ();
		}

		void OnQuit (object sender, EventArgs args)
		{
			Gtk.Application.Quit ();
		}

		void OnDeleteWindow (object sender, DeleteEventArgs args)
		{
			Gtk.Application.Quit ();
		}

		void OnNextButtonClicked (object sender, EventArgs args)
		{
			if (answer_entry.Text.Length > 0) {
				OnAnswerButtonClicked (sender, args);
				return;
			}

			session.ScoreGame (String.Empty);
			GetNextGame ();
			session.EnableTimer = true;
		}

		void OnTip (object sender, EventArgs args)
		{
			if (session.CurrentGame == null)
				return;

			UpdateSolution (session.CurrentGame.TipString, GameDrawingArea.SolutionType.Tip);
		}

		void OnNewGame (GameSession.Types type)
		{
			// If the translation is lower than MIN_TRANSLATION explain that running the English version is an option
			if (ShowTranslationWarning ())
				Translations ();

			session.Type = type;
			session.New ();
			GetNextGame ();
			GameSensitiveUI ();
			UpdateSolution (Catalog.GetString ("Once you have an answer type it in the \"Answer:\" entry box and press the \"OK\" button."),
				GameDrawingArea.SolutionType.Tip);
			UpdateStatusBar ();
		}

		public bool ShowTranslationWarning ()
		{
			// Notify the user once per version only
			if (String.Compare (Preferences.Get <string> (Preferences.EnglishVersionKey), Defines.VERSION, 0) == 0)
				return false;

			int percentage = ServiceLocator.Instance.GetService <ITranslations> ().TranslationPercentage;
			if (percentage > 0 && percentage < MIN_TRANSLATION)
			{
				Preferences.Set <string> (Preferences.EnglishVersionKey, Defines.VERSION);
				Preferences.Save ();
				return true;
			}

			return false;
		}

		void Translations ()
		{		
			HigMessageDialog dlg;
	
			dlg = new HigMessageDialog (app_window,
				Gtk.DialogFlags.DestroyWithParent,
				Gtk.MessageType.Warning,
				Gtk.ButtonsType.Ok,
				Catalog.GetString ("The level of translation of gbrainy for your language is low."),
				Catalog.GetString ("You may be exposed to partially translated games making it more difficult to play. If you prefer to play in English, there is an option for doing so in gbrainy's Preferences."));
		
			try {
	 			dlg.Run ();
	 		} finally {
	 			dlg.Destroy ();
	 		}
		}

		void OnMathOnly (object sender, EventArgs args)
		{
			OnNewGame (GameSession.Types.Calculation);
		}

		void OnVerbalOnly (object sender, EventArgs args)
		{
			OnNewGame (GameSession.Types.VerbalAnalogies);
		}

		void OnMemoryOnly (object sender, EventArgs args)
		{
			OnNewGame (GameSession.Types.Memory);
		}

		void OnPdfExport (object sender, EventArgs args)
		{
			PdfExportDialog pdf;

			pdf = new PdfExportDialog ();
			pdf.Run ();
			pdf.Destroy ();
		}

		void OnPreferences (object sender, EventArgs args)
		{
			PreferencesDialog dialog;

			dialog = new PreferencesDialog (session.PlayerHistory);
			if ((Gtk.ResponseType) dialog.Run () == ResponseType.Ok) {
				session.Difficulty = (GameDifficulty) Preferences.Get <int> (Preferences.DifficultyKey);
				session.GameManager.ColorBlind = Preferences.Get <bool> (Preferences.ColorBlindKey);

				if (dialog.NewThemeSet == true)
					drawing_area.ReloadBackground ();
			}
			dialog.Destroy ();
		}

		void OnCustomGame (object sender, EventArgs args)
		{
			CustomGameDialog dialog;

			dialog = new CustomGameDialog (session.GameManager);
			dialog.Run ();
			dialog.Destroy ();

			if (dialog.SelectionDone == true)
				OnNewGame (GameSession.Types.Custom);
		}

		void OnLogicOnly (object sender, EventArgs args)
		{
			OnNewGame (GameSession.Types.LogicPuzzles);
		}

		void OnAllGames (object sender, EventArgs args)
		{
			OnNewGame (GameSession.Types.AllGames);
		}

		void OnAnswerActivate (object sender, EventArgs args)
		{
			if (answer_entry.Text.Length > 0) {
				OnAnswerButtonClicked (sender, args);
				return;
			}
		}

		void OnEndGame (object sender, EventArgs args)
		{
			session.End ();

			UpdateSolution (String.Empty, GameDrawingArea.SolutionType.None);
			UpdateQuestion (String.Empty);
			UpdateStatusBar ();
			GameSensitiveUI ();
			drawing_area.QueueDraw ();
			ActiveInputControls (false);
			SetPauseResumeButtonUI (true);
		}

		void SetPauseResumeButtonUI (bool pause)
		{
			if (pause) {
				drawing_area.Paused = false;
				toolbar.PauseButton.StockId = "pause";
				toolbar.PauseButton.Label = Catalog.GetString ("Pause");
				ActiveInputControls (true);
			} else {
				drawing_area.Paused = true;
				toolbar.PauseButton.StockId = "resume";
				toolbar.PauseButton.Label = Catalog.GetString ("Resume");
				ActiveInputControls (false);
			}
			UpdateStatusBar ();
		}

		void SetPauseResumeButton (bool pause)
		{
			if (pause)
	 			session.Resume ();
			else
				session.Pause ();

			SetPauseResumeButtonUI (pause);
		}

		void OnPauseGame (object sender, EventArgs args)
		{
			SetPauseResumeButton (session.Paused);
		}

		void OnActivateToolbar (object sender, System.EventArgs args)
		{
			int width, height;
			Requisition requisition;

			requisition = toolbar.SizeRequest ();
			app_window.GetSize (out width, out height);
			toolbar.Visible = !toolbar.Visible;

			if (toolbar.Visible)
				toolbar.ShowArrow = false;

			toolbar_orientation_menuitem.Sensitive = toolbar.Visible;

			if (Preferences.Get <bool> (Preferences.ToolbarShowKey) != toolbar.Visible)
			{
				Preferences.Set <bool> (Preferences.ToolbarShowKey, toolbar.Visible);
				Preferences.Save ();
			}
			app_window.Resize (width, height - requisition.Height);
		}

		void OnVerticalToolbar (object sender, System.EventArgs args)
		{
			if (toolbar.InitCompleted  == false)
				return;

			const Gtk.Orientation orientation = Gtk.Orientation.Vertical;

			if ((Gtk.Orientation) Preferences.Get <int> (Preferences.ToolbarOrientationKey) != orientation)
			{
				Preferences.Set <int> (Preferences.ToolbarOrientationKey, (int) orientation);
				Preferences.Save ();
			}
			toolbar.Attach (orientation);
		}

		void OnHorizontalToolbar (object sender, System.EventArgs args)
		{
			if (toolbar.InitCompleted  == false)
				return;

			const Gtk.Orientation orientation = Gtk.Orientation.Horizontal;

			if ((Gtk.Orientation) Preferences.Get <int> (Preferences.ToolbarOrientationKey) != orientation)
			{
				Preferences.Set <int>  (Preferences.ToolbarOrientationKey, (int) Gtk.Orientation.Horizontal);
				Preferences.Save ();
			}
			toolbar.Attach (orientation);
		}

		void OnHistory (object sender, EventArgs args)
		{
			PlayerHistoryDialog dialog;

			dialog = new PlayerHistoryDialog (session.PlayerHistory);
			dialog.Run ();
			dialog.Destroy ();
		}

		void OnFullscreen (object sender, EventArgs args)
		{
			if (full_screen == false) {
				app_window.Fullscreen ();
			}
			else {
				app_window.Unfullscreen ();
			}

			full_screen = !full_screen;
		}

		void OnExtending (object sender, EventArgs args)
		{
			Process.Start ("http://live.gnome.org/gbrainy/Extending");
		}

		static void InitCoreLibraries ()
		{
			new DefaultServices ().RegisterServices ();

			// Configuration
			ServiceLocator.Instance.GetService <IConfiguration> ().Set (ConfigurationKeys.GamesDefinitions, Defines.DATA_DIR);
			ServiceLocator.Instance.GetService <IConfiguration> ().Set (ConfigurationKeys.GamesGraphics, Defines.DATA_DIR);
			ServiceLocator.Instance.GetService <IConfiguration> ().Set (ConfigurationKeys.ThemesDir, Defines.DATA_DIR);

			string assemblies_dir;
			assemblies_dir =  System.IO.Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
			ServiceLocator.Instance.GetService <IConfiguration> ().Set (ConfigurationKeys.AssembliesDir, assemblies_dir);
		}

		public static void Main (string [] args)
		{
			try {
				Unix.SetProcessName ("gbrainy");
			}
			catch (Exception e)
			{
				Console.WriteLine ("gbrainy.Main. Could not set process name. Error {0}", e);
			}

			DateTime start_time = DateTime.Now;

			InitCoreLibraries ();

			GtkClient app = new GtkClient ();
			CommandLine.Version ();

			CommandLine line = new CommandLine (args);
			line.Parse ();

			if (line.Continue == false)
				return;

			Gtk.Application.Init ();

			app.Initialize ();
			// Set RandomOrder before setting the custom list then it has effect of custom games
			app.Session.GameManager.RandomOrder = line.RandomOrder;
			if (line.PlayList.Length > 0) {
				app.Session.GameManager.PlayList = line.PlayList;
				app.InitialSessionType = GameSession.Types.Custom;
			}
			app.ProcessDefaults ();
			ThemeManager.Load ();

			TimeSpan span = DateTime.Now - start_time;
			Console.WriteLine (Catalog.GetString ("Startup time {0}"), span);
			Gtk.Application.Run ();
		}
	}
}
