using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlastZone_Windows
{
    /// <summary>
    /// a basic particle system
    /// made by Paul Hancock
    /// </summary>
    class Emitter
    {
        
        //vars
        private List<Particle> lParticles;
        private float fTimeToNext;

        private Particle pParticleTemplate;

        //private Texture2D tParticleTexture;

        public Vector2 vPosition;

        public Vector2 vPower;
        public float fEmitRate;
        public int iEmitCount;

        BlendState blendState = BlendState.AlphaBlend;

        /// <summary>
        /// default entry
        /// </summary>
        private Emitter() 
        {
            fTimeToNext = 0.0f;

            lParticles = new List<Particle>();

            vPosition = new Vector2();

            //template
            //pParticleTemplate = new Particle();
            //pParticleTemplate.fLifeLeft = 3.0f;
            //pParticleTemplate.vPosition = new Vector2();
            //pParticleTemplate.vVelocity = new Vector2();
            //pParticleTemplate.vAcceleration = new Vector2(0.0f, -98.0f);
            //pParticleTemplate.vDecay = new Vector2(1.0f, 1.0f);

        }

        /// <summary>
        /// create a particle emmitter
        /// </summary>
        /// <param name="template">the template particle to use</param>
        /// <param name="pos">the start position of the emmitter</param>
        /// <param name="pow">the emmission power vector to use</param>
        /// <param name="rate">the rate to use</param>
        /// <param name="count">the count of particles per emmission</param>
        public Emitter(Particle template, Vector2 pos, Vector2 pow, float rate = 0.0f, int count = 1) : this()
        {
            //tParticleTexture = tex;
            pParticleTemplate = new Particle(template);
            vPosition = pos;
            vPower = pow;
            fEmitRate = rate;
            iEmitCount = count;
        }

        //public ~Emitter() { }

        //update
        /// <summary>
        /// update the particles
        /// </summary>
        /// <param name="gametime">the gametime from the xna framework</param>
        public void Update(GameTime gametime)
        {
            fTimeToNext += (float)gametime.ElapsedGameTime.TotalSeconds;

            float rate = 1.0f / fEmitRate;

            while (fTimeToNext >= rate)
            {
                fTimeToNext -= rate;

                Emit();

            }

            //we dont want too many particles!
            //CutParticles(10000);

            //update particles
            foreach (Particle particle in lParticles)
                particle.Update(gametime);


            //cull dead particles
            List<Particle> removeList = new List<Particle>();
            foreach (Particle particle in lParticles)
                if (particle.fLifeLeft <= 0.0f)
                    removeList.Add(particle);

            foreach (Particle particle in removeList)
                lParticles.Remove(particle);

        }

        //draw
        /// <summary>
        /// draw the particles
        /// </summary>
        /// <param name="spriteBatch">the spritebatch used in rendering</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, blendState);
            
            foreach(Particle particle in lParticles)
                particle.Draw(spriteBatch);

            spriteBatch.End();
        }

        //Emit particle/s, we call this manually for 0.0f rate or in an event (eg; confetti)
        /// <summary>
        /// manual emmission
        /// </summary>
        /// <param name="customAmount">custom amount to emmitt, default will use already defined amount</param>
        public void Emit(int customAmount = 0)
        {
            Random rand = new Random();
            int count;
            if (customAmount <= 0)
                count = iEmitCount;
            else 
                count = customAmount;

            for (int i = 0; i < count; ++i)
            {
                Particle newParticle = new Particle(pParticleTemplate);

                newParticle.vPosition = vPosition;

                double angle = Math.PI * 2.0 * rand.NextDouble();
                float scale = (float)rand.NextDouble();

                newParticle.vVelocity.X = (float)Math.Sin(angle) * scale * vPower.X;
                newParticle.vVelocity.Y = (float)Math.Cos(angle) * scale * vPower.Y;

                lParticles.Add(newParticle);
            }
        }

        //cut excessive particles so we dont have too many, returns difference
        /// <summary>
        /// cut the excess particles
        /// </summary>
        /// <param name="maxNum">maximum amount</param>
        /// <returns>amount cut, or headroom if negative</returns>
        public int CutParticles(int maxNum)
        {
            int diff = lParticles.Count - maxNum;

            if (diff > 0)
                lParticles.RemoveRange(0, diff);

            return diff;
        }



    }


    //base particle class
    class Particle
    {
        //vars
        public float fLifeLeft;

        public Vector2 vPosition;
        public Vector2 vVelocity;
        public Vector2 vAcceleration;
        public Vector2 vDecay;

        public Texture2D tTexture;

        //construct
        /// <summary>
        /// create particle with default values
        /// </summary>
        public Particle()
        {
            fLifeLeft = 0.0f;
            vPosition = new Vector2();
            vVelocity = new Vector2();
            vAcceleration = new Vector2();
            vDecay = new Vector2();
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="p">existing particle to copy</param>
        public Particle(Particle p)
        {
            fLifeLeft = p.fLifeLeft;
            vPosition = p.vPosition;
            vVelocity = p.vVelocity;
            vAcceleration = p.vAcceleration;
            vDecay = p.vDecay;
            tTexture = p.tTexture;
        }

        //public ~Particle() { }

        //update
        /// <summary>
        /// update particle
        /// </summary>
        /// <param name="gametime">gametime from xna</param>
        public void Update(GameTime gametime)
        {
            fLifeLeft -= (float)gametime.ElapsedGameTime.TotalSeconds;
            vVelocity -= (vVelocity * vDecay) * (float)gametime.ElapsedGameTime.TotalSeconds;
            vVelocity += vAcceleration * (float)gametime.ElapsedGameTime.TotalSeconds;
            vPosition += vVelocity * (float)gametime.ElapsedGameTime.TotalSeconds;
        }

        //draw
        /// <summary>
        /// draw particle
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tTexture, vPosition, Color.White);
        }

    }
}
