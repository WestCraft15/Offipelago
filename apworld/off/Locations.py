import typing
from enum import Enum, auto
from typing import Dict, List, Optional

from .constants import LOCATION_ID_START, MAX_SHOP_SLOTS_PER_SHOP, SHOP_NAMES, ZODIAC_BOSS_NAMES


class LocationType(Enum):
    CHEST = auto()
    LIBRARY_BOOK = auto()
    POSTAL_HINT = auto()
    BOSS = auto()
    PILLAR_ART = auto()
    SECRET_BOSS = auto()
    SHOP = auto()
    ADD_ON = auto()
    EVENT = auto()


class LocationData(typing.NamedTuple):
    id: Optional[int]
    type: LocationType


def _numbered(prefix: str, count: int) -> List[str]:
    return [f"{prefix} {i}" for i in range(1, count + 1)]


# Amount of chests
ZONE_0_CHESTS = _numbered("Zone 0 Chest", 2)
ZONE_1_CHESTS = _numbered("Zone 1 Chest", 19)
ZONE_2_CHESTS = _numbered("Zone 2 Chest", 23)
ZONE_2_PURIFIED_CHESTS = _numbered("Zone 2 Purified Chest", 7)
ZONE_3_CHESTS = _numbered("Zone 3 Chest", 13)
ZONE_3_PURIFIED_CHESTS = _numbered("Zone 3 Purified Chest", 10)
CHAMBRE_CHESTS = _numbered("Chambre Chest", 5)

# The last 4 Zone 3 chests represent "Area 4", gated behind the Access Card in rules.py
ZONE_3_AREA_4_CHESTS = ZONE_3_CHESTS[-4:]

ZONE_2_BOOKS = _numbered("Zone 2 Library Book", 8)
ZONE_2_PURIFIED_BOOKS = _numbered("Zone 2 Purified Library Book", 8)

POSTAL_HINTS = _numbered("Postal Hint", 6)

# Bosses Locations
BOSS_LOCATIONS = ["Zone 1 Boss - Dedan", "Zone 2 Boss - Japhet", "Zone 3 Boss - Enoch"]

# Zodiac Bosses Locations
ZODIAC_BOSS_LOCATIONS = [f"Zodiac Boss - {name}" for name in ZODIAC_BOSS_NAMES]

# Secret Bosses Locations(not sure if sugar is the only one)
SECRET_BOSS_LOCATIONS = ["Sugar"]

# Locations for the 3 onion rings
ADD_ON_LOCATIONS = ["Alpha Recruited", "Omega Recruited", "Epsilon Recruited"]

# Pillar Art locations
PILLAR_ART_LOCATIONS = [
    "Zone 1 Pillar Art Chest",
    "Zone 2 Pillar Art Chest",
    "Zone 3 Pillar Art Chest",
    "The Room Pillar Art Chest",
    "Purified Zone Pillar Art Chest",
    "Bonus Pillar Art Chest 1",
    "Bonus Pillar Art Chest 2",
    "Bonus Pillar Art Chest 3",
]

# A location for the secret ending
SECRET_ENDING_LOCATION = "Secret Ending Reward"

# For Shops
SHOP_SLOT_LOCATIONS: Dict[str, List[str]] = {
    shop: _numbered(f"{shop} Slot", MAX_SHOP_SLOTS_PER_SHOP) for shop in SHOP_NAMES
}

# Event-only locationsso they don't enter the norma item pool
EVENT_LOCATIONS = [
    "Chambre Finale",
    "Dedan Defeated Event", "Japhet Defeated Event", "Enoch Defeated Event",
    *[f"{name} Defeated Event" for name in ZODIAC_BOSS_NAMES],
    "Sugar Defeated Event",
]


def _build_location_table() -> Dict[str, LocationData]:
    table: Dict[str, LocationData] = {}
    code = LOCATION_ID_START

    def add(names: List[str], ltype: LocationType) -> None:
        nonlocal code
        for n in names:
            table[n] = LocationData(code, ltype)
            code += 1

    add(ZONE_0_CHESTS, LocationType.CHEST)
    add(ZONE_1_CHESTS, LocationType.CHEST)
    add(ZONE_2_CHESTS, LocationType.CHEST)
    add(ZONE_2_PURIFIED_CHESTS, LocationType.CHEST)
    add(ZONE_3_CHESTS, LocationType.CHEST)
    add(ZONE_3_PURIFIED_CHESTS, LocationType.CHEST)
    add(CHAMBRE_CHESTS, LocationType.CHEST)
    add(ZONE_2_BOOKS, LocationType.LIBRARY_BOOK)
    add(ZONE_2_PURIFIED_BOOKS, LocationType.LIBRARY_BOOK)
    add(POSTAL_HINTS, LocationType.POSTAL_HINT)
    add(BOSS_LOCATIONS, LocationType.BOSS)
    add(ZODIAC_BOSS_LOCATIONS, LocationType.BOSS)
    add(ADD_ON_LOCATIONS, LocationType.ADD_ON)
    add(PILLAR_ART_LOCATIONS, LocationType.PILLAR_ART)
    add(SECRET_BOSS_LOCATIONS, LocationType.SECRET_BOSS)
    add([SECRET_ENDING_LOCATION], LocationType.CHEST)
    for shop, slots in SHOP_SLOT_LOCATIONS.items():
        add(slots, LocationType.SHOP)

    for name in EVENT_LOCATIONS:
        table[name] = LocationData(None, LocationType.EVENT)

    return table


location_table: Dict[str, LocationData] = _build_location_table()

location_name_to_id: Dict[str, int] = {
    name: data.id for name, data in location_table.items() if data.id is not None
}
