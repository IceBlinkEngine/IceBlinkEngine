using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;
using IceBlink;
using System.IO;
using System.Windows.Forms;
using SharpDX.Direct3D9;

namespace IceBlinkCore
{
    [Serializable]
    public class Sprite
    {
        [XmlIgnore]
        public Game game;

        #region Fields
        private Size spriteSize = new Size(64, 64);
        private Bitmap bitmap = null;
        private Texture texture = null;
        private SharpDX.Direct3D9.Sprite dxSprite = null;
        private SharpDX.DataStream dxTextureStream = null;
        private string filename = ""; //should include filename and extension ex. "tree.spt"
        private string spriteSheetFilename = ""; //should include filename and extension ex. "tree.png"
        private bool actor = false; //true = actor, false = prop
        private string tag = "newTag"; //a unique identifier for each sprite
        private bool topDown = false; //true = top down (4 directions to rotate), false = front facing (flip 2 ways)
        private bool animated = false; //true = animated, false = static
        private bool show = true; //true = show, false = hide (can be used to temporarily hide something)
        private bool visible = false; //if the image is within the LoS and in the visibility range
        private bool looping = false; //true = looping, false = one time through then stop
        private Point location = new Point(0, 0);
        private int frame = 0;
        private Point frameRange = new Point(0, 59);
        private int fps = 15;
        private long timeStamp = 0;
        [XmlElement]
        public Point ImageLayout = new Point(10, 6);
        private int numberOfFrames = 60;
        private int idleNumberOfFrames = 1;
        private int attackingNumberOfFrames = 1;
        private int walkingNumberOfFrames = 1;
        // * sinopip, 25.12.14
        private int deathNumberOfFrames = 1;
        //
        private int idleFPS = 4;
        private int attackingFPS = 4;
        private int walkingFPS = 4;
        // * sinopip, 25.12.14
        private int deathFPS = 4;
        //
        private int idleDelay = 2000;
        [XmlIgnore]
        public Rectangle oSourceRect = new Rectangle(0, 0, 64, 64);
        [XmlIgnore]
        public Rectangle oDestRect = new Rectangle(0, 0, 64, 64);
        #endregion

        #region Properties
        [XmlElement]
        public Size SpriteSize
        {
            get { return spriteSize; }
            set { spriteSize = value; }
        }
        [XmlIgnore]
        public Bitmap Image
        {
            get { return bitmap; }
            set { bitmap = value; }
        }
        [XmlIgnore]
        public Texture Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        [XmlIgnore]
        public SharpDX.Direct3D9.Sprite DxSprite
        {
            get { return dxSprite; }
            set { dxSprite = value; }
        }
        [XmlIgnore]
        public SharpDX.DataStream TextureStream
        {
            get { return dxTextureStream; }
            set { dxTextureStream = value; }
        }
        [XmlElement]
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }
        [XmlElement]
        public string SpriteSheetFilename
        {
            get { return spriteSheetFilename; }
            set { spriteSheetFilename = value; }
        }
        [XmlElement]
        public bool Actor
        {
            get { return actor; }
            set { actor = value; }
        }
        [XmlElement]
        public string Tag
        {
            get { return tag; }
            set { tag = value; }
        }
        [XmlElement]
        public bool TopDown
        {
            get { return topDown; }
            set { topDown = value; }
        }
        [XmlElement]
        public bool Animated
        {
            get { return animated; }
            set { animated = value; }
        }
        [XmlElement]
        public bool Show
        {
            get { return show; }
            set { show = value; }
        }
        [XmlElement]
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }
        [XmlElement]
        public bool Looping
        {
            get { return looping; }
            set { looping = value; }
        }        
        [XmlElement]
        public Point Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
                oDestRect = new Rectangle(location.X, location.Y, SpriteSize.Width, SpriteSize.Height);
            }
        }
        [XmlElement]
        public int Frame
        {
            get
            {
                return frame;
            }
            set
            {
                value = value % numberOfFrames;
                if (value > FrameRange.Y)
                    value = FrameRange.X - 1 + (value - FrameRange.Y);
                if (value < FrameRange.X)
                    value = FrameRange.Y - 1 - (value - FrameRange.X);
                frame = value;
                //we validate once and store it in temp variables
                Point layout = ImageLayout;
                Size sSize = SpriteSize;
                //for use here
                oSourceRect = new Rectangle((frame % layout.X) * sSize.Width,
                    Convert.ToInt32(frame / layout.X) * sSize.Height,
                    sSize.Width, sSize.Height);
            }
        }
        [XmlElement]
        public Point FrameRange
        {
            get
            {
                return frameRange;
            }
            set
            {
                if (value.X > -1 && value.X < numberOfFrames &&
                    value.Y > -1 && value.Y < numberOfFrames)
                {
                    frameRange = value;
                    Frame = frameRange.X;
                }
                else
                {
                    frameRange = new Point(0, numberOfFrames - 1);
                    Frame = frameRange.X;
                }
            }
        }
        [XmlElement]
        public int FPS
        {
            get { return fps; }
            set { fps = value; }
        }
        [XmlIgnore]
        public long TimeStamp
        {
            get { return timeStamp; }
            set { timeStamp = value; }
        }
        [XmlElement]
        public int NumberOfFrames
        {
            get { return numberOfFrames; }
            set { numberOfFrames = value; }
        }
        [XmlElement]
        public int IdleNumberOfFrames
        {
            get { return idleNumberOfFrames; }
            set { idleNumberOfFrames = value; }
        }
        [XmlElement]
        public int AttackingNumberOfFrames
        {
            get { return attackingNumberOfFrames; }
            set { attackingNumberOfFrames = value; }
        }
        [XmlElement]
        public int WalkingNumberOfFrames
        {
            get { return walkingNumberOfFrames; }
            set { walkingNumberOfFrames = value; }
        }
        // * sinopip, 25.12.14
        [XmlElement]
        public int DeathNumberOfFrames
        {
            get { return deathNumberOfFrames; }
            set { deathNumberOfFrames = value; }
        }
        //
        [XmlElement]
        public int IdleFPS
        {
            get { return idleFPS; }
            set { idleFPS = value; }
        }
        [XmlElement]
        public int AttackingFPS
        {
            get { return attackingFPS; }
            set { attackingFPS = value; }
        }
        [XmlElement]
        public int WalkingFPS
        {
            get { return walkingFPS; }
            set { walkingFPS = value; }
        }
        // * sinopip, 25.12.14
        [XmlElement]
        public int DeathFPS
        {
            get { return deathFPS; }
            set { deathFPS = value; }
        }
        //        
        [XmlElement]
        public int IdleDelay
        {
            get { return idleDelay; }
            set { idleDelay = value; }
        }
        #endregion

        public Sprite()
        {
        }
        public void passRefs(Game g)
        {
            game = g;
        }
        public override string ToString()
        {
            return Filename;
        }
        public bool MoveAnimation()
        {
            long elapsed = DateTime.Now.Ticks - timeStamp;
            if (timeStamp == 0)//if we are just starting animation
            {
                Frame = FrameRange.X;
                timeStamp = elapsed;
                return false;
            }
            //in ms
            int frameDuration = Convert.ToInt32(1000.0 / fps);
            //100ns=10,000 ticks in 1 ms
            int spriteFrame = Convert.ToInt32(elapsed / 10000 / frameDuration);
            if (spriteFrame > 0)
            {
                //looping done in frame validation so just add some frames
                Frame = Frame + spriteFrame;
                timeStamp = DateTime.Now.Ticks;
                return true;
            }
            return false;
        }
        public void saveSpriteFile(string filename)
        {
            StreamWriter writer = null;
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(Sprite));
                writer = new StreamWriter(filename);
                ser.Serialize(writer, this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save Sprite. Error: " + ex.Message);
                game.errorLog(ex.ToString());
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
                writer = null;
            }
        }
        public Sprite loadSpriteFile(string filename)
        {
            Sprite toReturn = null;
            XmlSerializer serializer = new XmlSerializer(typeof(Sprite));
            FileStream myFileStream = null;
            try
            {
                myFileStream = new FileStream(filename, FileMode.Open);
                toReturn = (Sprite)serializer.Deserialize(myFileStream);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open xml file. Error: " + ex.ToString());
                game.errorLog(ex.ToString());
            }
            finally
            {
                if (myFileStream != null)
                {
                    myFileStream.Close();
                }
            }
            return toReturn;
        }
        public Bitmap LoadSpriteSheetBitmap(string filename)
        {
            // Sets up an image object to be displayed.
            if (Image != null)
            {
                Image.Dispose();
            }
            Image = new Bitmap(filename);
            return Image;
        }
        public SharpDX.DataStream LoadTextureStream(string filename)
        {
            //var tempDevice = new Device(new Direct3D(), 0, DeviceType.Hardware, game.renderPanel.Handle, CreateFlags.SoftwareVertexProcessing, new PresentParameters(game.renderPanel.ClientSize.Width, game.renderPanel.ClientSize.Height));
            Texture pcTex = Texture.FromFile(game.DeviceDX9, filename);
            SharpDX.DataStream testTex = Texture.ToStream(pcTex, ImageFileFormat.Png);
            pcTex.Dispose();
            return testTex;
        }
        public Sprite DeepCopy()
        {
            Sprite other = (Sprite)this.MemberwiseClone();
            other.SpriteSize = new Size(this.SpriteSize.Width, this.SpriteSize.Height);
            other.Location = new Point(this.Location.X, this.Location.Y);
            other.FrameRange = new Point(this.FrameRange.X, this.FrameRange.Y);
            other.ImageLayout = new Point(this.ImageLayout.X, this.ImageLayout.Y);
            return other;
        }
    }
}
