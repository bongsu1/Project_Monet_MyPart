using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BS
{
    public static class NPCDataTable
    {
        private static Dictionary<string, Dictionary<string, object>> npcDT;
        private static Dictionary<string, Dictionary<string, object>> soliloquyDT;
        private static Dictionary<string, Dictionary<string, object>> conversationDT;
        private static Dictionary<string, Dictionary<string, object>> voiceDT;
        private static Dictionary<string, Dictionary<string, object>> convertEffectDT;

        private static bool isNPCDTComplete = false;
        private static bool isSoliloquyDTComplete = false;
        private static bool isConversationDTComplete = false;
        private static bool isVoiceDTComplete = false;
        private static bool isConvertEffectDTComplete = false;

        public static Dictionary<string, Dictionary<string, object>> GetDataTable(DTType type, string path)
        {
            switch (type)
            {
                case DTType.NPC:
                    if (isNPCDTComplete)
                        return npcDT;
                    else
                        return DTLoading(type, path);

                case DTType.Soliloquy:
                    if (isSoliloquyDTComplete)
                        return soliloquyDT;
                    else
                        return DTLoading(type, path);

                case DTType.Conversation:
                    if (isConversationDTComplete)
                        return conversationDT;
                    else
                        return DTLoading(type, path);

                case DTType.Voice:
                    if (isVoiceDTComplete)
                        return voiceDT;
                    else
                        return DTLoading(type, path);

                case DTType.ConverEffect:
                    if (isConvertEffectDTComplete)
                        return convertEffectDT;
                    else
                        return DTLoading(type, path);

                default:
                    return null;
            }
        }

        private static Dictionary<string, Dictionary<string, object>> DTLoading(DTType type, string path)
        {
/*#if UNITY_EDITOR
            string combinePath = Path.Combine(Application.dataPath, $"Resources/{path}.txt");
#else
            string combinePath = Path.Combine(Application.persistentDataPath, $"Resources/{path}.txt");
#endif
            if (!File.Exists(combinePath))
                return null;*/

            switch (type)
            {
                case DTType.NPC:
                    npcDT = CSVReader.ReadTSV(path);
                    isNPCDTComplete = true;
                    return npcDT;

                case DTType.Soliloquy:
                    soliloquyDT = CSVReader.ReadTSV(path);
                    isSoliloquyDTComplete = true;
                    return soliloquyDT;

                case DTType.Conversation:
                    conversationDT = CSVReader.ReadTSV(path);
                    isConversationDTComplete = true;
                    return conversationDT;

                case DTType.Voice:
                    voiceDT = CSVReader.ReadTSV(path);
                    isVoiceDTComplete = true;
                    return voiceDT;

                case DTType.ConverEffect:
                    convertEffectDT = CSVReader.ReadTSV(path);
                    isConvertEffectDTComplete = true;
                    return convertEffectDT;

                default:
                    return null;
            }
        }
    }

    public enum DTType { NPC, Soliloquy, Conversation, Voice, ConverEffect }
}
