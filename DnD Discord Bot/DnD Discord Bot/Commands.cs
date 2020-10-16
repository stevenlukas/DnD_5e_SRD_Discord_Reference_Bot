using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Discord.Commands;
using DnD_Discord_Bot.DnD_Discord_Bot;
using DnD_Discord_Bot;
using Newtonsoft.Json;
using Google.Apis.Util;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;

namespace DnD_Discord_Bot.Modules
{

    public class Commands : ModuleBase<SocketCommandContext>
    {
        public BotUtilities botUtilities = new BotUtilities();
        HttpClient _dndClient = new HttpClient();
        //private string searchItem = null;
        private string _dnd5eURL = $"https://www.dnd5eapi.co";
        private string _supportURL = $"https://github.com/stevenlukas/DnD_5e_SRD_Discord_Reference_Bot/wiki";

        [Command("test")]
        public async Task testMessage()
        {
            await ReplyAsync("HEY! I'm working here!");
        }
        [Command("roll")]
        public async Task Roll(string rollInput = null, string modifier = "normal")
        {
            if (rollInput == null)
            {
                await ReplyAsync("You can use any dice imaginable, even a d1000. Use the format !roll <dice type> <optional modifier> <optional advantage disadvantage>. This looks like !roll 1d20 +6 advantage");
            }
            else
            {
                string[] rollArray = rollInput.Split('d', '+', '-');

                int rollCount = int.Parse(rollArray[0]);
                int diceType = int.Parse(rollArray[1]);
                int abilityModifier = 0;
                int rollTotal = 0;
                int[] rollResult = new int[rollCount + 1];
                string modifierType = null;
                string resultOutput = null;

                bool critFail = false;
                bool critSuccess = false;

                Random rnd = new Random();

                if (rollInput.Contains("-"))
                {
                    modifierType = "-";
                    abilityModifier = int.Parse(rollArray[2]);
                }
                else if (rollInput.Contains("+"))
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
                        if (rollResult[1] > rollResult[0])
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
                    if (rollResult[0] == 20 || rollResult[1] == 20)
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
                    else if (abilityModifier != 0)
                    {
                        await ReplyAsync($"{rollTotal} {resultOutput} {modifierType}{abilityModifier}");
                    }
                    else if (rollCount > 1)
                    {
                        await ReplyAsync($"{rollTotal} {resultOutput}");
                    }
                    else
                    {
                        await ReplyAsync($"{rollTotal}");
                    }
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
                await ReplyAsync($"This bot can reference all 319 spells listed in the DnD 5e SRD. Use the format !spell <spell name>, lik !spell fireball");
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

                if (!validSpell)
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

                    if (spellObject.ritual)
                    {
                        spellHeader += "\nThis spell can be cast as a ritual (R)";
                    }
                    if (spellObject.concentration)
                    {
                        spellHeader += "\nThis spell requires concentration (C)";
                    }

                    spellHeader += "\n\n";

                    await ReplyAsync(spellHeader);

                    foreach (string desc in spellObject.desc)
                    {
                        await ReplyAsync($"{desc}");
                    }
                    if (spellObject.higher_level != null)
                    {
                        string higherLevel = "\nCasting with higher levels:\n";
                        foreach (string higherLevelDesc in spellObject.higher_level)
                        {
                            higherLevel += $"{higherLevelDesc}\n";
                        }
                        await ReplyAsync(higherLevel);
                    }

                    string additionalData = "\nComponents: ";
                    foreach (string component in spellObject.components)
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
                await ReplyAsync("Please choose one of the following: Charisma, Constitution, Dexterity, Intelligence, Strength, Wisdom. Use the command format !abilityscore <ability>");
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
                string abilityDesc = "\n";
                foreach (string desc in abilityScoreObject.desc)
                {
                    abilityDesc += $"{desc}\n";
                }
                string abilitySkill = "\nSkills:\n";
                foreach (AbilitySkill skill in abilityScoreObject.skills)
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
                await ReplyAsync($"You can reference any skill listed in the DnD 5e SRD. Use the format !skill <skill name>");
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
                string skillDesc = "\n";
                foreach (string desc in skillObject.desc)
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
                await ReplyAsync($"You can reference any Proficiency in the DnD 5e SRD. Use the format !proficiency <proficiency name>");
            }
            else
            {
                proficiency = proficiency.ToLower();
                if (proficiency.Contains(' '))
                {
                    proficiency.Replace(' ', '-');
                }
                if (proficiency.Contains('\''))
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
                await ReplyAsync($"You can reference any language in the DnD 5e SRD. Use the format !language <language name>");
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
                string languageDesc = $"\n{languageObject.desc}\n";
                string languageSpeakers = "\nTypical Speakers:\n";

                foreach (string speaker in languageObject.typical_speakers)
                {
                    languageSpeakers += $"{speaker}\n";
                }

                if (languageObject.desc != null)
                {
                    languageHeader += languageDesc;
                }

                if (languageObject.typical_speakers != null)
                {
                    languageHeader += languageSpeakers;
                }

                await ReplyAsync(languageHeader);
            }
        }
        [Command("class")]
        public async Task ClassLookup(string classes = null, [Remainder] string drillDown = null)
        {
            string classLookup;

            if (classes == null)
            {
                await ReplyAsync($"You can reference any class in the DnD 5e SRD. Use the format !class <class name> <optional parameter>\nThe optional parameters are spells, level, starting equipment, spellcasting. For example, !class wizard spells");
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
                        classLookup = _dnd5eURL + apiResult.results[i].url;
                        break;
                    }
                }

                classLookup = await _dndClient.GetStringAsync($"{classLookup}");
                if (classLookup.Contains("â€™")) ;
                {
                    classLookup = classLookup.Replace("â€™", "'");
                }

                Class classObject = JsonConvert.DeserializeObject<Class>(classLookup);

                if (drillDown != null)
                {
                    string[] commandResults;
                    string classLevel = null;
                    if (drillDown.Contains("level"))
                    {
                        commandResults = drillDown.Split(" ");
                        drillDown = "level";
                        classLevel = commandResults[1];
                    }
                    switch (drillDown)
                    {
                        case "spells":
                            if (classObject.spellcasting != null)
                            {
                                classLookup = _dnd5eURL + classObject.spells;
                                classLookup = await _dndClient.GetStringAsync($"{classLookup}");
                                ClassSpells spellObject = JsonConvert.DeserializeObject<ClassSpells>(classLookup);

                                string spellResult = $"{classObject.name} Spells:";

                                for (int i = 0; i < spellObject.results.Count; i++)
                                {
                                    spellResult += $"\n{spellObject.results[i].name}";
                                }
                                await ReplyAsync(spellResult);
                            }
                            else
                            {
                                await ReplyAsync($"The {classObject.name} class does not know any spells by default");
                            }
                            break;

                        case "level":
                            string levelSpellcasting = null;
                            string levelFeatureChoice = null;
                            string levelFeatures = null;
                            string levelHeader = null;
                            string levelResults = null;
                            classLookup = $"{_dnd5eURL}/api/classes/{classObject.name}/levels/{classLevel}";
                            classLookup = classLookup.ToLower();
                            classLookup = await _dndClient.GetStringAsync($"{classLookup}");
                            Level level = JsonConvert.DeserializeObject<Level>(classLookup);

                            //Build general header
                            levelHeader = $"Class: {level.@class.name}\nLevel: {level.level}\nProficiency Bonus: {level.prof_bonus}";
                            if (level.ability_score_bonuses != 0)
                            {
                                levelHeader += $"\nAbility Score Bonuses: {level.ability_score_bonuses}";
                            }

                            //Build features
                            if (level.features.Count != 0)
                            {
                                levelFeatures = "\nFeatures: ";
                                for (int i = 0; i < level.features.Count; i++)
                                {
                                    levelFeatures += $"{level.features[i].name}";
                                    if (i != level.features.Count - 1)
                                    {
                                        levelFeatures += ", ";
                                    }
                                    else
                                    {
                                        levelFeatures += "\n";
                                    }
                                }
                            }

                            //Build spellcasting for casters
                            if (level.spellcasting != null)
                            {
                                levelSpellcasting += "\n";
                                if (level.spellcasting.cantrips_known != 0)
                                {
                                    levelSpellcasting += $"Cantrips Known: {level.spellcasting.cantrips_known}\n";
                                }
                                if (level.spellcasting.spells_known != 0)
                                {
                                    levelSpellcasting += $"Spells Known: {level.spellcasting.spells_known}\n";
                                }
                                if (level.spellcasting.spell_slots_level_1 != 0)
                                {
                                    levelSpellcasting += $"Spell Slots L1: {level.spellcasting.spell_slots_level_1}\n";
                                }
                                if (level.spellcasting.spell_slots_level_2 != 0)
                                {
                                    levelSpellcasting += $"Spell Slots L2: {level.spellcasting.spell_slots_level_2}\n";
                                }
                                if (level.spellcasting.spell_slots_level_3 != 0)
                                {
                                    levelSpellcasting += $"Spell Slots L3: {level.spellcasting.spell_slots_level_3}\n";
                                }
                                if (level.spellcasting.spell_slots_level_4 != 0)
                                {
                                    levelSpellcasting += $"Spell Slots L4: {level.spellcasting.spell_slots_level_4}\n";
                                }
                                if (level.spellcasting.spell_slots_level_5 != 0)
                                {
                                    levelSpellcasting += $"Spell Slots L5: {level.spellcasting.spell_slots_level_5}\n";
                                }
                                if (level.spellcasting.spell_slots_level_6 != 0)
                                {
                                    levelSpellcasting += $"Spell Slots L6: {level.spellcasting.spell_slots_level_6}\n";
                                }
                                if (level.spellcasting.spell_slots_level_7 != 0)
                                {
                                    levelSpellcasting += $"Spell Slots L7: {level.spellcasting.spell_slots_level_7}\n";
                                }
                                if (level.spellcasting.spell_slots_level_8 != 0)
                                {
                                    levelSpellcasting += $"Spell Slots L8: {level.spellcasting.spell_slots_level_8}\n";
                                }
                                if (level.spellcasting.spell_slots_level_9 != 0)
                                {
                                    levelSpellcasting += $"Spell Slots L9: {level.spellcasting.spell_slots_level_9}\n";
                                }
                            }
                            if (level.feature_choices != null)
                            {
                                for (int i = 0; i < level.feature_choices.Count; i++)
                                {
                                    levelFeatureChoice += $"{level.feature_choices[i].name}\n";
                                }
                            }
                            if (level.class_specific != null)
                            {
                                if (level.class_specific.invocations_known != 0)
                                {
                                    levelSpellcasting += $"\nInvocations Known: {level.class_specific.invocations_known}\n";
                                }
                                if (level.class_specific.mystic_arcanum_level_6 != 0)
                                {
                                    levelSpellcasting += $"Mystic Arcanum L6: {level.class_specific.mystic_arcanum_level_6}\n";
                                }
                                if (level.class_specific.mystic_arcanum_level_7 != 0)
                                {
                                    levelSpellcasting += $"Mystic Arcanum L7: {level.class_specific.mystic_arcanum_level_7}\n";
                                }
                                if (level.class_specific.mystic_arcanum_level_8 != 0)
                                {
                                    levelSpellcasting += $"Mystic Arcanum L8: {level.class_specific.mystic_arcanum_level_8}\n";
                                }
                                if (level.class_specific.mystic_arcanum_level_9 != 0)
                                {
                                    levelSpellcasting += $"Mystic Arcanum L9: {level.class_specific.mystic_arcanum_level_9}\n";
                                }
                                if (level.class_specific.rageCount != 0)
                                {
                                    levelSpellcasting += $"Rage Count: {level.class_specific.rageCount}\n";
                                }
                                if (level.class_specific.rageDamageBonus != 0)
                                {
                                    levelSpellcasting += $"Rage Damage Bonus: {level.class_specific.rageDamageBonus}\n";
                                }
                                if (level.class_specific.brutalCriticalDice != 0)
                                {
                                    levelSpellcasting += $"Brutal Critical Dice: {level.class_specific.brutalCriticalDice}\n";
                                }
                                if (level.class_specific.BardicInspirationDie != 0)
                                {
                                    levelSpellcasting += $"Bardic Inspiratin die: {level.class_specific.BardicInspirationDie}\n";
                                }
                                if (level.class_specific.SongOfRestDie != 0)
                                {
                                    levelSpellcasting += $"Song of Rest die: {level.class_specific.SongOfRestDie}\n";
                                }
                                if (level.class_specific.MagicalSecretsMax5 != 0)
                                {
                                    levelSpellcasting += $"Magical Secrets (max L5): {level.class_specific.MagicalSecretsMax5}\n";
                                }
                                if (level.class_specific.MagicalSecretsMax7 != 0)
                                {
                                    levelSpellcasting += $"Magical Secrets (max L7): {level.class_specific.MagicalSecretsMax7}\n";
                                }
                                if (level.class_specific.MagicalSecretsMax9 != 0)
                                {
                                    levelSpellcasting += $"Magical Secrets (max L9): {level.class_specific.MagicalSecretsMax9}\n";
                                }
                                if (level.class_specific.ChannelDivinityCharges != 0)
                                {
                                    levelSpellcasting += $"Channel Divinity Charges: {level.class_specific.ChannelDivinityCharges}\n";
                                }
                                if (level.class_specific.DestroyUndeadCr != 0)
                                {
                                    levelSpellcasting += $"Destroy Undead CR: {level.class_specific.DestroyUndeadCr}\n";
                                }
                                if (level.class_specific.WildShapeMaxCr != 0)
                                {
                                    levelSpellcasting += $"Wild Shape Max CR: {level.class_specific.WildShapeMaxCr}\n";
                                }
                                if (level.class_specific.WildShapeSwim)
                                {
                                    levelSpellcasting += $"Wild shape forms can swim\n";
                                }
                                if (level.class_specific.WildShpeFly)
                                {
                                    levelSpellcasting += $"Wild shape forms can fly\n";
                                }
                                if (level.class_specific.ActionSurges != 0)
                                {
                                    levelSpellcasting += $"Action Surges: {level.class_specific.ActionSurges}\n";
                                }
                                if (level.class_specific.IndomitableUses != 0)
                                {
                                    levelSpellcasting += $"Indomitable Uses: {level.class_specific.invocations_known}\n";
                                }
                                if (level.class_specific.ExtraAttacks != 0)
                                {
                                    levelSpellcasting += $"Extra Attacks: {level.class_specific.ExtraAttacks}\n";
                                }
                                if (level.class_specific.martialArts != null)
                                {
                                    levelSpellcasting += $"Martial Arts Dice Type: d{level.class_specific.martialArts.DiceValue}\nMartial Arts Dice Count: {level.class_specific.martialArts.DiceCount}\n";
                                }
                                if (level.class_specific.AuraRange != 0)
                                {
                                    levelSpellcasting += $"Aura Range: {level.class_specific.AuraRange}\n";
                                }
                                if (level.class_specific.FavoredEnemies != 0)
                                {
                                    levelSpellcasting += $"Number of Favored Enemies: {level.class_specific.FavoredEnemies}\n";
                                }
                                if (level.class_specific.FavoredTerrain != 0)
                                {
                                    levelSpellcasting += $"Number of Favored Terrains: {level.class_specific.FavoredTerrain}\n";
                                }
                                if (level.class_specific.SneakAttack != null)
                                {
                                    levelSpellcasting += $"Sneak Attack Dice Type: d{level.class_specific.SneakAttack.DiceValue}\nSneak Attack Dice Count: {level.class_specific.SneakAttack.DiceCount}\n";
                                }
                                if (level.class_specific.SorceryPoints != 0)
                                {
                                    levelSpellcasting += $"Sorcery Points: {level.class_specific.SorceryPoints}\n";
                                }
                                if (level.class_specific.MetamagicKnown != 0)
                                {
                                    levelSpellcasting += $"Metamagic Known: {level.class_specific.MetamagicKnown}\n";
                                }
                                if (level.class_specific.CreatingSpellSlots != null)
                                {
                                    for (int i = 0; i < level.class_specific.CreatingSpellSlots.Length; i++)
                                    {
                                        levelSpellcasting += $"Created Spell Slot Level: {level.class_specific.CreatingSpellSlots[i].SpellSlotLevel}\nSorcery Point Cost: {level.class_specific.CreatingSpellSlots[i].SorceryPointCost}\n";
                                    }
                                }
                                if (level.class_specific.ArcaneRecoveryLevels != 0)
                                {
                                    levelSpellcasting += $"Arcane Recovery Levels: {level.class_specific.ArcaneRecoveryLevels}\n";
                                }
                            }

                            levelResults = levelHeader;
                            if (levelFeatures != null)
                            {
                                levelResults += levelFeatures;
                            }
                            if (levelFeatureChoice != null)
                            {
                                levelResults += levelFeatureChoice;
                            }
                            if (levelSpellcasting != null)
                            {
                                levelResults += levelSpellcasting;
                            }

                            await ReplyAsync(levelResults);
                            break;

                        case "starting equipment":
                            //found that dnd5eapi.co has an invalid JSON for starting equipment. If the equipment has two parts, there is an invalid JSON array. Info must be input manually
                            string startingEquipmentResponse = null;
                            switch (classObject.name.ToLower())
                            {
                                case "barbarian":
                                    startingEquipmentResponse += $"Class: Barbarian\nStarting Equipment: Explorer's Pack (x1), Javelin (x4)\nChoose 1: Great Axe (x1), Any Martial Melee Weapon (x1)\nChoose 1: Handaxe (x2), Any Simple Weapon (x1)";
                                    await ReplyAsync(startingEquipmentResponse);
                                    break;

                                case "bard":
                                    startingEquipmentResponse += $"Class: Bard\nStarting Equipment: Leather (x1), Dagger (x1)\nChoose 1: Rapier (x1), Longsword (x1), Any Simple Weapon (x1)\nChoose 1: Diplomat's Pack (x1), Entertainer's Pack (X1)\nChoose 1: Lute (x1), Any Musical Instrument (x1)";
                                    await ReplyAsync(startingEquipmentResponse);
                                    break;

                                case "cleric":
                                    startingEquipmentResponse += $"Class: Cleric\nStarting Equipment: Shield (x1)\nChoose 1: Mace (x1), Warhammer (x1)\nChoose 1: Scale Mail (x1), Leather (x1), Chain Mail (x1)\nChoose 1: Light Crossbow (x1) Crossbow Bolt (x20), Any Simple Weapon (x1)\nChoose 1: Priest's Pack (x1), Explorer's Pack (x1)\nChoose 1 Holy Symbol";
                                    await ReplyAsync(startingEquipmentResponse);
                                    break;

                                case "druid":
                                    startingEquipmentResponse += $"Class: Druid\nStarting Equipment: Leather (x1), Explorer's Pack (x1)\nChoose 1: Shield (x1), Any Simple Weapon (x1)\nChoose 1: Scimitar (x1), Any Simple Weapon (x1)\nChoose 1 Druidic Focus";
                                    await ReplyAsync(startingEquipmentResponse);
                                    break;

                                case "fighter":
                                    startingEquipmentResponse += $"Class: Fighter\nChoose 1: Chain Mail (x1), Leather (x1) Longbow (x1) and Arrows (x20)\nChoose 1: Shield (x1) and any Martial Weapon (x1), any 2 Martial Weapons\nChoose 1: Handaxe (x2), Light Crossbow (x1) and crossbow bolt(x20)\nChoose 1: Dungeoneer's Pack (x1), Explorer's Pack (x1)";
                                    await ReplyAsync(startingEquipmentResponse);
                                    break;

                                case "monk":
                                    startingEquipmentResponse += $"Class: Monk\nStarting Equipment: Dart (x10)\nChoose 1: Shortsword (x1), any Simple Weapon (x1)\nChoose 1: Dungeoneer's Pack (x1), Explorer's Pack (x1)";
                                    await ReplyAsync(startingEquipmentResponse);
                                    break;

                                case "paladin":
                                    startingEquipmentResponse += $"Class: Paladin\nStarting Equipment: Chain Mail (x1)\nChoose 1: Shield (x1) and any Martial Weapon (x1), any 2 Martial Weapons\nChoose 1: Javelin (x5), any Simple Weapon (x1)\nChoose 1: Priest's Pack (x1), Explorer's Pack (x1)\nChoose 1 Holy Symbol";
                                    await ReplyAsync(startingEquipmentResponse);
                                    break;

                                case "ranger":
                                    startingEquipmentResponse += $"Class: Ranger\nStarting Equipment: Longbow (x1) and Arrows (x20)\nChoose 1: Scale Mail (x1), Leather (x1)\nChoose 1: Shortsword (x2), any 2 Simple Weapons\nChoose 1: Dungeoneer's Pack (x1), Explorer's Pack (x1)";
                                    await ReplyAsync(startingEquipmentResponse);
                                    break;

                                case "rogue":
                                    startingEquipmentResponse += $"Class: Rogue\nStarting Equipment: Leather (x1), Dagger (x2), Thieve's Tools (x1)\nChoose 1: Rapier (x1), Shortsword (x1)\nChoose 1: Shortsword (x1), Shortbow (x1) and Arrows (x20)\nChoose 1: Burglar's Pack (x1), Dungeoneer's Pack (x1), Explorer's Pack (x1)";
                                    await ReplyAsync(startingEquipmentResponse);
                                    break;

                                case "sorcerer":
                                    startingEquipmentResponse += $"Class: Sorcerer\nStarting Equipment: Dagger (x2)\nChoose 1: Light Crossbow (x1) and Bolts (x20), any 1 Simple Weapon\nChoose 1: Component Pouch (x1), Arcane Focus (x1)\nChoose 1: Dungeoneer's Pack (x1), Explorer's Pack (x1)";
                                    await ReplyAsync(startingEquipmentResponse);
                                    break;

                                case "warlock":
                                    startingEquipmentResponse += $"Class: Warlock\nStarting Equipment: Dagger (x2), Leather (x1)\nChoose 1: Light Crossbow (x1) and Bolts (x20), any 1 Simple Weapon\nChoose 1: Component Pouch (x1), Arcane Focus (x1)\nChoose 1: Scholar's Pack (x1), Dungeoneer's Pack (x1)\nChoose any 1 Simple Weapon";
                                    await ReplyAsync(startingEquipmentResponse);
                                    break;

                                case "wizard":
                                    startingEquipmentResponse += $"Class: Wizard\nStarting Equipment: Spellbook (x1)\nChoose 1: Dagger (x1), Quarterstaff (x1)\nChoose 1: Component Pouch (x1), Arcane Focus (x1)\nChoose 1: Scholar's Pack (x1), Dungeoneer's Pack (x1)";
                                    await ReplyAsync(startingEquipmentResponse);
                                    break;

                                default:
                                    await ReplyAsync("There is no starting equipment programmed for this class at this time");
                                    break;
                            }
                            break;

                        case "spellcasting":
                            string spellcastingLookup = _dnd5eURL + "/api/spellcasting";
                            spellcastingLookup = await _dndClient.GetStringAsync(spellcastingLookup);
                            if (spellcastingLookup.Contains(classObject.name) || spellcastingLookup.Contains(classObject.name.ToLower()))
                            {
                                spellcastingLookup = _dnd5eURL + "/api/spellcasting/" + classObject.name;
                                spellcastingLookup = spellcastingLookup.ToLower();
                                spellcastingLookup = await _dndClient.GetStringAsync(spellcastingLookup);
                                SpellcastingRoot spellcastingObject = JsonConvert.DeserializeObject<SpellcastingRoot>(spellcastingLookup);

                                string spellcastingHeader = $"Class: {spellcastingObject.spellcastingClass.name}\nSpellcasting available at level {spellcastingObject.level}\nSpellcasting Ability: {spellcastingObject.spellcastingAbility.name}\n";

                                for (int i = 0; i < spellcastingObject.spellcastingInfo.Count; i++)
                                {
                                    if (i == 0)
                                    {
                                        spellcastingHeader += $"{spellcastingObject.spellcastingInfo[i].name}: ";
                                        await ReplyAsync(spellcastingHeader);
                                    }
                                    foreach (string desc in spellcastingObject.spellcastingInfo[i].spellcastingDescription)
                                    {
                                        await ReplyAsync(desc);
                                    }
                                }
                            }
                            else
                            {
                                await ReplyAsync($"There is no listed spellcasting for the class {classObject.name}");
                            }

                            break;

                        default:
                            Console.WriteLine("Spaces may be a problem...");
                            break;
                    }
                }
                else
                {
                    string classHeader = $"Name: {classObject.name}\nHit Die: d{classObject.hit_die}\n";

                    //proficiencies
                    string classProficiency = $"\n\nProficiencies";
                    for (int i = 0; i < classObject.proficiency_choices.Count; i++)
                    {
                        classProficiency += $"\nChoose {classObject.proficiency_choices[i].choose} from:";
                        if (classObject.proficiency_choices[i].from != null)
                        {
                            for (int j = 0; j < classObject.proficiency_choices[i].from.Count; j++)
                            {
                                classProficiency += $"\n{classObject.proficiency_choices[i].from[j].name}";
                            }
                            classProficiency.Replace("Skill: ", "");
                            classProficiency += "\n";
                        }
                    }
                    if (classObject.proficiencies != null)
                    {
                        classProficiency += $"\nAdditional Proficiencies ";
                        for (int i = 0; i < classObject.proficiencies.Count; i++)
                        {
                            classProficiency += $"\n{classObject.proficiencies[i].name}";
                        }
                    }

                    //saving throws
                    string classSavingThrow = $"Saving Throws: ";
                    for (int i = 0; i < classObject.saving_throws.Count; i++)
                    {
                        classSavingThrow += $"{classObject.saving_throws[i].name} ";
                    }
                    //subclasses
                    string classSubClass = $"\n\nSubclasses:";
                    for (int i = 0; i < classObject.subclasses.Count; i++)
                    {
                        classSubClass += $"\n{classObject.subclasses[i].name}";
                    }
                    await ReplyAsync(classHeader + classSavingThrow + classProficiency + classSubClass);
                    await ReplyAsync($"For Starting Equipment, Levels, Spellcasting, or Spells, use !class {classObject.name} starting equipment, level {{level number}}, spellcasting, or spells");
                }
            }
        }
        [Command("condition")]
        public async Task ConditionLookup([Remainder] string condition = null)
        {
            string conditionLookup = null;
            if (condition == null)
            {
                await ReplyAsync("You can lookup any condition listed in the DnD 5e SRD. Use the format !condition <condition name>");
            }
            else
            {
                conditionLookup = $"{_dnd5eURL}/api/conditions";
                conditionLookup = await _dndClient.GetStringAsync(conditionLookup.ToLower());
                if (conditionLookup.ToLower().Contains(condition.ToLower()))
                {
                    conditionLookup = $"{_dnd5eURL}/api/conditions/{condition}".ToLower();
                    conditionLookup = await _dndClient.GetStringAsync(conditionLookup);
                    ConditionRoot conditionObject = JsonConvert.DeserializeObject<ConditionRoot>(conditionLookup);

                    string conditionHeader = $"Condition: {conditionObject.name}\n";
                    foreach (string desc in conditionObject.desc)
                    {
                        conditionHeader += $"{desc}\n";
                    }
                    conditionHeader = conditionHeader.Replace("- ", "");
                    await ReplyAsync(conditionHeader);
                }
                else
                {
                    await ReplyAsync($"{condition} is not found in the DnD 5e SRD");
                }
            }
        }
        [Command("damage")]
        public async Task DamageLookup([Remainder] string damage = null)
        {
            if (damage == null)
            {
                await ReplyAsync($"You can reference any dmaage type in the DnD 5e SRD. Use the format !damage <damage type name>");
            }
            else
            {
                string damageLookup = $"{_dnd5eURL}/api/damage-types";
                damageLookup = await _dndClient.GetStringAsync(damageLookup);
                if (damageLookup.Contains(damage.ToLower()))
                {
                    damageLookup = $"{_dnd5eURL}/api/damage-types/{damage}".ToLower();
                    damageLookup = await _dndClient.GetStringAsync(damageLookup);
                    DamageTypeRoot damageType = JsonConvert.DeserializeObject<DamageTypeRoot>(damageLookup);

                    string damageHeader = $"Damage Type: {damageType.name}\n";
                    foreach (string desc in damageType.desc)
                    {
                        damageHeader += $"{desc}\n";
                    }

                    damageHeader = damageHeader.Replace("- ", "");
                    ReplyAsync(damageHeader);
                }
                else
                {
                    await ReplyAsync($"{damage} is not found in the DnD 5e SRD");
                }
            }
        }
        [Command("equipment")]
        public async Task EquipmentLookup([Remainder] string equipment = null)
        {
            string equipmentLookup = null;
            if (equipment == null)
            {
                await ReplyAsync("You can reference any item of equipment in the DnD 5e SRD. Use the format !equipment <equipment name>");
            }
            else
            {
                equipment = equipment.Replace(" ", "-");
                equipment = equipment.Replace("'", "");
                Console.WriteLine(equipment);
                equipmentLookup = $"{_dnd5eURL}/api/equipment";
                equipmentLookup = await _dndClient.GetStringAsync(equipmentLookup.ToLower());
                if (equipmentLookup.Contains(equipment.ToLower()))
                {
                    equipmentLookup = $"{_dnd5eURL}/api/equipment/{equipment}";
                    Console.WriteLine(equipmentLookup);
                    equipmentLookup = await _dndClient.GetStringAsync(equipmentLookup);
                    EquipmentRoot equipmentObject = JsonConvert.DeserializeObject<EquipmentRoot>(equipmentLookup);

                    string equipmentHeader = $"Name: {equipmentObject.name}\nEquipment Category: {equipmentObject.equipmentCategory.name}\nGear Type: {equipmentObject.gearCategory.name}\nCost: {equipmentObject.cost.quantity} {equipmentObject.cost.unit}\nWeight: {equipmentObject.weight} lb\n";
                    if (equipmentObject.desc != null)
                    {
                        foreach (string desc in equipmentObject.desc)
                        {
                            equipmentHeader += desc + " ";
                        }
                    }
                    await ReplyAsync(equipmentHeader);
                }
                else
                {
                    await ReplyAsync($"{equipment} is not found in the DnD 5e SRD");
                }
            }
        }
        [Command("feature")]
        public async Task FeatureLookup([Remainder] string feature = null)
        {
            if (feature == null)
            {
                await ReplyAsync("You can look up any feature in the DnD 5e SRD. Use the format !feature <feature name>");
            }
            else
            {
                feature = feature.Replace(" ", "-");
                feature = feature.Replace("'", "");
                string featureLookup = $"{_dnd5eURL}/api/features".ToLower();
                featureLookup = await _dndClient.GetStringAsync(featureLookup);
                if (featureLookup.Contains(feature))
                {
                    featureLookup = $"{_dnd5eURL}/api/features/{feature}".ToLower();
                    featureLookup = await _dndClient.GetStringAsync(featureLookup);
                    FeatureRoot featureObject = JsonConvert.DeserializeObject<FeatureRoot>(featureLookup);

                    string featureHeader = $"Feature: {featureObject.name}\nClass: {featureObject.featureClass.name}\nFeature available at level {featureObject.level}\n";
                    if (featureObject.desc != null)
                    {
                        foreach (string desc in featureObject.desc)
                        {
                            featureHeader += desc + " ";
                        }
                    }

                    await ReplyAsync(featureHeader);
                }
                else
                {
                    await ReplyAsync($"{feature} is not found in the DnD 5e SRD");
                }

            }
        }
        [Command("school")]
        public async Task MagicSchoolLookup([Remainder] string magicSchool = null)
        {
            if (magicSchool == null)
            {
                await ReplyAsync("You can reference any school of magic listed in the DnD 5e SRD. Use the format !school <school name>");
            }
            else
            {
                magicSchool = magicSchool.Replace(" ", "-");
                magicSchool = magicSchool.Replace("'", "");
                string magicSchoolLookup = $"{_dnd5eURL}/api/magic-schools";
                magicSchoolLookup = await _dndClient.GetStringAsync(magicSchoolLookup);

                if (magicSchoolLookup.Contains(magicSchool))
                {
                    magicSchoolLookup = $"{_dnd5eURL}/api/magic-schools/{magicSchool}".ToLower();
                    magicSchoolLookup = await _dndClient.GetStringAsync(magicSchoolLookup);
                    MagicSchoolRoot magicSchoolObject = JsonConvert.DeserializeObject<MagicSchoolRoot>(magicSchoolLookup);

                    string magicSchoolHeader = $"Name: {magicSchoolObject.name}\n{magicSchoolObject.desc}";

                    await ReplyAsync(magicSchoolHeader);
                }
                else
                {
                    await ReplyAsync($"{magicSchool} is not found in the DnD 5e SRD");
                }
            }
        }
        [Command("monster")]
        public async Task MonsterLookup([Remainder] string monster = null)
        {
            if (monster == null)
            {
                await ReplyAsync("You can reference any Monster in the DnD 5e SRD. Use the format !monster <monster name>");
            }
            else
            {

                monster = monster.Replace(" ", "-");
                monster = monster.Replace("'", "");
                string monsterLookup = $"{_dnd5eURL}/api/monsters";
                monsterLookup = await _dndClient.GetStringAsync(monsterLookup);

                if (monsterLookup.Contains(monster.ToLower()))
                {
                    monsterLookup = $"{_dnd5eURL}/api/monsters/{monster}".ToLower();
                    monsterLookup = await _dndClient.GetStringAsync(monsterLookup);

                    MonsterRoot monsterObject = JsonConvert.DeserializeObject<MonsterRoot>(monsterLookup);
                    string strMod = botUtilities.CalculateModifiers(monsterObject.strength);
                    string dexMod = botUtilities.CalculateModifiers(monsterObject.dexterity);
                    string conMod = botUtilities.CalculateModifiers(monsterObject.constitution);
                    string intMod = botUtilities.CalculateModifiers(monsterObject.intelligence);
                    string wisMod = botUtilities.CalculateModifiers(monsterObject.wisdom);
                    string chaMod = botUtilities.CalculateModifiers(monsterObject.charisma);
                    string monsterHeader = $"Name: {monsterObject.name}\n{monsterObject.size} {monsterObject.type}, {monsterObject.alignment}\n";
                    monsterHeader += $"Armor Class: {monsterObject.armorClass}\nHit Points: {monsterObject.hitPoints} ({monsterObject.hitDice})\nSpeed: Walk {monsterObject.speed.walk} ";
                    if (monsterObject.speed.swim != null)
                    {
                        monsterHeader += $"Swim {monsterObject.speed.swim} ";
                    }
                    if (monsterObject.speed.fly != null)
                    {
                        monsterHeader += $"Fly {monsterObject.speed.fly} ";
                    }
                    monsterHeader += $"\nSTR: {monsterObject.strength} {strMod} | DEX: {monsterObject.dexterity} {dexMod} | CON: {monsterObject.constitution} {conMod} | INT: {monsterObject.intelligence} {intMod} | WIS: {monsterObject.wisdom} {wisMod} | CHA: {monsterObject.charisma} {chaMod}\n";
                    bool skill = false;
                    bool savingThrow = false;
                    string proficiency = null;
                    for (int i = 0; i < monsterObject.proficiencies.Count; i++)
                    {
                        if (monsterObject.proficiencies[i].proficiency.index.Contains("saving"))
                        {
                            if (!savingThrow)
                            {
                                savingThrow = true;
                                monsterHeader += $"Saving Throw: ";
                                proficiency = monsterObject.proficiencies[i].proficiency.name.Replace("Saving Throw: ", "");
                                monsterHeader += $"{proficiency} ";
                            }
                            else
                            {
                                proficiency = monsterObject.proficiencies[i].proficiency.name.Replace("Saving Throw: ", "");
                                monsterHeader += $"{proficiency} ";
                            }
                        }
                        else if (monsterObject.proficiencies[i].proficiency.index.Contains("skill"))
                        {
                            if (!skill)
                            {
                                skill = true;
                                monsterHeader += $"\nSkill: ";
                                proficiency = monsterObject.proficiencies[i].proficiency.name.Replace("Skill: ", "");
                                monsterHeader += $"{proficiency} ";
                            }
                            else
                            {
                                proficiency = monsterObject.proficiencies[i].proficiency.name.Replace("Skill: ", "");
                                monsterHeader += $"{proficiency} ";
                            }
                        }
                    }
                    //TODO ADD Vulnerabilities, Resistances, Immunities
                    if (monsterObject.damageVulnerabilities != null)
                    {
                        monsterHeader += $"Damage Vulnerabilities: ";
                        foreach (string vulnerability in monsterObject.damageVulnerabilities)
                        {
                            monsterHeader += $"{vulnerability} ";
                        }
                        monsterHeader += "\n";
                    }
                    if (monsterObject.damageResistances != null)
                    {
                        monsterHeader += $"Damage Resistances: ";
                        foreach (string resistance in monsterObject.damageResistances)
                        {
                            monsterHeader += $"{resistance} ";
                        }
                        monsterHeader += "\n";
                    }
                    if (monsterObject.damageImmunities != null)
                    {
                        monsterHeader += $"Damage Immunities: ";
                        foreach (string immunity in monsterObject.damageImmunities)
                        {
                            monsterHeader += $"{immunity} ";
                        }
                        monsterHeader += "\n";
                    }
                    if (monsterObject.conditionImmunities != null)
                    {
                        monsterHeader += $"Condition Immunities: ";
                        foreach (MonsterConditionImmunities immunity in monsterObject.conditionImmunities)
                        {
                            monsterHeader += $"{immunity.name} ";
                        }
                        monsterHeader += "\n";
                    }

                    if (monsterObject.senses != null)
                    {
                        monsterHeader += $"\nSenses: ";
                        if (monsterObject.senses.darkvision != null)
                        {
                            monsterHeader += $"Dark Vision ({monsterObject.senses.darkvision}) ";
                        }
                        if (monsterObject.senses.passivePerception != null)
                        {
                            monsterHeader += $"Passive Perception ({monsterObject.senses.passivePerception}) ";
                        }
                        monsterHeader += $"\n";
                    }
                    if (monsterObject.languages != null)
                    {
                        monsterHeader += $"Languages: {monsterObject.languages}\n";
                    }
                    string crXP = botUtilities.CalculateXP(monsterObject.challengeRating);
                    monsterHeader += $"Challenge Rating {monsterObject.challengeRating} ({crXP} XP)\n";

                    string monsterSpecialAbilities = null;
                    if (monsterObject.specialAbilities != null)
                    {
                        monsterSpecialAbilities = "Special Abilities\n";
                        foreach (MonsterSpecialAbility ability in monsterObject.specialAbilities)
                        {
                            monsterSpecialAbilities += $"Name: {ability.name}\n{ability.desc}\n";
                        }
                    }
                    string monsterActions = null;
                    if (monsterObject.actions != null)
                    {
                        monsterActions = "Actions\n";
                        foreach (MonsterAction action in monsterObject.actions)
                        {
                            monsterActions += $"Name: {action.name}\n{action.desc}\n";
                            if (action.usage != null)
                            {
                                monsterActions += $"Uses: {action.usage.times} {action.usage.type}\n";
                            }
                        }
                    }
                    string monsterLegendaryActions = null;
                    if (monsterObject.legendaryActions != null)
                    {
                        monsterLegendaryActions = "Legendary Actions\n";
                        foreach (MonsterLegendaryAction legendaryAction in monsterObject.legendaryActions)
                        {
                            monsterLegendaryActions += $"Name: {legendaryAction.name}\n{legendaryAction.desc}\n";
                        }
                    }

                    await ReplyAsync(monsterHeader);
                    if (monsterSpecialAbilities != null)
                    {
                        await ReplyAsync(monsterSpecialAbilities);
                    }
                    if (monsterActions != null)
                    {
                        await ReplyAsync(monsterActions);
                    }
                    if (monsterLegendaryActions != null)
                    {
                        await ReplyAsync(monsterLegendaryActions);
                    }
                }
                else
                {
                    await ReplyAsync($"{monster} is not found in the DnD 5e SRD");
                }
            }
        }
        [Command("race")]
        public async Task RaceLookup([Remainder] string race = null)
        {
            if (race != null)
            {
                string raceLookup = $"{_dnd5eURL}/api/races";
                raceLookup = await _dndClient.GetStringAsync(raceLookup);
                race = race.Replace(" ", "-");
                race = race.Replace("'", "");
                if (raceLookup.Contains(race.ToLower()))
                {
                    raceLookup = $"{_dnd5eURL}/api/races/{race}";
                    raceLookup = await _dndClient.GetStringAsync(raceLookup);
                    RaceRoot raceObject = JsonConvert.DeserializeObject<RaceRoot>(raceLookup);

                    string raceHeader = $"Name: {raceObject.name}\nSpeed: {raceObject.speed} ft\nAbility Bonuses: ";
                    foreach (RaceAbilityBonus abilityBonus in raceObject.abilityBonuses)
                    {
                        raceHeader += $"{abilityBonus.name} +{abilityBonus.bonus} ";
                    }
                    raceHeader += $"\nAlignment: {raceObject.alignment}\nAge: {raceObject.age}\nSize: {raceObject.size}\n{raceObject.sizeDescription}\n";
                    string raceStartingProficiencies = null;
                    if (raceObject.startingProficiencies != null)
                    {
                        raceStartingProficiencies = $"Starting Proficiencies: ";
                        foreach (RaceStartingProficiencies startingProficiency in raceObject.startingProficiencies)
                        {
                            raceStartingProficiencies += $"{startingProficiency.name} ";
                        }
                    }
                    string raceLanguages = $"Language: {raceObject.languageDesc}\n";
                    string raceTraits = $"Traits: ";
                    foreach (RaceTraits trait in raceObject.traits)
                    {
                        raceTraits += $"{trait.name} ";
                    }
                    string raceSubrace = null;
                    if (raceObject.subraces != null)
                    {
                        raceSubrace = $"\nSubraces: ";
                        foreach (RaceSubraces subrace in raceObject.subraces)
                        {
                            raceSubrace += $"{subrace.name} ";
                        }
                    }

                    await ReplyAsync(raceHeader);
                    if (raceStartingProficiencies != null)
                    {
                        await ReplyAsync(raceStartingProficiencies);
                    }
                    await ReplyAsync(raceLanguages);
                    await ReplyAsync(raceTraits);
                    if (raceSubrace != null)
                    {
                        await ReplyAsync(raceSubrace);
                    }
                }
                else
                {
                    await ReplyAsync($"{race} is not found in the DnD 5e SRD");
                }
            }
            else
            {
                await ReplyAsync("You can reference any Race in the DnD 5e SRD. Use the format !race <race name>");
            }
        }
        [Command("subclass")]
        public async Task SubclassLookup([Remainder]string subclass = null)
        {
            if(subclass != null)
            {
                string subclassLookup = null;
                subclassLookup = $"{_dnd5eURL}/api/subclasses";
                subclassLookup = await _dndClient.GetStringAsync(subclassLookup);
                subclass = subclass.Replace(" ", "-");
                subclass = subclass.Replace("'", "");
                if (subclassLookup.Contains(subclass.ToLower()))
                {
                    subclassLookup = $"{_dnd5eURL}/api/subclasses/{subclass}".ToLower();
                    subclassLookup = await _dndClient.GetStringAsync(subclassLookup);
                    SubclassRoot subclassObject = JsonConvert.DeserializeObject<SubclassRoot>(subclassLookup);

                    string subclassHeader = $"Name: {subclassObject.name}\nClass: {subclassObject.subclassClass.name}\nSubclass Flavor: {subclassObject.flavor}\n";
                    foreach (string desc in subclassObject.desc)
                    {
                        subclassHeader += $"{desc} ";
                    }
                    switch(subclassObject.index)
                    {
                        case "thief":
                            subclassHeader += "\nFeatures at Level 3: Fast Hands, Second-Story Work\nFeatures at Level 9: Supreme Sneak\nFeatures at Level 13: Use Magic Device\nFeatures at Level 17: Thief's Reflexes";
                            break;

                        case "open-hand":
                            subclassHeader += "\nFeatures at level 3: Open Hand Technique\nFeatures at level 6: Wholeness of Body\nFeatures at level 11: Tranquility\nFeatures at level 17: Quivering Palm";
                            break;

                        case "lore":
                            subclassHeader += "\nFeatures at level 3: Bonus Proficiencies, Cutting Words\nFeatures at level 6: Additional Magical Secrets (max 3)\nFeatures at level 14: Peerless Skill";
                            break;

                        case "life":
                            subclassHeader += "\nFeatures at level 1: Disciple of Life\nFeatures at level 2: Channel Divinity (Preserve Life)\nFeatures at level 6: Blessed Healer\nFeatures at level 8: Divine Strike\nFeatures at level 17: Supreme Healing";
                            break;

                        case "land":
                            subclassHeader += "\nFeatures at level 2: Bonus Cantrip\nFeatures at level 6: Land's Stride\nFeatures at level 10: Nature's Ward\nFeatures at level 14: Nature's Sanctuary";
                            break;

                        case "hunter":
                            subclassHeader += "\nFeatures at level 3: Choose Hunter's Prey\nFeatures at level 7: Choose Defensive Tactics\nFeatures at level 11: Choose Multi-Attack\nFeatures at level 15: Choose Superior Hunter's Defense";
                            break;

                        case "fiend":
                            subclassHeader += "\nFeatures at level 1: Dark One's Blessing\nFeatures at level 6: Dark One's Own Luck\nFeatures at level 10: Fiendish Resilience\nFeatures at level 14: Hurl Through Hell";
                            break;

                        case "evocation":
                            subclassHeader += "\nFeatures at level 2: Evocation Savant, Sculpt Spells/nFeatures at level 6: Potent Cantrip\nFeatures at level 10: Empowered Evocation\nFeatures at level 14: Overchannel";
                            break;

                        case "draconic":
                            subclassHeader += "\nFeatures at level 1: Choose Dragon Ancestor, Draconic Resilience\nFeatures at level 6: Elemental Affinity\nFeatures at level 14: Dragon Wings\nFeatures at level 18: Draconic Presence";
                            break;

                        case "devotion":
                            subclassHeader += "\nFeatures at level 3: Channel Divinity\nFeatures at level 7: Aura of Devotion (10ft)\nFeatures at level 15: Purity of Spirit\nFeatures at level 18: Aura Range 30ft\nFeatures at level 20: Holy Nimbus";
                            break;

                        case "champion":
                            subclassHeader += "\nFeatures at level 3: Improved Critical\nFeatures at level 7: Remarkable Athlete\nFeatures at level 10: Choose Additional Fighting Style\nFeatures at level 15: Superior Critical\nFeatures at level 18: Survivor";
                            break;

                        case "berserker":
                            subclassHeader += "\nFeatures at level 3: Frenzy\nFeatures at level 6: Mindless Rage\nFeatures at level 10: Intimidating Presence\nFeatures at level 14: Retaliation";
                            break;

                        default:
                            break;
                    }
                    await ReplyAsync(subclassHeader);
                }
                else
                {
                    await ReplyAsync($"{subclass} is not found int the DnD 5e SRD");
                }
            }
            else
            {
                await ReplyAsync("You can reference any Subclass in the DnD 5e SRD. Use the format !subclass <subclass name>");
            }
        }
        [Command("trait")]
        public async Task TraitLookup([Remainder] string trait = null)
        {
            if(trait != null)
            {
                string traitLookup = $"{_dnd5eURL}/api/traits";
                traitLookup = await _dndClient.GetStringAsync(traitLookup);
                trait = trait.Replace(" ", "-");
                trait = trait.Replace("'", "");
                if(traitLookup.Contains(trait.ToLower()))
                {
                    traitLookup = $"{_dnd5eURL}/api/traits/{trait}".ToLower();
                    traitLookup = await _dndClient.GetStringAsync(traitLookup);
                    TraitRoot traitObject = JsonConvert.DeserializeObject<TraitRoot>(traitLookup);

                    string stringHeader = $"Name: {traitObject.name}\n";
                    if(traitObject.races != null)
                    {
                        stringHeader += "Races: ";
                        foreach(TraitRaces race in traitObject.races)
                        {
                            stringHeader += $"{race.name} ";
                        }
                        stringHeader += "\n";
                    }
                    if(traitObject.proficiencies != null)
                    {
                        stringHeader += "Proficiencies: ";
                        foreach(TraitProficiencies proficiency in traitObject.proficiencies)
                        {
                            stringHeader += $"{proficiency.name} ";
                        }
                        stringHeader += "\n";
                    }
                    foreach(string desc in traitObject.desc)
                    {
                        stringHeader += $"{desc} ";
                    }
                    await ReplyAsync(stringHeader);
                }
                else
                {
                    await ReplyAsync($"{trait} is not found in the DnD 5e SRD");
                }
            }
            else
            {
                await ReplyAsync("You can reference any trait in the DnD 5e SRD. Use the format !trait <trait name>");
            }
        }
        [Command("property")]
        public async Task PropertyLookup([Remainder] string weaponProperty = null)
        {
            if(weaponProperty != null)
            {
                weaponProperty = weaponProperty.Replace("'", "");
                weaponProperty = weaponProperty.Replace(" ", "-");
                string propertyLookup = $"{_dnd5eURL}/api/weapon-properties";
                propertyLookup = await _dndClient.GetStringAsync(propertyLookup);
                
                if(propertyLookup.Contains(weaponProperty.ToLower()))
                {
                    propertyLookup = $"{_dnd5eURL}/api/weapon-properties/{weaponProperty}".ToLower();
                    propertyLookup = await _dndClient.GetStringAsync(propertyLookup);
                    WeaponPropertyRoot propertyObject = JsonConvert.DeserializeObject<WeaponPropertyRoot>(propertyLookup);

                    string propertyHeader = $"Name: {propertyObject.name}\n";
                    foreach(string desc in propertyObject.desc)
                    {
                        propertyHeader += $"{desc} ";
                    }

                    await ReplyAsync(propertyHeader);
                }
                else
                {
                    await ReplyAsync($"{weaponProperty} is not found in the DnD 5e SRD");
                }
            }
            else
            {
                await ReplyAsync("You can reference any weapon property listed in the DnD 5e SRD. Use the format !property <weapon property name>");
            }
        }
        [Command("subrace")]
        public async Task SubraceLookup([Remainder] string subrace = null)
        {
            if(subrace != null)
            {
                string subraceLookup = $"{_dnd5eURL}/api/subraces";
                subraceLookup = await _dndClient.GetStringAsync(subraceLookup);
                subrace = subrace.Replace("'", "");
                subrace = subrace.Replace(" ", "-");
                if(subraceLookup.Contains(subrace.ToLower()))
                {
                    subraceLookup = $"{_dnd5eURL}/api/subraces/{subrace}".ToLower();
                    subraceLookup = await _dndClient.GetStringAsync(subraceLookup);
                    SubraceRoot subraceObject = JsonConvert.DeserializeObject<SubraceRoot>(subraceLookup);

                    string subraceHeader = $"Name: {subraceObject.name}\nAbility Bonuses: ";
                    foreach(SubraceAbilityBonuses abilityBonus in subraceObject.abilityBonuses)
                    {
                        subraceHeader += $"{abilityBonus.name} +{abilityBonus.bonus} ";
                    }
                    subraceHeader += "\nRacial Traits: ";
                    foreach(SubraceRacialTraits trait in subraceObject.racialTraits)
                    {
                        subraceHeader += $"{trait.name} ";
                    }
                    subraceHeader += "\n";
                    if(subraceObject.racialTraitOptions != null)
                    {
                        subraceHeader += $"Choose {subraceObject.racialTraitOptions.choose} additional trait(s): ";
                        foreach(SubraceRacialTraitOptionsFrom option in subraceObject.racialTraitOptions.from)
                        {
                            subraceHeader += $"{option.name} ";
                        }
                        subraceHeader += "\n";
                    }
                    if(subraceObject.startingProficiencies != null)
                    {
                        subraceHeader += $"Starting Proficiencies: ";
                        foreach(SubraceStartingProficiencies proficiency in subraceObject.startingProficiencies)
                        {
                            subraceHeader += $"{proficiency.name} ";
                        }
                        subraceHeader += "\n";
                    }
                    if(subraceObject.languageOptions != null)
                    {
                        subraceHeader += $"Choose {subraceObject.languageOptions.choose} additional language(s): ";
                        foreach(SubraceLanguageOptionsFrom option in subraceObject.languageOptions.from)
                        {
                            subraceHeader += $"{option.name} ";
                        }
                        subraceHeader += "\n";
                    }
                    subraceHeader += $"{subraceObject.desc} ";

                    await ReplyAsync(subraceHeader);
                }
                else
                {
                    await ReplyAsync($"{subrace} is not found in the DnD 5e SRD");
                }
            }
            else
            {
                await ReplyAsync("You can reference any Subrace in the DnD 5e SRD");
            }
        }
        [Command("help")]
        public async Task Help()
        {
            await ReplyAsync($"You can view the commands and their descriptions at {_supportURL}");
        }
    }
}
