from dataclasses import dataclass

from Options import DeathLink, PerGameCommonOptions, Choice, Range, Toggle, DefaultOnToggle


class Goal(Choice):
    """
    Choose your goal!!
    """
    display_name = "Goal"
    option_standard = 0
    option_main_bosses = 1
    option_main_bosses_and_zodiacs = 2
    option_all_bosses = 3
    option_macguffin_hunt = 4


class MacGuffinAmount(Range):
    """How many MacGuffin items should be present? (Only applies to MacGuffin Hunt)"""
    display_name = "MacGuffin Amount"
    range_start = 1
    range_end = 99
    default = 10


class MacGuffinRequirement(Range):
    """How many MacGuffin items should be needed to complete the game? (Set to 0 to require all)"""
    display_name = "MacGuffin Requirement"
    range_start = 0
    range_end = 99
    default = 0


class ProgressiveEquipment(DefaultOnToggle):
    """Should equipment be progressive?"""
    display_name = "Progressive Equipment"


class ProgressiveZones(Toggle):
    """Should Zones be progressive?"""
    display_name = "Progressive Zones"


class PureZonesAreUnlocks(DefaultOnToggle):
    """Should purified zones require an additional item?"""
    display_name = "Purified Zones Are Unlocks"


class DeprioritizeCarnival(DefaultOnToggle):
    """Should Carnival be unable to give an important item?"""
    display_name = "Deprioritize Carnival"


class DeprioritizeCob(DefaultOnToggle):
    """Should Cob be unable to give an important item?"""
    display_name = "Deprioritize Cob"


class CombatAssistance(Choice):
    """
    trust_the_grind: No special measures, win underlevelled

    grinding_npc: Requests that the companion patch add an always-accessible NPC (in the Nothingness) who
    will fight the Batter for a solid chunk of EXP, so a dedicated grinding option always exists.

    enemy_scaling: Make enemies scale down so it is easier
    """
    display_name = "Combat Assistance"
    option_trust_the_grind = 0
    option_grinding_npc = 1
    option_enemy_scaling = 2
    default = 0


class ExperienceMultiplier(Range):
    """
    Multiplies all EXP earned from battles (100 = normal rate). Higher values reduce how much grinding is
    needed to keep pace with logic; combine with Combat Assistance for extra breathing room.
    """
    display_name = "Experience Multiplier"
    range_start = 100
    range_end = 500
    default = 100


class RandomizeCompetences(Toggle):
    """
    Should the Add-ons and batter's competences randomised
    """
    display_name = "Randomize Competences"


def _create_filler_weight_class(item_name: str, description: str, default_weight: int):
    """
    Filler stuff(I prob need to write stuff down one day)
    """
    class_name = item_name.replace(" ", "").replace("'", "") + "FillerWeight"
    display_name = f"{item_name} Filler Weight"
    docstring = f"""Weight for {item_name} filler items. {description}"""

    return type(
        class_name,
        (Choice,),
        {
            "__doc__": docstring,
            "display_name": display_name,
            "option_none": 0,
            "option_low": 1,
            "option_medium": 3,
            "option_high": 5,
            "default": default_weight,
        }
    )


CashFillerWeight = _create_filler_weight_class(
    "Cash", "Generic money filler; also the fallback used if every other filler weight is 0.", default_weight=5)
LuckTicketFillerWeight = _create_filler_weight_class(
    "Luck ticket", "A consumable that improves random outcomes.", default_weight=3)
FortuneTicketFillerWeight = _create_filler_weight_class(
    "Fortune ticket", "A consumable that improves random outcomes.", default_weight=3)
SilverFleshFillerWeight = _create_filler_weight_class(
    "Silver flesh", "A healing consumable.", default_weight=3)
GoldenFleshFillerWeight = _create_filler_weight_class(
    "Golden flesh", "A stronger healing consumable.", default_weight=1)
JokerFillerWeight = _create_filler_weight_class(
    "Joker", "A wildcard consumable.", default_weight=1)
MolochsMeatFillerWeight = _create_filler_weight_class(
    "Moloch's meat", "A combat consumable.", default_weight=1)
BelialsMeatFillerWeight = _create_filler_weight_class(
    "Belial's meat", "A combat consumable.", default_weight=1)
AbaddonsMeatFillerWeight = _create_filler_weight_class(
    "Abaddon's meat", "A combat consumable.", default_weight=1)


class ShopChecks(Range):
    """How many shop slots should contain checks?"""
    display_name = "Shop Checks"
    range_start = 0
    range_end = 4
    default = 0


class Shopsanity(Toggle):
    """Should shops be fully randomized?"""


@dataclass
class OffOptions(PerGameCommonOptions):
    goal: Goal
    macguffin_amount: MacGuffinAmount
    macguffin_requirement: MacGuffinRequirement
    progressive_equipment: ProgressiveEquipment
    progressive_zones: ProgressiveZones
    zones_are_unlocks: PureZonesAreUnlocks
    deprioritize_carnival: DeprioritizeCarnival
    deprioritize_cob: DeprioritizeCob
    combat_assistance: CombatAssistance
    experience_multiplier: ExperienceMultiplier
    randomize_competences: RandomizeCompetences
    shop_checks: ShopChecks
    shopsanity: Shopsanity
    death_link: DeathLink
    cash_filler_weight: CashFillerWeight
    luck_ticket_filler_weight: LuckTicketFillerWeight
    fortune_ticket_filler_weight: FortuneTicketFillerWeight
    silver_flesh_filler_weight: SilverFleshFillerWeight
    golden_flesh_filler_weight: GoldenFleshFillerWeight
    joker_filler_weight: JokerFillerWeight
    molochs_meat_filler_weight: MolochsMeatFillerWeight
    belials_meat_filler_weight: BelialsMeatFillerWeight
    abaddons_meat_filler_weight: AbaddonsMeatFillerWeight
