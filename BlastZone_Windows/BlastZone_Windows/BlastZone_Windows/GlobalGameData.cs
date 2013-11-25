using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO.IsolatedStorage;
using System.IO;

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
        public static float SFXVolume = 1f;
        public static float MusicVolume = 0.8f;

        public static bool IsInBounds(int gx, int gy)
        {
            return (gx >= 0 && gy >= 0 && gx < gridSizeX && gy < gridSizeY);
        }

        public static Random rand = new Random();

        public static void SaveSettings()
        {
            FileStream storageStream;

#if XBOX360
            IsolatedStorageFile isolatedFile = IsolatedStorageFile.GetUserStoreForApplication();

            storageStream = new IsolatedStorageFileStream("blastzoneConfig", FileMode.Create, isolatedFile);
#else
            storageStream = new FileStream("blastzoneConfig", FileMode.Create);
#endif
            StreamWriter writer = new StreamWriter(storageStream);
            writer.WriteLine(SFXVolume);
            writer.WriteLine(MusicVolume);
            writer.WriteLine(LowQualityParticles);
            writer.Close();
            writer.Dispose();
        }

        public static void LoadSettings()
        {
            FileStream storageStream;

#if XBOX360
            IsolatedStorageFile isolatedFile = IsolatedStorageFile.GetUserStoreForApplication();

            try
            {
                storageStream = new IsolatedStorageFileStream("blastzoneConfig", FileMode.Open, isolatedFile);
            }
            catch (Exception ex)
            {
                //No file found to load settings, return
                return;
            }

#else
            try
            {
            storageStream = new FileStream("blastzoneConfig", FileMode.Open);
            }
            catch (FileNotFoundException)
            {
                //No file found to load settings, return
                return;
            }
#endif
            StreamReader reader = new StreamReader(storageStream);
            SFXVolume = Convert.ToSingle(reader.ReadLine());
            MusicVolume = Convert.ToSingle(reader.ReadLine());

#if XBOX360
            LowQualityParticles = true;
#else
            LowQualityParticles = Convert.ToBoolean(reader.ReadLine());
#endif
            reader.Close();
            reader.Dispose();
        }
    }
}
