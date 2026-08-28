from typing import Dict, TYPE_CHECKING

from BaseClasses import CollectionState, ItemClassification, LogicMixin, MultiWorld
from worlds.generic.Rules import set_rule

from .constants import PROGRESSIVE_ZONE_STEPS, ZODIAC_BOSS_NAMES, FEATURE_COMBAT_POWER_LOGIC
from .items import PROGRESSIVE_ZONE_ITEM, MACGUFFIN_ITEM
from .locations import SECRET_ENDING_LOCATION

if TYPE_CHECKING:
    from .world import OffWorld


# the logic is mostly about how strong the person is for now
class OffLogic(LogicMixin):
    off_power: Dict[int, float]

    def init_mixin(self, multiworld: MultiWorld) -> None:
        offs = multiworld.get_game_worlds("OFF")
        self.off_power = {off.player: 0.0 for off in offs}

    def copy_mixin(self, new_state: CollectionState) -> CollectionState:
        new_state.off_power = dict(self.off_power)
        return new_state


def _has_power(world: 'OffWorld', threshold: float):
    """Requires an accumulated combat-power score of at least `threshold`(ought to be determined eventually)"""
    if threshold <= 0:
        return lambda state: True
    player = world.player
    return lambda state: state.off_power[player] >= threshold


GRAND_ITEM_NAMES = [
    "Grand Finale", "Grand Diagonal", "Grand Spectral", "Grand Brachial", "Grand Chocolatier",
]

MAIN_BOSS_EVENTS = ["Dedan Defeated", "Japhet Defeated", "Enoch Defeated"]
ZODIAC_EVENTS = [f"{name} Defeated" for name in ZODIAC_BOSS_NAMES]
EXTRA_BOSS_EVENTS = ["Sugar Defeated"]


def _zone_access_rule(world: 'OffWorld', card_name: str):
    """Requires either the Nth Progressive Zone, or the named card, depending on options."""
    player = world.player
    if world.options.progressive_zones:
        step = PROGRESSIVE_ZONE_STEPS.index(card_name) + 1
        return lambda state: state.has(PROGRESSIVE_ZONE_ITEM, player, step)
    return lambda state: state.has(card_name, player)


def _has_all_grand_objects(state: CollectionState, player: int) -> bool:
    return state.has_all(GRAND_ITEM_NAMES, player)


def set_rules(world: 'OffWorld') -> None:
    player = world.player

    def entrance(name: str):
        return world.get_entrance(name)

    set_rule(entrance("Zone 0 -> Zone 1"), _zone_access_rule(world, "Leo-card"))
    set_rule(entrance("Zone 1 Boss Arena -> Zone 2"), _zone_access_rule(world, "Cancer-card"))
    set_rule(entrance("Zone 2 Boss Arena -> Zone 3"), _zone_access_rule(world, "Pisces-card"))
    set_rule(entrance("Zone 3 Boss Arena -> The Room"), _zone_access_rule(world, "Aquarius-card"))
    set_rule(entrance("The Room -> Chambre"), _zone_access_rule(world, "Sagittarius-card"))

    if world.options.zones_are_unlocks:
        set_rule(entrance("Zone 1 Boss Arena -> Zone 1 Purified"),
                 lambda state: state.has("Zone 1 Purified Key", player))
        set_rule(entrance("Zone 2 Boss Arena -> Zone 2 Purified"),
                 lambda state: state.has("Zone 2 Purified Key", player))
        set_rule(entrance("Zone 3 Boss Arena -> Zone 3 Purified"),
                 lambda state: state.has("Zone 3 Purified Key", player))

    # Justus is only fought after Source, Maldicion, and Psalmanazar & Herodotus are all down.
    justus_rule = lambda state: state.has_all(
        ["Source Defeated", "Maldicion Defeated", "Psalmanazar & Herodotus Defeated"], player)
    set_rule(world.get_location("Zodiac Boss - Justus"), justus_rule)
    set_rule(world.get_location("Justus Defeated Event"), justus_rule)

    # Carnival appears in the Nothingness only after Justus is defeated.
    carnival_rule = lambda state: state.has("Justus Defeated", player)
    set_rule(world.get_location("Zodiac Boss - Carnival"), carnival_rule)
    set_rule(world.get_location("Carnival Defeated Event"), carnival_rule)

    # Cob is found near Hugo only after Carnival is defeated.
    cob_rule = lambda state: state.has("Carnival Defeated", player)
    set_rule(world.get_location("Zodiac Boss - Cob"), cob_rule)
    set_rule(world.get_location("Cob Defeated Event"), cob_rule)

    # so we don't get a goober thing at sugar
    if FEATURE_COMBAT_POWER_LOGIC:
        combat_ready = _has_power(world, 8.0)
        sugar_rule = lambda state: any(state.has(name, player) for name in GRAND_ITEM_NAMES) and combat_ready(state)
    else:
        sugar_rule = lambda state: any(state.has(name, player) for name in GRAND_ITEM_NAMES)
    set_rule(world.get_location("Sugar"), sugar_rule)
    set_rule(world.get_location("Sugar Defeated Event"), sugar_rule)

    # Secret ending unlocked by trading all 5 Grands to Zacharie for the Aries-card
    set_rule(world.get_location(SECRET_ENDING_LOCATION), lambda state: state.has("Aries-card", player))

    # Zone 3's "Area 4" is locked behind the Access Card.
    for loc_name in world.zone_3_area_4_chests:
        set_rule(world.get_location(loc_name), lambda state: state.has("Access Card", player))

    # Deprioritize Carnival/Cob: forbid progression items from landing on those checks.
    if world.options.deprioritize_carnival:
        world.get_location("Zodiac Boss - Carnival").item_rule = \
            lambda item: item.classification != ItemClassification.progression
    if world.options.deprioritize_cob:
        world.get_location("Zodiac Boss - Cob").item_rule = \
            lambda item: item.classification != ItemClassification.progression

    world.multiworld.completion_condition[player] = _build_completion_condition(world)


def _build_completion_condition(world: 'OffWorld'):
    player = world.player
    goal = world.options.goal.value

    def standard(state: CollectionState) -> bool:
        return state.has("Victory", player)

    def main_bosses(state: CollectionState) -> bool:
        return state.has_all(MAIN_BOSS_EVENTS, player)

    def main_bosses_and_zodiacs(state: CollectionState) -> bool:
        return main_bosses(state) and state.has_all(ZODIAC_EVENTS, player)

    def all_bosses(state: CollectionState) -> bool:
        return main_bosses_and_zodiacs(state) and state.has_all(EXTRA_BOSS_EVENTS, player)

    def macguffin_hunt(state: CollectionState) -> bool:
        needed = world.macguffin_goal_amount
        return state.has(MACGUFFIN_ITEM, player, needed)

    return {
        0: standard,
        1: main_bosses,
        2: main_bosses_and_zodiacs,
        3: all_bosses,
        4: macguffin_hunt,
    }[goal]
