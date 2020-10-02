using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Discord.Commands;
using DnD_Discord_Bot.DnD_Discord_Bot;
using Newtonsoft.Json;

namespace DnD_Discord_Bot.Modules
{

    public class Commands : ModuleBase<SocketCommandContext>
    {
        HttpClient _dndClient = new HttpClient();
        //private string searchItem = null;
        private string _dnd5eURL = $"https://www.dnd5eapi.co";

        [Command("test")]
        public async Task testMessage()
        {
            await ReplyAsync("HEY! I'm working here!");
        }
        [Command("roll")]
        public async Task Roll(string rollInput, string modifier = "normal")
        {
            string[] rollArray = rollInput.Split('d', '+', '-');

            int rollCount = int.Parse(rollArray[0]);
            int diceType = int.Parse(rollArray[1]);
            int abilityModifier = 0;
            int rollTotal = 0;
            int[] rollResult = new int[rollCount+1];
            string modifierType = null;
            string resultOutput = null;

            bool critFail = false;
            bool critSuccess = false;

            Random rnd = new Random();

            if(rollInput.Contains("-"))
            {
                modifierType = "-";
                abilityModifier = int.Parse(rollArray[2]);
            }
            else if(rollInput.Contains("+"))
            {
                modifierType = "+";
                abilityModifier = int.Parse(rollArray[2]);
            }

            //calculate roll
            for (int i = 0; i < rollCount; i++)
            {
                //sets random number value
                rollResult[i] = rnd.Next(1, (diceType + 1));
                
                //applies adv/dis reroll and sets initial final value
                if (modifier == "advantage" || modifier == "adv" && rollCount == 1 && diceType == 20)
                {
                    rollResult[1] = rnd.Next(1, (diceType + 1));
                    if(rollResult[1] > rollResult[0])
                    {
                        rollTotal = rollResult[1];
                    }
                    else
                    {
                        rollTotal = rollResult[0];
                    }

                    if (abilityModifier != 0)
                    {
                        resultOutput = $"[{rollResult[0]}, {modifierType}{abilityModifier}] [{rollResult[1]}, {modifierType}{abilityModifier}]";
                    }
                    else
                    {
                        resultOutput = $"[{rollResult[0]}] [{rollResult[1]}]";
                    }
                }
                else if (modifier == "disadvantage" || modifier == "dis" && rollCount == 1 && diceType == 20)
                {
                    rollResult[1] = rnd.Next(1, (diceType + 1));
                    if (rollResult[1] < rollResult[0])
                    {
                        rollTotal = rollResult[1];
                    }
                    else
                    {
                        rollTotal = rollResult[0];
                    }

                    if (abilityModifier != 0)
                    {
                        resultOutput = $"[{rollResult[0]}, {modifierType}{abilityModifier}] [{rollResult[1]}, {modifierType}{abilityModifier}]";
                    }
                    else
                    {
                        resultOutput = $"[{rollResult[0]}] [{rollResult[1]}]";
                    }
                }
                else
                {
                        resultOutput += $"[{rollResult[i]}]";
                        rollTotal += rollResult[i];
                }                
            }

            if (diceType == 20 && rollCount == 1)
            {
                if (rollResult[0] == 20 || rollResult[1] == 20 )
                {
                    critSuccess = true;
                }
                else if (rollResult[0] == 1 || rollResult[1] == 1)
                {
                    critFail = true;
                }
            }

            //check for and apply modifier
            if (rollArray.Length == 3)
            {
                if (modifierType == "+")
                {
                    rollTotal += abilityModifier;
                }
                else if (modifierType == "-")
                {
                    rollTotal -= abilityModifier;
                }
            }
            

            //output result [roll+mod]
            //return result
            if (critFail)
            {
                await ReplyAsync($"CRITICAL FAILURE, Natural 1");
            }
            else if (critSuccess)
            {
                await ReplyAsync($"CRITICAL SUCCESS, Natural 20");
            }
            else
            {
                if (modifier == "disadvantage" || modifier == "advantage" || modifier == "adv" || modifier == "dis")
                {
                    await ReplyAsync($"{rollTotal} {resultOutput}");
                }
                else if(abilityModifier != 0)
                {
                    await ReplyAsync($"{rollTotal} {resultOutput} {modifierType}{abilityModifier}");
                }
                else if(rollCount > 1)
                {
                    await ReplyAsync($"{rollTotal} {resultOutput}");
                }
                else
                {
                    await ReplyAsync($"{rollTotal}");
                }
            }
        }
        [Command("spell")]
        public async Task SpellLookup([Remainder] string spell = null)
        {
            string spellLookup;
            string[] apiCallResult;

            bool validSpell = false;

            if (spell == null)
            {
                await ReplyAsync($"This bot can reference all 319 spells listed in the DnD 5e SRD\nThe full list is viewable here: https://www.dnd5eapi.co/api/spells/");
            }
            else
            {
                //Section pulls the spell from the API if available
                spellLookup = $"{_dnd5eURL}/api/spells/";
                _dndClient.DefaultRequestHeaders.Add("User-Agent", "C# console program");

                spellLookup = await _dndClient.GetStringAsync(spellLookup);
                APIResults apiResult = JsonConvert.DeserializeObject<APIResults>(spellLookup);

                for (int i = 0; i < apiResult.results.Length; i++)
                {
                    if (String.Equals(spell, apiResult.results[i].name, StringComparison.OrdinalIgnoreCase))
                    {
                        spellLookup = _dnd5eURL + apiResult.results[i].url;
                        validSpell = true;
                        break;
                    }
                }

                if(!validSpell)
                {
                    await ReplyAsync($"The spell name provided, {spell}, was not found");
                }
                else
                {
                    //spell was found in the SRD, and now needs to be converted to a usable spellObject
                    spellLookup = await _dndClient.GetStringAsync($"{spellLookup}");

                    if (spellLookup.Contains("â€™")) ;
                    {
                        spellLookup = spellLookup.Replace("â€™", "'");
                    }

                    Spell spellObject = JsonConvert.DeserializeObject<Spell>(spellLookup);

                    //spell has been converted, and each portion is being prepped for display
                    string spellHeader = $"Name: {spellObject.name}\nLevel: {spellObject.level}\nSchool: {spellObject.school.name}";
                    
                    if(spellObject.ritual)
                    {
                        spellHeader +="\nThis spell can be cast as a ritual (R)";
                    }
                    if(spellObject.concentration)
                    {
                        spellHeader += "\nThis spell requires concentration (C)";
                    }

                    spellHeader += "\n~\n";

                    await ReplyAsync(spellHeader);
                    
                    foreach(string desc in spellObject.desc)
                    {
                        await ReplyAsync($"{desc}");
                    }
                    if(spellObject.higher_level != null)
                    {
                        string higherLevel = "~\nCasting with higher levels:\n";
                        foreach(string higherLevelDesc in spellObject.higher_level)
                        {
                            higherLevel += $"{higherLevelDesc}\n";
                        }
                        await ReplyAsync(higherLevel);
                    }

                    string additionalData = "~\nComponents: ";
                    foreach(string component in spellObject.components)
                    {
                        additionalData += $"{component} ";
                    }

                    additionalData += $"\nCasting Time: {spellObject.casting_time}";
                    additionalData += $"\nRange: {spellObject.range}";
                    additionalData += $"\nDuration: {spellObject.duration}";
                    await ReplyAsync(additionalData);
                }
            }
        }
        [Command("abilityscore")]
        public async Task AbilityScoreLookup(string abilityScore = null)
        {
            string abilityScoreLookup;
            string tomatoLogic = null;

            if (abilityScore == null)
            {
                Console.WriteLine("I'm Null");
                await ReplyAsync("Please choose one of the following: Charisma, Constitution, Dexterity, Intelligence, Strength, Wisdom");
            }
            else
            {
                abilityScore = abilityScore.ToLower();
                switch (abilityScore)
                {
                    case "int":
                        abilityScore = "INT";
                        tomatoLogic = "Intelligence is knowing a tomato is a fruit";
                        break;
                    case "intelligence":
                        abilityScore = "INT";
                        tomatoLogic = "Intelligence is knowing a tomato is a fruit";
                        break;
                    case "str":
                        abilityScore = "STR";
                        tomatoLogic = "Strength is being able to crush a tomato";
                        break;
                    case "strength":
                        abilityScore = "STR";
                        tomatoLogic = "Strength is being able to crush a tomato";
                        break;
                    case "dex":
                        abilityScore = "DEX";
                        tomatoLogic = "Dexterity is being able to dodge a tomato";
                        break;
                    case "dexterity":
                        abilityScore = "DEX";
                        tomatoLogic = "Dexterity is being able to dodge a tomato";
                        break;
                    case "con":
                        abilityScore = "CON";
                        tomatoLogic = "Constitution is being able to eat a bad tomato";
                        break;
                    case "constitution":
                        abilityScore = "CON";
                        tomatoLogic = "Constitution is being able to eat a bad tomato";
                        break;
                    case "wis":
                        abilityScore = "WIS";
                        tomatoLogic = "Wisdom is knowing not to put a tomato in a fruit salad";
                        break;
                    case "wisdom":
                        abilityScore = "WIS";
                        tomatoLogic = "Wisdom is knowing not to put a tomato in a fruit salad";
                        break;
                    case "cha":
                        abilityScore = "CHA";
                        tomatoLogic = "Charisma is being able to sell a tomato based fruit salad";
                        break;
                    case "charisma":
                        abilityScore = "CHA";
                        tomatoLogic = "Charisma is being able to sell a tomato based fruit salad";
                        break;
                    default:
                        break;
                }

                abilityScoreLookup = $"{_dnd5eURL}/api/ability-scores/";
                _dndClient.DefaultRequestHeaders.Add("User-Agent", "C# console program");

                abilityScoreLookup = await _dndClient.GetStringAsync($"{abilityScoreLookup}");
                APIResults apiResult = JsonConvert.DeserializeObject<APIResults>(abilityScoreLookup);

                for (int i = 0; i < apiResult.results.Length; i++)
                {
                    if (String.Equals(abilityScore, apiResult.results[i].name, StringComparison.OrdinalIgnoreCase))
                    {
                        abilityScoreLookup = _dnd5eURL + apiResult.results[i].url;
                        break;
                    }
                }

                abilityScoreLookup = await _dndClient.GetStringAsync($"{abilityScoreLookup}");

                if (abilityScoreLookup.Contains("â€™")) ;
                {
                    abilityScoreLookup = abilityScoreLookup.Replace("â€™", "'");
                }

                Ability abilityScoreObject = JsonConvert.DeserializeObject<Ability>(abilityScoreLookup);

                string abilityHeader = $"Name: {abilityScoreObject.full_name}\nAbbreviation: {abilityScoreObject.name}\n";
                string abilityDesc = "~\n";
                foreach(string desc in abilityScoreObject.desc)
                {
                    abilityDesc += $"{desc}\n";
                }
                string abilitySkill = "~\nSkills:\n";
                foreach(AbilitySkill skill in abilityScoreObject.skills)
                {
                    abilitySkill += $"{skill.name}\n";
                }

                await ReplyAsync(abilityHeader + abilityDesc + tomatoLogic + "\n" + abilitySkill);
            }
        }
        [Command("skill")]
        public async Task SkillLookup([Remainder] string skill = null)
        {
            string skillLookup;

            if (skill == null)
            {
                await ReplyAsync($"You can reference any skill listed in the DnD 5e SRD. A list can be found here {_dnd5eURL}/api/skills");
            }
            else
            {
                skill = skill.ToLower();
                skillLookup = $"{_dnd5eURL}/api/skills/";
                _dndClient.DefaultRequestHeaders.Add("User-Agent", "C# console program");

                skillLookup = await _dndClient.GetStringAsync($"{skillLookup}");
                APIResults apiResult = JsonConvert.DeserializeObject<APIResults>(skillLookup);

                for (int i = 0; i < apiResult.results.Length; i++)
                {
                    if (String.Equals(skill, apiResult.results[i].name, StringComparison.OrdinalIgnoreCase))
                    {
                        skillLookup = _dnd5eURL + apiResult.results[i].url;
                        break;
                    }
                }

                skillLookup = await _dndClient.GetStringAsync($"{skillLookup}");

                if (skillLookup.Contains("â€™")) ;
                {
                    skillLookup = skillLookup.Replace("â€™", "'");
                }

                Skill skillObject = JsonConvert.DeserializeObject<Skill>(skillLookup);

                string skillHeader = $"Name: {skillObject.name}\nAbility: {skillObject.ability_score.name}\n";
                string skillDesc = "~\n";
                foreach(string desc in skillObject.desc)
                {
                    skillDesc += $"{desc}\n";
                }

                await ReplyAsync(skillHeader + skillDesc);
            }
        }
        [Command("proficiency")]
        public async Task ProficiencyLookup([Remainder] string proficiency = null)
        {
            string proficiencyLookup;

            if (proficiency == null)
            {
                await ReplyAsync( $"You can reference any Proficiency in the DnD 5e SRD. {_dnd5eURL}/api/proficiencies");
            }
            else
            {
                proficiency = proficiency.ToLower();
                if(proficiency.Contains(' '))
                {
                    proficiency.Replace(' ', '-');
                }
                if(proficiency.Contains('\''))
                {
                    proficiency.Remove('\'');
                }
                proficiencyLookup = $"{_dnd5eURL}/api/proficiencies/";
                _dndClient.DefaultRequestHeaders.Add("User-Agent", "C# console program");

                proficiencyLookup = await _dndClient.GetStringAsync($"{proficiencyLookup}");
                APIResults apiResult = JsonConvert.DeserializeObject<APIResults>(proficiencyLookup);

                for (int i = 0; i < apiResult.results.Length; i++)
                {
                    if (String.Equals(proficiency, apiResult.results[i].index, StringComparison.OrdinalIgnoreCase))
                    {
                        proficiencyLookup = _dnd5eURL + apiResult.results[i].url;
                        break;
                    }
                }

                proficiencyLookup = await _dndClient.GetStringAsync($"{proficiencyLookup}");

                if (proficiencyLookup.Contains("â€™")) ;
                {
                    proficiencyLookup = proficiencyLookup.Replace("â€™", "'");
                }

                Proficiency proficiencyObject = JsonConvert.DeserializeObject<Proficiency>(proficiencyLookup);

                string proficiencyHeader = $"Name: {proficiencyObject.name}\nType: {proficiencyObject.type}";
            }
        }
        [Command("language")]
        public async Task LanguageLookup([Remainder] string language = null)
        {
            string languageLookup;

            if (language == null)
            {
                await ReplyAsync($"You can reference any language in the DnD 5e SRD. {_dnd5eURL}/api/languages");
            }
            else
            {
                language = language.ToLower();
                languageLookup = $"{_dnd5eURL}/api/languages";
                _dndClient.DefaultRequestHeaders.Add("User-Agent", "C# console program");

                languageLookup = await _dndClient.GetStringAsync($"{languageLookup}");
                APIResults apiResult = JsonConvert.DeserializeObject<APIResults>(languageLookup);

                for (int i = 0; i < apiResult.results.Length; i++)
                {
                    if (String.Equals(language, apiResult.results[i].name, StringComparison.OrdinalIgnoreCase))
                    {
                        language = _dnd5eURL + apiResult.results[i].url;
                        break;
                    }
                }
                languageLookup = await _dndClient.GetStringAsync($"{language}");

                if (languageLookup.Contains("â€™")) ;
                {
                    languageLookup = languageLookup.Replace("â€™", "'");
                }

                Language languageObject = JsonConvert.DeserializeObject<Language>(languageLookup);

                string languageHeader = $"Name: {languageObject.name}\nType: {languageObject.type}\nScript: {languageObject.script}\n";
                string languageDesc = $"~\n{languageObject.desc}\n";
                string languageSpeakers = "~\nTypical Speakers:\n";

                foreach (string speaker in languageObject.typical_speakers)
                {
                    languageSpeakers += $"{speaker}\n";
                }

                if(languageObject.desc != null)
                {
                    languageHeader += languageDesc;
                }

                if(languageObject.typical_speakers != null)
                {
                    languageHeader += languageSpeakers;
                }

                await ReplyAsync(languageHeader);
            }
        }
        [Command("class")]
        public async Task ClassLookup(string classes = null, [Remainder] string drillDown = null)
        {
            long choice = 0;
            string subClasses = null;
            string proficiencies = null;
            string weaponList = null;
            string gearList = null;
            string specialEquipmentList = null;
            List <string> proficiencyChoiceList = new List<string>();
            string proficiencyChoices = null;
            string savingThrows = null;
            string classLookup;
            string[] apiCallResult;

            if (classes == null)
            {
                await ReplyAsync($"You can reference any class in the DnD 5e SRD. {_dnd5eURL}/api/classes");
            }
            else 
            {
                classes = classes.ToLower();
                classLookup = $"{_dnd5eURL}/api/classes/";
                _dndClient.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                
                classLookup = await _dndClient.GetStringAsync($"{classLookup}");
                APIResults apiResult = JsonConvert.DeserializeObject<APIResults>(classLookup);

                for (int i = 0; i < apiResult.results.Length; i++)
                {
                    if (String.Equals(classes, apiResult.results[i].name, StringComparison.OrdinalIgnoreCase))
                    {
                        classLookup =_dnd5eURL + apiResult.results[i].url;
                        break;
                    }
                }

                Console.WriteLine("Class Located and Obtained");
                classLookup = await _dndClient.GetStringAsync($"{classLookup}");
                if (classLookup.Contains("â€™"));
                {
                    classLookup = classLookup.Replace("â€™", "'");
                }

                Class classObject = JsonConvert.DeserializeObject<Class>(classLookup);
                string classHeader = $"Name: {classObject.name}\nHit Die: {classObject.hit_die}\n";

                Console.WriteLine("Class Converted");
                //proficiencies
                string classProficiency = $"~\n";
                for(int i = 0; i < classObject.proficiency_choices.Count; i++)
                {
                    Console.WriteLine("Iterating Proficiencies");
                    Console.WriteLine(classObject.proficiency_choices[i].choose);
                    classProficiency += $"Choose {classObject.proficiency_choices[i].choose} from:";
                    if(classObject.proficiency_choices[i].list != null)
                    {
                        for (int j = 0; j < classObject.proficiency_choices[i].list.Count; j++)
                        {
                            Console.WriteLine("Listing Choices");
                            Console.WriteLine(classObject.proficiency_choices[i].list[j].name);
                            classProficiency += $"\n{classObject.proficiency_choices[i].list[j].name}";
                        }
                    }
                }
                if(classObject.proficiencies != null)
                {
                    classProficiency += $"\n\nAdditional Proficiencies: ";
                    for(int i = 0; i < classObject.proficiencies.Count; i++)
                    {
                        classProficiency += $"\n{classObject.proficiencies[i].name}";
                    }
                }
                Console.WriteLine("Proficiencies listed");
                //saving throws
                string classSavingThrow = $"~\nSaving Throws:";
                for(int i = 0; i < classObject.saving_throws.Count; i++)
                {
                    classSavingThrow += $"\n{classObject.saving_throws[i].name}";
                }
                Console.WriteLine("Proficiencies Listed");
                //subclasses
                string classSubClass = $"~\nSubclasses:";
                for (int i = 0; i < classObject.subclasses.Count; i++)
                {
                    classSubClass += $"\n{classObject.subclasses[i].name}";
                }
                Console.WriteLine("Subclasses Listed");
                await ReplyAsync(classHeader + classSavingThrow + classProficiency + classSubClass);

                //starting equipment
                //levels
                //spellcasting
                //spells
            }
        }
        /*
        [Command("race")]
        public async Task RaceLookup([Remainder] string race = null)
        {
            string raceLookup;
            string raceTraits = null;
            string raceProficiencies = null;
            string raceSubraces = null;
            string[] apiCallResult;

            if (race == null)
            {
            }
            else
            {
                raceLookup = $"{_dnd5eURL}races/?name={race}";
                _dndClient.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                raceLookup = await _dndClient.GetStringAsync($"{raceLookup}");
                APIResults apiResult = JsonConvert.DeserializeObject<APIResults>(raceLookup);

                for (int i = 0; i < apiResult.results.Length; i++)
                {
                    if (String.Equals(race, apiResult.results[i].name, StringComparison.OrdinalIgnoreCase))
                    {
                        raceLookup = apiResult.results[i].url;
                        break;
                    }
                }
                raceLookup = await _dndClient.GetStringAsync($"{raceLookup}");

                if (raceLookup.Contains("â€™"))
                {
                    raceLookup = raceLookup.Replace("â€™", "'");
                }

                Race raceObject = JsonConvert.DeserializeObject<Race>(raceLookup);

                for (int i = 0; i < raceObject.startingProficiencies.Length; i++)
                {
                    raceProficiencies = raceProficiencies + "\n" + raceObject.startingProficiencies[i].name;
                }

                for (int i = 0; i < raceObject.traits.Length; i++)
                {
                    raceTraits = raceTraits + "\n" + raceObject.traits[i].name;
                }

                for (int i = 0; i < raceObject.subraces.Length; i++)
                {
                    raceSubraces = raceSubraces + "\n" + raceObject.subraces[i].name;
                }

                await ReplyAsync($"Race: {raceObject.name}\n~\nSpeed: {raceObject.speed}\n~\nAge: {raceObject.age}\n~\nAlignment: {raceObject.alignment}\n");
                await ReplyAsync($"­~\nSize: {raceObject.size}\n{raceObject.sizeDescription}\n~\nLanguages: {raceObject.languageDesc}\n");
                if (raceTraits != null)
                {
                    await ReplyAsync($"~\nTraits:{raceTraits}\n");
                }
                if (raceProficiencies != null)
                {
                    await ReplyAsync($"~\nProficiencies: {raceProficiencies}\n");
                }
                if (raceSubraces != null)
                {
                    await ReplyAsync($"~\nSubraces:{raceSubraces}");
                }
            }
        }
        */
        /*
        [Command("subclass")]
        public async Task SubclassLookup(string subclass = null, [Remainder]string drillDown = null)
        {
            string subclassLookup;
            string[] apiCallResult;
            string fileLocation = @"C:\Bots\Lists\DnD_Subclass_List.txt";

            if (subclass == null)
            {
                var subclassList = new List<string>();
                subclassLookup = $"{_dnd5eURL}subclasses";

                _dndClient.DefaultRequestHeaders.Add("User-Agent", "C# console program");

                subclassLookup = await _dndClient.GetStringAsync($"{subclassLookup}");
                //Console.WriteLine("Checkpoint");

                apiCallResult = subclassLookup.Split("name");
                //Console.WriteLine("Checkpoint");

                for (int i = 1; i != apiCallResult.Length; i++)
                {
                    subclassLookup = apiCallResult[i];
                    string[] nameSeparation = subclassLookup.Split('"');
                    subclassList.Add(nameSeparation[2]);
                }

                foreach (string languageName in subclassList)
                {
                    File.WriteAllLines(fileLocation, subclassList);
                }

                var embed = new EmbedBuilder();
                var embedList = embed.Build();

                //await ReplyAsync(embed: embedList);
                await Context.Channel.SendFileAsync(fileLocation);
            }
            else
            {
                if (drillDown == null)
                {
                    subclass = subclass.ToLower();
                    subclassLookup = $"{_dnd5eURL}subclasses";
                    _dndClient.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                        //Console.WriteLine(languageLookup);
                        
                    subclassLookup = await _dndClient.GetStringAsync($"{subclassLookup}");
                        //Console.WriteLine(subclassLookup);
                    APIResults apiResult = JsonConvert.DeserializeObject<APIResults>(subclassLookup);
                        //Console.WriteLine(languageLookup);
                        //Console.WriteLine(apiResult.results.Length);

                    for (int i = 0; i < apiResult.results.Length; i++)
                    {
                            //Console.WriteLine(apiResult.results[i].name);
                            
                        if (String.Equals(subclass, apiResult.results[i].name, StringComparison.OrdinalIgnoreCase))
                        {
                            subclass = apiResult.results[i].url;
                                //Console.WriteLine(subclass);
                            break;
                        }
                    }
                        //Console.WriteLine("Here?");
                        
                    subclass = await _dndClient.GetStringAsync($"{subclass}");
                    Subclass subclassObject = JsonConvert.DeserializeObject<Subclass>(subclass);
                        //Console.WriteLine(subclassObject.Class.name);
                        //Console.WriteLine(subclassObject.name);
                    await ReplyAsync($"{subclassObject.name}\n~\n");
                        //Console.WriteLine("No");
                    foreach (string description in subclassObject.desc)
                    {
                            //Console.WriteLine(description);
                        await ReplyAsync($"{description}");
                            
                    }
                    for (int i = 0; i < subclassObject.features.Length; i++)
                    {
                            //Console.WriteLine(subclassObject.features[i].name);
                            //await ReplyAsync($"{subclassObject.features[i].name}");
                            //Console.WriteLine(subclassObject.features[i].url);
                        subclassLookup = await _dndClient.GetStringAsync(subclassObject.features[i].url);
                        Feature subclassFeature = JsonConvert.DeserializeObject<Feature>(subclassLookup);
                            //Console.WriteLine(subclassFeature.name);
                        await ReplyAsync($"\n~\nSubclass Feature: {subclassFeature.name}");
                        //Console.WriteLine(subclassFeature.level);
                        await ReplyAsync($"\n\nObtained at Level {subclassFeature.level}\n\n");
                        foreach (string description in subclassFeature.desc)
                        {
                                //Console.WriteLine(description);
                            await ReplyAsync($"{description}");
                        }
                    }
                    //Druids likely have a different class makeup than other classes. Check what the results should look like for Clerics and Warlocks, then Druids.

                        //Console.WriteLine(subclassObject.index);

                        if (subclassObject.spells != null)
                    {
                        await ReplyAsync("\n~\nSubclass Spells\n\n");
                        Console.WriteLine(subclassObject.spells.Length);
                        for (int i = 0; i < subclassObject.spells.Length; i++)
                        {
                                //Console.WriteLine($"Spell acquired at Level {subclassObject.spells[i].levelAcquired}");
                            Console.WriteLine($"Spell Prerequisites: {subclassObject.spells[i].prerequisites.Length}");
                            //await ReplyAsync($"Spell Prerequisites: {subclassObject.spells[i].prerequisites.Length}");
                            for (int j = 0; j < subclassObject.spells[i].prerequisites.Length; j++)
                            {
                                Console.WriteLine(subclassObject.spells[i].prerequisites[j].name);
                                await ReplyAsync($"\n\n{subclassObject.spells[i].prerequisites[j].name}");
                            }
                            Console.WriteLine(subclassObject.spells[i].spell.name);
                            await ReplyAsync($"Spell: {subclassObject.spells[i].spell.name}");
                            Console.WriteLine(subclassObject.spells[i].spellAcquisitionMethod.name);
                            switch (subclassObject.spells[i].spellAcquisitionMethod.name)
                            {
                                case "level":
                                   await ReplyAsync($"Spell acquired at Level {subclassObject.spells[i].levelAcquired}");
                                    break;
                                default:
                                    Console.WriteLine("Case not set");
                                    break;
                            }
                            await ReplyAsync("\n");
                        }
                    }
                }
            }
        }*/
    }
}
