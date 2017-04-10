using System;
using System.Collections.Generic;
using System.IO;

namespace ProjectHub.Core
{
    public class ConfigurationData
    {
        public Dictionary<string, string> Data;

        public ConfigurationData(string FilePath)
        {
            Data = new Dictionary<string, string>();

            try
            {
                using (var Stream = new StreamReader(FilePath))
                {
                    string Line = "";

                    while ((Line = Stream.ReadLine()) != null)
                    {
                        if (Line.Length < 1 || Line.StartsWith("#"))
                        {
                            continue;
                        }

                        int DelimiterIndex = Line.IndexOf('=');

                        if (DelimiterIndex != -1)
                        {
                            string Key = Line.Substring(0, DelimiterIndex);
                            string Val = Line.Substring(DelimiterIndex + 1);

                            Data.Add(Key, Val);
                        }
                    }
                }
            }
            catch (Exception Error)
            {
                throw new ArgumentException("Could not process configuration file: " + Error.Message);
            }
        }
    }
}