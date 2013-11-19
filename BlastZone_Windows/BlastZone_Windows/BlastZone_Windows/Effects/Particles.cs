using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using System.Xml;
using System.Xml.Linq;
using System.IO;

using System.Threading;

using VectorGraphs;

/// <summary>
/// a basic particle system
/// made by Paul Hancock
/// </summary>
namespace Particles
{
    /// <summary>
    /// multitype emitter, uses an XML file98
    /// </summary>
    class MultiEmitter
    {
        //variables
        private Vector2 position;

        private bool emitFromSelf;
        /// <summary>
        /// position of the emitter
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                foreach (Emitter e in EmitterList)
                    e.position = e.origin + position;
            }
        }

        /// <summary>
        /// whether you want the emitters to emit at their own position
        /// </summary>
        public bool EmitFromSelf
        {
            get { return emitFromSelf; }
            set 
            { 
                foreach (Emitter e in EmitterList) e.emitFromSelf = value; 
                emitFromSelf = value; 
            }
        }

        private List<Emitter> EmitterList;

        //functions

        /// <summary>
        /// create the emitter
        /// </summary>
        public MultiEmitter()
        {
            position = new Vector2();
            EmitterList = new List<Emitter>();
            emitFromSelf = false;
        }

        /// <summary>
        /// load emitter data
        /// </summary>
        /// <param name="contentManager">the game ContentManager</param>
        /// <param name="xmlFile">the xml file</param>
        /// <returns>error code, 0 is no error</returns>
        public int Load(ContentManager contentManager, string xmlFile)
        {

            Dictionary<string, Texture2D> loadedTextures = new Dictionary<string, Texture2D>();
            Dictionary<string, VectorGraph> loadedGraphs = new Dictionary<string, VectorGraph>();

            Dictionary<string, Particle> loadedParticles = new Dictionary<string, Particle>();

            XElement document = XElement.Load(Path.Combine(contentManager.RootDirectory, xmlFile) + ".xml");

            int end = xmlFile.LastIndexOf('\\') + 1;
            string directory = xmlFile.Remove(end, xmlFile.Length - end);

            //XElement multiEmitter = document.Element("MultiEmitter");


            /////////////////////////////
            //Textures
            #region Textures

            foreach (XElement texture in document.Element("Textures").Elements())
            {
                Texture2D newTexture = contentManager.Load<Texture2D>(directory + texture.Attribute("texture").Value);
                loadedTextures.Add(texture.Name.LocalName, newTexture);
            }

            #endregion

            /////////////////////////////
            //Graphs
            #region Graphs

            foreach (XElement graph in document.Element("Graphs").Elements())
            {
                //make new graph
                VectorGraph newGraph;

                switch (graph.Attribute("type").Value)
                {
                    case "Basic":
                        newGraph = new BasicVectorGraph();
                        break;
                    case "Bezier":
                        newGraph = new BezierVectorGraph();
                        break;
                    default:
                        newGraph = new EmptyVectorGraph();
                        break;
                }

                //read through points in graph
                foreach (XElement point in graph.Elements())
                {
                    double x, y;
                    if (double.TryParse(point.Attribute("x").Value, out x) && 
                        double.TryParse(point.Attribute("y").Value, out y))
                    {
                        newGraph.AddPoint(new Vector2((float)x, (float)y));
                    }
                }

                //add graph to list
                loadedGraphs.Add(graph.Name.LocalName, newGraph);
            }

            #endregion

            /////////////////////////////
            //Particles
            #region Particles

            foreach (XElement particle in document.Element("Particles").Elements())
            {
                //make new
                Particle newParticle = new Particle();

                //read through attributes

                double angle;
                double length;
                double scale;
                int r, g, b, a;
                double x, y;

                //texture
                loadedTextures.TryGetValue(particle.Element("Texture").Attribute("texture").Value, out newParticle.texture);

                //life length
                if (double.TryParse(particle.Element("Life").Attribute("length").Value, out length))
                {
                    newParticle.lifeLength = (float)length;
                }

                //size scale and graph
                if (double.TryParse(particle.Element("Size").Attribute("scale").Value, out scale))
                {
                    newParticle.SetPixelScale((float)scale);
                }

                if (particle.Element("Size").Attribute("graph").Value != "")
                {
                    loadedGraphs.TryGetValue(particle.Element("Size").Attribute("graph").Value, out newParticle.sizeGraph);
                }

                //colour and graph
                if (int.TryParse(particle.Element("Colour").Attribute("r").Value, out r) &&
                    int.TryParse(particle.Element("Colour").Attribute("g").Value, out g) &&
                    int.TryParse(particle.Element("Colour").Attribute("b").Value, out b) &&
                    int.TryParse(particle.Element("Colour").Attribute("a").Value, out a))
                {
                    newParticle.colour = new Color(r, g, b, a);
                }

                if (particle.Element("Colour").Attribute("graph").Value != "")
                {
                    loadedGraphs.TryGetValue(particle.Element("Colour").Attribute("graph").Value, out newParticle.colourGraph);
                }

                // angle and graph
                //if (double.TryParse(particle.Element("Rotation").Attribute("angle").Value, out angle))
                //{
                //    newParticle.rotation = (float)angle;
                //}

                if (particle.Element("Rotation").Attribute("graph").Value != "")
                {
                    loadedGraphs.TryGetValue(particle.Element("Rotation").Attribute("graph").Value, out newParticle.rotationGraph);
                }

                //acceleration
                if (double.TryParse(particle.Element("Acceleration").Attribute("x").Value, out x) &&
                    double.TryParse(particle.Element("Acceleration").Attribute("y").Value, out y))
                {
                    newParticle.acceleration = new Vector2((float)x, (float)y);
                }

                //decay
                if (double.TryParse(particle.Element("Decay").Attribute("x").Value, out x) &&
                    double.TryParse(particle.Element("Decay").Attribute("y").Value, out y))
                {
                    newParticle.decay = new Vector2((float)x, (float)y);
                }


                //add
                loadedParticles.Add(particle.Name.LocalName, newParticle);
            }

            #endregion

            /////////////////////////////
            //Emitters
            #region Emitters

            foreach (XElement emitter in document.Element("Emitters").Elements())
            {
                //make new
                Emitter newEmitter;// = new Emitter();

                Vector2 newOrigin = new Vector2();
                Vector2 newPower = new Vector2();
                float newDepth = 0.0f;
                float newRate = 0.0f;
                int newCount = 0;

                double x, y, depth;
                double rate; int count;

                List<BlendState> blendstateList = new List<BlendState>();

                //read through attributes

                //Origin
                if (double.TryParse(emitter.Element("Origin").Attribute("x").Value, out x) &&
                    double.TryParse(emitter.Element("Origin").Attribute("y").Value, out y) &&
                    double.TryParse(emitter.Element("Origin").Attribute("depth").Value, out depth))
                {
                    newOrigin = new Vector2((float)x, (float)y);
                    newDepth = (float)depth;
                }

                //power
                if (double.TryParse(emitter.Element("Power").Attribute("x").Value, out x) &&
                    double.TryParse(emitter.Element("Power").Attribute("y").Value, out y))
                {
                    newPower = new Vector2((float)x, (float)y);
                }

                //rate
                if (double.TryParse(emitter.Element("Rate").Attribute("rate").Value, out rate))
                {
                    newRate = (float)rate;
                }
                if (int.TryParse(emitter.Element("Rate").Attribute("count").Value, out count))
                {
                    newCount = count;
                }

                //blendstates
                foreach (XElement state in emitter.Element("BlendStates").Elements())
                {
                    switch (state.Attribute("state").Value)
                    {
                        case "Additive":
                            blendstateList.Add(BlendState.Additive);
                            break;
                        case "NonPremultiplied":
                            blendstateList.Add(BlendState.NonPremultiplied);
                            break;
                        case "Opaque":
                            blendstateList.Add(BlendState.Opaque);
                            break;
                        default:
                            blendstateList.Add(BlendState.AlphaBlend);
                            break;
                    }
                }



                //initialise and add
                Particle newParticle;
                if (loadedParticles.TryGetValue(emitter.Name.LocalName, out newParticle))
                {
                    newEmitter = new Emitter(newParticle, new Vector2(), newPower, newRate, newCount);
                    newEmitter.blendStates = blendstateList;
                    newEmitter.origin = newOrigin;
                    newEmitter.depth = newDepth;
                    newEmitter.emitFromSelf = emitFromSelf;

                    EmitterList.Add(newEmitter);
                }
            }

            #endregion
            ///////////////////
            // Exit out


            return 0;
        }

        /// <summary>
        /// unload the emitter data
        /// </summary>
        /// <returns>error code, 0 is no error</returns>
        public int Unload()
        {
            //kill threads
            foreach (Emitter emitter in EmitterList)
                emitter.Terminate();
            //remove emitters
            EmitterList.Clear();
            //exit
            return 0;
        }

        /// <summary>
        /// add a point to the list for a set amount of time, 0.0 will still emit but will be removed afterwards
        /// </summary>
        /// <param name="pos">position</param>
        /// <param name="time">time</param>
        public void AddEmissionPoint(Vector2 pos, float time)
        {
            foreach (Emitter emitter in EmitterList)
                emitter.AddEmissionPoint(pos, time);
        }

        /// <summary>
        /// add a point to the list for one time
        /// </summary>
        /// <param name="pos">position</param>
        public void AddEmissionPoint(Vector2 pos) { AddEmissionPoint(pos, 0.0f); }

        /// <summary>
        /// manual emmission, emits for all positions
        /// </summary>
        public void EmitAll()
        {
            foreach(Emitter emitter in EmitterList)
                emitter.EmitAll();
        }

        /// <summary>
        /// emission logic
        /// </summary>
        public void Emit()
        {
            foreach (Emitter emitter in EmitterList)
                emitter.Emit();
        }

        /// <summary>
        /// emission logic
        /// </summary>
        /// <param name="position">position to emit at, uses emitter pos if null</param>
        public void Emit(Vector2 emitPos)
        {
            foreach (Emitter emitter in EmitterList)
                emitter.Emit(emitPos);
        }

        /// <summary>
        /// update the particles and emitters
        /// </summary>
        /// <param name="gametime">the game GameTime</param>
        /// <param name="emitFromPoint">emit particles from its own point</param>
        public void Update(GameTime gametime)
        {
            foreach (Emitter emitter in EmitterList)
                emitter.Update(gametime);
        }

        /// <summary>
        /// draw all the (active) particles
        /// </summary>
        /// <param name="spriteBatch">the game SpriteBatch</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Emitter emitter in EmitterList)
                emitter.Draw(spriteBatch);
        }

        /// <summary>
        /// terminates the emitter threads, needed to exit correctly
        /// </summary>
        /// <returns></returns>
        public int Terminate()
        {
            foreach (Emitter emitter in EmitterList)
                emitter.Terminate();
            return 0;
        }

    }


    /// <summary>
    /// basic single particle emitter
    /// </summary>
    class Emitter
    {

        //vars
        private List<Particle> particles;
        private float timeToNext;

        private Particle particleTemplate;

        //private Texture2D tParticleTexture;

        public Vector2 position;
        public Vector2 origin;
        public float depth;

        public Vector2 power;
        public float emitRate;
        public int emitCount;

        //private List<KeyValuePair<float, Vector2>> emissionPoints;
        class emissionNode{
            public Vector2 position;
            public float timeLeft;
            public emissionNode(Vector2 pos, float time){position = pos; timeLeft = time;}
            public void Update(float time){timeLeft -= time;}
        }
        private List<emissionNode> emissionPoints;

        public bool emitFromSelf;

        //public BlendState blendState = BlendState.AlphaBlend;
        public List<BlendState> blendStates;

        private Random random = new Random();

        //threading, one thread per emitter
        //private ManualResetEvent particlesUpdated;
        private AutoResetEvent updateParticles;
        private ManualResetEvent updateCall;
        private bool runUpdateThread;
        private Thread updateThread;

        /// <summary>
        /// default entry
        /// </summary>
        private Emitter()
        {
            timeToNext = 0.0f;

            particles = new List<Particle>();

            position = new Vector2();
            origin = new Vector2();
            depth = 0.0f;

            emissionPoints = new List<emissionNode>();

            emitFromSelf = false;

            blendStates = new List<BlendState>();
            blendStates.Add(BlendState.AlphaBlend);

            updateParticles = new AutoResetEvent(true);
            updateCall = new ManualResetEvent(false);
            runUpdateThread = true;
            updateThread = new Thread(new ThreadStart(ParticleUpdateThread));
            updateThread.Name = "emitterUpdateThread";
            updateThread.Start();

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
        public Emitter(Particle template, Vector2 pos, Vector2 pow, float rate, int count)
            : this()
        {
            //tParticleTexture = tex;
            particleTemplate = new Particle(template);
            position = pos;
            power = pow;
            emitRate = rate;
            emitCount = count;
        }

        /// <summary>
        /// add a point to the list for a set amount of time, 0.0 will still emit but will be removed afterwards
        /// </summary>
        /// <param name="pos">position</param>
        /// <param name="time">time, 0 by default(one time)</param>
        public void AddEmissionPoint(Vector2 pos, float time)
        {
            emissionPoints.Add(new emissionNode(pos,time));
        }

        /// <summary>
        /// add a point to the list for one cycle
        /// </summary>
        /// <param name="pos">position</param>
        public void AddEmissionPoint(Vector2 pos) { AddEmissionPoint(pos, 0.0f); }

        //update
        /// <summary>
        /// update the particles
        /// </summary>
        /// <param name="gametime">the gametime from the xna framework</param>
        /// <param name="emitFromPoint">emit particles from its own point</param>
        public void Update(GameTime gametime)
        {
            float time = (float)gametime.ElapsedGameTime.TotalSeconds;

            // emission system
            if (emitRate != 0.0f)
            {
                timeToNext += time * emitRate;

                while (timeToNext >= 1.0f)
                {
                    timeToNext -= 1.0f;

                    EmitAll();

                }
            }

            //update points
            foreach (emissionNode node in emissionPoints)
                node.Update(time);

            //remove old points
            for(int i = 0; i < emissionPoints.Count;)
            {
                if (emissionPoints[i].timeLeft <= 0.0f)
                {
                    emissionPoints.RemoveAt(i);
                }
                else
                    ++i;
            }

            //List<emissionNode> nodeRemoveList = new List<emissionNode>();
            //foreach (emissionNode node in emissionPoints)
            //    if (node.timeLeft <= 0.0f)
            //        nodeRemoveList.Add(node);
            //
            //foreach (emissionNode node in nodeRemoveList)
            //    emissionPoints.Remove(node);

            //we dont want too many particles!
            //CutParticles(10000);

            //update particles
            //foreach (Particle particle in particles)
            //    particle.Update(time);


            //thread sync
            updateParticles.WaitOne();

            //overburden check
            foreach (Particle particle in particles)
                if (particle.timeToAdd >= 1.0f)
                {
                    //particles.Remove(pair.Key);
                    //particles.RemoveRange(0, i-1);
                    Console.WriteLine("particle updating overburdened! flushing...");
                    particles.Clear();
                    break;
                }

            //add to update 
            foreach(Particle particle in particles)
                particle.timeToAdd += time;

            //trigger the thread to update
            updateCall.Set();
            updateParticles.Set();

        }

        /// <summary>
        /// the update cycle the thread uses
        /// </summary>
        private void ParticleUpdateThread()
        {

#if XBOX
            updateThread.SetProcessorAffinity(4); // 5 == 2nd thread on 3rd core
#endif

            while (runUpdateThread)
            {

                updateCall.WaitOne();

                //if(!runUpdateThread)
                //    updateThread.Abort();

                //update start


                //updateParticles.WaitOne();

                //while (updateList.Count > 0)
                for (int i = 0; i < particles.Count; )
                {

                    updateParticles.WaitOne();

                    if (i < particles.Count)
                    {

                        //KeyValuePair<Particle, float> pair = updateList.ElementAt(updateList.Count-1);
                        Particle particle = particles[i];

                        //update
                        particle.Update();

                        //remove dead
                        if (particle.lifeLeft <= 0.0f)
                            particles.Remove(particle);
                        else
                            ++i;

                        //remove from list
                        //updateList.RemoveAt(updateList.Count - 1);

                        //Thread.Sleep(0);
                    }
                    updateParticles.Set();
                    Thread.Sleep(0);
                }


                //updateParticles.Set();

                updateCall.Reset();

                Thread.Sleep(0);

                //Thread.Sleep(updateTickRate);
            }
        }

        //draw
        /// <summary>
        /// draw the particles
        /// </summary>
        /// <param name="spriteBatch">the spritebatch used in rendering</param>
        /// <param name="blend">custom blend, default if null</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //if (blend == null) blend = blendState;

            //thread sync
            //updateParticles.WaitOne();

            //draw
            foreach (BlendState blend in blendStates)
            {
                updateParticles.WaitOne();

                //spriteBatch.Begin(SpriteSortMode.BackToFront, blend);
                spriteBatch.Begin(SpriteSortMode.BackToFront, blend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

                foreach (Particle particle in particles)
                    particle.Draw(spriteBatch);

                spriteBatch.End();

                updateParticles.Set();
            }

            //updateParticles.Set();

        }

        //Emit particle/s, we call this manually for 0.0f rate or in an event (eg; confetti)
        /// <summary>
        /// manual emmission, emits for all positions
        /// </summary>
        public void EmitAll()
        {
            if (emitFromSelf)
                Emit(position);

            foreach (emissionNode node in emissionPoints)
            {
                Emit(node.position);
            }
        }

        /// <summary>
        /// emission logic
        /// </summary>
        /// <param name="position">position to emit at</param>
        public void Emit(Vector2 emitPos)
        {
            //thread sync
            updateParticles.WaitOne();

            for (int i = 0; i < emitCount; ++i)
            {
                Particle newParticle = new Particle(particleTemplate);

                newParticle.position = emitPos;
                newParticle.depth = depth;

                double angle = Math.PI * 2.0 * random.NextDouble();
                float scale = (float)random.NextDouble();

                newParticle.velocity.X = (float)Math.Sin(angle) * scale * power.X;
                newParticle.velocity.Y = (float)Math.Cos(angle) * scale * power.Y;

                particles.Add(newParticle);
            }

            updateParticles.Set();
        }

        /// <summary>
        /// trigger emission using the emitter's position (overrides emitFromSelf)
        /// </summary>
        public void Emit() { Emit(position); }

        //cut excessive particles so we dont have too many, returns difference
        /// <summary>
        /// cut the excess particles
        /// </summary>
        /// <param name="maxNum">maximum amount</param>
        /// <returns>amount cut, or headroom if negative</returns>
        public int CutParticles(int maxNum)
        {
            //thread sync
            updateParticles.WaitOne();

            int diff = particles.Count - maxNum;

            if (diff > 0)
                particles.RemoveRange(0, diff);

            updateParticles.Set();

            return diff;
        }

        /// <summary>
        /// terminates the emitter thread, needed to exit the game correctly
        /// </summary>
        /// <returns></returns>
        public int Terminate()
        {
            runUpdateThread = false;
            updateCall.Set();
            //updateThread.Abort();
            return 0;
        }

    }


    /// <summary>
    /// basic single type particle
    /// </summary>
    class Particle
    {
        //vars
        public float lifeLeft;
        public float lifeLength;

        public Vector2 position;
        public Vector2 velocity;
        public Vector2 acceleration;
        public Vector2 decay;

        public Texture2D texture;
        public Color colour;
        public float sizeScale;
        public float depth;

        public VectorGraph colourGraph;
        private float colourMulti;
        public VectorGraph sizeGraph;
        private float sizeMulti;
        public VectorGraph rotationGraph;
        private float rotationMulti;

        //ticking
        public static int tickCount = 1;
        private int currentTick;

        //alternate to dictionary
        public float timeToAdd;

        //construct
        /// <summary>
        /// create particle with default values
        /// </summary>
        public Particle()
        {
            lifeLeft = 1.0f;
            lifeLength = 1.0f;

            position = new Vector2();
            velocity = new Vector2();
            acceleration = new Vector2();
            decay = new Vector2();

            //texture = 
            colour = Color.White;
            sizeScale = 1.0f;
            depth = 0.0f;

            colourGraph = new EmptyVectorGraph();
            colourMulti = 0.0f;
            sizeGraph = new EmptyVectorGraph();
            sizeMulti = 0.0f;
            rotationGraph = new EmptyVectorGraph();
            rotationMulti = 0.0f;

            currentTick = 0;
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="p">existing particle to copy</param>
        public Particle(Particle p) : this()
        {
            lifeLeft = p.lifeLeft;
            lifeLength = p.lifeLength;

            position = p.position;
            velocity = p.velocity;
            acceleration = p.acceleration;
            decay = p.decay;

            texture = p.texture;
            colour = p.colour;
            sizeScale = p.sizeScale;
            depth = p.depth;

            colourGraph = p.colourGraph;
            sizeGraph = p.sizeGraph;
            rotationGraph = p.rotationGraph;
        }

        //public ~Particle() { }

        /// <summary>
        /// sets the scale based on the size of the texture, 
        /// </summary>
        /// <param name="pixelscale">max pixels to occupy in each dimension</param>
        public void SetPixelScale(float pixelscale)
        {
            if(texture != null)
                sizeScale = pixelscale / (float)Math.Max(texture.Width, texture.Height);
        }

        //update
        /// <summary>
        /// update particle
        /// </summary>
        /// <param name="gametime">gametime from xna</param>
        public void Update(float time) { timeToAdd += time; Update(); }

        /// <summary>
        /// update particle
        /// </summary>
        public void Update()
        {
            ++currentTick;
            //update, check ticking
            if (currentTick == tickCount)
            {
                //timeToAdd *= tickCount;
                if (lifeLeft >= 0.0f)
                {
                    //update values
                    velocity.X -= (velocity.X * decay.X) * timeToAdd;
                    velocity.Y -= (velocity.Y * decay.Y) * timeToAdd;
                    velocity.X += acceleration.X * timeToAdd;
                    velocity.Y += acceleration.Y * timeToAdd;
                    position.X += velocity.X * timeToAdd;
                    position.Y += velocity.Y * timeToAdd;

                    //Leaving comment here for you: You're getting value at the graph at "lifeLeft" which starts at 1?
                    //Doesn't that mean the graph is backwards, 1 being the start 0 being the end? 1-lifeLeft would make
                    //More sense imo because 0 would be "been alive for 0s"
                    //The way you formatted them (going from 0, 0.5, 1, or such) in the xml files makes me think you meant it to be 1-lifeLeft
                    //But because yours are all symmetric it made no difference. Anyway!
                    colourMulti = colourGraph.GetValue(lifeLeft);
                    sizeMulti = sizeGraph.GetValue(lifeLeft);
                    rotationMulti = 1.0f - rotationGraph.GetValue(lifeLeft);


                    lifeLeft -= timeToAdd / lifeLength;
                }
                timeToAdd = 0.0f;
                currentTick = 0;
            }
        }

        //draw
        /// <summary>
        /// draw particle
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(texture, position, Color.White);
            spriteBatch.Draw(texture, position, null, colour * colourMulti, 2.0f * (float)Math.PI * rotationMulti, new Vector2((float)(texture.Width / 2), (float)(texture.Height / 2)), sizeScale * sizeMulti, SpriteEffects.None, depth);
        }

    }
}
