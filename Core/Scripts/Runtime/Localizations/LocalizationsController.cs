using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Agate.Starcade.Runtime.Main;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using Debug = UnityEngine.Debug;

namespace Starcade.Core.Localizations
{
    public static class PetSkillsLocalizations
    {
        public static readonly TableReference Table = "PetSkills";
        
        public static readonly string SubSkillsKey = "SubSkills";
        public static Dictionary<string, string> GetSubSkillsParam(string boxName, double value)
        {
            return new Dictionary<string, string>()
            {
                { "boxName", boxName },
                { "value", value.ToString(CultureInfo.InvariantCulture) }
            };
        }
    }

    public static class PetAdventureBoxLocalizations
    {
        public static readonly TableReference Table = "PetAdventureBox";
    }
    
    public static class RewardLocalizations
    {
        public static readonly TableReference Table = "Reward";
    }

    public static class PetDescriptionLocalizations
    {
        public static readonly TableReference Table = "Pet Descriptions";
    }


    public class LocalizationsController
    {
        private List<StringTable> _stringTables;
        
        public async Task LoadAllTables(Locale locale = null)
        {
            var collection = LocalizationSettings.StringDatabase.GetAllTables(locale);
            
            await collection.Task;
            
            _stringTables = collection.Result.ToList();
        }

        public async void LoadTable(TableReference tableReference, Locale locale = null)
        {
            if (_stringTables == null) _stringTables = new List<StringTable>();

            if (_stringTables.Any(table => table.TableCollectionName == tableReference)) return;
            
            var collection = LocalizationSettings.StringDatabase.GetTableAsync(tableReference, locale);
            
            await collection.Task;
            
            _stringTables.Add(collection.Result);
        }

        public string GetLocalizedString(TableReference tableRef, string key, Dictionary<string, string> param = null)
        {
            if (_stringTables == null) _stringTables = new List<StringTable>();

            var table = _stringTables.Find(table => table.TableCollectionName == tableRef);

            if (table == null)
            {
                Debug.LogError("Cannot find the specified table, please make sure you load them!");
                
                return "Empty";
            }

            var entry = table.GetEntry(key);

            if (entry == null) return "Empty";

			return entry.GetLocalizedString(param);
        }

        public void GetLocalizedString(object rewardLocalizations)
        {
            throw new NotImplementedException();
        }
    }
}
