using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HLBBWS2
{
    public class KeyValueConfigurationSection : ConfigurationSection
    {
        public KeyValueConfigurationSection()
        {
            this["elements"] = new KeyValueConfigurationCollection();            
        }
       
        /*
        [ConfigurationProperty("savefilepath")]
        public KeyValueConfigurationCollection savefilepath
        {
            get { return (KeyValueConfigurationCollection)this["savefilepath"]; }
        }
        */

        [ConfigurationProperty("elements")]
        public KeyValueConfigurationCollection Elements
        {
            get { return (KeyValueConfigurationCollection)this["elements"]; }
        }
    }

    public class DevConfigurationSection : ConfigurationSection
    {
        public DevConfigurationSection()
        {
            this["elements"] = new KeyValueConfigurationCollection();
        }      

        [ConfigurationProperty("elements")]
        public KeyValueConfigurationCollection Elements
        {
            get { return (KeyValueConfigurationCollection)this["elements"]; }
        }
    }

    public class TestConfigurationSection : ConfigurationSection
    {
        public TestConfigurationSection()
        {
            this["elements"] = new KeyValueConfigurationCollection();
        }

        [ConfigurationProperty("elements")]
        public KeyValueConfigurationCollection Elements
        {
            get { return (KeyValueConfigurationCollection)this["elements"]; }
        }
    }

    public class ProdConfigurationSection : ConfigurationSection
    {
        public ProdConfigurationSection()
        {
            this["elements"] = new KeyValueConfigurationCollection();
        }

        [ConfigurationProperty("elements")]
        public KeyValueConfigurationCollection Elements
        {
            get { return (KeyValueConfigurationCollection)this["elements"]; }
        }
    }
}