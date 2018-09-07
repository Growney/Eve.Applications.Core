using Eve.Static.Standard.inv;
using Gware.Standard.Storage.Controller;
using System;
using System.Collections.Generic;

namespace Eve.Static.Standard
{
    public static class SkillHelper
    {
        public const byte c_numberOfTrainingAttributes = 5;
        public const byte c_availablePoints = 14;
        public const byte c_maximumRemappedPoints = 10;
        public static IEnumerable<inv.InvTypes> CreateTypeList(IStaticDataCache cache, IEnumerable<long> list)
        {
            List<inv.InvTypes> obj = new List<inv.InvTypes>();
            foreach (int typeID in list)
            {
                obj.Add(cache.GetItem<inv.InvTypes>(typeID));
            }
            return obj;
        }
        public static IEnumerable<(inv.InvTypes type, T extra)> CreateTypeList<T>(IStaticDataCache cache, IEnumerable<(int typeID, T extra)> list)
        {
            List<(inv.InvTypes type, T level)> retVal = new List<(inv.InvTypes type, T level)>();
            foreach ((int typeID, T level) in list)
            {
                retVal.Add((cache.GetItem<inv.InvTypes>(typeID), level));
            }
            return retVal;
        }

        public static TrainingAttributes CalculateOptimalRemap(ICommandController controller, IStaticDataCache cache,IEnumerable<(int typeID, double spToTrain)> skills)
        {
            return CalculateOptimalRemap(controller, cache, CreateTypeList(cache, skills));
        }
        public static TrainingAttributes CalculateOptimalRemap(ICommandController controller, IStaticDataCache cache, IEnumerable<(inv.InvTypes type, double spToTrain)> skills)
        {
            return CalculateOptimalRemap(controller, cache, skills, TrainingAttributes.Base);
        }
        public static TrainingAttributes CalculateOptimalRemap(ICommandController controller, IStaticDataCache cache, IEnumerable<(int typeID, double spToTrain)> skills, TrainingAttributes implants)
        {
            return CalculateOptimalRemap(controller, cache, CreateTypeList(cache, skills),implants);
        }
        public static TrainingAttributes CalculateOptimalRemap(ICommandController controller, IStaticDataCache cache,IEnumerable<(inv.InvTypes type, double spToTrain)> skills, TrainingAttributes implants)
        {

            Dictionary<eAttributeType, Dictionary<eAttributeType, double>> combinations = new Dictionary<eAttributeType, Dictionary<eAttributeType, double>>();

            foreach((inv.InvTypes type, double spToTrain) in skills)
            {
                Dictionary<int,dgm.dgmTypeAttribute> attributes = type.GetTypeGroupedAttributes();

                if(attributes.ContainsKey((int)eAttributeType.primaryAttribute) && attributes.ContainsKey((int)eAttributeType.secondaryAttribute))
                {
                    eAttributeType primary = (eAttributeType)attributes[(int)eAttributeType.primaryAttribute].GetMaxValue();
                    eAttributeType secondary = (eAttributeType)attributes[(int)eAttributeType.secondaryAttribute].GetMaxValue();

                    if (!combinations.ContainsKey(primary))
                    {
                        combinations.Add(primary, new Dictionary<eAttributeType, double>());
                    }

                    if (!combinations[primary].ContainsKey(secondary))
                    {
                        combinations[primary].Add(secondary, 0);
                    }

                    combinations[primary][secondary] += spToTrain;
                }
            }
            List<(double spToTrain, eAttributeType primary, eAttributeType secondary)> totals = new List<(double spToTrain, eAttributeType primary, eAttributeType secondary)>();
            foreach (eAttributeType primary in combinations.Keys)
            {
                foreach(eAttributeType secondary in combinations[primary].Keys)
                {
                    totals.Add((combinations[primary][secondary], primary, secondary));
                }
            }

            return CalculateOptimalRemap(totals,implants);
        }
        private static TrainingAttributes CalculateOptimalRemap(IEnumerable<(double spToTrain, eAttributeType primary, eAttributeType secondary)> skills)
        {
            return CalculateOptimalRemap(skills, TrainingAttributes.Zero);
        }
        private static TrainingAttributes CalculateOptimalRemap(IEnumerable<(double spToTrain, eAttributeType primary, eAttributeType secondary)> skills, TrainingAttributes implants)
        {
            HashSet<eAttributeType> used = new HashSet<eAttributeType>();
            foreach((_, eAttributeType primary, eAttributeType secondary) in skills)
            {
                if (!used.Contains(primary))
                {
                    used.Add(primary);
                }
                if (!used.Contains(secondary))
                {
                    used.Add(secondary);
                }

                if(used.Count >= c_numberOfTrainingAttributes)
                {
                    break;
                }
            }
            TrainingAttributes bestAttributes = TrainingAttributes.Base;
            TimeSpan shortest = CalculateTimeToTrain(bestAttributes,skills);
            int maxCharisma = (used.Contains(eAttributeType.charisma)) ? c_maximumRemappedPoints : 0;
            for (byte charisma = 0; charisma <= maxCharisma && charisma <= c_maximumRemappedPoints; charisma++)
            {
                int maxIntelligence = (used.Contains(eAttributeType.intelligence)) ? c_availablePoints - charisma : 0;
                for (byte intelligence = 0; intelligence <= maxIntelligence && intelligence <= c_maximumRemappedPoints; intelligence++)
                {
                    int maxMemory = (used.Contains(eAttributeType.memory)) ? maxIntelligence - intelligence : 0;
                    for (byte memory = 0; memory <= maxMemory && memory <= c_maximumRemappedPoints; memory++)
                    {
                        int maxPerception = (used.Contains(eAttributeType.perception)) ? maxMemory - memory : 0;
                        for (byte perception = 0; perception <= maxPerception && perception <= c_maximumRemappedPoints; perception++)
                        {
                            int maxWillpower = (used.Contains(eAttributeType.willpower)) ? maxPerception - perception : 0;
                            for (byte willpower = 0; willpower <= maxWillpower && willpower <= c_maximumRemappedPoints; willpower++)
                            {
                                TrainingAttributes attributes = new TrainingAttributes(perception, memory, willpower, intelligence, charisma) + TrainingAttributes.Base;
                                TimeSpan thisRun = CalculateTimeToTrain(attributes + implants,skills);
                                if(thisRun < shortest)
                                {
                                    shortest = thisRun;
                                    bestAttributes = attributes;
                                }
                            }
                        }
                    }
                }
            }
            return bestAttributes;
            
        }
        
                
        public static double CalculateSPTrained(ICommandController controller, IStaticDataCache cache, TrainingAttributes attributes, int typeID, TimeSpan trainedFor)
        {
            return CalculateSPTrained(controller, cache, attributes, cache.GetItem<inv.InvTypes>(typeID), trainedFor);
        }
        public static double CalculateSPTrained(ICommandController controller, IStaticDataCache cache, TrainingAttributes attributes, inv.InvTypes type, TimeSpan trainedFor)
        {
            double trained = 0;
            Dictionary<int, dgm.dgmTypeAttribute> skillAttributes = type.GetTypeGroupedAttributes();

            if (skillAttributes.ContainsKey((int)eAttributeType.skillTimeConstant)
                && skillAttributes.ContainsKey((int)eAttributeType.primaryAttribute)
                && skillAttributes.ContainsKey((int)eAttributeType.secondaryAttribute))
            {
                double spPerMinute = GetSPPerMinute(attributes, skillAttributes[(int)eAttributeType.primaryAttribute], skillAttributes[(int)eAttributeType.secondaryAttribute]);
                trained = spPerMinute * trainedFor.TotalMinutes;
            }
            return trained;
        }

       

        public static TimeSpan CalculateTimeToTrain(ICommandController controller, IStaticDataCache cache, TrainingAttributes attributes, int typeID, double spToTrain)
        {
            return CalculateTimeToTrain(controller, cache, attributes, new List<(int, double)>() { (typeID,spToTrain) });
        }
        public static TimeSpan CalculateTimeToTrain(ICommandController controller, IStaticDataCache cache, TrainingAttributes attributes, inv.InvTypes type, double spToTrain)
        {
            return CalculateTimeToTrain(controller, cache, attributes, new List<(inv.InvTypes type, double)>() { (type, spToTrain) });
        }
        public static TimeSpan CalculateTimeToTrain(ICommandController controller, IStaticDataCache cache, TrainingAttributes attributes, IEnumerable<(int type, double spToTrain)> skills)
        {
            return CalculateTimeToTrain(controller, cache, attributes, CreateTypeList(cache, skills));
        }
        public static TimeSpan CalculateTimeToTrain(ICommandController controller, IStaticDataCache cache, TrainingAttributes attributes, IEnumerable<(inv.InvTypes type, double spToTrain)> skills)
        {
            double minutes = 0;
            foreach ((inv.InvTypes type, double spToTrain) in skills)
            {
                minutes += spToTrain / GetSPPerMinute(controller, cache, attributes, type);
            }
            return TimeSpan.FromMinutes(minutes);

        }
        public static TimeSpan CalculateTimeToTrain(ICommandController controller,IStaticDataCache cache,TrainingAttributes attributes,int typeID,int level)
        {
            inv.InvTypes type = cache.GetItem<inv.InvTypes>(typeID);
            return CalculateTimeToTrain(controller, cache, attributes, type, level);

        }
        public static TimeSpan CalculateTimeToTrain(double sp,double primary,double secondary)
        {
            return TimeSpan.FromMinutes(sp / GetSPPerMinute(primary, secondary));
        }
        public static TimeSpan CalculateTimeToTrain(TrainingAttributes attributes, IEnumerable<(double sp, eAttributeType primary, eAttributeType secondary)> skills)
        {
            TimeSpan total = TimeSpan.Zero;
            foreach ((double spToTrain, eAttributeType primary, eAttributeType secondary) in skills)
            {
                total = total + CalculateTimeToTrain(spToTrain, attributes.GetValue(primary), attributes.GetValue(secondary));
            }
            return total;
        }
        

        public static double GetSPPerMinute(ICommandController controller, IStaticDataCache cache, TrainingAttributes attributes, int typeID)
        {
            return GetSPPerMinute(controller, cache, attributes, cache.GetItem<inv.InvTypes>(typeID));
        }
        public static double GetSPPerMinute(ICommandController controller, IStaticDataCache cache, TrainingAttributes attributes, inv.InvTypes type)
        {
            double retVal = 0;

            Dictionary<int, dgm.dgmTypeAttribute> skillAttributes = type.GetTypeGroupedAttributes();

            if (skillAttributes.ContainsKey((int)eAttributeType.skillTimeConstant)
                && skillAttributes.ContainsKey((int)eAttributeType.primaryAttribute)
                && skillAttributes.ContainsKey((int)eAttributeType.secondaryAttribute))
            {
                float rank = skillAttributes[(int)eAttributeType.skillTimeConstant].ValueFloat;
                retVal = GetSPPerMinute(attributes, skillAttributes[(int)eAttributeType.primaryAttribute], skillAttributes[(int)eAttributeType.secondaryAttribute]);
            }

            return retVal;
        }
        public static double GetSPPerMinute(TrainingAttributes attributes, dgm.dgmTypeAttribute primary, dgm.dgmTypeAttribute secondary)
        {
            return GetSPPerMinute(attributes, (eAttributeType)(int)primary.GetMaxValue(), (eAttributeType)(int)secondary.GetMaxValue());
        }
        public static double GetSPPerMinute(TrainingAttributes attributes, eAttributeType primary,eAttributeType secondary)
        {
            return GetSPPerMinute(attributes.GetValue(primary), attributes.GetValue(secondary));
        }
        public static double GetSPPerMinute(double primary, double secondary)
        {
            double spPerMinute = 0;
            spPerMinute += primary;
            spPerMinute += secondary / 2.0;
            return spPerMinute;
        }

       
        public static double GetSkillLevelSP(float rank, int level)
        {
            if (level > 0)
            {
                return (long)(Math.Pow(2.0, (2.5 * (level - 1.0))) * 250.0 * rank);
            }
            else
            {
                return 0.0;
            }

        }
        public static TrainingAttributes GetImplantsBonus(ICommandController controller, IStaticDataCache cache, IEnumerable<long> typeID)
        {
            return GetImplantsBonus(controller, cache, CreateTypeList(cache, typeID));
        }
        public static TrainingAttributes GetImplantsBonus(ICommandController controller, IStaticDataCache cache, IEnumerable<inv.InvTypes> types)
        {
            TrainingAttributes retVal = TrainingAttributes.Zero;

            foreach (inv.InvTypes type in types)
            {
                retVal += type.GetTypeGroupedAttributes();
            }

            return (retVal);
        }
    }
}
