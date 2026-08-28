BASE_ID = 3_690_000

ITEM_ID_START = BASE_ID
LOCATION_ID_START = BASE_ID + 10_000

PROGRESSIVE_ZONE_STEPS = ["Leo-card", "Cancer-card", "Pisces-card", "Aquarius-card", "Sagittarius-card"]
NUM_PROGRESSIVE_ZONE_STEPS = len(PROGRESSIVE_ZONE_STEPS)

ZODIAC_BOSS_NAMES = ["Source", "Maldicion", "Psalmanazar & Herodotus", "Justus", "Carnival", "Cob"]
NUM_MAIN_ZODIACS = len(ZODIAC_BOSS_NAMES)

#Regarding this I decided to save some slots for shopsanity even though only some are
# used in a given seed depending on shop_checks/shopsanity
MAX_SHOP_SLOTS_PER_SHOP = 10
SHOP_NAMES = ["Zone 1 Shop", "Zone 2 Shop", "Zone 3 Shop"]

MACGUFFIN_MAX = 99

# Stuff we can turn OFF(pun intended) just so we can test the client better
FEATURE_PROGRESSIVE_ZONES = True         # progressive_zones option; forced to individual zone cards when off
FEATURE_PROGRESSIVE_EQUIPMENT = True     # progressive_equipment option; forced to flat equipment when off
FEATURE_ZONES_ARE_UNLOCKS = True         # zones_are_unlocks option / Purified Key items; forced off (direct purified-zone access)
FEATURE_SHOPSANITY = True                # shopsanity / shop_checks; forced to 0 shop locations when off
FEATURE_MACGUFFIN_HUNT = True            # macguffin_hunt goal option; forced back to the standard goal when off

# Subsystems added this session:
FEATURE_ADD_ON_RANDOMIZATION = True      # Alpha/Omega/Epsilon as real checks/items (world.py, regions.py)
FEATURE_COMPETENCE_RANDOMIZATION = True  # the randomize_competences option
FEATURE_COMBAT_ASSISTANCE = True         # the grinding_npc / enemy_scaling combat_assistance choices
FEATURE_COMBAT_POWER_LOGIC = True        # OffLogic power score gating Sugar; forced to the old Grand-only check when off
FEATURE_WEIGHTED_FILLER = True           # weighted *FillerWeight filler pool; forced to plain Cash filler when off
