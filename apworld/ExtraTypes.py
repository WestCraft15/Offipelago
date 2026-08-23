import typing

from BaseClasses import Location, Item, ItemClassification

class RegionData(typing.NamedTuple):
    name: str
    exits: list[str]


class LocationData(typing.NamedTuple):
    id: int
    region: str


class OffLocation(Location):
    game = "Off"


class ItemData(typing.NamedTuple):
    id: int
    base_amount: int
    classification: ItemClassification | None


class OffItem(Item):
    game = "Off"
