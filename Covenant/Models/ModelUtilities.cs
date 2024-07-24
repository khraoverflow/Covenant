using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Covenant.Models.Grunts;
using System.Globalization;

namespace Covenant.Models
{
    public static class ModelUtilities
    {
        private enum OperatingSystem
        {
            Windows,
            Linux
        }

        public static List<FolderFileNode> ParseFolder(string output, string operatingSystem)
        {
            OperatingSystem os = operatingSystem.Contains("windows", StringComparison.CurrentCultureIgnoreCase) ? OperatingSystem.Windows : OperatingSystem.Linux;
            IEnumerable<string> lines = output.Split(Environment.NewLine).AsEnumerable();
            if (lines.Count() <= 2)
            {
                return null;
            }

            List<FolderFileNode> nodes = new List<FolderFileNode>();
            lines = lines.Skip(2);
            foreach(string line in lines)
            {
                try
                {
                    FolderFileNode node = ParseLine(line, os);
                    if (node != null)
                    {
                        nodes.Add(node);
                    }
                }
                catch (RegexParseException) { }
            }
            return nodes;
        }

        private static FolderFileNode ParseLine(string line, OperatingSystem operatingSystem)
        {
            IEnumerable<string> items = line.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
            List<string> reversed = items.Reverse().ToList();
            if (reversed.Count < 8)
            {
                return null;
            }

            string date12HPattern = @"\d{1,2}/\d{1,2}/\d{4} \d{1,2}:\d{2}:\d{2} (?:AM|PM)";
            string date24hPatter = @"\d{1,2}/\d{1,2}/\d{4} \d{1,2}:\d{2}:\d{2}";
            MatchCollection dates12hFormat = Regex.Matches(line,date12HPattern);
            MatchCollection dates24hFormat = Regex.Matches(line,date24hPatter);

            string strLastWriteTime;
            string strLastAccessTime;
            string strCreationTime;
            string strLength;

            if(dates12hFormat.Count > 2 )
            {
                strCreationTime = dates12hFormat[0].ToString();
                strLastAccessTime = dates12hFormat[1].ToString();
                strLastWriteTime = dates12hFormat[2].ToString();
                strLength = reversed[9];
            }
            else if (dates24hFormat.Count > 2 )
            {
                strCreationTime = dates24hFormat[0].ToString();
                strLastAccessTime = dates24hFormat[1].ToString();
                strLastWriteTime = dates24hFormat[2].ToString();
                strLength = reversed[6];
            }
            else 
            {
                return null;
            }

            string pattern = @$"^(.+)\s+{strLength}\s+{strCreationTime}\s+{strLastAccessTime}\s+{strLastWriteTime}";
            MatchCollection matches = Regex.Matches(line, pattern);

            string[] formats = { "M/d/yyyy h:m:ss tt", "MM/dd/yyyy hh:mm:ss tt", "dd/MM/yyyy HH:mm:ss", "MM/dd/yyyy HH:mm:ss" };

            if (matches.Count != 1 || matches[0].Groups.Count != 2 ||
                !long.TryParse(strLength, out long Length))
            {
                return null;
            }

            DateTime.TryParseExact(strLastWriteTime, formats, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime LastWriteTime);
            DateTime.TryParseExact(strLastAccessTime, formats, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime LastAccessTime);
            DateTime.TryParseExact(strCreationTime, formats, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime CreationTime);

            string FullName = matches[0].Groups[1].Value.TrimEnd();
            return Length == 0 ?
                new Folder
                {
                    FullName = FullName,
                    Name = GetFileName(FullName, operatingSystem.ToString()),
                    Length = Length,
                    CreationTime = CreationTime,
                    LastAccessTime = LastAccessTime,
                    LastWriteTime = LastWriteTime
                } :
                new FolderFile
                {
                    FullName = FullName,
                    Name = GetFileName(FullName, operatingSystem.ToString()),
                    Length = Length,
                    CreationTime = CreationTime,
                    LastAccessTime = LastAccessTime,
                    LastWriteTime = LastWriteTime
                };
        }

        public static string? GetDirectoryName(string? path, string operatingSystem)
        {
            OperatingSystem os = operatingSystem.Contains("windows", StringComparison.CurrentCultureIgnoreCase) ? OperatingSystem.Windows : OperatingSystem.Linux;
            if (path == null || (os == OperatingSystem.Windows ? PathInternalWindows.IsEffectivelyEmpty(path.AsSpan()) : PathInternalLinux.IsEffectivelyEmpty(path.AsSpan())))
                return null;

            int end = GetDirectoryNameOffset(path.AsSpan(), os);
            return end >= 0 ? (os == OperatingSystem.Windows ? PathInternalWindows.NormalizeDirectorySeparators(path.Substring(0, end)) : PathInternalLinux.NormalizeDirectorySeparators(path.Substring(0, end))) : null;
        }

        public static string? GetFileName(string? path, string operatingSystem)
        {
            OperatingSystem os = operatingSystem.Contains("windows", StringComparison.CurrentCultureIgnoreCase) ? OperatingSystem.Windows : OperatingSystem.Linux;
            if (path == null)
                return null;

            ReadOnlySpan<char> result = GetFileName(path.AsSpan(), os);
            if (path.Length == result.Length)
                return path;

            return result.ToString();
        }

        private static int GetDirectoryNameOffset(ReadOnlySpan<char> path, OperatingSystem operatingSystem)
        {
            int rootLength = operatingSystem == OperatingSystem.Windows ? PathInternalWindows.GetRootLength(path) : PathInternalLinux.GetRootLength(path);
            int end = path.Length;
            if (end <= rootLength)
                return -1;

            while (end > rootLength && !(operatingSystem == OperatingSystem.Windows ? PathInternalWindows.IsDirectorySeparator(path[--end]) : PathInternalLinux.IsDirectorySeparator(path[--end]))) ;

            // Trim off any remaining separators (to deal with C:\foo\\bar)
            while (end > rootLength && PathInternalWindows.IsDirectorySeparator(path[end - 1]))
                end--;

            return end;
        }

        /// <summary>
        /// The returned ReadOnlySpan contains the characters of the path that follows the last separator in path.
        /// </summary>
        private static ReadOnlySpan<char> GetFileName(ReadOnlySpan<char> path, OperatingSystem operatingSystem)
        {
            int root = GetPathRoot(path, operatingSystem).Length;

            // We don't want to cut off "C:\file.txt:stream" (i.e. should be "file.txt:stream")
            // but we *do* want "C:Foo" => "Foo". This necessitates checking for the root.

            for (int i = path.Length; --i >= 0;)
            {
                if (i < root || (operatingSystem == OperatingSystem.Windows ? PathInternalWindows.IsDirectorySeparator(path[i]) : PathInternalLinux.IsDirectorySeparator(path[i])))
                    return path.Slice(i + 1, path.Length - i - 1);
            }

            return path;
        }

        /// <remarks>
        /// Unlike the string overload, this method will not normalize directory separators.
        /// </remarks>
        private static ReadOnlySpan<char> GetPathRoot(ReadOnlySpan<char> path, OperatingSystem operatingSystem)
        {
            if (operatingSystem == OperatingSystem.Windows ? PathInternalWindows.IsEffectivelyEmpty(path) : PathInternalLinux.IsEffectivelyEmpty(path))
                return ReadOnlySpan<char>.Empty;

            int pathRoot = operatingSystem == OperatingSystem.Windows ? PathInternalWindows.GetRootLength(path) : PathInternalLinux.GetRootLength(path);
            return pathRoot <= 0 ? ReadOnlySpan<char>.Empty : path.Slice(0, pathRoot);
        }
    }
}
