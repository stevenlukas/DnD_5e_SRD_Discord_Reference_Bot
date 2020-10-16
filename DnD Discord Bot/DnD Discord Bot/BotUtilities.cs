using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace DnD_Discord_Bot.DnD_Discord_Bot
{
    public class BotUtilities
    {
        public string CalculateModifiers(int abilityScore)
        {
            string modifier = "(+0)";
            switch(abilityScore)
            {
                case 0:
                case 1:
                    modifier = "(-5)";
                    break;
                case 2:
                case 3:
                    modifier = "(-4)";
                    break;
                case 4:
                case 5:
                    modifier = "(-3)";
                    break;
                case 6:
                case 7:
                    modifier = "(-2)";
                    break;
                case 8:
                case 9:
                    modifier = "(-1)";
                    break;
                case 10:
                case 11:
                    break;
                case 12:
                case 13:
                    modifier = "(+1)";
                    break;
                case 14:
                case 15:
                    modifier = "(+2)";
                    break;
                case 16:
                case 17:
                    modifier = "(+3)";
                    break;
                case 18:
                case 19:
                    modifier = "(+4)";
                    break;
                case 20:
                case 21:
                    modifier = "(+5)";
                    break;
                case 22:
                case 23:
                    modifier = "(+6)";
                    break;
                case 24:
                case 25:
                    modifier = "(+7)";
                    break;
                case 26:
                case 27:
                    modifier = "(+8)";
                    break;
                case 28:
                case 29:
                    modifier = "(+9)";
                    break;
                case 30:
                    modifier = "(+10)";
                    break;
            }
            return modifier;
        }
        public string CalculateXP(double challengeRating)
        {
            string crXP = "0 or 10";
            switch (challengeRating)
            {
                case 0:
                    crXP = "0 or 10";
                    break;
                case 0.125:
                    crXP = "25";
                    break;
                case 0.25:
                    crXP = "50";
                    break;
                case 0.5:
                    crXP = "100";
                    break;
                case 1:
                    crXP = "200";
                    break;
                case 2:
                    crXP = "450";
                    break;
                case 3:
                    crXP = "700";
                    break;
                case 4:
                    crXP = "1100";
                    break;
                case 5:
                    crXP = "1800";
                    break;
                case 6:
                    crXP = "2300";
                    break;
                case 7:
                    crXP = "2900";
                    break;
                case 8:
                    crXP = "3900";
                    break;
                case 9:
                    crXP = "5000";
                    break;
                case 10:
                    crXP = "5900";
                    break;
                case 11:
                    crXP = "7200";
                    break;
                case 12:
                    crXP = "8400";
                    break;
                case 13:
                    crXP = "10000";
                    break;
                case 14:
                    crXP = "11500";
                    break;
                case 15:
                    crXP = "13000";
                    break;
                case 16:
                    crXP = "15000";
                    break;
                case 17:
                    crXP = "18000";
                    break;
                case 18:
                    crXP = "20000";
                    break;
                case 19:
                    crXP = "22000";
                    break;
                case 20:
                    crXP = "25000";
                    break;
                case 21:
                    crXP = "33000";
                    break;
                case 22:
                    crXP = "41000";
                    break;
                case 23:
                    crXP = "50000";
                    break;
                case 24:
                    crXP = "62000";
                    break;
                case 25:
                    crXP = "75000";
                    break;
                case 26:
                    crXP = "90000";
                    break;
                case 27:
                    crXP = "105000";
                    break;
                case 28:
                    crXP = "120000";
                    break;
                case 29:
                    crXP = "135000";
                    break;
                case 30:
                    crXP = "155000";
                    break;
            }
            return crXP;
        }
    }
}
