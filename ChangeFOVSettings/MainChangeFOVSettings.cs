using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using R2API;
using R2API.Utils;
using RoR2;

namespace ChangeFOVSettings
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(PrefabAPI))]
    public class MainChangeFOVSettings : BaseUnityPlugin
    {

        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "KosmosisDire";
        public const string PluginName = "ChangeFOVSettings";
        public const string PluginVersion = "1.0.0";

        //debugging config
        public static ConfigEntry<bool> consoleLoggingConfig;
        public static ConfigEntry<bool> chatLoggingConfig;
        public static ConfigEntry<bool> fileLoggingConfig;
        public static ConfigEntry<string> fileLoggingPath;
        public static bool consoleLogging = true;
        public static bool chatLogging = true;
        public static bool fileLogging = false;
        
        public static ManualLogSource log;
        public static bool runStarted;

        //configurable variables
        CameraRigController currentCamera;
        MenuSlider FOVSlider;
        MenuCheckbox SprintFOVEffectsCheckbox;

        public void Awake()
        {
            Log.Init(Logger);
            FOVSlider = new MenuSlider(60, 120, 50, true, "Base Camera FOV", "The base camera FOV to use.", SubPanel.Gameplay);
            FOVSlider.OnSliderChanged += (newValue) => { if (currentCamera) currentCamera.baseFov = newValue; };
            SprintFOVEffectsCheckbox = new MenuCheckbox(true, "Sprinting Increases FOV", "Enables an increase in FOV while sprinting.", SubPanel.Gameplay);

            On.RoR2.CameraRigController.OnEnable += hook_OnCameraEnable;
        }

        void hook_OnCameraEnable(On.RoR2.CameraRigController.orig_OnEnable orig, CameraRigController self) 
        {
            orig(self);
            currentCamera = self;
            currentCamera.baseFov = FOVSlider.GetValue();
        }

        void Update()
        {
            if (!SprintFOVEffectsCheckbox.GetValue())
            {
                if (RoR2.Run.instance) 
                {
                    currentCamera.SetFieldValue("fovVelocity", 0f);
                }
            }
        }
    }
}
