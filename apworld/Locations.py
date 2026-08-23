from .ExtraTypes import LocationData

def fill_locations():
    predefined_locations: dict[str, LocationData] = {
        "Zone 0 - Outdoors - Left Chest":                LocationData(1000000, "Zone 0"),
        "Zone 0 - Outdoors - Right Chest":               LocationData(1000001, "Zone 0"),

        "Zone 1 - Elsen - Promotion Elsen Chest":        LocationData(1000002, "Zone 1"),
        "Zone 1 - Elsen - Alpha":                        LocationData(1000003, "Zone 1"),
        "Zone 1 - Elsen - Hidden Chest":                 LocationData(1000004, "Zone 1"),
        "Zone 1 - Mines Entrance - Pillar Art Chest":    LocationData(1000005, "Zone 1"),
        "Zone 1 - Mines First Maze - Left Chest":        LocationData(1000006, "Zone 1"),
        "Zone 1 - Mines First Maze - Right Chest":       LocationData(1000007, "Zone 1"),
        "Zone 1 - Mines Safe Room - Chest":              LocationData(1000008, "Zone 1"),
        "Zone 1 - Mines Orb Room - Virgo-Orb Chest":     LocationData(1000009, "Zone 1"),
        "Zone 1 - Mines Orb Room - Scorpio-Orb Chest":   LocationData(1000010, "Zone 1"),
        "Zone 1 - Mines Orb Room - Capricorn-Orb Chest": LocationData(1000011, "Zone 1"),
        "Zone 1 - Mines Orb Room - Libra-Orb Chest":     LocationData(1000012, "Zone 1"),
        "Zone 1 - Mines Orb Room - Gemini-Orb Chest":    LocationData(1000013, "Zone 1"),
        "Zone 1 - Mines Orb Room - Taurus-Orb Chest":    LocationData(1000014, "Zone 1"),
        "Zone 1 - Mines Dark Maze - First Chest":        LocationData(1000015, "Zone 1"),
        "Zone 1 - Mines Dark Maze - Top Chest":          LocationData(1000016, "Zone 1"),
        "Zone 1 - Mines Dark Maze - Right Chest":        LocationData(1000017, "Zone 1"),
        "Zone 1 - Mines Shop 1":                         LocationData(1000018, "Zone 1"),
        "Zone 1 - Mines Shop 2":                         LocationData(1000019, "Zone 1"),
        "Zone 1 - Mines Shop 3":                         LocationData(1000020, "Zone 1"),
        "Zone 1 - Mines Shop 4":                         LocationData(1000021, "Zone 1"),
        "Zone 1 - Floor 2584 - Left Chest":              LocationData(1000022, "Zone 1"),
        "Zone 1 - Floor 2584 - Right Chest":             LocationData(1000023, "Zone 1"),
        "Zone 1 - Floor 10258 - Chest":                  LocationData(1000024, "Zone 1"),
        "Zone 1 - Floor 10258 Area 3/4 - Chest":         LocationData(1000025, "Zone 1"),
        "Zone 1 - Alma First Room - Shop 1":             LocationData(1000026, "Zone 1"),
        "Zone 1 - Alma First Room - Shop 2":             LocationData(1000027, "Zone 1"),
        "Zone 1 - Alma First Room - Shop 3":             LocationData(1000028, "Zone 1"),
        "Zone 1 - Alma First Room - Shop 4":             LocationData(1000029, "Zone 1"),
        "Zone 1 - Alma Bottom Right Room - Chest":       LocationData(1000030, "Zone 1"),
        "Zone 1 - Alma Before Dedan - Chest":            LocationData(1000031, "Zone 1"),

        "Zone 2 - Library Second Floor - Chest":         LocationData(1000032, "Zone 2"),
        "Zone 2 - Library Third Floor - Left Chest":     LocationData(1000033, "Zone 2"),
        "Zone 2 - Library Third Floor - Right Chest":    LocationData(1000034, "Zone 2"),
        "Zone 2 - Library Second Floor - Chest":         LocationData(1000035, "Zone 2"),
        "Zone 2 - Library Second Floor - Chest":         LocationData(1000036, "Zone 2"),
        "Zone 2 - Library Second Floor - Chest":         LocationData(1000037, "Zone 2"),
    }

    print(predefined_locations)
    return predefined_locations

location_table = fill_locations()
