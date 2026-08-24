import typing
from enum import Enum, auto
from typing import Dict, List, Optional

from BaseClasses import ItemClassification

from .constants import ITEM_ID_START


class ItemType(Enum):
    KEY = auto()            # Zone cards / plot keys
    GRAND = auto()           # The 5 "Grand" objects for Zacharie's Secret Boss sidequest
    LIBRARY = auto()         # Books borrowed from the Zone 2 Library
    MISC = auto()            # Other unique key-item-flavored collectibles
    WEAPON = auto()
    SHIELD = auto()
    BODY = auto()
    HEAD = auto()
    ACCESSORY = auto()
    SPECIAL = auto()         # Combat consumables (Inspiration/Expiration)
    MEDICINE = auto()
    SEED = auto()            # Stat-boosting orbs
    FILLER = auto()
    EVENT = auto()


class ItemData(typing.NamedTuple):
    code: Optional[int]
    type: ItemType
    classification: ItemClassification
    event: bool = False


_key_items: List[typing.Tuple[str, ItemClassification]] = [
    ("Leo-card", ItemClassification.progression),           # Allows access to Zone 1
    ("Cancer-card", ItemClassification.progression),        # Allows access to Zone 2
    ("Pisces-card", ItemClassification.progression),        # Allows access to Zone 3
    ("Aquarius-card", ItemClassification.progression),      # Allows access to The Room
    ("Sagittarius-card", ItemClassification.progression),   # Allows access to The Room (again) / Chambre
    ("Aries-card", ItemClassification.progression),         # Allows access to a secret place
    ("Access Card", ItemClassification.progression),        # Permits access to Area 4 of Zone 3
    ("Necktie", ItemClassification.progression_deprioritized),
    ("Music Box", ItemClassification.useful),
    ("Calendar Page", ItemClassification.useful),
    ("Stamped Note", ItemClassification.useful),
]

_grand_items: List[str] = [
    "Grand Finale",
    "Grand Diagonal",
    "Grand Spectral",
    "Grand Brachial",
    "Grand Chocolatier",
]

_library_items: List[str] = [
    "The Up Children Down",
    "Bismark",
    "The Cardinal Points",
    "Without Title",
    "Tales and Legends",
    "Explanations",
    "Written by E.S.",
    "Page 33",
]

_misc_items: List[str] = [
    "Photo of You",
    "Photo of Zacharie",
    "Eye",
]

_weapons: List[str] = [
    # Batter's bats
    "Harold Bat", "Masashi Bat", "Emmanuel Bat", "Michael Bat",
    "Yoshihiro Bat", "Lewis Bat", "Katsuhiro Bat", "Ashley Bat",
    # Add-Ons' symbols
    "Audacious Symbol", "Persistant Symbol", "Choleric Symbol", "Battlesome Symbol",
    "Loyal Symbol", "Solid Symbol", "Vengeful Symbol", "Hidden Symbol",
    "Fast Symbol", "Aggressive Symbol", "Silent Symbol", "Luminous Symbol",
    "Boastful Symbol", "Mysterious Symbol", "Temperamental Symbol", "Bleeding Symbol",
    "Perfect Symbol",
]

_shields: List[str] = [
    "Aura of Justice", "Aura of Fear", "Aura of Perception", "Aura of Greatness",
    "Aura of Clairvoyance", "Aura of Tenacity", "Aura of Lunacy", "Aura of Power",
]

_bodies: List[str] = [
    # Batter's tunics
    "Nicolas Tunic", "David Tunic", "Min-Woo Tunic", "Canepa Tunic", "Taiyou Tunic", "Neil Tunic",
    # Add-Ons' epidermises
    "Radius Epidermis", "Ulna Epidermis", "Humerus Epidermis",
    "Tibia Epidermis", "Fibula Epidermis", "Femur Epidermis",
]

_heads: List[str] = [
    "Colour of Wrath", "Colour of Pain", "Colour of Sadness", "Colour of Hatred",
    "Colour of Force", "Colour of Defeat", "Colour of Neglect", "The Eighth Colour",
]

_accessories: List[str] = [
    "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday", "Secret day",
]

_specials: List[str] = ["Inspiration", "Expiration"]

_medicine: List[str] = [
    "Luck ticket", "Fortune ticket", "Silver flesh", "Golden flesh",
    "Joker", "Moloch's meat", "Belial's meat", "Abaddon's meat",
]

_seeds: List[str] = [
    "Taurus-orb", "Libra-orb", "Scorpio-orb", "Gemini-orb", "Capricorn-orb", "Virgo-orb",
]


def _build_item_table() -> Dict[str, ItemData]:
    table: Dict[str, ItemData] = {}
    code = ITEM_ID_START

    def add(names: List[str], itype: ItemType, classification: ItemClassification) -> None:
        nonlocal code
        for n in names:
            table[n] = ItemData(code, itype, classification)
            code += 1

    for name, classification in _key_items:
        table[name] = ItemData(code, ItemType.KEY, classification)
        code += 1

    add(_grand_items, ItemType.GRAND, ItemClassification.progression)
    add(_library_items, ItemType.LIBRARY, ItemClassification.useful)
    add(_misc_items, ItemType.MISC, ItemClassification.useful)
    add(_weapons, ItemType.WEAPON, ItemClassification.useful)
    add(_shields, ItemType.SHIELD, ItemClassification.useful)
    add(_bodies, ItemType.BODY, ItemClassification.useful)
    add(_heads, ItemType.HEAD, ItemClassification.useful)
    add(_accessories, ItemType.ACCESSORY, ItemClassification.useful)
    add(_specials, ItemType.SPECIAL, ItemClassification.useful)
    add(_medicine, ItemType.MEDICINE, ItemClassification.filler)
    add(_seeds, ItemType.SEED, ItemClassification.useful)

    # Generic padding item representing money found in the world; used to fill out the
    # remainder of the item pool once every unique item above has been placed once.
    table["Cash"] = ItemData(code, ItemType.FILLER, ItemClassification.filler)
    code += 1

    return table


item_table: Dict[str, ItemData] = _build_item_table()

# Locked event items for logic completion stuff
event_item_table: Dict[str, ItemData] = {
    "Victory": ItemData(None, ItemType.EVENT, ItemClassification.progression, True),
}

filler_item_names: List[str] = ["Cash"]
