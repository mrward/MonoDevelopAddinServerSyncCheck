//
// Program.cs
//
// Author:
//       Matt Ward <matt.ward@microsoft.com>
//
// Copyright (c) 2020 Microsoft Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;

namespace AddinServerSyncCheck
{
	class MainClass
	{
		public static int Main (string[] args)
		{
			try {
				return Run(args);
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
				return -1;
			}
		}

		static int Run(string[] args)
		{
			if (args.Length != 2) {
				PrintUsage();
				return -2;
			}

			string directory = args[1];
			if (!Directory.Exists(directory)) {
				Console.WriteLine ("Directory does not exist: " + directory);
				return -3;
			}

			var mpacks = new MPackResolver(directory);
			mpacks.Resolve();

			var repositoryResolver = new RepositoryInfoResolver(args[0]);
			repositoryResolver.Resolve();

			Console.WriteLine ("Url: {0}", repositoryResolver.Uri);
			foreach (AddinInfo addin in mpacks.GetAddins ()) {
				AddinInfo repositoryAddin = repositoryResolver.GetAddin (addin.Id);
				if (repositoryAddin != null) {
					if (repositoryAddin.Version != addin.Version) {
						Console.WriteLine ("Addin version mismatch");
						Console.WriteLine ("    Server: {0}", repositoryAddin);
						Console.WriteLine ("    Local: {0}", addin);
					}
				} else {
					Console.WriteLine ("Addin missing from server: {0}", addin);
				}
			}

			return 0;
		}

		static void PrintUsage ()
		{
			Console.WriteLine ("Usage:");
			Console.WriteLine ("  AddinServerSyncCheck vsmac-version addin-directory");
		}
	}
}
