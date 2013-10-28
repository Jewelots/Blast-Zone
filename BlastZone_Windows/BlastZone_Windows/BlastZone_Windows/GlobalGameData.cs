using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlastZone_Windows
{
    class GlobalGameData
    {
        public const int gridSizeX = 15;
        public const int gridSizeY = 11;

        public const int tileSize = 16;
        public const int drawRatio = 3;

        public const int windowWidth = 1280;
        public const int windowHeight = 720;

        public const int levelSizeX = GlobalGameData.gridSizeX * GlobalGameData.tileSize * GlobalGameData.drawRatio;
        public const int levelSizeY = GlobalGameData.gridSizeY * GlobalGameData.tileSize * GlobalGameData.drawRatio;
    }
}
