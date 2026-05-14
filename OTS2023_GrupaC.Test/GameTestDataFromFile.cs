using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTS2026_GrupaC.Test
{
    public class GameTestDataFromFile
    {
        public static IEnumerable Get_CalculateAchievement_OKInput_SuccessfulCalculation_TestData(string filename)
        {
            string path = $@"{AppDomain.CurrentDomain.BaseDirectory}\{filename}";
            string[] lines = File.ReadAllLines(path);
            List<TestCaseData> testCasesData = new List<TestCaseData>();
            foreach (string line in lines)
            {
                string[] values = line.Split(null);
                int amountOfHoneyJars = int.Parse(values[0]);
                int amountOfNectar = int.Parse(values[1]);
                bool beeCollected = values[2] == "yes";
                Achievement? expectedAchievement = GetAchievementFromString(values[3]);
                testCasesData.Add(new TestCaseData(amountOfHoneyJars, amountOfNectar, beeCollected, expectedAchievement));
            }
            return testCasesData;
        }

        private static Achievement? GetAchievementFromString(string achievement)
        {
            if (achievement.ToLower().Equals("poor")) return Achievement.Poor;
            if (achievement.ToLower().Equals("average")) return Achievement.Average;
            if (achievement.ToLower().Equals("good")) return Achievement.Good;
            return null;
        }
    }
}
