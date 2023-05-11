using MAVTOOL.Comms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MAVTOOL
{
    public static class ParameterMetaDataRepository
    {
        /// <summary>
        /// Gets the parameter meta data.
        /// </summary>
        /// <param name="nodeKey">The node key.</param>
        /// <param name="metaKey">The meta key.</param>
        /// <returns></returns>
        public static string GetParameterMetaData(string nodeKey, string metaKey, string vechileType)
        {
            if (vechileType == "PX4")
            {
                return ParameterMetaDataRepositoryPX4.GetParameterMetaData(nodeKey, metaKey, vechileType);
            }
            else
            {
                return ParameterMetaDataRepositoryAPM.GetParameterMetaData(nodeKey, metaKey, vechileType);
            }
        }

        /// <summary>
        /// Return a key, value list off all options selectable
        /// </summary>
        /// <param name="nodeKey"></param>
        /// <returns></returns>
        public static List<KeyValuePair<int, string>> GetParameterOptionsInt(string nodeKey, string vechileType)
        {
            string availableValuesRaw = GetParameterMetaData(nodeKey, ParameterMetaDataConstants.Values, vechileType);
            string[] availableValues = availableValuesRaw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (availableValues.Any())
            {
                var splitValues = new List<KeyValuePair<int, string>>();
                // Add the values to the ddl
                foreach (string val in availableValues)
                {
                    try
                    {
                        string[] valParts = val.Split(new[] { ':' });
                        splitValues.Add(new KeyValuePair<int, string>(int.Parse(valParts[0].Trim()),
                            (valParts.Length > 1) ? valParts[1].Trim() : valParts[0].Trim()));
                    }
                    catch
                    {
                        Console.WriteLine("Bad entry in param meta data: " + nodeKey);
                    }
                }
                ;

                return splitValues;
            }

            return new List<KeyValuePair<int, string>>();
        }

        public static List<KeyValuePair<int, string>> GetParameterBitMaskInt(string nodeKey, string vechileType)
        {
            string availableValuesRaw;

            availableValuesRaw = GetParameterMetaData(nodeKey, ParameterMetaDataConstants.Bitmask, vechileType);

            string[] availableValues = availableValuesRaw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (availableValues.Any())
            {
                var splitValues = new List<KeyValuePair<int, string>>();
                // Add the values to the ddl
                foreach (string val in availableValues)
                {
                    try
                    {
                        string[] valParts = val.Split(new[] { ':' });
                        splitValues.Add(new KeyValuePair<int, string>(int.Parse(valParts[0].Trim()),
                            (valParts.Length > 1) ? valParts[1].Trim() : valParts[0].Trim()));
                    }
                    catch
                    {
                        Console.WriteLine("Bad entry in param meta data: " + nodeKey);
                    }
                }
                ;

                return splitValues;
            }

            return new List<KeyValuePair<int, string>>();
        }

        public static bool GetParameterRange(string nodeKey, ref double min, ref double max, string vechileType)
        {
            string rangeRaw = ParameterMetaDataRepository.GetParameterMetaData(nodeKey, ParameterMetaDataConstants.Range,
                vechileType);

            string[] rangeParts = rangeRaw.Split(new[] { ' ' });
            if (rangeParts.Count() == 2)
            {
                float lowerRange;
                if (float.TryParse(rangeParts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out lowerRange))
                {
                    float upperRange;
                    if (float.TryParse(rangeParts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out upperRange))
                    {
                        min = lowerRange;
                        max = upperRange;

                        return true;
                    }
                }
            }

            return false;
        }

        public static bool GetParameterRebootRequired(string nodeKey, string vechileType)
        {
            // set the default answer
            bool answer = false;

            string rebootrequired = ParameterMetaDataRepository.GetParameterMetaData(nodeKey,
                ParameterMetaDataConstants.RebootRequired, vechileType);

            if (!string.IsNullOrEmpty(rebootrequired))
            {
                bool.TryParse(rebootrequired, out answer);
            }

            return answer;
        }

        public static bool GetParameterIncrement(string nodeKey, ref double inc, string vechileType)
        {
            string incrementAmt = ParameterMetaDataRepository.GetParameterMetaData(nodeKey,
                ParameterMetaDataConstants.Increment, vechileType);
            if (incrementAmt.Length == 0) return false;
            float Amt = 0;
            float.TryParse(incrementAmt, NumberStyles.Float, CultureInfo.InvariantCulture, out Amt);
            inc = Amt;
            return true;
        }
    }

    public class ParameterMetaDataRepositoryPX4
    {
        private static XDocument _parameterMetaDataXML;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMetaDataRepository"/> class.
        /// </summary>
        public static void CheckLoad()
        {
            if (_parameterMetaDataXML == null)
                Reload();
        }

        public static void Reload()
        {
            string paramMetaDataXMLFileName = String.Format("{0}{1}", Settings.GetUserDataDirectory(), "ParameterFactMetaData.xml");

            string paramMetaDataXMLFileNameBackup = String.Format("{0}{1}{2}", Settings.GetRunningDirectory(),
                Path.DirectorySeparatorChar, "ParameterFactMetaData.xml");

            try
            {
                if (File.Exists(paramMetaDataXMLFileName))
                    _parameterMetaDataXML = XDocument.Load(paramMetaDataXMLFileName);

                // error loading the good file, load the backup
                if (File.Exists(paramMetaDataXMLFileNameBackup) && _parameterMetaDataXML == null)
                {
                    _parameterMetaDataXML = XDocument.Load(paramMetaDataXMLFileNameBackup);
                    Console.WriteLine("Using backup param data");
                }
            }
            catch
            {
            }
        }

        static string ConvertMetaKey(string input)
        {
            if (input == ParameterMetaDataConstants.DisplayName)
                return "short_desc";

            if (input == ParameterMetaDataConstants.Range)
                return "range";

            if (input == ParameterMetaDataConstants.Range)
                return "range";

            if (input == ParameterMetaDataConstants.Description)
                return "long_desc";

            if (input == ParameterMetaDataConstants.Increment)
                return "increment";

            if (input == ParameterMetaDataConstants.Units)
                return "unit";

            if (input == ParameterMetaDataConstants.Values)
                return "values";

            return input;
        }

        /// <summary>
        /// Gets the parameter meta data.
        /// </summary>
        /// <param name="nodeKey">The node key.</param>
        /// <param name="metaKey">The meta key.</param>
        /// <returns></returns>
        public static string GetParameterMetaData(string nodeKey, string metaKey, string vechileType = "")
        {
            CheckLoad();

            if (_parameterMetaDataXML != null)
            {
                metaKey = ConvertMetaKey(metaKey);

                try
                {
                    //parameters - group - parameter
                    //metakeys - short_desc min max decimal long_desc increment unit
                    //values value

                    var nodeKeyLower = nodeKey.ToLower();

                    var groups = _parameterMetaDataXML.Element("parameters").Elements("group");

                    foreach (var group in groups)
                    {
                        if (group != null && group.HasElements)
                        {
                            var parameters = group.Elements("parameter");

                            foreach (var parameter in parameters)
                            {
                                if (parameter != null && parameter.HasElements)
                                {
                                    // match param name
                                    var node = parameter.Attribute("name");
                                    if (node.Value.ToLower() == nodeKeyLower)
                                    {
                                        if (metaKey == "values")
                                        {
                                            try
                                            {
                                                var values = parameter.Element("values");
                                                if (values == null)
                                                    return string.Empty;
                                                var valuearray = values.Elements("value");
                                                string value = "";
                                                foreach (var valueelement in valuearray)
                                                {
                                                    var no = valueelement.Attribute("code");
                                                    if (no == null)
                                                        continue;
                                                    var val = valueelement.Value;
                                                    if (val == null)
                                                        continue;
                                                    value += no.Value + ":" + val + ",";
                                                }

                                                return value.TrimEnd(',');
                                            }
                                            catch
                                            {
                                                return string.Empty;
                                            }
                                        }
                                        else if (metaKey.ToLower() == "range")
                                        {
                                            return GetParameterMetaData(nodeKey, "min") + " " + GetParameterMetaData(nodeKey, "max");
                                        }
                                        else
                                        {
                                            var key = parameter.Element(metaKey);
                                            if (key != null)
                                            {
                                                return key.Value;
                                            }
                                            else
                                            {
                                                return string.Empty;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                } // Exception System.ArgumentException: '' is an invalid expanded name.
            }

            return string.Empty;
        }
    }


    public sealed class ParameterMetaDataConstants
    {
        #region Markers

        public const string ParamDelimeter = "@";
        public const string PathDelimeter = ",";
        public const string Param = "Param";
        public const string Group = "Group";
        public const string Path = "Path";

        public const string NestedGroup = @"AP_NESTEDGROUPINFO\((.+),.+\)";

        #endregion

        #region Meta Keys

        public const string DisplayName = "DisplayName";
        public const string Description = "Description";
        public const string Units = "Units";
        public const string Range = "Range";
        public const string Values = "Values";
        public const string Increment = "Increment";
        public const string User = "User";
        public const string RebootRequired = "RebootRequired";
        public const string Bitmask = "Bitmask";
        public const string ReadOnly = "ReadOnly";

        #endregion

        #region Meta Values

        public const string Advanced = "Advanced";
        public const string Standard = "Standard";

        #endregion
    }
}
