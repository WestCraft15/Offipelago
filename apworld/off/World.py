from typing import Any, Dict, List

from BaseClasses import Item, ItemClassification, Location, Region, CollectionState
from worlds.AutoWorld import World

from . import locations as loc
from .items import (
    ItemData, ItemType, event_item_table, filler_item_names, item_table,
    FLAT_EQUIPMENT_GROUPS, PROGRESSIVE_EQUIPMENT_COUNTS, PURIFIED_KEY_ITEMS,
    PROGRESSIVE_ZONE_ITEM, MACGUFFIN_ITEM, ADD_ON_NAMES, COMPETENCE_ITEM_NAMES, ALL_COMPETENCE_ITEMS,
    MEDICINE_ITEM_NAMES, POWER_WEIGHTS,
)
from .constants import (
    NUM_PROGRESSIVE_ZONE_STEPS,
    FEATURE_PROGRESSIVE_ZONES, FEATURE_PROGRESSIVE_EQUIPMENT, FEATURE_ZONES_ARE_UNLOCKS,
    FEATURE_SHOPSANITY, FEATURE_MACGUFFIN_HUNT, FEATURE_ADD_ON_RANDOMIZATION,
    FEATURE_COMPETENCE_RANDOMIZATION, FEATURE_COMBAT_ASSISTANCE, FEATURE_WEIGHTED_FILLER,
)
from .locations import LocationData, location_name_to_id, location_table
from .options import OffOptions
from .regions import create_regions as build_regions
from .rules import set_rules as build_rules
from .web_world import OFFWeb

_individual_zone_cards = ["Leo-card", "Cancer-card", "Pisces-card", "Aquarius-card", "Sagittarius-card"]

# filler item names
FILLER_WEIGHT_OPTION_NAMES: Dict[str, str] = {
    "Cash": "cash_filler_weight",
    "Luck ticket": "luck_ticket_filler_weight",
    "Fortune ticket": "fortune_ticket_filler_weight",
    "Silver flesh": "silver_flesh_filler_weight",
    "Golden flesh": "golden_flesh_filler_weight",
    "Joker": "joker_filler_weight",
    "Moloch's meat": "molochs_meat_filler_weight",
    "Belial's meat": "belials_meat_filler_weight",
    "Abaddon's meat": "abaddons_meat_filler_weight",
}

# event location names
EVENT_ITEM_PLACEMENTS: Dict[str, str] = {
    "Chambre Finale": "Victory",
    "Dedan Defeated Event": "Dedan Defeated",
    "Japhet Defeated Event": "Japhet Defeated",
    "Enoch Defeated Event": "Enoch Defeated",
    "Source Defeated Event": "Source Defeated",
    "Maldicion Defeated Event": "Maldicion Defeated",
    "Psalmanazar & Herodotus Defeated Event": "Psalmanazar & Herodotus Defeated",
    "Justus Defeated Event": "Justus Defeated",
    "Carnival Defeated Event": "Carnival Defeated",
    "Cob Defeated Event": "Cob Defeated",
    "Sugar Defeated Event": "Sugar Defeated",
}


class OFFItem(Item):
    game = "OFF"


class OFFLocation(Location):
    game = "OFF"


class OffWorld(World):
    """
    OFF is an amazing game go play it neow!!
    """
    game = "OFF"
    web = OFFWeb()

    options_dataclass = OffOptions
    options: OffOptions

    item_name_to_id = {name: data.code for name, data in item_table.items() if data.code is not None}
    location_name_to_id = location_name_to_id

    item_name_groups = {
        "Cards": {*_individual_zone_cards, "Aries-card", PROGRESSIVE_ZONE_ITEM},
        "Grand Objects": {
            "Grand Finale", "Grand Diagonal", "Grand Spectral", "Grand Brachial", "Grand Chocolatier",
        },
        "Library Books": {name for name, data in item_table.items() if data.type == ItemType.LIBRARY},
        "Weapons": {*FLAT_EQUIPMENT_GROUPS[ItemType.WEAPON], "Progressive Batter Weapon", "Progressive Add-On Weapon"},
        "Equipment": {
            *FLAT_EQUIPMENT_GROUPS[ItemType.WEAPON], *FLAT_EQUIPMENT_GROUPS[ItemType.SHIELD],
            *FLAT_EQUIPMENT_GROUPS[ItemType.BODY], *FLAT_EQUIPMENT_GROUPS[ItemType.HEAD],
            *FLAT_EQUIPMENT_GROUPS[ItemType.ACCESSORY], *PROGRESSIVE_EQUIPMENT_COUNTS.keys(),
        },
        "Medicine": {name for name, data in item_table.items() if data.type == ItemType.MEDICINE},
        "Add-Ons": set(ADD_ON_NAMES),
        "Competences": set(ALL_COMPETENCE_ITEMS),
    }

    location_name_groups = {
        "Chests": {name for name, data in location_table.items() if data.type.name == "CHEST"},
        "Library Books": {name for name, data in location_table.items() if data.type.name == "LIBRARY_BOOK"},
        "Bosses": {name for name, data in location_table.items() if data.type.name == "BOSS"},
        "Secret Bosses": {name for name, data in location_table.items() if data.type.name == "SECRET_BOSS"},
        "Shops": {name for name, data in location_table.items() if data.type.name == "SHOP"},
        "Add-Ons": {name for name, data in location_table.items() if data.type.name == "ADD_ON"},
    }

    zone_3_area_4_chests = loc.ZONE_3_AREA_4_CHESTS

    def generate_early(self) -> None:
        if not FEATURE_PROGRESSIVE_ZONES:
            self.options.progressive_zones.value = 0
        if not FEATURE_PROGRESSIVE_EQUIPMENT:
            self.options.progressive_equipment.value = 0
        if not FEATURE_ZONES_ARE_UNLOCKS:
            self.options.zones_are_unlocks.value = 0
        if not FEATURE_SHOPSANITY:
            self.options.shopsanity.value = 0
            self.options.shop_checks.value = 0
        if not FEATURE_MACGUFFIN_HUNT and self.options.goal.value == self.options.goal.option_macguffin_hunt:
            self.options.goal.value = self.options.goal.option_standard
        if not FEATURE_COMPETENCE_RANDOMIZATION:
            self.options.randomize_competences.value = 0
        if not FEATURE_COMBAT_ASSISTANCE:
            self.options.combat_assistance.value = self.options.combat_assistance.option_trust_the_grind

    def create_region(self, name: str, locations: List[str]) -> Region:
        region = Region(name, self.player, self.multiworld)
        for loc_name in locations:
            loc_data: LocationData = location_table[loc_name]
            region.locations.append(OFFLocation(self.player, loc_name, loc_data.id, region))
        return region

    def create_regions(self) -> None:
        build_regions(self, self.player)

        for location_name, item_name in EVENT_ITEM_PLACEMENTS.items():
            location = self.get_location(location_name)
            data = event_item_table[item_name]
            event_item = OFFItem(item_name, data.classification, None, self.player)
            location.place_locked_item(event_item)

    def create_item(self, name: str) -> Item:
        data: ItemData = item_table[name]
        return OFFItem(name, data.classification, data.code, self.player)

    def collect(self, state: CollectionState, item: Item) -> bool:
        change = super().collect(state, item)
        if change:
            data = item_table.get(item.name)
            if data is not None and data.type in POWER_WEIGHTS:
                state.off_power[self.player] += POWER_WEIGHTS[data.type]
        return change

    def remove(self, state: CollectionState, item: Item) -> bool:
        change = super().remove(state, item)
        if change:
            data = item_table.get(item.name)
            if data is not None and data.type in POWER_WEIGHTS:
                state.off_power[self.player] -= POWER_WEIGHTS[data.type]
        return change

    def create_items(self) -> None:
        self.build_filler_pools()
        pool: List[Item] = []

        # Zone cards: progressive or individual.
        if self.options.progressive_zones:
            pool += [self.create_item(PROGRESSIVE_ZONE_ITEM) for _ in range(NUM_PROGRESSIVE_ZONE_STEPS)]
        else:
            pool += [self.create_item(name) for name in _individual_zone_cards]

        # Plot keys other than zone cards
        for name in ["Aries-card", "Access Card", "Music Box", "Calendar Page", "Stamped Note"]:
            pool.append(self.create_item(name))

        if self.options.zones_are_unlocks:
            pool += [self.create_item(name) for name in PURIFIED_KEY_ITEMS]

        # Add-Ons randomization
        if FEATURE_ADD_ON_RANDOMIZATION:
            pool += [self.create_item(name) for name in ADD_ON_NAMES]

        if self.options.randomize_competences:
            for names in COMPETENCE_ITEM_NAMES.values():
                pool += [self.create_item(name) for name in names]

        for name, data in item_table.items():
            if data.type in (ItemType.GRAND, ItemType.LIBRARY, ItemType.MISC,
                              ItemType.SPECIAL, ItemType.SEED):
                pool.append(self.create_item(name))

        # whether equipement progressive or flat
        if self.options.progressive_equipment:
            for name, count in PROGRESSIVE_EQUIPMENT_COUNTS.items():
                pool += [self.create_item(name) for _ in range(count)]
        else:
            for group in FLAT_EQUIPMENT_GROUPS.values():
                pool += [self.create_item(name) for name in group]

        non_event_location_count = sum(
            1 for l in self.multiworld.get_locations(self.player) if l.address is not None
        )

        # MacGuffins replace filler, rather than being added on top of the normal pool, so a
        # large macguffin_amount can't overflow the number of available locations.
        self.macguffin_goal_amount = 0
        if self.options.goal.value == self.options.goal.option_macguffin_hunt:
            budget = max(non_event_location_count - len(pool), 1)
            amount = min(self.options.macguffin_amount.value, budget)
            requirement = self.options.macguffin_requirement.value
            self.macguffin_goal_amount = amount if requirement == 0 else min(requirement, amount)
            pool += [self.create_item(MACGUFFIN_ITEM) for _ in range(amount)]

        remaining = non_event_location_count - len(pool)
        for _ in range(max(remaining, 0)):
            pool.append(self.create_item(self.get_filler_item_name()))

        self.multiworld.itempool += pool

    def build_filler_pools(self) -> None:
        """
        Pre-computes a weighted filler pool from the *FillerWeight options
        """
        if not FEATURE_WEIGHTED_FILLER:
            self._filler_pool: List[str] = ["Cash"]
            return
        weighted: List[str] = []
        for name, option_name in FILLER_WEIGHT_OPTION_NAMES.items():
            weight = getattr(self.options, option_name).value
            weighted += [name] * weight
        # Fallback for the edge case where every weight is 0 Cash remains the guaranteed padding
        # item, matching its existing role elsewhere in this file.
        self._filler_pool = weighted or ["Cash"]

    def get_filler_item_name(self) -> str:
        if not FEATURE_WEIGHTED_FILLER:
            return "Cash"
        if not hasattr(self, "_filler_pool"):
            self.build_filler_pools()
        return self.multiworld.random.choice(self._filler_pool)

    def set_rules(self) -> None:
        build_rules(self)

    def fill_slot_data(self) -> Dict[str, Any]:
        return {
            "goal": self.options.goal.value,
            "macguffin_goal_amount": getattr(self, "macguffin_goal_amount", 0),
            "progressive_equipment": bool(self.options.progressive_equipment),
            "progressive_zones": bool(self.options.progressive_zones),
            "zones_are_unlocks": bool(self.options.zones_are_unlocks),
            "shop_checks": self.options.shop_checks.value,
            "shopsanity": bool(self.options.shopsanity),
            "combat_assistance": self.options.combat_assistance.value,
            "experience_multiplier": self.options.experience_multiplier.value,
            "randomize_competences": bool(self.options.randomize_competences),
            "death_link": bool(self.options.death_link),
        }
