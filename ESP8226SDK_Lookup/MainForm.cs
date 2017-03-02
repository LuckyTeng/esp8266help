using System;
using System.Diagnostics;       // Debug
using System.IO;                // Path
using System.Linq;
using System.Xml.Linq;

using System.Windows.Forms;

namespace ESP8226SDK_Lookup
{
    public partial class MainForm :Form
    {
        private const string ATTR_DOC_PATH = "docpath";
        private const string ATTR_GROUP = "group";
        private const string ATTR_NAME = "name";

        public MainForm()
        {
            InitializeComponent();

        }

        private void txtKey_TextChanged(object sender, EventArgs e)
        {
            updateList();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            updateGroup();
            updateList();
            //updateDoc("os_timer_arm");
        }

        private void updateList()
        {
            string keyword = txtKey.Text;
            string group = cbGrp.Text;

            // clear items
            lbKey.Items.Clear();

            XDocument doc = XDocument.Load("esp8266.xml");

            if (keyword == String.Empty && group == String.Empty)
            {
                foreach (XElement result in doc.Elements("esp8266doc")
                                               .Elements("keyword"))
                    lbKey.Items.Add(result.Attribute("name").Value);
            }
            else
            {
                // filter by keyword
                foreach (XElement result in doc.Elements("esp8266doc")
                                               .Elements("keyword")
                                               .Where(w => (keyword == String.Empty || w.Attribute("name").Value.Contains(keyword) )
                                                    && ( group == String.Empty || w.Attribute("group").Value.Contains(group) ) ))
                
                    lbKey.Items.Add(result.Attribute("name").Value);
            }
        }

        private void updateGroup()
        {
            cbGrp.Items.Clear();

            XDocument doc = XDocument.Load("esp8266.xml");

            foreach (XElement result in doc.Elements("esp8266doc")
                                               .Elements("groups")
                                               .Elements("group"))
                cbGrp.Items.Add(result.Value);
        }

        private void updateDoc(string name)
        {
            webBrowser1.Url = null;

            XDocument doc = XDocument.Load("esp8266.xml");

            XElement result = doc.Elements("esp8266doc")
                                 .Elements("keyword").FirstOrDefault(w => w.Attribute("name").Value == name);

            if (result != null)
            {
                var xDocPath = result.Attribute(ATTR_DOC_PATH);
                var xGroup = result.Attribute(ATTR_GROUP);
                var xName = result.Attribute(ATTR_NAME);

                var docfullpath = PathResolver( xDocPath, xGroup, xName);
                Debug.WriteLine(String.Format("the path is {0}", docfullpath));

                if (docfullpath == String.Empty)
                    docfullpath = "about:blank";        // should we change to error.htm?
                webBrowser1.Url = new Uri(docfullpath);
            }
        }

        /// <summary>
        /// Return the physical file address 
        /// </summary>
        /// <remarks>
        /// The return path is determined by follow sequence
        /// 1. the docpath
        /// 2. the path docs/[name]
        /// 3. the path docs/[group]/[name]
        /// </remarks>
        private string PathResolver(XAttribute xDocPath, XAttribute xGroup, XAttribute xName)
        {
            string relePath = String.Empty;
            bool bSearched = false;

            // if we have doc?
            if (xDocPath != null)
            {
                relePath = xDocPath.Value;
                bSearched = File.Exists(System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + relePath);
            }

            if (!bSearched)
            {
                if (xName != null && xGroup == null)
                {
                    relePath = "docs" + Path.PathSeparator + xName.Value + ".htm";
                    bSearched = File.Exists(System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + relePath);
                }
            }

            if (!bSearched)
            {
                if (xName != null && xGroup != null)
                {
                    relePath = "docs" + Path.DirectorySeparatorChar + xGroup.Value + Path.DirectorySeparatorChar + xName.Value + ".htm";
                    bSearched = File.Exists(System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + relePath);
                }
            }



            if (bSearched)
                return "file://" + System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + relePath;

            return String.Empty;
        }

        private void lbKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            var key = lbKey.Text;
            updateDoc(key);
        }

        private void cbGrp_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void cbGrp_TextUpdate(object sender, EventArgs e)
        {
            
        }

        private void cbGrp_TextChanged(object sender, EventArgs e)
        {
            updateList();
        }


    }
}