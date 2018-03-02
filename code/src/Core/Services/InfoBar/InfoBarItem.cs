// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Templates.Core.Services.InfoBar
{
    public class InfoBarItem
    {
        public string Text { get; set; }

        public InfoBarItemType Type { get; set; }

        public Action Action { get; set; }

        public bool CloseAfterAction { get; set; }

        public InfoBarItem(string text, InfoBarItemType type, Action action, bool closeAfterAction = true)
        {
            Text = text;
            Type = type;
            Action = action;
            CloseAfterAction = closeAfterAction;
        }
    }

    public enum InfoBarItemType
    {
        Button,
        HyperLink,
        Close
    }
}
