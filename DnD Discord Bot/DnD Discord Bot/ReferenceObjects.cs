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

    //Used for Condition API lookups
    public class ConditionRoot
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("desc")]
        public string[] desc;
        [JsonProperty("url")]
        public string url;
    }

    //Used for Damage Type API lookups
    public class DamageTypeRoot
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("desc")]
        public string[] desc;
        [JsonProperty("url")]
        public string url;
    }

    //Used for Equipment API lookups
    public class EquipmentRoot
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("equipment_category")]
        public EquipmentCategory equipmentCategory;
        [JsonProperty("gear_category")]
        public GearCategory gearCategory;
        [JsonProperty("cost")]
        public EquipmentCost cost;
        [JsonProperty("desc")]
        public string[] desc;
        [JsonProperty("weight")]
        public int weight;
        [JsonProperty("url")]
        public string url;
    }
    public class EquipmentCategory
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }
    public class GearCategory
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }
    public class EquipmentCost
    {
        [JsonProperty("quantity")]
        public int quantity;
        [JsonProperty("unit")]
        public string unit;
    }

    //Used for Feature API lookups
    public class FeatureRoot
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("class")]
        public FeatureClass featureClass;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("level")]
        public int level;
        [JsonProperty("desc")]
        public string[] desc;
        [JsonProperty("url")]
        public string url;
    }
    public class FeatureClass
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }

    //Used for Magic SChool API lookups
    public class MagicSchoolRoot
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("desc")]
        public string desc;
        [JsonProperty("url")]
        public string url;
    }

    //Used for Monster API lookups
    public class MonsterRoot
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("size")]
        public string size;
        [JsonProperty("type")]
        public string type;
        [JsonProperty("subtype")]
        public string subtype;
        [JsonProperty("alignment")]
        public string alignment;
        [JsonProperty("armor_class")]
        public int armorClass;
        [JsonProperty("hit_points")]
        public int hitPoints;
        [JsonProperty("hit_dice")]
        public string hitDice;
        [JsonProperty("speed")]
        public MonsterSpeed speed;
        [JsonProperty("strength")]
        public int strength;
        [JsonProperty("dexterity")]
        public int dexterity;
        [JsonProperty("constitution")]
        public int constitution;
        [JsonProperty("intelligence")]
        public int intelligence;
        [JsonProperty("wisdom")]
        public int wisdom;
        [JsonProperty("charisma")]
        public int charisma;
        [JsonProperty("proficiencies")]
        public List<MonsterProficiencies> proficiencies;
        [JsonProperty("damage_vulnerabilities")]
        public string[] damageVulnerabilities;
        [JsonProperty("damage_resistances")]
        public string[] damageResistances;
        [JsonProperty("damage_imunities")]
        public string[] damageImmunities;
        [JsonProperty("condition_immunities")]
        public MonsterConditionImmunities[] conditionImmunities;
        [JsonProperty("senses")]
        public MonsterSense senses;
        [JsonProperty("languages")]
        public string languages;
        [JsonProperty("challenge_rating")]
        public double challengeRating;
        [JsonProperty("special_abilities")]
        public MonsterSpecialAbility[] specialAbilities;
        [JsonProperty("actions")]
        public MonsterAction[] actions;
        [JsonProperty("legendary_actions")]
        public MonsterLegendaryAction[] legendaryActions;
        [JsonProperty("url")]
        public string url;
    }
    public class MonsterConditionImmunities
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }
    public class MonsterSpeed
    {
        [JsonProperty("walk")]
        public string walk;
        [JsonProperty("swim")]
        public string swim;
        [JsonProperty("fly")]
        public string fly;
    }
    public class MonsterProficiencies
    {
        [JsonProperty("value")]
        public int value;
        [JsonProperty("proficiency")]
        public MonsterProficienciesDetails proficiency;
    }
    public class MonsterProficienciesDetails
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }
    public class MonsterSense
    {
        [JsonProperty("darkvision")]
        public string darkvision;
        [JsonProperty("passive_perception")]
        public int passivePerception;
    }
    public class MonsterSpecialAbility
    {
        [JsonProperty("name")]
        public string name;
        [JsonProperty("desc")]
        public string desc;
        [JsonProperty("dc")]
        public MonsterSpecialAbilityDC dc;
    }
    public class MonsterSpecialAbilityDC
    {
        [JsonProperty("dc_type")]
        public MonsterSpecialAbilityDCType dcType;
        [JsonProperty("dc_value")]
        public int dcValue;
        [JsonProperty("success_type")]
        public string successType;
    }
    public class MonsterSpecialAbilityDCType
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }
    public class MonsterAction
    {
        [JsonProperty("name")]
        public string name;
        [JsonProperty("desc")]
        public string desc;
        [JsonProperty("attack_bonus")]
        public int attackBonus;
        [JsonProperty("dc")]
        public MonsterActionDC dc;
        [JsonProperty("damage")]
        public MonsterAttackDamage[] damage;
        [JsonProperty("usage")]
        public MonsterActionUsage usage;
    }
    public class MonsterActionDC
    {
        [JsonProperty("dc_type")]
        public MonsterActionDCType type;
        [JsonProperty("dc_value")]
        public int dcValue;
        [JsonProperty("success_type")]
        public string successType;
    }
    public class MonsterActionDCType
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }
    public class MonsterAttackDamage
    {
        [JsonProperty("damage_type")]
        public MonsterAttackDamageType type;
        [JsonProperty("damage_dice")]
        public string damageDice;
    }
    public class MonsterAttackDamageType
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }
    public class MonsterActionUsage
    {
        [JsonProperty("type")]
        public string type;
        [JsonProperty("times")]
        public int times;
    }
    public class MonsterLegendaryAction
    {
        [JsonProperty("name")]
        public string name;
        [JsonProperty("desc")]
        public string desc;
        [JsonProperty("attack_bonus")]
        public int attackBonus;
        [JsonProperty("damage")]
        public MonsterLegendaryActionDamage[] damage;
    }
    public class MonsterLegendaryActionDamage
    {
        [JsonProperty("damage_type")]
        public MonsterLegendaryActionDamageType type;
        [JsonProperty("damage_dice")]
        public string damageDice;
    }
    public class MonsterLegendaryActionDamageType
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }

    //Used for Race API lookups
    public class RaceRoot
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("speed")]
        public int speed;
        [JsonProperty("ability_bonuses")]
        public RaceAbilityBonus[] abilityBonuses;
        [JsonProperty("alignment")]
        public string alignment;
        [JsonProperty("age")]
        public string age;
        [JsonProperty("size")]
        public string size;
        [JsonProperty("size_description")]
        public string sizeDescription;
        [JsonProperty("starting_proficiencies")]
        public RaceStartingProficiencies[] startingProficiencies;
        [JsonProperty("languages")]
        public RaceLanguages[] languages;
        [JsonProperty("language_desc")]
        public string languageDesc;
        [JsonProperty("traits")]
        public RaceTraits[] traits;
        [JsonProperty("trait_options", NullValueHandling = NullValueHandling.Ignore)]
        public RaceTraitOptions traitOptions;
        [JsonProperty("subraces")]
        public RaceSubraces[] subraces;
    }
    public class RaceAbilityBonus
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("bonus")]
        public int bonus;
        [JsonProperty("url")]
        public string url;
    }
    public class RaceStartingProficiencies
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }
    public class RaceLanguages
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }
    public class RaceTraits
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }
    public class RaceTraitOptions
    {
        [JsonProperty("choose")]
        public int choose;
        [JsonProperty("from")]
        public RaceTraitOptionsFrom[] from;
        [JsonProperty("type")]
        public string type;
    }
    public class RaceTraitOptionsFrom
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }
    public class RaceSubraces
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }

    //Used for Subclass API lookups
    public class SubclassRoot
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("class")]
        public SubclassClass subclassClass;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("subclass_flavor")]
        public string flavor;
        [JsonProperty("desc")]
        public string[] desc;
        [JsonProperty("subclass_levels")]
        public string levels;
        [JsonProperty("url")]
        public string url;
    }
    public class SubclassClass
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }

    //Used for Trait API lookups
    public class TraitRoot
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("races")]
        public TraitRaces[] races;
        [JsonProperty("subraces")]
        public TraitSubraces[] subraces;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("desc")]
        public string[] desc;
        [JsonProperty("proficiencies")]
        public TraitProficiencies[] proficiencies;
        [JsonProperty("url")]
        public string url;
    }
    public class TraitRaces
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }
    public class TraitSubraces
    {
        [JsonProperty("name")]
        public string name;
    }
    public class TraitProficiencies
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }

    //Used for Weapon Property API lookups
    public class WeaponPropertyRoot
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("desc")]
        public string[] desc;
        [JsonProperty("url")]
        public string url;
    }

    //Used for Subrace API lookups
    public class SubraceRoot
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("desc")]
        public string desc;
        [JsonProperty("ability_bonuses")]
        public SubraceAbilityBonuses[] abilityBonuses;
        [JsonProperty("language_options", NullValueHandling = NullValueHandling.Ignore)]
        public SubraceLanguageOptions languageOptions;
        [JsonProperty("racial_traits")]
        public SubraceRacialTraits[] racialTraits;
        [JsonProperty("racial_trait_options", NullValueHandling = NullValueHandling.Ignore)]
        public SubraceRacialTraitOptions racialTraitOptions;
        [JsonProperty("starting_proficiencies")]
        public SubraceStartingProficiencies[] startingProficiencies;
        [JsonProperty("url")]
        public string url;
    }
    public class SubraceAbilityBonuses
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("bonus")]
        public int bonus;
        [JsonProperty("url")]
        public string url;
    }
    public class SubraceLanguageOptions
    {
        [JsonProperty("choose")]
        public int choose;
        [JsonProperty("from")]
        public SubraceLanguageOptionsFrom[] from;
        [JsonProperty("type")]
        public string type;
    }
    public class SubraceLanguageOptionsFrom
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }
    public class SubraceRacialTraits
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }
    public class SubraceRacialTraitOptions
    {
        [JsonProperty("choose")]
        public int choose;
        [JsonProperty("from")]
        public SubraceRacialTraitOptionsFrom[] from;
        [JsonProperty("type")]
        public string type;
    }
    public class SubraceRacialTraitOptionsFrom
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }
    public class SubraceStartingProficiencies
    {
        [JsonProperty("index")]
        public string index;
        [JsonProperty("name")]
        public string name;
        [JsonProperty("url")]
        public string url;
    }
}
