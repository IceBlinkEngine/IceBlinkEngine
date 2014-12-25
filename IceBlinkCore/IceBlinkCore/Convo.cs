using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using IceBlink;


namespace IceBlinkCore
{
    [Serializable]
    public class Convo
    {
        [XmlIgnore]
        public Game game;

        private string convoFileName = "";
        private bool narration = false;
        private bool partyChat = false;
        private string portraitBitmap = "";
        private string defaultNpcName = "";
        private int f_nextIdNum = 0;

        [XmlElement]
        public string ConvoFileName
        {
            get
            {
                return convoFileName;
            }
            set
            {
                convoFileName = value;
            }
        }
        
        [XmlElement]
        public bool Narration
        {
            get
            {
                return narration;
            }
            set
            {
                narration = value;
            }
        }

        [XmlElement]
        public bool PartyChat
        {
            get
            {
                return partyChat;
            }
            set
            {
                partyChat = value;
            }
        }

        [XmlElement]
        public string NpcPortraitBitmap
        {
            get
            {
                return portraitBitmap;
            }
            set
            {
                portraitBitmap = value;
            }
        }

        [XmlElement]
        public string DefaultNpcName
        {
            get
            {
                return defaultNpcName;
            }
            set
            {
                defaultNpcName = value;
            }
        }

        [XmlArrayItem("ContentNode")]
        public List<ContentNode> subNodes = new List<ContentNode>();
                
        [XmlElement]
        public int NextIdNum
        {
            get
            {
                return f_nextIdNum;
            }
            set
            {
                f_nextIdNum = value;
            }
        }

        public Convo()
        {
        }
        public void passRefs(Game g)
        {
            game = g;
        }
        public Convo GetConversation(string xmlFileName)
        {
            Convo toReturn = null;
            XmlSerializer serializer = new XmlSerializer(typeof(Convo));
            FileStream myFileStream = null;
            try
            {
                myFileStream = new FileStream(xmlFileName, FileMode.Open);
                toReturn = (Convo)serializer.Deserialize(myFileStream);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open xml file. Error:\n{0}", ex.Message);
                game.errorLog(ex.ToString());
            }
            finally
            {
                if (myFileStream != null)
                {
                    myFileStream.Close();
                }
                //SortConversation(toReturn);
            }
            if (toReturn != null)
            {
                //toReturn.log = myLog;
            }
            return toReturn;
        }
        public void SaveContentConversation(string fileName)
        {
            StreamWriter writer = null;
            // * sinopip 22.12.14 - James' fix
			if (fileName.Substring(fileName.Length-4,4) != ".xml") //JamesManhattan 11/17/14 added this just to be sure to save conversations correctly, it worked!!
			{
				fileName = fileName + ".xml";
			}   
			//
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(Convo));
                writer = new StreamWriter(fileName);
                ser.Serialize(writer, this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save conversation. Error: " + ex.Message);
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
        public ContentNode GetContentNodeById(int idNum)
        {
            ContentNode tempNode = null;
            foreach (ContentNode subNode in subNodes)
            {
                tempNode = subNode.SearchContentNodeById(idNum);
                if (tempNode != null)
                {
                    return tempNode;
                }
            }
            return null;
        }
        public string GetTextById(int idNum)
        {
            ContentNode tempNode = GetContentNodeById(idNum);
            return tempNode.conversationText;
        }
        public void AddNodeToRoot(ContentNode contentNode)
        {
            subNodes.Add(contentNode);
        }
        public void RemoveNodeFromRoot(ContentNode contentNode)
        {
            subNodes.Remove(contentNode);
        }
    }

    [Serializable]
    public class ConvoSavedValues
    {
        private string convoFileName = "";
        private int nodeNotActiveIdNum = -1;

        [XmlElement]
        public string ConvoFileName
        {
            get { return convoFileName; }
            set { convoFileName = value; }
        }
        [XmlElement]
        public int NodeNotActiveIdNum
        {
            get { return nodeNotActiveIdNum; }
            set { nodeNotActiveIdNum = value; }
        }

        public ConvoSavedValues()
        {
        }
    }    
}
