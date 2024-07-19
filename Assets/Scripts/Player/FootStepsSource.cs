using System.Linq;
using UnityEngine;

namespace MonstersDomain
{
    public class FootStepsSource : MonoBehaviour
    {
        [SerializeField] FootSteps[] _terrainSteps;
        [SerializeField] TagAndFootSteps[] _tagAndFootSteps;

        FootSteps CheckGroundType(GameObject go)
        {
            if (!go)
            {
                return FootSteps.Indoor;
            }
            if (go.TryGetComponent(out Terrain terrain))
            {
                var pos = transform.position - terrain.transform.position;
                var tData = terrain.terrainData;
                var offsetX = (int)(tData.alphamapWidth * pos.x / tData.size.x);
                var offsetZ = (int)(tData.alphamapHeight * pos.z / tData.size.z);
                var alphaMaps = tData.GetAlphamaps(offsetX, offsetZ, 1, 1);

                //  地面のLayerの比率を取得
                var weights = alphaMaps.OfType<float>().ToArray();
                //  比率が最大の物のIndexを取得
                var maxIndex = weights.ToList().IndexOf(weights.Max());
                return _terrainSteps[maxIndex];
            }

            foreach (var pair in _tagAndFootSteps)
            {
                if (go.CompareTag(pair.Tag))
                {
                    return pair.FootSteps;
                }
            }
            
            return FootSteps.Indoor;
        }
        public void PlayFootSteps(GameObject ground, float pitch, float volume)
        {
            AudioManager.Instance.PlayFootSteps(CheckGroundType(ground), pitch, volume);
        }

        public void PlayJumpingSE(GameObject ground, float pitch, float volume)
        {
            switch (CheckGroundType(ground))
            {
                case FootSteps.Concrete :
                    AudioManager.Instance.PlayFootSteps(FootSteps.RockJump, pitch, volume);
                    break;
                case FootSteps.Grass :
                    AudioManager.Instance.PlayFootSteps(FootSteps.GrassJump, pitch, volume);
                    break;
                case FootSteps.Indoor :
                    AudioManager.Instance.PlayFootSteps(FootSteps.WoodJump, pitch, volume);
                    break;
                case FootSteps.Dirt :
                    AudioManager.Instance.PlayFootSteps(FootSteps.DirtJump, pitch, volume);
                    break;
            }
        }

        public void PlayLandingSE(GameObject ground, float pitch, float volume)
        {
            switch (CheckGroundType(ground))
            {
                case FootSteps.Concrete :
                    AudioManager.Instance.PlayFootSteps(FootSteps.RockLand, pitch, volume);
                    break;
                case FootSteps.Grass :
                    AudioManager.Instance.PlayFootSteps(FootSteps.GrassLand, pitch, volume);
                    break;
                case FootSteps.Indoor :
                    AudioManager.Instance.PlayFootSteps(FootSteps.WoodLand, pitch, volume);
                    break;
                case FootSteps.Dirt :
                    AudioManager.Instance.PlayFootSteps(FootSteps.DirtLand, pitch, volume);
                    break;
            }
        }
    }
}