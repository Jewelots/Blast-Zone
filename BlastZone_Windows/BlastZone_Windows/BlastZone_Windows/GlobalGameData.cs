using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlastZone_Windows
{
    /// <summary>
    /// Encapsulates global game data that's never changed and is accessed a lot
    /// </summary>
    class GlobalGameData
    {
        //Size of the window in pixels
        public const int windowWidth = 1280;
        public const int windowHeight = 720;

        //Grid width/height in tiles
        public const int gridSizeX = 15;
        public const int gridSizeY = 11;

        //Tile size in pixels
        public const int tileSize = 16;

        //Scale ratio to draw tiles at
        public const int drawRatio = 3;

        //Calculate physical level width/height in pixels for easy use later
        public const int levelSizeX = GlobalGameData.gridSizeX * GlobalGameData.tileSize * GlobalGameData.drawRatio;
        public const int levelSizeY = GlobalGameData.gridSizeY * GlobalGameData.tileSize * GlobalGameData.drawRatio;

        //Low quality particle toggle
        #if XBOX360
        public static bool LowQualityParticles = true;
        #else
        public static bool LowQualityParticles = false;
        #endif

        //Volume levels
        //Add loading from file?
        public static float SFXVolume = 0.8f;
        public static float MusicVolume = 0.8f;

        public static bool IsInBounds(int gx, int gy)
        {
            return (gx >= 0 && gy >= 0 && gx < gridSizeX && gy < gridSizeY);
        }

        public static Random rand = new Random();
    }
}
