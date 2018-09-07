using System;
using System.Collections.Generic;

namespace Eve.Static.Standard.inv
{
    public struct TrainingAttributes
    {
        private const int c_attributeBase = 17;

        public static readonly TrainingAttributes Zero = new TrainingAttributes(0, 0, 0, 0, 0);
        public static readonly TrainingAttributes Base = new TrainingAttributes(c_attributeBase, c_attributeBase, c_attributeBase, c_attributeBase, c_attributeBase);

        public double Perception { get; }
        public double Memory { get; }
        public double Willpower { get; }
        public double Intelligence { get; }
        public double Charisma { get; }

        public TrainingAttributes(double perception, double memory, double willpower, double intelligence, double charisma)
        {
            Perception = perception;
            Memory = memory;
            Willpower = willpower;
            Intelligence = intelligence;
            Charisma = charisma;
        }
        public bool Equals(TrainingAttributes attributes)
        {
            return this == attributes;
        }
        public override bool Equals(object o)
        {
            if(o is TrainingAttributes a)
            {
                return Equals(a);
            }
            else
            {
                return false;
            }
            
        }
        public override int GetHashCode()
        {
            return (Perception, Memory, Willpower, Intelligence, Charisma).GetHashCode();
        }
        public double GetValue(eAttributeType type)
        {
            double retVal = 0;
            switch (type)
            {
                case eAttributeType.charisma:
                    retVal = Charisma;
                    break;
                case eAttributeType.intelligence:
                    retVal = Intelligence;
                    break;
                case eAttributeType.memory:
                    retVal = Memory;
                    break;
                case eAttributeType.perception:
                    retVal = Perception;
                    break;
                case eAttributeType.willpower:
                    retVal = Willpower;
                    break;
                default:
                    break;
            }
            return retVal;
        }

        public static bool operator ==(TrainingAttributes a1,TrainingAttributes a2)
        {
            return a1.Perception == a2.Perception && a1.Memory == a2.Memory && a1.Willpower == a2.Willpower && a1.Intelligence == a2.Intelligence && a1.Charisma == a2.Charisma;
        }
        public static bool operator !=(TrainingAttributes a1,TrainingAttributes a2)
        {
            return a1.Perception != a2.Perception || a1.Memory != a2.Memory || a1.Willpower != a2.Willpower || a1.Intelligence != a2.Intelligence || a1.Charisma != a2.Charisma;
        }
        public static TrainingAttributes operator +(TrainingAttributes a1, Dictionary<int, dgm.dgmTypeAttribute> dic1)
        {
            return a1 + CreateFrom(dic1);
        }
        public static TrainingAttributes operator +(Dictionary<int, dgm.dgmTypeAttribute> dic1, TrainingAttributes a1)
        {
            return CreateFrom(dic1) + a1;
        }
        public static TrainingAttributes operator +(TrainingAttributes a1,TrainingAttributes a2)
        {
            return new TrainingAttributes(a1.Perception + a2.Perception,
                a1.Memory + a2.Memory,
                a1.Willpower + a2.Willpower,
                a1.Intelligence + a2.Intelligence,
                a1.Charisma + a2.Charisma);
        }
        public static TrainingAttributes operator -(TrainingAttributes a1, TrainingAttributes a2)
        {
            return new TrainingAttributes(a1.Perception - a2.Perception,
                a1.Memory - a2.Memory,
                a1.Willpower - a2.Willpower,
                a1.Intelligence - a2.Intelligence,
                a1.Charisma - a2.Charisma);
        }
        public static TrainingAttributes operator -(Dictionary<int, dgm.dgmTypeAttribute> dic1, TrainingAttributes a2)
        {
            return CreateFrom(dic1) - a2;
        }
        public static TrainingAttributes operator -(TrainingAttributes a1, Dictionary<int, dgm.dgmTypeAttribute> dic1)
        {
            return a1 - CreateFrom(dic1);
        }
        public static TrainingAttributes CreateFrom(Dictionary<eAttributeType,double> dic)
        {
            return new TrainingAttributes(
                perception: GetFromDictionary(dic, eAttributeType.perception, eAttributeType.perceptionBonus),
                memory: GetFromDictionary(dic, eAttributeType.memory, eAttributeType.memoryBonus),
                willpower: GetFromDictionary(dic, eAttributeType.willpower, eAttributeType.willpowerBonus),
                intelligence: GetFromDictionary(dic, eAttributeType.intelligence, eAttributeType.intelligenceBonus),
                charisma: GetFromDictionary(dic, eAttributeType.charisma, eAttributeType.charismaBonus));
        }
        public static TrainingAttributes CreateFrom(Dictionary<eAttributeType, int> dic)
        {
            return new TrainingAttributes(
                perception: GetFromDictionary(dic, eAttributeType.perception,eAttributeType.perceptionBonus),
                memory: GetFromDictionary(dic, eAttributeType.memory,eAttributeType.memoryBonus),
                willpower: GetFromDictionary(dic, eAttributeType.willpower,eAttributeType.willpowerBonus),
                intelligence: GetFromDictionary(dic, eAttributeType.intelligence,eAttributeType.intelligenceBonus),
                charisma: GetFromDictionary(dic, eAttributeType.charisma,eAttributeType.charismaBonus));
        }
        public static TrainingAttributes CreateFrom(Dictionary<int, dgm.dgmTypeAttribute> dic1)
        {
            double perception = Math.Max(GetValueFromAttributeTupleDictionary(dic1, eAttributeType.perception),GetValueFromAttributeTupleDictionary(dic1, eAttributeType.perceptionBonus));
            double memory = Math.Max(GetValueFromAttributeTupleDictionary(dic1, eAttributeType.memory), GetValueFromAttributeTupleDictionary(dic1, eAttributeType.memoryBonus));
            double willpower = Math.Max(GetValueFromAttributeTupleDictionary(dic1, eAttributeType.willpower), GetValueFromAttributeTupleDictionary(dic1, eAttributeType.willpowerBonus));
            double intelligence = Math.Max(GetValueFromAttributeTupleDictionary(dic1, eAttributeType.intelligence), GetValueFromAttributeTupleDictionary(dic1, eAttributeType.intelligenceBonus));
            double charisma = Math.Max(GetValueFromAttributeTupleDictionary(dic1, eAttributeType.charisma), GetValueFromAttributeTupleDictionary(dic1, eAttributeType.charismaBonus));

            return new TrainingAttributes(perception, memory, willpower, intelligence, charisma);
        }
        private static double GetValueFromAttributeTupleDictionary(Dictionary<int, dgm.dgmTypeAttribute> dic, eAttributeType type)
        {
            double retVal = 0;
            if (dic.TryGetValue((int)type, out dgm.dgmTypeAttribute attribute))
            {
                retVal = attribute.GetMaxValue();
            }
            return retVal;
        }
        private static double GetFromDictionary(Dictionary<eAttributeType,int> dic,params eAttributeType[] type)
        {
            int maxValue = 0;
            for (int i = 0; i < type.Length; i++)
            {
                if(dic.TryGetValue(type[i], out int value))
                {
                    if(value > maxValue)
                    {
                        maxValue = value;
                    }
                }
            }
            return maxValue;
        }
        private static double GetFromDictionary(Dictionary<eAttributeType, double> dic, params eAttributeType[] type)
        {
            double maxValue = 0;
            for (int i = 0; i < type.Length; i++)
            {
                if (dic.TryGetValue(type[i], out double value))
                {
                    if (value > maxValue)
                    {
                        maxValue = value;
                    }
                }
            }
            return maxValue;
        }
    }
}
