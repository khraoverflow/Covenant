﻿// Author: Ryan Cobb (@cobbr_io)
// Project: Covenant (https://github.com/cobbr/Covenant)
// License: GNU GPLv3

using System;
using System.Linq;
using Microsoft.CodeAnalysis;

using Covenant.Models.Listeners;
using Covenant.Models.Grunts;

namespace Covenant.Models.Launchers
{
    public class BinaryLauncher : Launcher
    {
        public BinaryLauncher()
        {
            this.Type = LauncherType.Binary;
            this.Description = "Uses a generated .NET Framework binary to launch a Grunt.";
            this.OutputKind = OutputKind.ConsoleApplication;
            this.CompressStager = false;
        }

        public override string GetLauncherString(string StagerCode, byte[] StagerAssembly, Grunt grunt, ImplantTemplate template)
        {
            this.StagerCode = StagerCode;
            this.LauncherILBytes = StagerAssembly;
            this.LauncherString = this.GetFilename();
            return this.LauncherString;
        }

        public override string GetHostedLauncherString(Listener listener, HostedFile hostedFile)
        {
            HttpListener httpListener = (HttpListener)listener;
            if (httpListener != null)
            {
				Uri hostedLocation = new Uri(httpListener.Urls.FirstOrDefault() + hostedFile.Path);
                this.LauncherString = hostedFile.Path.Split("\\").Last().Split("/").Last();
                return hostedLocation.ToString();
            }
            else { return ""; }
        }
    }
}
