from BaseClasses import Tutorial
from worlds.AutoWorld import WebWorld
from Options import OptionGroup, DeathLink

from .options import (
    Goal, MacGuffinAmount, MacGuffinRequirement, ProgressiveEquipment, ProgressiveZones,
    PureZonesAreUnlocks, DeprioritizeCarnival, DeprioritizeCob, ShopChecks, Shopsanity,
    CombatAssistance, ExperienceMultiplier, RandomizeCompetences,
    CashFillerWeight, LuckTicketFillerWeight, FortuneTicketFillerWeight, SilverFleshFillerWeight,
    GoldenFleshFillerWeight, JokerFillerWeight, MolochsMeatFillerWeight, BelialsMeatFillerWeight,
    AbaddonsMeatFillerWeight,
)


class OFFWeb(WebWorld):
    theme = "grass"
    tutorials = [
        Tutorial(
            "Multiworld Setup Guide",
            "A guide to setting up OFF for Archipelago multiworld games.",
            "English",
            "setup_en.md",
            "setup/en",
            ["WestCraft15", "Lyxn"]
        )
    ]

    option_groups = [
        OptionGroup("Goal", [
            Goal,
            MacGuffinAmount,
            MacGuffinRequirement,
        ]),
        OptionGroup("World Settings", [
            ProgressiveEquipment,
            ProgressiveZones,
            PureZonesAreUnlocks,
        ]),
        OptionGroup("Sanities", [
            ShopChecks,
            Shopsanity,
            DeprioritizeCarnival,
            DeprioritizeCob,
        ]),
        OptionGroup("Combat Assistance", [
            CombatAssistance,
            ExperienceMultiplier,
            RandomizeCompetences,
        ]),
        OptionGroup("Filler Items", [
            CashFillerWeight,
            LuckTicketFillerWeight,
            FortuneTicketFillerWeight,
            SilverFleshFillerWeight,
            GoldenFleshFillerWeight,
            JokerFillerWeight,
            MolochsMeatFillerWeight,
            BelialsMeatFillerWeight,
            AbaddonsMeatFillerWeight,
        ], start_collapsed=True),
        OptionGroup("Death Link", [
            DeathLink,
        ]),
    ]
