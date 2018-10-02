﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.Templates.Core.Diagnostics;
using Microsoft.Templates.Core.Gen;
using Microsoft.Templates.Core.PostActions.Catalog.Merge;

namespace Microsoft.Templates.Fakes
{
    public class FakeContextProvider : IContextProvider
    {
        public string ProjectName { get; set; }

        public string GenerationOutputPath { get; set; }

        public string DestinationPath { get; set; }

        public List<string> Projects { get; } = new List<string>();

        public Dictionary<string, List<string>> ProjectReferences { get; } = new Dictionary<string, List<string>>();

        public List<NugetReference> NugetReferences { get; } = new List<NugetReference>();

        public List<string> ProjectItems { get; } = new List<string>();

        public List<string> FilesToOpen { get; } = new List<string>();

        public List<FailedMergePostActionInfo> FailedMergePostActions { get; } = new List<FailedMergePostActionInfo>();

        public Dictionary<string, List<MergeInfo>> MergeFilesFromProject { get; } = new Dictionary<string, List<MergeInfo>>();

        public Dictionary<ProjectMetricsEnum, double> ProjectMetrics { get; } = new Dictionary<ProjectMetricsEnum, double>();
    }
}
