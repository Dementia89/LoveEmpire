//***********************************************
//  Script: DataManagement (Love Empire)        *
//  Created By: Benjamin Holton                 *
//  Created On: 13FEB2018                       *
//  Copyright: Psychosis Entertainment (2018)   *
//***********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LoveEmpire
{
    class DataManagement
    {
        // Job Levels
        public int job1Level;
        public int job2Level;
        public int job3Level;
        public int job4Level;

        // Resources
        public int browniePoints;
        public int charismaPoints;
        public int charismaPointsInBank;

        public void SaveGame(int job1, int job2, int job3, int job4, int browniePoints, int charismaPoints, int charismaBank)
        {
            
        }

        public void LoadGame()
        {

        }
    }
}
