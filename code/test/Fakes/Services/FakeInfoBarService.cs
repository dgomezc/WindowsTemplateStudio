// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Templates.Core.Services.InfoBar;

namespace Microsoft.Templates.Fakes.Services
{
    public class FakeInfoBarService : IInfoBarService
    {
        public void ShowInfoBar(string message, InfoBarItem[] items)
        {
            // TODO : Show old infobar control
        }
    }
}
