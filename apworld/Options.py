from dataclasses import dataclass

from Options import DeathLink, PerGameCommonOptions, Choice, Range, Toggle, DefaultOnToggle


class Goal(Choice):
    """
    Choose how you unlock Hugo's room.

    standard: Defeat Vader Eloha

    main_bosses: Defeat the bosses of each zone

    main_bosses_and_zodiacs: Defeat the bosses of each zone and the 4 main zodiacs

    all_bosses: Defeat every boss in the game

    macguffin_hunt: Find enough MacGuffin items scattered through the multiworld
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
    shop_checks: ShopChecks
    shopsanity: Shopsanity
    death_link: DeathLink
