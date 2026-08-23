from BaseClasses import ItemClassification
from .ExtraTypes import ItemData

item_table: dict[str, ItemData] = {
    # Events
    "Victory":               ItemData(999999, 0, ItemClassification.progression),

	# Progression
	"Progression":                ItemData(1000000, 0, ItemClassification.progression),

	# Filler
	"Filler":   ItemData(1200000, 0, ItemClassification.filler),

	# Traps
	"Trap":           ItemData(1300000, 0, ItemClassification.trap),

	# Other
	"Nothing":           ItemData(2000000, 0, ItemClassification.filler),
    
}
