/*
 * Copyright (C) 2011 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using Mono.Unix;

// To localize Mono.Addins.Gui we should include the GUI strings in the application
// See: http://groups.google.com/group/mono-addins/browse_thread/thread/b6c1d648c3493a65
//
// This file is never compiled just used from POTFILE.in to get the strings
public class MonoAddinsStrings
{
	void SingularStrings ()
	{
		Catalog.GetString ("Add-in Manager");
		Catalog.GetString ("Additional extensions are required to perform this operation.");
		Catalog.GetString ("The following add-ins will be installed:");
		Catalog.GetString ("<big><b>Add-in Manager</b></big>");
		Catalog.GetString ("The following add-ins are currently installed:");
		Catalog.GetString ("_Install Add-ins...");
		Catalog.GetString ("_Repositories...");
		Catalog.GetString ("_Uninstall...");
		Catalog.GetString ("Enable");
		Catalog.GetString ("Disable");
		Catalog.GetString ("Add-in");
		Catalog.GetString ("Version");
		Catalog.GetString ("Other");
		Catalog.GetString ("Version:");
		Catalog.GetString ("Author:");
		Catalog.GetString ("Copyright:");
		Catalog.GetString ("Add-in Dependencies:");
		Catalog.GetString ("<b>Select the add-ins to install and click on Next</b>");
		Catalog.GetString ("Show all packages");
		Catalog.GetString ("Show new versions only");
		Catalog.GetString ("Show updates only");
		Catalog.GetString ("_Unselect All");
		Catalog.GetString ("Select _All");
		Catalog.GetString ("Add-in Installation");
		Catalog.GetString ("Name");
		Catalog.GetString ("Url");
		Catalog.GetString ("Install from:");
		Catalog.GetString ("Repository");
		Catalog.GetString ("All registered repositories");
		Catalog.GetString ("Register an on-line repository");
		Catalog.GetString ("Select the location of the repository you want to register:");
		Catalog.GetString ("Register a local repository");
		Catalog.GetString ("Url:");
		Catalog.GetString ("Browse...");
		Catalog.GetString ("Path:");

		/* Mono Addins 0.6 */
		Catalog.GetString ("Installed");
		Catalog.GetString ("Updates");
		Catalog.GetString ("Gallery");
		Catalog.GetString ("Repository:");
		Catalog.GetString ("Install from file...");
		Catalog.GetString ("No selection");
		Catalog.GetString ("No add-ins found");
		Catalog.GetString ("Refresh");
		Catalog.GetString ("Add-in packages");
		Catalog.GetString ("Install Add-in Package");
		Catalog.GetString ("All repositories");
		Catalog.GetString ("Manage Repositories...");
		Catalog.GetString ("Add-in Repository Management");
	}

	void PluralStrings ()
	{
		int variable = 0;
		// Translators: {0} is a number indicating the Addins available for update
		Catalog.GetPluralString ("{0} update available", "{0} updates available", variable)
	}
}

