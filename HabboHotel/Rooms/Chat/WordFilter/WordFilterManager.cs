using System.Collections.Generic;
using ProjectHub.Database.Interfaces;
using System.Data;
using System.Text.RegularExpressions;
using System.Linq;
using System;

namespace ProjectHub.HabboHotel.Rooms.Chat.WordFilter
{
    public class WordFilterManager
    {
        private List<WordFilter> FilteredWords;

        public WordFilterManager()
        {
            FilteredWords = new List<WordFilter>();

            if (FilteredWords.Count > 0)
            {
                FilteredWords.Clear();
            }

            DataTable Data = null;

            using (IQueryAdapter DbClient = ProjectHub.GetDatabaseManager().GetQueryReactor())
            {
                DbClient.SetQuery("SELECT * FROM `" + ProjectHub.DbPrefix + "wordfilter`");
                Data = DbClient.getTable();

                if (Data != null)
                {
                    foreach (DataRow Row in Data.Rows)
                    {
                        FilteredWords.Add(new WordFilter(Convert.ToString(Row["word"]), ProjectHub.GetSettingsData().Data["wordfilter.replacement"], ProjectHub.EnumToBool(Row["strict"].ToString()), ProjectHub.EnumToBool(Row["bannable"].ToString())));
                    }
                }
            }
        }

        public string CheckMessage(string Message)
        {
            foreach (WordFilter Filter in FilteredWords.ToList())
            {
                if (Message.ToLower().Contains(Filter.GetWord) && Filter.GetStrict || Message == Filter.GetWord)
                {
                    Message = Regex.Replace(Message, Filter.GetWord, Filter.GetReplacement, RegexOptions.IgnoreCase);
                }
                else if (Message.ToLower().Contains(Filter.GetWord) && !Filter.GetStrict || Message == Filter.GetWord)
                {
                    string[] Words = Message.Split(' ');
                    Message = "";

                    foreach (string Word in Words.ToList())
                    {
                        if (Word.ToLower() == Filter.GetWord)
                        {
                            Message += Filter.GetReplacement + " ";
                        }
                        else
                        {
                            Message += Word + " ";
                        }
                    }
                }
            }

            return Message.TrimEnd(' ');
        }

        public bool CheckBannedWords(string Message)
        {
            Message = Message.Replace(" ", "").Replace(".", "").Replace("_", "").ToLower();

            foreach (WordFilter Filter in FilteredWords.ToList())
            {
                if (!Filter.GetBannable)
                {
                    continue;
                }

                if (Message.Contains(Filter.GetWord))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsFiltered(string Message)
        {
            foreach (WordFilter Filter in FilteredWords.ToList())
            {
                if (Message.Contains(Filter.GetWord))
                {
                    return true;
                }
            }

            return false;
        }
    }
}