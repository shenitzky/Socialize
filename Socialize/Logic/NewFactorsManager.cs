using log4net;
using Socialize.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Socialize.Logic
{
    public class NewFactorsManager
    {
        //Add logger
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private int MAX_VOTES_TO_ADD_TO_DB => 5;

        //Singleton implementation
        private static NewFactorsManager Instance;

        public static NewFactorsManager GetInstance()
        {
            if(Instance == null)
            {
                Instance = new NewFactorsManager();
            }
            return Instance;
        }

        //Hold the new suggested factors and number of suggestion to each one
        private Dictionary<string, int> SuggestedFactors;

        private NewFactorsManager()
        {
            SuggestedFactors = new Dictionary<string, int>();
        }

        //Add new suggested sub-class, in case sub-class already exists, inc suggestion number and add to DB if above MAX_VOTES_TO_ADD_TO_DB
        public void AddNewSuggestedFactor(string newSubClass)
        {
            if (!SuggestedFactors.ContainsKey(newSubClass))
            {
                SuggestedFactors.Add(newSubClass, 1);
            }
            else
            {
                SuggestedFactors[newSubClass] = SuggestedFactors[newSubClass] + 1;
            }
            if(SuggestedFactors[newSubClass] > MAX_VOTES_TO_ADD_TO_DB)
            {
                Log.Debug($"Add new sub-class {newSubClass} to DB");
                AddSuggestedSubClassToSystemFactors(newSubClass);
                SuggestedFactors.Remove(newSubClass);
            }
        }

        //Add new suggested sub-class, in case sub-class already exists, inc suggestion number and add to DB if above MAX_VOTES_TO_ADD_TO_DB
        public void ReduceNewSuggestedFactor(string newSubClass)
        {
            if (!SuggestedFactors.ContainsKey(newSubClass))
            {
                return;
            }
            else if (SuggestedFactors[newSubClass] > 0)
            {
                SuggestedFactors[newSubClass] = SuggestedFactors[newSubClass] - 1;
            }
        }

        private void AddSuggestedSubClassToSystemFactors(string newSubClass)
        {
            using (var db = ApplicationDbContext.Create())
            {
                var factor = db.Factors.Include(x => x.SubClasses).FirstOrDefault(x => x.Class == "Suggested by users");
                var subClass = new SubClass() { Name = newSubClass };

                factor.SubClasses.Add(subClass);

                db.SaveChanges();
            }
        }
    }
}