// This code is distributed under MIT license. 
// Copyright (c) 2015 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php
// Adapted from globalmousekeyhook https://github.com/gmamaladze/globalmousekeyhook

using System;
using System.Windows.Forms;

namespace Gma.System.MouseKeyHook
{
    /// <summary>
    ///     Provides all mouse events.
    /// </summary>
    public interface IMouseEvents
    {
        /// <summary>
        ///     Occurs when the mouse pointer is moved.
        /// </summary>
        event MouseEventHandler MouseMove;
   
    }
}