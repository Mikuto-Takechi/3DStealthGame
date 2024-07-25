using System;
using System.Linq;
using UnityEngine;

namespace MonstersDomain
{
    /// <summary>
    /// 足音関連の処理を行うクラス。
    /// </summary>
    public class FootStepsSource : MonoBehaviour
    {
        [SerializeField, Tooltip("しゃがみ歩き時の音の距離、半径")] float _crouchSoundDistance = 1;
        [SerializeField, Tooltip("歩き時の音の距離、半径")] float _walkSoundDistance = 5;
        [SerializeField, Tooltip("走り時の音の距離、半径")] float _runSoundDistance = 15;
        [SerializeField, Tooltip("しゃがみ音の間隔")] float _crouchInterval = 0.2f;
        [SerializeField, Tooltip("歩行音の間隔")] float _walkInterval = 0.2f;
        [SerializeField, Tooltip("ダッシュ音の間隔")] float _runInterval = 0.2f;
        [SerializeField] FootSteps[] _terrainSteps;
        [SerializeField] TagAndFootSteps[] _tagAndFootSteps;
        float _footStepsTimer;
        /// <summary>
        /// タグやTerrainの情報からどの地面かを判定する。
        /// </summary>
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

        public void UpdateFootSteps(PlayerState state, bool isMove, CheckGround checkGround)
        {
            if (!checkGround.IsGrounded.Value || !isMove)
            {
                _footStepsTimer = 0f;
                return;
            }
            
            switch (state)
            {
                case PlayerState.Crouch:
                    if (_footStepsTimer <= _crouchInterval) break;
                    _footStepsTimer = 0f;
                    AudioManager.Instance.PlayFootSteps(CheckGroundType(checkGround.HitGameObject), 0.8f, 1f);
                    HearingManager.Instance.OnSoundEmitted(transform.position, _crouchSoundDistance);
                    break;
                case PlayerState.Walk:
                    if (_footStepsTimer <= _walkInterval) break;
                    _footStepsTimer = 0f;
                    AudioManager.Instance.PlayFootSteps(CheckGroundType(checkGround.HitGameObject), 1f, 1f);
                    HearingManager.Instance.OnSoundEmitted(transform.position, _walkSoundDistance);
                    break;
                case PlayerState.Run:
                    if (_footStepsTimer <= _runInterval) break;
                    _footStepsTimer = 0f;
                    AudioManager.Instance.PlayFootSteps(CheckGroundType(checkGround.HitGameObject), 1.4f, 1f);
                    HearingManager.Instance.OnSoundEmitted(transform.position, _runSoundDistance);
                    break;
            }
            _footStepsTimer += Time.deltaTime;
        }
        /// <summary>
        /// 地面に合ったジャンプ音を再生する。
        /// </summary>
        public void PlayJumpingSE(GameObject ground, float pitch, float volume)
        {
            FootSteps jumpType = CheckGroundType(ground) switch
            {
                FootSteps.Concrete => FootSteps.RockJump,
                FootSteps.Grass => FootSteps.GrassJump,
                FootSteps.Indoor => FootSteps.WoodJump,
                FootSteps.Dirt => FootSteps.DirtJump,
                _ => throw new ArgumentOutOfRangeException()
            };
            AudioManager.Instance.PlayFootSteps(jumpType, pitch, volume);
        }
        /// <summary>
        /// 地面に合った着地音を再生する。
        /// </summary>
        public void PlayLandingSE(GameObject ground, float pitch, float volume)
        {
            FootSteps landingType = CheckGroundType(ground) switch
            {
                FootSteps.Concrete => FootSteps.RockLand,
                FootSteps.Grass => FootSteps.GrassLand,
                FootSteps.Indoor => FootSteps.WoodLand,
                FootSteps.Dirt => FootSteps.DirtLand,
                _ => throw new ArgumentOutOfRangeException()
            };
            AudioManager.Instance.PlayFootSteps(landingType, pitch, volume);
        }
    }
}