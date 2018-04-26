﻿namespace Constants {

    public static class UIValues {
        public const string SELECTEDGROUP = "_selected";
        public const string HOVERGROUP = "_hover";
        public const string PERSISTANCEGROUP = "_persistance";
        public const string BUTTONSUFFIX = "_BTN";
        public const string UISUFFIX = "_UI";
        public const string TEXTSUFFIX = "_TEXT";
        public const string SPRITESUFFIX = "_SP";

        public static class Structure {
            public const string CASTLEUI = "Castle" + UISUFFIX;
            public const string SPAWNGROUP = "_spawn";
            public const string SPAWNBUTTON = "Spawn" + BUTTONSUFFIX;
            public const string BACKBUTTON = "Back" + BUTTONSUFFIX;
            public const string SPAWNWARRIOR = "Warrior" + BUTTONSUFFIX;
            public const string SPAWNMAGE = "Mage" + BUTTONSUFFIX;
            public const string SPAWNARCHER = "Archer" + BUTTONSUFFIX;
        }

        public static class Unit {
            public const string UNITUI = "Unit" + UISUFFIX;
            public const string SELECTIONCIRCLE = "SelectionCircle" + SPRITESUFFIX;
            public const string ENDBUTTON = "End" + BUTTONSUFFIX;
            public const string ATTACKBUTTON = "Attack" + BUTTONSUFFIX;
            public const string MOVEBUTTON = "Move" + BUTTONSUFFIX;
            public const string INFOTEXT = "Info" + TEXTSUFFIX;
        }

        public static class Player {
            public const string PLAYERUI = "Player" + UISUFFIX;
            public const string ENDBUTTON = "End" + BUTTONSUFFIX;
            public const string INFOTEXT = "Info" + TEXTSUFFIX;
        }
    }
}