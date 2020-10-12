using System.Collections.Generic;
using System.Text.Json.Serialization;
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
        [JsonProperty("rage_count")]
        public long rageCount { get; set; }
        [JsonProperty("rage_damage_bonus")]
        public long rageDamageBonus { get; set; }
        [JsonProperty("brutal_critical_dice")]
        public long brutalCriticalDice { get; set; }
        [JsonProperty("bardic_inspiration_die")]
        public long BardicInspirationDie { get; set; }
        [JsonProperty("song_of_rest_die")]
        public long SongOfRestDie { get; set; }
        [JsonProperty("magical_secrets_max_5")]
        public long MagicalSecretsMax5 { get; set; }
        [JsonProperty("magical_secrets_max_7")]
        public long MagicalSecretsMax7 { get; set; }
        [JsonProperty("magical_secrets_max_9")]
        public long MagicalSecretsMax9 { get; set; }
        [JsonProperty("channel_divinity_charges")]
        public long ChannelDivinityCharges { get; set; }
        [JsonProperty("destroy_undead_cr")]
        public float DestroyUndeadCr { get; set; }
        [JsonProperty("wild_shape_max_cr")]
        public float WildShapeMaxCr { get; set; }
        [JsonProperty("wild_shape_swim")]
        public bool WildShapeSwim { get; set; }
        [JsonProperty("wild_shape_fly")]
        public bool WildShpeFly { get; set; }
        [JsonProperty("action_surges")]
        public int ActionSurges { get; set; }
        [JsonProperty("indomitable_uses")]
        public int IndomitableUses { get; set; }
        [JsonProperty("extra_attacks")]
        public int ExtraAttacks { get; set; }
        [JsonProperty("ki_points")]
        public int KiPoints { get; set; }
        [JsonProperty("unarmored_movement")]
        public int UnarmoredMovement { get; set; }
        [JsonProperty("martial_arts")]
        public LevelMartialArts martialArts { get; set; }
        [JsonProperty("aura_range")]
        public int AuraRange { get; set; }
        [JsonProperty("favored_enemies")]
        public int FavoredEnemies { get; set; }
        [JsonProperty("favored_terrain")]
        public int FavoredTerrain { get; set; }
        [JsonProperty("sneak_attack")]
        public LevelSneakAttack SneakAttack { get; set; }
        [JsonProperty("sorcery_points")]
        public long SorceryPoints { get; set; }
        [JsonProperty("metamagic_known")]
        public long MetamagicKnown { get; set; }
        [JsonProperty("creating_spell_slots")]
        public LevelCreatingSpellSlot[] CreatingSpellSlots { get; set; }
        [JsonProperty("arcane_recovery_levels")]
        public int ArcaneRecoveryLevels { get; set; }
    }
    public partial class LevelCreatingSpellSlot
    {
        [JsonProperty("spell_slot_level")]
        public long SpellSlotLevel { get; set; }

        [JsonProperty("sorcery_point_cost")]
        public long SorceryPointCost { get; set; }
    }
    public partial class LevelSneakAttack
    {
        [JsonProperty("dice_count")]
        public long DiceCount { get; set; }

        [JsonProperty("dice_value")]
        public long DiceValue { get; set; }
    }
    public partial class LevelMartialArts
    {
        [JsonProperty("dice_count")]
        public long DiceCount { get; set; }
        [JsonProperty("dice_value")]
        public long DiceValue { get; set; }
    }
    public class LevelClass
    {
        public string index { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }
    public class Level
    {
        public int level { get; set; }
        public int ability_score_bonuses { get; set; }
        public int prof_bonus { get; set; }
        public List<LevelFeatureChoice> feature_choices { get; set; }
        public List<LevelFeature> features { get; set; }
        public LevelSpellcasting spellcasting { get; set; }
        public LevelClassSpecific class_specific { get; set; }
        public string index { get; set; }
        public LevelClass @class { get; set; }
        public string url { get; set; }
    }

    //Used for Spellcasting API lookups
    public class SpellcastingRoot
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("class")]
        public SpellcastingClass spellcastingClass;
        [JsonProperty("level")]
        public int level;
        [JsonProperty("spellcasting_ability")]
        public SpellcastingAbility spellcastingAbility;
        [JsonProperty("info")]
        public List<SpellcastingInfo> spellcastingInfo;
        [JsonProperty("url")]
        public string url;
    }
    public class SpellcastingClass
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }
    public class SpellcastingAbility
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }
    public class SpellcastingInfo
    {
        [JsonProperty("name")]
        public string name;
        [JsonProperty("desc")]
        public string[] spellcastingDescription;
    }
}
