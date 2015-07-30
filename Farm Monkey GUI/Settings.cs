/*
 * Created by SharpDevelop.
 * User: amanda
 * Date: 07/27/2015
 * Time: 15:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;   
using System.Configuration;

namespace FarmMonkey_GUI
{
    /// <summary>
    /// Configuration section &lt;Settings&gt;
    /// </summary>
    /// <remarks>
    /// Assign properties to your child class that has the attribute 
    /// <c>[ConfigurationProperty]</c> to store said properties in the xml.
    /// </remarks>
    public sealed class Settings : ConfigurationSection
    {
        System.Configuration.Configuration _Config;


        #region ConfigurationProperties

        /// <summary>
        /// Set to true if you have a gps file for moving to and from the safe.
        /// </summary>
        [ConfigurationProperty("EnableGPS", DefaultValue = false)]
        public bool EnableGPS
        {
            get { return (bool)this["EnableGPS"]; }
            set { this["EnableGPS"] = value; }
        }

        /// <summary>
        /// Set to true if your gps file has paths from the Nui ( generally not necessary in safe zone farming )
        /// </summary>
        [ConfigurationProperty("DeathCheck", DefaultValue = false)]
        public bool DeathCheck
        {
            get { return (bool)this["DeathCheck"]; }
            set { this["DeathCheck"] = value; }
        }

        /// <summary>
        /// This gps file needs 2 points " Safe " & " Farm "
        /// </summary>
        [ConfigurationProperty("GpsFile", DefaultValue = "")]
        public string GpsFile
        {
            get { return (string)this["GpsFile"]; }
            set { this["GpsFile"] = value; }
        }

        /// <summary>
        /// Minimum Labor for harvesting
        /// </summary>
        [ConfigurationProperty("MinLabor", DefaultValue = 20)]
        public uint MinLabor
        {
            get { return (uint)this["MinLabor"]; }
            set { this["MinLabor"] = value; }
        }

        #endregion

        /// <summary>
        /// Private Constructor used by our factory method.
        /// </summary>
        private Settings () : base () {
            // Allow this section to be stored in user.app. By default this is forbidden.
            this.SectionInformation.AllowExeDefinition =
                ConfigurationAllowExeDefinition.MachineToLocalUser;
        }

        #region Public Methods
        
        /// <summary>
        /// Saves the configuration to the config file.
        /// </summary>
        public void Save() {
            _Config.Save();
        }
        
        #endregion
        
        #region Static Members
        
        /// <summary>
        /// Gets the current applications &lt;Settings&gt; section.
        /// </summary>
        /// <param name="ConfigLevel">
        /// The &lt;ConfigurationUserLevel&gt; that the config file
        /// is retrieved from.
        /// </param>
        /// <returns>
        /// The configuration file's &lt;Settings&gt; section.
        /// </returns>
        public static Settings GetSection (ConfigurationUserLevel ConfigLevel) {
            /* 
             * This class is setup using a factory pattern that forces you to
             * name the section &lt;Settings&gt; in the config file.
             * If you would prefer to be able to specify the name of the section,
             * then remove this method and mark the constructor public.
             */ 
            System.Configuration.Configuration Config = ConfigurationManager.OpenExeConfiguration
                (ConfigLevel);
            Settings oSettings;
            
            oSettings =
                (Settings)Config.GetSection("Settings");
            if (oSettings == null) {
                oSettings = new Settings();
                Config.Sections.Add("SettingsSettings", oSettings);
            }
            oSettings._Config = Config;
            
            return oSettings;
        }
        
        #endregion
    }
}

