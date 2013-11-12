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


/// <summary>
/// animated sprite system that uses a sheet and a corresponding xml file
/// made by Paul Hancock
/// </summary>
namespace SpritesheetAnimation
{
    /// <summary>
    /// animation sheet class, uses single xml file and spritesheet
    /// </summary>
    class AnimationSheet
    {

        //mesh and animation list
        //public int meshX { get; private set; }
        //public int meshY { get; private set; }
        private Rectangle? mesh;

        public float defaultFrameRate { get; private set; }
        public Vector2 defaultSpriteSize { get; private set; }
        //public float defaultScale { get; private set; }
        public Texture2D texture { get; private set; }

        private Dictionary<string,Animation> animationList;


        //funtions

        /// <summary>
        /// construct
        /// </summary>
        public AnimationSheet() 
        {
            //meshX = 1;
            //meshY = 1;
            mesh = null;
            defaultFrameRate = 0.0f;
            defaultSpriteSize = new Vector2(0.0f);
            //defaultScale = 1.0f;
            texture = null;
            animationList = null;
        }

        /// <summary>
        /// load sheet data
        /// </summary>
        /// <param name="contentManager">the game contentmanager</param>
        /// <param name="xmlFile">the data file</param>
        /// <returns>error code, 0 if errorless</returns>
        public int Load(ContentManager contentManager, string xmlFile)
        {
            animationList = new Dictionary<string, Animation>();

            XElement document = XElement.Load(Path.Combine(contentManager.RootDirectory, xmlFile) + ".xml");

            int end = xmlFile.LastIndexOf('\\') + 1;
            string directory = xmlFile.Remove(end, xmlFile.Length - end);

            this.texture = contentManager.Load<Texture2D>(directory + document.Name.LocalName);

            int x = 0, y = 0;
            double rate = 0.0;

            //mesh
            if (document.Element("Mesh") != null)
            {
                if (int.TryParse(document.Element("Mesh").Attribute("x").Value, out x) &&
                    int.TryParse(document.Element("Mesh").Attribute("y").Value, out y))
                    mesh = new Rectangle(x, y, texture.Width / x, texture.Height / y);
            }

            //framerate
            if (double.TryParse(document.Element("Framerate").Attribute("default").Value, out rate))
                defaultFrameRate = (float)rate;

            //size
            if (int.TryParse(document.Element("Size").Attribute("x").Value, out x) &&
                int.TryParse(document.Element("Size").Attribute("y").Value, out y))
                defaultSpriteSize = new Vector2((float)x, (float)y);


            //animations
            foreach (XElement animation in document.Element("Animations").Elements())
            {
                Animation newAnimation = new Animation();
                newAnimation.frameList = new List<Frame>();

                //frames
                foreach (XElement frame in animation.Elements("Frame"))
                {
                    Frame newFrame = new Frame();

                    if (mesh != null)
                    {
                        //not null, we create as it were a solid mesh
                        newFrame.coords.Width = mesh.Value.Width;
                        newFrame.coords.Height = mesh.Value.Height;

                        //coords
                        if (int.TryParse(frame.Attribute("x").Value, out x))
                            newFrame.coords.X = x * newFrame.coords.Width;

                        if (int.TryParse(frame.Attribute("y").Value, out y))
                            newFrame.coords.Y = y * newFrame.coords.Height;
                    }
                    else
                    {
                        //null, so we grab the manually defined values
                        if (int.TryParse(animation.Attribute("width").Value, out x))
                            newFrame.coords.Width = x;
                        if (int.TryParse(animation.Attribute("height").Value, out y))
                            newFrame.coords.Height = y;

                        //coords, per-pixel
                        if (int.TryParse(frame.Attribute("x").Value, out x))
                            newFrame.coords.X = x;

                        if (int.TryParse(frame.Attribute("y").Value, out y))
                            newFrame.coords.Y = y;
                    }

                    //check if flip
                    if(frame.Attribute("flip") != null)
                        switch (frame.Attribute("flip").Value)
                        {
                            case "H":
                                newFrame.flip = SpriteEffects.FlipHorizontally;
                                break;
                            case "V":
                                newFrame.flip = SpriteEffects.FlipVertically;
                                break;
                            default:
                                break;
                        }

                    newAnimation.frameList.Add(newFrame);
                }

                newAnimation.returnAnimation = animation.Element("End").Attribute("return").Value;

                newAnimation.name = animation.Name.LocalName;

                //add to list
                animationList.Add(animation.Name.LocalName, newAnimation);
            }

            //done

            return 0;
        }

        /// <summary>
        /// unload animations
        /// </summary>
        public void UnLoad()
        {
            animationList.Clear();
        }


        /// <summary>
        /// gets animation out of list
        /// </summary>
        /// <param name="name">name of animation</param>
        /// <returns>animation? null if not existant</returns>
        public Animation? GetAnimation(string name)
        {
            Animation returnAnimation;
            if (animationList.TryGetValue(name, out returnAnimation))
                return returnAnimation;
            else
                return null;
        }

        

    }

    /// <summary>
    /// animated sprite, uses animated sheet
    /// </summary>
    class AnimatedSprite
    {
        public Vector2 position;
        public float rotation;
        public Color colour;
        //public float scale;
        public Vector2 size;
        public float depth;

        //private AnimationNode animationNode;
        private AnimationSheet animationSheet;
        private Animation? currentAnimation;
        private float currentAnimatonTime;

        private Frame currentFrame;

        public float frameRate;

        public Animation? Animation { get { return currentAnimation; } }


        private AnimatedSprite()
        {
            position = new Vector2();
            rotation = 0.0f;
            colour = Color.White;
            //scale = 1.0f;
            size = new Vector2();
            depth = 0.0f;

            currentAnimation = null;
            frameRate = 0.0f;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="sheet">the animation sheet to use</param>
        /// <param name="animation">the start animation</param>
        public AnimatedSprite(AnimationSheet sheet, string animation) : this()
        {
            animationSheet = sheet;
            //animationNode = new AnimationNode();
            //animationNode.currentAnimation = animation;
            currentAnimation = animationSheet.GetAnimation(animation);

            //scale = animationSheet.defaultScale;
            size = animationSheet.defaultSpriteSize;
            frameRate = animationSheet.defaultFrameRate;
        }

        /// <summary>
        /// update
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {

            //if empty, then no frame
            if (currentAnimation != null)
            {
                //time
                currentAnimatonTime += (float)gametime.ElapsedGameTime.TotalSeconds * frameRate;

                //if gone past animation life, switch to return
                if (currentAnimatonTime > currentAnimation.Value.frameList.Count)
                {
                    //set to return
                    SetAnimation(currentAnimation.Value.returnAnimation);
                }
                else
                {
                    //get render frame from list
                    currentFrame = currentAnimation.Value.frameList.ElementAt((int)currentAnimatonTime);
                }

            }
        }

        /// <summary>
        /// draw
        /// </summary>
        /// <param name="spritebatch"></param>
        public void Draw(SpriteBatch spritebatch)
        {
            //spritebatch.Draw(texture, position, null, colour * colourMulti, 2.0f * (float)Math.PI * rotationMulti, new Vector2((float)(texture.Width / 2), (float)(texture.Height / 2)), sizeScale * sizeMulti, SpriteEffects.None, depth);
            if(currentAnimation != null)
                spritebatch.Draw(animationSheet.texture, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), currentFrame.coords, colour, rotation, new Vector2((float)(currentFrame.coords.Width / 2), (float)(currentFrame.coords.Height / 2)), currentFrame.flip, depth);
        }

        /// <summary>
        /// set the current animation, resets time
        /// </summary>
        /// <param name="animation">name of animation</param>
        public void SetAnimation(string animation)
        {
            currentAnimation = animationSheet.GetAnimation(animation);
            currentAnimatonTime = 0.0f;
        }

        /// <summary>
        /// set the current animation, without resetting time
        /// </summary>
        /// <param name="animation">name of animation</param>
        public void ContinueAnimation(string animation)
        {
            currentAnimation = animationSheet.GetAnimation(animation);
            while (currentAnimatonTime > currentAnimation.Value.frameList.Count)
                currentAnimatonTime -= (float)currentAnimation.Value.frameList.Count;
        }
        
    }

    
    /// <summary>
    /// frame data
    /// </summary>
    struct Frame
    {
        public Rectangle coords;
        public SpriteEffects flip;
    }
    
    /// <summary>
    /// animation data
    /// </summary>
    struct Animation
    {
        public string name;
        public List<Frame> frameList;
        public string returnAnimation;
    }
    
}
