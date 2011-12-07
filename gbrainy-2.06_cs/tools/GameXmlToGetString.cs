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
using System.IO;
using System.Collections.Generic;

using gbrainy.Core.Main.Xml;

public class GameXmlToGetString
{
	static readonly string output = "GameXmlGetString.cs";
	static readonly string games = "../data/games.xml";
	static readonly string template = "GameXmlGetStringTemplate.cs";

	static void Main (string[] args)
	{
		Dictionary <string, string> tokens = new Dictionary <string, string> ();
		StringBuilder strings = new StringBuilder ();
		GamesXmlFactory factory;
		TextWriter tw;
		string games_file, template_file, output_file, str;
		int cnt = 0;

		output_file = args.Length > 1 ?  Path.Combine (args[1], output) : output;

		tw = new StreamWriter (output_file);
		factory = new GamesXmlFactory ();

		games_file = args.Length > 0 ?  Path.Combine (args[0], games) : games;
		factory.Read (games_file);

		// Build GetStrings
		foreach (GameXmlDefinition definition in factory.Definitions)
		{
			if (definition.Question != null) {
				str = GetStringFromDefinition (definition.Question);
				if (String.IsNullOrEmpty (str) == false) {
					strings.AppendLine (str);
					cnt++;
				}
			}

			if (definition.Rationale != null) {
				str = GetStringFromDefinition (definition.Rationale);
				if (String.IsNullOrEmpty (str) == false) {
					strings.AppendLine (str);
					cnt++;
				}
			}

			foreach (GameXmlDefinitionVariant variant in definition.Variants)
			{
				if (variant.Question != null)
				{
					str = GetStringFromDefinition (variant.Question);
					if (String.IsNullOrEmpty (str) == false) {
						strings.AppendLine (str);
						cnt++;
					}
				}

				if (variant.Rationale != null)
				{
					str = GetStringFromDefinition (variant.Rationale);
					if (String.IsNullOrEmpty (str) == false) {
						strings.AppendLine (str);
						cnt++;
					}
				}
			}
		}

		tokens.Add ("@STRINGS@", strings.ToString ());

		// Replace tokens
		template_file = args.Length > 0 ?  Path.Combine (args[0], template) : template;

		string line;
		Stream read = File.OpenRead (template_file);
		StreamReader sr = new StreamReader (read);
		while (true)
		{
			line = sr.ReadLine ();
			if (line == null)
				break;

			foreach (string token in tokens.Keys)
			{
				line = line.Replace (token, tokens[token]);
			}
			tw.WriteLine (line);
		}
		read.Close ();
		tw.Close ();

		Console.WriteLine ("gbrainy's GameXmlToGetString, {0} strings extracted", cnt);
	}

	static string GetStringFromDefinition (LocalizableString localizable)
	{
		if (localizable.IsPlural () == false)
			return null;

		return String.Format ("\t\tCatalog.GetPluralString (\"{0}\",\n\t\t\t\"{1}\",\n\t\t\t{2});\n",
			localizable.String,
			localizable.PluralString,
			"variable");
	}
}
