using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using System.Windows.Forms;

namespace ESP8226SDK_Lookup
{
    public partial class Form1 :Form
    {
        private const string DOC_PATH = "docpath";

        public Form1()
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
                var xAttribute = result.Attribute(DOC_PATH);

                string message;
                if (xAttribute == null)
                {
                    // we try to use name to get file
                    xAttribute  = result.Attribute("name");
                    if (xAttribute == null) return;

                    message = "docs\\" + xAttribute.Value + ".htm";
                }
                else
                    message = xAttribute.Value;
                var docfullpath = "file://" + System.IO.Directory.GetCurrentDirectory() + "\\" + message;
                Debug.WriteLine(docfullpath);

                webBrowser1.Url = new Uri(docfullpath);
            }
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