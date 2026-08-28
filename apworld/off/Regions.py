from typing import TYPE_CHECKING, Dict, List

from . import locations as loc
from .constants import MAX_SHOP_SLOTS_PER_SHOP, SHOP_NAMES, FEATURE_ADD_ON_RANDOMIZATION

if TYPE_CHECKING:
    from .world import OffWorld


# Which locations live in which region not I haven't really played the remastered so idrk where are the pillar arts
BASE_REGION_LOCATIONS: Dict[str, List[str]] = {
    "Menu": [],
    "Zone 0": [*loc.ZONE_0_CHESTS, "Sugar", "Sugar Defeated Event"],
    "Zone 1": [*loc.ZONE_1_CHESTS, "Postal Hint 1", "Postal Hint 2"],
    "Zone 1 Boss Arena": ["Zone 1 Boss - Dedan", "Dedan Defeated Event", "Zone 1 Pillar Art Chest"],
    "Zone 1 Purified": ["Zodiac Boss - Source", "Source Defeated Event"],
    "Zone 2": [*loc.ZONE_2_CHESTS, *loc.ZONE_2_BOOKS, "Postal Hint 3", "Postal Hint 4"],
    "Zone 2 Boss Arena": ["Zone 2 Boss - Japhet", "Japhet Defeated Event", "Zone 2 Pillar Art Chest"],
    "Zone 2 Purified": [
        *loc.ZONE_2_PURIFIED_CHESTS, *loc.ZONE_2_PURIFIED_BOOKS,
        "Zodiac Boss - Maldicion", "Maldicion Defeated Event",
    ],
    "Zone 3": [*loc.ZONE_3_CHESTS, "Postal Hint 5", "Postal Hint 6"],
    "Zone 3 Boss Arena": ["Zone 3 Boss - Enoch", "Enoch Defeated Event", "Zone 3 Pillar Art Chest"],
    "Zone 3 Purified": [
        *loc.ZONE_3_PURIFIED_CHESTS,
        "Zodiac Boss - Psalmanazar & Herodotus", "Psalmanazar & Herodotus Defeated Event",
    ],
    "The Room": [
        "The Room Pillar Art Chest",
        "Zodiac Boss - Justus", "Justus Defeated Event",
    ],
    "Nothingness": ["Zodiac Boss - Carnival", "Carnival Defeated Event"],
    "Chambre": [
        *loc.CHAMBRE_CHESTS, "Purified Zone Pillar Art Chest", "Chambre Finale",
        "Zodiac Boss - Cob", "Cob Defeated Event", loc.SECRET_ENDING_LOCATION,
        "Bonus Pillar Art Chest 1", "Bonus Pillar Art Chest 2", "Bonus Pillar Art Chest 3",
    ],
}

# how do zones connect to each other
REGION_EXITS: Dict[str, List[str]] = {
    "Menu": ["Zone 0", "Nothingness"],
    "Zone 0": ["Zone 1"],
    "Zone 1": ["Zone 1 Boss Arena"],
    "Zone 1 Boss Arena": ["Zone 2", "Zone 1 Purified"],
    "Zone 2": ["Zone 2 Boss Arena"],
    "Zone 2 Boss Arena": ["Zone 3", "Zone 2 Purified"],
    "Zone 3": ["Zone 3 Boss Arena"],
    "Zone 3 Boss Arena": ["The Room", "Zone 3 Purified"],
    "The Room": ["Chambre"],
}

# Which region each shop's slots belong to.
SHOP_REGION: Dict[str, str] = {
    "Zone 1 Shop": "Zone 1",
    "Zone 2 Shop": "Zone 2",
    "Zone 3 Shop": "Zone 3",
}


def create_regions(world: 'OffWorld', player: int) -> None:
    multiworld = world.multiworld

    region_locations = {name: list(locs) for name, locs in BASE_REGION_LOCATIONS.items()}
    region_exits = {name: list(exits) for name, exits in REGION_EXITS.items()}

    # Alpha/Omega/Epsilon recruitment checks, they are already reserved in locations.py but only actually
    # placed into their zones when the feature flag is on
    if FEATURE_ADD_ON_RANDOMIZATION:
        region_locations["Zone 1"].append("Alpha Recruited")
        region_locations["Zone 2"].append("Omega Recruited")
        region_locations["Zone 3"].append("Epsilon Recruited")

    # Shop slots: shopsanity overrides shop_checks and fills every reserved slot.
    if world.options.shopsanity:
        slot_count = MAX_SHOP_SLOTS_PER_SHOP
    else:
        slot_count = min(world.options.shop_checks.value, MAX_SHOP_SLOTS_PER_SHOP)

    for shop_name in SHOP_NAMES:
        target_region = SHOP_REGION[shop_name]
        slots = loc.SHOP_SLOT_LOCATIONS[shop_name][:slot_count]
        region_locations[target_region].extend(slots)

    regions = {}
    for name, locations in region_locations.items():
        region = world.create_region(name, locations)
        regions[name] = region
        multiworld.regions.append(region)

    for name, exits in region_exits.items():
        for target in exits:
            regions[name].connect(regions[target], f"{name} -> {target}")
