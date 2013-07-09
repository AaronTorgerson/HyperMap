/*
Borrowed from the Nancy project: https://github.com/NancyFx/Nancy

The MIT License

Copyright (c) 2010 Andreas Håkansson, Steven Robbins and contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

namespace HyperMap
{

	using System;
	using System.ComponentModel;

	/// <summary>
	/// Helper interface used to hide the base <see cref="Object"/>  members from the fluent API to make it much cleaner 
	/// in Visual Studio intellisense.
	/// </summary>
	/// <remarks>Created by Daniel Cazzulino http://www.clariusconsulting.net/blogs/kzu/archive/2008/03/10/58301.aspx</remarks>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IHideObjectMembers
	{
		/// <summary>
		/// Hides the <see cref="Equals"/> method.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		bool Equals(object obj);

		/// <summary>
		/// Hides the <see cref="GetHashCode"/> method.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		int GetHashCode();

		/// <summary>
		/// Hides the <see cref="GetType"/> method.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		Type GetType();

		/// <summary>
		/// Hides the <see cref="ToString"/> method.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		string ToString();
	}
}