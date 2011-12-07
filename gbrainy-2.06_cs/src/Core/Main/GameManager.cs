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
using System.Collections.Generic;
using System.Reflection;

using gbrainy.Core.Services;

#if MONO_ADDINS
using Mono.Addins;
using Mono.Addins.Setup;
#endif

using gbrainy.Core.Main.Verbal;
using gbrainy.Core.Main.Xml;

namespace gbrainy.Core.Main
{
	public class GameManager
	{
		static Type[] VerbalAnalogiesInternal = new Type[]
		{
			typeof (AnalogiesQuestionAnswer),
			typeof (AnalogiesMultipleOptions),
			typeof (AnalogiesPairOfWordsOptions),
			typeof (AnalogiesPairOfWordsCompare),
		};

		public class GameLocator
		{
			public Type TypeOf { get; set; }
			public int Variant { get; set; }
			public GameTypes GameType { get; set; }
			public bool IsGame { get; set; }

			public GameLocator (Type type, int variant, GameTypes game_type, bool isGame)
			{
				TypeOf = type;
				Variant = variant;
				GameType = game_type;
				IsGame = isGame;
			}
		}

		GameSession.Types game_type;
		IEnumerator <int> enumerator;
		GameDifficulty difficulty;
		bool color_blind;

		List <GameLocator> available_games; 	// List of all available games in the system
		List <int> play_list;  		// Play list for the Selected difficulty, game types
		int cnt_logic, cnt_memory, cnt_calculation, cnt_verbal;

		public GameManager ()
		{
			game_type = GameSession.Types.None;
			difficulty = GameDifficulty.Medium;
			available_games = new List <GameLocator> ();
			play_list = new List <int> ();
			cnt_logic = cnt_memory = cnt_calculation = cnt_verbal = 0;
			RandomOrder = true;
		}

		public GameTypes AvailableGameTypes {
			get {
				GameTypes types = GameTypes.None;

				if (cnt_logic > 0)
					types |= GameTypes.LogicPuzzle;

				if (cnt_calculation > 0)
					types |= GameTypes.Calculation;

				if (cnt_memory > 0)
					types |= GameTypes.Memory;

				if (cnt_verbal > 0)
					types |= GameTypes.VerbalAnalogy;

				return types;
			}
		}

		public GameSession.Types GameType {
			get {return game_type; }
			set {
				if (game_type == value)
					return;

				game_type = value;

				if ((game_type & GameSession.Types.Custom) != GameSession.Types.Custom)
					BuildPlayList (available_games);
			}
		}

		public GameDifficulty Difficulty {
			set {
				if (difficulty == value)
					return;

				difficulty = value;

				if ((game_type & GameSession.Types.Custom) != GameSession.Types.Custom)
					BuildPlayList (available_games);
			}
			get { return difficulty; }
		}

		// Establish the PlayList (the indices of the array to available games)
		public int [] PlayList {
			get { return play_list.ToArray ();}
			set {
				play_list = new List <int> (value);

				if (RandomOrder == true)
				{
					ArrayListIndicesRandom random = new ArrayListIndicesRandom (play_list.Count);
					random.RandomizeFromArray (play_list);
					enumerator = random.GetEnumerator ();
				}
				else
					enumerator = play_list.GetEnumerator ();
			}
		}

		// Indicates if the PlayList for CustomGames is delivered in RandomOrder
 		public bool RandomOrder { get; set; }

		public bool ColorBlind { 
			get { return color_blind;} 
			set {
				if (color_blind == value)
					return;

				color_blind = value;

				if ((game_type & GameSession.Types.Custom) != GameSession.Types.Custom)
					BuildPlayList (available_games);	
			}
		}

		// Returns all the games available for playing
		public GameLocator [] AvailableGames {
			get { return available_games.ToArray (); }
		}
		
		// Gives the Assembly.Load used in GameManager the right path to load the application assemblies
		static Assembly ResolveAssemblyLoad (object sender, ResolveEventArgs args)
		{
			IConfiguration config = ServiceLocator.Instance.GetService <IConfiguration> ();	
			string asm_dir = config.Get <string> (ConfigurationKeys.AssembliesDir);
        		string full_name = System.IO.Path.Combine (asm_dir, args.Name);
			return Assembly.LoadFile (full_name);
   		}
		
		static bool domain_load;

		// Dynamic load of the gbrainy.Games.Dll assembly
		public void LoadAssemblyGames (string file)
		{
			const string CLASS = "gbrainy.Games.GameList";
			const string LOGIC_METHOD = "LogicPuzzles";
			const string CALCULATION_METHOD = "Calculation";
			const string MEMORY_METHOD = "Memory";

			Assembly asem;
			Type type = null;
			PropertyInfo prop;
			object obj;

			try
			{
				if (domain_load == false)
				{
					AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler (ResolveAssemblyLoad);
					domain_load = true;					
				}
				
				asem = Assembly.Load (file);
				
				foreach (Type t in asem.GetTypes())
				{
					if (t.FullName == CLASS)
					{
						type = t;
						break;
					}
				}

				obj = Activator.CreateInstance (type);

				prop = type.GetProperty (LOGIC_METHOD);
				if (prop != null)
					cnt_logic += AddGamesAndVariations ((Type []) prop.GetValue (obj, null));

				prop = type.GetProperty (MEMORY_METHOD);
				if (prop != null)
					cnt_memory += AddGamesAndVariations ((Type []) prop.GetValue (obj, null));

				prop = type.GetProperty (CALCULATION_METHOD);
				if (prop != null)
					cnt_calculation += AddGamesAndVariations ((Type []) prop.GetValue (obj, null));
			}

			catch (Exception e)
			{
				Console.WriteLine ("GameManager.LoadAssemblyGames. Could not load file {0}. Error {1}", file, e);
			}
		}

		// XML are stored using the Variant as a pointer to the game + the internal variant
		public void LoadGamesFromXml (string file)
		{
			// Load defined XML games
			GamesXmlFactory xml_games;

			xml_games = new GamesXmlFactory ();
			xml_games.Read (file);

			Type type = typeof (GameXml);
			int cnt = 0;

			foreach (GameXmlDefinition game in xml_games.Definitions)
			{
				// If the game has variants the game definition is used as reference 
				// but only the variants are playable. The first variant is used as game (IsGame = true)
				available_games.Add (new GameLocator (type, cnt++, game.Type, true));
				
				switch (game.Type) {
				case GameTypes.LogicPuzzle:
					cnt_logic++;
					break;
				case GameTypes.Calculation:
					cnt_calculation++;
					break;
				default:
					break;
				}
					
				for (int i = 1; i < game.Variants.Count; i++)
				{
					available_games.Add (new GameLocator (type, cnt++, game.Type, false));
				}
			}
		}

		// Load Mono plugins
		public void LoadPlugins ()
		{

	#if MONO_ADDINS
			try {
				ExtensionNodeList addins;
				Game game;
				Type [] type = new Type [1];
				string dir = System.IO.Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), "gbrainy");

				AddinManager.Initialize (dir);
				AddinManager.Registry.Update (null);
				new SetupService (AddinManager.Registry);

				addins = AddinManager.GetExtensionNodes ("/gbrainy/games/logic");
				foreach (TypeExtensionNode node in addins) {
					game = (Game) node.CreateInstance ();
					Console.WriteLine ("Loading external logic game: {0}", game);
					type [0] = game.GetType ();
					AddGamesAndVariations (type);
				}

				addins = AddinManager.GetExtensionNodes ("/gbrainy/games/memory");
				foreach (TypeExtensionNode node in addins) {
					game = (Game) node.CreateInstance ();
					Console.WriteLine ("Loading external memory game: {0}", game);
					type [0] = game.GetType ();
					AddGamesAndVariations (type);
				}

				addins = AddinManager.GetExtensionNodes ("/gbrainy/games/calculation");
				foreach (TypeExtensionNode node in addins) {
					game = (Game) node.CreateInstance ();
					Console.WriteLine ("Loading external calculation game: {0}", game);
					type [0] = game.GetType ();
					AddGamesAndVariations (type);
				}

				addins = AddinManager.GetExtensionNodes ("/gbrainy/games/verbal");
				foreach (TypeExtensionNode node in addins) {
					game = (Game) node.CreateInstance ();
					Console.WriteLine ("Loading external verbal analogy game: {0}", game);
					type [0] = game.GetType ();
					AddGamesAndVariations (type);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine ("GameManager.LoadPlugins. Error loading plugins. Error {0}", e);
			}
	#endif
		}

		// Load an XML file with analogies
		public void LoadVerbalAnalogies (string file)
		{
			AnalogiesFactory.Read (file);
			cnt_verbal += AddVerbalGamesAndVariations (VerbalAnalogiesInternal);
		}

		// Unload previous assembly, xml and verbal analogies loaded games
		public void ResetAvailableGames ()
		{
			available_games.Clear ();
		}

		public string GetGamesSummary ()
		{
			String s = string.Empty;
	#if MONO_ADDINS
			s += ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Extensions database:") + " " + 
					System.IO.Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), "gbrainy");

			s += Environment.NewLine;
	#endif
			// Translators: 'Games registered' is the games know to gbrainy (build-in and load from addins-in and external files)
			s += String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("{0} games registered: {1} logic puzzles, {2} calculation trainers, {3} memory trainers, {4} verbal analogies"),
					cnt_logic + cnt_memory + cnt_calculation + cnt_verbal,
					cnt_logic, cnt_calculation, cnt_memory, cnt_verbal);

			return s;
		}

		// Adds all the games and its variants into the available games list
		int AddGamesAndVariations (Type [] types)
		{
			Game game;
			int cnt = 0;

			foreach (Type type in types)
			{
				game = (Game) Activator.CreateInstance (type, true);
				for (int i = 0; i < game.Variants; i++)
				{
					available_games.Add (new GameLocator (type, i, game.Type, game.Variants == 1));
				}
				cnt += game.Variants;
			}
			return cnt;
		}

		// Adds all the games and its variants into the available games list
		int AddVerbalGamesAndVariations (Type [] types)
		{
			Game game;
			int cnt = 0;

			foreach (Type type in types)
			{
				game = (Game) Activator.CreateInstance (type, true);
				for (int i = 0; i < game.Variants; i++)
				{
					available_games.Add (new GameLocator (type, i, game.Type, true));
				}
				cnt += game.Variants;
			}
			return cnt;
		}


		// Taking a GameLocator list builds the play_list
		void BuildPlayList (List <GameLocator> all_games)
		{
			if ((game_type & GameSession.Types.Custom) == GameSession.Types.Custom)
				throw new InvalidOperationException ();

			play_list.Clear ();

			ArrayListIndicesRandom indices = new ArrayListIndicesRandom (all_games.Count);
			indices.Initialize ();

			List <int> logic_indices = new List <int> (cnt_logic);
			List <int> calculation_indices = new List <int> (cnt_calculation);
			List <int> memory_indices = new List <int> (cnt_memory);
			List <int> verbal_indices = new List <int> (cnt_verbal);
			bool logic, memory, calculation, verbal;

			// Decide which game_types are part of the game
			if ((game_type & GameSession.Types.AllGames) == GameSession.Types.AllGames)
			{
				logic = memory = calculation = verbal = true;
			}
			else
			{
				logic = (game_type & GameSession.Types.LogicPuzzles) == GameSession.Types.LogicPuzzles;
				calculation = (game_type & GameSession.Types.Calculation) == GameSession.Types.Calculation;
				memory = (game_type & GameSession.Types.Memory) == GameSession.Types.Memory;
				verbal = (game_type & GameSession.Types.VerbalAnalogies) == GameSession.Types.VerbalAnalogies;
			}

			// Create arrays by game type
			for (int n = 0; n < all_games.Count; n++)
			{
				switch (all_games [indices [n]].GameType) {
				case GameTypes.LogicPuzzle:
					if (logic)
						logic_indices.Add (indices [n]);
					break;
				case GameTypes.Memory:
					if (memory)
						memory_indices.Add (indices [n]);
					break;
				case GameTypes.Calculation:
					if (calculation)
						calculation_indices.Add (indices [n]);
					break;
				case GameTypes.VerbalAnalogy:
					if (verbal)
						verbal_indices.Add (indices [n]);
					break;
				default:
					throw new InvalidOperationException ("Unknown value");
				}
			}

			int total = logic_indices.Count + memory_indices.Count + calculation_indices.Count + verbal_indices.Count;
			int pos_logic, pos_memory, pos_calculation, pos_verbal;
			Random random = new Random ();

			pos_logic = pos_memory = pos_calculation = pos_verbal = 0;

			while (play_list.Count < total)
			{
				switch (random.Next (3)) {
				case 0:
					if (pos_calculation < calculation_indices.Count) play_list.Add (calculation_indices[pos_calculation++]);
					if (pos_logic < logic_indices.Count) play_list.Add (logic_indices[pos_logic++]);
					if (pos_memory < memory_indices.Count) play_list.Add (memory_indices[pos_memory++]);
					if (pos_verbal < verbal_indices.Count) play_list.Add (verbal_indices[pos_verbal++]);
					break;
				case 1:
					if (pos_memory < memory_indices.Count) play_list.Add (memory_indices[pos_memory++]);
					if (pos_calculation < calculation_indices.Count) play_list.Add (calculation_indices[pos_calculation++]);
					if (pos_verbal < verbal_indices.Count) play_list.Add (verbal_indices[pos_verbal++]);
					if (pos_logic < logic_indices.Count) play_list.Add (logic_indices[pos_logic++]);
					break;
				case 2:
					if (pos_calculation < calculation_indices.Count) play_list.Add (calculation_indices[pos_calculation++]);
					if (pos_verbal < verbal_indices.Count) play_list.Add (verbal_indices[pos_verbal++]);
					if (pos_memory < memory_indices.Count) play_list.Add (memory_indices[pos_memory++]);
					if (pos_logic < logic_indices.Count) play_list.Add (logic_indices[pos_logic++]);
					break;
				}
			}
			enumerator = play_list.GetEnumerator ();
		}

		public Game GetPuzzle ()
		{
			Game puzzle, first = null;

			while (true) {

				if (enumerator.MoveNext () == false) { // All the games have been played, restart again
					enumerator = play_list.GetEnumerator ();
					enumerator.MoveNext ();
				}

				puzzle =  (Game) Activator.CreateInstance ((Type) available_games [enumerator.Current].TypeOf, true);

				puzzle.Variant = available_games [enumerator.Current].Variant;

				if (first != null && first.GetType () == puzzle.GetType ())
					break;

				if (puzzle.IsPlayable == false)
					continue;

				if (ColorBlind == true && puzzle.UsesColors == true)
					continue;

				if (first == null)
					first = puzzle;

				if ((puzzle.Difficulty & difficulty) == difficulty)
					break;
			}

			puzzle.CurrentDifficulty = Difficulty;
			return puzzle;
		}
	}
}
