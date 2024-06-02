using System;
using System.Collections.Generic;

namespace FIMSpace.AnimationTools
{
    /// <summary>
    /// Class which holds most important definition for chains of transforms/bones
    /// </summary>
    [System.Serializable]
    public class BodyExtraBoneChainsInfo : BodyLimbInfo
    {
        public List<BodyExtraChainReference> ExtraChains = new List<BodyExtraChainReference>();

        public BodyExtraChainReference GetChain(int index)
        {
            if (index < 0) return null;
            if (index >= ExtraChains.Count) return null;
            return ExtraChains[index];
        }

        public override int CountBones()
        {
            int count = 0;
            for (int i = 0; i < ExtraChains.Count; i += 1) count += ExtraChains[i].CountBones();
            return count;
        }

        public void IterateBones(Action<IBodyBone> boneAction)
        {
            for (int i = 0; i < ExtraChains.Count; i++) ExtraChains[i].IterateBones(boneAction);
        }

        #region Get Chains by names utils

        public BodyExtraChainReference GetChainByName(string name)
        {
            if (_chainsDictio == null) InitChainsDictionary();
            BodyExtraChainReference chain;
            if (_chainsDictio.TryGetValue(name, out chain)) return chain;
            return chain;
        }

        public BodyExtraChainReference GetChainByNameIgnoreCaseAndSpaces(string name)
        {
            string lowercasename = name.ToLower().Replace(" ", "");

            for (int i = 0; i < ExtraChains.Count; i++)
            {
                if (ExtraChains[i].ChainNameID.ToLower().Replace(" ", "") == lowercasename) return ExtraChains[i];
            }

            return null;
        }


        Dictionary<string, BodyExtraChainReference> _chainsDictio = null;
        void InitChainsDictionary()
        {
            _chainsDictio = new Dictionary<string, BodyExtraChainReference>();
            for (int i = 0; i < ExtraChains.Count; i++)
            {
                string tgtName = ExtraChains[i].ChainNameID;
                if (_chainsDictio.ContainsKey(tgtName)) continue; // Prevent duplicates
                _chainsDictio.Add(tgtName, ExtraChains[i]);
            }
        }

        #endregion

    }
}