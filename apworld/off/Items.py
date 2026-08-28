import typing
from enum import Enum, auto
from typing import Dict, List, Optional

from BaseClasses import ItemClassification

from .constants import ITEM_ID_START, NUM_PROGRESSIVE_ZONE_STEPS


class ItemType(Enum):
    KEY = auto()               # Zone cards
    PROGRESSIVE_ZONE = auto()  # Progressive zone items
    PURIFIED_KEY = auto()      # Unlocks for purified zones
    GRAND = auto()             # The 5 "Grand" objects for Zacharie's Secret Boss sidequest
    LIBRARY = auto()           # Books borrowed from the Zone 2 Library
    MISC = auto()              # Other unique key-item-flavored collectibles
    ADD_ON = auto()            # Alpha/Omega/Epsilon party-member recruitment
    COMPETENCE = auto()        # Batter/Add-On special attacks 
    WEAPON = auto()
    SHIELD = auto()
    BODY = auto()
    HEAD = auto()
    ACCESSORY = auto()
    PROGRESSIVE_EQUIP = auto()
    SPECIAL = auto()
    MEDICINE = auto()
    SEED = auto()
    MACGUFFIN = auto()
    FILLER = auto()
    EVENT = auto()


class ItemData(typing.NamedTuple):
    code: Optional[int]
    type: ItemType
    classification: ItemClassification
    event: bool = False

_individual_zone_cards: List[str] = [
    "Leo-card", "Cancer-card", "Pisces-card", "Aquarius-card", "Sagittarius-card",
]

_key_items: List[typing.Tuple[str, ItemClassification]] = [
    ("Aries-card", ItemClassification.progression),         # Allows access to the secret place
    ("Access Card", ItemClassification.progression),        # Permits access to Area 4 of Zone 3
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

# Add on names
ADD_ON_NAMES: List[str] = ["Alpha", "Omega", "Epsilon"]

#region Competences
BATTER_COMPETENCES: List[str] = [
    "Wide Angle", "Save First Base", "Run with Courage", "Furious Homerun",
    "Save Second Base", "Run with Grace", "Special Homerun", "Save Third Base",
    "Run with Dementia", "Magic Homerun", "Save Fourth Base", "Run with Belief",
    "Save Secret Base",
]

ADD_ON_COMPETENCES: Dict[str, List[str]] = {
    "Alpha": [
        "Saturated Chain", "Awaited Embrace", "Converted Chain", "Requisite Embrace",
        "Long Chain", "Open Embrace",
        # TODO
    ],
    "Omega": [
        "Inverse Perspective", "Overdone Perspective", "Optimised Blur", "Photographic Blur",
        # TODO
    ],
    "Epsilon": [
        # TODO
    ],
}

COMPETENCE_ITEM_NAMES: Dict[str, List[str]] = {
    owner: [f"{owner}: {name}" for name in names]
    for owner, names in {"Batter": BATTER_COMPETENCES, **ADD_ON_COMPETENCES}.items()
}
ALL_COMPETENCE_ITEMS: List[str] = [
    name for names in COMPETENCE_ITEM_NAMES.values() for name in names
]
#endregion

#region Equipement
_batter_weapons: List[str] = [
    "Harold Bat", "Masashi Bat", "Emmanuel Bat", "Michael Bat",
    "Yoshihiro Bat", "Lewis Bat", "Katsuhiro Bat", "Ashley Bat",
]
_addon_weapons: List[str] = [
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
_batter_body: List[str] = ["Nicolas Tunic", "David Tunic", "Min-Woo Tunic", "Canepa Tunic", "Taiyou Tunic", "Neil Tunic"]
_addon_body: List[str] = ["Radius Epidermis", "Ulna Epidermis", "Humerus Epidermis", "Tibia Epidermis", "Fibula Epidermis", "Femur Epidermis"]
_heads: List[str] = [
    "Colour of Wrath", "Colour of Pain", "Colour of Sadness", "Colour of Hatred",
    "Colour of Force", "Colour of Defeat", "Colour of Neglect", "The Eighth Colour",
]
_accessories: List[str] = [
    "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday", "Secret day",
]

# Flat-mode groups, keyed by the ItemType they're tagged with.
FLAT_EQUIPMENT_GROUPS: Dict[ItemType, List[str]] = {
    ItemType.WEAPON: _batter_weapons + _addon_weapons,
    ItemType.SHIELD: _shields,
    ItemType.BODY: _batter_body + _addon_body,
    ItemType.HEAD: _heads,
    ItemType.ACCESSORY: _accessories,
}

# Progressive-mode items: name -> number of copies placed in the pool (one per equipment tier).
PROGRESSIVE_EQUIPMENT_COUNTS: Dict[str, int] = {
    "Progressive Batter Weapon": len(_batter_weapons),
    "Progressive Add-On Weapon": len(_addon_weapons),
    "Progressive Shield": len(_shields),
    "Progressive Batter Body": len(_batter_body),
    "Progressive Add-On Body": len(_addon_body),
    "Progressive Head": len(_heads),
    "Progressive Accessory": len(_accessories),
}
#endregion

_specials: List[str] = ["Inspiration", "Expiration"]

_medicine: List[str] = [
    "Luck ticket", "Fortune ticket", "Silver flesh", "Golden flesh",
    "Joker", "Moloch's meat", "Belial's meat", "Abaddon's meat",
]
MEDICINE_ITEM_NAMES: List[str] = _medicine

_seeds: List[str] = [
    "Taurus-orb", "Libra-orb", "Scorpio-orb", "Gemini-orb", "Capricorn-orb", "Virgo-orb",
]

PURIFIED_KEY_ITEMS: List[str] = ["Zone 1 Purified Key", "Zone 2 Purified Key", "Zone 3 Purified Key"]

PROGRESSIVE_ZONE_ITEM = "Progressive Zone"

MACGUFFIN_ITEM = "MacGuffin"

# weights for combat power
POWER_WEIGHTS: Dict[ItemType, float] = {
    ItemType.WEAPON: 1.0,
    ItemType.SHIELD: 1.0,
    ItemType.BODY: 1.0,
    ItemType.HEAD: 1.0,
    ItemType.ACCESSORY: 0.5,
    ItemType.PROGRESSIVE_EQUIP: 1.0,
    ItemType.ADD_ON: 3.0,
    ItemType.COMPETENCE: 1.5,
    ItemType.GRAND: 2.0,
    ItemType.SEED: 1.0,
}


def _build_item_table() -> Dict[str, ItemData]:
    table: Dict[str, ItemData] = {}
    code = ITEM_ID_START

    def add(names: List[str], itype: ItemType, classification: ItemClassification) -> None:
        nonlocal code
        for n in names:
            table[n] = ItemData(code, itype, classification)
            code += 1

    # Both zone-card representations are reserved; only one set is actually placed in the
    # pool for a given seed, chosen in world.create_items() based on progressive_zones
    add(_individual_zone_cards, ItemType.KEY, ItemClassification.progression)
    table[PROGRESSIVE_ZONE_ITEM] = ItemData(code, ItemType.PROGRESSIVE_ZONE, ItemClassification.progression)
    code += 1

    for name, classification in _key_items:
        table[name] = ItemData(code, ItemType.KEY, classification)
        code += 1

    add(PURIFIED_KEY_ITEMS, ItemType.PURIFIED_KEY, ItemClassification.progression)
    add(_grand_items, ItemType.GRAND, ItemClassification.progression)
    add(_library_items, ItemType.LIBRARY, ItemClassification.useful)
    add(_misc_items, ItemType.MISC, ItemClassification.useful)

    # Add-Ons are progression
    add(ADD_ON_NAMES, ItemType.ADD_ON, ItemClassification.progression)

    # Competences are reserved unconditionally
    add(ALL_COMPETENCE_ITEMS, ItemType.COMPETENCE, ItemClassification.useful)

    # Flat equipment (used when progressive_equipment is off)
    add(FLAT_EQUIPMENT_GROUPS[ItemType.WEAPON], ItemType.WEAPON, ItemClassification.useful)
    add(FLAT_EQUIPMENT_GROUPS[ItemType.SHIELD], ItemType.SHIELD, ItemClassification.useful)
    add(FLAT_EQUIPMENT_GROUPS[ItemType.BODY], ItemType.BODY, ItemClassification.useful)
    add(FLAT_EQUIPMENT_GROUPS[ItemType.HEAD], ItemType.HEAD, ItemClassification.useful)
    add(FLAT_EQUIPMENT_GROUPS[ItemType.ACCESSORY], ItemType.ACCESSORY, ItemClassification.useful)

    # Progressive equipment (used when progressive_equipment is on).
    for name in PROGRESSIVE_EQUIPMENT_COUNTS:
        table[name] = ItemData(code, ItemType.PROGRESSIVE_EQUIP, ItemClassification.useful)
        code += 1

    add(_specials, ItemType.SPECIAL, ItemClassification.useful)
    add(_medicine, ItemType.MEDICINE, ItemClassification.filler)
    add(_seeds, ItemType.SEED, ItemClassification.useful)

    # macguffin_hunt goal item; quantity is decided per-seed in world.create_items().
    table[MACGUFFIN_ITEM] = ItemData(code, ItemType.MACGUFFIN, ItemClassification.progression)
    code += 1

    # Generic padding item representing money found in the world; used to fill out the
    # remainder of the item pool once everything else has been placed.
    table["Cash"] = ItemData(code, ItemType.FILLER, ItemClassification.filler)
    code += 1

    return table


item_table: Dict[str, ItemData] = _build_item_table()

event_item_table: Dict[str, ItemData] = {
    "Victory": ItemData(None, ItemType.EVENT, ItemClassification.progression, True),
    "Dedan Defeated": ItemData(None, ItemType.EVENT, ItemClassification.progression, True),
    "Japhet Defeated": ItemData(None, ItemType.EVENT, ItemClassification.progression, True),
    "Enoch Defeated": ItemData(None, ItemType.EVENT, ItemClassification.progression, True),
    "Source Defeated": ItemData(None, ItemType.EVENT, ItemClassification.progression, True),
    "Maldicion Defeated": ItemData(None, ItemType.EVENT, ItemClassification.progression, True),
    "Psalmanazar & Herodotus Defeated": ItemData(None, ItemType.EVENT, ItemClassification.progression, True),
    "Justus Defeated": ItemData(None, ItemType.EVENT, ItemClassification.progression, True),
    "Carnival Defeated": ItemData(None, ItemType.EVENT, ItemClassification.progression, True),
    "Cob Defeated": ItemData(None, ItemType.EVENT, ItemClassification.progression, True),
    "Sugar Defeated": ItemData(None, ItemType.EVENT, ItemClassification.progression, True),
}

filler_item_names: List[str] = ["Cash", *MEDICINE_ITEM_NAMES]
