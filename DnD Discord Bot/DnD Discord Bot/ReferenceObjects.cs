using System.Collections.Generic;
using Newtonsoft.Json;

namespace DnD_Discord_Bot.DnD_Discord_Bot
{
    //Used for resolving general API Calls for JSON conversion
    public partial class APIResults
    {
        [JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)]
        public int count { get; set; }

        [JsonProperty("results", NullValueHandling = NullValueHandling.Ignore)]
        public Result[] results { get; set; }
        
    }
    public partial class Result
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string name { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string url { get; set; }
        [JsonProperty("index", NullValueHandling = NullValueHandling.Ignore)]
        public string index { get; set; }
    }

    //Used for resolving Spell API Calls

    public class SpellDamageType
    {
        public string index { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }
    public class SpellDamage
{
    public SpellDamageType damage_type { get; set; }
}
    public class SpellSchool
{
    public string index { get; set; }
    public string name { get; set; }
    public string url { get; set; }
}
    public class SpellClass
{
    public string index { get; set; }
    public string name { get; set; }
    public string url { get; set; }
}
    public class SpellSubclass
{
    public string index { get; set; }
    public string name { get; set; }
    public string url { get; set; }
}
    public partial class Spell
{
    public string index { get; set; }
    public string name { get; set; }
    public IList<string> desc { get; set; }
    public IList<string> higher_level { get; set; }
    public string range { get; set; }
    public IList<string> components { get; set; }
    public string material { get; set; }
    public bool ritual { get; set; }
    public string duration { get; set; }
    public bool concentration { get; set; }
    public string casting_time { get; set; }
    public int level { get; set; }
    public string attack_type { get; set; }
    public SpellDamage damage { get; set; }
    public SpellSchool school { get; set; }
    public IList<SpellClass> classes { get; set; }
    public IList<SpellSubclass> subclasses { get; set; }
    public string url { get; set; }
}

    //Used for resolving Ability Score API Calls

    public class AbilitySkill
    {
        public string name { get; set; }
        public string index { get; set; }
        public string url { get; set; }
    }
    public class Ability
    {
        public string index { get; set; }
        public string name { get; set; }
        public string full_name { get; set; }
        public List<string> desc { get; set; }
        public List<AbilitySkill> skills { get; set; }
        public string url { get; set; }
    }

    //Used for resolving Skill API Calls

    public class SkillAbility
    {
        public string index { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }
    public class Skill
    {
        public string index { get; set; }
        public string name { get; set; }
        public List<string> desc { get; set; }
        public SkillAbility ability_score { get; set; }
        public string url { get; set; }
    }

    //Used for resolving Proficiency API Calls
    public class ProficiencyReference
    {
        public string index { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }
    public class Proficiency
    {
        public string index { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public List<object> classes { get; set; }
        public List<object> races { get; set; }
        public string url { get; set; }
        public List<ProficiencyReference> references { get; set; }
    }

    //Used for resolving Language API Calls
    public class Language
    {
        public string index { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public string type { get; set; }
        public List<string> typical_speakers { get; set; }
        public string script { get; set; }
        public string url { get; set; }
    }

    //Used for resolving Class API Calls
    public class ClassFrom
    {
        public string index { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }
    public class ClassProficiencyChoice
    {
        public int choose { get; set; }
        public string type { get; set; }
        public List<ClassFrom> from { get; set; }
    }
    public class ClassProficiency
    {
        public string index { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }
    public class ClassSavingThrow
    {
        public string index { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }
    public class ClassSubclass
    {
        public string index { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }
    public class Class
    {
        public string index { get; set; }
        public string name { get; set; }
        public int hit_die { get; set; }
        public List<ClassProficiencyChoice> proficiency_choices { get; set; }
        public List<Proficiency> proficiencies { get; set; }
        public List<ClassSavingThrow> saving_throws { get; set; }
        public string starting_equipment { get; set; }
        public string class_levels { get; set; }
        public List<ClassSubclass> subclasses { get; set; }
        public string spellcasting { get; set; }
        public string spells { get; set; }
        public string url { get; set; }
    }

    //Used for resolving Class Spell List API Calls
    public class SpellListResult
    {
        public string index { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }
    public class ClassSpells
    {
        public int count { get; set; }
        public List<SpellListResult> results { get; set; }
    }

    //Used for resolving Class Level API Calls
    public class LevelFeatureChoice
    {
        public string index { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class LevelFeature
    {
        public string index { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class LevelSpellcasting
    {
        public int cantrips_known { get; set; }
        public int spells_known { get; set; }
        public int spell_slots_level_1 { get; set; }
        public int spell_slots_level_2 { get; set; }
        public int spell_slots_level_3 { get; set; }
        public int spell_slots_level_4 { get; set; }
        public int spell_slots_level_5 { get; set; }
        public int spell_slots_level_6 { get; set; }
        public int spell_slots_level_7 { get; set; }
        public int spell_slots_level_8 { get; set; }
        public int spell_slots_level_9 { get; set; }
    }

    public class LevelClassSpecific
    {
        public int invocations_known { get; set; }
        public int mystic_arcanum_level_6 { get; set; }
        public int mystic_arcanum_level_7 { get; set; }
        public int mystic_arcanum_level_8 { get; set; }
        public int mystic_arcanum_level_9 { get; set; }
    }

    public class LevelClass
    {
        public string index { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class LevelSubclass
    {
        public string index { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class LevelContents
    {
        public int level { get; set; }
        public int ability_score_bonuses { get; set; }
        public int prof_bonus { get; set; }
        public List<LevelFeatureChoice> feature_choices { get; set; }
        public List<LevelFeature> features { get; set; }
        public LevelSpellcasting spellcasting { get; set; }
        public LevelClassSpecific class_specific { get; set; }
        public string index { get; set; }
        public Class @class { get; set; }
        public string url { get; set; }
        public LevelSubclass subclass { get; set; }
    }

    public class Level
    {
        public List<LevelContents> levelContents { get; set; }
    }



    //Used for resolving Starting Equipment API Calls
    public class StartingEquipmentClass
    {
        public string index { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }
    public class StartingEquipmentEquipment
    {
        public string index { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }
    public class StartingEquipment
    {
        public StartingEquipmentEquipment equipment { get; set; }
        public int quantity { get; set; }
    }
    public class StartingEquipmentOption
    {
        public int choose { get; set; }
        public string type { get; set; }
        public object from { get; set; }
    }
    public class ClassStartingEquipment
    {
        public string index { get; set; }
        public Class @class { get; set; }
        public List<StartingEquipment> starting_equipment { get; set; }
        public List<StartingEquipmentOption> starting_equipment_options { get; set; }
        public string url { get; set; }
    }



}
