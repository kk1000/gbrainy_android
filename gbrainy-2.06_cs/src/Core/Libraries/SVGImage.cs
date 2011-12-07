/*
 * Copyright (C) 2008-2009 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.IO;
using System.Runtime.InteropServices;

namespace gbrainy.Core.Libraries
{
	//
	// SVG image class based on rsvg library
	//
	public class SVGImage : IDisposable
	{
		//lib rsvg2
		[DllImport("rsvg-2")]
		static extern void rsvg_handle_render_cairo (IntPtr Rsvghandle, IntPtr cairo_t);

		[DllImport("rsvg-2")]
		static extern IntPtr rsvg_handle_new_from_file (string file_name, out int error);

		[DllImport("rsvg-2")]
		static extern void rsvg_handle_free (IntPtr handle);

		[DllImport("rsvg-2")]
		static extern void rsvg_handle_get_dimensions (IntPtr handle, ref RsvgDimensionData dimension);

		[DllImport("rsvg-2")]
		static extern IntPtr rsvg_handle_new_from_data (byte[] data, int len, out int error);

		[StructLayout(LayoutKind.Sequential)]
		struct RsvgDimensionData
		{
		    	public int width;
		    	public int height;
		    	public double em;
			public double ex;
		}

		RsvgDimensionData dimension;
		IntPtr handle;

		public SVGImage (System.Reflection.Assembly _assembly, string resource)
		{
			try {
				byte[] array;
				Stream stream;
				int error = 0;

				stream =  _assembly.GetManifestResourceStream (resource);
				array = new byte [stream.Length];

				stream.Read (array, 0, (int) stream.Length);
			
				handle = rsvg_handle_new_from_data (array, array.Length, out error);
				rsvg_handle_get_dimensions (handle, ref dimension);
			} 
			finally
			{
				if (handle == IntPtr.Zero)
					throw new System.IO.IOException ("Resource not found: " + resource);
			}
		}
	
		public SVGImage (string file)
		{
			int error = 0;
			dimension = new RsvgDimensionData ();

			try {
				handle = rsvg_handle_new_from_file (file, out error);

				if (handle != IntPtr.Zero)		
					rsvg_handle_get_dimensions (handle, ref dimension);

			}

			finally
			{
				if (handle == IntPtr.Zero)
					throw new System.IO.IOException ("File not found: " + file);

			}
		}

		public int Width {
			get { return dimension.width; }
		}
	
		public int Height {
			get { return dimension.height; }
		}

		~SVGImage ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			System.GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (handle == IntPtr.Zero)
				return;

			rsvg_handle_free (handle);
			handle = IntPtr.Zero;
		}

		public void RenderToCairo (IntPtr cairo_surface)
		{
			if (handle != IntPtr.Zero)
				rsvg_handle_render_cairo (handle, cairo_surface);
		}
	}
}
