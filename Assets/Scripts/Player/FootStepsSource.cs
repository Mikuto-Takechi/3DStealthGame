using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonstersDomain
{
    public class FootStepsSource : MonoBehaviour
    {
        [SerializeField] FootSteps[] _terrainSteps;
        [SerializeField] TagAndFootSteps[] _tagAndFootSteps;
        readonly HashSet<GameObject> _collisionObjects = new();
        readonly HashSet<Terrain> _collisionTerrains = new();

        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent(out Terrain terrain)) _collisionTerrains.Add(terrain);

            foreach (var pair in _tagAndFootSteps)
                if (other.gameObject.CompareTag(pair.Tag))
                {
                    _collisionObjects.Add(other.gameObject);
                    break;
                }
        }

        void OnCollisionExit(Collision other)
        {
            if (other.gameObject.TryGetComponent(out Terrain terrain)) _collisionTerrains.Remove(terrain);
            foreach (var pair in _tagAndFootSteps)
                if (other.gameObject.CompareTag(pair.Tag))
                {
                    _collisionObjects.Remove(other.gameObject);
                    break;
                }
        }

        public void PlayFootSteps(float pitch, float volume)
        {
            var footTerrain = _collisionTerrains.LastOrDefault();
            var footGameObject = _collisionObjects.LastOrDefault();
            if (footTerrain)
            {
                var pos = transform.position - footTerrain.transform.position;
                var tData = footTerrain.terrainData;
                var offsetX = (int)(tData.alphamapWidth * pos.x / tData.size.x);
                var offsetZ = (int)(tData.alphamapHeight * pos.z / tData.size.z);
                var alphaMaps = tData.GetAlphamaps(offsetX, offsetZ, 1, 1);

                //  地面のLayerの比率を取得
                var weights = alphaMaps.OfType<float>().ToArray();
                //  比率が最大の物のIndexを取得
                var maxIndex = weights.ToList().IndexOf(weights.Max());
                AudioManager.Instance.PlayFootSteps(_terrainSteps[maxIndex], pitch, volume);
            }
            else if (footGameObject)
            {
                var footSteps = _tagAndFootSteps.First(pair => footGameObject.CompareTag(pair.Tag)).FootSteps;
                AudioManager.Instance.PlayFootSteps(footSteps, pitch, volume);
            }
        }
    }
}