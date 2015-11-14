﻿#region

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using HearthDb.CARD;

#endregion

namespace HearthDb
{
    public static class CardDbfWrapper
    {
        static CardDbfWrapper()
        {
            Records = new Dictionary<string, DbfRecordWrapper>();
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("HearthDb.CardDefs.xml");
            if(stream != null)
            {
                using(TextReader tr = new StreamReader(stream))
                {
                    var xml = new XmlSerializer(typeof(Dbf));
                    var cardDefs = (Dbf)xml.Deserialize(tr);
                    foreach(var record in cardDefs.Records)
                    {
                        var wrapper = new DbfRecordWrapper(record);
                        Records.Add(wrapper.LongGuid, wrapper);
                    }
                }
            }
        }

        public static Dictionary<string, DbfRecordWrapper> Records { get; }
    }

    public class DbfRecordWrapper
    {
        public DbfRecordWrapper(Record record)
        {
            Id = int.Parse(record.Fields.FirstOrDefault(x => x.Column == "ID")?.Value ?? "-1");
            MiniGuid = record.Fields.FirstOrDefault(x => x.Column == "NOTE_MINI_GUID")?.Value;
            IsCollectible = bool.Parse(record.Fields.FirstOrDefault(x => x.Column == "IS_COLLECTIBLE")?.Value.ToLower() ?? "false");
            LongGuid = record.Fields.FirstOrDefault(x => x.Column == "LONG_GUID")?.Value;
            HeroPowerId = int.Parse(record.Fields.FirstOrDefault(x => x.Column == "HERO_POWER_ID")?.Value.ToLower() ?? "-1");
            CraftingEvent = record.Fields.FirstOrDefault(x => x.Column == "CRAFTING_EVENT")?.Value;
        }

        public int Id { get; }
        public string MiniGuid { get; }
        public bool IsCollectible { get; }
        public string LongGuid { get; }
        public int HeroPowerId { get; }
        public string CraftingEvent { get; }
    }
}