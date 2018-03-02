using Microsoft.Templates.UI.Threading;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
