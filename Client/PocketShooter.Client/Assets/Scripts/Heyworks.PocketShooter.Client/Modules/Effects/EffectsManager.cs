using System.Collections.Generic;
using Heyworks.PocketShooter.Core.SchedulerManager;
using Heyworks.PocketShooter.Singleton;
using Heyworks.PocketShooter.Utils;
using Lean.Pool;
using UnityEngine;

namespace Heyworks.PocketShooter.Modules.EffectsManager
{
    public class EffectsManager : Singleton<EffectsManager>
    {
        [SerializeField]
        private List<EffectCategoryInfo> effectCategoryInfos;

        private Dictionary<int, EffectTypeInfo> effectsTypeInfoDictionary;

        public Dictionary<int, EffectTypeInfo> EffectsTypeInfoDictionary
        {
            get
            {
                if (effectsTypeInfoDictionary == null)
                {
                    effectsTypeInfoDictionary = new Dictionary<int, EffectTypeInfo>();

                    for (int j = 0; j < effectCategoryInfos.Count; j++)
                    {
                        var effectInfo = effectCategoryInfos[j];

                        for (int i = 0; i < effectInfo.EffectTypeInfos.Count; i++)
                        {
                            effectsTypeInfoDictionary.Add((int)effectInfo.EffectTypeInfos[i].EffectType, effectInfo.EffectTypeInfos[i]);
                        }
                    }
                }

                return effectsTypeInfoDictionary;
            }
        }

        public GameObject PlayEffect(EffectType effectType, Transform parent, bool isAutoDestroy = false)
        {
            return CreateEffect(effectType, parent, Vector3.zero, Quaternion.identity, Vector3.one, isAutoDestroy);
        }

        public GameObject PlayEffect(EffectType effectType, Transform parent, Vector3 localPosition, bool isAutoDestroy = false)
        {
            return CreateEffect(effectType, parent, localPosition, Quaternion.identity, Vector3.one, isAutoDestroy);
        }

        public GameObject PlayEffect(EffectType effectType, Vector3 worldPosition, bool isAutoDestroy = false, Transform parent = null)
        {
            GameObject effect = CreateEffect(effectType, parent, Vector3.zero, Quaternion.identity, Vector3.one, isAutoDestroy);
            effect.transform.position = worldPosition;

            return effect;
        }

        public void StopEffect(GameObject effect, bool isImmediately = true)
        {
            if (effect != null && effect.activeSelf)
            {
                if (isImmediately)
                {
                    VFXAutoDestroyer vFXAutoDestroyer = effect.GetComponent<VFXAutoDestroyer>();
                    if (vFXAutoDestroyer != null)
                    {
                        Destroy(vFXAutoDestroyer);
                    }

                    LeanPool.Despawn(effect);
                }
                else
                {
                    if (effect.GetComponent<VFXAutoDestroyer>() == null)
                    {
                        effect.AddComponent<VFXAutoDestroyer>();
                    }

                    effect.GetComponent<ParticleSystem>().Stop(true);
                }
            }
        }

        GameObject CreateEffect(EffectType effectType, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, bool isAutoDestroy = false)
        {
            EffectTypeInfo effectTypeInfo = EffectsTypeInfoDictionary[(int)effectType];
            GameObject effectPrefab = effectTypeInfo.EffectPrefab;

            if (effectPrefab != null)
            {
                LeanGameObjectPool lgop = LeanGameObjectPool.FindPoolByPrefab(effectPrefab);
                if (lgop == null)
                {
                    lgop = gameObject.AddComponent<LeanGameObjectPool>();
                    lgop.Prefab = effectPrefab;
                }

                GameObject effect = lgop.Spawn(localPosition, localRotation, parent);

                // Reset effect
                ParticleSystem[] systems = effect.GetComponentsInChildren<ParticleSystem>();
                for (int i = 0, l = systems.Length; i < l; i++)
                {
                    var system = systems[i];
                    system.Clear();
                }

                // Reset trails
                TrailRenderer[] trails = effect.GetComponentsInChildren<TrailRenderer>();
                for (int i = 0, n = trails.Length; i < n; i++)
                {
                    trails[i].Clear();
                }

                effect.transform.parent = parent;
                effect.transform.localPosition = localPosition;
                effect.transform.localRotation = localRotation;
                effect.transform.localScale = localScale;

                if (isAutoDestroy)
                {
                    if (effect.GetComponent<VFXAutoDestroyer>() == null)
                    {
                        effect.AddComponent<VFXAutoDestroyer>();
                    }
                    //else
                    //{
                    //    SchedulerManager.Instance.CallActionWithDelay(this, () =>
                    //    {
                    //        StopEffect(effect);
                    //    }, effectTypeInfo.TimeAfterDestroyEffect);
                    //}
                }

                return effect;
            }
            else
            {
                throw new System.Exception("missing effect : " + effectType);
            }
        }
    }

    [System.Serializable]
    public struct EffectTypeInfo
    {
        [SerializeField]
        private EffectType effectType;
        [SerializeField]
        private GameObject effectPrefab;
        //[SerializeField]
        //private float timeAfterDestroyEffect;

        public EffectType EffectType => effectType;

        public GameObject EffectPrefab => effectPrefab;

        //public float TimeAfterDestroyEffect => timeAfterDestroyEffect;
    }

    [System.Serializable]
    public struct EffectCategoryInfo
    {
        [SerializeField]
        private string categoryName;

        [EnumArray("effectType", typeof(EffectType))]
        [SerializeField]
        private List<EffectTypeInfo> effectTypeInfos;

        public List<EffectTypeInfo> EffectTypeInfos => effectTypeInfos;
    }
}