// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using Microsoft.Templates.Core.Services.InfoBar;
using Microsoft.Templates.UI.Threading;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.Templates.UI.VisualStudio.InfoBar
{
    public class InfoBarEvents : IVsInfoBarUIEvents
    {
        private readonly InfoBarItem[] _items;
        private readonly Action _onClose;

        public InfoBarEvents(InfoBarItem[] items, Action onClose)
        {
            _items = items;
            _onClose = onClose;
        }

        public void OnActionItemClicked(IVsInfoBarUIElement infoBarUIElement, IVsInfoBarActionItem actionItem)
        {
            SafeThreading.JoinableTaskFactory.SwitchToMainThreadAsync();

            var item = _items.FirstOrDefault(i => i.Text == actionItem.Text);

            item.Action?.Invoke();

            if (!item.CloseAfterAction)
            {
                return;
            }

            infoBarUIElement.Close();
        }

        public void OnClosed(IVsInfoBarUIElement infoBarUIElement)
        {
            _onClose();
        }
    }
}
