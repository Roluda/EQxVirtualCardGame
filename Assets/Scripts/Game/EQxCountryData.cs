using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game {
    [Serializable]
    public class EQxCountryData {
        public string countryName;
        public string isoCountryCode;
        public float valueCreation;
        public float valueExtraction;
        public float power;
        public float value;
        public float politicalPower;
        public float economicPower;
        public float politicalValue;
        public float economicValue;
        public float stateCapture;
        public float regulatoryCapture;
        public float humanCapture;
        public float industryDominance;
        public float firmDominance;
        public float creativeDestruction;
        public float givingIncome;
        public float takingIncome;
        public float unearnedIncome;
        public float producerRent;
        public float capitalRent;
        public float laborRent;

        public float GetValue(EQxVariableType variable) {
            switch (variable) {
                case EQxVariableType.ValueCreation:
                    return valueCreation;
                case EQxVariableType.ValueExtraction:
                    return valueExtraction;
                case EQxVariableType.Power:
                    return power;
                case EQxVariableType.Value:
                    return value;
                case EQxVariableType.PoliticalPower:
                    return politicalPower;
                case EQxVariableType.EconomicPower:
                    return economicPower;
                case EQxVariableType.PoliticalValue:
                    return politicalValue;
                case EQxVariableType.EconomicValue:
                    return economicValue;
                case EQxVariableType.StateCapture:
                    return stateCapture;
                case EQxVariableType.RegulatoryCapture:
                    return regulatoryCapture;
                case EQxVariableType.HumanCapture:
                    return humanCapture;
                case EQxVariableType.IndustryDominance:
                    return industryDominance;
                case EQxVariableType.FirmDominace:
                    return firmDominance;
                case EQxVariableType.CreativeDestruction:
                    return creativeDestruction;
                case EQxVariableType.GivingIncome:
                    return givingIncome;
                case EQxVariableType.TakingIcome:
                    return takingIncome;
                case EQxVariableType.UnearnedIncome:
                    return unearnedIncome;
                case EQxVariableType.ProducerRent:
                    return producerRent;
                case EQxVariableType.CapitalRent:
                    return capitalRent;
                case EQxVariableType.LaborRent:
                    return laborRent;
                default:
                    return 0;
            }
        }
    }
}
