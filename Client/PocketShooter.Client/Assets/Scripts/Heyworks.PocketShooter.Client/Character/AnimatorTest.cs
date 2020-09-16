using System.Collections.Generic;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Weapons;
using Microsoft.Extensions.Logging;
using UnityEngine;

namespace Heyworks.PocketShooter.Character
{
    public class AnimatorTest : MonoBehaviour
    {
        [System.Serializable]
        private class TrooperWeapon
        {
            public TrooperAvatar Avatar;
            public WeaponName WeaponIdentifer;
        }

        private const string SpeedXParameter = "SpeedX";
        private const string SpeedZParameter = "SpeedZ";

        [SerializeField]
        private float currentRightSpeed;

        [SerializeField]
        private float currentForwardSpeed;

        [SerializeField]
        private List<TrooperWeapon> weaponsTable;

        [SerializeField]
        private WeaponsConfig weaponsConfig;

        private List<Animator> animators = new List<Animator>();
        private List<AnimatorControllerParameter> parameters = new List<AnimatorControllerParameter>();
        private Dictionary<string, object> values = new Dictionary<string, object>();

        private void Awake()
        {
            var options = new LoggerFilterOptions();
            var providers = new ILoggerProvider[]
            {
                new ConsoleLoggerProvider(),
                new FileLoggerProvider(new FileLoggerSettings("PocketShooter.log")),
            };

            var configuration = new LoggerConfiguration();
            var loggerFactory = new LoggerFactory(providers, new LoggerFilterOptionsMonitor(options, configuration));

            MLog.Setup(loggerFactory, options, configuration);

            GetComponentsInChildren(animators);

            if (animators.Count > 0)
            {
                var any = animators[0];
                if (any)
                {
                    for (int i = 0; i < any.parameterCount; i++)
                    {
                        AnimatorControllerParameter p = any.GetParameter(i);
                        parameters.Add(p);
                        switch (p.type)
                        {
                            case AnimatorControllerParameterType.Int:
                                values.Add(p.name, p.defaultInt);
                                break;
                            case AnimatorControllerParameterType.Bool:
                                values.Add(p.name, p.defaultBool);
                                break;
                            case AnimatorControllerParameterType.Trigger:
                                values.Add(p.name, p.defaultBool);
                                break;
                            case AnimatorControllerParameterType.Float:
                                values.Add(p.name, p.defaultFloat);
                                break;
                        }
                    }
                }
            }

            var avatars = GetComponentsInChildren<TrooperAvatar>();
            foreach (TrooperAvatar avatar in avatars)
            {
                TrooperWeapon trooperWeapon = weaponsTable.Find(x => x.Avatar == avatar);
                if (trooperWeapon != null)
                {
                    WeaponViewConfig weaponConfig = weaponsConfig.GetWeaponByName(trooperWeapon.WeaponIdentifer);
                    Instantiate(weaponConfig.View, avatar.WeaponViewParent);
                }
            }
        }

        private void Update()
        {
            animators.ForEach(SetSpeed);
        }

        private void OnGUI()
        {
            int scale = 2;
            GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
            GUILayout.BeginArea(new Rect(Screen.width / scale - 100, 30, 100, 500));
            {
                GUILayout.BeginVertical();
                {
                    if (animators.Count > 0)
                    {
                        Animator animator = animators[0];
                        for (int i = 1; i < animator.layerCount; i++)
                        {
                            float weight = animator.GetLayerWeight(i);
                            {
                                GUILayout.Label(animator.GetLayerName(i), GUILayout.MaxWidth(100));
                                weight = GUILayout.HorizontalSlider(weight, 0, 1);
                            }

                            foreach (Animator a in animators)
                            {
                                a.SetLayerWeight(i, weight);
                            }
                        }
                    }
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndArea();

            GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
            GUILayout.BeginArea(new Rect(30, 30, 100, 500));
            {
                GUILayout.BeginVertical();
                {
                    currentRightSpeed = GUILayout.HorizontalSlider(currentRightSpeed, -1, 1);
                    currentForwardSpeed = GUILayout.HorizontalSlider(currentForwardSpeed, -1, 1);

                    foreach (var p in parameters)
                    {
                        switch (p.type)
                        {
                            case AnimatorControllerParameterType.Int:
                                break;
                            case AnimatorControllerParameterType.Bool:
                                bool bvalue = (bool)values[p.name];
                                bvalue = GUILayout.Toggle(bvalue, p.name);
                                values[p.name] = bvalue;
                                animators.ForEach(
                                    (animator) => { animator.SetBool(p.name, bvalue); });
                                break;
                            case AnimatorControllerParameterType.Trigger:
                                if (GUILayout.Button(p.name))
                                {
                                    animators.ForEach(
                                        (animator) => { animator.SetTrigger(p.name); });
                                }

                                break;
                            case AnimatorControllerParameterType.Float:
                                break;
                        }
                    }
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndArea();
        }

        private void SetSpeed(Animator animator)
        {
            animator.SetFloat(SpeedXParameter, currentRightSpeed);
            animator.SetFloat(SpeedZParameter, currentForwardSpeed);
        }
    }
}