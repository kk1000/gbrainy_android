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
using System.ComponentModel;

namespace gbrainy.Core.Main
{
	// Used by any class that can draw and due to logic requirements may need to request a redraw
	public interface IDrawRequest
	{
		// Used to synchronize threads of the application with UI thread 
		ISynchronizeInvoke SynchronizingObject { get; set;}

		// Any element that draws on request may need to request a redraw
		event EventHandler DrawRequest;
	}
}
