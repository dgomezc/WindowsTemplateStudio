// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Templates.UI.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.Templates.UI.VisualStudio.InfoBar
{
    public class InfoBarService
    {
        private readonly IServiceProvider _serviceProvider;

        public InfoBarService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? ServiceProvider.GlobalProvider;
        }

        public void ShowInfoBarInActiveView(string message, params InfoBarItem[] items)
        {
            ShowInfoBar(true, message, items);
        }

        public void ShowInfoBarInGlobalView(string message, params InfoBarItem[] items)
        {
            ShowInfoBar(false, message, items);
        }

        private void ShowInfoBar(bool activeView, string message, params InfoBarItem[] items)
        {
            if (TryGetInfoBarData(activeView, out var infoBarHost))
            {
                CreateInfoBar(infoBarHost, message, items);
            }
        }

        private bool TryGetInfoBarData(bool activeView, out IVsInfoBarHost infoBarHost)
        {
            SafeThreading.JoinableTaskFactory.SwitchToMainThreadAsync();
            infoBarHost = null;

            if (activeView)
            {
                var monitorSelectionService = _serviceProvider.GetService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;

                // We want to get whichever window is currently in focus (including toolbars) as we could have had an exception thrown from the error list
                // or interactive window
                if (monitorSelectionService == null ||
                    ErrorHandler.Failed(monitorSelectionService.GetCurrentElementValue((uint)VSConstants.VSSELELEMID.SEID_WindowFrame, out var value)))
                {
                    return false;
                }

                var frame = value as IVsWindowFrame;
                if (ErrorHandler.Failed(frame.GetProperty((int)__VSFPROPID7.VSFPROPID_InfoBarHost, out var activeViewInfoBar)))
                {
                    return false;
                }

                infoBarHost = activeViewInfoBar as IVsInfoBarHost;
                return infoBarHost != null;
            }

            // global error info, show it on main window info bar
            var shell = _serviceProvider.GetService(typeof(SVsShell)) as IVsShell;
            if (shell == null ||
                ErrorHandler.Failed(shell.GetProperty((int)__VSSPROPID7.VSSPROPID_MainWindowInfoBarHost, out var globalInfoBar)))
            {
                return false;
            }

            infoBarHost = globalInfoBar as IVsInfoBarHost;
            return infoBarHost != null;
        }

        private void CreateInfoBar(IVsInfoBarHost infoBarHost, string message, InfoBarItem[] items)
        {
            SafeThreading.JoinableTaskFactory.SwitchToMainThreadAsync();

            var factory = _serviceProvider.GetService(typeof(SVsInfoBarUIFactory)) as IVsInfoBarUIFactory;
            if (factory == null)
            {
                // no info bar factory, don't do anything
                return;
            }

            var textSpans = new List<IVsInfoBarTextSpan>()
            {
                new InfoBarTextSpan(message)
            };

            // create action item list
            var actionItems = new List<IVsInfoBarActionItem>();

            foreach (var item in items)
            {
                switch (item.Type)
                {
                    case InfoBarItemType.Button:
                        actionItems.Add(new InfoBarButton(item.Text));
                        break;
                    case InfoBarItemType.HyperLink:
                        actionItems.Add(new InfoBarHyperlink(item.Text));
                        break;
                    case InfoBarItemType.Close:
                        break;
                    default:
                        throw new Exception();
                }
            }

            var infoBarModel = new InfoBarModel(
                textSpans,
                actionItems.ToArray(),
                KnownMonikers.StatusInformation,
                isCloseButtonVisible: true);

            if (!TryCreateInfoBarItem(factory, infoBarModel, out var infoBarUI))
            {
                return;
            }

            uint? infoBarCookie = null;
            var eventSink = new InfoBarEvents(items, () =>
            {
                SafeThreading.JoinableTaskFactory.SwitchToMainThreadAsync();

                // run given onClose action if there is one.
                items.FirstOrDefault(i => i.Type == InfoBarItemType.Close).Action?.Invoke();

                if (infoBarCookie.HasValue)
                {
                    infoBarUI.Unadvise(infoBarCookie.Value);
                }
            });

            infoBarUI.Advise(eventSink, out var cookie);
            infoBarCookie = cookie;

            infoBarHost.AddInfoBar(infoBarUI);
        }

        private static bool TryCreateInfoBarItem(IVsInfoBarUIFactory infoBarUIFactory, IVsInfoBar infoBar, out IVsInfoBarUIElement uiElement)
        {
            SafeThreading.JoinableTaskFactory.SwitchToMainThreadAsync();

            uiElement = infoBarUIFactory.CreateInfoBar(infoBar);
            return uiElement != null;
        }
    }
}
