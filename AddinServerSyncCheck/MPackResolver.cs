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
using System.Collections.Generic;
using System.IO;

namespace AddinServerSyncCheck
{
	class MPackResolver
	{
		string directory;
		Dictionary<string, AddinInfo> addins = new Dictionary<string, AddinInfo>(StringComparer.OrdinalIgnoreCase);

		public MPackResolver (string directory)
		{
			this.directory = directory;
		}

		public IEnumerable<AddinInfo> GetAddins()
		{
			return addins.Values;
		}

		public void Resolve()
		{
			foreach (string file in Directory.EnumerateFiles(directory, "*.mpack", SearchOption.AllDirectories)) {
				AddinInfo addin = GetAddinInfo(file);
				if (addin == null)
					continue;

				if (addins.TryGetValue (addin.Id, out AddinInfo existingAddin)) {
					if (addin.Version > existingAddin.Version) {
						addins [addin.Id] = addin;
					}
				} else {
					addins [addin.Id] = addin;
				}
			}
		}

		AddinInfo GetAddinInfo (string file)
		{
			string name = Path.GetFileNameWithoutExtension (file);
			int index = name.IndexOf ('_');

			if (index < 0)
				index = name.IndexOf('-');

			if (index < 0)
				return null;

			string id = name.Substring(0, index);
			string version = name.Substring(index + 1);

			return new AddinInfo(id, version);
		}
	}
}