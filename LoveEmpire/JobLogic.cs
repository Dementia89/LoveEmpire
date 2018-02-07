//***********************************************
//  Script: JobLogic (Love Empire)              *
//  Created By: Benjamin Holton                 *
//  Created On: 05FEB2018                       *
//  Copyright: Psychosis Entertainment (2018)   *
//***********************************************

namespace LoveEmpire
{
    public class JobLogic
    {
        public float costForLevel;
        float costMultiplier;
        public int timeForAutoComplete;
        public float lastTime;
        public float nextTime;
        public int earnings;
        int earningsPerLevel;
        public int jobLevel;

        public JobLogic(float levelCost, float costMult, int autoTime, int baseEarn, int perLevelEarn, int currentJobLevel)
        {
            costForLevel = levelCost;
            costMultiplier = costMult;
            timeForAutoComplete = autoTime;
            earnings = baseEarn;
            earningsPerLevel = perLevelEarn;
            jobLevel = currentJobLevel;
        }

        /// <summary>
        /// Checks if they have enough money to purchase a new level
        /// </summary>
        /// <param name="currencyOnHand">Money on hand.</param>
        /// <returns></returns>
        public bool CheckLevelCost(int currencyOnHand)
        {
            if (costForLevel <= currencyOnHand)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Adds a level after the CheckLevelCost returns true
        /// </summary>
        /// <param name="currencyOnHand">However much money they have on hand.</param>
        /// <returns></returns>
        public int AddLevel(int currencyOnHand)
        {
            currencyOnHand -= (int)costForLevel;
            if(jobLevel > 0)
            {
                costForLevel *= 1 + (costMultiplier * jobLevel);
            }
            else
            {
                costForLevel *= 1 + costMultiplier;
            }
            earnings += earningsPerLevel;
            jobLevel++;
            return currencyOnHand;
        }
    }
}
