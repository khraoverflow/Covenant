﻿// Author: Ryan Cobb (@cobbr_io)
// Project: Covenant (https://github.com/cobbr/Covenant)
// License: GNU GPLv3

using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;
using YamlDotNet.Serialization;

using Covenant.Core;
using Covenant.Models.Grunts;

namespace Covenant.Models.Listeners
{
    public class ListenerType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), YamlIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [JsonIgnore, System.Text.Json.Serialization.JsonIgnore, YamlIgnore]
        public List<Listener> Listeners { get; set; }
        [JsonIgnore, System.Text.Json.Serialization.JsonIgnore, YamlIgnore]
        public List<ImplantTemplate> ImplantTemplates { get; set; }
    }

    public enum ListenerStatus
    {
        Uninitialized,
        Active,
        Stopped,
        Deleted
    }

    public class Listener : ILoggable
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = Utilities.CreateShortGuid();
        [Required, StringLength(100), RegularExpression("^[a-zA-Z0-9]*$")]
        public string GUID { get; set; } = Utilities.CreateShortGuid();
        [Required]
        public string Description { get; set; } = "A generic listener.";
        [Required, RegularExpression(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$", ErrorMessage = "The field BindAddress must be an IPv4 Address.")]
        public string BindAddress { get; set; } = "0.0.0.0";
        [Required, Range(1, 65535)]
        public int BindPort { get; set; } = 80;
        [Required]
        public List<string> ConnectAddresses { get; set; } = new List<string>();
        [Required, Range(1, 65535)]
        public int ConnectPort { get; set; } = 80;
        [Required]
        public int ProfileId { get; set; }
        public Profile Profile { get; set; }

        [Required]
        public int ListenerTypeId { get; set; }
        public ListenerType ListenerType { get; set; }

        [Required]
        public ListenerStatus Status { get; set; } = ListenerStatus.Uninitialized;
        public string CovenantUrl { get; set; }
        public string CovenantToken { get; set; }

        public DateTime StartTime { get; set; } = DateTime.MinValue;

        public virtual CancellationTokenSource Start() { return null; }
        public virtual void Stop(CancellationTokenSource cancellationTokenSource) { }

        protected string ListenerDirectory { get { return Common.CovenantListenersDirectory + Utilities.GetSanitizedFilename(this.GUID) + Path.DirectorySeparatorChar; } }

        // Listener|Action|ID|Name|GUID|BindAddress|BindPort|ConnectAddresses|ConnectPort|ProfileID|ListenerType|Status
        public string ToLog(LogAction action) => $"Listener|{action}|{this.Id}|{this.Name}|{this.GUID}|{this.BindAddress}|{this.BindPort}|{string.Join(",", this.ConnectAddresses)}|{this.ConnectPort}|{this.ProfileId}|{this.ListenerType.Name}|{this.Status}";
    }

    public class ListenerStartException : Exception
    {
        public ListenerStartException(string message) : base(message) { }
    }
}
