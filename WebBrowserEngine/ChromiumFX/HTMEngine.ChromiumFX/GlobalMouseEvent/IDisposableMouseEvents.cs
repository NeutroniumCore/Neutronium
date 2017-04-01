// This code is distributed under MIT license. 
// Copyright (c) 2015 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php
// Adapted from globalmousekeyhook https://github.com/gmamaladze/globalmousekeyhook

using System;

namespace Gma.System.MouseKeyHook
{
    /// <summary>
    ///     Provides keyboard and mouse events.
    /// </summary>
    public interface IDisposableMouseEvents : IMouseEvents, IDisposable
    {
    }
}