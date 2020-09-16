using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Heyworks.PocketShooter.Core;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Networking;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Service;
using Heyworks.PocketShooter.Singleton;
using Heyworks.PocketShooter.UI;
using Heyworks.PocketShooter.UI.Core;
using Microsoft.Extensions.Logging;
using UnityEngine;
using Zenject;

namespace Heyworks.PocketShooter.Modules.Analytics
{
    public class AnalyticsManager : Singleton<AnalyticsManager>
    {
        private const string AnalyticaLastLevelSendedKey = "AnalyticaLastLevelSendedKey";
        private const string AppLaunchCountKey = "AppLaunchCountKey";

        [Inject]
        private Main main;

        [Inject]
        private RealtimeRunBehavior runBehavior;

        [Inject]
        private IConfigurationsProvider configurationsProvider;

        private Room room;

        private Player player;

        private IRttProvider RttProvider => runBehavior._RoomController.CurrentRoom.Connection;

        private bool isBattle;

        private float averageFps;
        private int fpsCounter;
        private int[] fpsGroups = new int[6];

        private float averagePing;
        private int pingCounter;
        private int[] pingGroups = new int[7];

        private DateTime startMatchMakingTime;
        private bool isStartMatchmaking;

        private int countBattleInSession;
        private bool isAnalyticsDisabled;

        private CharacterManager characterManager;
        private BotManager botManager;
        private Dictionary<string, object> playerBattleFinishParameters;

        private void OnEnable()
        {
            main.GameStateReset += MainGameStateReset;
        }

        private void OnDisable()
        {
            main.GameStateReset -= MainGameStateReset;
        }

        private void Update()
        {
            if (isBattle)
            {
                float fps = 1 / Time.smoothDeltaTime;
                fpsGroups[Math.Min((int)(fps / 10f), fpsGroups.Length - 1)]++;
                fpsCounter++;
                averageFps += (fps - averageFps) / fpsCounter;

                float ping = RttProvider.LastRoundTripTimeMs;
                pingCounter++;
                averagePing += (ping - averagePing) / pingCounter;

                if (ping < 100)
                {
                    pingGroups[0]++;
                }
                else if (ping < 200)
                {
                    pingGroups[1]++;
                }
                else if (ping < 300)
                {
                    pingGroups[2]++;
                }
                else if (ping < 500)
                {
                    pingGroups[3]++;
                }
                else if (ping < 1000)
                {
                    pingGroups[4]++;
                }
                else if (ping < 2000)
                {
                    pingGroups[5]++;
                }
                else
                {
                    pingGroups[6]++;
                }
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                if (isBattle)
                {
                    SavePlayerBattleFinishData(FinishBattleReason.Fold_to_background);
                    SendPlayerBattleFinish();
                }

                SendAppClosed();
            }
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void UpdateProfile()
        {
            var profile = new YandexAppMetricaUserProfile()
             .Apply(YandexAppMetricaAttribute.CustomString(AnalyticsKeys.ParametrsKeys.ProfileID).WithValue(player.Id.ToString()))
             .Apply(YandexAppMetricaAttribute.CustomBoolean(AnalyticsKeys.ParametrsKeys.Payer).WithValue(player.IAPCount > 0))
             .Apply(YandexAppMetricaAttribute.CustomNumber(AnalyticsKeys.ParametrsKeys.PaidTotal).WithValue(player.IAPTotalUSD))
             .Apply(YandexAppMetricaAttribute.CustomNumber(AnalyticsKeys.ParametrsKeys.HardCurrencyBalance).WithValue(player.Gold))
             .Apply(YandexAppMetricaAttribute.CustomNumber(AnalyticsKeys.ParametrsKeys.SoftCurrencyBalance).WithValue(player.Cash))
             .Apply(YandexAppMetricaAttribute.CustomString(AnalyticsKeys.ParametrsKeys.RegDate).WithValue(player.RegisteredAt.ToLongDateString() + player.RegisteredAt.ToShortTimeString()))
             .Apply(YandexAppMetricaAttribute.CustomNumber(AnalyticsKeys.ParametrsKeys.Battles).WithValue(player.MatchesCount))
             .Apply(YandexAppMetricaAttribute.CustomBoolean(AnalyticsKeys.ParametrsKeys.TestAccount).WithValue(player.IsAnalyticsDisabled));

            AppMetrica.Instance.ReportUserProfile(profile);

            isAnalyticsDisabled = false;// player.IsAnalyticsDisabled;

            if (PlayerPrefs.GetInt(AnalyticaLastLevelSendedKey) < player.Level)
            {
                SendLevelUp(player.Level);
            }
        }

        public void LogStartMatch()
        {
            room = runBehavior._RoomController.CurrentRoom;
            room.Connection.OnDisconnected += Connection_OnDisconnected;
            room.Connection.OnDisconnectedByServer += Connection_OnDisconnectedByServer;

            isBattle = true;

            countBattleInSession++;

            Array.Clear(fpsGroups, 0, fpsGroups.Length);
            fpsCounter = 0;
            averageFps = 0;

            Array.Clear(pingGroups, 0, pingGroups.Length);
            pingCounter = 0;
            averagePing = 0;
        }

        public void SendMatchPerfomance(MapNames mapName)
        {
            if (isBattle)
            {
                Dictionary<string, object> parameters = CreatePlayerParameters();

                parameters.Add(AnalyticsKeys.ParametrsKeys.AverageFPS, averageFps);
                parameters.Add(AnalyticsKeys.ParametrsKeys.Group_0_10_FPS, Math.Round((float)fpsGroups[0] / fpsCounter * 100f, 2));
                parameters.Add(AnalyticsKeys.ParametrsKeys.Group_10_20_FPS, Math.Round((float)fpsGroups[1] / fpsCounter * 100f, 2));
                parameters.Add(AnalyticsKeys.ParametrsKeys.Group_20_30_FPS, Math.Round((float)fpsGroups[2] / fpsCounter * 100f, 2));
                parameters.Add(AnalyticsKeys.ParametrsKeys.Group_30_40_FPS, Math.Round((float)fpsGroups[3] / fpsCounter * 100f, 2));
                parameters.Add(AnalyticsKeys.ParametrsKeys.Group_40_50_FPS, Math.Round((float)fpsGroups[4] / fpsCounter * 100f, 2));
                parameters.Add(AnalyticsKeys.ParametrsKeys.Group_50_60_FPS, Math.Round((float)fpsGroups[5] / fpsCounter * 100f, 2));
                parameters.Add(AnalyticsKeys.ParametrsKeys.AveragePing, averagePing);

                parameters.Add(AnalyticsKeys.ParametrsKeys.Group_0_100_Ping, Math.Round((float)pingGroups[0] / pingCounter * 100f, 2));
                parameters.Add(AnalyticsKeys.ParametrsKeys.Group_100_200_Ping, Math.Round((float)pingGroups[1] / pingCounter * 100f, 2));
                parameters.Add(AnalyticsKeys.ParametrsKeys.Group_200_300_Ping, Math.Round((float)pingGroups[2] / pingCounter * 100f, 2));
                parameters.Add(AnalyticsKeys.ParametrsKeys.Group_300_500_Ping, Math.Round((float)pingGroups[3] / pingCounter * 100f, 2));
                parameters.Add(AnalyticsKeys.ParametrsKeys.Group_500_1000_Ping, Math.Round((float)pingGroups[4] / pingCounter * 100f, 2));
                parameters.Add(AnalyticsKeys.ParametrsKeys.Group_1000_2000_Ping, Math.Round((float)pingGroups[5] / pingCounter * 100f, 2));
                parameters.Add(AnalyticsKeys.ParametrsKeys.Group_2000_INFINITY_Ping, Math.Round((float)pingGroups[6] / pingCounter * 100f, 2));

                parameters.Add(AnalyticsKeys.ParametrsKeys.Map, mapName.ToString());

                ReportEvent(AnalyticsKeys.EventNames.BattlePerformance, parameters);

                isBattle = false;

                room.Connection.OnDisconnected -= Connection_OnDisconnected;
                room.Connection.OnDisconnectedByServer -= Connection_OnDisconnectedByServer;
            }
        }

        public void SetupBattleManagers(CharacterManager characterManager, BotManager botManager)
        {
            this.characterManager = characterManager;
            this.botManager = botManager;
        }

        public void SavePlayerBattleFinishData(FinishBattleReason finishBattleReason, bool isDraw = false, bool isWinner = false)
        {
            if (characterManager == null || botManager == null)
            {
                AnalyticsLog.Log.LogError("Setup characterManager and botManager");
            }
            else
            {
                playerBattleFinishParameters = CreatePlayerParameters();

                if (finishBattleReason == FinishBattleReason.Normal)
                {
                    bool isMatchFinishedByPoints = room.GameTime > 0;
                    MatchResult matchResult = isDraw ? MatchResult.Draw : (isWinner ? MatchResult.Winner : MatchResult.Loser);

                    playerBattleFinishParameters.Add(AnalyticsKeys.ParametrsKeys.Result, matchResult.ToString());
                    playerBattleFinishParameters.Add(AnalyticsKeys.ParametrsKeys.ResultCondition, (isMatchFinishedByPoints ? ResultMatchCondition.Points : ResultMatchCondition.Timer).ToString());
                }

                var playerTroopers = main.MetaGame.Army.Troopers;

                List<string> localCharacterUsedTrooperClassesNames = new List<string>();
                List<int> localCharacterUsedTrooperClassesLevels = new List<int>();

                foreach (TrooperClass usedtrooperClass in characterManager.LocalCharacterTrooperClasses)
                {
                    Trooper usedTrooper = playerTroopers.FirstOrDefault(_ => _.Class == usedtrooperClass);

                    localCharacterUsedTrooperClassesNames.Add(usedtrooperClass.ToString());
                    localCharacterUsedTrooperClassesLevels.Add(usedTrooper.Level);
                }

                //playerBattleFinishParameters.Add("battle_id", "");
                playerBattleFinishParameters.Add(AnalyticsKeys.ParametrsKeys.GameMode, main.DefaultMatchType.ToString());
                playerBattleFinishParameters.Add(AnalyticsKeys.ParametrsKeys.Duration, Constants.ToSeconds(room.GameTime));
                playerBattleFinishParameters.Add(AnalyticsKeys.ParametrsKeys.Finish_reason, finishBattleReason.ToString());
                playerBattleFinishParameters.Add(AnalyticsKeys.ParametrsKeys.Map, main.DefaultMatchMap.ToString());
                playerBattleFinishParameters.Add(AnalyticsKeys.ParametrsKeys.TroopersList, localCharacterUsedTrooperClassesNames);
                playerBattleFinishParameters.Add(AnalyticsKeys.ParametrsKeys.TroopersLevels, localCharacterUsedTrooperClassesLevels);
                playerBattleFinishParameters.Add(AnalyticsKeys.ParametrsKeys.AIUnits, botManager.Bots.Count());
            }
        }

        public void SendPlayerBattleFinish(MatchResultsData matchResultsData = null)
        {
            if (playerBattleFinishParameters == null)
            {
                playerBattleFinishParameters = CreatePlayerParameters();
            }

            if (matchResultsData != null)
            {
                playerBattleFinishParameters.Add(AnalyticsKeys.ParametrsKeys.SoftReward, matchResultsData.Reward.Cash);
                playerBattleFinishParameters.Add(AnalyticsKeys.ParametrsKeys.HardReward, matchResultsData.Reward.Gold);
                playerBattleFinishParameters.Add(AnalyticsKeys.ParametrsKeys.Kills, matchResultsData.Stats.Kills);
                playerBattleFinishParameters.Add(AnalyticsKeys.ParametrsKeys.Death, matchResultsData.Stats.Deaths);
                playerBattleFinishParameters.Add(AnalyticsKeys.ParametrsKeys.MVP, matchResultsData.Stats.IsMVP);
            }

            ReportEvent(AnalyticsKeys.EventNames.PlayerBattleFinish, playerBattleFinishParameters);
        }

        public void StartMatchmaking()
        {
            isStartMatchmaking = true;
            startMatchMakingTime = DateTime.Now;
        }

        public void EndMatchmaking(MatchType matchType, MatchmakingResult result)
        {
            if (isStartMatchmaking)
            {
                int timeInMatchmaking = (DateTime.Now - startMatchMakingTime).Seconds;

                Dictionary<string, object> parameters = CreatePlayerParameters();
                parameters.Add(AnalyticsKeys.ParametrsKeys.WaitTime, timeInMatchmaking);
                parameters.Add(AnalyticsKeys.ParametrsKeys.GameMode, matchType.ToString());
                parameters.Add(AnalyticsKeys.ParametrsKeys.Result, result.ToString());
                //parameters.Add(AnalyticsKeys.ParametrsKeys.MatchmakingGroup, "");

                ReportEvent(AnalyticsKeys.EventNames.Matchmaking, parameters);
            }
            else
            {
                AnalyticsLog.Log.LogWarning("Didn't call StartMatchmaking");
            }

            isStartMatchmaking = false;
        }

        public void SendLevelUp(int level)
        {
            Dictionary<string, object> parameters = CreatePlayerParameters();

            PlayerPrefs.SetInt(AnalyticaLastLevelSendedKey, level);

            ReportEvent(AnalyticsKeys.EventNames.UserLevelUp, parameters);
        }

        public void SendOfferPopupActions(string offerName, OfferAction offerAction)
        {
            Dictionary<string, object> parameters = CreatePlayerParameters();

            parameters.Add(AnalyticsKeys.ParametrsKeys.Name, offerName);
            parameters.Add(AnalyticsKeys.ParametrsKeys.Button, offerAction.ToString());

            ReportEvent(AnalyticsKeys.EventNames.Offer, parameters);
        }

        public void SendOpenNewTrooperWithPriceUSD(TrooperClass trooperClass, int trooperLevel, string trooperRank, double priceUSD)
        {
            SendOpenNewTrooper(trooperClass, trooperLevel, trooperRank, "USD", priceUSD);
        }

        public void SendOpenNewTrooperWithLocalPrice(TrooperClass trooperClass, int trooperLevel, string trooperRank, Price price)
        {
            int priceValue = 0;
            if (price.Type == PriceType.Gold)
            {
                priceValue = price.GoldAmount;
            }
            else if (price.Type == PriceType.Cash)
            {
                priceValue = price.CashAmount;
            }

            SendOpenNewTrooper(trooperClass, trooperLevel, trooperRank, price.Type.ToString(), priceValue);
        }

        public void SendLogin(bool isSuccess)
        {
            Dictionary<string, object> parameters = CreatePlayerParameters();
            parameters.Add(AnalyticsKeys.ParametrsKeys.Success, isSuccess);

            ReportEvent(AnalyticsKeys.EventNames.Login, parameters);
        }

        public void SendOpenNewTrooper(TrooperClass trooperClass, int trooperLevel, string trooperRank, string currency, double price)
        {
            Dictionary<string, object> parameters = CreatePlayerParameters();

            parameters.Add(AnalyticsKeys.ParametrsKeys.TrooperName, trooperClass.ToString());
            parameters.Add(AnalyticsKeys.ParametrsKeys.TrooperRank, trooperRank);
            parameters.Add(AnalyticsKeys.ParametrsKeys.TrooperLevel, trooperLevel);
            parameters.Add(AnalyticsKeys.ParametrsKeys.Currency_name, currency);
            parameters.Add(AnalyticsKeys.ParametrsKeys.Value, price);
            parameters.Add(AnalyticsKeys.ParametrsKeys.PurchaseSource, ScreensController.Instance.CurrentView.GetType().Name);

            ReportEvent(AnalyticsKeys.EventNames.TrooperOpen, parameters);
        }

        public void LogRevenue(UnityEngine.Purchasing.Product product)
        {
            InAppPurchase inAppPurchase = configurationsProvider.ShopConfiguration.GetPurchase(product.definition.id);
            double productPrice = inAppPurchase != null ? Convert.ToDouble(inAppPurchase.PriceUSD) : 0;

            var yandexAppMetricaRevenue = new YandexAppMetricaRevenue(productPrice, "USD")
            {
                ProductID = product.definition.id,
                Quantity = 1,
            };

            if (product.hasReceipt)
            {
                YandexAppMetricaReceipt yaReceipt = default;
                Receipt receipt = JsonUtility.FromJson<Receipt>(product.receipt);
#if UNITY_ANDROID
            PayloadAndroid payloadAndroid = JsonUtility.FromJson<PayloadAndroid>(receipt.Payload);
            yaReceipt.Signature = payloadAndroid.Signature;
            yaReceipt.Data = payloadAndroid.Json;
#elif UNITY_IPHONE
            yaReceipt.TransactionID = receipt.TransactionID;
            yaReceipt.Data = receipt.Payload;
#endif
                yandexAppMetricaRevenue.Receipt = yaReceipt;
            }

            if (!isAnalyticsDisabled)
            {
                AppMetrica.Instance.ReportRevenue(yandexAppMetricaRevenue);

                AnalyticsLog.Log.LogInformation("ReportRevenue " + yandexAppMetricaRevenue.ProductID + ", price: " + yandexAppMetricaRevenue.Price.ToString());
            }
        }

        #region Shop

        public void SendCurrencyIncomingForBattle(PlayerReward reward)
        {
            Dictionary<string, object> parameters = CreatePlayerParameters();

            parameters.Add(AnalyticsKeys.ParametrsKeys.ID, "Battle");
            parameters.Add(AnalyticsKeys.ParametrsKeys.Tr_Type, 1);
            parameters.Add(AnalyticsKeys.ParametrsKeys.ShopCategory, CurrencyIncomingCategory.Battle.ToString());
            parameters.Add(AnalyticsKeys.ParametrsKeys.Screen, ScreensController.Instance.CurrentScreen.GetType().Name);

            if (reward.Cash > 0)
            {
                SendCurrencyShopEvent(parameters, AnalyticsKeys.EventNames.SoftCurrency, reward.Cash);
            }

            if (reward.Gold > 0)
            {
                SendCurrencyShopEvent(parameters, AnalyticsKeys.EventNames.HardCurrency, reward.Gold);
            }
        }

        public void SendCurrencyIncomingForLevelUp(IEnumerable<IContentIdentity> content)
        {
            SendCurrencyIncoming("LevelUp", CurrencyIncomingCategory.Level_up.ToString(), content);
        }

        public void SendCurrencyIncomingFromShop(string id, IEnumerable<IContentIdentity> content, IReadOnlyList<ShopCategory> category)
        {
            ShopProductCategory shopCategory = GetShopProductCategoryByContent(content, category);

            SendCurrencyIncoming(id, shopCategory.ToString(), content);
        }

        public void SendInAppPurchase(string id, double price, IEnumerable<IContentIdentity> content, IReadOnlyList<ShopCategory> category, string localCurrency, double localPrice, string transactionId)
        {
            ShopProductCategory shopCategory = GetShopProductCategoryByContent(content, category);

            SendInAppPurchase(id, price, shopCategory, localCurrency, localPrice, transactionId);
        }

        public void SendCharecterInAppPurchase(string id, double price, string localCurrency, double localPrice, string transactionId)
        {
            SendInAppPurchase(id, price, ShopProductCategory.Buy_character, localCurrency, localPrice, transactionId);
        }

        public void SendInGamePurchase(string id, Price price, IEnumerable<IContentIdentity> content, IReadOnlyList<ShopCategory> category)
        {
            Dictionary<string, object> parameters = CreatePlayerParameters();

            ShopProductCategory shopCategory = GetShopProductCategoryByContent(content, category);

            parameters.Add(AnalyticsKeys.ParametrsKeys.ID, id);
            parameters.Add(AnalyticsKeys.ParametrsKeys.Tr_Type, 0);
            parameters.Add(AnalyticsKeys.ParametrsKeys.ShopCategory, shopCategory.ToString());
            parameters.Add(AnalyticsKeys.ParametrsKeys.Screen, ScreensController.Instance.CurrentScreen.GetType().Name);

            if (price.Type == PriceType.Cash)
            {
                SendCurrencyShopEvent(parameters, AnalyticsKeys.EventNames.SoftCurrency, -price.CashAmount);
            }
            else if (price.Type == PriceType.Gold)
            {
                SendCurrencyShopEvent(parameters, AnalyticsKeys.EventNames.HardCurrency, -price.GoldAmount);
            }
        }

        public void SendBuyingLevelUp(IArmyItem armyItem, bool isInstantBuying)
        {
            ShopUpgradeCategory shopUpgradeCategory = ShopUpgradeCategory.Other;
            switch (armyItem)
            {
                case IArmyTrooperItem trooperItem:
                    shopUpgradeCategory = ShopUpgradeCategory.Lvl_character;
                    break;
                case IArmyArmorItem armorItem:
                    shopUpgradeCategory = ShopUpgradeCategory.Lvl_armor;
                    break;
                case IArmyHelmetItem helmetItem:
                    shopUpgradeCategory = ShopUpgradeCategory.Lvl_helmet;
                    break;
                case IArmyWeaponItem weaponItem:
                    shopUpgradeCategory = ShopUpgradeCategory.Lvl_weapon;
                    break;
            }

            Price price = isInstantBuying ? armyItem.InstantLevelUpPrice : armyItem.RegularLevelUpPrice;

            SendBuyingUpgrade(armyItem.ItemName, price, shopUpgradeCategory);
        }

        public void SendBuyingEvolution(IArmyItem armyItem)
        {
            ShopUpgradeCategory shopUpgradeCategory = ShopUpgradeCategory.Other;
            switch (armyItem)
            {
                case IArmyTrooperItem trooperItem:
                    shopUpgradeCategory = ShopUpgradeCategory.Evo_character;
                    break;
                case IArmyArmorItem armorItem:
                    shopUpgradeCategory = ShopUpgradeCategory.Evo_armor;
                    break;
                case IArmyHelmetItem helmetItem:
                    shopUpgradeCategory = ShopUpgradeCategory.Evo_helmet;
                    break;
                case IArmyWeaponItem weaponItem:
                    shopUpgradeCategory = ShopUpgradeCategory.Evo_weapon;
                    break;
            }

            SendBuyingUpgrade(armyItem.ItemName, armyItem.InstantGradeUpPrice, shopUpgradeCategory);
        }

        #endregion

        #region Private

        private void ReportEvent(string message, Dictionary<string, object> parametrs)
        {
            if (!isAnalyticsDisabled)
            {
                AppMetrica.Instance.ReportEvent(message, parametrs);

                AnalyticsLog.Log.LogInformation("ReportEvent " + message);

                foreach (KeyValuePair<string, object> kvp in parametrs)
                {
                    AnalyticsLog.Log.LogInformation(kvp.Key + " : " + kvp.Value);
                }
            }
        }

        private Dictionary<string, object> CreatePlayerParameters()
        {
            if (player != null)
            {
                return new Dictionary<string, object>
                {
                    { AnalyticsKeys.ParametrsKeys.ProfileID, player.Id.ToString() },
                    { AnalyticsKeys.ParametrsKeys.RegDate, player.RegisteredAt.ToShortTimeString() },
                    { AnalyticsKeys.ParametrsKeys.TestAccount, player.IsAnalyticsDisabled },
                    { AnalyticsKeys.ParametrsKeys.AccountLevel, player.Level },
                    { AnalyticsKeys.ParametrsKeys.Battles, player.MatchesCount },
                    { AnalyticsKeys.ParametrsKeys.Payer, player.IAPCount > 0 },
                    { AnalyticsKeys.ParametrsKeys.PaidCount, player.IAPCount },
                    { AnalyticsKeys.ParametrsKeys.PaidTotal, player.IAPTotalUSD },
                };
            }

            return new Dictionary<string, object>();
        }

        private void SendAppLaunch()
        {
            int appLaunchCount = PlayerPrefs.GetInt(AppLaunchCountKey);

            Dictionary<string, object> parameters = CreatePlayerParameters();
            parameters.Add(AnalyticsKeys.ParametrsKeys.Success, appLaunchCount);

            ReportEvent(AnalyticsKeys.EventNames.AppLaunch, parameters);
        }

        private void SendAppClosed()
        {
            int appLaunchCount = PlayerPrefs.GetInt(AppLaunchCountKey);

            Dictionary<string, object> parameters = CreatePlayerParameters();
            parameters.Add(AnalyticsKeys.ParametrsKeys.Location, (isBattle ? AppClosingLocation.Battle : AppClosingLocation.Meta).ToString());
            parameters.Add(AnalyticsKeys.ParametrsKeys.SessionNumber, appLaunchCount);
            parameters.Add(AnalyticsKeys.ParametrsKeys.SessionBattles, countBattleInSession);

            ReportEvent(AnalyticsKeys.EventNames.AppClosed, parameters);
        }

        private void SendCurrencyIncoming(string id, string category, IEnumerable<IContentIdentity> content)
        {
            int cash = 0;
            int gold = 0;
            foreach (var contentIdentity in content)
            {
                if (contentIdentity is ResourceIdentity resource)
                {
                    cash += resource.Cash;
                    gold += resource.Gold;
                }
            }

            Dictionary<string, object> parameters = CreatePlayerParameters();

            parameters.Add(AnalyticsKeys.ParametrsKeys.ID, id);
            parameters.Add(AnalyticsKeys.ParametrsKeys.Tr_Type, 1);
            parameters.Add(AnalyticsKeys.ParametrsKeys.ShopCategory, category);
            parameters.Add(AnalyticsKeys.ParametrsKeys.Screen, ScreensController.Instance.CurrentScreen.GetType().Name);

            if (cash > 0)
            {
                SendCurrencyShopEvent(parameters, AnalyticsKeys.EventNames.SoftCurrency, cash);
            }

            if (gold > 0)
            {
                SendCurrencyShopEvent(parameters, AnalyticsKeys.EventNames.HardCurrency, gold);
            }
        }

        private void SendBuyingUpgrade(string id, Price price, ShopUpgradeCategory shopUpgradeCategory)
        {
            Dictionary<string, object> parameters = CreatePlayerParameters();

            parameters.Add(AnalyticsKeys.ParametrsKeys.ID, id);
            parameters.Add(AnalyticsKeys.ParametrsKeys.Tr_Type, 0);
            parameters.Add(AnalyticsKeys.ParametrsKeys.ShopCategory, shopUpgradeCategory.ToString());
            parameters.Add(AnalyticsKeys.ParametrsKeys.Screen, ScreensController.Instance.CurrentScreen.GetType().Name);

            if (price.Type == PriceType.Cash)
            {
                SendCurrencyShopEvent(parameters, AnalyticsKeys.EventNames.SoftCurrency, -price.CashAmount);
            }
            else if (price.Type == PriceType.Gold)
            {
                SendCurrencyShopEvent(parameters, AnalyticsKeys.EventNames.HardCurrency, -price.GoldAmount);
            }
        }

        private void SendCurrencyShopEvent(Dictionary<string, object> parameters, string eventCurrencyName, int money)
        {
            parameters.Add(AnalyticsKeys.ParametrsKeys.Value, money);

            ReportEvent(eventCurrencyName, parameters);

            parameters.Remove(AnalyticsKeys.ParametrsKeys.Value);
        }

        private void SendInAppPurchase(string id, double price, ShopProductCategory shopProductCategory, string localCurrency, double localPrice, string transactionId)
        {
            Dictionary<string, object> parameters = CreatePlayerParameters();
            parameters.Add(AnalyticsKeys.ParametrsKeys.InAppID, id);
            parameters.Add(AnalyticsKeys.ParametrsKeys.TransactionId, transactionId);
            parameters.Add(AnalyticsKeys.ParametrsKeys.DollarPrice, price);
            parameters.Add(AnalyticsKeys.ParametrsKeys.LocalCurrency, localCurrency);
            parameters.Add(AnalyticsKeys.ParametrsKeys.LocalPrice, localPrice);
            parameters.Add(AnalyticsKeys.ParametrsKeys.ShopCategory, shopProductCategory.ToString());

            ReportEvent(AnalyticsKeys.EventNames.PaymentSucceed, parameters);
        }

        private ShopProductCategory GetShopProductCategoryByContent(IEnumerable<IContentIdentity> content, IReadOnlyList<ShopCategory> category)
        {
            // TODO: v.fillipov Remake with new purchases !!!!

            ShopProductCategory shopCategory = ShopProductCategory.Others;

            if (category.Contains(ShopCategory.Offers))
            {
                shopCategory = ShopProductCategory.Offers;
            }
            else 
            {
                switch (content.FirstOrDefault())
                {
                    case TrooperIdentity trooper:
                        shopCategory = ShopProductCategory.Buy_character;

                        break;
                    case WeaponIdentity weapon:
                        shopCategory = ShopProductCategory.Buy_weapon;

                        break;
                    case HelmetIdentity helmet:
                        shopCategory = ShopProductCategory.Buy_helmet;

                        break;
                    case ArmorIdentity armor:
                        shopCategory = ShopProductCategory.Buy_armor;

                        break;
                    case ResourceIdentity resource:
                        if (resource.Gold > 0 && resource.Cash <= 0)
                        {
                            shopCategory = ShopProductCategory.Hard_bundle;
                        }
                        else if (resource.Gold <= 0 && resource.Cash > 0)
                        {
                            shopCategory = ShopProductCategory.Soft_bundle;
                        }
                        else if (resource.Gold > 0 && resource.Cash > 0)
                        {
                            shopCategory = ShopProductCategory.Hard_Cash_bundle;
                        }

                        break;
                    default:
                        AnalyticsLog.Log.LogError("Emulating loading purchase list.");
                        break;
                }
            }

            return shopCategory;
        }

        private void MainGameStateReset(MetaGame game)
        {
            player = game.Player;

            UpdateProfile();

            int appLaunchCount = PlayerPrefs.GetInt(AppLaunchCountKey);
            PlayerPrefs.SetInt(AppLaunchCountKey, appLaunchCount + 1);

            SendAppLaunch();
        }

        private void Connection_OnDisconnected()
        {
            if (runBehavior._RoomController != null &&
                !runBehavior._RoomController.IsGameEnded)
            {
                SavePlayerBattleFinishData(FinishBattleReason.Leave);
                SendPlayerBattleFinish();
            }
        }

        private void Connection_OnDisconnectedByServer()
        {
            if (room != null)
            {
                SavePlayerBattleFinishData(FinishBattleReason.Disconnect);
                SendPlayerBattleFinish();
            }
        }

        #endregion

        // Declaration of the Receipt structure for getting
        // information about the IAP.
        [Serializable]
        public struct Receipt
        {
            public string Store;
            public string TransactionID;
            public string Payload;
        }

        // Additional information about the IAP for Android.
        [Serializable]
        public struct PayloadAndroid
        {
            public string Json;
            public string Signature;
        }

        public enum OfferAction
        {
            ToShop,
            Close,
        }

        public enum MatchmakingResult
        {
            Matched,
            Disconnected,
            Left,
        }

        private enum ShopProductCategory
        {
            Offers,
            Hard_bundle,
            Soft_bundle,
            Hard_Cash_bundle,
            Buy_character,
            Buy_weapon,
            Buy_helmet,
            Buy_armor,
            Others,
        }

        private enum ShopUpgradeCategory
        {
            Lvl_character,
            Evo_character,
            Lvl_weapon,
            Evo_weapon,
            Lvl_armor,
            Evo_armor,
            Lvl_helmet,
            Evo_helmet,
            Other,
        }

        private enum CurrencyIncomingCategory
        {
            Battle,
            Level_up,
            Other,
        }

        private enum MatchResult
        {
            Draw,
            Winner,
            Loser,
        }

        public enum FinishBattleReason
        {
            Fold_to_background,
            Leave,
            Disconnect,
            Normal,
        }

        private enum AppClosingLocation
        {
            Battle,
            Meta,
        }

        private enum ResultMatchCondition
        {
            Points,
            Timer,
        }
    }
}