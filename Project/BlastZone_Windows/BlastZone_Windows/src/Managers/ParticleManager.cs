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
    /// <summary>
    /// static manager class for particle emitters in the game
    /// </summary>
    static class ParticleManager
    {
        static string effectsDirectory = "Effects";

        static Dictionary<string, MultiEmitter> multiEmitterRegistry = new Dictionary<string, MultiEmitter>();


        /// <summary>
        /// add a new emitter and load straight away
        /// </summary>
        /// <param name="contentmanager">game contentmanager</param>
        /// <param name="name">name of the emitter file</param>
        /// <returns>error code, 0 if fine</returns>
        static public int AddAndLoad(ContentManager contentmanager, string emitter)
        {
            MultiEmitter newMultiEmitter = new MultiEmitter();
            newMultiEmitter.Load(contentmanager, effectsDirectory + "\\" + emitter);

            multiEmitterRegistry.Add(emitter, newMultiEmitter);

            return 0;
        }

        /// <summary>
        /// removes an emitter
        /// </summary>
        /// <param name="name">name of the emitter</param>
        /// <returns></returns>
        static public bool Remove(string emitter)
        {
            multiEmitterRegistry[emitter].Unload();
            return multiEmitterRegistry.Remove(emitter);
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
        static public void AddEmissionPoint(string emitter, Vector2 pos, float time)
        {
            multiEmitterRegistry[emitter].AddEmissionPoint(pos, time);
        }

        /// <summary>
        /// add an emission point for particles to come out of, once
        /// </summary>
        /// <param name="name">emitter name</param>
        /// <param name="pos">position to emit</param>
        static public void AddEmissionPoint(string emitter, Vector2 pos)
        {
            multiEmitterRegistry[emitter].AddEmissionPoint(pos);
        }

        /// <summary>
        /// add an emission point based on grid coords
        /// </summary>
        /// <param name="name">emitter name</param>
        /// <param name="gx">grid position X</param>
        /// <param name="gy">grid position Y</param>
        /// <param name="time">time to emit</param>
        static public void AddEmissionPointFromGrid(string emitter, int gx, int gy, float time)
        {
            //add the point
            AddEmissionPoint(emitter, GetCoordsForGrid(gx, gy), time);
        }

        /// <summary>
        /// add an emission point based on grid coords, once
        /// </summary>
        /// <param name="name">emitter name</param>
        /// <param name="gx">grid position X</param>
        /// <param name="gy">grid position Y</param>
        static public void AddEmissionPointFromGrid(string emitter, int gx, int gy)
        {
            AddEmissionPointFromGrid(emitter, gx, gy, 0.0f);
        }

        /// <summary>
        /// emit from the positions
        /// </summary>
        /// <param name="emitter">emitter name</param>
        static public void EmitAll(string emitter)
        {
            multiEmitterRegistry[emitter].EmitAll();
        }

        /// <summary>
        /// emit from the emitter's own position
        /// </summary>
        /// <param name="emitter">emitter name</param>
        static public void Emit(string emitter)
        {
            multiEmitterRegistry[emitter].Emit();
        }

        /// <summary>
        /// emit particles from position
        /// </summary>
        /// <param name="emitter">emitter name</param>
        /// <param name="emitPos">position</param>
        static public void Emit(string emitter, Vector2 emitPos)
        {
            multiEmitterRegistry[emitter].Emit(emitPos);
        }

        /// <summary>
        /// emit particles from grid position
        /// </summary>
        /// <param name="emitter">emitter name</param>
        /// <param name="gx">grid X</param>
        /// <param name="gy">grid Y</param>
        static public void EmitFromGridPosition(string emitter, int gx, int gy)
        {
            multiEmitterRegistry[emitter].Emit(GetCoordsForGrid(gx, gy));
        }

        /// <summary>
        /// gets whether the emitter emits from its own point
        /// </summary>
        /// <param name="emitter">emitter name</param>
        /// <returns>the emitfrompos value</returns>
        static public bool GetEmitFromSelf(string emitter)
        {
            return multiEmitterRegistry[emitter].EmitFromSelf;
        }

        /// <summary>
        /// sets the emitter emitfromself
        /// </summary>
        /// <param name="emitter">emitter name</param>
        /// <param name="value">emitfromself value</param>
        static public void SetEmitFromSelf(string emitter, bool value)
        {
            multiEmitterRegistry[emitter].EmitFromSelf = value;
        }

        /// <summary>
        /// gets the emitter's position
        /// </summary>
        /// <param name="emitter">emitter name</param>
        /// <returns>emitter's Vector2</returns>
        static public Vector2 GetPosition(string emitter)
        {
            return multiEmitterRegistry[emitter].Position;
        }

        /// <summary>
        /// sets the emitter's position
        /// </summary>
        /// <param name="emitter">emitter name</param>
        /// <param name="pos">Vcector2 position</param>
        static public void SetPosition(string emitter, Vector2 pos)
        {
            multiEmitterRegistry[emitter].Position = pos;
        }

        /// <summary>
        /// update all the emitters
        /// </summary>
        /// <param name="gametime">game gametime</param>
        static public void Update(GameTime gametime)
        {
            foreach (MultiEmitter emitter in multiEmitterRegistry.Values)
                emitter.Update(gametime);
        }

        /// <summary>
        /// draw all the particles
        /// </summary>
        /// <param name="spritebatch">game spritebatch, DONT PRE START</param>
        static public void Draw(SpriteBatch spritebatch)
        {
            foreach (MultiEmitter emitter in multiEmitterRegistry.Values)
                emitter.Draw(spritebatch);
        }

        /// <summary>
        /// calculates the Vector2 from grid coords
        /// </summary>
        /// <param name="gx">grid X</param>
        /// <param name="gy">grid Y</param>
        /// <returns>position Vector2</returns>
        static Vector2 GetCoordsForGrid(int gx, int gy)
        {

            //position
            Vector2 pos = new Vector2(gx, gy) * GlobalGameData.tileSize * GlobalGameData.drawRatio;

            //Draw offset to center sprite
            Vector2 offset = new Vector2(GlobalGameData.tileSize / 2 * GlobalGameData.drawRatio, GlobalGameData.tileSize / 2 * GlobalGameData.drawRatio);

            return pos + offset;
        }

        /// <summary>
        /// Clear all particles
        /// </summary>
        static public void ClearAll()
        {
            foreach (MultiEmitter emitter in multiEmitterRegistry.Values)
            {
                emitter.ClearAllParticles();
            }
        }
    }
}
