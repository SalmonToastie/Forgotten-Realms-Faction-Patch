using ForgottenRealms;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace ForgottenRealmsFactionPatch
{
    public class ForgottenRealmsPatch : Mod
    {
        public ForgottenRealmsPatch(ModContentPack content) : base(content)
        {
            new Harmony("ForgottenRealmsFactionPatch.Mod").PatchAll();
        }
    }

    [HarmonyPatch(typeof(PatchOperation_OnDemand), "LoadDefsInto")]
    public static class PatchOperation_OnDemand_Patch
    {
        public static void Postfix(PatchOperation_OnDemand __instance, ref bool __result, XmlDocument xml, ModContentPack content, string folder)
        {
            if (!__result)
            {
                content = LoadedModManager.GetMod<ForgottenRealmsPatch>().Content;
                string text = Path.Combine(Path.Combine(content.RootDir, __instance.source), folder);
                if (PatchOperation_OnDemand.loaded.Contains(text))
                {
                    __result = false;
                    return;
                }
                if (!Directory.Exists(text))
                {
                    if (!__instance.autoload)
                    {
                        Log.Warning(content.Name + " is trying to load non-existant folder " + __instance.source + "/" + folder);
                    }
                    __result = false;
                    return;
                }
                foreach (XmlNode childNode in LoadedModManager.CombineIntoUnifiedXML(DirectXmlLoader.XmlAssetsInModFolder(content, Path.Combine(__instance.source, folder)).ToList(),
                    new Dictionary<XmlNode, LoadableXmlAsset>()).DocumentElement.ChildNodes)
                {
                    xml.DocumentElement.AppendChild(xml.ImportNode(childNode, deep: true));
                }
                PatchOperation_OnDemand.loaded.Add(text);
                __result = true;
            }
        }
    }

}
