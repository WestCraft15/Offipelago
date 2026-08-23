from .Options import OffOptions
from .ExtraTypes import OffItem, OffLocation
from .Items import item_table
from .Locations import location_table
from .Regions import region_table
from worlds.AutoWorld import World
from BaseClasses import Entrance, Item, ItemClassification, Region

class OffWorld(World):
    """Off for Archipelago."""
    game = "Off"
    options_dataclass = OffOptions
    options: OffOptions # type: ignore # typing hints for option results

    # The following two dicts are required for the generation to know which
    # items exist. They could be generated from json or something else. They can
    # include events, but don't have to since events will be placed manually.
    item_name_to_id = {name: data.id for name, data in item_table.items()}
    location_name_to_id = {name: data.id for name, data in location_table.items()}
    
    def create_regions(self):
        def CreateRegion(region_name: str, exits: list[str]):
            region = Region(region_name, self.player, self.multiworld)
            region.locations += (OffLocation(self.player, name, location.id, region) for name, location in location_table.items() if location.region == region_name)
            region.exits += (Entrance(self.player, exit, region) for exit in exits)
            return region
        self.multiworld.regions += [CreateRegion(name, exits) for name, exits in region_table.items()]

    def create_items(self):
        # Modified from Undertale's create_items()
        self.multiworld.get_location("Zone 1 - Mines First Maze - Right Chest", self.player).place_locked_item(self.create_item("Victory"))
        self.multiworld.completion_condition[self.player] = lambda state: state.has("Victory", self.player)

        itempool = []

        for name, item in item_table.items():
            if item.classification is not None:
                base = item.base_amount
                
                itempool += [name] * base
                        

        # Convert itempool into real items
        itempool = [item for item in map(lambda name: self.create_item(name), itempool)]
        # Fill remaining items with randomly generated junk
        while len(itempool) < len(self.multiworld.get_unfilled_locations(self.player)):
            itempool.append(self.create_filler())

        self.multiworld.itempool += itempool
    
    def get_filler_item_name(self) -> str:
        return "Filler"

    def create_item(self, name: str) -> Item:
        item_data = item_table[name]
        item = OffItem(name, item_data.classification or ItemClassification.filler, item_data.id, self.player)
        return item

    def connect_entrances(self):
        for exits in region_table.values():
            for e in exits:
                self.multiworld.get_entrance(e, self.player).connect(self.multiworld.get_region(e, self.player))
