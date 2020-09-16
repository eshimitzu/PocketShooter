using System;
using System.Collections.Generic;
using System.ComponentModel;
using Heyworks.PocketShooter.Communication;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Core;
using Heyworks.PocketShooter.Networking;
using UniRx.Async;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Heyworks.PocketShooter.Diagnostics
{
    /// <summary>
    /// SR Server options.
    /// </summary>
    public class SRServerOptions : IDisposable
    {
        private const string LastServer = "LastServer";

        private RealtimeRunBehavior realtimeRunBehavior;
        private Main main;
        private IAppConfiguration appConfiguration;

        private SRServersProvider serversProvider;

        public SRServerOptions(
            RealtimeRunBehavior realtimeRunBehavior,
            Main main,
            IAppConfiguration appConfiguration)
        {
            this.realtimeRunBehavior = realtimeRunBehavior;
            this.main = main;
            this.appConfiguration = appConfiguration;

            serversProvider = new SRServersProvider();
            serversProvider.Load();

            serversProvider.Add(new SRServerAddress("127.0.0.1:5000", "Local"));
            serversProvider.Add(new SRServerAddress("18.223.111.37:5000", "Dev"));

            serversProvider.Save();

            foreach (var item in serversProvider.Servers)
            {
                item.OnClick += ServerOnClick;
                SRDebug.Instance.AddOptionContainer(item);
            }
        }

        public void Dispose()
        {
            foreach (var item in serversProvider.Servers)
            {
                SRDebug.Instance.RemoveOptionContainer(item);
            }
        }

        /// <summary>
        /// Gets or sets the ip.
        /// </summary>
        [Category("Server")]
        [SROptions.Sort(2)]
        public string CustomIp { get; set; } = "127.0.0.1";

        private string CustomServerAddress => CustomIp + ":5000";

        /// <summary>
        /// Connects to default.
        /// </summary>
        [SROptions.DisplayName("Default")]
        [Category("Server")]
        [SROptions.Sort(-1)]
        public async void ConnectToDefault()
        {
            PlayerPrefs.SetString(LastServer, null);
            PlayerPrefs.Save();

            await Connect(appConfiguration.MetaServerAddress);
        }

        /// <summary>
        /// Connect to the specified IP.
        /// </summary>
        [Category("Server")]
        [SROptions.Sort(3)]
        public async void Connect()
        {
            var serverAddress = new SRServerAddress(CustomServerAddress, CustomIp);
            if (serversProvider.Add(serverAddress))
            {
                serverAddress.OnClick += ServerOnClick;
                SRDebug.Instance.AddOptionContainer(serverAddress);
                serversProvider.Save();
            }

            await Connect(new ServerAddress(CustomServerAddress));
        }

        private async void ServerOnClick(string server)
        {
            PlayerPrefs.SetString(LastServer, server);
            PlayerPrefs.Save();

            await Connect(new ServerAddress(server));
        }

        private async UniTask Connect(ServerAddress address)
        {
            ConfigurationProvider.SetDebugServerAddress(address);

            SceneManager.LoadScene(0);
        }

        [System.Serializable]
        private class SRServersProvider
        {
            private const string ServerConfig = "ServerConfig";

            [SerializeField]
            private List<SRServerAddress> servers = new List<SRServerAddress>();

            public IEnumerable<SRServerAddress> Servers => servers;

            public bool Add(SRServerAddress server)
            {
                var s = servers.Find(f => { return (f.Name == server.Name); });
                if (s == null)
                {
                    servers.Add(server);
                    return true;
                }

                return false;
            }

            public void Load()
            {
                string json = PlayerPrefs.GetString(ServerConfig);
                JsonUtility.FromJsonOverwrite(json, this);
            }

            public void Save()
            {
                string json = JsonUtility.ToJson(this);
                PlayerPrefs.SetString(ServerConfig, json);
                PlayerPrefs.Save();
            }
        }

        [System.Serializable]
        private class SRServerAddress
        {
            [SerializeField]
            private string server;

            [SerializeField]
            private string name;

            internal string Server => server;

            internal string Name => name;

            internal System.Action<string> OnClick { get; set; }

            public SRServerAddress(string server)
                : this(server, server)
            {
            }

            public SRServerAddress(string server, string name)
            {
                this.server = server;
                this.name = name;
            }

            internal string PropertyName => Name;

            [SROptions.DisplayName("PropertyName", true)]
            [Category("Server")]
            public void Connect()
            {
                OnClick?.Invoke(Server);
            }
        }
    }
}