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

#if CSHARP_STATIC

using System;
using System.Text;
using System.Text.RegularExpressions;

using Mono.CSharp;
using System.Reflection;
using gbrainy.Core.Services;

namespace gbrainy.Core.Libraries
{
	// This encapsulates Mono.CSharp previous to Mono 2.10 when it was a static class
	public class CSharpCompilerStatic : ICSharpCompiler
	{
		static bool? monofix_needed;
		static string stored_vars;
		static int unload_domain = 0;
		static AppDomain tempDomain;
		const int maximum_uses = 50;

		public void EvaluateCode (string c)
		{
			if (tempDomain == null)
				tempDomain = AppDomain.CreateDomain ("MonoCSharpDomain");

			// Load the Mono Compiler service in a separate domain then
			// we can recycle it to reduce memory consumption
			//
			// After Mono 2.12 this is no longer need
			// http://tirania.org/blog/archive/2011/Feb-24.html
			IConfiguration config = ServiceLocator.Instance.GetService <IConfiguration> ();
			string asm_dir = config.Get <string> (ConfigurationKeys.AssembliesDir);
			string full_name = System.IO.Path.Combine (asm_dir, "gbrainy.Core.dll");
			AssemblyName aname = AssemblyName.GetAssemblyName (full_name);
			Assembly asem = tempDomain.Load (aname);

  			CSharpCompilerStaticDomainProxy proxy = (CSharpCompilerStaticDomainProxy) tempDomain.CreateInstanceAndUnwrap (asem.FullName,
				typeof (CSharpCompilerStaticDomainProxy).FullName);

			proxy.SetCode (c);
			tempDomain.DoCallBack (proxy.EvaluateVariables);
			stored_vars = proxy.GetVars ();
			stored_vars = FixGetVars (stored_vars);

			if (unload_domain > maximum_uses)
			{
				AppDomain.Unload (tempDomain);
				unload_domain = 0;
				tempDomain = null;
			}
			else
				unload_domain++;
		}
		public string GetAllVariables ()
		{
			return stored_vars;
		}

		public string GetVariableValue (string _var)
		{
			const string exp = "([a-z0-9._%+-]+) ([a-z0-9._%+-]+) (=) ([0-9]+)";
			Match match;
			int idx, cur, newline_len;
			string line;
			string vars;

			vars = GetAllVariables ();
			Regex regex = new Regex (exp, RegexOptions.IgnoreCase);

			newline_len = System.Environment.NewLine.Length;
			cur = 0;

			do
			{
				// Process a line
				idx = vars.IndexOf (System.Environment.NewLine, cur);
				if (idx == -1) idx = vars.Length;

				line = vars.Substring (cur, idx - cur);
				cur = idx + newline_len;
				match = regex.Match (line);

				//  "int num = 2";
				//   group 1 -> int,  group 2 -> num,  group 3 -> =, group 4 -> 2
				if (match.Groups.Count == 5)
				{
					if (match.Groups[2].Value == _var)
						return match.Groups[4].Value;
				}

			} while (cur < vars.Length);

			return string.Empty;
		}


		// Before Mono 2.6 (rev. 156533) there is no line separator between variables
		static string FixGetVars (string str)
		{
			if (monofix_needed == null)
			{
				string eval, vars;

				eval = "int a = 1; int b = 1;";
				Evaluator.Run (eval);
				vars = Evaluator.GetVars ();

				monofix_needed = vars.IndexOf (System.Environment.NewLine) == -1;
			}

			if (monofix_needed == false)
				return str;

			// We just guarantee that int, doubles, and float are separated as modern Mono versions do
			StringBuilder output = new StringBuilder ();
			string [] keywords = new string [] {"int", "double", "float"};
			int pos = 0, cur = 0, tmp_pos, keyword;

			while (pos != -1)
			{
				pos = keyword = -1;
				// Look for the nearest of these keywords
				for (int i = 0; i < keywords.Length; i++)
				{
					tmp_pos = str.IndexOf (keywords [i], cur);
					if (tmp_pos == -1)
						continue;

					if (pos == -1 || pos > 0 && tmp_pos < pos) {
						keyword = i;
						pos = tmp_pos;
					}
				}

				if (pos == -1)
					continue;

				output.Append (str.Substring (cur, pos - cur));
				output.AppendLine ();
				output.Append (str.Substring (pos, keywords[keyword].Length));
				cur = pos + keywords[keyword].Length;
			}

			output.Append (str.Substring (cur, str.Length - cur));
			return output.ToString ();
		}
	}
}
#endif
