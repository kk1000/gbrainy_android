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
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;
using System.Threading;

namespace gbrainy.Core.Platform
{
	//
	// Unix system calls
	//
	static public class Unix
	{
		[DllImport("libc")]
		static extern IntPtr localeconv ();

		[DllImport ("libc")] // Linux
		static extern int prctl (int option, byte [] arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5);

		[DllImport ("libc")] // BSD
		static extern void setproctitle (byte [] fmt, byte [] str_arg);

		[DllImport("libgtk-win32-2.0-0.dll")]
		static extern unsafe bool gtk_show_uri(IntPtr screen, IntPtr uri, uint timestamp, out IntPtr error);

		/* Taken from locale.h  */
		[StructLayout (LayoutKind.Sequential,  Pack = 1, Size = 68)]
		struct lconv
		{
			public string decimal_point;
			// 64 bytes follow
		}

		// Mono supports less locales that Unix systems
		// To overcome this limitation we setup the right locale parameters
		// when the Mono locale is InvariantCulture, that is, when the user's locale
		// has not been identified and the default Mono locale is used
		//
		// See: https://bugzilla.novell.com/show_bug.cgi?id=420468
		//
		static public void FixLocaleInfo ()
		{
			IntPtr st = IntPtr.Zero;
			lconv lv;
			int platform = (int) Environment.OSVersion.Platform;

			if (platform != 4 && platform != 128) // Only in Unix based systems
				return;

			if (CultureInfo.CurrentCulture != CultureInfo.InvariantCulture) // Culture well supported
				return;

			try {
				st = localeconv ();
				if (st == IntPtr.Zero) return;

				lv = (lconv) Marshal.PtrToStructure (st, typeof (lconv));
				CultureInfo culture = (CultureInfo) CultureInfo.CurrentCulture.Clone ();
				culture.NumberFormat.NumberDecimalSeparator = lv.decimal_point;
				Thread.CurrentThread.CurrentCulture = culture;
			}
			catch (Exception e)
			{
				Console.WriteLine ("Unix.FixLocaleInfo. Error {0}", e);
			}
		}

		public static void SetProcessName (string name)
		{
			int platform = (int) Environment.OSVersion.Platform;
			if (platform != 4 && platform != 128)
				return;

			try {
				if (prctl (15 /* PR_SET_NAME */, Encoding.ASCII.GetBytes (name + "\0"),
					IntPtr.Zero, IntPtr.Zero, IntPtr.Zero) != 0) {
					throw new ApplicationException ("Error setting process name: " +
						Mono.Unix.Native.Stdlib.GetLastError ());
				}
			} catch (EntryPointNotFoundException) {
				setproctitle (Encoding.ASCII.GetBytes ("%s\0"),
					Encoding.ASCII.GetBytes (name + "\0"));
			}
		}

		public static unsafe bool ShowUri (Gdk.Screen screen, string uri, uint timestamp)
		{
			IntPtr native_uri = GLib.Marshaller.StringToPtrGStrdup (uri);
			IntPtr error = IntPtr.Zero;
			bool raw_ret = gtk_show_uri(screen == null ? IntPtr.Zero : screen.Handle, native_uri, timestamp, out error);
			bool ret = raw_ret;
			GLib.Marshaller.Free (native_uri);
			if (error != IntPtr.Zero) throw new GLib.GException (error);
			return ret;
		}
	}
}
