using System;
using Microsoft.Templates.UI.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.Templates.UI.VisualStudio
{
    public static class InfoBarManager
    {
        private static IVsInfoBarUIElement currentInfoBarElement;

        public static void Show(string textInfo)
        {
            SafeThreading.JoinableTaskFactory.SwitchToMainThreadAsync();
            IServiceProvider serviceProvider = ServiceProvider.GlobalProvider;

            var vsShell = (IVsShell)serviceProvider.GetService(typeof(SVsShell));
            var infoBarUIFactory = (IVsInfoBarUIFactory)serviceProvider.GetService(typeof(SVsInfoBarUIFactory));

            // if show in windows use __VSFPROPID7.VSFPROPID_InfoBarHost (for now not work, investigate)
            if (ErrorHandler.Failed(vsShell.GetProperty((int)__VSSPROPID7.VSSPROPID_MainWindowInfoBarHost, out var tmp)))
            {
                // we don't want to crash just because we can't show the error bar
                return;
            }

            var infoBarHost = (IVsInfoBarHost)tmp;

            // make sure we close the previous infobar
            currentInfoBarElement?.Close();

            var infoBarModel = new InfoBarModel(textInfo, isCloseButtonVisible: true, image: KnownMonikers.StatusInformation);

            uint eventCookie = 0;
            currentInfoBarElement = infoBarUIFactory.CreateInfoBar(infoBarModel);
            currentInfoBarElement.Advise(new InfoBarUIEvents(OnClose), out eventCookie);

            infoBarHost.AddInfoBar(currentInfoBarElement);

            void OnClose()
            {
                currentInfoBarElement.Unadvise(eventCookie);
            }
        }

        private sealed class InfoBarUIEvents : IVsInfoBarUIEvents
        {
            private readonly Action onClose;

            public InfoBarUIEvents(Action onClose)
            {
                this.onClose = onClose;
            }

            public void OnClosed(IVsInfoBarUIElement infoBarUIElement) => onClose();

            public void OnActionItemClicked(IVsInfoBarUIElement infoBarUIElement, IVsInfoBarActionItem actionItem)
            {
                SafeThreading.JoinableTaskFactory.SwitchToMainThreadAsync();
                (actionItem.ActionContext as Action)?.Invoke();
            }
        }
    }
}
