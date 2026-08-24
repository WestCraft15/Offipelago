import typing
from enum import Enum, auto
from typing import Dict, List, Optional

from .constants import LOCATION_ID_START


class LocationType(Enum):
    CHEST = auto()
    LIBRARY_BOOK = auto()
    POSTAL_HINT = auto()
    BOSS = auto()
    PILLAR_ART = auto()
    SECRET_BOSS = auto()
    EVENT = auto()


class LocationData(typing.NamedTuple):
    id: Optional[int]
    type: LocationType


def _numbered(prefix: str, count: int) -> List[str]:
    return [f"{prefix} {i}" for i in range(1, count + 1)]


ZONE_0_CHESTS = _numbered("Zone 0 Chest", 2)
ZONE_1_CHESTS = _numbered("Zone 1 Chest", 19)
ZONE_2_CHESTS = _numbered("Zone 2 Chest", 23)
ZONE_2_PURIFIED_CHESTS = _numbered("Zone 2 Purified Chest", 7)
ZONE_3_CHESTS = _numbered("Zone 3 Chest", 13)
ZONE_3_PURIFIED_CHESTS = _numbered("Zone 3 Purified Chest", 10)
CHAMBRE_CHESTS = _numbered("Chambre Chest", 5)

ZONE_3_AREA_4_CHESTS = ZONE_3_CHESTS[-4:]

ZONE_2_BOOKS = _numbered("Zone 2 Library Book", 8)
ZONE_2_PURIFIED_BOOKS = _numbered("Zone 2 Purified Library Book", 8)

POSTAL_HINTS = _numbered("Postal Hint", 6)

BOSS_LOCATIONS = ["Zone 1 Boss - Dedan", "Zone 2 Boss - Japhet", "Zone 3 Boss - Enoch"]

PILLAR_ART_LOCATIONS = [
    "Zone 1 Pillar Art Chest",
    "Zone 2 Pillar Art Chest",
    "Zone 3 Pillar Art Chest",
    "The Room Pillar Art Chest",
    "Purified Zone Pillar Art Chest",
]
BONUS_PILLAR_ART_LOCATIONS = _numbered("Bonus Pillar Art Chest", 3)

SECRET_BOSS_LOCATIONS = _numbered("Secret Boss", 6)

EVENT_LOCATIONS = ["Chambre Finale"]


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
    add(PILLAR_ART_LOCATIONS, LocationType.PILLAR_ART)
    add(BONUS_PILLAR_ART_LOCATIONS, LocationType.PILLAR_ART)
    add(SECRET_BOSS_LOCATIONS, LocationType.SECRET_BOSS)

    for name in EVENT_LOCATIONS:
        table[name] = LocationData(None, LocationType.EVENT)

    return table


location_table: Dict[str, LocationData] = _build_location_table()

location_name_to_id: Dict[str, int] = {
    name: data.id for name, data in location_table.items() if data.id is not None
}
