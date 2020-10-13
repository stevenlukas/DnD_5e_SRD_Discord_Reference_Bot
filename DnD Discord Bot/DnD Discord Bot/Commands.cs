using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Discord.Commands;
using DnD_Discord_Bot.DnD_Discord_Bot;
using Newtonsoft.Json;
using Google.Apis.Util;

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
                await ReplyAsync($"You can reference any Proficiency in the DnD 5e SRD. {_dnd5eURL}/api/proficiencies");
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
                await ReplyAsync("You can lookup any condition listed in the DnD 5e SRD. You can view the condition list at dnd5eapi.co/api/conditions");
            }
            else
            {
                conditionLookup = $"{_dnd5eURL}/api/conditions";
                conditionLookup = await _dndClient.GetStringAsync(conditionLookup.ToLower());
                if(conditionLookup.ToLower().Contains(condition.ToLower()))
                {
                    conditionLookup = $"{_dnd5eURL}/api/conditions/{condition}".ToLower();
                    conditionLookup = await _dndClient.GetStringAsync(conditionLookup);
                    ConditionRoot conditionObject = JsonConvert.DeserializeObject<ConditionRoot>(conditionLookup);

                    string conditionHeader = $"Condition: {conditionObject.name}\n";
                    foreach(string desc in conditionObject.desc)
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
            if(damage == null)
            {
                await ReplyAsync($"You can reference any dmaage type in the DnD 5e SRD. You can view the list at dnd5eapi.co/api/damage-types");
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
                    foreach(string desc in damageType.desc)
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
        public async Task EquipmentLookup([Remainder]string equipment = null)
        {
            string equipmentLookup = null;
            if(equipment == null)
            {
                await ReplyAsync("You can reference any item of equipment in the DnD 5e SRD");
            }
            else
            {
                equipmentLookup = $"{_dnd5eURL}/api/equipment";
                equipmentLookup = await _dndClient.GetStringAsync(equipmentLookup.ToLower());
                if(equipmentLookup.Contains(equipment.ToLower()))
                {
                    equipmentLookup = $"{_dnd5eURL}/api/equpment/{equipment.Replace(" ", "-")}".ToLower();
                    equipmentLookup = await _dndClient.GetStringAsync(equipmentLookup);
                    EquipmentRoot equipmentObject = JsonConvert.DeserializeObject<EquipmentRoot>(equipmentLookup);

                    string equipmentHeader = $"Name: {equipmentObject.name}\nEquipment Category: {equipmentObject.equipmentCategory.name}\nGear Type: {equipmentObject.gearCategory.name}\nCost: {equipmentObject.cost.quantity} {equipmentObject.cost.unit}\nWeight: {equipmentObject.weight} lb";
                    if(equipmentObject.desc != null)
                    {
                        foreach(string desc in equipmentObject.desc)
                        {
                            equipmentHeader += desc;
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

        }
        [Command("school")]
        public async Task MagicSchoolLookup([Remainder] string magicSchool = null)
        {

        }
        [Command("monster")]
        public async Task MonsterLookup([Remainder] string monster = null)
        {

        }
        [Command("race")]
        public async Task RaceLookup([Remainder] string race = null)
        {

        }
        [Command("subclass")]
        public async Task SubclassLookup([Remainder] string subclass = null)
        {

        }
        [Command("trait")]
        public async Task TraitLookup([Remainder] string trait = null)
        {

        }
        [Command("property")]
        public async Task PropertyLookup([Remainder] string weaponProperty = null)
        {

        }
    }
}
