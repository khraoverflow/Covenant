// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Covenant.API.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class Grunt
    {
        /// <summary>
        /// Initializes a new instance of the Grunt class.
        /// </summary>
        public Grunt()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Grunt class.
        /// </summary>
        /// <param name="dotNetVersion">Possible values include: 'Net35',
        /// 'Net40', 'Net50'</param>
        /// <param name="runtimeIdentifier">Possible values include: 'win_x64',
        /// 'win_x86', 'win_arm', 'win_arm64', 'win7_x64', 'win7_x86',
        /// 'win81_x64', 'win81_x86', 'win81_arm', 'win10_x64', 'win10_x86',
        /// 'win10_arm', 'win10_arm64', 'linux_x64', 'linux_musl_x64',
        /// 'linux_arm', 'linux_arm64', 'rhel_x64', 'rhel_6_x64', 'tizen',
        /// 'tizen_4_0_0', 'tizen_5_0_0', 'osx_x64', 'osx_10_10_x64',
        /// 'osx_10_11_x64', 'osx_10_12_x64', 'osx_10_13_x64', 'osx_10_14_x64',
        /// 'osx_10_15_x64'</param>
        /// <param name="status">Possible values include: 'Uninitialized',
        /// 'Stage0', 'Stage1', 'Stage2', 'Active', 'Lost', 'Exited',
        /// 'Disconnected'</param>
        /// <param name="integrity">Possible values include: 'Untrusted',
        /// 'Low', 'Medium', 'High', 'System'</param>
        public Grunt(string name, string originalServerGuid, int implantTemplateId, bool validateCert, bool useCertPinning, string smbPipeName, int delay, int jitterPercent, int connectAttempts, System.DateTime killDate, DotNetVersion dotNetVersion, RuntimeIdentifier runtimeIdentifier, GruntStatus status, bool hidden, IntegrityLevel integrity, int? id = default(int?), string guid = default(string), IList<string> children = default(IList<string>), ImplantTemplate implantTemplate = default(ImplantTemplate), int? listenerId = default(int?), Listener listener = default(Listener), string note = default(string), string process = default(string), string userDomainName = default(string), string userName = default(string), string ipAddress = default(string), string hostname = default(string), string operatingSystem = default(string), string gruntSharedSecretPassword = default(string), string gruntRSAPublicKey = default(string), string gruntNegotiatedSessionKey = default(string), string gruntChallenge = default(string), System.DateTime? activationTime = default(System.DateTime?), System.DateTime? lastCheckIn = default(System.DateTime?), string powerShellImport = default(string), IList<GruntCommand> gruntCommands = default(IList<GruntCommand>), IList<Folder> folderRoots = default(IList<Folder>))
        {
            Id = id;
            Name = name;
            OriginalServerGuid = originalServerGuid;
            Guid = guid;
            Children = children;
            ImplantTemplateId = implantTemplateId;
            ImplantTemplate = implantTemplate;
            ValidateCert = validateCert;
            UseCertPinning = useCertPinning;
            SmbPipeName = smbPipeName;
            ListenerId = listenerId;
            Listener = listener;
            Note = note;
            Delay = delay;
            JitterPercent = jitterPercent;
            ConnectAttempts = connectAttempts;
            KillDate = killDate;
            DotNetVersion = dotNetVersion;
            RuntimeIdentifier = runtimeIdentifier;
            Status = status;
            Hidden = hidden;
            Integrity = integrity;
            Process = process;
            UserDomainName = userDomainName;
            UserName = userName;
            IpAddress = ipAddress;
            Hostname = hostname;
            OperatingSystem = operatingSystem;
            GruntSharedSecretPassword = gruntSharedSecretPassword;
            GruntRSAPublicKey = gruntRSAPublicKey;
            GruntNegotiatedSessionKey = gruntNegotiatedSessionKey;
            GruntChallenge = gruntChallenge;
            ActivationTime = activationTime;
            LastCheckIn = lastCheckIn;
            PowerShellImport = powerShellImport;
            GruntCommands = gruntCommands;
            FolderRoots = folderRoots;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int? Id { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "originalServerGuid")]
        public string OriginalServerGuid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "guid")]
        public string Guid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "children")]
        public IList<string> Children { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "implantTemplateId")]
        public int ImplantTemplateId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "implantTemplate")]
        public ImplantTemplate ImplantTemplate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "validateCert")]
        public bool ValidateCert { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "useCertPinning")]
        public bool UseCertPinning { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "smbPipeName")]
        public string SmbPipeName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "listenerId")]
        public int? ListenerId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "listener")]
        public Listener Listener { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "note")]
        public string Note { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "delay")]
        public int Delay { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "jitterPercent")]
        public int JitterPercent { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "connectAttempts")]
        public int ConnectAttempts { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "killDate")]
        public System.DateTime KillDate { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'Net35', 'Net40', 'Net50'
        /// </summary>
        [JsonProperty(PropertyName = "dotNetVersion")]
        public DotNetVersion DotNetVersion { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'win_x64', 'win_x86',
        /// 'win_arm', 'win_arm64', 'win7_x64', 'win7_x86', 'win81_x64',
        /// 'win81_x86', 'win81_arm', 'win10_x64', 'win10_x86', 'win10_arm',
        /// 'win10_arm64', 'linux_x64', 'linux_musl_x64', 'linux_arm',
        /// 'linux_arm64', 'rhel_x64', 'rhel_6_x64', 'tizen', 'tizen_4_0_0',
        /// 'tizen_5_0_0', 'osx_x64', 'osx_10_10_x64', 'osx_10_11_x64',
        /// 'osx_10_12_x64', 'osx_10_13_x64', 'osx_10_14_x64', 'osx_10_15_x64'
        /// </summary>
        [JsonProperty(PropertyName = "runtimeIdentifier")]
        public RuntimeIdentifier RuntimeIdentifier { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'Uninitialized', 'Stage0',
        /// 'Stage1', 'Stage2', 'Active', 'Lost', 'Exited', 'Disconnected'
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public GruntStatus Status { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "hidden")]
        public bool Hidden { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'Untrusted', 'Low', 'Medium',
        /// 'High', 'System'
        /// </summary>
        [JsonProperty(PropertyName = "integrity")]
        public IntegrityLevel Integrity { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "process")]
        public string Process { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "userDomainName")]
        public string UserDomainName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ipAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "hostname")]
        public string Hostname { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "operatingSystem")]
        public string OperatingSystem { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "gruntSharedSecretPassword")]
        public string GruntSharedSecretPassword { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "gruntRSAPublicKey")]
        public string GruntRSAPublicKey { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "gruntNegotiatedSessionKey")]
        public string GruntNegotiatedSessionKey { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "gruntChallenge")]
        public string GruntChallenge { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "activationTime")]
        public System.DateTime? ActivationTime { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "lastCheckIn")]
        public System.DateTime? LastCheckIn { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "powerShellImport")]
        public string PowerShellImport { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "gruntCommands")]
        public IList<GruntCommand> GruntCommands { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "folderRoots")]
        public IList<Folder> FolderRoots { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Name == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Name");
            }
            if (OriginalServerGuid == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "OriginalServerGuid");
            }
            if (SmbPipeName == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "SmbPipeName");
            }
            if (Listener != null)
            {
                Listener.Validate();
            }
            if (Delay > 2147483647)
            {
                throw new ValidationException(ValidationRules.InclusiveMaximum, "Delay", 2147483647);
            }
            if (Delay < 0)
            {
                throw new ValidationException(ValidationRules.InclusiveMinimum, "Delay", 0);
            }
            if (JitterPercent > 100)
            {
                throw new ValidationException(ValidationRules.InclusiveMaximum, "JitterPercent", 100);
            }
            if (JitterPercent < 0)
            {
                throw new ValidationException(ValidationRules.InclusiveMinimum, "JitterPercent", 0);
            }
            if (ConnectAttempts > 2147483647)
            {
                throw new ValidationException(ValidationRules.InclusiveMaximum, "ConnectAttempts", 2147483647);
            }
            if (ConnectAttempts < 0)
            {
                throw new ValidationException(ValidationRules.InclusiveMinimum, "ConnectAttempts", 0);
            }
            if (GruntCommands != null)
            {
                foreach (var element in GruntCommands)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
            if (FolderRoots != null)
            {
                foreach (var element1 in FolderRoots)
                {
                    if (element1 != null)
                    {
                        element1.Validate();
                    }
                }
            }
        }
    }
}
