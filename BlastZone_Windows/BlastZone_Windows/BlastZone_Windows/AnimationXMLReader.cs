using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace BlastZone_Windows
{
    struct AnimationFrameDetails
    {
        public int x, y, w, h;
        public bool flip;
    }

    struct AnimationDetails
    {
        public String name;
        public Texture2D tex;
        public List<AnimationFrameDetails> frames;
        public bool loop;
    }

    class AnimationXMLReader
    {
        static int GetIntAttribHelper(IEnumerable<XAttribute> attribs, string key)
        {
            var foundattrib = attribs.SingleOrDefault(x => x.Name == key);

            if (foundattrib == null) throw new Exception("Key '" + key + "' missing.");

            return Convert.ToInt32(foundattrib.Value);
        }

        static bool GetBoolAttribHelper(IEnumerable<XAttribute> attribs, string key, bool defaultReturn = false)
        {
            var foundattrib = attribs.SingleOrDefault(x => x.Name == key);

            if (foundattrib == null) return defaultReturn;

            return Convert.ToBoolean(foundattrib.Value);
        }

        public static List<AnimationDetails> LoadContent(ContentManager Content, string filename)
        {
            List<AnimationDetails> aDetailsList = new List<AnimationDetails>();

            var doc = XElement.Load(Path.Combine(Content.RootDirectory, filename));

            Texture2D tex;

            try
            {
                tex = Content.Load<Texture2D>(doc.Element("Metadata").Element("FileName").Value);
            }
            catch (ContentLoadException cle)
            {
                Console.Out.WriteLine("ANIMATIONXMLREADER: Texture from animation '" + filename + "' not found.");
                return aDetailsList;
            }
            

            foreach (XElement img in doc.Elements("Images").Elements("Image"))
            {
                AnimationDetails aDetails;

                aDetails.name = img.Attribute("name").Value;
                aDetails.tex = tex;
                aDetails.frames = new List<AnimationFrameDetails>();

                var loopAttrib = img.Attribute("loop");
                aDetails.loop = Convert.ToBoolean(loopAttrib.Value ?? "false");

                foreach (XElement frame in img.Elements("Frames").Elements("Frame"))
                {
                    AnimationFrameDetails aFrame;

                    var attribs = frame.Attributes();

                    aFrame.x = GetIntAttribHelper(attribs, "x");
                    aFrame.y = GetIntAttribHelper(attribs, "y");
                    aFrame.w = GetIntAttribHelper(attribs, "w");
                    aFrame.h = GetIntAttribHelper(attribs, "h");
                    aFrame.flip = GetBoolAttribHelper(attribs, "flip", false);

                    aDetails.frames.Add(aFrame);
                }

                aDetailsList.Add(aDetails);
            }

            return aDetailsList;
        }
    }
}
