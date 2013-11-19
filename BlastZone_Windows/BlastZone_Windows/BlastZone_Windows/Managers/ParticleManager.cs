using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Particles;

namespace BlastZone_Windows.Managers
{
    static class ParticleManager
    {
        static string effectsDirectory = "Effects";

        static Dictionary<string, MultiEmitter> multiEmitterRegistry = new Dictionary<string, MultiEmitter>();

        /// <summary>
        /// init the manager
        /// </summary>
        //ParticleManager()
        //{
        //}

        /// <summary>
        /// add a new emitter and load straight away
        /// </summary>
        /// <param name="contentmanager">game contentmanager</param>
        /// <param name="name">name of the emitter file</param>
        /// <returns>error code, 0 if fine</returns>
        static public int AddAndLoad(ContentManager contentmanager, string name)
        {
            MultiEmitter newMultiEmitter = new MultiEmitter();
            newMultiEmitter.Load(contentmanager, effectsDirectory + "\\" + name);

            multiEmitterRegistry.Add(name, newMultiEmitter);

            return 0;
        }

        /// <summary>
        /// removes an emitter
        /// </summary>
        /// <param name="name">name of the emitter</param>
        /// <returns></returns>
        static public bool Remove(string name)
        {
            multiEmitterRegistry[name].Unload();
            return multiEmitterRegistry.Remove(name);
        }

        /// <summary>
        /// completely wipes the emitter registry
        /// </summary>
        static public void UnloadAll()
        {

            foreach (MultiEmitter emitter in multiEmitterRegistry.Values)
                emitter.Unload();

            multiEmitterRegistry.Clear();
        }

        /// <summary>
        /// add an emission point for particles to come out of
        /// </summary>
        /// <param name="name">emitter name</param>
        /// <param name="pos">position to emit</param>
        /// <param name="time">time to emit</param>
        static public void AddEmissionPoint(string name, Vector2 pos, float time)
        {
            multiEmitterRegistry[name].AddEmissionPoint(pos, time);
        }

        /// <summary>
        /// add an emission point for particles to come out of, once
        /// </summary>
        /// <param name="name">emitter name</param>
        /// <param name="pos">position to emit</param>
        static public void AddEmissionPoint(string name, Vector2 pos)
        {
            multiEmitterRegistry[name].AddEmissionPoint(pos);
        }

        /// <summary>
        /// add an emission point based on grid coords
        /// </summary>
        /// <param name="name">emitter name</param>
        /// <param name="gx">grid position X</param>
        /// <param name="gy">grid position Y</param>
        /// <param name="time">time to emit</param>
        static public void AddEmissionPointFromGrid(string name, int gx, int gy, float time)
        {
            //position
            Vector2 pos = new Vector2(gx, gy) * GlobalGameData.tileSize * GlobalGameData.drawRatio;

            //Draw offset to center sprite
            Vector2 offset = new Vector2(GlobalGameData.tileSize / 2 * GlobalGameData.drawRatio, GlobalGameData.tileSize / 2 * GlobalGameData.drawRatio);

            //add the point
            AddEmissionPoint(name, pos + offset, time);
        }

        /// <summary>
        /// add an emission point based on grid coords, once
        /// </summary>
        /// <param name="name">emitter name</param>
        /// <param name="gx">grid position X</param>
        /// <param name="gy">grid position Y</param>
        static public void AddEmissionPointFromGrid(string name, int gx, int gy)
        {
            AddEmissionPointFromGrid(name, gx, gy, 0.0f);
        }

        /// <summary>
        /// update all the emitters
        /// </summary>
        /// <param name="gametime">game gametime</param>
        static void Update(GameTime gametime)
        {
            foreach (MultiEmitter emitter in multiEmitterRegistry.Values)
                emitter.Update(gametime);
        }

        /// <summary>
        /// draw all the particles
        /// </summary>
        /// <param name="spritebatch">game spritebatch, DONT PRE START</param>
        static void Draw(SpriteBatch spritebatch)
        {
            foreach (MultiEmitter emitter in multiEmitterRegistry.Values)
                emitter.Draw(spritebatch);
        }

    }
}
