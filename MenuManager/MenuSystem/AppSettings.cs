//  http://wellroundedgeek.com/post/2011/01/25/Simple-XNA-Cross-Platform-Settings-Manager.aspx

#region usings

using System.IO;

using System.IO.IsolatedStorage;

using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;


#endregion


//namespace ScreenSystem
namespace MenuSystem
{
    #region application settings



    public class AppSettings
    {

        public enum GraphicsDetailLevels
        {
            Low,
            Medium,
            Full,
        }


        ///  GRAPHICS ///////////////////////
        public string Resolution { get; set; }

        public GraphicsDetailLevels GraphicsDetailLevel { get; set; }

        public bool IsFullScreen { get; set; }


        /// SOUND ///////////////////////////
        public float VolumeMain { get; set; }

        public bool EnableMusic { get; set; }

        public bool EnableSfx { get; set; }

        

        /// GAME FUNCTIONS ////////////////////
        public bool HasRunOnce { get; set; }

        public bool InventorySelectOnPickupAuto { get; set; }

        public AppSettings()
        {

            // Create our default settings

            HasRunOnce = true;

            InventorySelectOnPickupAuto = false;

            EnableMusic = true;

            EnableSfx = true;

            Resolution = "800x480";

            VolumeMain = 7.0f;

            GraphicsDetailLevel = GraphicsDetailLevels.Medium;

            IsFullScreen = false;


        }

    }



    #endregion





    #region settings manager



    public static class SettingsManager
    {



        private static string fileName = "settings.xml";

        public static AppSettings Settings = new AppSettings();





        public static void LoadSettings()
        {

            // Create our exposed settings class. This class gets serialized to load/save the settings.

            Settings = new AppSettings();

            //Obtain a virtual store for application


            IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForDomain();



            // Check if file is there

            if (fileStorage.FileExists(fileName))
            {

                XmlSerializer serializer = new XmlSerializer(Settings.GetType());

                StreamReader stream = new StreamReader(new IsolatedStorageFileStream(fileName, FileMode.Open, fileStorage));

                try
                {

                    Settings = (AppSettings)serializer.Deserialize(stream);

                    stream.Close();

                }

                catch
                {

                    // An error occurred so let's use the default settings.

                    stream.Close();

                    Settings = new AppSettings();

                    // Saving is optional - in this sample we assume it works and the error is due to the file not being there.

                    SaveSettings();

                    // Handle other errors here

                }

            }

            else
            {

                SaveSettings();

            }

        }





        public static void SaveSettings()
        {

            //Obtain a virtual store for application


            IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForDomain();


            XmlSerializer serializer = new XmlSerializer(Settings.GetType());

            StreamWriter stream = new StreamWriter(new IsolatedStorageFileStream(fileName, FileMode.Create, fileStorage));

            try
            {

                serializer.Serialize(stream, Settings);

            }

            catch
            {

                // Handle your errors here

            }

            stream.Close();

        }


        public static Viewport GetResolutionViewport(string vResolutionString)
        {
            return new Viewport(0, 0, GetResolutionWidth(vResolutionString), GetResolutionHeight(vResolutionString));
        }


        public static int GetResolutionWidth(string vResolutionString)
        {
            string[] res = vResolutionString.Split('x');

            return int.Parse(res[0]);

            // TODO: check for invalid resultions
        }

        public static int GetResolutionHeight(string vResolutionString)
        {
            string[] res = vResolutionString.Split('x');

            return int.Parse(res[1]);

            // TODO: check for invalid resultions
        }

        public static float GetResolutionScale(string vResolutionString)
        {
            string[] res = vResolutionString.Split('x');

            //use 1024 as our "BASE" height
            return int.Parse(res[1]) / 1024.0f;

        }


    }


    


    #endregion
}
